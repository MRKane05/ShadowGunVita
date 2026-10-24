using UnityEngine;

internal class GOAPActionCoverLeaveRightLeft : GOAPAction
{
	private AgentActionCoverLeave Action;

	private Vector3 FinalPos;

	public GOAPActionCoverLeaveRightLeft(AgentHuman owner)
		: base(E_GOAPAction.CoverLeaveRightLeft, owner)
	{
	}

	public override void InitAction()
	{
		WorldEffects.SetWSProperty(E_PropKey.CoverState, E_CoverState.None);
		Cost = 1f;
		Precedence = 10;
		Interruptible = false;
	}

	public override bool ValidateContextPreconditions(WorldState current, bool planning)
	{
		if (!planning)
		{
			return true;
		}
		if (Owner.BlackBoard.Cover == null)
		{
			return false;
		}
		if (Owner.BlackBoard.CoverPosition == E_CoverDirection.Middle)
		{
			return false;
		}
		if (Vector3.Dot(Owner.BlackBoard.Desires.MoveDirection, Owner.BlackBoard.Cover.Forward) < 0.4f)
		{
			return false;
		}
		return true;
	}

	public override void Activate()
	{
		base.Activate();
		Action = AgentActionFactory.Create(AgentActionFactory.E_Type.CoverLeave) as AgentActionCoverLeave;
		Action.FinalViewDirection = Owner.BlackBoard.Cover.Forward;
		if (Owner.BlackBoard.CoverPosition == E_CoverDirection.Left)
		{
			Action.TypeOfLeave = AgentActionCoverLeave.E_Type.Left;
		}
		else
		{
			Action.TypeOfLeave = AgentActionCoverLeave.E_Type.Right;
		}
		Action.Cover = Owner.BlackBoard.Cover;
		Owner.BlackBoard.ActionAdd(Action);
	}

	public override bool IsActionComplete()
	{
		return Action != null && !Action.IsActive();
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
