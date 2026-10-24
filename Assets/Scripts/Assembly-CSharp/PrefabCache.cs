using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PrefabCache<T> where T : UnityEngine.Object
{
	public T m_Prefab;

	public int m_InitialCacheSize;

	public List<T> m_FreeObjects = new List<T>();

	~PrefabCache()
	{
		Clear();
	}

	public T Get()
	{
		if (m_FreeObjects.Count > 0)
		{
			T result = m_FreeObjects[0];
			m_FreeObjects.RemoveAt(0);
			return result;
		}
		return (T)null;
	}

	public void Return(T inObject)
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

	public void Init()
	{
		if (m_InitialCacheSize > 0)
		{
			for (int num = m_InitialCacheSize; num > 0; num--)
			{
				CreateNewInstance();
			}
		}
	}

	public void CreateNewInstance()
	{
		if (m_Prefab != null)
		{
			T val = UnityEngine.Object.Instantiate(m_Prefab) as T;
			GameObject gameObject = val as GameObject;
			Component component = val as Component;
			if (gameObject != null)
			{
				gameObject.SetActiveRecursively(false);
			}
			else if (component != null)
			{
				component.gameObject.SetActiveRecursively(false);
			}
			m_FreeObjects.Add(val);
		}
	}

	public void Clear()
	{
		m_Prefab = (T)null;
		m_FreeObjects.Clear();
	}
}
