using UnityEngine;

public abstract class MongoWeaponBase : MonoBehaviour
{
	public E_ProjectileType m_ProjectileType;

	public Projectile.InitSettingsInspector m_ProjectileSettingsEx;

	public Transform m_Transform;

	public GameObject m_Muzzle;

	private Vector3 m_MuzzleInitScale;

	public ParticleSystem m_LaunchEffect;

	public AudioClip m_LaunchSound;

	protected MongoWeapons m_ParentWeaponObject;

	private void Reset()
	{
		m_Transform = base.transform;
	}

	private void Awake()
	{
		m_Transform = base.transform;
		if (m_Muzzle != null)
		{
			m_MuzzleInitScale = m_Muzzle.transform.localScale;
			m_Muzzle.GetComponent<Renderer>().enabled = false;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public virtual void WeaponReset()
	{
		HideMuzzle();
		CancelInvoke();
	}

	public void SetupParentInitObject(MongoWeapons inParentWeaponObject)
	{
		m_ParentWeaponObject = inParentWeaponObject;
	}

	public virtual void Fire()
	{
		LaunchProjectile();
		PlayLaunchSound();
		PlayLaunchEffect();
	}

	internal void LaunchProjectile()
	{
		Projectile.InitSettings initSettings = new Projectile.InitSettings(m_ProjectileSettingsEx);
		initSettings.IgnoreTransform = m_Transform;
		ProjectileManager.Instance.SpawnProjectile(m_ProjectileType, m_Transform.position, m_Transform.forward, initSettings);
	}

	internal void PlayLaunchSound()
	{
		if (m_LaunchSound != null && m_ParentWeaponObject != null)
		{
			m_ParentWeaponObject.PlaySound(m_LaunchSound, base.transform.position);
		}
	}

	internal void PlayLaunchEffect()
	{
		if (m_LaunchEffect != null)
		{
			m_LaunchEffect.Play();
		}
		else if (m_Muzzle != null)
		{
			ShowMuzzle();
			Invoke("HideMuzzle", 0.05f);
		}
	}

	internal void ShowMuzzle()
	{
		if ((bool)m_Muzzle)
		{
			Transform transform = m_Muzzle.transform;
			float num = 1f + Random.Range(-0.2f, 0.2f);
			transform.localScale = m_MuzzleInitScale * num;
			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, Random.Range(0, 50));
			m_Muzzle.GetComponent<Renderer>().enabled = true;
		}
	}

	internal void HideMuzzle()
	{
		if ((bool)m_Muzzle)
		{
			m_Muzzle.GetComponent<Renderer>().enabled = false;
		}
	}
}
