// NavMeshRecoveryWindow.cs
// Tools > NavMesh Recovery
//
// 1. Pick the ORIGINAL level file (text/YAML .unity, can be outside the project).
// 2. Pick the folder of old NavMesh .asset files (+ their .meta files if you have them).
// 3. Find Match: decodes every navmesh, checks for a GUID match, then scores each by how well it
//    lines up with the level's Navigation Static geometry.
// 4. Build: bakes a NavMeshData from the recovered polygons only and injects it into either
//      - a NavMeshSurface component on "!navmesh" (NavMeshComponents package), or
//      - the scene's built-in navmesh (the one the Navigation window bakes into; no component).
//    "Rebuild" redoes step 4 with new settings without re-matching.
//
// Unity 2018.1/2018.2, C# 4.0 compatible. NavMeshSurface is accessed by reflection, so this
// compiles whether or not the NavMeshComponents package is in the project.

using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NavMeshRecovery;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class NavMeshRecoveryWindow : EditorWindow
{
    enum Target { NavMeshSurface, SceneNavMesh }

    const string RootName = "!navmesh";
    const string OutFolder = "Assets/RecoveredNavMesh";
    const string PrefScene = "NavMeshRecovery.ScenePath";
    const string PrefFolder = "NavMeshRecovery.NavFolder";
    const string SurfaceTypeName = "UnityEngine.AI.NavMeshSurface";

    string m_ScenePath = "";
    string m_NavFolder = "";
    bool m_IncludeSubfolders = true;
    float m_MatchRadius = 12f;
    float m_HeightTolerance = 30f;
    float m_ConfidentScore = 0.5f;
    float m_ConfidentMargin = 0.2f;
    bool m_AutoBuild = true;

    Target m_Target = Target.NavMeshSurface;
    int m_AgentTypeIndex = 0;
    float m_BuildRadius = 0.1f;
    float m_VoxelSize = 0.05f;
    bool m_ClearLegacy = true;

    SceneInfo m_Scene;
    List<MatchResult> m_Results;
    List<DecodedNavMesh> m_Failed = new List<DecodedNavMesh>();
    string m_Status = "";
    Vector2 m_Scroll;

    static System.Type s_SurfaceType;
    static bool s_SurfaceTypeSearched;

    [MenuItem("Tools/NavMesh Recovery")]
    static void Open()
    {
        GetWindow<NavMeshRecoveryWindow>("NavMesh Recovery");
    }

    void OnEnable()
    {
        m_ScenePath = EditorPrefs.GetString(PrefScene, "");
        m_NavFolder = EditorPrefs.GetString(PrefFolder, "");
        if (SurfaceType == null) m_Target = Target.SceneNavMesh;
    }

    // ------------------------------------------------------------------ GUI

    void OnGUI()
    {
        EditorGUILayout.LabelField("Sources", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        m_ScenePath = EditorGUILayout.TextField("Original level (.unity)", m_ScenePath);
        if (GUILayout.Button("...", GUILayout.Width(30)))
        {
            string p = EditorUtility.OpenFilePanel("Original level (text YAML)", DirOf(m_ScenePath), "unity");
            if (!string.IsNullOrEmpty(p)) { m_ScenePath = p; m_Results = null; }
        }
        EditorGUILayout.EndHorizontal();

        SceneAsset dropped = (SceneAsset)EditorGUILayout.ObjectField("  or drag scene asset", null, typeof(SceneAsset), false);
        if (dropped != null)
        {
            m_ScenePath = Path.GetFullPath(AssetDatabase.GetAssetPath(dropped));
            m_Results = null;
        }

        EditorGUILayout.BeginHorizontal();
        m_NavFolder = EditorGUILayout.TextField("NavMesh assets folder", m_NavFolder);
        if (GUILayout.Button("...", GUILayout.Width(30)))
        {
            string p = EditorUtility.OpenFolderPanel("Folder of old NavMesh .asset files", m_NavFolder, "");
            if (!string.IsNullOrEmpty(p)) { m_NavFolder = p; m_Results = null; }
        }
        EditorGUILayout.EndHorizontal();
        m_IncludeSubfolders = EditorGUILayout.Toggle("Include subfolders", m_IncludeSubfolders);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Matching", EditorStyles.boldLabel);
        m_MatchRadius = EditorGUILayout.FloatField(new GUIContent("Match radius (m)", "How close a Navigation Static object must be to a navmesh polygon to count as overlapping."), m_MatchRadius);
        m_HeightTolerance = EditorGUILayout.FloatField("Height tolerance (m)", m_HeightTolerance);
        m_ConfidentScore = EditorGUILayout.Slider("Confident score", m_ConfidentScore, 0f, 1f);
        m_ConfidentMargin = EditorGUILayout.Slider("Confident margin over 2nd", m_ConfidentMargin, 0f, 1f);
        m_AutoBuild = EditorGUILayout.Toggle(new GUIContent("Auto-build if confident", "Builds straight away when there is a GUID match, or one clear winner."), m_AutoBuild);

        EditorGUILayout.Space();
        using (new EditorGUI.DisabledScope(!File.Exists(m_ScenePath) || !Directory.Exists(m_NavFolder)))
        {
            if (GUILayout.Button("Find Match", GUILayout.Height(28)))
                FindMatch();
        }

        if (!string.IsNullOrEmpty(m_Status))
            EditorGUILayout.HelpBox(m_Status, MessageType.Info);

        if (m_Results != null)
            DrawResults();

        DrawBuildSettings();
    }

    void DrawBuildSettings()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("NavMesh build", EditorStyles.boldLabel);

        m_Target = (Target)EditorGUILayout.EnumPopup(new GUIContent("Put navmesh into",
            "NavMeshSurface: a NavMeshSurface component on '" + RootName + "'.\n" +
            "SceneNavMesh: the scene's built-in navmesh (same slot the Navigation window bakes into)."), m_Target);
        if (m_Target == Target.NavMeshSurface && SurfaceType == null)
            EditorGUILayout.HelpBox("NavMeshSurface not found. Add the NavMeshComponents package, or use SceneNavMesh.", MessageType.Warning);

        int count = NavMesh.GetSettingsCount();
        string[] names = new string[count];
        for (int i = 0; i < count; i++)
            names[i] = NavMesh.GetSettingsNameFromID(NavMesh.GetSettingsByIndex(i).agentTypeID);
        m_AgentTypeIndex = Mathf.Clamp(EditorGUILayout.Popup("Agent type", m_AgentTypeIndex, names), 0, Mathf.Max(0, count - 1));

        m_BuildRadius = Mathf.Max(0.01f, EditorGUILayout.FloatField(new GUIContent("Build radius (m)",
            "Erosion used when rebuilding. The recovered polygons are already shrunk by the original agent radius, " +
            "so keep this small to avoid shrinking them twice. Agents still use their own radius for avoidance."), m_BuildRadius));
        m_VoxelSize = Mathf.Max(0.01f, EditorGUILayout.FloatField(new GUIContent("Voxel size (m)",
            "Smaller = closer to the original outline but slower to build. Keep at or below half the build radius."), m_VoxelSize));
        if (m_Target == Target.NavMeshSurface)
            m_ClearLegacy = EditorGUILayout.Toggle(new GUIContent("Clear scene's own bake",
                "Removes the scene's built-in navmesh so it doesn't overlap the NavMeshSurface one."), m_ClearLegacy);

        RecoveredNavMeshSource src = LoadSource(EditorSceneManager.GetActiveScene().name);
        using (new EditorGUI.DisabledScope(src == null))
        {
            if (GUILayout.Button(src == null ? "Rebuild (nothing recovered for this scene yet)" : "Rebuild from recovered source with these settings"))
                BuildAndInject(src);
        }
    }

    void DrawResults()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(string.Format("Candidates ({0})", m_Results.Count), EditorStyles.boldLabel);

        m_Scroll = EditorGUILayout.BeginScrollView(m_Scroll, GUILayout.MinHeight(120), GUILayout.MaxHeight(320));
        for (int i = 0; i < m_Results.Count; i++)
        {
            MatchResult r = m_Results[i];
            DecodedNavMesh n = r.NavMesh;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            string label = string.Format("{0}{1}   {2:0.0}%", r.GuidMatch ? "[GUID MATCH] " : "", Path.GetFileName(n.AssetPath), r.Score * 100f);
            EditorGUILayout.LabelField(label, i == 0 ? EditorStyles.boldLabel : EditorStyles.label);
            if (GUILayout.Button("Build", GUILayout.Width(60)))
                Build(r);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField(string.Format("   {0} polys, areas {1}, agent h {2:0.##} r {3:0.##} climb {4:0.##}",
                n.Polys.Count, AreasToString(n), n.WalkableHeight, n.WalkableRadius, n.WalkableClimb), EditorStyles.miniLabel);
            EditorGUILayout.LabelField("   " + n.AssetPath, EditorStyles.miniLabel);
            EditorGUILayout.EndVertical();
        }
        foreach (DecodedNavMesh f in m_Failed)
            EditorGUILayout.LabelField("Skipped " + Path.GetFileName(f.AssetPath) + ": " + f.Error, EditorStyles.miniLabel);
        EditorGUILayout.EndScrollView();
    }

    // ------------------------------------------------------------------ matching

    void FindMatch()
    {
        EditorPrefs.SetString(PrefScene, m_ScenePath);
        EditorPrefs.SetString(PrefFolder, m_NavFolder);
        m_Results = null;
        m_Failed.Clear();

        try
        {
            EditorUtility.DisplayProgressBar("NavMesh Recovery", "Parsing level...", 0f);
            m_Scene = SceneParser.Parse(m_ScenePath);

            string[] files = Directory.GetFiles(m_NavFolder, "*.asset",
                m_IncludeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            List<DecodedNavMesh> navs = new List<DecodedNavMesh>();
            for (int i = 0; i < files.Length; i++)
            {
                if (EditorUtility.DisplayCancelableProgressBar("NavMesh Recovery", "Decoding " + Path.GetFileName(files[i]), (float)i / files.Length))
                    break;
                if (!NavMeshDecoder.LooksLikeNavMeshAsset(files[i]))
                    continue;
                DecodedNavMesh n = NavMeshDecoder.Decode(files[i]);
                if (n.IsValid) navs.Add(n); else m_Failed.Add(n);
            }

            EditorUtility.DisplayProgressBar("NavMesh Recovery", "Scoring...", 1f);
            m_Results = NavMeshMatcher.Rank(m_Scene, navs, m_MatchRadius, m_HeightTolerance);
        }
        catch (System.Exception e)
        {
            m_Status = "Failed: " + e.Message;
            Debug.LogException(e);
            return;
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        m_Status = string.Format("Level: {0} Navigation Static mesh objects, navmesh GUID {1}. Decoded {2} navmeshes ({3} skipped).",
            m_Scene.NavStaticPoints.Count, m_Scene.NavMeshGuid ?? "none", m_Results.Count, m_Failed.Count);

        if (m_Results.Count == 0) { m_Status += "\nNo navmeshes found."; return; }

        MatchResult best = m_Results[0];
        float second = m_Results.Count > 1 ? m_Results[1].Score : 0f;
        bool confident = best.GuidMatch || (best.Score >= m_ConfidentScore && best.Score - second >= m_ConfidentMargin);

        if (confident)
        {
            m_Status += string.Format("\nBest match: {0} ({1}).", Path.GetFileName(best.NavMesh.AssetPath),
                best.GuidMatch ? "GUID match" : string.Format("{0:0.0}% vs {1:0.0}%", best.Score * 100f, second * 100f));
            if (m_AutoBuild) Build(best);
        }
        else
        {
            m_Status += "\nNo confident match. Review the candidates and press Build on the right one.";
        }
    }

    // ------------------------------------------------------------------ build: recovered polygons -> source meshes

    void Build(MatchResult r)
    {
        DecodedNavMesh nav = r.NavMesh;
        var active = EditorSceneManager.GetActiveScene();

        string levelName = Path.GetFileNameWithoutExtension(m_ScenePath);
        if (!string.Equals(active.name, levelName, System.StringComparison.OrdinalIgnoreCase))
        {
            if (!EditorUtility.DisplayDialog("NavMesh Recovery",
                string.Format("The open scene is '{0}' but the selected level is '{1}'. Build into '{0}' anyway?", active.name, levelName),
                "Build", "Cancel"))
                return;
        }

        EnsureFolder();
        string baseName = SafeName(active.name);

        // Triangulate each polygon (fan), grouped by navigation area.
        Dictionary<int, List<int>> trisByArea = new Dictionary<int, List<int>>();
        foreach (NavPoly p in nav.Polys)
        {
            List<int> tris;
            if (!trisByArea.TryGetValue(p.Area, out tris)) { tris = new List<int>(); trisByArea[p.Area] = tris; }
            for (int k = 1; k < p.Verts.Length - 1; k++)
                AddUpFacingTri(nav, tris, p.Verts[0], p.Verts[k], p.Verts[k + 1]);
        }

        Vector3[] verts = new Vector3[nav.Verts.Count];
        for (int i = 0; i < verts.Length; i++)
            verts[i] = new Vector3(nav.Verts[i].x, nav.Verts[i].y, nav.Verts[i].z);

        List<Mesh> meshes = new List<Mesh>();
        List<int> areas = new List<int>();
        foreach (KeyValuePair<int, List<int>> kv in trisByArea)
        {
            Mesh mesh = new Mesh();
            mesh.name = baseName + "_area" + kv.Key;
            if (verts.Length > 65000) mesh.indexFormat = IndexFormat.UInt32;
            mesh.vertices = verts;
            mesh.SetTriangles(kv.Value, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            meshes.Add(SaveAssetReplacing(mesh, OutFolder + "/" + mesh.name + ".asset"));
            areas.Add(kv.Key);
        }

        RecoveredNavMeshSource src = ScriptableObject.CreateInstance<RecoveredNavMeshSource>();
        src.sourceMeshes = meshes.ToArray();
        src.sourceAreas = areas.ToArray();
        src.sourceAssetPath = nav.AssetPath;
        src.originalAgentHeight = nav.WalkableHeight;
        src.originalAgentRadius = nav.WalkableRadius;
        src.originalAgentClimb = nav.WalkableClimb;
        SaveAssetReplacing(src, SourcePath(active.name));
        AssetDatabase.SaveAssets();

        string[] areaNames = GameObjectUtility.GetNavMeshAreaNames();
        foreach (int area in areas)
        {
            if (area >= 3 && (area >= areaNames.Length || string.IsNullOrEmpty(areaNames[area])))
                Debug.LogWarningFormat("NavMesh Recovery: area {0} is used but has no name in Navigation > Areas. Define it (and its cost), then press Rebuild.", area);
        }

        BuildAndInject(src);
    }

    // ------------------------------------------------------------------ build: source meshes -> NavMeshData -> target

    void BuildAndInject(RecoveredNavMeshSource src)
    {
        if (m_Target == Target.NavMeshSurface && SurfaceType == null)
        {
            EditorUtility.DisplayDialog("NavMesh Recovery", "NavMeshSurface isn't in this project. Add the NavMeshComponents package or switch the target to SceneNavMesh.", "OK");
            return;
        }

        var scene = EditorSceneManager.GetActiveScene();
        NavMeshBuildSettings settings;
        NavMeshData data = BuildData(src, out settings);
        if (data == null)
            return;

        string path = OutFolder + "/" + SafeName(scene.name) + "_NavMeshData.asset";
        data.name = Path.GetFileNameWithoutExtension(path);

        bool ok = m_Target == Target.NavMeshSurface
            ? InjectIntoSurface(data, path, settings.agentTypeID)
            : InjectIntoScene(data, path);
        if (!ok)
            return;

        EditorSceneManager.MarkSceneDirty(scene);
        AssetDatabase.SaveAssets();

        Debug.LogFormat("NavMesh Recovery: built {0} into {1} for agent type '{2}' (build radius {3}, voxel {4}) from {5}. Original agent: height {6:0.##}, radius {7:0.##}, climb {8:0.##}.",
            path, m_Target, NavMesh.GetSettingsNameFromID(settings.agentTypeID), settings.agentRadius, settings.voxelSize,
            src.sourceAssetPath, src.originalAgentHeight, src.originalAgentRadius, src.originalAgentClimb);
    }

    // Bakes from the recovered meshes only (nothing else in the scene), in world space.
    NavMeshData BuildData(RecoveredNavMeshSource src, out NavMeshBuildSettings settings)
    {
        int count = NavMesh.GetSettingsCount();
        settings = NavMesh.GetSettingsByIndex(Mathf.Clamp(m_AgentTypeIndex, 0, count - 1));
        settings.agentRadius = m_BuildRadius;
        settings.overrideVoxelSize = true;
        settings.voxelSize = Mathf.Min(m_VoxelSize, m_BuildRadius);
        // The recovered polygons were already filtered for slope/step by the original bake, so be permissive.
        settings.agentClimb = Mathf.Max(settings.agentClimb, src.originalAgentClimb);
        settings.agentSlope = Mathf.Max(settings.agentSlope, 60f);

        List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
        Bounds bounds = new Bounds();
        bool hasBounds = false;
        for (int i = 0; i < src.sourceMeshes.Length; i++)
        {
            Mesh m = src.sourceMeshes[i];
            if (m == null) continue;
            NavMeshBuildSource s = new NavMeshBuildSource();
            s.shape = NavMeshBuildSourceShape.Mesh;
            s.sourceObject = m;
            s.transform = Matrix4x4.identity; // recovered meshes are in world space
            s.area = src.sourceAreas != null && i < src.sourceAreas.Length ? src.sourceAreas[i] : 0;
            sources.Add(s);
            if (hasBounds) bounds.Encapsulate(m.bounds); else { bounds = m.bounds; hasBounds = true; }
        }
        if (sources.Count == 0)
        {
            Debug.LogError("NavMesh Recovery: the recovered source has no meshes. Run Find Match / Build again.");
            return null;
        }
        bounds.Expand(new Vector3(1f, 4f, 1f));

        foreach (string msg in settings.ValidationReport(bounds))
            Debug.LogWarning("NavMesh Recovery: " + msg);

        NavMeshData data;
        try
        {
            EditorUtility.DisplayProgressBar("NavMesh Recovery", "Building NavMeshData...", 0.5f);
            data = UnityEngine.AI.NavMeshBuilder.BuildNavMeshData(settings, sources, bounds, Vector3.zero, Quaternion.identity);
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
        if (data == null)
            Debug.LogError("NavMesh Recovery: NavMeshBuilder.BuildNavMeshData returned null.");
        return data;
    }

    // Target 1: NavMeshSurface on "!navmesh" (reuses an existing one if it's already there).
    bool InjectIntoSurface(NavMeshData data, string path, int agentTypeID)
    {
        System.Type st = SurfaceType;
        GameObject root = FindRoot();
        if (root == null)
        {
            root = new GameObject(RootName);
            Undo.RegisterCreatedObjectUndo(root, "Create " + RootName);
        }

        // The data is built in world space; NavMeshSurface places it at its own transform.
        Transform t = root.transform;
        if (t.position != Vector3.zero || t.rotation != Quaternion.identity)
        {
            Undo.RecordObject(t, "Reset " + RootName + " transform");
            t.position = Vector3.zero;
            t.rotation = Quaternion.identity;
            Debug.LogWarning("NavMesh Recovery: moved " + RootName + " to the origin so the recovered navmesh lines up with the level.");
        }

        Component surface = root.GetComponent(st);
        if (surface == null)
            surface = Undo.AddComponent(root, st);

        Undo.RecordObject(surface, "Inject recovered navmesh");
        Invoke(surface, "RemoveData");
        SetProp(surface, "navMeshData", null);
        SetProp(surface, "agentTypeID", agentTypeID);

        if (m_ClearLegacy)
            UnityEditor.AI.NavMeshBuilder.ClearAllNavMeshes();

        SetProp(surface, "navMeshData", SaveAssetReplacing(data, path));
        if (((Behaviour)surface).isActiveAndEnabled)
            Invoke(surface, "AddData");
        EditorUtility.SetDirty(surface);
        Selection.activeGameObject = root;
        return true;
    }

    // Target 2: the scene's built-in navmesh (NavMeshSettings.m_NavMeshData), no component needed.
    bool InjectIntoScene(NavMeshData data, string path)
    {
        // NavMeshBuilder.navMeshSettingsObject is what the Navigation window itself edits. Its visibility
        // has varied between Unity versions, so look it up by reflection.
        Object settingsObject = null;
        PropertyInfo sp = typeof(UnityEditor.AI.NavMeshBuilder).GetProperty("navMeshSettingsObject",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if (sp != null)
            settingsObject = sp.GetValue(null, null) as Object;
        if (settingsObject == null)
        {
            Debug.LogError("NavMesh Recovery: couldn't access the scene's NavMeshSettings in this Unity version. Use the NavMeshSurface target instead.");
            return false;
        }
        SerializedObject so = new SerializedObject(settingsObject);
        SerializedProperty prop = so.FindProperty("m_NavMeshData");
        if (prop == null)
        {
            Debug.LogError("NavMesh Recovery: NavMeshSettings has no m_NavMeshData property in this Unity version.");
            return false;
        }

        // Unregister whatever the scene currently has (also deletes an old Navigation-window bake asset).
        UnityEditor.AI.NavMeshBuilder.ClearAllNavMeshes();

        NavMeshData saved = SaveAssetReplacing(data, path);
        so.Update();
        prop.objectReferenceValue = saved;
        so.ApplyModifiedPropertiesWithoutUndo();

        var scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        if (EditorUtility.DisplayDialog("NavMesh Recovery",
            "The scene now references the recovered navmesh. Unity only loads a scene's navmesh when the scene opens.\n\nSave and reopen the scene now?",
            "Save and reopen", "Later"))
        {
            if (EditorSceneManager.SaveScene(scene))
                EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Single);
        }
        return true;
    }

    // ------------------------------------------------------------------ NavMeshSurface reflection

    static System.Type SurfaceType
    {
        get
        {
            if (!s_SurfaceTypeSearched)
            {
                s_SurfaceTypeSearched = true;
                foreach (Assembly asm in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    System.Type t = asm.GetType(SurfaceTypeName, false);
                    if (t != null && typeof(Component).IsAssignableFrom(t)) { s_SurfaceType = t; break; }
                }
            }
            return s_SurfaceType;
        }
    }

    static void SetProp(object target, string name, object value)
    {
        PropertyInfo p = target.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
        if (p == null || !p.CanWrite)
            throw new System.MissingMemberException(target.GetType().Name, name);
        p.SetValue(target, value, null);
    }

    static void Invoke(object target, string method)
    {
        MethodInfo m = target.GetType().GetMethod(method, BindingFlags.Instance | BindingFlags.Public, null, System.Type.EmptyTypes, null);
        if (m == null)
            throw new System.MissingMethodException(target.GetType().Name, method);
        m.Invoke(target, null);
    }

    // ------------------------------------------------------------------ helpers

    static string SourcePath(string sceneName)
    {
        return OutFolder + "/" + SafeName(sceneName) + "_Source.asset";
    }

    static RecoveredNavMeshSource LoadSource(string sceneName)
    {
        return AssetDatabase.LoadAssetAtPath<RecoveredNavMeshSource>(SourcePath(sceneName));
    }

    static T SaveAssetReplacing<T>(T obj, string path) where T : Object
    {
        if (AssetDatabase.LoadAssetAtPath<Object>(path) != null)
            AssetDatabase.DeleteAsset(path);
        AssetDatabase.CreateAsset(obj, path);
        return obj;
    }

    static void EnsureFolder()
    {
        if (!AssetDatabase.IsValidFolder(OutFolder))
            AssetDatabase.CreateFolder("Assets", "RecoveredNavMesh");
    }

    static void AddUpFacingTri(DecodedNavMesh nav, List<int> tris, int a, int b, int c)
    {
        V3 va = nav.Verts[a], vb = nav.Verts[b], vc = nav.Verts[c];
        // Y component of Vector3.Cross(b - a, c - a). Positive = front face points up in Unity
        // (clockwise when seen from above). Downward-facing triangles would be treated as unwalkable.
        float ny = (vb.z - va.z) * (vc.x - va.x) - (vb.x - va.x) * (vc.z - va.z);
        if (ny >= 0f) { tris.Add(a); tris.Add(b); tris.Add(c); }
        else { tris.Add(a); tris.Add(c); tris.Add(b); }
    }

    static GameObject FindRoot()
    {
        foreach (GameObject go in EditorSceneManager.GetActiveScene().GetRootGameObjects())
            if (go.name == RootName) return go;
        return null;
    }

    static string AreasToString(DecodedNavMesh n)
    {
        List<string> parts = new List<string>();
        foreach (KeyValuePair<int, int> kv in n.AreaCounts)
            parts.Add(kv.Key + ":" + kv.Value);
        return "{" + string.Join(", ", parts.ToArray()) + "}";
    }

    static string SafeName(string s)
    {
        foreach (char c in Path.GetInvalidFileNameChars()) s = s.Replace(c, '_');
        return s;
    }

    static string DirOf(string path)
    {
        return string.IsNullOrEmpty(path) ? "" : Path.GetDirectoryName(path);
    }
}
