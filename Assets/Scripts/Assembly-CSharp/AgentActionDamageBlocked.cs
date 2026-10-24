public class AgentActionDamageBlocked : AgentAction
{
	public AgentHuman Attacker;

	public bool BreakBlock;

	public AgentActionDamageBlocked()
		: base(AgentActionFactory.E_Type.DamageBlocked)
	{
	}

	public override void Reset()
	{
		BreakBlock = false;
	}
}
