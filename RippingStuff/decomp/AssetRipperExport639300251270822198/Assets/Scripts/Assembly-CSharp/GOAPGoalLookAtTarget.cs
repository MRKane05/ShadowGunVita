using UnityEngine;

internal class GOAPGoalLookAtTarget : GOAPGoal
{
	public GOAPGoalLookAtTarget(AgentHuman owner)
		: base(E_GOAPGoals.LookAtTarget, owner)
	{
	}

	public override void InitGoal()
	{
	}

	public override float GetMaxRelevancy()
	{
		return base.Owner.BlackBoard.GoapSetup.LookAtTargetRelevancy;
	}

	public override void CalculateGoalRelevancy()
	{
		base.GoalRelevancy = 0f;
		if (!base.Owner.IsInCover && !base.Owner.WorldState.GetWSProperty(E_PropKey.LookingAtTarget).GetBool() && base.Owner.WorldState.GetWSProperty(E_PropKey.AtTargetPos).GetBool() && !(base.Owner.BlackBoard.DangerousEnemy == null) && base.Owner.CanProjectileHitTarget(base.Owner.BlackBoard.DangerousEnemy))
		{
			base.GoalRelevancy = Mathf.Min(base.Owner.BlackBoard.GoapSetup.LookAtTargetRelevancy, Vector3.Angle(base.Owner.Forward, (base.Owner.BlackBoard.DangerousEnemy.Position - base.Owner.Position).normalized) * 0.01f);
			if (base.GoalRelevancy < 0.2f)
			{
				base.GoalRelevancy = 0f;
			}
		}
	}

	public override void SetDisableTime()
	{
		base.NextEvaluationTime = base.Owner.BlackBoard.GoapSetup.LookAtTargetDelay + Time.timeSinceLevelLoad;
	}

	public override void SetWSSatisfactionForPlanning(WorldState worldState)
	{
		worldState.SetWSProperty(E_PropKey.LookingAtTarget, true);
	}

	public override bool IsWSSatisfiedForPlanning(WorldState worldState)
	{
		WorldStateProp wSProperty = worldState.GetWSProperty(E_PropKey.LookingAtTarget);
		if (wSProperty != null && wSProperty.GetBool())
		{
			return true;
		}
		return false;
	}

	public override bool IsSatisfied()
	{
		WorldStateProp wSProperty = base.Owner.WorldState.GetWSProperty(E_PropKey.LookingAtTarget);
		if (wSProperty.GetBool())
		{
			return true;
		}
		return IsPlanFinished();
	}

	public override bool ReplanRequired()
	{
		return false;
	}

	public override bool Activate(GOAPPlan plan)
	{
		return base.Activate(plan);
	}
}
