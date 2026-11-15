using UnityEngine;

internal class GOAPGoalMove : GOAPGoal
{
	public GOAPGoalMove(AgentHuman owner)
		: base(E_GOAPGoals.Move, owner)
	{
	}

	public override void InitGoal()
	{
	}

	public override float GetMaxRelevancy()
	{
		return base.Owner.BlackBoard.GoapSetup.MoveRelevancy;
	}

	public override void CalculateGoalRelevancy()
	{
		base.GoalRelevancy = 0f;
		if (!base.Owner.BlackBoard.Cover && base.Owner.BlackBoard.MotionType == E_MotionType.None)
		{
			WorldStateProp wSProperty = base.Owner.WorldState.GetWSProperty(E_PropKey.AtTargetPos);
			if (!(wSProperty != null) || !wSProperty.GetBool())
			{
				base.GoalRelevancy = base.Owner.BlackBoard.GoapSetup.MoveRelevancy;
			}
		}
	}

	public override void SetDisableTime()
	{
		base.NextEvaluationTime = base.Owner.BlackBoard.GoapSetup.MoveDelay + Time.timeSinceLevelLoad;
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

	public override bool IsSatisfied()
	{
		WorldStateProp wSProperty = base.Owner.WorldState.GetWSProperty(E_PropKey.AtTargetPos);
		if (wSProperty != null && wSProperty.GetBool())
		{
			return true;
		}
		return false;
	}
}
