using UnityEngine;

internal class GOAPGoalPlayAnim : GOAPGoal
{
	public GOAPGoalPlayAnim(AgentHuman owner)
		: base(E_GOAPGoals.PlayAnim, owner)
	{
	}

	public override void InitGoal()
	{
	}

	public override float GetMaxRelevancy()
	{
		return base.Owner.BlackBoard.GoapSetup.PlayAnimRelevancy;
	}

	public override void CalculateGoalRelevancy()
	{
		WorldStateProp wSProperty = base.Owner.WorldState.GetWSProperty(E_PropKey.PlayAnim);
		if (wSProperty != null && wSProperty.GetBool())
		{
			base.GoalRelevancy = base.Owner.BlackBoard.GoapSetup.PlayAnimRelevancy;
		}
		else
		{
			base.GoalRelevancy = 0f;
		}
	}

	public override void SetDisableTime()
	{
		base.NextEvaluationTime = base.Owner.BlackBoard.GoapSetup.CriticalInjuryDelay + Time.timeSinceLevelLoad;
	}

	public override void SetWSSatisfactionForPlanning(WorldState worldState)
	{
		worldState.SetWSProperty(E_PropKey.PlayAnim, false);
	}

	public override bool IsWSSatisfiedForPlanning(WorldState worldState)
	{
		WorldStateProp wSProperty = worldState.GetWSProperty(E_PropKey.PlayAnim);
		if (wSProperty != null && !wSProperty.GetBool())
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
