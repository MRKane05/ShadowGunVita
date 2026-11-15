internal class GOAPActionWeaponReload : GOAPAction
{
	private AgentActionReload Action;

	public GOAPActionWeaponReload(AgentHuman owner)
		: base(E_GOAPAction.WeaponReload, owner)
	{
	}

	public override void InitAction()
	{
		WorldEffects.SetWSProperty(E_PropKey.WeaponLoaded, true);
		Interruptible = false;
		Cost = 1f;
		Precedence = 10;
	}

	public override bool ValidateContextPreconditions(WorldState current, bool planning)
	{
		if (!planning)
		{
			return true;
		}
		if (Owner.WeaponComponent.GetCurrentWeapon().WeaponAmmo == 0)
		{
			return false;
		}
		return true;
	}

	public override void Activate()
	{
		base.Activate();
		Action = AgentActionFactory.Create(AgentActionFactory.E_Type.Reload) as AgentActionReload;
		Owner.BlackBoard.ActionAdd(Action);
	}

	public override void Deactivate()
	{
		base.Deactivate();
		if (Action.IsSuccess() && Owner.WeaponComponent.GetCurrentWeapon().ClipAmmo > 0)
		{
			Owner.WorldState.SetWSProperty(E_PropKey.WeaponLoaded, true);
		}
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
