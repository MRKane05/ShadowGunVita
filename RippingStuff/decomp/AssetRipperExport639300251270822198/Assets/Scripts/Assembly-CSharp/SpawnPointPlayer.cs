using UnityEngine;

[AddComponentMenu("Game/Spawn Player")]
public class SpawnPointPlayer : SpawnPoint
{
	private void Start()
	{
		base.enabled = false;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(base.transform.position, 0.5f);
	}
}
