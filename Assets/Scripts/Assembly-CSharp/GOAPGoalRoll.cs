using UnityEngine;

internal class GOAPGoalRoll : GOAPGoal
{
	private float WorldStateTime;

	public GOAPGoalRoll(AgentHuman owner)
		: base(E_GOAPGoals.Roll, owner)
	{
	}

	public override void InitGoal()
	{
	}

	public override float GetMaxRelevancy()
	{
		return base.Owner.BlackBoard.GoapSetup.DodgeRelevancy;
	}

	public override void CalculateGoalRelevancy()
	{
		base.GoalRelevancy = 0f;
		if (base.Owner.WorldState.GetWSProperty(E_PropKey.InDodge).GetBool() && !base.Owner.IsInCover)
		{
			base.GoalRelevancy = base.Owner.BlackBoard.GoapSetup.DodgeRelevancy;
		}
	}

	public override void SetDisableTime()
	{
		base.NextEvaluationTime = base.Owner.BlackBoard.GoapSetup.DodgeDelay + Time.timeSinceLevelLoad;
	}

	public override void SetWSSatisfactionForPlanning(WorldState worldState)
	{
		worldState.SetWSProperty(E_PropKey.InDodge, false);
	}

	public override bool IsWSSatisfiedForPlanning(WorldState worldState)
	{
		return !worldState.GetWSProperty(E_PropKey.InDodge).GetBool();
	}

	public override bool IsSatisfied()
	{
		return IsPlanFinished();
	}
}
