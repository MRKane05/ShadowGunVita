using UnityEngine;

internal class GOAPActionShielderGoTo : GOAPAction
{
	private AgentActionGoToByAnim Action;

	public GOAPActionShielderGoTo(AgentHuman owner)
		: base(E_GOAPAction.ShielderGoto, owner)
	{
	}

	public override void InitAction()
	{
		WorldEffects.SetWSProperty(E_PropKey.AtTargetPos, true);
		Cost = 1f;
		Precedence = 0;
		Interruptible = true;
	}

	public override void SolvePlanWSVariable(WorldState currentState, WorldState goalState)
	{
		base.SolvePlanWSVariable(currentState, goalState);
		WorldStateProp wSProperty = goalState.GetWSProperty(E_PropKey.TargetNode);
		if (wSProperty != null)
		{
			currentState.SetWSProperty(wSProperty);
		}
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
		if (Owner.WorldState.GetWSProperty(E_PropKey.DoSpawnAction).GetBool())
		{
			return false;
		}
		WorldStateProp wSProperty = current.GetWSProperty(E_PropKey.TargetNode);
		if (wSProperty == null)
		{
			return false;
		}
		return true;
	}

	public override void Activate()
	{
		base.Activate();
		Action = AgentActionFactory.Create(AgentActionFactory.E_Type.GotoByAnim) as AgentActionGoToByAnim;
		Action.MoveType = GetMoveType();
		Action.FinalPosition = Owner.WorldState.GetWSProperty(E_PropKey.TargetNode).GetVector();
		Owner.BlackBoard.ActionAdd(Action);
	}

	public override void Deactivate()
	{
		Owner.WorldState.SetWSProperty(E_PropKey.AtTargetPos, true);
		if (Action != null && Action.IsActive())
		{
			AgentActionIdle action = AgentActionFactory.Create(AgentActionFactory.E_Type.Idle) as AgentActionIdle;
			Owner.BlackBoard.ActionAdd(action);
		}
		if (Owner.BlackBoard.DangerousEnemy != null)
		{
			Owner.BlackBoard.Desires.Rotation.SetLookRotation((Owner.BlackBoard.DangerousEnemy.Position - Owner.Position).normalized);
		}
		base.Deactivate();
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
		return true;
	}

	private E_MoveType GetMoveType()
	{
		Transform transform = Owner.Transform;
		Vector2 from = new Vector2(transform.forward.x, transform.forward.z);
		Vector2 from2 = new Vector2(transform.right.x, transform.right.z);
		Vector3 normalized = (Owner.WorldState.GetWSProperty(E_PropKey.TargetNode).GetVector() - Owner.Position).normalized;
		Vector2 to = new Vector2(normalized.x, normalized.z);
		float num = Vector2.Angle(from, to);
		float num2 = Vector2.Angle(from2, to);
		if (num < 45f)
		{
			return E_MoveType.Forward;
		}
		if (num > 135f)
		{
			return E_MoveType.Backward;
		}
		if (num2 < 45f)
		{
			return E_MoveType.StrafeRight;
		}
		return E_MoveType.StrafeLeft;
	}
}
