using UnityEngine;

internal class GOAPActionCoverJumpOver : GOAPAction
{
	private AgentActionCoverLeave Action;

	private Vector3 FinalPos;

	public GOAPActionCoverJumpOver(AgentHuman owner)
		: base(E_GOAPAction.CoverJumpOver, owner)
	{
	}

	public override void InitAction()
	{
		WorldEffects.SetWSProperty(E_PropKey.CoverState, E_CoverState.None);
		Cost = 1f;
		Precedence = 40;
		Interruptible = false;
	}

	public override bool ValidateContextPreconditions(WorldState current, bool planning)
	{
		if (!planning)
		{
			return true;
		}
		if (Owner.BlackBoard.DangerousEnemy == null)
		{
			return false;
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
		WorldStateProp wSProperty = current.GetWSProperty(E_PropKey.TargetNode);
		if (wSProperty != null)
		{
			if (Vector3.Dot(Owner.BlackBoard.Cover.Forward, (wSProperty.GetVector() - Owner.BlackBoard.Cover.Position).normalized) < 0.3f)
			{
				return false;
			}
		}
		else if (Vector3.Dot(Owner.BlackBoard.Cover.Forward, (Owner.BlackBoard.DangerousEnemy.Position - Owner.BlackBoard.Cover.Position).normalized) > 0f)
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
		Action.FinalViewDirection = Owner.BlackBoard.DirToTarget;
		Action.Cover = Owner.BlackBoard.Cover;
		Owner.BlackBoard.ActionAdd(Action);
	}

	public override void Deactivate()
	{
		base.Deactivate();
		if (Action.IsSuccess())
		{
			Owner.SetCoverCompromised(Action.Cover);
		}
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
