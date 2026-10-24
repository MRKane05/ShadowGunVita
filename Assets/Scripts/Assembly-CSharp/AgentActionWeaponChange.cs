public class AgentActionWeaponChange : AgentAction
{
	public E_WeaponType NewWeapon;

	public AgentActionWeaponChange()
		: base(AgentActionFactory.E_Type.WeaponChange)
	{
	}
}
