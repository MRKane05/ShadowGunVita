using UnityEngine;

internal class GOAPGoalDodge : GOAPGoal
{
	private static float _DisabledForEverybodyTimer;

	private float WorldStateTime;

	protected override float DisabledForEverybodyTimer
	{
		get
		{
			return _DisabledForEverybodyTimer;
		}
		set
		{
			_DisabledForEverybodyTimer = value;
		}
	}

	public GOAPGoalDodge(AgentHuman owner)
		: base(E_GOAPGoals.Dodge, owner)
	{
	}

	public override void InitGoal()
	{
		base.FailChance = 10;
		DisabledForEverybodyDelay = 2f;
	}

	public override float GetMaxRelevancy()
	{
		return base.Owner.BlackBoard.GoapSetup.DodgeRelevancy;
	}

	public override void CalculateGoalRelevancy()
	{
		base.GoalRelevancy = 0f;
		if (base.Owner.WorldState.GetWSProperty(E_PropKey.CoverState).GetCoverState() == E_CoverState.None && base.Owner.WorldState.GetWSProperty(E_PropKey.LookingAtTarget).GetBool())
		{
			if (base.Owner.WorldState.GetWSProperty(E_PropKey.EnemyLookingAtMe).GetBool())
			{
				base.GoalRelevancy = base.Owner.BlackBoard.GoapSetup.DodgeRelevancy * (base.Owner.BlackBoard.Dodge / 100f);
			}
			if (base.Owner.WorldState.GetWSProperty(E_PropKey.Event).GetEvent() == E_EventTypes.EnemyInjuredMe)
			{
				base.GoalRelevancy = base.Owner.BlackBoard.GoapSetup.DodgeRelevancy * (base.Owner.BlackBoard.Dodge / 100f);
			}
			if (base.GoalRelevancy < 0.2f)
			{
				base.GoalRelevancy = 0f;
			}
		}
	}

	public override void SetDisableTime()
	{
		base.NextEvaluationTime = base.Owner.BlackBoard.GoapSetup.DodgeDelay + Time.timeSinceLevelLoad;
	}

	public override void SetWSSatisfactionForPlanning(WorldState worldState)
	{
		worldState.SetWSProperty(E_PropKey.InDodge, true);
	}

	public override bool IsWSSatisfiedForPlanning(WorldState worldState)
	{
		return worldState.GetWSProperty(E_PropKey.InDodge).GetBool();
	}

	public override bool IsSatisfied()
	{
		return IsPlanFinished();
	}
}
