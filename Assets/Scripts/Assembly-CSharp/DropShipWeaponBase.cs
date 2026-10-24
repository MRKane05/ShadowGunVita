using UnityEngine;

public abstract class DropShipWeaponBase : MonoBehaviour
{
	protected Transform m_Transform;

	public GameObject m_Muzzle;

	private Vector3 m_MuzzleInitScale;

	public ParticleSystem m_LaunchEffect;

	protected DropShipWeapons m_ParentWeaponObject;

	public float m_Dispersion = 0.5f;

	public Vector3 vectorDispersion
	{
		get
		{
			return new Vector3(Random.Range(0f - m_Dispersion, m_Dispersion), Random.Range(0f - m_Dispersion, m_Dispersion), 0f);
		}
	}

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

	public void SetupParentInitObject(DropShipWeapons inParentWeaponObject)
	{
		m_ParentWeaponObject = inParentWeaponObject;
	}

	public virtual void Fire(Projectile.InitSettings inSettings, Vector3 inTargetPos)
	{
		LaunchProjectile(inSettings, inTargetPos);
		PlayLaunchEffect();
	}

	internal void LaunchProjectile(Projectile.InitSettings inSettings, Vector3 inTargetPos)
	{
		m_Transform.LookAt(inTargetPos);
		Vector3 vector = vectorDispersion;
		m_Transform.eulerAngles += vector;
		inSettings.IgnoreTransform = m_Transform;
		ProjectileManager.Instance.SpawnProjectile(E_ProjectileType.SMG, m_Transform.position, m_Transform.forward, inSettings);
	}

	internal void PlayLaunchSound()
	{
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
