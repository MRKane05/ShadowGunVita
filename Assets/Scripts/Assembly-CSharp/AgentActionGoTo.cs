using UnityEngine;

public class AgentActionGoTo : AgentAction
{
	public Vector3 FinalPosition;

	public E_MoveType MoveType;

	public E_MotionType Motion = E_MotionType.Run;

	public Transform LookTarget;

	public float MinDistance = 0.3f;

	public bool DontChangeParameters;

	public bool UseNavMeshAgentRotation;

	public AgentActionGoTo()
		: base(AgentActionFactory.E_Type.Goto)
	{
	}

	public override void Reset()
	{
		MoveType = E_MoveType.Forward;
		Motion = E_MotionType.Run;
		LookTarget = null;
		MinDistance = 0.3f;
		DontChangeParameters = false;
		UseNavMeshAgentRotation = false;
	}
}
