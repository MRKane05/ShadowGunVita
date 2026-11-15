internal class GOAPActionPlayAnim : GOAPAction
{
	private AgentActionPlayAnim Action;

	public GOAPActionPlayAnim(AgentHuman owner)
		: base(E_GOAPAction.PlayAnim, owner)
	{
	}

	public override void InitAction()
	{
		WorldEffects.SetWSProperty(E_PropKey.PlayAnim, false);
		Cost = 1f;
		Interruptible = true;
	}

	public override void Activate()
	{
		base.Activate();
		Action = AgentActionFactory.Create(AgentActionFactory.E_Type.PlayAnim) as AgentActionPlayAnim;
		Action.AnimName = Owner.BlackBoard.Desires.Animation;
		Owner.BlackBoard.ActionAdd(Action);
	}

	public override void Deactivate()
	{
		Owner.WorldState.SetWSProperty(E_PropKey.PlayAnim, false);
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
