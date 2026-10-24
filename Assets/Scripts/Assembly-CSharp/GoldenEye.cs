using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class GoldenEye : MonoBehaviour
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
		public E_WeaponType m_Type;

		public float m_Speed = 50f;

		public float m_Damage = 5f;
	}

	private AgentHuman m_Target;

	public GameObject m_Head;

	public GameObject m_Barrel;

	public GameObject m_Muzzle;

	private Transform m_HeadTransform;

	private Transform m_BarrelTransform;

	private bool m_IsFireing;

	public float m_FireDistance = 5f;

	public ShootSettings m_ShootSettings;

	public ProjectileSettings m_ProjectileSettings;

	public Transform[] m_SpawnPoints = new Transform[4];

	private Vector3 dirToTarget
	{
		get
		{
			return m_HeadTransform.position - m_Target.ChestPosition;
		}
	}

	private void Start()
	{
		m_Head = ((!(m_Head == null)) ? m_Head : base.gameObject);
		m_HeadTransform = m_Head.transform;
		m_Barrel = ((!(m_Barrel == null)) ? m_Barrel : base.gameObject);
		m_BarrelTransform = m_Barrel.transform;
		m_Target = ((!(m_Target == null)) ? m_Target : Player.Instance.Owner);
	}

	private void Update()
	{
		if (m_Target == null)
		{
			return;
		}
		Vector3 inTargetPos = m_Target.ChestPosition - new Vector3(0f, 3f, 0f);
		float num = Vector3.Magnitude(dirToTarget);
		LookAtTarget(inTargetPos);
		if (num < m_FireDistance)
		{
			Debug.Log("Mark 1");
			if (CanFire())
			{
				Debug.Log("Mark 2");
				m_IsFireing = true;
				FireAtTarget(inTargetPos);
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

	internal void LookAtTarget(Vector3 inTargetPos)
	{
		m_HeadTransform.LookAt(inTargetPos);
		Debug.DrawLine(m_HeadTransform.position, inTargetPos, Color.red);
	}

	internal void FireAtTarget(Vector3 inTargetPos)
	{
		SpawnProjectile();
		ShowMuzzle();
		Invoke("HideMuzzle", 0.05f);
	}

	internal void SpawnProjectile()
	{
		Debug.Log("Mark 3");
		Vector3 position = m_BarrelTransform.position;
		Quaternion rotation = m_BarrelTransform.rotation;
		rotation.eulerAngles += m_ShootSettings.vectorDispersion;
		ProjectileManager.Instance.SpawnProjectile(E_ProjectileType.SMG, position, rotation * Vector3.forward, new Projectile.InitSettings());
	}

	internal bool IsInBurstPauseMode()
	{
		ShootSettings shootSettings = m_ShootSettings;
		return Time.realtimeSinceStartup - shootSettings.m_LastBurstTime < shootSettings.m_AfterBurstPause;
	}

	internal bool CanFire()
	{
		ShootSettings shootSettings = m_ShootSettings;
		if (Time.realtimeSinceStartup - shootSettings.m_LastShootTime < shootSettings.m_FireRate)
		{
			return false;
		}
		if (shootSettings.m_BulletsInBurstFired > 0 && Time.realtimeSinceStartup - shootSettings.m_LastShootTime > shootSettings.m_AfterBurstPause)
		{
			shootSettings.m_BulletsInBurstFired = 0;
		}
		if (shootSettings.m_BulletsInBurstFired >= shootSettings.m_BulletsInBurst)
		{
			shootSettings.m_LastBurstTime = Time.realtimeSinceStartup;
			shootSettings.m_BulletsInBurstFired = 0;
			return false;
		}
		if (IsInBurstPauseMode())
		{
			return false;
		}
		return true;
	}

	internal void UpdatePropertisAfterShot()
	{
		m_ShootSettings.m_BulletsInBurstFired++;
		m_ShootSettings.m_LastShootTime = Time.realtimeSinceStartup;
	}

	internal void ShowMuzzle()
	{
		if ((bool)m_Muzzle)
		{
			Transform transform = m_Muzzle.transform;
			float num = 1f + Random.Range(-0.2f, 0.2f);
			transform.localScale = new Vector3(num, num, num);
			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, Random.Range(0, 50));
			m_Muzzle.active = true;
		}
	}

	internal void HideMuzzle()
	{
		if ((bool)m_Muzzle)
		{
			m_Muzzle.active = false;
		}
	}
}
