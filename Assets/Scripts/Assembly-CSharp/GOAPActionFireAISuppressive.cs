internal class GOAPActionFireAISuppressive : GOAPActionFireAI
{
	public GOAPActionFireAISuppressive(AgentHuman owner)
		: base(owner)
	{
		Type = E_GOAPAction.FireAISuppressive;
	}

	public override void Update()
	{
		Fact validFact = Owner.Memory.GetValidFact(E_EventTypes.EnemyHideInCover);
		if (validFact != null)
		{
			Owner.BlackBoard.Desires.Rotation.SetLookRotation((validFact.Position - Owner.Position).normalized);
		}
		UpdateState();
	}

	public override bool ValidateContextPreconditions(WorldState current, bool planning)
	{
		if (Owner.WorldState.GetWSProperty(E_PropKey.CoverState).GetCoverState() != E_CoverState.None)
		{
			return false;
		}
		Fact validFact = Owner.Memory.GetValidFact(E_EventTypes.EnemyHideInCover);
		if (validFact == null)
		{
			return false;
		}
		return true;
	}

	public override bool ValidateAction()
	{
		Fact validFact = Owner.Memory.GetValidFact(E_EventTypes.EnemyHideInCover);
		if (validFact == null)
		{
			return false;
		}
		return Owner.IsAlive && Owner.WorldState.GetWSProperty(E_PropKey.WeaponLoaded).GetBool();
	}
}
