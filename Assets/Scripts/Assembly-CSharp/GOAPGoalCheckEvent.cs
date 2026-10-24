using UnityEngine;

internal class GOAPGoalCheckEvent : GOAPGoal
{
	private static float _DisabledForEverybodyTimer;

	private E_EventTypes CheckingEvent;

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

	public GOAPGoalCheckEvent(AgentHuman owner)
		: base(E_GOAPGoals.CheckEvent, owner)
	{
	}

	public override void InitGoal()
	{
		base.FailChance = 10;
		DisabledForEverybodyDelay = 0.5f;
	}

	public override float GetMaxRelevancy()
	{
		return base.Owner.BlackBoard.GoapSetup.CheckEventRelevancy;
	}

	public override void CalculateGoalRelevancy()
	{
		base.GoalRelevancy = 0f;
		if (base.Owner.WorldState.GetWSProperty(E_PropKey.SeeEnemy).GetBool())
		{
			return;
		}
		Fact validFact = base.Owner.Memory.GetValidFact(E_EventTypes.EnemyInjuredMe);
		if (validFact != null)
		{
			base.GoalRelevancy = base.Owner.BlackBoard.GoapSetup.CheckEventRelevancy * validFact.Belief;
		}
		else
		{
			validFact = base.Owner.Memory.GetValidFact(E_EventTypes.EnemyHideInCover);
			if (validFact != null)
			{
				base.GoalRelevancy = base.Owner.BlackBoard.GoapSetup.CheckEventRelevancy * validFact.Belief * 0.7f;
			}
			else
			{
				validFact = base.Owner.Memory.GetValidFact(E_EventTypes.EnemyLost);
				if (validFact != null)
				{
					base.GoalRelevancy = base.Owner.BlackBoard.GoapSetup.CheckEventRelevancy * validFact.Belief * 0.9f;
				}
				else
				{
					validFact = base.Owner.Memory.GetValidFact(E_EventTypes.EnemyFire);
					if (validFact != null)
					{
						base.GoalRelevancy = base.Owner.BlackBoard.GoapSetup.CheckEventRelevancy * validFact.Belief * 0.8f;
					}
					else
					{
						validFact = base.Owner.Memory.GetValidFact(E_EventTypes.EnemyStep);
						if (validFact == null)
						{
							return;
						}
						base.GoalRelevancy = base.Owner.BlackBoard.GoapSetup.CheckEventRelevancy * validFact.Belief * 0.7f;
					}
				}
			}
		}
		CheckingEvent = validFact.Type;
		if ((CheckingEvent == E_EventTypes.EnemyFire || CheckingEvent == E_EventTypes.EnemyInjuredMe || CheckingEvent == E_EventTypes.EnemyFire) && Vector3.Dot((validFact.Position - base.Owner.Position).normalized, base.Owner.Forward) >= 0.75f)
		{
			base.GoalRelevancy = 0f;
			CheckingEvent = E_EventTypes.None;
		}
	}

	public override void SetDisableTime()
	{
		base.NextEvaluationTime = base.Owner.BlackBoard.GoapSetup.CheckEventDelay + Time.timeSinceLevelLoad;
	}

	public override void SetWSSatisfactionForPlanning(WorldState worldState)
	{
		worldState.SetWSProperty(E_PropKey.Event, E_EventTypes.None);
	}

	public override void ChangeCurrentWSForPlanning(WorldState worldState)
	{
		worldState.SetWSProperty(E_PropKey.Event, CheckingEvent);
	}

	public override bool IsWSSatisfiedForPlanning(WorldState worldState)
	{
		return worldState.GetWSProperty(E_PropKey.Event).GetEvent() == E_EventTypes.None;
	}

	public override bool IsSatisfied()
	{
		return IsPlanFinished();
	}

	public override bool ReplanRequired()
	{
		return false;
	}

	public override bool Activate(GOAPPlan plan)
	{
		base.GoalRelevancy = base.Owner.BlackBoard.GoapSetup.CheckEventRelevancy;
		return base.Activate(plan);
	}
}
