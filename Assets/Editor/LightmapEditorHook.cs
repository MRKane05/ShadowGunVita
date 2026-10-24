using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Editor counterpart of LightmapRestorer: install the level's lightmaps and light probes
// whenever a scene is opened (and after every script reload), so the Scene and Game views
// show the baked lighting without entering play mode. Nothing here is saved into the
// scene; it only sets LightmapSettings in memory, exactly as the runtime path does.
[InitializeOnLoad]
public static class LightmapEditorHook
{
	static LightmapEditorHook()
	{
		EditorSceneManager.sceneOpened += OnSceneOpened;
		EditorApplication.delayCall += () => LightmapRestorer.Apply(SceneManager.GetActiveScene());
	}

	private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
	{
		LightmapRestorer.Apply(scene);
	}

	[MenuItem("Tools/Lightmaps/Re-apply baked lighting to open scene")]
	private static void Reapply()
	{
		LightmapRestorer.Apply(SceneManager.GetActiveScene());
		foreach (BakedLightmapInfo info in Object.FindObjectsOfType<BakedLightmapInfo>())
		{
			info.Apply();
		}
		SceneView.RepaintAll();
	}
}
