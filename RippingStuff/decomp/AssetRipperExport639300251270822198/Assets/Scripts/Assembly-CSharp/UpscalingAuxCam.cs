using UnityEngine;

public class UpscalingAuxCam : MonoBehaviour
{
	public RenderTexture m_RenderTex;

	private void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		if ((bool)m_RenderTex)
		{
			Graphics.Blit(m_RenderTex, dst);
		}
	}
}
