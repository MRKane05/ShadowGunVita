using UnityEngine;

internal class GOAPGoalCoverFire : GOAPGoal
{
	public GOAPGoalCoverFire(AgentHuman owner)
		: base(E_GOAPGoals.CoverFire, owner)
	{
	}

	public override void InitGoal()
	{
	}

	public override float GetMaxRelevancy()
	{
		return base.Owner.BlackBoard.GoapSetup.KillTargetRelevancy;
	}

	public override void CalculateGoalRelevancy()
	{
		base.GoalRelevancy = 0f;
		if (!base.Owner.CanFire() || !base.Owner.BlackBoard.Desires.WeaponTriggerOn)
		{
			return;
		}
		WorldStateProp wSProperty = base.Owner.WorldState.GetWSProperty(E_PropKey.CoverState);
		if (wSProperty.GetCoverState() == E_CoverState.Middle)
		{
			if (!base.Owner.BlackBoard.Cover.CoverFlags.Get(Cover.E_CoverFlags.UpCrouch))
			{
				return;
			}
		}
		else if (wSProperty.GetCoverState() != E_CoverState.Edge)
		{
			return;
		}
		base.GoalRelevancy = base.Owner.BlackBoard.GoapSetup.KillTargetRelevancy;
	}

	public override void SetDisableTime()
	{
		base.NextEvaluationTime = Time.timeSinceLevelLoad + 0.001f;
	}

	public override void SetWSSatisfactionForPlanning(WorldState worldState)
	{
		worldState.SetWSProperty(E_PropKey.KillTarget, true);
	}

	public override bool IsWSSatisfiedForPlanning(WorldState worldState)
	{
		WorldStateProp wSProperty = worldState.GetWSProperty(E_PropKey.KillTarget);
		if (wSProperty.GetBool())
		{
			return true;
		}
		return false;
	}

	public override bool IsSatisfied()
	{
		return IsPlanFinished();
	}
}
