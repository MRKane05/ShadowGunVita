internal class GOAPActionShielderCritInjury : GOAPAction
{
	private AgentActionInjuryCrit Action;

	public GOAPActionShielderCritInjury(AgentHuman owner)
		: base(E_GOAPAction.CritInjury, owner)
	{
	}

	public override void InitAction()
	{
		WorldEffects.SetWSProperty(E_PropKey.CriticalInjury, false);
		Cost = 1f;
		Interruptible = false;
	}

	public override void Activate()
	{
		base.Activate();
		Action = AgentActionFactory.Create(AgentActionFactory.E_Type.InjuryCrit) as AgentActionInjuryCrit;
		Owner.BlackBoard.ActionAdd(Action);
	}

	public override void Deactivate()
	{
		Owner.WorldState.SetWSProperty(E_PropKey.CriticalInjury, false);
		AgentActionIdle action = AgentActionFactory.Create(AgentActionFactory.E_Type.Idle) as AgentActionIdle;
		Owner.BlackBoard.ActionAdd(action);
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
