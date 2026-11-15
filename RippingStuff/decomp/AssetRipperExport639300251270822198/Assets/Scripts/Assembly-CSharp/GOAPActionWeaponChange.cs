internal class GOAPActionWeaponChange : GOAPAction
{
	private AgentActionWeaponChange Action;

	public GOAPActionWeaponChange(AgentHuman owner)
		: base(E_GOAPAction.WeaponChange, owner)
	{
	}

	public override void InitAction()
	{
		WorldEffects.SetWSProperty(E_PropKey.WeaponChange, false);
		Cost = 1f;
		Interruptible = false;
	}

	public override void Activate()
	{
		base.Activate();
		Action = AgentActionFactory.Create(AgentActionFactory.E_Type.WeaponChange) as AgentActionWeaponChange;
		Action.NewWeapon = Owner.BlackBoard.Desires.Weapon;
		Owner.BlackBoard.ActionAdd(Action);
	}

	public override void Deactivate()
	{
		Owner.WorldState.SetWSProperty(E_PropKey.WeaponChange, false);
		base.Deactivate();
	}

	public override bool IsActionComplete()
	{
		if (!Action.IsActive())
		{
			return true;
		}
		return false;
	}
}
