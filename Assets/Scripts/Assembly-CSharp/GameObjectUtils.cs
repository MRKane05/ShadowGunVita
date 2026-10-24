using UnityEngine;

internal class GameObjectUtils
{
	public static T GetFirstComponentUpward<T>(GameObject inGameObject) where T : Component
	{
		if (inGameObject == null)
		{
			return (T)null;
		}
		T component = inGameObject.GetComponent<T>();
		if (component != null)
		{
			return component;
		}
		if (inGameObject.transform.parent == null || inGameObject.transform.parent.gameObject == null)
		{
			return (T)null;
		}
		return GetFirstComponentUpward<T>(inGameObject.transform.parent.gameObject);
	}

	public static T GetComponentWithInterface<T>(GameObject inGameObject) where T : class
	{
		Component[] components = inGameObject.GetComponents<Component>();
		foreach (Component component in components)
		{
			T val = component as T;
			if (val != null)
			{
				return val;
			}
		}
		return (T)null;
	}

	public static T GetFirstComponentUpwardWithInterface<T>(GameObject inGameObject) where T : class
	{
		if (inGameObject != null)
		{
			Component[] components = inGameObject.GetComponents<Component>();
			foreach (Component component in components)
			{
				T val = component as T;
				if (val != null)
				{
					return val;
				}
			}
			if (inGameObject.transform.parent != null && inGameObject.transform.parent.gameObject != null)
			{
				return GetFirstComponentUpwardWithInterface<T>(inGameObject.transform.parent.gameObject);
			}
		}
		return (T)null;
	}
}
