using UnityEngine;

internal class GOAPGoalFallBack : GOAPGoal
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

	public GOAPGoalFallBack(AgentHuman owner)
		: base(E_GOAPGoals.FallBack, owner)
	{
	}

	public override void InitGoal()
	{
		base.FailChance = 10;
		DisabledForEverybodyDelay = 2f;
	}

	public override float GetMaxRelevancy()
	{
		return base.Owner.BlackBoard.GoapSetup.FallbackRelevancy;
	}

	public override void CalculateGoalRelevancy()
	{
		base.GoalRelevancy = 0f;
		if ((base.Owner.BlackBoard.CoverTime != 0f && base.Owner.BlackBoard.CoverTime < 7f) || base.Owner.BlackBoard.DistanceToTarget > base.Owner.BlackBoard.WeaponRange || (base.Owner.WorldState.GetWSProperty(E_PropKey.CoverState).GetCoverState() != E_CoverState.None && base.Owner.BlackBoard.Fear < 50f) || base.Owner.BlackBoard.DangerousEnemy == null)
		{
			return;
		}
		float num = 0f;
		if (base.Owner.BlackBoard.DistanceToTarget < base.Owner.BlackBoard.CombatRange * 0.3f)
		{
			num = 1f;
		}
		else
		{
			float num2 = base.Owner.BlackBoard.DistanceToTarget - base.Owner.BlackBoard.CombatRange;
			num = 1f - num2 / (base.Owner.BlackBoard.WeaponRange - base.Owner.BlackBoard.CombatRange);
			num *= num;
			if (num < 0.5f)
			{
				return;
			}
		}
		if ((bool)base.Owner.BlackBoard.Desires.CoverFallback.Cover)
		{
			Cover cover = base.Owner.BlackBoard.Desires.CoverFallback.Cover;
			AdvancePos = cover.GetNearestPointOnCover(base.Owner.Position) - cover.Forward;
		}
		else
		{
			AiRecon.NearPositionData bestPositionInDirection = base.Owner.BlackBoard.AiRecon.GetBestPositionInDirection((base.Owner.Position - base.Owner.BlackBoard.DangerousEnemy.Position).normalized, 2f, 0f, false);
			if (bestPositionInDirection == null)
			{
				bestPositionInDirection = base.Owner.BlackBoard.AiRecon.GetBestPositionInDirection((base.Owner.Position - base.Owner.BlackBoard.DangerousEnemy.Position).normalized, 2f, 0f, false);
				if (bestPositionInDirection == null)
				{
					return;
				}
			}
			AdvancePos = bestPositionInDirection.Position;
		}
		num *= base.Owner.BlackBoard.Fear / 100f;
		num = Mathf.Clamp(num, 0f, 1f);
		base.GoalRelevancy = base.Owner.BlackBoard.GoapSetup.FallbackRelevancy * num;
		if (base.GoalRelevancy < 0.2f)
		{
			base.GoalRelevancy = 0f;
		}
	}

	public override void SetDisableTime()
	{
		base.NextEvaluationTime = base.Owner.BlackBoard.GoapSetup.FallbackDelay + Time.timeSinceLevelLoad;
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
		WorldStateProp wSProperty = base.Owner.WorldState.GetWSProperty(E_PropKey.TargetNode);
		if (wSProperty != null && (wSProperty.GetVector() - AdvancePos).sqrMagnitude < 0.02f)
		{
			return true;
		}
		return false;
	}

	public override bool Activate(GOAPPlan plan)
	{
		base.GoalRelevancy = base.Owner.BlackBoard.GoapSetup.FallbackRelevancy;
		base.Owner.WorldState.SetWSProperty(E_PropKey.AtTargetPos, false);
		base.Owner.WorldState.SetWSProperty(E_PropKey.TargetNode, AdvancePos);
		return base.Activate(plan);
	}

	public override void Deactivate()
	{
		base.Deactivate();
	}
}
