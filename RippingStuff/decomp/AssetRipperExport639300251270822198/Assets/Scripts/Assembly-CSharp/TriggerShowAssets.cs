using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Triggers/Show Assets")]
public class TriggerShowAssets : MonoBehaviour
{
	public List<GameObject> GameObjectsList = new List<GameObject>();

	private void OnTriggerEnter(Collider other)
	{
		if (!(other != Player.Instance.Owner.CharacterController))
		{
			for (int i = 0; i < Mission.Instance.ManagedGameObject.Count; i++)
			{
				Mission.Instance.ManagedGameObject[i].SetActiveRecursively(IsInList(Mission.Instance.ManagedGameObject[i]));
			}
		}
	}

	private bool IsInList(GameObject gameObject)
	{
		for (int i = 0; i < GameObjectsList.Count; i++)
		{
			if (gameObject == GameObjectsList[i])
			{
				return true;
			}
		}
		return false;
	}
}
