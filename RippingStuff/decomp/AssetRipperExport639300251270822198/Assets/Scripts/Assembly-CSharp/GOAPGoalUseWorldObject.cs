using UnityEngine;

internal class GOAPGoalUseWorldObject : GOAPGoal
{
	public GOAPGoalUseWorldObject(AgentHuman owner)
		: base(E_GOAPGoals.UseWorldObject, owner)
	{
	}

	public override void InitGoal()
	{
	}

	public override float GetMaxRelevancy()
	{
		return base.Owner.BlackBoard.GoapSetup.UseWorlObjectRelevancy;
	}

	public override void CalculateGoalRelevancy()
	{
		WorldStateProp wSProperty = base.Owner.WorldState.GetWSProperty(E_PropKey.UseWorldObject);
		if (wSProperty != null && wSProperty.GetBool())
		{
			base.GoalRelevancy = base.Owner.BlackBoard.GoapSetup.UseWorlObjectRelevancy;
		}
		else
		{
			base.GoalRelevancy = 0f;
		}
	}

	public override void SetDisableTime()
	{
		base.NextEvaluationTime = base.Owner.BlackBoard.GoapSetup.UseWorlObjectDelay + Time.timeSinceLevelLoad;
	}

	public override void SetWSSatisfactionForPlanning(WorldState worldState)
	{
		worldState.SetWSProperty(E_PropKey.UseWorldObject, false);
	}

	public override bool IsWSSatisfiedForPlanning(WorldState worldState)
	{
		WorldStateProp wSProperty = worldState.GetWSProperty(E_PropKey.UseWorldObject);
		if (!wSProperty.GetBool())
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
