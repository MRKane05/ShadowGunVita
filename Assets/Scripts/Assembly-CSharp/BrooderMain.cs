#define DEBUG
using System.Collections;
using UnityEngine;

public class BrooderMain : MonoBehaviour, IGameZoneControledObject, IHitZoneOwner
{
	public float m_HitPoints = 500f;

	private float m_DefaultHitPoints;

	private Agent m_Child;

	private SpawnPointEnemy m_SpawnPoint;

	private GameZone m_GameZone;

	public Animation m_Animation;

	private AudioSource m_Audio;

	private BrooderCamera m_Camera;

	public GameEvent m_DeathEvent;

	private void Awake()
	{
		m_SpawnPoint = base.gameObject.GetComponentInChildren<SpawnPointEnemy>();
		m_GameZone = base.gameObject.GetFirstComponentUpward<GameZone>();
		m_Camera = base.gameObject.GetComponentInChildren<BrooderCamera>();
		m_Audio = base.GetComponent<AudioSource>();
		DebugUtils.Assert(m_SpawnPoint != null);
		DebugUtils.Assert(m_GameZone != null);
		DebugUtils.Assert(m_Camera != null);
		DebugUtils.Assert(m_Animation != null);
		DebugUtils.Assert(m_Audio != null);
		m_Camera.enabled = false;
		m_DefaultHitPoints = m_HitPoints;
		m_GameZone.RegisterControllableObject(this);
	}

	private void Start()
	{
		if (m_DeathEvent.Name != string.Empty && !GameBlackboard.Instance.GameEvents.Exist(m_DeathEvent.Name))
		{
			Debug.LogError("BrooderMain :: Death event is not registered in GlobalBlackboard");
			Debug.LogError("BrooderMain :: Please add event " + m_DeathEvent.Name + " into Mission.GlobalBlackboard");
			m_DeathEvent = null;
		}
	}

	private void Update()
	{
	}

	public void Enable()
	{
		m_Animation.enabled = true;
		m_Animation.Stop();
		m_HitPoints = m_DefaultHitPoints;
		m_Camera.enabled = true;
		StartCoroutine(SpawnEnemy_Corutine());
	}

	public void Reset()
	{
		Disable();
		m_Camera.Reset();
		Enable();
	}

	public void Disable()
	{
		m_Camera.enabled = false;
		StopAllCoroutines();
		m_Audio.Stop();
		m_Animation.Stop();
	}

	private IEnumerator SpawnEnemy_Corutine()
	{
		while (m_HitPoints > 0f)
		{
			if ((m_Child == null || !m_Child.IsAlive) && m_Camera.IsEnemyVisible())
			{
				yield return new WaitForSeconds(2f);
				m_Animation.Play();
				m_Audio.Play();
				yield return new WaitForSeconds(0.5f);
				Spawn(m_SpawnPoint);
				yield return new WaitForSeconds(0.7f);
				m_Audio.Play();
			}
			else
			{
				yield return new WaitForSeconds(0.5f);
			}
		}
	}

	private void Spawn(SpawnPointEnemy spawnpoint)
	{
		if (spawnpoint.EnemyType != E_EnemyType.None)
		{
			GameObject agendFromCache = Mission.Instance.GetAgendFromCache(spawnpoint.EnemyType, spawnpoint.Transform);
			if (agendFromCache == null)
			{
				Debug.LogError(string.Concat(base.name, " No enemy in cache ", spawnpoint.EnemyType, " on ", spawnpoint.name));
			}
			else
			{
				agendFromCache.SendMessage("Activate", spawnpoint);
				m_Child = agendFromCache.GetComponent<Agent>();
				m_GameZone.AddEnemy(m_Child);
			}
		}
	}

	public void OnProjectileHit(Projectile inProjectile, HitZone inHitZone)
	{
		if (m_HitPoints <= 0f)
		{
			return;
		}
		float num = inProjectile.Damage * inHitZone.DamageModifier;
		m_HitPoints -= num;
		if (m_HitPoints < 0f)
		{
			StopAllCoroutines();
			m_Animation.enabled = false;
			m_Camera.Destroy();
			if (m_DeathEvent != null)
			{
				Mission.Instance.SendGameEvent(m_DeathEvent);
			}
		}
	}

	public void OnReceiveRangeDamage(float inDamage, HitZone inHitZone)
	{
		if (m_HitPoints <= 0f)
		{
			return;
		}
		float num = inDamage * inHitZone.DamageModifier;
		m_HitPoints -= num;
		if (m_HitPoints < 0f)
		{
			StopAllCoroutines();
			m_Animation.enabled = false;
			m_Camera.Destroy();
			if (m_DeathEvent != null)
			{
				Mission.Instance.SendGameEvent(m_DeathEvent);
			}
		}
	}
}
