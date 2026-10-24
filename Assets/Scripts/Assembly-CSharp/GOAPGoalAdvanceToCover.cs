using UnityEngine;

internal class GOAPGoalAdvanceToCover : GOAPGoal
{
	private static float _DisabledForEverybodyTimer;

	private Vector3 AdvancePos;

	private Cover Cover;

	private E_CoverDirection CoverPosition;

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

	public GOAPGoalAdvanceToCover(AgentHuman owner)
		: base(E_GOAPGoals.Advance, owner)
	{
	}

	public override void InitGoal()
	{
		base.FailChance = 10;
		DisabledForEverybodyDelay = 2f;
	}

	public override float GetMaxRelevancy()
	{
		return base.Owner.BlackBoard.GoapSetup.AdvanceRelevancy;
	}

	public override void CalculateGoalRelevancy()
	{
		base.GoalRelevancy = 0f;
		if ((base.Owner.BlackBoard.CoverTime == 0f || !(base.Owner.BlackBoard.CoverTime < 7f)) && (!(base.Owner.BlackBoard.VisibleTarget == null) || !(base.Owner.BlackBoard.DangerousEnemy == null)) && !(base.Owner.BlackBoard.DistanceToTarget < base.Owner.BlackBoard.CombatRange * 0.8f) && !(base.Owner.BlackBoard.Desires.CoverAdvance.Cover == null))
		{
			float num = 0f;
			num = ((!(base.Owner.BlackBoard.DistanceToTarget > base.Owner.BlackBoard.WeaponRange)) ? (base.Owner.BlackBoard.Rage / 100f) : 1f);
			Cover = base.Owner.BlackBoard.Desires.CoverAdvance.Cover;
			CoverAIInfo coverAdvance = base.Owner.BlackBoard.Desires.CoverAdvance;
			if (coverAdvance.MiddleCoverValidity > coverAdvance.LeftCoverValidity && coverAdvance.MiddleCoverValidity > coverAdvance.RightCoverValidity)
			{
				AdvancePos = Cover.Transform.position + Cover.Forward * -1f;
				num *= coverAdvance.MiddleCoverValidity;
				CoverPosition = E_CoverDirection.Middle;
			}
			else if (coverAdvance.LeftCoverValidity > coverAdvance.MiddleCoverValidity && coverAdvance.LeftCoverValidity > coverAdvance.RightCoverValidity)
			{
				AdvancePos = Cover.LeftEdge + Cover.Forward * -0.7f;
				num *= coverAdvance.LeftCoverValidity;
				CoverPosition = E_CoverDirection.Left;
			}
			else
			{
				AdvancePos = Cover.RightEdge + Cover.Forward * -0.7f;
				num *= coverAdvance.RightCoverValidity;
				CoverPosition = E_CoverDirection.Right;
			}
			base.GoalRelevancy = base.Owner.BlackBoard.GoapSetup.AdvanceRelevancy * num;
			if (base.GoalRelevancy < 0.2f)
			{
				base.GoalRelevancy = 0f;
			}
		}
	}

	public override void SetDisableTime()
	{
		base.NextEvaluationTime = base.Owner.BlackBoard.GoapSetup.AdvanceDelay + Time.timeSinceLevelLoad;
	}

	public override void ChangeCurrentWSForPlanning(WorldState worldState)
	{
		worldState.SetWSProperty(E_PropKey.AtTargetPos, false);
		worldState.SetWSProperty(E_PropKey.TargetNode, AdvancePos);
	}

	public override void SetWSSatisfactionForPlanning(WorldState worldState)
	{
		worldState.SetWSProperty(E_PropKey.AtTargetPos, true);
		worldState.SetWSProperty(E_PropKey.TargetNode, AdvancePos);
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

	public override bool IsPlanValid()
	{
		if (Cover.IsLockedForEnemy(base.Owner, CoverPosition))
		{
			return false;
		}
		if (!base.Owner.WorldState.GetWSProperty(E_PropKey.AtTargetPos).GetBool() && !Cover.CanUseItAgainst(base.Owner, base.Owner.BlackBoard.VisibleTarget))
		{
			return false;
		}
		return base.IsPlanValid();
	}

	public override bool IsSatisfied()
	{
		return IsPlanFinished();
	}

	public override bool Activate(GOAPPlan plan)
	{
		if (Cover.IsLockedForEnemy(base.Owner, CoverPosition))
		{
			return false;
		}
		base.Owner.BlackBoard.Desires.LookAtTarget = false;
		base.Owner.WorldState.SetWSProperty(E_PropKey.AtTargetPos, false);
		base.Owner.WorldState.SetWSProperty(E_PropKey.TargetNode, AdvancePos);
		base.Owner.BlackBoard.SetFear(0f);
		base.Owner.BlackBoard.SetRage(0f);
		Cover.LockPosition(CoverPosition, base.Owner);
		GOAPAction action = base.Owner.GetAction(E_GOAPAction.CoverEnter);
		if (action != null)
		{
			base.Owner.BlackBoard.Desires.CoverSelected = Cover;
			base.Owner.BlackBoard.Desires.CoverPosition = CoverPosition;
			plan.PushBack(action);
		}
		base.GoalRelevancy = base.Owner.BlackBoard.GoapSetup.AdvanceRelevancy;
		return base.Activate(plan);
	}

	public override void Deactivate()
	{
		base.Deactivate();
		base.Owner.BlackBoard.Desires.LookAtTarget = true;
		if (base.Owner.WorldState.GetWSProperty(E_PropKey.CoverState).GetCoverState() == E_CoverState.None)
		{
			Cover.LockPosition(CoverPosition, null);
			base.Owner.BlackBoard.Cover = null;
			base.Owner.BlackBoard.CoverPosition = E_CoverDirection.Unknown;
		}
		else if (base.Owner.BlackBoard.DangerousEnemy == null)
		{
			base.Owner.BlackBoard.Desires.Rotation.SetLookRotation(Cover.Forward);
		}
		else
		{
			base.Owner.BlackBoard.Desires.Rotation.SetLookRotation((base.Owner.BlackBoard.DangerousEnemy.Position - Cover.Position).normalized);
		}
	}
}
