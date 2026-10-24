public class AgentActioCombatMove : AgentAction
{
	public AgentHuman Target;

	public float DistanceToMove;

	public float MinDistanceToTarget;

	public E_MoveType MoveType;

	public E_MotionType MotionType = E_MotionType.Walk;

	public AgentActioCombatMove()
		: base(AgentActionFactory.E_Type.CombatMove)
	{
	}

	public override void Reset()
	{
		base.Reset();
		DistanceToMove = 0f;
		MinDistanceToTarget = 0f;
		MoveType = E_MoveType.Forward;
		MotionType = E_MotionType.Walk;
	}
}
