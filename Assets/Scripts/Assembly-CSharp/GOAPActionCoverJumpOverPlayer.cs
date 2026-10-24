using UnityEngine;

internal class GOAPActionCoverJumpOverPlayer : GOAPAction
{
	private AgentActionCoverLeave Action;

	private Vector3 FinalPos;

	public GOAPActionCoverJumpOverPlayer(AgentHuman owner)
		: base(E_GOAPAction.CoverJumpOverPlayer, owner)
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
		if (Owner.BlackBoard.CoverPosition != E_CoverDirection.Middle)
		{
			return false;
		}
		if (!Owner.BlackBoard.Cover.CoverFlags.Get(Cover.E_CoverFlags.UpCrouch))
		{
			return false;
		}
		if (Vector3.Dot(Owner.BlackBoard.Desires.MoveDirection, Owner.BlackBoard.Cover.Forward) < 0.4f)
		{
			return false;
		}
		Cover oppositeCover = Mission.Instance.CurrentGameZone.GetOppositeCover(Owner.BlackBoard.Cover);
		if (oppositeCover == null || !oppositeCover.CoverFlags.Get(Cover.E_CoverFlags.UpCrouch))
		{
			return false;
		}
		return true;
	}

	public override void Update()
	{
	}

	public override void Activate()
	{
		base.Activate();
		Action = AgentActionFactory.Create(AgentActionFactory.E_Type.CoverLeave) as AgentActionCoverLeave;
		Action.TypeOfLeave = AgentActionCoverLeave.E_Type.Jump;
		Action.FinalViewDirection = Owner.BlackBoard.Cover.Forward;
		Action.Cover = Owner.BlackBoard.Cover;
		Owner.BlackBoard.ActionAdd(Action);
	}

	public override void Deactivate()
	{
		base.Deactivate();
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
