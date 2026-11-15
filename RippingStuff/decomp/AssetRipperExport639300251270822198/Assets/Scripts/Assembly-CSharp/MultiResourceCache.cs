using System.Collections.Generic;
using UnityEngine;

internal class MultiResourceCache<CacheKeyType, CacheType> where CacheType : ResourceCache
{
	private Dictionary<CacheKeyType, CacheType> Caches = new Dictionary<CacheKeyType, CacheType>();

	public CacheType this[CacheKeyType inKey]
	{
		get
		{
			return Caches[inKey];
		}
		set
		{
			Caches[inKey] = value;
		}
	}

	public GameObject GetWeapon(CacheKeyType type)
	{
		if (!Caches.ContainsKey(type))
		{
			Debug.LogError("MultiResourceCache: unknown type " + type);
			return null;
		}
		if (Caches[type] == null)
		{
			Debug.Log("MultiResourceCache: We don't have resource for this type " + type);
			return null;
		}
		CacheType val = Caches[type];
		return val.Get();
	}

	public void Return(CacheKeyType type, GameObject inObject)
	{
		if (inObject == null)
		{
			Debug.LogError("MultiResourceCache: sombody is trying return null object to cache");
			return;
		}
		if (!Caches.ContainsKey(type))
		{
			Debug.LogError("MultiResourceCache: unknown type " + type);
			return;
		}
		if (Caches[type] == null)
		{
			Debug.LogError("MultiResourceCache: We don't have cache for this type. This object was not created by this factory");
			return;
		}
		CacheType val = Caches[type];
		val.Return(inObject);
	}
}
