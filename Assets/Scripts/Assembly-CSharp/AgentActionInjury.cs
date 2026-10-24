using UnityEngine;

public class AgentActionInjury : AgentAction
{
	public AgentHuman Attacker;

	public E_WeaponType FromWeapon;

	public Vector3 Impuls;

	public float Damage;

	public AgentActionInjury()
		: base(AgentActionFactory.E_Type.Injury)
	{
	}
}
