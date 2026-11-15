using UnityEngine;

public class AgentActionDodgeStrafe : AgentAction
{
	public E_StrafeDirection StrafeDirection;

	public E_MotionType Motion;

	public bool AnimationDriven;

	public Vector3 FinalPos;

	public AgentActionDodgeStrafe()
		: base(AgentActionFactory.E_Type.DodgeStrafe)
	{
	}

	public override void Reset()
	{
		AnimationDriven = false;
	}
}
