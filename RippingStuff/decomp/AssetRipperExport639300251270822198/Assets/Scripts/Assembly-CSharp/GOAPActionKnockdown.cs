internal class GOAPActionKnockdown : GOAPAction
{
	private AgentActionKnockdown Action;

	public GOAPActionKnockdown(AgentHuman owner)
		: base(E_GOAPAction.Knockdown, owner)
	{
	}

	public override void InitAction()
	{
		WorldEffects.SetWSProperty(E_PropKey.Event, E_EventTypes.None);
		Interruptible = false;
		Cost = 1f;
		Precedence = 100;
	}

	public override bool ValidateContextPreconditions(WorldState current, bool planning)
	{
		WorldStateProp wSProperty = Owner.WorldState.GetWSProperty(E_PropKey.Event);
		if (wSProperty == null || wSProperty.GetEvent() != E_EventTypes.Knockdown)
		{
			return false;
		}
		if (!Owner.IsAlive)
		{
			return false;
		}
		return true;
	}

	public override void Activate()
	{
		base.Activate();
		Action = null;
		WorldStateProp wSProperty = Owner.WorldState.GetWSProperty(E_PropKey.Event);
		if (!(wSProperty == null) && wSProperty.GetEvent() == E_EventTypes.Knockdown)
		{
			SendAction();
		}
	}

	public override void Deactivate()
	{
		Action = null;
		base.Deactivate();
		Owner.WorldState.SetWSProperty(E_PropKey.Event, E_EventTypes.None);
	}

	public override void Update()
	{
		WorldStateProp wSProperty = Owner.WorldState.GetWSProperty(E_PropKey.Event);
		if (wSProperty.GetEvent() == E_EventTypes.EnemyInjuredMe || wSProperty.GetEvent() == E_EventTypes.Died)
		{
			SendActionKill();
		}
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
		return true;
	}

	private void SendAction()
	{
	}

	private void SendActionKill()
	{
		Owner.WorldState.SetWSProperty(E_PropKey.Event, E_EventTypes.None);
		Owner.BlackBoard.DontUpdate = true;
	}
}
