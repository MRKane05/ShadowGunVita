using UnityEngine;

internal class GOAPActionGoTo : GOAPAction
{
	private AgentActionGoTo Action;

	private Vector3 Position;

	public GOAPActionGoTo(AgentHuman owner)
		: base(E_GOAPAction.Goto, owner)
	{
	}

	public override void InitAction()
	{
		WorldEffects.SetWSProperty(E_PropKey.AtTargetPos, true);
		Cost = 5f;
		Precedence = 20;
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
		if (Owner.WorldState.GetWSProperty(E_PropKey.CoverState).GetCoverState() != E_CoverState.None)
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
		WorldStateProp wSProperty = current.GetWSProperty(E_PropKey.TargetNode);
		if (wSProperty == null)
		{
			return false;
		}
		Position = wSProperty.GetVector();
		return true;
	}

	public override void Activate()
	{
		base.Activate();
		Action = AgentActionFactory.Create(AgentActionFactory.E_Type.Goto) as AgentActionGoTo;
		Action.MoveType = E_MoveType.Forward;
		Action.Motion = E_MotionType.Run;
		Action.FinalPosition = Position;
		if (Owner.BlackBoard.Desires.LookAtTarget)
		{
			if ((bool)Owner.BlackBoard.VisibleTarget)
			{
				Action.LookTarget = Owner.BlackBoard.VisibleTarget.Transform;
			}
			else if ((bool)Owner.BlackBoard.VisibleTarget)
			{
				Action.LookTarget = Owner.BlackBoard.DangerousEnemy.Transform;
			}
		}
		if (Action.LookTarget == null)
		{
			Action.UseNavMeshAgentRotation = true;
		}
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
}
