public class AgentActionUse : AgentAction
{
	public InteractionObject InterObj;

	public AgentActionUse()
		: base(AgentActionFactory.E_Type.Use)
	{
	}
}
