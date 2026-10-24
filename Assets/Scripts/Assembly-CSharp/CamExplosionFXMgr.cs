#define DEBUG
using UnityEngine;

public class CamExplosionFXMgr : MonoBehaviour
{
	public static CamExplosionFXMgr Instance;

	private bool m_PrevDbgBtnState;

	private void Awake()
	{
		if (!Instance)
		{
			Instance = this;
		}
	}

	private void OnDestroy()
	{
		Instance = null;
	}

	private void LateUpdate()
	{
		DisableCamFX();
	}

	public void Reset()
	{
		if (MFExplosionPostFX.Instance != null)
		{
			MFExplosionPostFX.Instance.Reset();
		}
	}

	public static void PreloadResources()
	{
		if (!MFExplosionPostFX.Instance)
		{
			Camera.main.gameObject.AddComponent<MFExplosionPostFX>();
			DebugUtils.Assert(MFExplosionPostFX.Instance);
		}
	}

	private void CreateCamFXInstance()
	{
		if (!MFExplosionPostFX.Instance)
		{
			Camera.main.gameObject.AddComponent<MFExplosionPostFX>();
			DebugUtils.Assert(MFExplosionPostFX.Instance);
		}
		MFExplosionPostFX.Instance.enabled = true;
	}

	private void DisableCamFX()
	{
		if ((bool)MFExplosionPostFX.Instance && MFExplosionPostFX.Instance.enabled && MFExplosionPostFX.Instance.NumActiveEffects() == 0)
		{
			MFExplosionPostFX.Instance.enabled = false;
		}
	}

	private void DbgEmitFX()
	{
		bool button = Input.GetButton("Fire1");
		if (button && !m_PrevDbgBtnState)
		{
			DbgEmitGrenadeExplWave();
		}
		m_PrevDbgBtnState = button;
	}

	private void DbgEmitGrenadeExplWave()
	{
		if ((bool)Camera.main)
		{
			CreateCamFXInstance();
			Vector2 normScreenPos = default(Vector2);
			normScreenPos.x = Random.Range(0f, 1f);
			normScreenPos.y = Random.Range(0f, 1f);
			MFExplosionPostFX.S_WaveParams waveParams = default(MFExplosionPostFX.S_WaveParams);
			waveParams.m_Amplitude = 0.3f;
			waveParams.m_Duration = 1.5f;
			waveParams.m_Freq = 20f;
			waveParams.m_Speed = 1.5f;
			waveParams.m_Radius = 1f;
			waveParams.m_Delay = 0f;
			MFExplosionPostFX.Instance.EmitGrenadeExplosionWave(normScreenPos, waveParams);
		}
	}

	public void SpawnExplosionWaveFX(Vector3 worldPos, MFExplosionPostFX.S_WaveParams waveParams)
	{
		waveParams.m_Delay = 0f;
		InternalSpawnExplosionWaveFX(worldPos, waveParams);
	}

	public void SpawnExplosionWaveFX(Vector3 worldPos, MFExplosionPostFX.S_WaveParams waveParams, float inDelay)
	{
		waveParams.m_Delay = inDelay;
		InternalSpawnExplosionWaveFX(worldPos, waveParams);
	}

	private bool InternalSpawnExplosionWaveFX(Vector3 worldPos, MFExplosionPostFX.S_WaveParams waveParams)
	{
		if (!Camera.main)
		{
			return false;
		}
		Vector3 v = Camera.main.worldToCameraMatrix.MultiplyPoint(worldPos);
		if (v.z > 0f)
		{
			if (waveParams.m_Radius < 0.75f)
			{
				return false;
			}
			v.z = 0f - v.z;
		}
		Vector3 position = Camera.main.cameraToWorldMatrix.MultiplyPoint(v);
		Vector3 vector = Camera.main.WorldToViewportPoint(position);
		CreateCamFXInstance();
		Vector2 normScreenPos = default(Vector2);
		normScreenPos.x = vector.x;
		normScreenPos.y = vector.y;
		MFExplosionPostFX.Instance.EmitGrenadeExplosionWave(normScreenPos, waveParams);
		return true;
	}
}
