using System;
using UnityEngine;

public class CollisionUtils
{
	public static int CompareHits(RaycastHit x, RaycastHit y)
	{
		return x.distance.CompareTo(y.distance);
	}

	public static GameObject FirstCollisionOnRay(Ray ray, float distance, GameObject ignoreGO)
	{
		RaycastHit[] array = Physics.RaycastAll(ray, distance);
		if (array.Length > 1)
		{
			Array.Sort(array, CompareHits);
		}
		RaycastHit[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			RaycastHit raycastHit = array2[i];
			GameObject gameObject = raycastHit.transform.gameObject;
			if (!(gameObject == ignoreGO) && !raycastHit.collider.isTrigger)
			{
				return gameObject;
			}
		}
		return null;
	}
}
