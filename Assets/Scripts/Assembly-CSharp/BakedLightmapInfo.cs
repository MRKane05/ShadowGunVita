using UnityEngine;

// Carries the baked-lightmap assignment of one renderer.
//
// Unity 3.5 stored each renderer's lightmap index and UV scale/offset in the scene file
// (m_LightmapIndex / m_LightmapTilingOffset on the MeshRenderer). Unity 5 and later keep
// that data in the scene's LightingData.asset instead, so the editor drops the old fields
// the first time the scene is saved - and this project cannot produce a LightingData.asset
// without re-baking, which would throw the original lighting away.
//
// Tools/inject_lightmap_info.py adds one of these next to every lightmapped MeshRenderer,
// filled from the pristine export. Renderer.lightmapIndex / lightmapScaleOffset are plain
// settable properties, so re-applying them on enable (in the editor too, hence
// ExecuteInEditMode) survives any number of scene saves. LightmapRestorer installs the
// lightmap textures themselves.
[ExecuteInEditMode]
[DisallowMultipleComponent]
public class BakedLightmapInfo : MonoBehaviour
{
	public int lightmapIndex = -1;

	public Vector4 scaleOffset = new Vector4(1f, 1f, 0f, 0f);

	private void OnEnable()
	{
		Apply();
	}

	public void Apply()
	{
		Renderer r = GetComponent<Renderer>();
		if (r == null)
		{
			return;
		}
		r.lightmapIndex = lightmapIndex;
		r.lightmapScaleOffset = scaleOffset;
	}
}
