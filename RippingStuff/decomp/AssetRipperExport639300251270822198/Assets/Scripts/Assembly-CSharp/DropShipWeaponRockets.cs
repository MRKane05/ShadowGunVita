using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Enemies/DropShip/WeaponRockets")]
public class DropShipWeaponRockets : MonoBehaviour
{
	public Projectile.InitSettingsInspector m_RocketSettings;

	private Projectile.InitSettings m_RocketSettingsEx;

	public Projectile m_RocketPrefab;

	public Transform[] m_LeftRocketsHandles;

	public Transform[] m_RightRocketsHandles;

	public float m_RocketsFireRate = 0.5f;

	private Projectile[] m_LeftRockets;

	private Projectile[] m_RightRockets;

	private DropShip m_DropShipMain;

	private List<Projectile> ActiveProjectiles = new List<Projectile>();

	private void Awake()
	{
		InstatiateRockets();
		m_RocketSettingsEx = new Projectile.InitSettings(m_RocketSettings);
		m_DropShipMain = base.gameObject.GetFirstComponentUpward<DropShip>();
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (Game.Instance.GameState != E_GameState.Game || Time.deltaTime <= 0f)
		{
			return;
		}
		foreach (Projectile activeProjectile in ActiveProjectiles)
		{
			if (!activeProjectile.IsFinished())
			{
				activeProjectile.ProjectileUpdate();
			}
		}
	}

	private void FixedUpdate()
	{
		for (int i = 0; i < ActiveProjectiles.Count; i++)
		{
			if (ActiveProjectiles[i].IsFinished())
			{
				ActiveProjectiles.RemoveAt(i);
			}
		}
	}

	private void Activate(SpawnPoint spawn)
	{
		for (int num = m_LeftRockets.Length - 1; num >= 0; num--)
		{
			foreach (Transform item in m_LeftRockets[num].transform)
			{
				item.gameObject.SetActiveRecursively(false);
			}
		}
		for (int num2 = m_RightRockets.Length - 1; num2 >= 0; num2--)
		{
			foreach (Transform item2 in m_RightRockets[num2].transform)
			{
				item2.gameObject.SetActiveRecursively(false);
			}
		}
	}

	private void Deactivate()
	{
		ResetRockets();
		ActiveProjectiles.Clear();
	}

	internal void InstatiateRockets()
	{
		if (m_RocketPrefab == null)
		{
			Debug.LogError("Can't instatiate rockets. Prefab is not assigned");
			return;
		}
		if (m_LeftRocketsHandles.Length > 0)
		{
			m_LeftRockets = new Projectile[m_LeftRocketsHandles.Length];
			for (int num = m_LeftRockets.Length - 1; num >= 0; num--)
			{
				m_LeftRockets[num] = Object.Instantiate(m_RocketPrefab, m_LeftRocketsHandles[num].position, m_LeftRocketsHandles[num].rotation) as Projectile;
				m_LeftRockets[num].transform.parent = m_LeftRocketsHandles[num];
				foreach (Transform item in m_LeftRockets[num].transform)
				{
					item.gameObject.SetActiveRecursively(false);
				}
			}
		}
		if (m_RightRocketsHandles.Length <= 0)
		{
			return;
		}
		m_RightRockets = new Projectile[m_RightRocketsHandles.Length];
		for (int num2 = m_RightRockets.Length - 1; num2 >= 0; num2--)
		{
			m_RightRockets[num2] = Object.Instantiate(m_RocketPrefab, m_RightRocketsHandles[num2].position, m_RightRocketsHandles[num2].rotation) as Projectile;
			m_RightRockets[num2].transform.parent = m_RightRocketsHandles[num2];
			foreach (Transform item2 in m_RightRockets[num2].transform)
			{
				item2.gameObject.SetActiveRecursively(false);
			}
		}
	}

	public IEnumerator LauncheRockets()
	{
		ActiveProjectiles.Clear();
		int numOfRockets = Mathf.Max(m_RightRockets.Length - 1, m_LeftRockets.Length - 1);
		for (int i = numOfRockets; i >= 0; i--)
		{
			if (m_LeftRockets[i] != null)
			{
				ActiveProjectiles.Add(m_LeftRockets[i]);
				m_LeftRockets[i].ProjectileInit(Vector3.zero, Vector3.zero, m_RocketSettingsEx);
				PlayLaunchSound();
				yield return new WaitForSeconds(m_RocketsFireRate);
			}
			if (m_RightRockets[i] != null)
			{
				ActiveProjectiles.Add(m_RightRockets[i]);
				m_RightRockets[i].ProjectileInit(Vector3.zero, Vector3.zero, m_RocketSettingsEx);
				PlayLaunchSound();
				yield return new WaitForSeconds(m_RocketsFireRate);
			}
		}
	}

	public void ResetRockets()
	{
		for (int num = m_LeftRockets.Length - 1; num >= 0; num--)
		{
			m_LeftRockets[num].ProjectileDeinit();
		}
		for (int num2 = m_RightRockets.Length - 1; num2 >= 0; num2--)
		{
			m_RightRockets[num2].ProjectileDeinit();
		}
	}

	internal void PlayLaunchSound()
	{
		m_DropShipMain.Audio_PlayRocketLaunchSound();
	}
}
