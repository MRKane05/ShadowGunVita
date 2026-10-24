internal class GOAPActionTeleport : GOAPAction
{
	private AgentActionTeleport Action;

	public GOAPActionTeleport(AgentHuman owner)
		: base(E_GOAPAction.Teleport, owner)
	{
	}

	public override void InitAction()
	{
		WorldEffects.SetWSProperty(E_PropKey.Teleport, false);
		WorldEffects.SetWSProperty(E_PropKey.CoverState, E_CoverState.None);
		Cost = 1f;
		Interruptible = false;
		Precedence = 90;
	}

	public override void Activate()
	{
		base.Activate();
		Action = AgentActionFactory.Create(AgentActionFactory.E_Type.Teleport) as AgentActionTeleport;
		Action.Destination = Owner.BlackBoard.Desires.TeleportDestination;
		Action.Rotation = Owner.BlackBoard.Desires.TeleportRotation;
		Owner.BlackBoard.ActionAdd(Action);
	}

	public override void Deactivate()
	{
		base.Deactivate();
		Owner.WorldState.SetWSProperty(E_PropKey.Teleport, false);
		Owner.BlackBoard.Stop = false;
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
