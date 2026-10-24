using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Weapons/TestProjectiles")]
public class TestProjectiles : MonoBehaviour
{
	public GameObject m_ProjectilePrefab;

	public E_ProjectileType m_ProjectileType;

	public bool m_UseProjectileType = true;

	public GameObject m_SpawnPos;

	public Projectile.InitSettingsInspector m_ProjectileSettingsEx;

	public float m_LaunchRepeatTime = 2f;

	private List<Projectile> ActiveProjectiles = new List<Projectile>();

	private void LaunchProjectile()
	{
		Projectile.InitSettings initSettings = new Projectile.InitSettings(m_ProjectileSettingsEx);
		initSettings.IgnoreTransform = base.transform;
		if (m_UseProjectileType)
		{
			ProjectileManager.Instance.SpawnProjectile(m_ProjectileType, m_SpawnPos.transform.position, m_SpawnPos.transform.forward, initSettings);
			return;
		}
		GameObject gameObject = Object.Instantiate(m_ProjectilePrefab, m_SpawnPos.transform.position, m_SpawnPos.transform.rotation) as GameObject;
		Projectile component = gameObject.GetComponent<Projectile>();
		component.ProjectileInit(m_SpawnPos.transform.position, m_SpawnPos.transform.forward.normalized, initSettings);
		ActiveProjectiles.Add(component);
	}

	private void Awake()
	{
		InvokeRepeating("LaunchProjectile", m_LaunchRepeatTime, m_LaunchRepeatTime);
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
				ActiveProjectiles[i].ProjectileDeinit();
				Object.DestroyObject(ActiveProjectiles[i], 0.1f);
				ActiveProjectiles.RemoveAt(i--);
			}
		}
	}
}
