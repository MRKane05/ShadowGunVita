internal class GOAPActionRoll : GOAPAction
{
	private AgentActionRoll Action;

	private E_StrafeDirection StrafeDirection;

	public GOAPActionRoll(AgentHuman owner)
		: base(E_GOAPAction.Roll, owner)
	{
	}

	public override void InitAction()
	{
		WorldEffects.SetWSProperty(E_PropKey.InDodge, false);
		Cost = 5f;
		Precedence = 70;
	}

	public override bool ValidateContextPreconditions(WorldState current, bool planning)
	{
		return true;
	}

	public override void Activate()
	{
		base.Activate();
		Owner.BlackBoard.Desires.WeaponTriggerOn = false;
		Owner.WorldState.SetWSProperty(E_PropKey.InDodge, false);
		Action = AgentActionFactory.Create(AgentActionFactory.E_Type.Roll) as AgentActionRoll;
		Action.Direction = Owner.GetMoveDirectionType(Owner.BlackBoard.Desires.MoveDirection);
		Owner.BlackBoard.ActionAdd(Action);
	}

	public override void Deactivate()
	{
		Owner.WorldState.SetWSProperty(E_PropKey.InDodge, false);
		base.Deactivate();
	}

	public override bool IsActionComplete()
	{
		if (Action != null && !Action.IsActive())
		{
			return true;
		}
		return false;
	}

	public override bool ValidateAction()
	{
		if (Action != null && Action.IsFailed())
		{
			return false;
		}
		return Owner.IsAlive;
	}
}
