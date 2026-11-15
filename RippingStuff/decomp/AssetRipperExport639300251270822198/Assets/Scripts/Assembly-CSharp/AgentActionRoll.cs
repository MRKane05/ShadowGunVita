public class AgentActionRoll : AgentAction
{
	public E_Direction Direction;

	public AgentActionRoll()
		: base(AgentActionFactory.E_Type.Roll)
	{
	}
}
