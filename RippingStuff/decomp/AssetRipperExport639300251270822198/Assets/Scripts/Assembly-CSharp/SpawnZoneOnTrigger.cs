using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[AddComponentMenu("Game/SpawnZone On Trigger")]
public class SpawnZoneOnTrigger : SpawnZoneBase
{
	private void OnTriggerEnter(Collider other)
	{
		if (base.State == E_State.E_WAITING_FOR_START && !(other != Player.Instance.Owner.CharacterController))
		{
			StartSpawn();
		}
	}
}
