using UnityEngine;

internal class GOAPGoalGetOutOfCover : GOAPGoal
{
	private static float _DisabledForEverybodyTimer;

	protected override float DisabledForEverybodyTimer
	{
		get
		{
			return _DisabledForEverybodyTimer;
		}
		set
		{
			_DisabledForEverybodyTimer = value;
		}
	}

	public GOAPGoalGetOutOfCover(AgentHuman owner)
		: base(E_GOAPGoals.GetOutOfCover, owner)
	{
	}

	public override void InitGoal()
	{
		DisabledForEverybodyDelay = 2f;
		base.FailChance = 5;
	}

	public override float GetMaxRelevancy()
	{
		return base.Owner.BlackBoard.GoapSetup.CoverRelevancy;
	}

	public override void CalculateGoalRelevancy()
	{
		base.GoalRelevancy = 0f;
		if ((bool)base.Owner.BlackBoard.Cover && (bool)base.Owner.BlackBoard.DangerousEnemy)
		{
			float num = 0f;
			Fact validFact = base.Owner.Memory.GetValidFact(E_EventTypes.EnemyInjuredMe);
			if (base.Owner.WorldState.GetWSProperty(E_PropKey.CoverState).GetCoverState() != E_CoverState.Aim && validFact != null && validFact.Belief > 0.5f)
			{
				num = Random.Range(0.1f, (1f - base.Owner.BlackBoard.Health / base.Owner.BlackBoard.BaseSetup.MaxHealth) * 0.7f);
			}
			Vector3 vector = base.Owner.BlackBoard.DangerousEnemy.Position - base.Owner.Position;
			float magnitude = vector.magnitude;
			if (magnitude < 3f)
			{
				num += 1f - magnitude / 3f;
			}
			float num2 = Vector3.Dot(base.Owner.BlackBoard.Cover.Forward, vector.normalized);
			if (num2 < 0.2f)
			{
				num2 = Mathf.Clamp(num2, 0f, 0.2f);
				num += 1f - num2 / 0.2f;
			}
			base.GoalRelevancy = base.Owner.BlackBoard.GoapSetup.CoverRelevancy * num;
			base.GoalRelevancy = Mathf.Clamp(base.GoalRelevancy, 0f, base.Owner.BlackBoard.GoapSetup.CoverRelevancy);
			if (base.GoalRelevancy < 0.25f)
			{
				base.GoalRelevancy = 0f;
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
		return base.Activate(plan);
	}

	public override void Reset()
	{
		base.Reset();
	}
}
