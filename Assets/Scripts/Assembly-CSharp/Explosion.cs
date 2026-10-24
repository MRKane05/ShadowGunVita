#define DEBUG
using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Explosion : MonoBehaviour
{
	[SerializeField]
	private float m_DamageRadius = 5f;

	[SerializeField]
	private float m_MaxDamage = 5f;

	[SerializeField]
	private E_WeaponType m_WeaponType = E_WeaponType.None;

	[SerializeField]
	private GameCamera.ShakingEffect m_ShakingEffect;

	[SerializeField]
	private float m_ShakingEffectDistance = -1f;

	[SerializeField]
	private float m_ShakingEffectScale = 1f;

	public bool m_GenerateWaveFX;

	public float m_WaveFXDelay;

	public float m_WaveFXAmplitudeMin = 1f;

	public float m_WaveFXAmplitudeMax = 1f;

	public float m_WaveFXFreq = 20f;

	public float m_WaveFXDuration = 1.5f;

	public float m_WaveFXRadiusMin = 0.1f;

	public float m_WaveFXRadiusMax = 1f;

	public float m_WaveFXSpeed = 1.3f;

	public float m_WaveFXMaxWrldDist = 30f;

	public float m_ParticleCriticalDistance = 5f;

	private ParticleSystem[] m_Emitters;

	private GameObject m_GameObject;

	private bool m_Exploded;

	private float shakingEffectDistance
	{
		get
		{
			return (!(m_ShakingEffectDistance < 0f)) ? m_ShakingEffectDistance : damageRadius;
		}
	}

	public Agent causer { get; set; }

	public float damage { get; set; }

	public float damageRadius { get; set; }

	public Transform[] noBlocking { get; set; }

	public Explosion cacheKey { get; set; }

	private void Awake()
	{
		m_GameObject = base.gameObject;
		m_Emitters = m_GameObject.GetComponentsInChildren<ParticleSystem>();
		damage = m_MaxDamage;
		damageRadius = m_DamageRadius;
	}

	private void OnDestroy()
	{
		m_Emitters = null;
	}

	private void Update()
	{
		if (!m_Exploded)
		{
			Explode();
			m_Exploded = true;
			return;
		}
		bool flag = false;
		ParticleSystem[] emitters = m_Emitters;
		foreach (ParticleSystem particleSystem in emitters)
		{
			if (particleSystem.isPlaying || particleSystem.particleCount > 0)
			{
				flag = true;
				break;
			}
		}
		if (!flag && !base.GetComponent<AudioSource>().isPlaying)
		{
			CleanUp();
		}
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	private void OnDrawGizmos()
	{
	}

	public void Explode()
	{
		DebugUtils.Assert(!m_Exploded);
		if (m_ParticleCriticalDistance < 0f || Camera.main == null || Vector3.Magnitude(Camera.main.transform.position - base.transform.position) > m_ParticleCriticalDistance)
		{
			ParticleSystem[] emitters = m_Emitters;
			foreach (ParticleSystem particleSystem in emitters)
			{
				particleSystem.Play();
			}
		}
		if (base.GetComponent<AudioSource>() != null && base.GetComponent<AudioSource>().clip != null)
		{
			base.GetComponent<AudioSource>().Play();
		}
		if (damageRadius > 0f && damage > 0f)
		{
			ApplyDamage();
		}
		if (m_ShakingEffect != GameCamera.ShakingEffect.None && shakingEffectDistance > 0.1f && m_ShakingEffectScale > 0f)
		{
			PlayShakingCameraEffect();
		}
		if (m_GenerateWaveFX && (bool)CamExplosionFXMgr.Instance)
		{
			GenerateWaveFX();
		}
	}

	public void CleanUp()
	{
		if (cacheKey != null)
		{
			Mission.Instance.ExplosionCache.Return(this);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public void Reset()
	{
		ParticleSystem[] emitters = m_Emitters;
		foreach (ParticleSystem particleSystem in emitters)
		{
			particleSystem.Stop();
		}
		base.GetComponent<AudioSource>().Stop();
		m_Exploded = false;
		causer = null;
		damage = m_MaxDamage;
		damageRadius = m_DamageRadius;
		noBlocking = null;
	}

	private void ApplyDamage()
	{
		Vector3 position = base.transform.position;
		Collider[] array = Physics.OverlapSphere(position, damageRadius);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			HitZone component = collider.GetComponent<HitZone>();
			Vector3 outInpuls;
			if (component != null)
			{
				float num = ComputeTotalDamage(collider, position, out outInpuls);
				if (num >= 0f)
				{
					component.OnReceiveRangeDamage(causer, num, outInpuls, m_WeaponType);
				}
				continue;
			}
			Agent firstComponentUpward = collider.gameObject.GetFirstComponentUpward<Agent>();
			if (firstComponentUpward != null)
			{
				float num2 = ComputeTotalDamage(collider, position, out outInpuls);
				if (num2 >= 0f)
				{
					firstComponentUpward.OnReceiveRangeDamage(causer, num2, outInpuls, m_WeaponType);
				}
				continue;
			}
			DestroyObject component2 = collider.GetComponent<DestroyObject>();
			if (component2 != null)
			{
				float num3 = ComputeTotalDamage(collider, position, out outInpuls);
				if (num3 >= 0f)
				{
					component2.OnReceiveRangeDamage(causer, num3, outInpuls, m_WeaponType);
				}
				continue;
			}
			DamageEvent component3 = collider.GetComponent<DamageEvent>();
			if (component3 != null)
			{
				float num4 = ComputeTotalDamage(collider, position, out outInpuls);
				if (num4 >= 0f)
				{
					component3.OnReceiveRangeDamage(causer, num4, outInpuls, m_WeaponType);
				}
			}
		}
	}

	private float ComputeTotalDamage(Collider inVictim, Vector3 inExplosionPos, out Vector3 outInpuls)
	{
		Vector3 vector = inVictim.ClosestPointOnBounds(inExplosionPos);
		float num = Vector3.Distance(vector, inExplosionPos);
		outInpuls = Vector3.zero;
		if (IsCollisionBetween(inExplosionPos, vector, inVictim))
		{
			return -1f;
		}
		float num2 = 1f - Mathf.Clamp01(num / damageRadius);
		float result = num2 * damage;
		outInpuls = inVictim.bounds.center - inExplosionPos;
		outInpuls.y = vector.y - inExplosionPos.y;
		outInpuls.Normalize();
		return result;
	}

	private bool IsCollisionBetween(Vector3 inFrom, Vector3 inTo, Collider inVictim)
	{
		Vector3 vector = inTo - inFrom;
		RaycastHit[] array = Physics.RaycastAll(inFrom - vector.normalized * 0.1f, vector.normalized, vector.magnitude);
		if (array.Length > 1)
		{
			Array.Sort(array, CollisionUtils.CompareHits);
		}
		RaycastHit[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			RaycastHit raycastHit = array2[i];
			if (!(raycastHit.collider == inVictim) && !raycastHit.collider.isTrigger && (noBlocking == null || !IgnoreInBlockingTest(raycastHit.collider.transform)))
			{
				return true;
			}
		}
		return false;
	}

	private bool IgnoreInBlockingTest(Transform inTransform)
	{
		Transform[] array = noBlocking;
		foreach (Transform transform in array)
		{
			if (transform == inTransform)
			{
				return true;
			}
		}
		return false;
	}

	private void PlayShakingCameraEffect()
	{
		if (!(GameCamera.Instance == null))
		{
			Vector3 position = base.transform.position;
			Vector3 position2 = GameCamera.Instance.transform.position;
			float num = Vector3.Distance(position, position2);
			if (!(num > shakingEffectDistance))
			{
				float inIntensity = (1f - num / shakingEffectDistance) * m_ShakingEffectScale;
				GameCamera.Instance.PlayShakingAnim(m_ShakingEffect, inIntensity);
			}
		}
	}

	private void GenerateWaveFX()
	{
		if (!(Camera.main == null))
		{
			float magnitude = (base.transform.position - Camera.main.transform.position).magnitude;
			float t = Mathf.Min(magnitude / m_WaveFXMaxWrldDist, 1f);
			MFExplosionPostFX.S_WaveParams waveParams = default(MFExplosionPostFX.S_WaveParams);
			waveParams.m_Amplitude = Mathf.Lerp(m_WaveFXAmplitudeMax, m_WaveFXAmplitudeMin, t);
			waveParams.m_Duration = m_WaveFXDuration;
			waveParams.m_Freq = m_WaveFXFreq;
			waveParams.m_Speed = m_WaveFXSpeed;
			waveParams.m_Radius = Mathf.Lerp(m_WaveFXRadiusMax, m_WaveFXRadiusMin, t);
			waveParams.m_Delay = m_WaveFXDelay;
			CamExplosionFXMgr.Instance.SpawnExplosionWaveFX(base.transform.position, waveParams, m_WaveFXDelay);
		}
	}
}
