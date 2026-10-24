using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

// Exports one level as a self-contained .unitypackage (scene + every asset it references
// by GUID, plus the Resources the game loads by name and the restore scripts).
//   Unity.exe -batchmode -quit -projectPath <proj> -executeMethod ExportLevelPackage.Run
// Level and output path come from the LEVEL / OUT environment variables
// (defaults: level_01, D:\unity\shadowgun\level01_testcase\level_01.unitypackage).
public static class ExportLevelPackage
{
	public static void Run()
	{
		string level = System.Environment.GetEnvironmentVariable("LEVEL");
		if (string.IsNullOrEmpty(level)) level = "level_01";
		string outPath = System.Environment.GetEnvironmentVariable("OUT");
		if (string.IsNullOrEmpty(outPath)) outPath = @"D:\unity\shadowgun\level01_testcase\" + level + ".unitypackage";
		Directory.CreateDirectory(Path.GetDirectoryName(outPath));

		List<string> roots = new List<string>
		{
			"Assets/" + level + ".unity",
			"Assets/" + level,                       // per-level folder (occlusion data)
			"Assets/Resources",                      // loaded by name at runtime: lightmaps, probes, prefabs
			"Assets/Scripts",                        // whole game code, so the scene's MonoBehaviours resolve
			"Assets/Plugins",
			"Assets/Editor/LightmapEditorHook.cs",
			"Assets/Editor/ConversionCheck.cs",
			"Assets/Editor/ExportLevelPackage.cs",
			"Assets/Gui_16_9.unity",                 // additively loaded by the game at runtime
			"Assets/Gui_16_9_HD.unity",
			"Assets/empty.unity",
		};
		List<string> existing = new List<string>();
		foreach (string r in roots)
		{
			if (File.Exists(r) || Directory.Exists(r)) existing.Add(r);
			else Debug.LogWarning("ExportLevelPackage: skipping missing " + r);
		}
		string[] deps = AssetDatabase.GetDependencies(existing.ToArray(), true);
		Debug.Log("ExportLevelPackage: " + existing.Count + " roots, " + deps.Length + " assets with dependencies");
		AssetDatabase.ExportPackage(existing.ToArray(), outPath, ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);
		File.WriteAllLines(Path.ChangeExtension(outPath, ".contents.txt"), deps);
		Debug.Log("ExportLevelPackage: wrote " + outPath + " (" + new FileInfo(outPath).Length / (1024 * 1024) + " MB)");
	}
}
