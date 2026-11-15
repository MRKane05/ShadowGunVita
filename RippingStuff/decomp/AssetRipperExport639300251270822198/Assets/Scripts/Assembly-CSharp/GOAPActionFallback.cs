using UnityEngine;

internal class GOAPActionFallback : GOAPAction
{
	private AgentActionGoTo Action;

	private Vector3 FinalPos;

	private AgentHuman DangerousEnemy;

	public GOAPActionFallback(AgentHuman owner)
		: base(E_GOAPAction.Fallback, owner)
	{
	}

	public override void InitAction()
	{
		WorldEffects.SetWSProperty(E_PropKey.AtTargetPos, true);
		Cost = 2f;
		Precedence = 30;
	}

	public override void SetPlanWSPreconditions(WorldState goalState)
	{
		base.SetPlanWSPreconditions(goalState);
		if (Owner.WorldState.GetWSProperty(E_PropKey.CoverState).GetCoverState() != 0)
		{
			goalState.SetWSProperty(E_PropKey.CoverState, E_CoverState.None);
		}
	}

	public override bool ValidateContextPreconditions(WorldState current, bool planning)
	{
		if (!planning)
		{
			return true;
		}
		DangerousEnemy = Owner.BlackBoard.DangerousEnemy;
		if (DangerousEnemy == null)
		{
			return false;
		}
		WorldStateProp wSProperty = current.GetWSProperty(E_PropKey.TargetNode);
		if (wSProperty == null)
		{
			return false;
		}
		FinalPos = wSProperty.GetVector();
		Vector3 lhs = DangerousEnemy.Position - Owner.Position;
		Vector3 rhs = wSProperty.GetVector() - Owner.Position;
		lhs.Normalize();
		rhs.Normalize();
		float num = Vector3.Dot(lhs, rhs);
		if (num > -0.5f)
		{
			return false;
		}
		return true;
	}

	public override void Update()
	{
		if (!Owner.BlackBoard.CombatSetup.DontFireWhenRunning)
		{
			if (Owner.BlackBoard.Rage > 70f)
			{
				Owner.BlackBoard.Desires.WeaponTriggerOn = true;
			}
			else if (Owner.BlackBoard.Rage < 50f)
			{
				Owner.BlackBoard.Desires.WeaponTriggerOn = false;
			}
		}
	}

	public override void Activate()
	{
		base.Activate();
		Action = AgentActionFactory.Create(AgentActionFactory.E_Type.Goto) as AgentActionGoTo;
		Action.MoveType = E_MoveType.Backward;
		Action.Motion = E_MotionType.Run;
		Action.LookTarget = ((!DangerousEnemy) ? null : DangerousEnemy.Transform);
		Action.FinalPosition = FinalPos;
		Owner.BlackBoard.ActionAdd(Action);
	}

	public override void Deactivate()
	{
		base.Deactivate();
		if (Action != null && Action.IsActive())
		{
			AgentActionIdle action = AgentActionFactory.Create(AgentActionFactory.E_Type.Idle) as AgentActionIdle;
			Owner.BlackBoard.ActionAdd(action);
		}
		Owner.WorldState.SetWSProperty(E_PropKey.AtTargetPos, true);
		Owner.BlackBoard.Desires.WeaponTriggerOn = false;
	}

	public override bool IsActionComplete()
	{
		if (Action != null && Action.IsSuccess())
		{
			return true;
		}
		return false;
	}

	public override bool ValidateAction()
	{
		if (Action != null && Action.IsFailed())
		{
			return false;
		}
		return Owner.IsAlive;
	}
}
