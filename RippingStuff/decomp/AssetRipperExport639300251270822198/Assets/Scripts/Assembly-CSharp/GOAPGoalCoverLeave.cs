using UnityEngine;

internal class GOAPGoalCoverLeave : GOAPGoal
{
	public GOAPGoalCoverLeave(AgentHuman owner)
		: base(E_GOAPGoals.CoverLeave, owner)
	{
	}

	public override void InitGoal()
	{
		if (!base.Owner.IsPlayer)
		{
			Debug.LogError(ToString() + "  is only for player !!!!");
		}
	}

	public override float GetMaxRelevancy()
	{
		return base.Owner.BlackBoard.GoapSetup.CoverRelevancy;
	}

	public override void CalculateGoalRelevancy()
	{
		base.GoalRelevancy = 0f;
		if (base.Owner.BlackBoard.Cover == null || base.Owner.BlackBoard.Desires.MoveDirection == Vector3.zero)
		{
			return;
		}
		float num = Vector3.Dot(base.Owner.BlackBoard.Cover.Forward, base.Owner.BlackBoard.Desires.MoveDirection);
		if (num > 0.75f)
		{
			if (base.Owner.BlackBoard.CoverPosition == E_CoverDirection.Middle)
			{
				if (!base.Owner.BlackBoard.Cover.CoverFlags.Get(Cover.E_CoverFlags.UpCrouch))
				{
					return;
				}
				Cover oppositeCover = Mission.Instance.CurrentGameZone.GetOppositeCover(base.Owner.BlackBoard.Cover);
				if (oppositeCover == null || !oppositeCover.CoverFlags.Get(Cover.E_CoverFlags.UpCrouch))
				{
					return;
				}
			}
		}
		else if (num > -0.75f)
		{
			return;
		}
		base.GoalRelevancy = base.Owner.BlackBoard.GoapSetup.CoverRelevancy;
	}

	public override void SetDisableTime()
	{
		base.NextEvaluationTime = base.Owner.BlackBoard.GoapSetup.CoverDelay + Time.timeSinceLevelLoad;
	}

	public override void SetWSSatisfactionForPlanning(WorldState worldState)
	{
		worldState.SetWSProperty(E_PropKey.CoverState, E_CoverState.None);
	}

	public override bool IsWSSatisfiedForPlanning(WorldState worldState)
	{
		WorldStateProp wSProperty = worldState.GetWSProperty(E_PropKey.CoverState);
		if (wSProperty.GetCoverState() == E_CoverState.None)
		{
			return true;
		}
		return false;
	}

	public override bool Activate(GOAPPlan plan)
	{
		base.Owner.BlackBoard.SetFear(0f);
		return base.Activate(plan);
	}

	public override bool IsSatisfied()
	{
		return IsPlanFinished();
	}
}
