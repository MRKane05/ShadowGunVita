using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[AddComponentMenu("Enemies/Mongo/Weapons")]
public class MongoWeapons : MonoBehaviour
{
	[Serializable]
	public class ShootSettings
	{
		public int m_BurstCount = 3;

		public int m_BulletsInBurst = 40;

		public float m_FireRate = 0.1f;

		public float m_AfterBurstPause = 2f;
	}

	[Serializable]
	public class GrenadesSettings : ShootSettings
	{
		public float m_MinDispersion = 3f;

		public float m_MaxDispersion = 6f;

		public Vector3 groundVectorDispersion
		{
			get
			{
				float num = Random.Range(m_MinDispersion, m_MaxDispersion);
				float f = Random.Range(0f, (float)Math.PI * 2f);
				return new Vector3(num * Mathf.Sin(f), 0f, num * Mathf.Cos(f));
			}
		}
	}

	private MongoMain m_MongoMain;

	public MongoWeaponBase m_GrenadeLeftOut;

	public MongoWeaponBase m_GrenadeLeftIn;

	public MongoWeaponBase m_GrenadeRightIn;

	public MongoWeaponBase m_GrenadeRightOut;

	public MongoWeaponBase m_Gun;

	public MongoWeaponBase m_RocketL;

	public MongoWeaponBase m_RocketR;

	private AgentHuman m_Target;

	public Transform m_HeadTransform;

	public GrenadesSettings m_GrenadesSettings;

	public ShootSettings m_GunSettings;

	public ShootSettings m_RocketSettings;

	public float m_FireGrenadesWaitTime = 2f;

	public float m_FireGunWaitTime = 2f;

	public float m_FireRocketsWaitTime = 2f;

	private bool isFireing { get; set; }

	private void Reset()
	{
		m_GrenadesSettings.m_BurstCount = 2;
		m_GrenadesSettings.m_BulletsInBurst = 4;
		m_GrenadesSettings.m_FireRate = 0.5f;
		m_GrenadesSettings.m_AfterBurstPause = 5f;
		m_GunSettings.m_BurstCount = 3;
		m_GunSettings.m_BulletsInBurst = 40;
		m_GunSettings.m_FireRate = 0.1f;
		m_GunSettings.m_AfterBurstPause = 2f;
		m_RocketSettings.m_BurstCount = 2;
		m_RocketSettings.m_BulletsInBurst = 5;
		m_RocketSettings.m_FireRate = 2f;
		m_RocketSettings.m_AfterBurstPause = 5f;
	}

	private void WeaponReset()
	{
		StopFireing();
		if ((bool)m_GrenadeLeftOut)
		{
			m_GrenadeLeftOut.WeaponReset();
		}
		if ((bool)m_GrenadeLeftIn)
		{
			m_GrenadeLeftIn.WeaponReset();
		}
		if ((bool)m_GrenadeRightIn)
		{
			m_GrenadeRightIn.WeaponReset();
		}
		if ((bool)m_GrenadeRightOut)
		{
			m_GrenadeRightOut.WeaponReset();
		}
		if ((bool)m_Gun)
		{
			m_Gun.WeaponReset();
		}
		if ((bool)m_RocketL)
		{
			m_RocketL.WeaponReset();
		}
		if ((bool)m_RocketR)
		{
			m_RocketR.WeaponReset();
		}
	}

	private void Awake()
	{
		m_MongoMain = GetComponent<MongoMain>();
	}

	private void Start()
	{
		m_GrenadeLeftOut.SetupParentInitObject(this);
		m_GrenadeLeftIn.SetupParentInitObject(this);
		m_GrenadeRightIn.SetupParentInitObject(this);
		m_GrenadeRightOut.SetupParentInitObject(this);
		m_Gun.SetupParentInitObject(this);
		m_RocketL.SetupParentInitObject(this);
		m_RocketR.SetupParentInitObject(this);
		if (m_HeadTransform == null)
		{
			Debug.LogError("MongoWeapons.Awake - m_HeadTransform was not set");
		}
		m_Target = ((!(m_Target == null)) ? m_Target : Player.Instance.Owner);
	}

	private void LateUpdate()
	{
		if (Time.deltaTime > 0f)
		{
			Vector3 chestPosition = m_Target.ChestPosition;
			LookAtTarget(chestPosition);
		}
	}

	private void OnEnable()
	{
		isFireing = false;
	}

	private void OnDisable()
	{
		WeaponReset();
	}

	public void Phase1_StartFire()
	{
		if (!isFireing)
		{
			StartCoroutine(Phase1_Corutine());
			isFireing = true;
		}
	}

	public void Phase2_StartFire()
	{
		if (!isFireing)
		{
			StartCoroutine(Phase2_Corutine());
			isFireing = true;
		}
	}

	public IEnumerator Phase1_StartFire_Corutine()
	{
		if (!isFireing)
		{
			isFireing = true;
			return Phase1_Corutine();
		}
		return null;
	}

	public IEnumerator Phase2_StartFire_Corutine()
	{
		if (!isFireing)
		{
			isFireing = true;
			return Phase2_Corutine();
		}
		return null;
	}

	public bool IsFireing()
	{
		return isFireing;
	}

	public void StopFireing()
	{
		isFireing = false;
		StopAllCoroutines();
		CancelInvoke();
	}

