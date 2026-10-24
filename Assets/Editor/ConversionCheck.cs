using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Batch-mode verification of the light probe conversion and the static mesh separation.
//   Unity.exe -batchmode -quit -projectPath <proj> -executeMethod ConversionCheck.Run -logFile <log>
// Writes a plain-text report next to the project (ConversionCheck.txt) so it can be read
// without digging through the editor log.
public static class ConversionCheck
{
	public static void Run()
	{
		StringBuilder sb = new StringBuilder();
		string[] levels = { "level_01", "level_02", "level_03", "level_04", "level_05", "level_06", "level_07",
			"level_08", "level_09", "level_10", "level_11_dlc", "level_12_dlc", "level_13_dlc", "level_14_dlc" };

		sb.AppendLine("== Light probes ==");
		foreach (string lv in levels)
		{
			LightProbes lp = Resources.Load<LightProbes>("lightprobes/" + lv);
			if (lp == null)
			{
				sb.AppendLine(lv + ": NOT LOADED");
				continue;
			}
			Vector3[] pos = lp.positions;
			UnityEngine.Rendering.SphericalHarmonicsL2[] sh = lp.bakedProbes;
			string first = (sh != null && sh.Length > 0) ? string.Format("L0 rgb=({0:F3},{1:F3},{2:F3})", sh[0][0, 0], sh[0][1, 0], sh[0][2, 0]) : "no coefficients";
			// The ambient (L0) term must be positive in every channel of every probe; a
			// negative one means the 27 coefficients are in the wrong order.
			int badL0 = 0;
			float minL0 = float.MaxValue;
			if (sh != null)
			{
				foreach (UnityEngine.Rendering.SphericalHarmonicsL2 p in sh)
				{
					for (int c = 0; c < 3; c++)
					{
						minL0 = Mathf.Min(minL0, p[c, 0]);
						if (p[c, 0] <= 0f) badL0++;
					}
				}
			}
			sb.AppendLine(string.Format("{0}: count={1} positions={2} cellCount={3} {4} minL0={5:F3} nonPositiveL0={6}", lv, lp.count, pos == null ? -1 : pos.Length, lp.cellCount, first, minL0, badL0));
		}

		sb.AppendLine();
		sb.AppendLine("== Shader compile ==");
		// Importing a shader only parses it; surface shaders are compiled when first used.
		// ShaderHasError forces the compile so errors show up here instead of in the editor.
		// ShaderHasError / GetShaderErrorCount are internal in 2018.2, hence reflection.
		System.Reflection.BindingFlags any = System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;
		System.Reflection.MethodInfo hasError = typeof(ShaderUtil).GetMethod("ShaderHasError", any, null, new[] { typeof(Shader) }, null);
		System.Reflection.MethodInfo errCount = typeof(ShaderUtil).GetMethod("GetShaderErrorCount", any, null, new[] { typeof(Shader) }, null);
		int shaderErrors = 0, shadersChecked = 0;
		foreach (string guid in AssetDatabase.FindAssets("t:Shader", new[] { "Assets/Shader" }))
		{
			string sp = AssetDatabase.GUIDToAssetPath(guid);
			Shader s = AssetDatabase.LoadAssetAtPath<Shader>(sp);
			if (s == null) continue;
			shadersChecked++;
			bool bad = false;
			if (hasError != null) bad = (bool)hasError.Invoke(null, new object[] { s });
			else if (errCount != null) bad = (int)errCount.Invoke(null, new object[] { s }) > 0;
			if (bad)
			{
				shaderErrors++;
				sb.AppendLine("ERROR: " + sp + " (" + s.name + ")");
			}
		}
		sb.AppendLine(string.Format("shaders checked: {0} with errors: {1} (method: {2})", shadersChecked, shaderErrors,
			hasError != null ? "ShaderHasError" : errCount != null ? "GetShaderErrorCount" : "NONE AVAILABLE"));

		sb.AppendLine();
		sb.AppendLine("== Mesh topology probe ==");
		// The 3.5 export stores every index buffer as a triangle strip (isTriStrip). If Unity
		// 2018 still honours that field, Plane01 (a 4-index strip) reads back as 2 triangles.
		foreach (string mp in new[] { "Assets/Mesh/Plane01.asset", "Assets/Mesh/Separated/i2_sloup_01.asset" })
		{
			Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(mp);
			if (mesh == null)
			{
				sb.AppendLine(mp + ": not loaded");
				continue;
			}
			sb.AppendLine(string.Format("{0}: vertices={1} submeshes={2} topology={3} indexCount={4} triangles={5} bounds={6}",
				mp, mesh.vertexCount, mesh.subMeshCount, mesh.GetTopology(0), mesh.GetIndexCount(0), mesh.triangles.Length / 3, mesh.bounds));
		}

		sb.AppendLine();
		sb.AppendLine("== Scenes ==");
		foreach (string lv in levels)
		{
			string path = "Assets/" + lv + ".unity";
			if (!File.Exists(path))
			{
				sb.AppendLine(lv + ": missing scene");
				continue;
			}
			EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
			int filters = 0, nullMesh = 0, batched = 0, combined = 0, separated = 0, lightmapped = 0, probeUsers = 0;
			foreach (MeshFilter mf in Object.FindObjectsOfType<MeshFilter>())
			{
				filters++;
				if (mf.sharedMesh == null)
				{
					nullMesh++;
					continue;
				}
				string ap = AssetDatabase.GetAssetPath(mf.sharedMesh);
				if (mf.sharedMesh.name.StartsWith("Combined Mesh")) combined++;
				if (ap.Contains("/Separated/")) separated++;
			}
			foreach (MeshRenderer r in Object.FindObjectsOfType<MeshRenderer>())
			{
				if (r.isPartOfStaticBatch) batched++;
				if (r.lightmapIndex >= 0 && r.lightmapIndex < 65534) lightmapped++;
				if (r.lightProbeUsage != UnityEngine.Rendering.LightProbeUsage.Off) probeUsers++;
			}
			// Runtime restorer does this in play mode; in the editor do it here to prove the data binds.
			LightProbes lp = Resources.Load<LightProbes>("lightprobes/" + lv);
			string probeMsg = "no probes";
			if (lp != null)
			{
				LightmapSettings.lightProbes = lp;
				Vector3 p = lp.positions[0];
				UnityEngine.Rendering.SphericalHarmonicsL2 sh;
				LightProbes.GetInterpolatedProbe(p, null, out sh);
				probeMsg = string.Format("interpolated at probe0 L0=({0:F3},{1:F3},{2:F3})", sh[0, 0], sh[1, 0], sh[2, 0]);
			}
			sb.AppendLine(string.Format("{0}: meshFilters={1} nullMesh={2} combinedRefs={3} separatedRefs={4} staticBatched={5} lightmapped={6} probeUsers={7} | {8}",
				lv, filters, nullMesh, combined, separated, batched, lightmapped, probeUsers, probeMsg));
		}

		string outPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "ConversionCheck.txt");
		File.WriteAllText(outPath, sb.ToString());
		Debug.Log("ConversionCheck written to " + outPath + "\n" + sb);
	}
}
