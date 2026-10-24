public interface AgentCallbackterface
{
	void RecieveHit(AgentHuman attacker, E_WeaponType weapon);

	void RecieveKnockDown(AgentHuman attacker, E_WeaponType weapon);

	void GOAPGoalActivate(E_GOAPGoals goal);

	void GOAPGoalDeactivated(E_GOAPGoals goal);
}
