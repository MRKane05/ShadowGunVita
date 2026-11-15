using UnityEngine;

internal class GOAPGoalSuppressed : GOAPGoal
{
	public GOAPGoalSuppressed(AgentHuman owner)
		: base(E_GOAPGoals.Suppressed, owner)
	{
	}

	public override void InitGoal()
	{
	}

	public override float GetMaxRelevancy()
	{
		return base.Owner.BlackBoard.GoapSetup.SuppressedRelevancy;
	}

	public override void CalculateGoalRelevancy()
	{
		base.GoalRelevancy = 0f;
		base.GoalRelevancy = base.Owner.BlackBoard.GoapSetup.SuppressedRelevancy * (base.Owner.BlackBoard.Fear / 100f);
	}

	public override void SetDisableTime()
	{
		base.NextEvaluationTime = base.Owner.BlackBoard.GoapSetup.SuppressedDelay + Time.timeSinceLevelLoad;
	}

	public override void SetWSSatisfactionForPlanning(WorldState worldState)
	{
		worldState.SetWSProperty(E_PropKey.Start, true);
	}

	public override bool IsWSSatisfiedForPlanning(WorldState worldState)
	{
		return worldState.GetWSProperty(E_PropKey.Start).GetBool();
	}

	public override bool IsSatisfied()
	{
		return IsPlanFinished();
	}
}
