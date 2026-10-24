using UnityEngine;

internal class GOAPGoalCoverMove : GOAPGoal
{
	public GOAPGoalCoverMove(AgentHuman owner)
		: base(E_GOAPGoals.CoverMove, owner)
	{
	}

	public override void InitGoal()
	{
	}

	public override float GetMaxRelevancy()
	{
		return base.Owner.BlackBoard.GoapSetup.GoToRelevancy;
	}

	public override void CalculateGoalRelevancy()
	{
		base.GoalRelevancy = 0f;
		if (base.Owner.BlackBoard.Cover == null || base.Owner.BlackBoard.MotionType != E_MotionType.None)
		{
			return;
		}
		WorldStateProp wSProperty = base.Owner.WorldState.GetWSProperty(E_PropKey.AtTargetPos);
		if (!(wSProperty != null) || !wSProperty.GetBool())
		{
			E_CoverState coverState = base.Owner.WorldState.GetWSProperty(E_PropKey.CoverState).GetCoverState();
			if ((coverState != E_CoverState.Middle || !(Vector3.Dot(base.Owner.BlackBoard.Desires.MoveDirection, base.Owner.BlackBoard.Cover.Forward) > 0.6f)) && (base.Owner.BlackBoard.CoverPosition != E_CoverDirection.Left || !(Vector3.Dot(base.Owner.BlackBoard.Desires.MoveDirection, base.Owner.BlackBoard.Cover.Left) > -0.4f)) && (base.Owner.BlackBoard.CoverPosition != E_CoverDirection.Right || !(Vector3.Dot(base.Owner.BlackBoard.Desires.MoveDirection, base.Owner.BlackBoard.Cover.Right) > -0.4f)))
			{
				base.GoalRelevancy = base.Owner.BlackBoard.GoapSetup.GoToRelevancy;
			}
		}
	}

	public override void SetDisableTime()
	{
		base.NextEvaluationTime = base.Owner.BlackBoard.GoapSetup.GoToDelay + Time.timeSinceLevelLoad;
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
