// NavMeshRecoveryCore.cs
// Pure C# (no UnityEngine/UnityEditor references) so it can be tested outside Unity.
// Decodes Unity 3.5-era NavMesh .asset files (text/YAML, Detour tile data) and matches them
// against an original text-serialized .unity scene.
//
// C# 4.0 compatible.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace NavMeshRecovery
{
    public struct V3
    {
        public float x, y, z;
        public V3(float x, float y, float z) { this.x = x; this.y = y; this.z = z; }
    }

    public struct Q4
    {
        public float x, y, z, w;
        public Q4(float x, float y, float z, float w) { this.x = x; this.y = y; this.z = z; this.w = w; }
        public static readonly Q4 Identity = new Q4(0, 0, 0, 1);

        public static Q4 Mul(Q4 a, Q4 b)
        {
            return new Q4(
                a.w * b.x + a.x * b.w + a.y * b.z - a.z * b.y,
                a.w * b.y - a.x * b.z + a.y * b.w + a.z * b.x,
                a.w * b.z + a.x * b.y - a.y * b.x + a.z * b.w,
                a.w * b.w - a.x * b.x - a.y * b.y - a.z * b.z);
        }

        public static V3 Rotate(Q4 q, V3 v)
        {
            Q4 p = Mul(Mul(q, new Q4(v.x, v.y, v.z, 0)), new Q4(-q.x, -q.y, -q.z, q.w));
            return new V3(p.x, p.y, p.z);
        }
    }

    public class NavPoly
    {
        public int[] Verts;
        public int Area;
    }

    public class DecodedNavMesh
    {
        public string AssetPath;
        public string MetaGuid;
        public List<V3> Verts = new List<V3>();
        public List<NavPoly> Polys = new List<NavPoly>();
        public Dictionary<int, int> AreaCounts = new Dictionary<int, int>();
        public float WalkableHeight, WalkableRadius, WalkableClimb;
        public V3 BMin, BMax;
        public string Error;

        public bool IsValid { get { return Error == null && Polys.Count > 0; } }
    }

    public class SceneInfo
    {
        public string Path;
        public string NavMeshGuid;
        public List<V3> NavStaticPoints = new List<V3>();
        public int NavigationLayerObjects; // objects using a non-default 3.5 navmesh layer
    }

    public class MatchResult
    {
        public DecodedNavMesh NavMesh;
        public float Score;          // 0..1 fraction of polys near Navigation Static geometry
        public bool GuidMatch;
    }

    public static class NavMeshDecoder
    {
        const int SetHeaderSize = 40;   // MSET magic, version, numTiles, dtNavMeshParams (28 bytes)
        const int TileHeaderSize = 8;   // tileRef, dataSize
        const int MeshHeaderSize = 104; // Unity 3.5 dtMeshHeader (Detour version 13)
        const int PolySize = 36;        // Unity 3.5 dtPoly (32-bit flags)

        public static bool LooksLikeNavMeshAsset(string path)
        {
            try
            {
                using (StreamReader r = new StreamReader(path))
                {
                    for (int i = 0; i < 12; i++)
                    {
                        string line = r.ReadLine();
                        if (line == null) return false;
                        if (line.StartsWith("NavMesh:")) return true;
                    }
                }
            }
            catch (IOException) { }
            return false;
        }

        public static DecodedNavMesh Decode(string path)
        {
            DecodedNavMesh nav = new DecodedNavMesh();
            nav.AssetPath = path;
            nav.MetaGuid = ReadMetaGuid(path + ".meta");

            string hex = null;
            foreach (string line in File.ReadAllLines(path))
            {
                int idx = line.IndexOf("m_MeshData: ", StringComparison.Ordinal);
                if (idx >= 0) { hex = line.Substring(idx + 12).Trim(); break; }
            }
            if (string.IsNullOrEmpty(hex)) { nav.Error = "No m_MeshData (binary asset, or not a navmesh)"; return nav; }

            byte[] b;
            try { b = HexToBytes(hex); }
            catch (Exception e) { nav.Error = "Bad hex data: " + e.Message; return nav; }

            if (b.Length < SetHeaderSize || b[0] != 'T' || b[1] != 'E' || b[2] != 'S' || b[3] != 'M')
            {
                nav.Error = "Unrecognised navmesh header";
                return nav;
            }

            int numTiles = BitConverter.ToInt32(b, 8);
            int off = SetHeaderSize;
            bool first = true;

            for (int t = 0; t < numTiles; t++)
            {
                if (off + TileHeaderSize + MeshHeaderSize > b.Length) { nav.Error = "Truncated tile " + t; return nav; }
                int dataSize = BitConverter.ToInt32(b, off + 4);
                int d = off + TileHeaderSize;
                off = d + dataSize;
                if (dataSize <= 0) continue;

                if (b[d] != 'V' || b[d + 1] != 'A' || b[d + 2] != 'N' || b[d + 3] != 'D')
                {
                    nav.Error = "Tile " + t + " missing DNAV magic";
                    return nav;
                }
                int version = BitConverter.ToInt32(b, d + 4);
                if (version != 13)
                {
                    nav.Error = "Unsupported Detour tile version " + version + " (decoder written for 13)";
                    return nav;
                }

                int polyCount = BitConverter.ToInt32(b, d + 24);
                int vertCount = BitConverter.ToInt32(b, d + 28);

                if (first)
                {
                    nav.WalkableHeight = BitConverter.ToSingle(b, d + 64);
                    nav.WalkableRadius = BitConverter.ToSingle(b, d + 68);
                    nav.WalkableClimb = BitConverter.ToSingle(b, d + 72);
                    first = false;
                }

                int vo = d + MeshHeaderSize;
                int po = vo + vertCount * 12;
                if (po + polyCount * PolySize > b.Length) { nav.Error = "Tile " + t + " data overruns buffer"; return nav; }

                int vbase = nav.Verts.Count;
                for (int i = 0; i < vertCount; i++)
                {
                    int o = vo + i * 12;
                    nav.Verts.Add(new V3(BitConverter.ToSingle(b, o), BitConverter.ToSingle(b, o + 4), BitConverter.ToSingle(b, o + 8)));
                }

                for (int p = 0; p < polyCount; p++)
                {
                    int o = po + p * PolySize;
                    int vc = b[o + 32];
                    int area = b[o + 33] & 0x3f;
                    if (vc < 3 || vc > 6) { nav.Error = "Bad polygon vertex count in tile " + t + " (layout mismatch?)"; return nav; }

                    int[] vs = new int[vc];
                    for (int k = 0; k < vc; k++)
                    {
                        int vi = BitConverter.ToUInt16(b, o + 4 + k * 2);
                        if (vi >= vertCount) { nav.Error = "Polygon vertex index out of range in tile " + t; return nav; }
                        vs[k] = vbase + vi;
                    }

                    NavPoly poly = new NavPoly();
                    poly.Verts = vs;
                    poly.Area = area;
                    nav.Polys.Add(poly);

                    int c;
                    nav.AreaCounts.TryGetValue(area, out c);
                    nav.AreaCounts[area] = c + 1;
                }
            }

            if (nav.Verts.Count > 0)
            {
                V3 mn = nav.Verts[0], mx = nav.Verts[0];
                foreach (V3 v in nav.Verts)
                {
                    mn.x = Math.Min(mn.x, v.x); mn.y = Math.Min(mn.y, v.y); mn.z = Math.Min(mn.z, v.z);
                    mx.x = Math.Max(mx.x, v.x); mx.y = Math.Max(mx.y, v.y); mx.z = Math.Max(mx.z, v.z);
                }
                nav.BMin = mn; nav.BMax = mx;
            }
            return nav;
        }

        static string ReadMetaGuid(string metaPath)
        {
            if (!File.Exists(metaPath)) return null;
            foreach (string line in File.ReadAllLines(metaPath))
            {
                string l = line.Trim();
                if (l.StartsWith("guid:")) return l.Substring(5).Trim();
            }
            return null;
        }

        static byte[] HexToBytes(string hex)
        {
            byte[] r = new byte[hex.Length / 2];
            for (int i = 0; i < r.Length; i++)
                r[i] = (byte)((HexVal(hex[i * 2]) << 4) | HexVal(hex[i * 2 + 1]));
            return r;
        }

        static int HexVal(char c)
        {
            if (c >= '0' && c <= '9') return c - '0';
            if (c >= 'a' && c <= 'f') return c - 'a' + 10;
            if (c >= 'A' && c <= 'F') return c - 'A' + 10;
            throw new FormatException("Invalid hex character '" + c + "'");
        }
    }

    public static class SceneParser
    {
        const int NavigationStaticFlag = 8; // StaticEditorFlags.NavigationStatic

        static readonly Regex DocHeader = new Regex(@"^--- !u!(\d+) &(-?\d+)", RegexOptions.Compiled);
        static readonly Regex FileIdRx = new Regex(@"fileID: (-?\d+)", RegexOptions.Compiled);
        static readonly Regex GuidRx = new Regex(@"guid: ([0-9a-f]{32})", RegexOptions.Compiled);
        static readonly Regex VecRx = new Regex(@"\{x: ([^,}]+), y: ([^,}]+), z: ([^,}]+)(?:, w: ([^,}]+))?\}", RegexOptions.Compiled);

        class TransformData
        {
            public V3 Pos = new V3(0, 0, 0);
            public Q4 Rot = Q4.Identity;
            public V3 Scale = new V3(1, 1, 1);
            public long Father;
            public long GameObject;
            public bool Resolved;
            public V3 WPos, WScale;
            public Q4 WRot;
        }

        public static SceneInfo Parse(string path)
        {
            SceneInfo info = new SceneInfo();
            info.Path = path;

            Dictionary<long, TransformData> transforms = new Dictionary<long, TransformData>();
            Dictionary<long, int> goFlags = new Dictionary<long, int>();
            HashSet<long> goWithMeshFilter = new HashSet<long>();

            int cls = -1;
            TransformData current = null;
            long currentId = 0;
            bool inNavSettings = false;

            using (StreamReader r = new StreamReader(path))
            {
                string first = r.ReadLine();
                if (first == null || !first.StartsWith("%YAML"))
                    throw new InvalidDataException("Scene is not text/YAML serialized: " + path);

                string line;
                while ((line = r.ReadLine()) != null)
                {
                    if (line.StartsWith("---"))
                    {
                        Match h = DocHeader.Match(line);
                        cls = h.Success ? int.Parse(h.Groups[1].Value) : -1;
                        currentId = h.Success ? long.Parse(h.Groups[2].Value) : 0;
                        current = null;
                        inNavSettings = cls == 196;
                        if (cls == 4)
                        {
                            current = new TransformData();
                            transforms[currentId] = current;
                        }
                        continue;
                    }

                    string t = line.TrimStart();

                    if (inNavSettings && t.StartsWith("m_NavMesh:"))
                    {
                        Match g = GuidRx.Match(t);
                        if (g.Success) info.NavMeshGuid = g.Groups[1].Value;
                    }
                    else if (cls == 1)
                    {
                        if (t.StartsWith("m_StaticEditorFlags:"))
                        {
                            long f;
                            if (long.TryParse(t.Substring(20).Trim(), out f)) goFlags[currentId] = (int)(f & 0xffffffff);
                        }
                        else if (t.StartsWith("m_NavMeshLayer:") && t.Substring(15).Trim() != "0")
                        {
                            info.NavigationLayerObjects++;
                        }
                    }
                    else if (cls == 33 && t.StartsWith("m_GameObject:"))
                    {
                        Match m = FileIdRx.Match(t);
                        if (m.Success) goWithMeshFilter.Add(long.Parse(m.Groups[1].Value));
                    }
                    else if (cls == 4 && current != null)
                    {
                        if (t.StartsWith("m_LocalPosition:")) current.Pos = ParseV3(t, current.Pos);
                        else if (t.StartsWith("m_LocalRotation:")) current.Rot = ParseQ4(t, current.Rot);
                        else if (t.StartsWith("m_LocalScale:")) current.Scale = ParseV3(t, current.Scale);
                        else if (t.StartsWith("m_Father:")) current.Father = ParseFileId(t);
                        else if (t.StartsWith("m_GameObject:")) current.GameObject = ParseFileId(t);
                    }
                }
            }

            foreach (KeyValuePair<long, TransformData> kv in transforms)
            {
                long go = kv.Value.GameObject;
                int flags;
                if (!goFlags.TryGetValue(go, out flags)) continue;
                if ((flags & NavigationStaticFlag) == 0) continue;
                if (!goWithMeshFilter.Contains(go)) continue;
                Resolve(kv.Value, transforms, 0);
                info.NavStaticPoints.Add(kv.Value.WPos);
            }
            return info;
        }

        static void Resolve(TransformData t, Dictionary<long, TransformData> all, int depth)
        {
            if (t.Resolved) return;
            TransformData parent;
            if (t.Father != 0 && depth < 256 && all.TryGetValue(t.Father, out parent))
            {
                Resolve(parent, all, depth + 1);
                V3 scaled = new V3(t.Pos.x * parent.WScale.x, t.Pos.y * parent.WScale.y, t.Pos.z * parent.WScale.z);
                V3 rp = Q4.Rotate(parent.WRot, scaled);
                t.WPos = new V3(parent.WPos.x + rp.x, parent.WPos.y + rp.y, parent.WPos.z + rp.z);
                t.WRot = Q4.Mul(parent.WRot, t.Rot);
                t.WScale = new V3(parent.WScale.x * t.Scale.x, parent.WScale.y * t.Scale.y, parent.WScale.z * t.Scale.z);
            }
            else
            {
                t.WPos = t.Pos; t.WRot = t.Rot; t.WScale = t.Scale;
            }
            t.Resolved = true;
        }

        static long ParseFileId(string s)
        {
            Match m = FileIdRx.Match(s);
            return m.Success ? long.Parse(m.Groups[1].Value) : 0;
        }

        static float F(string s)
        {
            float f;
            return float.TryParse(s.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out f) ? f : 0f;
        }

        static V3 ParseV3(string s, V3 fallback)
        {
            Match m = VecRx.Match(s);
            return m.Success ? new V3(F(m.Groups[1].Value), F(m.Groups[2].Value), F(m.Groups[3].Value)) : fallback;
        }

        static Q4 ParseQ4(string s, Q4 fallback)
        {
            Match m = VecRx.Match(s);
            if (!m.Success || !m.Groups[4].Success) return fallback;
            return new Q4(F(m.Groups[1].Value), F(m.Groups[2].Value), F(m.Groups[3].Value), F(m.Groups[4].Value));
        }
    }

    public static class NavMeshMatcher
    {
        /// Fraction of navmesh polygons whose centroid has a Navigation Static mesh object pivot
        /// within `radius` metres horizontally (and `heightTolerance` vertically).
        public static float Score(DecodedNavMesh nav, SceneInfo scene, float radius, float heightTolerance)
        {
            if (!nav.IsValid || scene.NavStaticPoints.Count == 0) return 0f;

            Dictionary<long, List<V3>> grid = new Dictionary<long, List<V3>>();
            foreach (V3 p in scene.NavStaticPoints)
            {
                long key = Key((int)Math.Floor(p.x / radius), (int)Math.Floor(p.z / radius));
                List<V3> cell;
                if (!grid.TryGetValue(key, out cell)) { cell = new List<V3>(); grid[key] = cell; }
                cell.Add(p);
            }

            float r2 = radius * radius;
            int hits = 0;
            foreach (NavPoly poly in nav.Polys)
            {
                float cx = 0, cy = 0, cz = 0;
                foreach (int vi in poly.Verts) { V3 v = nav.Verts[vi]; cx += v.x; cy += v.y; cz += v.z; }
                cx /= poly.Verts.Length; cy /= poly.Verts.Length; cz /= poly.Verts.Length;

                int gx = (int)Math.Floor(cx / radius), gz = (int)Math.Floor(cz / radius);
                bool found = false;
                for (int dx = -1; dx <= 1 && !found; dx++)
                {
                    for (int dz = -1; dz <= 1 && !found; dz++)
                    {
                        List<V3> cell;
                        if (!grid.TryGetValue(Key(gx + dx, gz + dz), out cell)) continue;
                        foreach (V3 p in cell)
                        {
                            float ex = p.x - cx, ez = p.z - cz;
                            if (ex * ex + ez * ez <= r2 && Math.Abs(p.y - cy) <= heightTolerance) { found = true; break; }
                        }
                    }
                }
                if (found) hits++;
            }
            return (float)hits / nav.Polys.Count;
        }

        static long Key(int x, int z) { return ((long)x << 32) ^ (uint)z; }

        public static List<MatchResult> Rank(SceneInfo scene, List<DecodedNavMesh> navs, float radius, float heightTolerance)
        {
            List<MatchResult> results = new List<MatchResult>();
            foreach (DecodedNavMesh nav in navs)
            {
                if (!nav.IsValid) continue;
                MatchResult r = new MatchResult();
                r.NavMesh = nav;
                r.GuidMatch = scene.NavMeshGuid != null && nav.MetaGuid == scene.NavMeshGuid;
                r.Score = Score(nav, scene, radius, heightTolerance);
                results.Add(r);
            }
            results.Sort(delegate (MatchResult a, MatchResult b)
            {
                if (a.GuidMatch != b.GuidMatch) return a.GuidMatch ? -1 : 1;
                return b.Score.CompareTo(a.Score);
            });
            return results;
        }
    }
}
