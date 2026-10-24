using UnityEngine;

public class AgentActionGoToByAnim : AgentAction
{
	public Vector3 FinalPosition;

	public E_MoveType MoveType;

	public float MinDistance = 0.3f;

	public bool UseNavMeshAgentRotation;

	public AgentActionGoToByAnim()
		: base(AgentActionFactory.E_Type.GotoByAnim)
	{
	}

	public override void Reset()
	{
		MoveType = E_MoveType.Forward;
		MinDistance = 0.6f;
		UseNavMeshAgentRotation = false;
	}
}
