using System.Collections;
using UnityEngine;

[AddComponentMenu("Enemies/LightTurret/Weapon")]
public class LightTurretWeapon : MonoBehaviour
{
	public Transform m_SpawnPos;

	public Projectile.InitSettingsInspector m_ProjectileSettings;

	private LightTurret m_Turret;

	public int m_ProjectilesInBurst = 40;

	public float m_FireRate = 0.05f;

	private AgentHuman m_Target;

	public GameObject m_Head;

	private Transform m_HeadTransform;

	public ParticleSystem m_LaunchEffect;

	private void Awake()
	{
		m_Turret = GetComponent<LightTurret>();
		if (m_Turret == null)
		{
			Debug.LogWarning("Neccessary script missing - BigSpider");
		}
		m_Head = ((!(m_Head == null)) ? m_Head : base.gameObject);
		m_HeadTransform = m_Head.transform;
		m_Target = ((!(m_Target == null)) ? m_Target : Player.Instance.Owner);
	}

	private void Start()
	{
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
		StartCoroutine(Update_Corutine());
	}

	private void OnDisable()
	{
		StopAllCoroutines();
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

	private IEnumerator Update_Corutine()
	{
		while (true)
		{
			yield return StartCoroutine(Reload_Corutine());
			yield return StartCoroutine(Fireing_Corutine());
		}
	}

	private IEnumerator Fireing_Corutine()
	{
		for (int projCnt = 0; projCnt < m_ProjectilesInBurst; projCnt++)
		{
			yield return new WaitForSeconds(m_FireRate);
			LauncheProjectile();
		}
	}

	private IEnumerator Reload_Corutine()
	{
		Reload();
		yield return 0;
		while (IsReloading())
		{
			yield return new WaitForSeconds(0.2f);
		}
	}

	private void LauncheProjectile()
	{
		Vector3 chestPosition = m_Target.ChestPosition;
		m_SpawnPos.LookAt(chestPosition);
		Projectile.InitSettings initSettings = new Projectile.InitSettings(m_ProjectileSettings);
		initSettings.IgnoreTransform = base.transform;
		ProjectileManager.Instance.SpawnProjectile(E_ProjectileType.Turret, m_SpawnPos.position, m_SpawnPos.forward, initSettings);
		PlayShotSound();
		if (m_LaunchEffect != null)
		{
			m_LaunchEffect.Play();
		}
	}

	private void Reload()
	{
		m_Turret.Reload();
	}

	private bool IsReloading()
	{
		return m_Turret.IsReloading();
	}

	private void PlayShotSound()
	{
		m_Turret.PlayShotSound();
	}
}
