using UnityEngine;

public class WayPoint : MonoBehaviour
{
	public Transform Transform { get; private set; }

	public Vector3 Position
	{
		get
		{
			return Transform.position;
		}
	}

	private void Awake()
	{
		Transform = base.transform;
	}

	private void OnDrawGizmos()
	{
		if (base.enabled)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(base.transform.position, 0.1f);
		}
	}
}
