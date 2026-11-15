public class AgentActionCoverEnter : AgentAction
{
	public Cover Cover;

	public AgentActionCoverEnter()
		: base(AgentActionFactory.E_Type.CoverEnter)
	{
	}
}
