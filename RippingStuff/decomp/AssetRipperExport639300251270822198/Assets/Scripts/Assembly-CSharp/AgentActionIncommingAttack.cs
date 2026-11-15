public class AgentActionIncommingAttack : AgentAction
{
	public AgentHuman Attacker;

	public float HitTime;

	public AgentActionIncommingAttack()
		: base(AgentActionFactory.E_Type.IncommingAttack)
	{
	}
}
