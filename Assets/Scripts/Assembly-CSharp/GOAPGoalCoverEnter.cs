using UnityEngine;

internal class GOAPGoalCoverEnter : GOAPGoal
{
	public GOAPGoalCoverEnter(AgentHuman owner)
		: base(E_GOAPGoals.CoverEnter, owner)
	{
	}

	public override void InitGoal()
	{
	}

	public override float GetMaxRelevancy()
	{
		return base.Owner.BlackBoard.GoapSetup.CoverRelevancy;
	}

	public override void CalculateGoalRelevancy()
	{
		base.GoalRelevancy = 0f;
		if (base.Owner.BlackBoard.MotionType == E_MotionType.None)
		{
			WorldStateProp wSProperty = base.Owner.WorldState.GetWSProperty(E_PropKey.CoverState);
			if (!(wSProperty == null) && wSProperty.GetCoverState() == E_CoverState.None && !(base.Owner.BlackBoard.Desires.CoverNear.Cover == null))
			{
				base.GoalRelevancy = base.Owner.BlackBoard.GoapSetup.CoverRelevancy;
				base.Owner.BlackBoard.Desires.CoverSelected = base.Owner.BlackBoard.Desires.CoverNear.Cover;
			}
		}
	}

	public override void SetDisableTime()
	{
		base.NextEvaluationTime = base.Owner.BlackBoard.GoapSetup.CoverDelay + Time.timeSinceLevelLoad;
	}

	public override void SetWSSatisfactionForPlanning(WorldState worldState)
	{
		worldState.SetWSProperty(E_PropKey.CoverState, E_CoverState.Middle);
		worldState.SetWSProperty(base.Owner.WorldState.GetWSProperty(E_PropKey.TargetNode));
	}

	public override bool IsWSSatisfiedForPlanning(WorldState worldState)
	{
		WorldStateProp wSProperty = worldState.GetWSProperty(E_PropKey.CoverState);
		if (wSProperty.GetCoverState() != E_CoverState.None)
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
		base.Owner.BlackBoard.Desires.CoverSelected.LockPosition(E_CoverDirection.Unknown, base.Owner);
		Cover coverSelected = base.Owner.BlackBoard.Desires.CoverSelected;
		Vector3 nearestPointOnCover = coverSelected.GetNearestPointOnCover(base.Owner.Position);
		if (coverSelected.IsRightAllowed && Vector3.Magnitude(nearestPointOnCover - coverSelected.RightEdge) < 0.3f)
		{
			base.Owner.BlackBoard.Desires.CoverPosition = E_CoverDirection.Right;
		}
		else if (coverSelected.IsLeftAllowed && Vector3.Magnitude(nearestPointOnCover - coverSelected.LeftEdge) < 0.3f)
		{
			base.Owner.BlackBoard.Desires.CoverPosition = E_CoverDirection.Left;
		}
		else
		{
			base.Owner.BlackBoard.Desires.CoverPosition = E_CoverDirection.Unknown;
		}
		return base.Activate(plan);
	}

	public override void Deactivate()
	{
		base.Deactivate();
		if (base.Owner.WorldState.GetWSProperty(E_PropKey.CoverState).GetCoverState() == E_CoverState.None)
		{
			base.Owner.BlackBoard.Cover.LockPosition(E_CoverDirection.Unknown, null);
		}
	}
}
