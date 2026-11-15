using UnityEngine;

internal class GOAPGoalCriticalInjury : GOAPGoal
{
	public GOAPGoalCriticalInjury(AgentHuman owner)
		: base(E_GOAPGoals.CriticalInjury, owner)
	{
	}

	public override void InitGoal()
	{
	}

	public override float GetMaxRelevancy()
	{
		return base.Owner.BlackBoard.GoapSetup.CriticalInjuryRelevancy;
	}

	public override void CalculateGoalRelevancy()
	{
		base.GoalRelevancy = 0f;
		WorldStateProp wSProperty = base.Owner.WorldState.GetWSProperty(E_PropKey.CriticalInjury);
		if (!(wSProperty == null) && wSProperty.GetBool())
		{
			base.GoalRelevancy = base.Owner.BlackBoard.GoapSetup.CriticalInjuryRelevancy;
		}
	}

	public override void SetDisableTime()
	{
		base.NextEvaluationTime = base.Owner.BlackBoard.GoapSetup.CriticalInjuryDelay + Time.timeSinceLevelLoad;
	}

	public override void SetWSSatisfactionForPlanning(WorldState worldState)
	{
		worldState.SetWSProperty(E_PropKey.CriticalInjury, false);
	}

	public override bool IsWSSatisfiedForPlanning(WorldState worldState)
	{
		WorldStateProp wSProperty = worldState.GetWSProperty(E_PropKey.CriticalInjury);
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
