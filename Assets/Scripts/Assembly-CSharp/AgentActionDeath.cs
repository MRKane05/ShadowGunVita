using UnityEngine;

public class AgentActionDeath : AgentAction
{
	public AgentHuman Attacker;

	public E_CriticalHitType DecapType;

	public E_WeaponType FromWeapon;

	public Vector3 Impuls;

	public AgentActionDeath()
		: base(AgentActionFactory.E_Type.Death)
	{
	}
}
