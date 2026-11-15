internal class GOAPActionCoverEnter : GOAPAction
{
	private AgentActionCoverEnter Action;

	public GOAPActionCoverEnter(AgentHuman owner)
		: base(E_GOAPAction.CoverEnter, owner)
	{
	}

	public override void InitAction()
	{
		WorldEffects.SetWSProperty(E_PropKey.CoverState, E_CoverState.Middle);
		WorldPreconditions.SetWSProperty(E_PropKey.AtTargetPos, true);
		Interruptible = false;
		Cost = 1f;
		Precedence = 60;
	}

	public override bool ValidateContextPreconditions(WorldState current, bool planning)
	{
		if (!planning)
		{
			return true;
		}
		return true;
	}

	public override void Activate()
	{
		base.Activate();
		if ((bool)Owner.BlackBoard.Desires.CoverSelected)
		{
			Owner.BlackBoard.Desires.CoverSelected.LockPosition(Owner.BlackBoard.Desires.CoverPosition, Owner);
			Action = AgentActionFactory.Create(AgentActionFactory.E_Type.CoverEnter) as AgentActionCoverEnter;
			Action.Cover = Owner.BlackBoard.Desires.CoverSelected;
			Owner.BlackBoard.Cover = Action.Cover;
			Owner.BlackBoard.ActionAdd(Action);
		}
	}

	public override bool ValidateAction()
	{
		return Owner.BlackBoard.Desires.CoverSelected != null;
	}

	public override void Deactivate()
	{
		base.Deactivate();
		if (Action != null && Action.IsSuccess())
		{
			Owner.BlackBoard.SetFear(0f);
		}
		else
		{
			Owner.WorldState.SetWSProperty(E_PropKey.CoverState, E_CoverState.None);
			Owner.BlackBoard.Cover = null;
		}
		Owner.BlackBoard.Desires.CoverSelected = null;
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
