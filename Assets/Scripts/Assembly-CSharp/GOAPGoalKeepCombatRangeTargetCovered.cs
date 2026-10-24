using UnityEngine;

internal class GOAPGoalKeepCombatRangeTargetCovered : GOAPGoal
{
	private static float _DisabledForEverybodyTimer;

	private Vector3 AdvancePos;

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

	public GOAPGoalKeepCombatRangeTargetCovered(AgentHuman owner)
		: base(E_GOAPGoals.KeepCombatRange, owner)
	{
	}

	public override void InitGoal()
	{
		base.FailChance = 20;
		DisabledForEverybodyDelay = 0.5f;
	}

	public override float GetMaxRelevancy()
	{
		return base.Owner.BlackBoard.GoapSetup.KeepCombatRangeTargetCoveredRelevancy;
	}

	public override void CalculateGoalRelevancy()
	{
		base.GoalRelevancy = 0f;
		if ((base.Owner.BlackBoard.CoverTime != 0f && base.Owner.BlackBoard.CoverTime < 15f) || base.Owner.WorldState.GetWSProperty(E_PropKey.CoverState).GetCoverState() != E_CoverState.None || base.Owner.BlackBoard.VisibleTarget != null)
		{
			return;
		}
		Fact validFact = base.Owner.Memory.GetValidFact(E_EventTypes.EnemyHideInCover);
		if (validFact == null)
		{
			return;
		}
		float num = 0f;
		E_MoveType e_MoveType;
		if (base.Owner.BlackBoard.DistanceToTarget > base.Owner.BlackBoard.CombatRange)
		{
			num = (base.Owner.BlackBoard.DistanceToTarget - base.Owner.BlackBoard.CombatRange) / base.Owner.BlackBoard.CombatRange;
			e_MoveType = E_MoveType.Forward;
		}
		else
		{
			num = (base.Owner.BlackBoard.CombatRange - base.Owner.BlackBoard.DistanceToTarget) / base.Owner.BlackBoard.CombatRange;
			e_MoveType = E_MoveType.Backward;
		}
		if (!(num < 0.2f))
		{
			AiRecon.NearPositionData nearPositionData = ((e_MoveType != E_MoveType.Forward) ? base.Owner.BlackBoard.AiRecon.GetFarestPositionToPos(validFact.Position, 1.5f, 3f) : base.Owner.BlackBoard.AiRecon.GetNearestPositionToPos(validFact.Position, 1.5f, 3f));
			if (nearPositionData != null)
			{
				AdvancePos = nearPositionData.Position;
				base.GoalRelevancy = base.Owner.BlackBoard.GoapSetup.KeepCombatRangeTargetCoveredRelevancy * num;
			}
		}
	}

	public override void SetDisableTime()
	{
		base.NextEvaluationTime = base.Owner.BlackBoard.GoapSetup.KeepCombatRangeTargetCoveredDelay + Time.timeSinceLevelLoad;
	}

	public override void ChangeCurrentWSForPlanning(WorldState worldState)
	{
		worldState.SetWSProperty(E_PropKey.AtTargetPos, false);
		worldState.SetWSProperty(E_PropKey.TargetNode, AdvancePos);
	}

	public override void SetWSSatisfactionForPlanning(WorldState worldState)
	{
		worldState.SetWSProperty(E_PropKey.AtTargetPos, true);
		worldState.SetWSProperty(E_PropKey.TargetNode, AdvancePos);
		worldState.SetWSProperty(E_PropKey.CoverState, E_CoverState.None);
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

	public override bool IsSatisfied()
	{
		return IsPlanFinished();
	}

	public override bool Activate(GOAPPlan plan)
	{
		base.Owner.WorldState.SetWSProperty(E_PropKey.AtTargetPos, false);
		base.Owner.WorldState.SetWSProperty(E_PropKey.TargetNode, AdvancePos);
		base.GoalRelevancy = base.Owner.BlackBoard.GoapSetup.KeepCombatRangeTargetCoveredRelevancy;
		return base.Activate(plan);
	}

	public override void Deactivate()
	{
		base.Deactivate();
	}
}
