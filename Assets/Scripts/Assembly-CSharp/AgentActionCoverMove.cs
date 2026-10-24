public class AgentActionCoverMove : AgentAction
{
	public enum E_Direction
	{
		Unknown = 0,
		Left = 1,
		Right = 2
	}

	public E_Direction Direction;

	public float Speed;

	public AgentActionCoverMove()
		: base(AgentActionFactory.E_Type.CoverMove)
	{
	}

	public override void Reset()
	{
		Direction = E_Direction.Unknown;
		Speed = 0f;
	}
}
