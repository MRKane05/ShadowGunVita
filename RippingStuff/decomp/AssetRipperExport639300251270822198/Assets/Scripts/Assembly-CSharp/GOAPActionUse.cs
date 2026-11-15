internal class GOAPActionUse : GOAPAction
{
	private AgentActionUse Action;

	public GOAPActionUse(AgentHuman owner)
		: base(E_GOAPAction.Use, owner)
	{
	}

	public override void InitAction()
	{
		WorldPreconditions.SetWSProperty(E_PropKey.AtTargetPos, true);
		WorldEffects.SetWSProperty(E_PropKey.UseWorldObject, false);
		Interruptible = false;
		Cost = 2f;
	}

	public override void Activate()
	{
		base.Activate();
		Action = AgentActionFactory.Create(AgentActionFactory.E_Type.Use) as AgentActionUse;
		Action.InterObj = Owner.BlackBoard.Desires.InteractionObject;
		Owner.BlackBoard.InteractionObject = Owner.BlackBoard.Desires.InteractionObject;
		Owner.BlackBoard.Desires.InteractionObject = null;
		Owner.BlackBoard.ActionAdd(Action);
		Owner.BlackBoard.BusyAction = true;
		Owner.BlackBoard.Desires.WeaponTriggerOn = false;
		Owner.BlackBoard.Stop = true;
	}

	public override void Deactivate()
	{
		Owner.BlackBoard.InteractionObject = null;
		Owner.BlackBoard.Stop = false;
		Owner.BlackBoard.BusyAction = false;
		Owner.WorldState.SetWSProperty(E_PropKey.UseWorldObject, false);
		Owner.WorldState.SetWSProperty(E_PropKey.AtTargetPos, true);
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
