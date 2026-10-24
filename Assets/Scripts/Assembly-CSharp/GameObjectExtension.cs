using UnityEngine;

public static class GameObjectExtension
{
	public static T GetFirstComponentUpwardWithInterface<T>(this GameObject inGameObject) where T : class
	{
		return GameObjectUtils.GetFirstComponentUpwardWithInterface<T>(inGameObject);
	}

	public static T GetFirstComponentUpward<T>(this GameObject inGameObject) where T : Component
	{
		return GameObjectUtils.GetFirstComponentUpward<T>(inGameObject);
	}

	public static T GetComponentWithInterface<T>(this GameObject inGameObject) where T : class
	{
		return GameObjectUtils.GetComponentWithInterface<T>(inGameObject);
	}
}
