using System.Collections;
using UnityEngine;

[AddComponentMenu("Enemies/DropShip/Weapons")]
public class DropShipWeapons : MonoBehaviour
{
	public Projectile.InitSettingsInspector m_GunSettings;

	public Transform m_GunRoot;

	public Transform[] m_GunShoots;

	public DropShipWeaponGun[] m_GunsComp;

	private int m_CurrentGun = -1;

	public int m_ProjectilesInBurst = 20;

	public float m_PerProjectileWaitTime = 0.1f;

	private PredictPosition m_TargetPosPredictor = new PredictPosition(3);

	public float m_PredictionCoef = 1f;

	private DropShip m_DropShipMain;

	public DropShipWeaponRockets m_RocketsComp;

	private AgentHuman m_Target;

	private Vector3 m_LastTargetPos;

	private bool isFireing { get; set; }

	private void Awake()
	{
		m_DropShipMain = base.gameObject.GetFirstComponentUpward<DropShip>();
	}

	private void Start()
	{
		m_Target = Player.Instance.Owner;
		DropShipWeaponGun[] gunsComp = m_GunsComp;
		foreach (DropShipWeaponGun dropShipWeaponGun in gunsComp)
		{
			dropShipWeaponGun.SetupParentInitObject(this);
		}
	}

	private void Update()
	{
		if (m_Target.IsAlive)
		{
			Vector3 chestPosition = m_Target.ChestPosition;
			if (m_PredictionCoef > 0f)
			{
				float magnitude = (m_Target.ChestPosition - m_GunRoot.position).magnitude;
				float inDeltaTime = magnitude / m_GunSettings.Speed;
				m_TargetPosPredictor.AddSample(chestPosition);
				chestPosition += m_PredictionCoef * m_TargetPosPredictor.EstimatedMove(inDeltaTime);
			}
			m_LastTargetPos = chestPosition;
			Gun_LookAtTarget(chestPosition);
		}
		else if (isFireing)
		{
			StopFireing();
		}
	}

	private void OnEnable()
	{
		isFireing = false;
	}

	private void OnDisable()
	{
		StopFireing();
	}

	public void FireGun()
	{
		if (!isFireing)
		{
			StartCoroutine(FireGun_Corutine());
			isFireing = true;
		}
	}

	public void FireRockets2()
	{
		if (!isFireing && (bool)m_RocketsComp)
		{
			StartCoroutine(FireRockets2_Corutine());
			isFireing = true;
		}
	}

	public void ReloadRockets2()
	{
		if ((bool)m_RocketsComp)
		{
			m_RocketsComp.ResetRockets();
		}
	}

	public void StopFireing()
	{
		isFireing = false;
		StopAllCoroutines();
		CancelInvoke();
	}

	private IEnumerator FireRockets2_Corutine()
	{
		yield return StartCoroutine(m_RocketsComp.LauncheRockets());
		isFireing = false;
	}

	private IEnumerator FireGun_Corutine()
	{
		for (int projCnt = 0; projCnt < m_ProjectilesInBurst; projCnt++)
		{
			yield return new WaitForSeconds(m_PerProjectileWaitTime);
			GunFire();
		}
		isFireing = false;
	}

	internal void Gun_LookAtTarget(Vector3 inTargetPos)
	{
		m_GunRoot.LookAt(inTargetPos);
		Debug.DrawLine(m_GunRoot.position, inTargetPos, Color.blue);
	}

	internal void GunFire()
	{
		m_CurrentGun = ++m_CurrentGun % m_GunShoots.Length;
		Projectile.InitSettings inSettings = new Projectile.InitSettings(m_GunSettings);
		m_GunsComp[m_CurrentGun].Fire(inSettings, m_LastTargetPos);
		m_DropShipMain.Audio_PlayGunShotSound();
	}
}
