using UnityEngine;

public class SentinelPatrolPoint : MonoBehaviour
{
	private void OnDrawGizmos()
	{
		if (base.enabled)
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawSphere(base.transform.position, 0.25f);
		}
	}
}
