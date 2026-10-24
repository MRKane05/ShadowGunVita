using UnityEngine;

public class CameraUpscaling : MonoBehaviour
{
	public bool m_EnableUpscaling;

	public int m_ReduceScreenSizeByPercent = 10;

	private RenderTexture m_RenderTex;

	private GameObject m_GameObj;

	private UpscalingAuxCam m_AuxCam;

	private int m_MinRTSize = 64;

	private void Awake()
	{
		if ((bool)base.GetComponent<Camera>() && m_EnableUpscaling && m_ReduceScreenSizeByPercent > 0)
		{
			m_GameObj = new GameObject("UpscalingAuxGO");
			m_GameObj.AddComponent<Camera>();
			m_AuxCam = m_GameObj.AddComponent<UpscalingAuxCam>() as UpscalingAuxCam;
			m_GameObj.GetComponent<Camera>().clearFlags = CameraClearFlags.Nothing;
			m_GameObj.GetComponent<Camera>().cullingMask = 0;
			m_GameObj.GetComponent<Camera>().depthTextureMode = DepthTextureMode.None;
			m_GameObj.GetComponent<Camera>().nearClipPlane = 0.1f;
			m_GameObj.GetComponent<Camera>().farClipPlane = 100f;
			m_GameObj.GetComponent<Camera>().transform.position = new Vector3(9999f, 9999f, 9999f);
			m_GameObj.GetComponent<Camera>().name = "UpscalingAUXCam";
			int width = Screen.width;
			int height = Screen.height;
			int num = (int)((float)width * (1f - (float)m_ReduceScreenSizeByPercent / 100f));
			int num2 = (int)((float)height * (1f - (float)m_ReduceScreenSizeByPercent / 100f));
			if (num < m_MinRTSize)
			{
				num = m_MinRTSize;
			}
			if (num2 < m_MinRTSize)
			{
				num2 = m_MinRTSize;
			}
			Init(num, num2);
			base.GetComponent<Camera>().targetTexture = m_RenderTex;
		}
	}

	private bool Init(int width, int height)
	{
		m_RenderTex = new RenderTexture(width, height, 16, RenderTextureFormat.RGB565);
		m_RenderTex.name = "DownscaledRT";
		if (!m_RenderTex.Create())
		{
			return false;
		}
		m_AuxCam.m_RenderTex = m_RenderTex;
		return true;
	}
}
