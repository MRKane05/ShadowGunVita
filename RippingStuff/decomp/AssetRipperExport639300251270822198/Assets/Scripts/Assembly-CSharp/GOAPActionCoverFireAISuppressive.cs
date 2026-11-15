using UnityEngine;

internal class GOAPActionCoverFireAISuppressive : GOAPActionCoverFireAI
{
	public GOAPActionCoverFireAISuppressive(AgentHuman owner)
		: base(owner)
	{
		Type = E_GOAPAction.CoverFireAISuppressive;
	}

	public override bool ValidateContextPreconditions(WorldState current, bool planning)
	{
		if (Owner.BlackBoard.Rage < Owner.BlackBoard.CombatSetup.RageMax * 0.7f)
		{
			return false;
		}
		if (Owner.WorldState.GetWSProperty(E_PropKey.CoverState).GetCoverState() == E_CoverState.None)
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

	public override void Update()
	{
		if (State == E_State.Waiting && (Owner.BlackBoard.DangerousEnemy == null || Vector3.Dot(Owner.Forward, (Owner.BlackBoard.DangerousEnemy.Position - Owner.Position).normalized) < 0.7f || !Owner.IsAlive || !Owner.BlackBoard.DangerousEnemy.IsAlive))
		{
			ChangeStateToEnd();
		}
		UpdateStates();
	}
}
