using UnityEngine;

internal class GOAPGoalHideInCover : GOAPGoal
{
	private static float _DisabledForEverybodyTimer;

	private Vector3 EntryCoverPos;

	private Cover Cover;

	private E_CoverDirection CoverPosition;

	private bool HaveToMove;

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

	public GOAPGoalHideInCover(AgentHuman owner)
		: base(E_GOAPGoals.HideInCover, owner)
	{
	}

	public override void InitGoal()
	{
		base.FailChance = 0;
		DisabledForEverybodyDelay = 2f;
	}

	public override float GetMaxRelevancy()
	{
		return base.Owner.BlackBoard.GoapSetup.CoverRelevancy;
	}

	public override void CalculateGoalRelevancy()
	{
		base.GoalRelevancy = 0f;
		WorldStateProp wSProperty = base.Owner.WorldState.GetWSProperty(E_PropKey.CoverState);
		if (wSProperty != null && wSProperty.GetCoverState() != E_CoverState.None)
		{
			return;
		}
		Cover = base.Owner.BlackBoard.Desires.CoverNear.Cover;
		CoverAIInfo coverNear = base.Owner.BlackBoard.Desires.CoverNear;
		if (!(Cover == null) && Cover.IsAvailableForEnemy())
		{
			float num = 1f;
			if (coverNear.MiddleCoverValidity > coverNear.LeftCoverValidity && coverNear.MiddleCoverValidity > coverNear.RightCoverValidity)
			{
				CoverPosition = E_CoverDirection.Middle;
				EntryCoverPos = Cover.Transform.position + Cover.Forward * -1f;
				num = coverNear.MiddleCoverValidity;
			}
			else if (coverNear.LeftCoverValidity > coverNear.MiddleCoverValidity && coverNear.LeftCoverValidity > coverNear.RightCoverValidity)
			{
				CoverPosition = E_CoverDirection.Left;
				EntryCoverPos = Cover.LeftEdge + Cover.Forward * -0.7f;
				num = coverNear.LeftCoverValidity;
			}
			else
			{
				CoverPosition = E_CoverDirection.Right;
				EntryCoverPos = Cover.RightEdge + Cover.Forward * -0.7f;
				num = coverNear.RightCoverValidity;
			}
			if (base.Owner.BlackBoard.IdleTimer < 10f)
			{
				num *= base.Owner.BlackBoard.Fear / 100f;
			}
			base.GoalRelevancy = base.Owner.BlackBoard.GoapSetup.HideInCoverRelevancy * num;
			base.GoalRelevancy = Mathf.Clamp(base.GoalRelevancy, 0f, base.Owner.BlackBoard.GoapSetup.HideInCoverRelevancy);
			if (base.GoalRelevancy < 0.2f)
			{
				base.GoalRelevancy = 0f;
			}
			HaveToMove = (base.Owner.Position - EntryCoverPos).magnitude > 0.3f;
		}
	}

	public override void SetDisableTime()
	{
		base.NextEvaluationTime = base.Owner.BlackBoard.GoapSetup.HideInCoverRelevancyDelay + Time.timeSinceLevelLoad;
	}

	public override void SetWSSatisfactionForPlanning(WorldState worldState)
	{
		worldState.SetWSProperty(E_PropKey.CoverState, E_CoverState.Middle);
		if (HaveToMove)
		{
			worldState.SetWSProperty(E_PropKey.AtTargetPos, true);
			worldState.SetWSProperty(E_PropKey.TargetNode, EntryCoverPos);
		}
	}

	public override void ChangeCurrentWSForPlanning(WorldState worldState)
	{
		if (HaveToMove)
		{
			worldState.SetWSProperty(E_PropKey.AtTargetPos, false);
		}
	}

	public override bool IsWSSatisfiedForPlanning(WorldState worldState)
	{
		if (HaveToMove)
		{
			return worldState.GetWSProperty(E_PropKey.CoverState).GetCoverState() != E_CoverState.None && worldState.GetWSProperty(E_PropKey.TargetNode).GetVector() == EntryCoverPos;
		}
		return worldState.GetWSProperty(E_PropKey.CoverState).GetCoverState() != E_CoverState.None;
	}

	public override bool IsSatisfied()
	{
		return IsPlanFinished();
	}

	public override bool IsPlanValid()
	{
		if (!base.Owner.WorldState.GetWSProperty(E_PropKey.AtTargetPos).GetBool() && !Cover.CanUseItAgainst(base.Owner, base.Owner.BlackBoard.VisibleTarget))
		{
			return false;
		}
		if (Cover.IsLockedForEnemy(base.Owner, CoverPosition))
		{
			return false;
		}
		Vector3 position;
		if ((bool)base.Owner.BlackBoard.VisibleTarget)
		{
			position = base.Owner.BlackBoard.VisibleTarget.Position;
		}
		else
		{
			if (!base.Owner.BlackBoard.DangerousEnemy)
			{
				return IsPlanFinished();
			}
			position = base.Owner.BlackBoard.DangerousEnemy.Position;
		}
		if ((position - base.Owner.Position).sqrMagnitude < 9f)
		{
			return false;
		}
		if ((position - EntryCoverPos).sqrMagnitude < 4f)
		{
			return false;
		}
		return base.IsPlanValid();
	}

	public override bool Activate(GOAPPlan plan)
	{
		if (Cover.IsLockedForEnemy(base.Owner, CoverPosition))
		{
			return false;
		}
		base.Owner.BlackBoard.Desires.LookAtTarget = false;
		base.Owner.SensorsComponent.DeactivateSensor(E_SensorType.CoverAI);
		base.Owner.BlackBoard.Desires.CoverSelected = Cover;
		base.Owner.BlackBoard.Desires.CoverPosition = CoverPosition;
		base.Owner.WorldState.SetWSProperty(E_PropKey.TargetNode, EntryCoverPos);
		Cover.LockPosition(CoverPosition, base.Owner);
		base.Owner.BlackBoard.SetFear(0f);
		base.Owner.BlackBoard.SetRage(0f);
		base.GoalRelevancy = base.Owner.BlackBoard.GoapSetup.HideInCoverRelevancy;
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
		base.Owner.SensorsComponent.ActivateSensor(E_SensorType.CoverAI);
	}

	public override bool IsPlanInterruptible()
	{
		if ((Cover.Position - base.Owner.Position).magnitude < 2f)
		{
			return false;
		}
		return base.IsPlanInterruptible();
	}
}
