using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Unity 3.5 stored baked lightmap references and the light probe asset inline in the
// scene's LightmapSettings. Unity 5+ moved both into a LightingData asset and cannot read
// the old form, so on import every level lost its lightmaps and its light probes: all
// lightmapped surfaces rendered unlit (very dark, since this game bakes its lighting and
// has almost no runtime lights) and characters got no probe lighting at all.
//
// The data itself survived the export, so this reattaches it at runtime:
//   Resources/lightmaps/<sceneName>-<index>.png   -> LightmapSettings.lightmaps
//   Resources/lightprobes/<sceneName>.asset       -> LightmapSettings.lightProbes
// (the probe asset is the Unity 3.5 one converted to the 2018 layout by
// Tools/convert_lightprobes.py). The per-renderer lightmap index and UV offset are
// carried by BakedLightmapInfo components, which apply themselves on enable.
//
// Also usable from the editor (see Editor/LightmapEditorHook.cs) so the Scene view shows
// the baked lighting.
public static class LightmapRestorer
{
	[RuntimeInitializeOnLoadMethod]
	private static void Init()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
		Apply(SceneManager.GetActiveScene());
	}

	private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		Apply(scene);
	}

	public static void Apply(Scene scene)
	{
		if (!scene.IsValid() || string.IsNullOrEmpty(scene.name))
		{
			return;
		}
		List<LightmapData> maps = new List<LightmapData>();
		for (int i = 0; ; i++)
		{
			Texture2D tex = Resources.Load<Texture2D>("lightmaps/" + scene.name + "-" + i);
			if (tex == null)
			{
				break;
			}
			LightmapData d = new LightmapData();
			d.lightmapColor = tex;
			maps.Add(d);
		}
		// Additively loaded scenes (the GUI scene) have no lightmaps of their own -
		// leave whatever the level already installed alone.
		if (maps.Count > 0)
		{
			LightmapSettings.lightmapsMode = LightmapsMode.NonDirectional;
			LightmapSettings.lightmaps = maps.ToArray();
		}
		LightProbes probes = Resources.Load<LightProbes>("lightprobes/" + scene.name);
		if (probes != null)
		{
			LightmapSettings.lightProbes = probes;
		}
	}
}
