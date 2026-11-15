using UnityEngine;

public class AgentActionTeleport : AgentAction
{
	public Vector3 Destination;

	public Quaternion Rotation;

	public AgentActionTeleport()
		: base(AgentActionFactory.E_Type.Teleport)
	{
	}

	public override void Reset()
	{
	}
}
