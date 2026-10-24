using UnityEngine;

internal class GOAPGoalSuppressiveFire : GOAPGoal
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

	public GOAPGoalSuppressiveFire(AgentHuman owner)
		: base(E_GOAPGoals.SuppressiveFire, owner)
	{
	}

	public override void InitGoal()
	{
		base.FailChance = 10;
		DisabledForEverybodyDelay = 1.5f;
	}

	public override float GetMaxRelevancy()
	{
		return base.Owner.BlackBoard.GoapSetup.SuppressiveFireRelevancy;
	}

	public override void CalculateGoalRelevancy()
	{
		base.GoalRelevancy = 0f;
		if ((base.Owner.BlackBoard.VisibleTarget != null && base.Owner.BlackBoard.DangerousEnemy == null) || !base.Owner.CanFire())
		{
			return;
		}
		Fact validFact = base.Owner.Memory.GetValidFact(E_EventTypes.EnemyHideInCover);
		if (validFact != null && base.Owner.CanProjectileHitTarget(validFact.Agent))
		{
			float num = validFact.Belief * validFact.Belief;
			base.GoalRelevancy = base.Owner.BlackBoard.GoapSetup.SuppressiveFireRelevancy * num;
			if (base.Owner.BlackBoard.MotionType != E_MotionType.None)
			{
				base.GoalRelevancy *= 0.5f;
			}
		}
	}

	public override void SetDisableTime()
	{
		base.NextEvaluationTime = base.Owner.BlackBoard.GoapSetup.SuppressiveFireDelay + Time.timeSinceLevelLoad;
	}

	public override void SetWSSatisfactionForPlanning(WorldState worldState)
	{
		worldState.SetWSProperty(E_PropKey.KillTarget, true);
	}

	public override bool IsWSSatisfiedForPlanning(WorldState worldState)
	{
		return worldState.GetWSProperty(E_PropKey.KillTarget).GetBool();
	}

	public override bool IsSatisfied()
	{
		return IsPlanFinished();
	}

	public override bool Activate(GOAPPlan plan)
	{
		base.GoalRelevancy = base.Owner.BlackBoard.GoapSetup.KillTargetRelevancy;
		base.Owner.BlackBoard.SetFear(0f);
		return base.Activate(plan);
	}

	public override void Deactivate()
	{
		base.Owner.BlackBoard.SetRage(0f);
		base.Deactivate();
	}
}
