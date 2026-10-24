using UnityEngine;

internal class GOAPGoalRunAway : GOAPGoal
{
	public GOAPGoalRunAway(AgentHuman owner)
		: base(E_GOAPGoals.RunAway, owner)
	{
	}

	public override void InitGoal()
	{
		base.FailChance = 0;
	}

	public override float GetMaxRelevancy()
	{
		return base.Owner.BlackBoard.GoapSetup.RunAwayRelevancy;
	}

	public override void CalculateGoalRelevancy()
	{
		base.GoalRelevancy = 0f;
		if (!(base.Owner.BlackBoard.Fear < 50f) && base.Owner.WorldState.GetWSProperty(E_PropKey.SeeEnemy).GetBool())
		{
			float num = base.Owner.BlackBoard.Fear / 100f;
			base.GoalRelevancy = base.Owner.BlackBoard.GoapSetup.RunAwayRelevancy * num;
			if (base.GoalRelevancy < 0.2f)
			{
				base.GoalRelevancy = 0f;
			}
		}
	}

	public override void SetDisableTime()
	{
		base.NextEvaluationTime = base.Owner.BlackBoard.GoapSetup.RunAwayDelay + Time.timeSinceLevelLoad;
	}

	public override void SetWSSatisfactionForPlanning(WorldState worldState)
	{
		worldState.SetWSProperty(E_PropKey.SeeEnemy, false);
	}

	public override bool IsWSSatisfiedForPlanning(WorldState worldState)
	{
		WorldStateProp wSProperty = worldState.GetWSProperty(E_PropKey.SeeEnemy);
		if (wSProperty != null && !wSProperty.GetBool())
		{
			return true;
		}
		return false;
	}

	public override bool Activate(GOAPPlan plan)
	{
		base.Owner.BlackBoard.SetRage(0f);
		base.GoalRelevancy = 1f;
		return base.Activate(plan);
	}

	public override bool IsSatisfied()
	{
		return IsPlanFinished();
	}
}
