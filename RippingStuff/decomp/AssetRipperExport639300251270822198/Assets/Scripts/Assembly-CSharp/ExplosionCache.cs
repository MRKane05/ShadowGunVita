using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ExplosionCache
{
	[Serializable]
	public class ExplosionPrefabCache : PrefabCache<Explosion>
	{
	}

	public ExplosionPrefabCache WaterExplosion;

	public List<ExplosionPrefabCache> Definition = new List<ExplosionPrefabCache>();

	private Dictionary<Explosion, ExplosionPrefabCache> _Cache = new Dictionary<Explosion, ExplosionPrefabCache>();

	public void Init()
	{
		foreach (ExplosionPrefabCache item in Definition)
		{
			_Cache[item.m_Prefab] = item;
			_Cache[item.m_Prefab].Init();
		}
		if (WaterExplosion != null && WaterExplosion.m_Prefab != null)
		{
			_Cache[WaterExplosion.m_Prefab] = WaterExplosion;
			_Cache[WaterExplosion.m_Prefab].Init();
		}
	}

	public Explosion GetWaterExplosion(Vector3 inPosition, Quaternion inRotation)
	{
		if (WaterExplosion != null && WaterExplosion.m_Prefab != null)
		{
			return Get(WaterExplosion.m_Prefab, inPosition, inRotation);
		}
		Debug.LogWarning("ExplosionCache: Water explosion is not defined ");
		return null;
	}

	public Explosion Get(Explosion inPrefab, Vector3 inPosition, Quaternion inRotation)
	{
		return Get(inPrefab, inPosition, inRotation, (Transform[])null);
	}

	public Explosion Get(Explosion inPrefab, Vector3 inPosition, Quaternion inRotation, Transform inNoBlocking)
	{
		return Get(inPrefab, inPosition, inRotation, new Transform[1] { inNoBlocking });
	}

	public Explosion Get(Explosion inPrefab, Vector3 inPosition, Quaternion inRotation, Transform[] inNoBlocking)
	{
		if (!_Cache.ContainsKey(inPrefab))
		{
			Debug.LogError("ExplosionCache: unknown type " + inPrefab);
			return null;
		}
		if (_Cache[inPrefab] == null)
		{
			Debug.Log(string.Concat("ExplosionCache: For this type ", inPrefab, " we don't have resource"));
			return null;
		}
		Explosion explosion = _Cache[inPrefab].Get();
		if (explosion == null)
		{
			Debug.LogWarning(string.Concat("ExplosionCache: Cache for item ", inPrefab, " is empty we must create new one."));
			_Cache[inPrefab].CreateNewInstance();
			explosion = _Cache[inPrefab].Get();
		}
		if (explosion == null)
		{
			Debug.LogError("ExplosionCache: Can't create explosion for type " + inPrefab);
			return null;
		}
		explosion.cacheKey = inPrefab;
		explosion.noBlocking = inNoBlocking;
		explosion.transform.position = inPosition;
		explosion.transform.rotation = inRotation;
		explosion.gameObject.SetActiveRecursively(true);
		return explosion;
	}

	public void Return(Explosion inExplosion)
	{
		if (inExplosion == null)
		{
			Debug.LogError("ExplosionCache: sombody is trying return null object to cache");
			return;
		}
		if (!_Cache.ContainsKey(inExplosion.cacheKey))
		{
			Debug.LogError("ExplosionCache: unknown type " + inExplosion.cacheKey);
			return;
		}
		if (_Cache[inExplosion.cacheKey] == null)
		{
			Debug.LogError("ExplosionCache: We don't have cache for this type. This object was not created by this manager");
			return;
		}
		Explosion cacheKey = inExplosion.cacheKey;
		inExplosion.Reset();
		inExplosion.gameObject.SetActiveRecursively(false);
		inExplosion.cacheKey = null;
		_Cache[cacheKey].Return(inExplosion);
	}

	public void Reset()
	{
		Explosion[] array = UnityEngine.Object.FindObjectsOfType(typeof(Explosion)) as Explosion[];
		Explosion[] array2 = array;
		foreach (Explosion explosion in array2)
		{
			if (explosion.cacheKey != null && _Cache.ContainsKey(explosion.cacheKey))
			{
				Return(explosion);
			}
		}
	}

	public void Clear()
	{
		if (WaterExplosion != null)
		{
			WaterExplosion.Clear();
		}
		foreach (ExplosionPrefabCache item in Definition)
		{
			item.Clear();
		}
		Definition.Clear();
		_Cache.Clear();
	}
}
