using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SentinelWeapon : MonoBehaviour
{
	[Serializable]
	public class ShootSettings
	{
		public int m_BulletsInBurst = 40;

		public float m_FireRate = 0.1f;

		public float m_AfterBurstPause = 2f;

		public float m_Dispersion = 0.05f;

		internal int m_BulletsInBurstFired;

		internal float m_LastShootTime;

		internal float m_LastBurstTime = float.MinValue;

		public Vector3 vectorDispersion
		{
			get
			{
				return new Vector3(Random.Range(0f - m_Dispersion, m_Dispersion), Random.Range(0f - m_Dispersion, m_Dispersion), 0f);
			}
		}
	}

	[Serializable]
	public class ProjectileSettings
	{
		public E_ProjectileType m_Type = E_ProjectileType.Flyer;

		public float m_Speed = 50f;

		public float m_Damage = 5f;
	}

	private AgentHuman m_Target;

	public GameObject m_Weapon;

	public GameObject m_Barrel;

	public GameObject m_Muzzle;

	private Transform m_WeaponTransform;

	private Transform m_BarrelTransform;

	private bool m_IsFireing;

	private float m_DetectionDistance = 10f;

	private float m_FireDistance = 5f;

	public ShootSettings m_ShootSettings;

	public ProjectileSettings m_ProjectileSettings;

	private Projectile.InitSettings m_InitProjSettings;

	private Sentinel m_Sentinel;

	public float detectionDistancet
	{
		get
		{
			return m_DetectionDistance;
		}
		set
		{
			m_DetectionDistance = value;
		}
	}

	public float fireDistance
	{
		get
		{
			return m_FireDistance;
		}
		set
		{
			m_FireDistance = value;
		}
	}

	private Vector3 dirToTarget
	{
		get
		{
			return m_WeaponTransform.position - m_Target.ChestPosition;
		}
	}

	private void Start()
	{
		m_Weapon = ((!(m_Weapon == null)) ? m_Weapon : base.gameObject);
		m_WeaponTransform = m_Weapon.transform;
		m_Barrel = ((!(m_Barrel == null)) ? m_Barrel : base.gameObject);
		m_BarrelTransform = m_Barrel.transform;
		m_Target = ((!(m_Target == null)) ? m_Target : Player.Instance.Owner);
		m_Sentinel = GetComponent<Sentinel>();
		if (m_Sentinel == null)
		{
			Debug.LogError("Neccessary script missing - Sentinel");
		}
		m_InitProjSettings = new Projectile.InitSettings();
		m_InitProjSettings.Speed = m_ProjectileSettings.m_Speed;
		m_InitProjSettings.Damage = m_ProjectileSettings.m_Damage;
		m_InitProjSettings.IgnoreTransform = m_BarrelTransform;
	}

	private void Activate(SpawnPoint spawn)
	{
		HideMuzzle();
	}

	private void Deactivate()
	{
		m_ShootSettings.m_BulletsInBurstFired = 0;
		m_ShootSettings.m_LastShootTime = 0f;
		m_ShootSettings.m_LastBurstTime = float.MinValue;
		m_IsFireing = false;
		StopAllCoroutines();
		CancelInvoke();
		HideMuzzle();
	}

	private void Update()
	{
		if (m_Target == null || Time.deltaTime <= float.Epsilon)
		{
			return;
		}
		Vector3 chestPosition = m_Target.ChestPosition;
		float num = Vector3.Magnitude(dirToTarget);
		if (num < m_DetectionDistance)
		{
			LookAtTarget(chestPosition);
			if (num < m_FireDistance)
			{
				if (CanFire())
				{
					m_IsFireing = true;
					FireAtTarget(chestPosition);
					UpdatePropertisAfterShot();
				}
				else if (IsInBurstPauseMode())
				{
					m_IsFireing = false;
					HideMuzzle();
				}
			}
			else if (m_IsFireing)
			{
				m_IsFireing = false;
				HideMuzzle();
			}
		}
		else if (m_IsFireing)
		{
			m_IsFireing = false;
			HideMuzzle();
		}
	}

	private void OnDestroy()
	{
		HideMuzzle();
	}

	internal void LookAtTarget(Vector3 inTargetPos)
	{
		m_WeaponTransform.LookAt(inTargetPos);
		Debug.DrawLine(m_WeaponTransform.position, inTargetPos, Color.red);
	}

	internal void FireAtTarget(Vector3 inTargetPos)
	{
		SpawnProjectile();
		m_Sentinel.PlayFireSound();
		ShowMuzzle();
		Invoke("HideMuzzle", 0.05f);
	}

	internal void SpawnProjectile()
	{
		Vector3 position = m_BarrelTransform.position;
		Quaternion rotation = m_BarrelTransform.rotation;
		rotation.eulerAngles += m_ShootSettings.vectorDispersion;
		ProjectileSettings projectileSettings = m_ProjectileSettings;
		ProjectileManager.Instance.SpawnProjectile(projectileSettings.m_Type, position, rotation * Vector3.forward, m_InitProjSettings);
	}

	internal bool IsInBurstPauseMode()
	{
		ShootSettings shootSettings = m_ShootSettings;
		return Time.timeSinceLevelLoad - shootSettings.m_LastBurstTime < shootSettings.m_AfterBurstPause;
	}

	internal bool CanFire()
	{
		if (m_Target == null || !m_Target.IsAlive)
		{
			return false;
		}
		ShootSettings shootSettings = m_ShootSettings;
		if (Time.timeSinceLevelLoad - shootSettings.m_LastShootTime < shootSettings.m_FireRate)
		{
			return false;
		}
		if (shootSettings.m_BulletsInBurstFired > 0 && Time.timeSinceLevelLoad - shootSettings.m_LastShootTime > shootSettings.m_AfterBurstPause)
		{
			shootSettings.m_BulletsInBurstFired = 0;
		}
		if (shootSettings.m_BulletsInBurstFired >= shootSettings.m_BulletsInBurst)
		{
			shootSettings.m_LastBurstTime = Time.timeSinceLevelLoad;
			shootSettings.m_BulletsInBurstFired = 0;
			return false;
		}
		if (IsInBurstPauseMode())
		{
			return false;
		}
		GameObject gameObject = CollisionUtils.FirstCollisionOnRay(new Ray(m_BarrelTransform.position, m_BarrelTransform.forward), dirToTarget.magnitude, base.gameObject);
		if (gameObject != null && gameObject != m_Target.GameObject)
		{
			return false;
		}
		return true;
	}

	internal void UpdatePropertisAfterShot()
	{
		m_ShootSettings.m_BulletsInBurstFired++;
		m_ShootSettings.m_LastShootTime = Time.timeSinceLevelLoad;
	}

	internal void ShowMuzzle()
	{
		if ((bool)m_Muzzle && (bool)m_Muzzle.GetComponent<Renderer>())
		{
			Transform transform = m_Muzzle.transform;
			float num = 1f + Random.Range(-0.2f, 0.2f);
			transform.localScale = new Vector3(num, num, num);
			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, Random.Range(0, 50));
			m_Muzzle.GetComponent<Renderer>().enabled = true;
		}
	}

	internal void HideMuzzle()
	{
		if ((bool)m_Muzzle && (bool)m_Muzzle.GetComponent<Renderer>())
		{
			m_Muzzle.GetComponent<Renderer>().enabled = false;
		}
	}
}
