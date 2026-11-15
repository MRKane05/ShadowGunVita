using UnityEngine;

internal class GOAPGoalCoverJumpOver : GOAPGoal
{
	private Vector3 AdvancePos;

	public GOAPGoalCoverJumpOver(AgentHuman owner)
		: base(E_GOAPGoals.CoverJumpOver, owner)
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
		if (!(base.Owner.BlackBoard.Cover == null) && base.Owner.BlackBoard.CoverPosition == E_CoverDirection.Middle && base.Owner.BlackBoard.Cover.CoverFlags.Get(Cover.E_CoverFlags.UpCrouch) && !(Vector3.Dot(base.Owner.BlackBoard.Cover.Forward, base.Owner.BlackBoard.Desires.MoveDirection) < 0.75f))
		{
			Cover oppositeCover = Mission.Instance.CurrentGameZone.GetOppositeCover(base.Owner.BlackBoard.Cover);
			if (!(oppositeCover == null) && oppositeCover.CoverFlags.Get(Cover.E_CoverFlags.UpCrouch))
			{
				AdvancePos = oppositeCover.Position - oppositeCover.Forward;
				base.GoalRelevancy = base.Owner.BlackBoard.GoapSetup.CoverRelevancy;
			}
		}
	}

	public override void SetDisableTime()
	{
		base.NextEvaluationTime = base.Owner.BlackBoard.GoapSetup.CoverDelay + Time.timeSinceLevelLoad;
	}

	public override void SetWSSatisfactionForPlanning(WorldState worldState)
	{
		worldState.SetWSProperty(E_PropKey.CoverState, E_CoverState.None);
		worldState.SetWSProperty(E_PropKey.TargetNode, AdvancePos);
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

	public override bool IsSatisfied()
	{
		return IsPlanFinished();
	}

	public override bool Activate(GOAPPlan plan)
	{
		base.Owner.WorldState.SetWSProperty(E_PropKey.TargetNode, AdvancePos);
		base.Owner.BlackBoard.SetFear(0f);
		return base.Activate(plan);
	}
}
