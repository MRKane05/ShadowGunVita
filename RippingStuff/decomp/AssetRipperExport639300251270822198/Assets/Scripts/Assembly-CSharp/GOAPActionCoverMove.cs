using UnityEngine;

internal class GOAPActionCoverMove : GOAPAction
{
	private AgentActionCoverMove Action;

	private Vector3 FinalPos;

	public GOAPActionCoverMove(AgentHuman owner)
		: base(E_GOAPAction.CoverMove, owner)
	{
	}

	public override void InitAction()
	{
		WorldEffects.SetWSProperty(E_PropKey.AtTargetPos, true);
		Cost = 5f;
		Precedence = 40;
	}

	public override bool ValidateContextPreconditions(WorldState current, bool planning)
	{
		E_CoverState coverState = Owner.WorldState.GetWSProperty(E_PropKey.CoverState).GetCoverState();
		if (coverState == E_CoverState.None)
		{
			return false;
		}
		if (Owner.BlackBoard.Desires.MoveDirection == Vector3.zero)
		{
			return false;
		}
		if (coverState == E_CoverState.Middle && Vector3.Dot(Owner.BlackBoard.Desires.MoveDirection, Owner.BlackBoard.Cover.Forward) < 0.6f)
		{
			return true;
		}
		if (Owner.BlackBoard.CoverPosition == E_CoverDirection.Left && Vector3.Dot(Owner.BlackBoard.Desires.MoveDirection, Owner.BlackBoard.Cover.Left) < 0.4f)
		{
			return true;
		}
		if (Owner.BlackBoard.CoverPosition == E_CoverDirection.Right && Vector3.Dot(Owner.BlackBoard.Desires.MoveDirection, Owner.BlackBoard.Cover.Right) < 0.4f)
		{
			return true;
		}
		return false;
	}

	public override void Update()
	{
		AgentActionCoverMove.E_Direction e_Direction = ((!(Vector3.Dot(Owner.BlackBoard.Desires.MoveDirection, Owner.Right) > 0f)) ? AgentActionCoverMove.E_Direction.Left : AgentActionCoverMove.E_Direction.Right);
		if (Action.Speed != Owner.BlackBoard.Desires.MoveSpeedModifier || Action.Direction != e_Direction)
		{
			Action = AgentActionFactory.Create(AgentActionFactory.E_Type.CoverMove) as AgentActionCoverMove;
			Action.Speed = Owner.BlackBoard.Desires.MoveSpeedModifier;
			Action.Direction = e_Direction;
			Owner.BlackBoard.ActionAdd(Action);
		}
	}

	public override void Activate()
	{
		base.Activate();
		Action = AgentActionFactory.Create(AgentActionFactory.E_Type.CoverMove) as AgentActionCoverMove;
		Action.Speed = Owner.BlackBoard.Desires.MoveSpeedModifier;
		if (Vector3.Dot(Owner.BlackBoard.Desires.MoveDirection, Owner.Right) > 0f)
		{
			Action.Direction = AgentActionCoverMove.E_Direction.Right;
		}
		else
		{
			Action.Direction = AgentActionCoverMove.E_Direction.Left;
		}
		Owner.BlackBoard.ActionAdd(Action);
	}

	public override void Deactivate()
	{
		base.Deactivate();
		Owner.WorldState.SetWSProperty(E_PropKey.AtTargetPos, true);
		AgentActionIdle action = AgentActionFactory.Create(AgentActionFactory.E_Type.Idle) as AgentActionIdle;
		Owner.BlackBoard.ActionAdd(action);
	}

	public override bool IsActionComplete()
	{
		if ((Action != null && !Action.IsActive()) || Owner.BlackBoard.Desires.MoveDirection == Vector3.zero || Owner.WorldState.GetWSProperty(E_PropKey.AtTargetPos).GetBool())
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
