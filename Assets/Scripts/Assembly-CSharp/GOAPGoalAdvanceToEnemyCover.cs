using UnityEngine;

internal class GOAPGoalAdvanceToEnemyCover : GOAPGoal
{
	private static float _DisabledForEverybodyTimer;

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

	public GOAPGoalAdvanceToEnemyCover(AgentHuman owner)
		: base(E_GOAPGoals.AdvanceTargetCovered, owner)
	{
	}

	public override void InitGoal()
	{
		base.FailChance = 10;
		DisabledForEverybodyDelay = 4f;
	}

	public override float GetMaxRelevancy()
	{
		return base.Owner.BlackBoard.GoapSetup.AdvanceToEnemyCoverRelevancy;
	}

	public override void CalculateGoalRelevancy()
	{
		base.GoalRelevancy = 0f;
		if ((bool)base.Owner.BlackBoard.VisibleTarget || (base.Owner.BlackBoard.CoverTime != 0f && base.Owner.BlackBoard.CoverTime < 3f))
		{
			return;
		}
		Fact validFact = base.Owner.Memory.GetValidFact(E_EventTypes.EnemyFire);
		if (validFact != null)
		{
			return;
		}
		validFact = base.Owner.Memory.GetValidFact(E_EventTypes.EnemyHideInCover);
		if (validFact != null)
		{
			base.GoalRelevancy = base.Owner.BlackBoard.GoapSetup.AdvanceToEnemyCoverRelevancy * (1f - base.Owner.BlackBoard.Fear / 100f);
			if (base.GoalRelevancy < 0.2f)
			{
				base.GoalRelevancy = 0f;
			}
		}
	}

	public override void SetDisableTime()
	{
		base.NextEvaluationTime = base.Owner.BlackBoard.GoapSetup.AdvanceToEnemyCoverDelay + Time.timeSinceLevelLoad;
	}

	public override void SetWSSatisfactionForPlanning(WorldState worldState)
	{
		worldState.SetWSProperty(E_PropKey.AtTargetPos, true);
	}

	public override bool IsWSSatisfiedForPlanning(WorldState worldState)
	{
		WorldStateProp wSProperty = worldState.GetWSProperty(E_PropKey.AtTargetPos);
		if (wSProperty != null && wSProperty.GetBool())
		{
			return true;
		}
		return false;
	}

	public override void ChangeCurrentWSForPlanning(WorldState worldState)
	{
		worldState.SetWSProperty(E_PropKey.AtTargetPos, false);
	}

	public override bool IsSatisfied()
	{
		return IsPlanFinished();
	}

	public override bool Activate(GOAPPlan plan)
	{
		base.GoalRelevancy = base.Owner.BlackBoard.GoapSetup.AdvanceToEnemyCoverRelevancy;
		base.Owner.BlackBoard.SetFear(0f);
		return base.Activate(plan);
	}
}
