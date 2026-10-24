using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class _CrashRepro
{
	public static void Probe2()
	{
		EditorSceneManager.OpenScene("Assets/level_01.unity", OpenSceneMode.Single);
		Texture2D tex = Resources.Load<Texture2D>("lightmaps/level_01-0");
		Debug.Log("P2 texture=" + (tex == null ? "NULL" : "ok"));
		LightmapData d = new LightmapData();
		d.lightmapColor = tex;
		LightmapSettings.lightmapsMode = LightmapsMode.NonDirectional;
		LightmapSettings.lightmaps = new LightmapData[] { d };
		Debug.Log("P2 lightmaps now=" + LightmapSettings.lightmaps.Length);
		int lit = 0, total = 0;
		foreach (MeshRenderer r in Object.FindObjectsOfType<MeshRenderer>())
		{
			total++;
			if (r.lightmapIndex >= 0 && r.lightmapIndex < 65535) lit++;
		}
		Debug.Log("P2 after assigning lightmaps: renderers=" + total + " withIndex=" + lit);
		Debug.Log("P2: DONE");
	}
}
