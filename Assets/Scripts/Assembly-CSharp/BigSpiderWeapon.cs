using UnityEngine;

[AddComponentMenu("Enemies/BigSpider/Weapon")]
public class BigSpiderWeapon : MonoBehaviour
{
	public GameObject m_SpawnPos;

	public Projectile.InitSettingsInspector m_ProjectileSettings;

	private BigSpider m_Spider;

	public int m_RocketsBeforeReload = 5;

	private int m_RocketsCount;

	private AgentHuman m_Target;

	public GameObject m_Head;

	private Transform m_HeadTransform;

	public ParticleSystem m_LaunchEffect;

	private void Awake()
	{
	}

	private void Start()
	{
		m_Spider = GetComponent<BigSpider>();
		if (m_Spider == null)
		{
			Debug.LogWarning("Neccessary script missing - BigSpider");
		}
		m_Head = ((!(m_Head == null)) ? m_Head : base.gameObject);
		m_HeadTransform = m_Head.transform;
		m_Target = ((!(m_Target == null)) ? m_Target : Player.Instance.Owner);
		m_RocketsCount = 0;
	}

	private void Update()
	{
		if (Time.deltaTime > 0f)
		{
			Vector3 chestPosition = m_Target.ChestPosition;
			LookAtTarget(chestPosition);
		}
	}

	private void OnEnable()
	{
		m_RocketsCount = 0;
		if (!IsInvoking("LaunchProjectile"))
		{
			InvokeRepeating("LaunchProjectile", 2f, 2f);
		}
	}

	private void OnDisable()
	{
		CancelInvoke();
	}

	private void LookAtTarget(Vector3 inTargetPos)
	{
		Vector3 lookRotation = inTargetPos - m_HeadTransform.position;
		Quaternion inTo = default(Quaternion);
		inTo.SetLookRotation(lookRotation);
		m_HeadTransform.rotation = RotateToward(m_HeadTransform.rotation, inTo, 20f, Time.deltaTime);
		Debug.DrawLine(m_HeadTransform.position, inTargetPos, Color.red);
	}

	internal Quaternion RotateToward(Quaternion inFrom, Quaternion inTo, float inRotSpeed, float inTime)
	{
		float num = Quaternion.Angle(inFrom, inTo);
		float num2 = num / inRotSpeed;
		float t = ((num2 != 0f) ? Mathf.Clamp(inTime / num2, 0f, 1f) : 0f);
		return Quaternion.Slerp(inFrom, inTo, t);
	}

	private void LaunchProjectile()
	{
		if (IsReloading())
		{
			return;
		}
		if (m_RocketsCount <= 0)
		{
			Reload();
			m_RocketsCount = m_RocketsBeforeReload;
			return;
		}
		Projectile.InitSettings initSettings = new Projectile.InitSettings(m_ProjectileSettings);
		initSettings.IgnoreTransform = base.transform;
		ProjectileManager.Instance.SpawnProjectile(E_ProjectileType.BigSpiderRocket, m_SpawnPos.transform.position, m_SpawnPos.transform.forward, initSettings);
		m_RocketsCount--;
		PlayShotSound();
		if (m_LaunchEffect != null)
		{
			m_LaunchEffect.Play();
		}
	}

	private void Reload()
	{
		m_Spider.Reload();
	}

	private bool IsReloading()
	{
		return m_Spider.IsReloading();
	}

	private void PlayShotSound()
	{
		m_Spider.PlayShotSound();
	}
}
