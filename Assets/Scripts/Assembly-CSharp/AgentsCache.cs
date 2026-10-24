using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AgentsCache
{
	private GameObject Prefab;

	public E_EnemyType EnemyType = E_EnemyType.None;

	public int MaxCount;

	private List<GameObject> _Cache = new List<GameObject>();

	private List<GameObject> _AgentsInScene = new List<GameObject>();

	public void Init()
	{
		switch (EnemyType)
		{
		case E_EnemyType.Mutant:
			Prefab = Resources.Load("Enemies/EnemyMutant") as GameObject;
			break;
		case E_EnemyType.Flyer:
			Prefab = Resources.Load("Enemies/EnemyFlyer") as GameObject;
			break;
		case E_EnemyType.Spider:
			Prefab = Resources.Load("Enemies/EnemySpider") as GameObject;
			break;
		case E_EnemyType.Heavy:
			Prefab = Resources.Load("Enemies/EnemyHeavy") as GameObject;
			break;
		case E_EnemyType.Elite:
			Prefab = Resources.Load("Enemies/EnemyElite") as GameObject;
			break;
		case E_EnemyType.MutantShield:
			Prefab = Resources.Load("Enemies/EnemyShielder") as GameObject;
			break;
		case E_EnemyType.BigSpider:
			Prefab = Resources.Load("Enemies/EnemyMBossL02") as GameObject;
			break;
		case E_EnemyType.Kamikaze:
			Prefab = Resources.Load("Enemies/EnemyKamikaze") as GameObject;
			break;
		case E_EnemyType.LightTurret:
			Prefab = Resources.Load("Enemies/EnemyLightTurret") as GameObject;
			break;
		case E_EnemyType.DropShip:
			Prefab = Resources.Load("Enemies/EnemyDropShip") as GameObject;
			break;
		case E_EnemyType.Turret:
			Prefab = Resources.Load("Enemies/Turret") as GameObject;
			break;
		case E_EnemyType.Scientist:
			Prefab = Resources.Load("Enemies/EnemyScientist") as GameObject;
			break;
		default:
			Debug.LogError("HumanCache::Init -  Unknow enemy type");
			break;
		}
		if (Application.isEditor && Prefab == null)
		{
			Debug.LogError(string.Concat(ToString(), " Prefab ", EnemyType, " is null"));
		}
		for (int i = 0; i < MaxCount; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(Prefab) as GameObject;
			gameObject.GetComponent<Agent>().EnemyType = EnemyType;
			ComponentWeaponsAI component = gameObject.GetComponent<ComponentWeaponsAI>();
			if (component != null)
			{
				component.Initialize();
			}
			gameObject.name += i;
			gameObject.SetActiveRecursively(false);
			_Cache.Add(gameObject);
		}
	}

	public GameObject Get()
	{
		if (_Cache.Count > 0)
		{
			GameObject gameObject = _Cache[0];
			_Cache.RemoveAt(0);
			_AgentsInScene.Add(gameObject);
			gameObject.SetActiveRecursively(true);
			gameObject.transform.position = new Vector3(0f, 0f, 10000f);
			return gameObject;
		}
		return null;
	}

	public bool Return(GameObject enemy)
	{
		int num = _AgentsInScene.IndexOf(enemy);
		if (num >= 0)
		{
			enemy.SetActiveRecursively(false);
			_AgentsInScene.RemoveAt(num);
			_Cache.Add(enemy);
			return true;
		}
		return false;
	}

	public void Clear()
	{
		_Cache.Clear();
		_AgentsInScene.Clear();
		Sentinel.HACK_ClearStaticFields();
		Prefab = null;
	}
}
