using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Teleport : MonoBehaviour
{
	public Transform Destination;

	public float TeleportDelay;

	public float FadeOUtTime = 1f;

	public float FadeInTime = 1f;

	public AudioClip Sound;

	private void Start()
	{
	}

	private void OnDrawGizmos()
	{
		BoxCollider boxCollider = GetComponent("BoxCollider") as BoxCollider;
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(boxCollider.transform.position + boxCollider.center, boxCollider.size);
		if (Destination != null)
		{
			Gizmos.DrawLine(boxCollider.transform.position, Destination.position);
			Gizmos.DrawWireCube(Destination.position, Vector3.one * 0.5f);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
	}
}