	private IEnumerator Phase1_Corutine()
	{
		yield return StartCoroutine(LaunchGrenades(m_GrenadesSettings.m_BurstCount, m_GrenadesSettings.m_FireRate, m_GrenadesSettings.m_AfterBurstPause, true));
		yield return new WaitForSeconds(3.5f);
		yield return StartCoroutine(FireFromGun(m_GunSettings.m_BurstCount, m_GunSettings.m_BulletsInBurst, m_GunSettings.m_FireRate, m_GunSettings.m_AfterBurstPause));
		isFireing = false;
	}

	private IEnumerator Phase2_Corutine()
	{
		yield return StartCoroutine(FireFromGun(m_GunSettings.m_BurstCount, m_GunSettings.m_BulletsInBurst, m_GunSettings.m_FireRate, m_GunSettings.m_AfterBurstPause));
		yield return StartCoroutine(FireRocket(m_RocketSettings.m_BurstCount, m_RocketSettings.m_BulletsInBurst, m_RocketSettings.m_FireRate, m_RocketSettings.m_AfterBurstPause));
		isFireing = false;
	}

	private IEnumerator FireFromGun(int inBurstCount, int inProjectilesInBurst, float inPerProjectileWaitTime, float inPerBurstWaitTime)
	{
		m_MongoMain.GunFireStart();
		yield return new WaitForSeconds(m_FireGunWaitTime);
		for (int i = 0; i < inBurstCount; i++)
		{
			if (i != 0)
			{
				yield return new WaitForSeconds(inPerBurstWaitTime);
			}
			for (int projCnt = 0; projCnt < inProjectilesInBurst; projCnt++)
			{
				yield return new WaitForSeconds(inPerProjectileWaitTime);
				GunFire();
			}
		}
	}

	private IEnumerator FireRocket(int inBurstCount, int inProjectilesInBurst, float inPerProjectileWaitTime, float inPerBurstWaitTime)
	{
		m_MongoMain.RocketLaunchStart();
		yield return new WaitForSeconds(m_FireRocketsWaitTime);
		for (int i = 0; i < inBurstCount; i++)
		{
			if (i != 0)
			{
				yield return new WaitForSeconds(inPerBurstWaitTime);
			}
			for (int projCnt = 0; projCnt < inProjectilesInBurst; projCnt++)
			{
				yield return new WaitForSeconds(inPerProjectileWaitTime);
				if (projCnt % 2 == 0)
				{
					m_RocketL.Fire();
				}
				else
				{
					m_RocketR.Fire();
				}
			}
		}
	}

	private IEnumerator LaunchGrenades(int inBurstCount, float inPerProjectileWaitTime, float inPerBurstWaitTime, bool inPingPong)
	{
		m_MongoMain.GrenadeLaunchStart();
		yield return new WaitForSeconds(m_FireGrenadesWaitTime);
		for (int i = 0; i < inBurstCount; i++)
		{
			if (i != 0)
			{
				yield return new WaitForSeconds(inPerBurstWaitTime);
			}
			if (inPingPong && i % 2 == 1)
			{
				m_GrenadeRightOut.Fire();
				yield return new WaitForSeconds(inPerProjectileWaitTime);
				m_GrenadeRightIn.Fire();
				yield return new WaitForSeconds(inPerProjectileWaitTime);
				m_GrenadeLeftIn.Fire();
				yield return new WaitForSeconds(inPerProjectileWaitTime);
				m_GrenadeLeftOut.Fire();
			}
			else
			{
				m_GrenadeLeftOut.Fire();
				yield return new WaitForSeconds(inPerProjectileWaitTime);
				m_GrenadeLeftIn.Fire();
				yield return new WaitForSeconds(inPerProjectileWaitTime);
				m_GrenadeRightIn.Fire();
				yield return new WaitForSeconds(inPerProjectileWaitTime);
				m_GrenadeRightOut.Fire();
			}
		}
	}

	internal void GunFire()
	{
		m_Gun.Fire();
	}

	public void PlaySound(AudioClip inLaunchSound, Vector3 inPosition)
	{
		if (base.GetComponent<AudioSource>() != null)
		{
			base.GetComponent<AudioSource>().PlayOneShot(inLaunchSound);
		}
	}

	private void LookAtTarget(Vector3 inTargetPos)
	{
		if (m_HeadTransform != null)
		{
			Vector3 lookRotation = inTargetPos - m_HeadTransform.position;
			Quaternion inTo = default(Quaternion);
			inTo.SetLookRotation(lookRotation);
			m_HeadTransform.rotation = RotateToward(m_HeadTransform.rotation, inTo, 60f, Time.deltaTime);
			Debug.DrawLine(m_HeadTransform.position, inTargetPos, Color.red);
		}
	}

	internal Quaternion RotateToward(Quaternion inFrom, Quaternion inTo, float inRotSpeed, float inTime)
	{
		float num = Quaternion.Angle(inFrom, inTo);
		float num2 = num / inRotSpeed;
		float t = ((num2 != 0f) ? Mathf.Clamp(inTime / num2, 0f, 1f) : 0f);
		return Quaternion.Slerp(inFrom, inTo, t);
	}

	internal IEnumerator WeaponTest()
	{
		yield return 0;
	}
}
