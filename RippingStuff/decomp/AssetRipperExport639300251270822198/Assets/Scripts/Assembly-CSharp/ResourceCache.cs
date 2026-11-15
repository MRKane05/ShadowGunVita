using System.Collections.Generic;
using UnityEngine;

internal class ResourceCache
{
	private string m_ResourceName;

	private int m_InitialCacheSize;

	private GameObject m_Prefab;

	private List<GameObject> m_FreeObjects = new List<GameObject>();

	public ResourceCache(string inName, int inInitialCacheSize)
	{
		m_ResourceName = inName;
		m_InitialCacheSize = inInitialCacheSize;
		if (m_InitialCacheSize > 0)
		{
			CreateResources(m_InitialCacheSize);
		}
	}

	~ResourceCache()
	{
		m_Prefab = null;
		m_FreeObjects.Clear();
	}

	public GameObject Get()
	{
		if (m_FreeObjects.Count == 0)
		{
			CreateResources(Mathf.Max(1, m_InitialCacheSize));
		}
		if (m_FreeObjects.Count > 0)
		{
			GameObject result = m_FreeObjects[0];
			m_FreeObjects.RemoveAt(0);
			return result;
		}
		return null;
	}

	public void Return(GameObject inObject)
	{
		if (inObject == null)
		{
			Debug.LogWarning("You are trying return null object");
		}
		else if (m_FreeObjects.Contains(inObject))
		{
			Debug.LogWarning(string.Concat("Object [", inObject, "] is already in Free list"));
		}
		else
		{
			m_FreeObjects.Add(inObject);
		}
	}

	internal void CreateResources(int inNumberOfItems)
	{
		if (m_Prefab == null)
		{
			m_Prefab = Resources.Load(m_ResourceName) as GameObject;
		}
		if (m_Prefab == null)
		{
			Debug.LogWarning("Can't initialize ResourceCache for resource : " + m_ResourceName);
			return;
		}
		for (int num = inNumberOfItems; num > 0; num--)
		{
			GameObject gameObject = Object.Instantiate(m_Prefab) as GameObject;
			gameObject.SetActiveRecursively(false);
			gameObject.name += m_FreeObjects.Count;
			m_FreeObjects.Add(gameObject);
		}
	}
}
