#define DEBUG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReSpawnZone : MonoBehaviour, IGameZoneControledObject
{
	public enum E_SpawnOrder
	{
		InOrder = 0,
		Random = 1
	}

	private Agent m_Child;

	public List<SpawnPointEnemy> m_SpawnPoints = new List<SpawnPointEnemy>();

	public float m_RespawnDelay = 0.5f;

	public E_SpawnOrder m_SpawnOrder;

	private int m_CurrentSpawnIndex = -1;

	private GameZone m_GameZone;

	public OnGameEvent m_OnEvent;

	private bool m_IsActive;

	private void Awake()
	{
		m_GameZone = base.gameObject.GetFirstComponentUpward<GameZone>();
		DebugUtils.Assert(m_GameZone != null);
		m_GameZone.RegisterControllableObject(this);
		for (int num = m_SpawnPoints.Count - 1; num >= 0; num--)
		{
			if (m_SpawnPoints[num] == null && m_SpawnPoints[num].EnemyType == E_EnemyType.None)
			{
				m_SpawnPoints.RemoveAt(1);
			}
		}
	}

	private void Start()
	{
		if (m_SpawnPoints.Count > 0 && m_OnEvent.Name != string.Empty)
		{
			if (!GameBlackboard.Instance.GameEvents.Exist(m_OnEvent.Name))
			{
				Debug.LogError("ReSpawnZone :: On Event is not registered in GlobalBlackboard");
			}
			else
			{
				GameBlackboard.Instance.GameEvents.AddEventChangeHandler(m_OnEvent.Name, EventHandler);
			}
		}
	}

	public void Enable()
	{
		m_CurrentSpawnIndex = -1;
		UpdateActivityState();
	}

	public void Reset()
	{
		Disable();
		Enable();
	}

	public void Disable()
	{
		StopAllCoroutines();
		m_IsActive = false;
	}

	public void EventHandler(string name, GameEvents.E_State state)
	{
		UpdateActivityState();
	}

	private void UpdateActivityState()
	{
		if (!base.enabled || !base.gameObject.active)
		{
			return;
		}
		bool flag = GameBlackboard.Instance.GameEvents.GetState(m_OnEvent.Name) == m_OnEvent.State;
		if (flag != m_IsActive)
		{
			if (flag)
			{
				StartCoroutine(Wait_Corutine());
			}
			else
			{
				StopAllCoroutines();
			}
			m_IsActive = flag;
		}
	}

	private IEnumerator SpawnEnemy_Corutine()
	{
		yield return new WaitForSeconds(m_RespawnDelay);
		if (m_SpawnOrder == E_SpawnOrder.InOrder)
		{
			m_CurrentSpawnIndex = ++m_CurrentSpawnIndex % m_SpawnPoints.Count;
			Spawn(m_SpawnPoints[m_CurrentSpawnIndex]);
		}
		else if (m_SpawnOrder == E_SpawnOrder.Random)
		{
			Spawn(m_SpawnPoints[Random.Range(0, m_SpawnPoints.Count)]);
		}
		StartCoroutine(Wait_Corutine());
	}

	private IEnumerator Wait_Corutine()
	{
		while (m_Child != null && m_Child.IsAlive)
		{
			yield return new WaitForSeconds(0.5f);
		}
		StartCoroutine(SpawnEnemy_Corutine());
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
}
