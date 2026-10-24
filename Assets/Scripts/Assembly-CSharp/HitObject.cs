using UnityEngine;

public class HitObject : BreakableObject
{
	public GameObject InteractionObject;

	private AgentHuman Owner;

	private void Awake()
	{
		Initialize();
	}

	protected override void OnDone()
	{
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(base.transform.position, 0.5f);
	}
}
