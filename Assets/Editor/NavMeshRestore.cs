// NavMeshRestore.cs  -  Unity 2018.2.19f1 editor tool, self-contained (drop this one file into Assets/Editor).
//
// Rebuilds each level's navmesh from the Unity 3.5 "NavMesh" assets (class 194) that the AssetRipper export
// left in Assets/NavMesh/. Unity 2018 cannot read those assets, so every scene's NavMeshSettings ended up with
// m_NavMeshData: {fileID: 0} and the enemy AI has nothing to path on.
//
// What it does, per level:
//   1. parses the old asset's m_MeshData blob (a Detour "MSET" v1 tile set with "DNAV" v13 tiles),
//   2. builds one Mesh per nav area out of the old polygons (the detail mesh of each polygon, or a fan
//      triangulation of the polygon - see SourceFromDetailMesh),
//   3. bakes a Unity 2018 NavMeshData from those meshes with UnityEngine.AI.NavMeshBuilder, keeping the old
//      per-polygon area id (0 Default, 2 Jump, 3 Flyer, 4 WalkOnlyWhenSpawn) as the Unity area index,
//   4. saves it as Assets/<level>/NavMesh.asset and assigns it to the scene's NavMeshSettings,
//   5. reopens the saved scene and verifies it: triangulation counts, bounds, and NavMesh.CalculatePath between
//      pairs of Cover positions, compared with the connectivity of the old polygon graph.
// A report is written to <project>/NavMeshRestore.txt.
//
// Menu:   Tools/Navigation/Restore navmesh for open scene
//         Tools/Navigation/Restore navmesh for all levels
// Batch:  Unity.exe -batchmode -quit -projectPath "<project>" -executeMethod NavMeshRestore.Run -logFile <log>
//         (optional: -navmeshSource detail|fan, -navmeshVoxel <metres>, -navmeshClimb <metres>, to change the bake without recompiling)
//
// Why the bake settings differ from the scene's (radius 0.5, height 2, climb 0.4, slope 45):
//   - the old polygons are already eroded by the original agent radius, so the rebuild uses a tiny radius
//     (0.05) and a 0.05 m voxel; anything bigger would shrink corridors a second time;
//   - the only geometry fed to the bake is the old walkable surface, there are no walls or ceilings, so the
//     agent height only matters where one navmesh floor lies above another. Several levels have floors
//     stacked 0.95-2.2 m apart (level_03, level_02, level_08, level_11_dlc, level_12_dlc); an agent height of 2
//     would delete the lower floor, so 0.5 is used;
//   - slope 60 instead of 45 because the input is already the walkable set; filtering it again can only
//     punch holes.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public static class NavMeshRestore
{
	// ------------------------------------------------------------------ configuration

	const float AgentRadius = 0.05f;
	const float AgentHeight = 0.5f;
	static float AgentClimb = 0.4f; // overridable at batch time with -navmeshClimb <metres>
	const float AgentSlope = 60f;
	const float MinRegionArea = 0.5f;
	static float VoxelSize = 0.05f; // overridable at batch time with -navmeshVoxel <metres>
	const int TileSizeVoxels = 256;
	const float BoundsMargin = 2f;
	const int MaxPathPairs = 300;
	const float SampleDistance = 2f;      // NavMesh.SamplePosition radius for the path test
	const float OldMeshSnapDistance = 2.5f; // how far a Cover may be from the old polygons to count as "on" them

	// true: triangles come from the old detail mesh (exact floor heights, what the old agents stood on).
	// false: fan-triangulate each polygon (flat polygons, heights only at the corners).
	// Overridable at batch time with -navmeshSource detail|fan.
	static bool SourceFromDetailMesh = true;

	// level scene -> old asset (from the pristine 3.5 scenes' NavMeshSettings.m_NavMesh references)
	static readonly string[][] LevelAssets =
	{
		new[] { "level_01", "NavMesh_12" }, new[] { "level_02", "NavMesh_10" }, new[] { "level_03", "NavMesh_9" },
		new[] { "level_04", "NavMesh_11" }, new[] { "level_05", "NavMesh_5" }, new[] { "level_06", "NavMesh_6" },
		new[] { "level_07", "NavMesh_1" }, new[] { "level_08", "NavMesh_2" }, new[] { "level_09", "NavMesh_7" },
		new[] { "level_10", "NavMesh_4" }, new[] { "level_11_dlc", "NavMesh_3" }, new[] { "level_12_dlc", "NavMesh_8" },
		new[] { "level_13_dlc", "NavMesh_0" }, new[] { "level_14_dlc", "NavMesh" },
	};

	static readonly string[] AreaNames = { "Default", "Not Walkable", "Jump", "Flyer", "WalkOnlyWhenSpawn" };

	const string OldAssetFolder = "Assets/NavMesh";
	const string ReportFileName = "NavMeshRestore.txt";
	const string BackupRoot = "D:/unity/shadowgun/backups/scenes_before_navmesh";

	// ------------------------------------------------------------------ Detour data (Unity 3.5 = Detour tile set "MSET" v1, tiles "DNAV" v13)

	class DtPoly
	{
		public uint firstLink;
		public int[] verts = new int[6];
		public int[] neis = new int[6];
		public int flags;
		public int vertCount;
		public int area;
	}

	struct DtLink
	{
		public uint reference;
		public uint next;
		public byte edge, side, bmin, bmax;
	}

	struct DtDetail
	{
		public int vertBase, triBase, vertCount, triCount;
	}

	class DtTile
	{
		public uint tileRef;
		public int x, y, layer;
		public int polyCount, vertCount, maxLinkCount, detailMeshCount, detailVertCount, detailTriCount, bvNodeCount, offMeshConCount, offMeshBase;
		public float walkableHeight, walkableRadius, walkableClimb;
		public Vector3 bmin, bmax;
		public Vector3[] verts;
		public DtPoly[] polys;
		public DtLink[] links;
		public DtDetail[] details;
		public Vector3[] detailVerts;
		public int[] detailTris; // 4 per triangle (a, b, c, flags)
		public int polyBase;     // index of this tile's first polygon in the whole-mesh numbering
	}

	class DtNavMesh
	{
		public string assetPath;
		public Vector3 orig;
		public float tileWidth, tileHeight;
		public int maxTiles, maxPolys;
		public int tileBits, polyBits;
		public List<DtTile> tiles = new List<DtTile>();
		public int TotalPolys { get { int n = 0; foreach (DtTile t in tiles) n += t.polyCount; return n; } }
		public int TotalVerts { get { int n = 0; foreach (DtTile t in tiles) n += t.vertCount; return n; } }
		public int TotalDetailVerts { get { int n = 0; foreach (DtTile t in tiles) n += t.detailVertCount; return n; } }
		public int TotalDetailTris { get { int n = 0; foreach (DtTile t in tiles) n += t.detailTriCount; return n; } }
		public Bounds Bounds
		{
			get
			{
				Bounds b = new Bounds(tiles[0].bmin, Vector3.zero);
				foreach (DtTile t in tiles) { b.Encapsulate(t.bmin); b.Encapsulate(t.bmax); }
				return b;
			}
		}
	}

	class ByteReader
	{
		readonly byte[] b;
		public int pos;
		public ByteReader(byte[] bytes, int start) { b = bytes; pos = start; }
		public int Remaining { get { return b.Length - pos; } }
		public byte U8() { return b[pos++]; }
		public ushort U16() { ushort v = (ushort)(b[pos] | (b[pos + 1] << 8)); pos += 2; return v; }
		public int I32() { int v = BitConverter.ToInt32(b, pos); pos += 4; return v; }
		public uint U32() { uint v = BitConverter.ToUInt32(b, pos); pos += 4; return v; }
		public float F32() { float v = BitConverter.ToSingle(b, pos); pos += 4; return v; }
		public Vector3 V3() { return new Vector3(F32(), F32(), F32()); }
	}

	static byte[] ReadMeshDataBlob(string assetPath)
	{
		string text = File.ReadAllText(assetPath);
		Match m = Regex.Match(text, @"m_MeshData:\s*([0-9a-fA-F]+)");
		if (!m.Success) throw new Exception("no m_MeshData hex blob in " + assetPath);
		string hex = m.Groups[1].Value;
		if ((hex.Length & 1) != 0) throw new Exception("odd hex length in " + assetPath);
		byte[] bytes = new byte[hex.Length / 2];
		for (int i = 0; i < bytes.Length; i++)
			bytes[i] = (byte)((HexVal(hex[2 * i]) << 4) | HexVal(hex[2 * i + 1]));
		return bytes;
	}

	static int HexVal(char c)
	{
		if (c >= '0' && c <= '9') return c - '0';
		if (c >= 'a' && c <= 'f') return c - 'a' + 10;
		if (c >= 'A' && c <= 'F') return c - 'A' + 10;
		throw new Exception("bad hex char " + c);
	}

	const uint MSET_MAGIC = ('M' << 24) | ('S' << 16) | ('E' << 8) | 'T';
	const uint DNAV_MAGIC = ('D' << 24) | ('N' << 16) | ('A' << 8) | 'V';
	const uint DT_NULL_LINK = 0xffffffff;
	const int DT_EXT_LINK = 0x8000;

	static int Ilog2(uint v) { int r = 0; while ((v >>= 1) != 0) r++; return r; }
	static uint NextPow2(uint v) { v--; v |= v >> 1; v |= v >> 2; v |= v >> 4; v |= v >> 8; v |= v >> 16; v++; return v; }

	// Layout, confirmed byte for byte on all 14 assets (every tile is consumed exactly, all vertices lie
	// inside the tile bounds, all polygon / detail indices are in range):
	//   dtNavMeshSetHeader: magic 'MSET', version 1, numTiles, params { orig[3], tileWidth, tileHeight, maxTiles, maxPolys }
	//   per tile: tileRef u32, dataSize i32, then the tile:
	//   dtMeshHeader (104 bytes): magic 'DNAV', version 13, x, y, layer, userId, polyCount, vertCount, maxLinkCount,
	//     detailMeshCount, detailVertCount, detailTriCount, bvNodeCount, offMeshConCount, offMeshBase, <one reserved int>,
	//     walkableHeight, walkableRadius, walkableClimb, bmin[3], bmax[3], bvQuantFactor
	//   verts      float[3]            x vertCount
	//   polys      36 bytes            x polyCount   { firstLink u32, verts u16[6], neis u16[6], flags u16, pad u16, vertCount u8, area u8, pad u16 }
	//   links      12 bytes            x maxLinkCount { ref u32, next u32, edge u8, side u8, bmin u8, bmax u8 }
	//   details    12 bytes            x detailMeshCount { vertBase u32, triBase u32, vertCount u8, triCount u8, pad u16 }
	//   detailVerts float[3]           x detailVertCount
	//   detailTris u16[4]              x detailTriCount
	//   bvTree     16 bytes            x bvNodeCount
	//   offMeshCons                    x offMeshConCount (none in this game)
	static DtNavMesh ParseDetourAsset(string assetPath)
	{
		byte[] data = ReadMeshDataBlob(assetPath);
		ByteReader r = new ByteReader(data, 0);
		DtNavMesh nm = new DtNavMesh();
		nm.assetPath = assetPath;
		uint magic = r.U32();
		int version = r.I32();
		int numTiles = r.I32();
		if (magic != MSET_MAGIC) throw new Exception(string.Format("{0}: bad set magic 0x{1:x8}", assetPath, magic));
		if (version != 1) throw new Exception(string.Format("{0}: unexpected set version {1}", assetPath, version));
		nm.orig = r.V3();
		nm.tileWidth = r.F32();
		nm.tileHeight = r.F32();
		nm.maxTiles = r.I32();
		nm.maxPolys = r.I32();
		nm.tileBits = Ilog2(NextPow2((uint)nm.maxTiles));
		nm.polyBits = Ilog2(NextPow2((uint)nm.maxPolys));

		int polyBase = 0;
		for (int ti = 0; ti < numTiles; ti++)
		{
			DtTile t = new DtTile();
			t.tileRef = r.U32();
			int dataSize = r.I32();
			int tileStart = r.pos;
			if (dataSize <= 0 || dataSize > r.Remaining) throw new Exception(string.Format("{0}: tile {1} dataSize {2} out of range", assetPath, ti, dataSize));

			uint tmagic = r.U32();
			int tversion = r.I32();
			if (tmagic != DNAV_MAGIC) throw new Exception(string.Format("{0}: tile {1} bad magic 0x{2:x8}", assetPath, ti, tmagic));
			if (tversion != 13) throw new Exception(string.Format("{0}: tile {1} unexpected version {2} (expected 13)", assetPath, ti, tversion));
			t.x = r.I32(); t.y = r.I32(); t.layer = r.I32(); r.I32(); // userId
			t.polyCount = r.I32(); t.vertCount = r.I32(); t.maxLinkCount = r.I32();
			t.detailMeshCount = r.I32(); t.detailVertCount = r.I32(); t.detailTriCount = r.I32();
			t.bvNodeCount = r.I32(); t.offMeshConCount = r.I32(); t.offMeshBase = r.I32();
			r.I32(); // reserved / unused in this version
			t.walkableHeight = r.F32(); t.walkableRadius = r.F32(); t.walkableClimb = r.F32();
			t.bmin = r.V3(); t.bmax = r.V3();
			r.F32(); // bvQuantFactor
			if (r.pos - tileStart != 104) throw new Exception("header size mismatch");

			t.verts = new Vector3[t.vertCount];
			for (int i = 0; i < t.vertCount; i++)
			{
				Vector3 v = r.V3();
				t.verts[i] = v;
				const float eps = 0.01f;
				if (v.x < t.bmin.x - eps || v.y < t.bmin.y - eps || v.z < t.bmin.z - eps || v.x > t.bmax.x + eps || v.y > t.bmax.y + eps || v.z > t.bmax.z + eps)
					throw new Exception(string.Format("{0}: tile {1} vertex {2} {3} outside tile bounds {4}..{5}; header layout is off", assetPath, ti, i, v, t.bmin, t.bmax));
			}

			t.polys = new DtPoly[t.polyCount];
			for (int i = 0; i < t.polyCount; i++)
			{
				DtPoly p = new DtPoly();
				p.firstLink = r.U32();
				for (int k = 0; k < 6; k++) p.verts[k] = r.U16();
				for (int k = 0; k < 6; k++) p.neis[k] = r.U16();
				p.flags = r.U16();
				r.U16();
				p.vertCount = r.U8();
				p.area = r.U8() & 0x3f;
				r.U16();
				if (p.vertCount < 3 || p.vertCount > 6) throw new Exception(string.Format("{0}: tile {1} poly {2} vertCount {3}; polygon layout is off", assetPath, ti, i, p.vertCount));
				for (int k = 0; k < p.vertCount; k++)
					if (p.verts[k] >= t.vertCount) throw new Exception(string.Format("{0}: tile {1} poly {2} vertex index {3} >= {4}", assetPath, ti, i, p.verts[k], t.vertCount));
				t.polys[i] = p;
			}

			t.links = new DtLink[t.maxLinkCount];
			for (int i = 0; i < t.maxLinkCount; i++)
			{
				DtLink l;
				l.reference = r.U32(); l.next = r.U32();
				l.edge = r.U8(); l.side = r.U8(); l.bmin = r.U8(); l.bmax = r.U8();
				t.links[i] = l;
			}

			t.details = new DtDetail[t.detailMeshCount];
			for (int i = 0; i < t.detailMeshCount; i++)
			{
				DtDetail d;
				d.vertBase = (int)r.U32(); d.triBase = (int)r.U32();
				d.vertCount = r.U8(); d.triCount = r.U8();
				r.U16();
				if (d.vertBase + d.vertCount > t.detailVertCount || d.triBase + d.triCount > t.detailTriCount)
					throw new Exception(string.Format("{0}: tile {1} detail mesh {2} out of range", assetPath, ti, i));
				t.details[i] = d;
			}

			t.detailVerts = new Vector3[t.detailVertCount];
			for (int i = 0; i < t.detailVertCount; i++) t.detailVerts[i] = r.V3();

			t.detailTris = new int[t.detailTriCount * 4];
			for (int i = 0; i < t.detailTriCount * 4; i++) t.detailTris[i] = r.U16();

			r.pos += t.bvNodeCount * 16;
			if (t.offMeshConCount != 0)
				Debug.LogWarning(string.Format("{0}: tile {1} has {2} off-mesh connections; they are not restored", assetPath, ti, t.offMeshConCount));
			else if (r.pos - tileStart != dataSize)
				throw new Exception(string.Format("{0}: tile {1} consumed {2} of {3} bytes; section layout is off", assetPath, ti, r.pos - tileStart, dataSize));
			r.pos = tileStart + dataSize;

			t.polyBase = polyBase;
			polyBase += t.polyCount;
			nm.tiles.Add(t);
		}
		if (nm.tiles.Count == 0) throw new Exception(assetPath + ": no tiles");
		return nm;
	}

	// ------------------------------------------------------------------ old polygon graph (for the path baseline)

	static int TileIndexFromRef(DtNavMesh nm, uint reference)
	{
		uint tileMask = (uint)((1 << nm.tileBits) - 1);
		uint it = (reference >> nm.polyBits) & tileMask;
		for (int i = 0; i < nm.tiles.Count; i++)
			if (((nm.tiles[i].tileRef >> nm.polyBits) & tileMask) == it) return i;
		return -1;
	}

	static int[] OldComponents(DtNavMesh nm, out int componentCount)
	{
		int n = nm.TotalPolys;
		int[] parent = new int[n];
		for (int i = 0; i < n; i++) parent[i] = i;
		uint polyMask = (uint)((1 << nm.polyBits) - 1);
		foreach (DtTile t in nm.tiles)
		{
			for (int pi = 0; pi < t.polyCount; pi++)
			{
				DtPoly p = t.polys[pi];
				// internal neighbours (neis = neighbour index + 1, 0 = border, DT_EXT_LINK bit = goes to another tile)
				for (int k = 0; k < p.vertCount; k++)
				{
					int nei = p.neis[k];
					if (nei == 0 || (nei & DT_EXT_LINK) != 0) continue;
					if (nei - 1 < t.polyCount) Union(parent, t.polyBase + pi, t.polyBase + nei - 1);
				}
				// link list covers both internal and cross-tile connections
				uint li = p.firstLink;
				int guard = 0;
				while (li != DT_NULL_LINK && li < (uint)t.links.Length && guard++ < 100000)
				{
					DtLink l = t.links[li];
					int ot = TileIndexFromRef(nm, l.reference);
					int op = (int)(l.reference & polyMask);
					if (ot >= 0 && op < nm.tiles[ot].polyCount) Union(parent, t.polyBase + pi, nm.tiles[ot].polyBase + op);
					li = l.next;
				}
			}
		}
		int[] comp = new int[n];
		Dictionary<int, int> ids = new Dictionary<int, int>();
		for (int i = 0; i < n; i++)
		{
			int root = Find(parent, i);
			int id;
			if (!ids.TryGetValue(root, out id)) { id = ids.Count; ids[root] = id; }
			comp[i] = id;
		}
		componentCount = ids.Count;
		return comp;
	}

	static int Find(int[] parent, int i) { while (parent[i] != i) { parent[i] = parent[parent[i]]; i = parent[i]; } return i; }
	static void Union(int[] parent, int a, int b) { a = Find(parent, a); b = Find(parent, b); if (a != b) parent[a] = b; }

	// nearest old polygon to a point (xz distance to the polygon, 0 when inside; vertical window of 3 m)
	static int NearestOldPoly(DtNavMesh nm, Vector3 p, float maxDist)
	{
		int best = -1;
		float bestD = maxDist;
		foreach (DtTile t in nm.tiles)
		{
			for (int pi = 0; pi < t.polyCount; pi++)
			{
				DtPoly poly = t.polys[pi];
				float cy = 0;
				for (int k = 0; k < poly.vertCount; k++) cy += t.verts[poly.verts[k]].y;
				cy /= poly.vertCount;
				if (Mathf.Abs(cy - p.y) > 3f) continue;
				float d = DistanceXZToPoly(t, poly, p);
				// prefer a polygon the point is inside; otherwise the closest one
				if (d < bestD || (d == bestD && d == 0f && Mathf.Abs(cy - p.y) < 1f))
				{
					bestD = d;
					best = t.polyBase + pi;
				}
			}
		}
		return best;
	}

	static float DistanceXZToPoly(DtTile t, DtPoly poly, Vector3 p)
	{
		bool inside = false;
		float minEdge = float.MaxValue;
		int n = poly.vertCount;
		for (int i = 0, j = n - 1; i < n; j = i++)
		{
			Vector3 vi = t.verts[poly.verts[i]], vj = t.verts[poly.verts[j]];
			if (((vi.z > p.z) != (vj.z > p.z)) && (p.x < (vj.x - vi.x) * (p.z - vi.z) / (vj.z - vi.z) + vi.x)) inside = !inside;
			minEdge = Mathf.Min(minEdge, DistancePointSegmentXZ(p, vi, vj));
		}
		return inside ? 0f : minEdge;
	}

	static float DistancePointSegmentXZ(Vector3 p, Vector3 a, Vector3 b)
	{
		float abx = b.x - a.x, abz = b.z - a.z;
		float apx = p.x - a.x, apz = p.z - a.z;
		float len = abx * abx + abz * abz;
		float tt = len > 0 ? Mathf.Clamp01((apx * abx + apz * abz) / len) : 0f;
		float dx = a.x + tt * abx - p.x, dz = a.z + tt * abz - p.z;
		return Mathf.Sqrt(dx * dx + dz * dz);
	}

	static float PolyAreaXZ(DtTile t, DtPoly poly)
	{
		float a = 0;
		for (int i = 0; i < poly.vertCount; i++)
		{
			Vector3 v1 = t.verts[poly.verts[i]], v2 = t.verts[poly.verts[(i + 1) % poly.vertCount]];
			a += v1.x * v2.z - v2.x * v1.z;
		}
		return Mathf.Abs(a) * 0.5f;
	}

	static float TriAreaXZ(Vector3 a, Vector3 b, Vector3 c)
	{
		return Mathf.Abs((b.x - a.x) * (c.z - a.z) - (c.x - a.x) * (b.z - a.z)) * 0.5f;
	}

	// ------------------------------------------------------------------ meshes per area

	class AreaMesh
	{
		public int area;
		public List<Vector3> verts = new List<Vector3>();
		public List<int> tris = new List<int>();
		public int polyCount;
		public float oldArea;
		public void AddTriangle(Vector3 a, Vector3 b, Vector3 c)
		{
			// Unity front face = clockwise seen from the normal side; cross(b-a, c-a).y > 0 means the face points up.
			Vector3 n = Vector3.Cross(b - a, c - a);
			if (n.sqrMagnitude < 1e-12f) return;
			if (n.y < 0) { Vector3 tmp = b; b = c; c = tmp; }
			int i = verts.Count;
			verts.Add(a); verts.Add(b); verts.Add(c);
			tris.Add(i); tris.Add(i + 1); tris.Add(i + 2);
		}
	}

	static Dictionary<int, AreaMesh> BuildAreaMeshes(DtNavMesh nm, bool useDetail)
	{
		Dictionary<int, AreaMesh> result = new Dictionary<int, AreaMesh>();
		foreach (DtTile t in nm.tiles)
		{
			for (int pi = 0; pi < t.polyCount; pi++)
			{
				DtPoly p = t.polys[pi];
				AreaMesh am;
				if (!result.TryGetValue(p.area, out am)) { am = new AreaMesh(); am.area = p.area; result[p.area] = am; }
				am.polyCount++;
				am.oldArea += PolyAreaXZ(t, p);
				bool done = false;
				if (useDetail && pi < t.details.Length)
				{
					DtDetail d = t.details[pi];
					if (d.triCount > 0)
					{
						for (int k = 0; k < d.triCount; k++)
						{
							int tb = (d.triBase + k) * 4;
							Vector3 a = DetailVert(t, p, d, t.detailTris[tb]);
							Vector3 b = DetailVert(t, p, d, t.detailTris[tb + 1]);
							Vector3 c = DetailVert(t, p, d, t.detailTris[tb + 2]);
							am.AddTriangle(a, b, c);
						}
						done = true;
					}
				}
				if (!done)
				{
					Vector3 v0 = t.verts[p.verts[0]];
					for (int k = 1; k + 1 < p.vertCount; k++)
						am.AddTriangle(v0, t.verts[p.verts[k]], t.verts[p.verts[k + 1]]);
				}
			}
		}
		return result;
	}

	static Vector3 DetailVert(DtTile t, DtPoly p, DtDetail d, int idx)
	{
		if (idx < p.vertCount) return t.verts[p.verts[idx]];
		return t.detailVerts[d.vertBase + idx - p.vertCount];
	}

	// ------------------------------------------------------------------ per-level work

	class LevelResult
	{
		public string level, oldAsset, newAsset;
		public StringBuilder text = new StringBuilder();
		public bool ok;
	}

	static NavMeshBuildSettings MakeSettings()
	{
		NavMeshBuildSettings s = NavMesh.GetSettingsByID(0);
		s.agentRadius = AgentRadius;
		s.agentHeight = AgentHeight;
		s.agentClimb = AgentClimb;
		s.agentSlope = AgentSlope;
		s.minRegionArea = MinRegionArea;
		s.overrideVoxelSize = true;
		s.voxelSize = VoxelSize;
		s.overrideTileSize = true;
		s.tileSize = TileSizeVoxels;
		return s;
	}

	static string ProjectRoot { get { return Path.GetDirectoryName(Application.dataPath).Replace('\\', '/'); } }

	static void BackupOnce(string projectRelativePath)
	{
		try
		{
			string src = ProjectRoot + "/" + projectRelativePath;
			if (!File.Exists(src)) return;
			string dst = BackupRoot + "/" + projectRelativePath.Replace("Assets/", "");
			if (File.Exists(dst)) return; // keep the first (oldest) backup
			Directory.CreateDirectory(Path.GetDirectoryName(dst));
			File.Copy(src, dst);
		}
		catch (Exception e)
		{
			Debug.LogWarning("NavMeshRestore: backup of " + projectRelativePath + " failed: " + e.Message);
		}
	}

	static string AreaLabel(int area)
	{
		string name = area >= 0 && area < AreaNames.Length ? AreaNames[area] : "?";
		string unityName = SafeAreaName(area);
		return area + " " + name + (unityName != "" && unityName != name ? " (project: " + unityName + ")" : "");
	}

	static string SafeAreaName(int area)
	{
		try
		{
			string[] names = GameObjectUtility.GetNavMeshAreaNames();
			foreach (string n in names) if (NavMesh.GetAreaFromName(n) == area) return n;
		}
		catch { }
		return "";
	}

	static LevelResult RestoreLevel(string level, string oldAssetName, bool progress, int index, int total)
	{
		LevelResult res = new LevelResult();
		res.level = level;
		res.oldAsset = OldAssetFolder + "/" + oldAssetName + ".asset";
		res.newAsset = "Assets/" + level + "/NavMesh.asset";
		StringBuilder sb = res.text;
		sb.AppendLine("== " + level + "  (old: " + res.oldAsset + ")");
		string scenePath = "Assets/" + level + ".unity";
		if (!File.Exists(ProjectRoot + "/" + scenePath)) { sb.AppendLine("ERROR: scene missing: " + scenePath); return res; }
		if (!File.Exists(ProjectRoot + "/" + res.oldAsset)) { sb.AppendLine("ERROR: old navmesh asset missing: " + res.oldAsset); return res; }

		if (progress) EditorUtility.DisplayProgressBar("NavMesh restore", level + ": parsing " + oldAssetName, (index + 0.1f) / total);

		// 1. parse
		DtNavMesh nm = ParseDetourAsset(res.oldAsset);
		Bounds oldBounds = nm.Bounds;
		sb.AppendLine(string.Format("old: tiles={0} verts={1} polys={2} detailVerts={3} detailTris={4} tileSize={5}x{6} m",
			nm.tiles.Count, nm.TotalVerts, nm.TotalPolys, nm.TotalDetailVerts, nm.TotalDetailTris, nm.tileWidth, nm.tileHeight));
		sb.AppendLine(string.Format("old bounds: min=({0:F2}, {1:F2}, {2:F2}) max=({3:F2}, {4:F2}, {5:F2})",
			oldBounds.min.x, oldBounds.min.y, oldBounds.min.z, oldBounds.max.x, oldBounds.max.y, oldBounds.max.z));
		foreach (DtTile t in nm.tiles)
			sb.AppendLine(string.Format("  tile ({0},{1}) polys={2} verts={3} header walkableHeight={4:F2} walkableRadius={5:F2} walkableClimb={6:F2}", t.x, t.y, t.polyCount, t.vertCount, t.walkableHeight, t.walkableRadius, t.walkableClimb));

		// 2. meshes per area
		Dictionary<int, AreaMesh> areaMeshes = BuildAreaMeshes(nm, SourceFromDetailMesh);
		List<int> areaIds = new List<int>(areaMeshes.Keys);
		areaIds.Sort();
		StringBuilder oldAreas = new StringBuilder("old per area: ");
		float oldTotalArea = 0;
		foreach (int a in areaIds)
		{
			AreaMesh am = areaMeshes[a];
			oldTotalArea += am.oldArea;
			oldAreas.AppendFormat("[{0}] {1} polys {2:F1} m2 ({3} src tris)  ", AreaLabel(a), am.polyCount, am.oldArea, am.tris.Count / 3);
		}
		sb.AppendLine(oldAreas.ToString().TrimEnd());
		int oldComponentCount;
		int[] oldComp = OldComponents(nm, out oldComponentCount);
		int[] compSizes = new int[oldComponentCount];
		foreach (int c in oldComp) compSizes[c]++;
		Array.Sort(compSizes); Array.Reverse(compSizes);
		StringBuilder cs = new StringBuilder();
		for (int i = 0; i < compSizes.Length && i < 8; i++) cs.Append(compSizes[i]).Append(i + 1 < compSizes.Length && i < 7 ? ", " : "");
		sb.AppendLine(string.Format("old connectivity: {0} island(s), polygon counts: {1}{2}", oldComponentCount, cs, compSizes.Length > 8 ? ", ..." : ""));

		// open the scene (needed for the NavMeshSettings object, the Cover positions and the save)
		if (progress) EditorUtility.DisplayProgressBar("NavMesh restore", level + ": opening scene", (index + 0.2f) / total);
		Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

		// 3. bake
		if (progress) EditorUtility.DisplayProgressBar("NavMesh restore", level + ": baking", (index + 0.3f) / total);
		List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
		List<Mesh> tempMeshes = new List<Mesh>();
		foreach (int a in areaIds)
		{
			AreaMesh am = areaMeshes[a];
			Mesh mesh = new Mesh();
			mesh.name = level + "_area" + a;
			if (am.verts.Count > 65000) mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
			mesh.SetVertices(am.verts);
			mesh.SetTriangles(am.tris, 0);
			mesh.RecalculateBounds();
			tempMeshes.Add(mesh);
			NavMeshBuildSource src = new NavMeshBuildSource();
			src.shape = NavMeshBuildSourceShape.Mesh;
			src.sourceObject = mesh;
			src.transform = Matrix4x4.identity;
			src.area = a;
			sources.Add(src);
		}
		Bounds bakeBounds = oldBounds;
		bakeBounds.Expand(new Vector3(BoundsMargin * 2, BoundsMargin * 2, BoundsMargin * 2));
		NavMeshBuildSettings settings = MakeSettings();
		string[] validation = settings.ValidationReport(bakeBounds);
		sb.AppendLine(string.Format("bake settings: agentTypeID={0} radius={1} height={2} climb={3} slope={4} voxel={5} tile={6} minRegionArea={7} source={8}",
			settings.agentTypeID, settings.agentRadius, settings.agentHeight, settings.agentClimb, settings.agentSlope, settings.voxelSize, settings.tileSize, settings.minRegionArea, SourceFromDetailMesh ? "detail mesh" : "polygon fan"));
		if (validation != null && validation.Length > 0) foreach (string v in validation) sb.AppendLine("  validation: " + v);
		System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
		NavMeshData data = NavMeshBuilder.BuildNavMeshData(settings, sources, bakeBounds, Vector3.zero, Quaternion.identity);
		sw.Stop();
		foreach (Mesh m in tempMeshes) Object.DestroyImmediate(m);
		if (data == null) { sb.AppendLine("ERROR: BuildNavMeshData returned null"); return res; }
		data.name = "NavMesh";
		sb.AppendLine(string.Format("bake: {0:F1} s, sourceBounds min=({1:F2}, {2:F2}, {3:F2}) max=({4:F2}, {5:F2}, {6:F2})", sw.Elapsed.TotalSeconds,
			data.sourceBounds.min.x, data.sourceBounds.min.y, data.sourceBounds.min.z, data.sourceBounds.max.x, data.sourceBounds.max.y, data.sourceBounds.max.z));

		// 4. save the asset
		if (progress) EditorUtility.DisplayProgressBar("NavMesh restore", level + ": saving asset", (index + 0.6f) / total);
		if (!AssetDatabase.IsValidFolder("Assets/" + level)) AssetDatabase.CreateFolder("Assets", level);
		BackupOnce(res.newAsset);
		NavMeshData existing = AssetDatabase.LoadAssetAtPath<NavMeshData>(res.newAsset);
		if (existing != null) AssetDatabase.DeleteAsset(res.newAsset);
		AssetDatabase.CreateAsset(data, res.newAsset);
		AssetDatabase.SaveAssets();
		string guid = AssetDatabase.AssetPathToGUID(res.newAsset);
		sb.AppendLine("asset: " + res.newAsset + " guid=" + guid);

		// 5. assign to the scene and save it
		if (progress) EditorUtility.DisplayProgressBar("NavMesh restore", level + ": saving scene", (index + 0.7f) / total);
		Object settingsObject = UnityEditor.AI.NavMeshBuilder.navMeshSettingsObject;
		bool assigned = false;
		if (settingsObject == null) sb.AppendLine("ERROR: NavMeshBuilder.navMeshSettingsObject is null");
		else
		{
			SerializedObject so = new SerializedObject(settingsObject);
			SerializedProperty prop = so.FindProperty("m_NavMeshData");
			if (prop == null) sb.AppendLine("ERROR: NavMeshSettings has no m_NavMeshData property");
			else
			{
				prop.objectReferenceValue = data;
				so.ApplyModifiedPropertiesWithoutUndo();
				assigned = true;
			}
		}
		bool saved = false;
		if (assigned)
		{
			BackupOnce(scenePath);
			EditorSceneManager.MarkSceneDirty(scene);
			saved = EditorSceneManager.SaveScene(scene);
		}
		sb.AppendLine("scene: reference assigned=" + (assigned ? "yes" : "NO") + " saved=" + (saved ? "yes" : "NO"));
		if (!saved) return res;

		// 6. verify from disk: reopen, triangulate, path test
		if (progress) EditorUtility.DisplayProgressBar("NavMesh restore", level + ": verifying", (index + 0.8f) / total);
		scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
		NavMeshTriangulation tri = NavMesh.CalculateTriangulation();
		bool loadedFromScene = tri.indices != null && tri.indices.Length > 0;
		NavMeshDataInstance tempInstance = new NavMeshDataInstance();
		if (!loadedFromScene)
		{
			// should not happen; fall back to an explicit instance so the path test still runs
			NavMeshData reloaded = AssetDatabase.LoadAssetAtPath<NavMeshData>(res.newAsset);
			if (reloaded != null) tempInstance = NavMesh.AddNavMeshData(reloaded);
			tri = NavMesh.CalculateTriangulation();
			sb.AppendLine("WARNING: the reopened scene had no navmesh loaded; tested through NavMesh.AddNavMeshData instead");
		}
		int newTris = tri.indices != null ? tri.indices.Length / 3 : 0;
		int newVerts = tri.vertices != null ? tri.vertices.Length : 0;
		Bounds newBounds = new Bounds();
		if (newVerts > 0)
		{
			newBounds = new Bounds(tri.vertices[0], Vector3.zero);
			foreach (Vector3 v in tri.vertices) newBounds.Encapsulate(v);
		}
		Dictionary<int, int> triPerArea = new Dictionary<int, int>();
		Dictionary<int, float> areaPerArea = new Dictionary<int, float>();
		float newTotalArea = 0;
		for (int i = 0; i < newTris; i++)
		{
			int a = tri.areas[i];
			float ar = TriAreaXZ(tri.vertices[tri.indices[3 * i]], tri.vertices[tri.indices[3 * i + 1]], tri.vertices[tri.indices[3 * i + 2]]);
			newTotalArea += ar;
			int c; triPerArea.TryGetValue(a, out c); triPerArea[a] = c + 1;
			float f; areaPerArea.TryGetValue(a, out f); areaPerArea[a] = f + ar;
		}
		sb.AppendLine(string.Format("new: verts={0} tris={1} bounds min=({2:F2}, {3:F2}, {4:F2}) max=({5:F2}, {6:F2}, {7:F2}) (navmesh loaded from the saved scene: {8})",
			newVerts, newTris, newBounds.min.x, newBounds.min.y, newBounds.min.z, newBounds.max.x, newBounds.max.y, newBounds.max.z, loadedFromScene ? "yes" : "no"));
		List<int> newAreaIds = new List<int>(triPerArea.Keys);
		foreach (int a in areaIds) if (!newAreaIds.Contains(a)) newAreaIds.Add(a);
		newAreaIds.Sort();
		StringBuilder na = new StringBuilder("new per area: ");
		foreach (int a in newAreaIds)
		{
			int c; triPerArea.TryGetValue(a, out c);
			float f; areaPerArea.TryGetValue(a, out f);
			float oldA = areaMeshes.ContainsKey(a) ? areaMeshes[a].oldArea : 0f;
			na.AppendFormat("[{0}] {1} tris {2:F1} m2{3}  ", AreaLabel(a), c, f, oldA > 0 ? string.Format(" ({0:+0.0;-0.0}% vs old)", (f - oldA) / oldA * 100f) : " (NOT IN OLD MESH)");
		}
		sb.AppendLine(na.ToString().TrimEnd());
		sb.AppendLine(string.Format("walkable area: old {0:F1} m2, new {1:F1} m2 ({2:+0.0;-0.0}%)", oldTotalArea, newTotalArea, oldTotalArea > 0 ? (newTotalArea - oldTotalArea) / oldTotalArea * 100f : 0f));

		CoverageTest(nm, sb);
		PathTest(nm, oldComp, sb);

		if (tempInstance.valid) tempInstance.Remove();
		res.ok = newTris > 0;
		return res;
	}

	// Is there new navmesh under every old polygon? Samples each old polygon's centroid (and, for polygons
	// bigger than a few m2, its corners pulled 0.3 m inwards) against the baked mesh.
	static void CoverageTest(DtNavMesh nm, StringBuilder sb)
	{
		int polys = 0, lost = 0;
		float lostArea = 0;
		StringBuilder list = new StringBuilder();
		foreach (DtTile t in nm.tiles)
		{
			for (int pi = 0; pi < t.polyCount; pi++)
			{
				DtPoly p = t.polys[pi];
				polys++;
				Vector3 c = Vector3.zero;
				for (int k = 0; k < p.vertCount; k++) c += t.verts[p.verts[k]];
				c /= p.vertCount;
				NavMeshHit hit;
				bool found = NavMesh.SamplePosition(c, out hit, 0.5f, NavMesh.AllAreas);
				if (!found)
				{
					// thin polygons: the centroid can sit on the eroded rim, try points pulled towards the centre
					for (int k = 0; k < p.vertCount && !found; k++)
						found = NavMesh.SamplePosition(Vector3.Lerp(t.verts[p.verts[k]], c, 0.5f), out hit, 0.5f, NavMesh.AllAreas);
				}
				if (!found)
				{
					lost++;
					float a = PolyAreaXZ(t, p);
					lostArea += a;
					if (lost <= 10) list.AppendLine(string.Format("  old polygon without new navmesh: tile ({0},{1}) poly {2} area id {3} {4:F1} m2 at ({5:F1}, {6:F1}, {7:F1})", t.x, t.y, pi, p.area, a, c.x, c.y, c.z));
				}
			}
		}
		sb.AppendLine(string.Format("coverage: {0} of {1} old polygons have no new navmesh within 0.5 m of their centre ({2:F1} m2)", lost, polys, lostArea));
		if (list.Length > 0) sb.Append(list);
	}

	static List<Vector3> FindCoverPositions()
	{
		List<Vector3> result = new List<Vector3>();
		List<string> keys = new List<string>();
		Scene active = SceneManager.GetActiveScene();
		foreach (MonoBehaviour mb in Resources.FindObjectsOfTypeAll<MonoBehaviour>())
		{
			if (mb == null || mb.GetType().Name != "Cover") continue;
			if (mb.gameObject.scene != active) continue;
			if ((mb.gameObject.hideFlags & HideFlags.HideInHierarchy) != 0) continue;
			Vector3 p = mb.transform.position;
			keys.Add(string.Format("{0}|{1:F3}|{2:F3}|{3:F3}", mb.gameObject.name, p.x, p.y, p.z));
			result.Add(p);
		}
		// deterministic order so a re-run tests the same pairs
		int[] order = new int[result.Count];
		for (int i = 0; i < order.Length; i++) order[i] = i;
		string[] k = keys.ToArray();
		Array.Sort(k, order, StringComparer.Ordinal);
		List<Vector3> sorted = new List<Vector3>(result.Count);
		foreach (int i in order) sorted.Add(result[i]);
		return sorted;
	}

	static void PathTest(DtNavMesh nm, int[] oldComp, StringBuilder sb)
	{
		List<Vector3> covers = FindCoverPositions();
		int n = covers.Count;
		int[] coverComp = new int[n];
		Vector3[] sampled = new Vector3[n];
		bool[] sampledOk = new bool[n];
		int onOld = 0, onNew = 0;
		for (int i = 0; i < n; i++)
		{
			NavMeshHit hit;
			sampledOk[i] = NavMesh.SamplePosition(covers[i], out hit, SampleDistance, NavMesh.AllAreas);
			if (sampledOk[i]) { sampled[i] = hit.position; onNew++; }
			// which old island is this cover on? Looked up at the point the new navmesh snapped to, so both
			// sides of the comparison talk about the same spot. Covers the new mesh could not sample are listed
			// below with their distance to the old polygons instead (the coverage line above says whether any
			// old polygon is actually missing).
			if (NearestOldPoly(nm, covers[i], OldMeshSnapDistance) >= 0) onOld++;
			int poly = sampledOk[i] ? NearestOldPoly(nm, sampled[i], 0.75f) : -1;
			coverComp[i] = poly >= 0 ? oldComp[poly] : -1;
		}
		sb.AppendLine(string.Format("covers: {0} in scene, {1} within {2} m of the old polygons, {3} within {4} m of the new navmesh", n, onOld, OldMeshSnapDistance, onNew, SampleDistance));
		int listed = 0;
		for (int i = 0; i < n && listed < 10; i++)
		{
			if (sampledOk[i]) continue;
			int nearOld = NearestOldPoly(nm, covers[i], OldMeshSnapDistance);
			if (nearOld < 0) continue;
			NavMeshHit far;
			bool farOk = NavMesh.SamplePosition(covers[i], out far, 10f, NavMesh.AllAreas);
			float oldDist = -1f;
			foreach (DtTile t in nm.tiles) if (nearOld >= t.polyBase && nearOld < t.polyBase + t.polyCount) oldDist = DistanceXZToPoly(t, t.polys[nearOld - t.polyBase], covers[i]);
			sb.AppendLine(string.Format("  cover {0} at ({1:F1}, {2:F1}, {3:F1}): {4:F2} m (xz) from the nearest old polygon, nearest new navmesh {5}", i, covers[i].x, covers[i].y, covers[i].z, oldDist,
				farOk ? string.Format("{0:F2} m away at ({1:F1}, {2:F1}, {3:F1})", Vector3.Distance(covers[i], far.position), far.position.x, far.position.y, far.position.z) : "more than 10 m away"));
			listed++;
		}
		long totalPairs = (long)n * (n - 1) / 2;
		if (totalPairs == 0) { sb.AppendLine("paths: no pairs to test"); return; }
		// evenly spread sample of the pairs
		List<int[]> pairs = new List<int[]>();
		double step = totalPairs <= MaxPathPairs ? 1.0 : (double)totalPairs / MaxPathPairs;
		double next = 0;
		long idx = 0;
		for (int i = 0; i < n && pairs.Count < MaxPathPairs; i++)
			for (int j = i + 1; j < n && pairs.Count < MaxPathPairs; j++, idx++)
				if (idx >= next) { pairs.Add(new[] { i, j }); next += step; }

		int tested = 0, complete = 0, partial = 0, invalid = 0, notSampled = 0;
		int oldConnected = 0, oldConnectedComplete = 0, oldDisconnected = 0, oldDisconnectedComplete = 0, oldUnknown = 0, oldUnknownComplete = 0;
		NavMeshPath path = new NavMeshPath();
		StringBuilder failures = new StringBuilder();
		int failuresListed = 0;
		foreach (int[] pr in pairs)
		{
			int i = pr[0], j = pr[1];
			tested++;
			bool oldKnown = coverComp[i] >= 0 && coverComp[j] >= 0;
			bool oldConn = oldKnown && coverComp[i] == coverComp[j];
			if (!oldKnown) oldUnknown++; else if (oldConn) oldConnected++; else oldDisconnected++;
			if (!sampledOk[i] || !sampledOk[j]) { notSampled++; continue; }
			bool ok = NavMesh.CalculatePath(sampled[i], sampled[j], NavMesh.AllAreas, path) && path.status == NavMeshPathStatus.PathComplete;
			if (ok) complete++;
			else if (path.status == NavMeshPathStatus.PathPartial) partial++;
			else invalid++;
			if (ok)
			{
				if (!oldKnown) oldUnknownComplete++; else if (oldConn) oldConnectedComplete++; else oldDisconnectedComplete++;
			}
			else if (oldConn && failuresListed < 10)
			{
				// where did the partial path stop, and how far from the goal
				Vector3 end = path.corners != null && path.corners.Length > 0 ? path.corners[path.corners.Length - 1] : sampled[i];
				failures.AppendLine(string.Format("  old-connected pair failed: cover {0} ({1:F1}, {2:F1}, {3:F1}) -> cover {4} ({5:F1}, {6:F1}, {7:F1}) status={8} path stops at ({9:F1}, {10:F1}, {11:F1}), {12:F1} m short, dy {13:F2}",
					i, sampled[i].x, sampled[i].y, sampled[i].z, j, sampled[j].x, sampled[j].y, sampled[j].z, path.status, end.x, end.y, end.z, Vector3.Distance(end, sampled[j]), sampled[j].y - end.y));
				failuresListed++;
			}
		}
		sb.AppendLine(string.Format("paths: {0} cover pairs tested (of {1}): complete={2} partial={3} invalid={4} endpoint-not-on-navmesh={5}", tested, totalPairs, complete, partial, invalid, notSampled));
		sb.AppendLine(string.Format("paths vs old mesh: connected in old mesh {0} -> complete now {1} ({2:F1}%); disconnected in old mesh {3} -> complete now {4}; endpoint off old mesh {5} -> complete now {6}",
			oldConnected, oldConnectedComplete, oldConnected > 0 ? 100f * oldConnectedComplete / oldConnected : 0f, oldDisconnected, oldDisconnectedComplete, oldUnknown, oldUnknownComplete));
		if (failures.Length > 0) sb.Append(failures);
	}

	// ------------------------------------------------------------------ drivers

	static string Header()
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendLine("NavMeshRestore report  -  " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  -  Unity " + Application.unityVersion + (Application.isBatchMode ? " (batch)" : " (editor)"));
		sb.AppendLine(string.Format("bake: agentRadius={0} agentHeight={1} agentClimb={2} agentSlope={3} voxelSize={4} tileSize={5} minRegionArea={6} boundsMargin={7} triangles from {8}",
			AgentRadius, AgentHeight, AgentClimb, AgentSlope, VoxelSize, TileSizeVoxels, MinRegionArea, BoundsMargin, SourceFromDetailMesh ? "old detail mesh" : "polygon fans"));
		StringBuilder areas = new StringBuilder("project nav areas (NavMesh.GetAreaFromName): ");
		foreach (string a in AreaNames) areas.AppendFormat("{0}={1} ", a, NavMesh.GetAreaFromName(a));
		sb.AppendLine(areas.ToString().TrimEnd());
		sb.AppendLine("agent types: " + NavMesh.GetSettingsCount() + ", id 0 = " + NavMesh.GetSettingsNameFromID(0));
		sb.AppendLine();
		return sb.ToString();
	}

	static void WriteReport(string text, bool append)
	{
		string path = ProjectRoot + "/" + ReportFileName;
		if (append && File.Exists(path)) File.AppendAllText(path, text, new UTF8Encoding(false));
		else File.WriteAllText(path, text, new UTF8Encoding(false));
		Debug.Log("NavMeshRestore: report written to " + path);
	}

	static string LevelForScene(string scenePath)
	{
		string name = Path.GetFileNameWithoutExtension(scenePath);
		foreach (string[] la in LevelAssets) if (la[0] == name) return la[1];
		return null;
	}

	static void ReadCommandLineOptions()
	{
		string[] args = Environment.GetCommandLineArgs();
		for (int i = 0; i + 1 < args.Length; i++)
		{
			if (args[i] == "-navmeshSource")
			{
				string v = args[i + 1].ToLowerInvariant();
				if (v == "fan") SourceFromDetailMesh = false;
				else if (v == "detail") SourceFromDetailMesh = true;
			}
			if (args[i] == "-navmeshClimb")
			{
				float f;
				if (float.TryParse(args[i + 1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out f) && f > 0.05f && f < 3f) AgentClimb = f;
			}
			if (args[i] == "-navmeshVoxel")
			{
				float f;
				if (float.TryParse(args[i + 1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out f) && f > 0.005f && f < 1f) VoxelSize = f;
			}
		}
	}

	static string RunLevels(List<string[]> levels, bool progress)
	{
		StringBuilder all = new StringBuilder(Header());
		int okCount = 0;
		try
		{
			for (int i = 0; i < levels.Count; i++)
			{
				LevelResult r;
				try
				{
					r = RestoreLevel(levels[i][0], levels[i][1], progress, i, levels.Count);
				}
				catch (Exception e)
				{
					r = new LevelResult();
					r.level = levels[i][0];
					r.text.AppendLine("== " + levels[i][0]);
					r.text.AppendLine("ERROR: " + e.GetType().Name + ": " + e.Message);
					r.text.AppendLine(e.StackTrace);
					Debug.LogException(e);
				}
				if (r.ok) okCount++;
				all.Append(r.text).AppendLine();
				Debug.Log(r.text.ToString());
			}
		}
		finally
		{
			if (progress) EditorUtility.ClearProgressBar();
		}
		all.AppendLine(string.Format("{0} of {1} level(s) restored with a non-empty navmesh.", okCount, levels.Count));
		return all.ToString();
	}

	[MenuItem("Tools/Navigation/Restore navmesh for open scene")]
	public static void RestoreOpenScene()
	{
		Scene scene = SceneManager.GetActiveScene();
		string asset = LevelForScene(scene.path);
		if (asset == null)
		{
			EditorUtility.DisplayDialog("NavMesh restore", "The open scene '" + scene.name + "' is not one of the 14 levels with an old navmesh asset.", "OK");
			return;
		}
		if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
		List<string[]> one = new List<string[]>();
		one.Add(new[] { scene.name, asset });
		string text = RunLevels(one, true);
		WriteReport(text, true);
		EditorUtility.DisplayDialog("NavMesh restore", "Done. See " + ReportFileName + " in the project folder.", "OK");
	}

	[MenuItem("Tools/Navigation/Restore navmesh for all levels")]
	public static void RestoreAllLevels()
	{
		if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
		List<string[]> levels = new List<string[]>(LevelAssets);
		string text = RunLevels(levels, true);
		WriteReport(text, false);
		EditorUtility.DisplayDialog("NavMesh restore", "Done. See " + ReportFileName + " in the project folder.", "OK");
	}

	// Batch entry point:
	//   Unity.exe -batchmode -quit -projectPath "D:/unity/shadowgun/ExportedProject" -executeMethod NavMeshRestore.Run -logFile <log>
	public static void Run()
	{
		// Temp/UnityLockfile is created by whichever editor has the project open. Unity itself refuses to start a
		// second instance on the same project, so this only catches a lock file that predates this process.
		string lockFile = ProjectRoot + "/Temp/UnityLockfile";
		if (File.Exists(lockFile))
		{
			DateTime started = System.Diagnostics.Process.GetCurrentProcess().StartTime;
			if (File.GetLastWriteTime(lockFile) < started.AddSeconds(-10))
			{
				string msg = "NavMeshRestore.Run: Temp/UnityLockfile is older than this process; another editor has the project open. Stopping.";
				Debug.LogError(msg);
				WriteReport(msg + Environment.NewLine, false);
				if (Application.isBatchMode) EditorApplication.Exit(2);
				return;
			}
		}
		ReadCommandLineOptions();
		List<string[]> levels = new List<string[]>(LevelAssets);
		string text = RunLevels(levels, !Application.isBatchMode);
		WriteReport(text, false);
	}
}
