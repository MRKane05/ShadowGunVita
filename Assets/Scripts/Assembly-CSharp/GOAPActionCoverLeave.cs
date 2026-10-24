using UnityEngine;

internal class GOAPActionCoverLeave : GOAPAction
{
	private AgentActionCoverLeave Action;

	private Vector3 FinalPos;

	public GOAPActionCoverLeave(AgentHuman owner)
		: base(E_GOAPAction.CoverLeave, owner)
	{
	}

	public override void InitAction()
	{
		WorldEffects.SetWSProperty(E_PropKey.CoverState, E_CoverState.None);
		Cost = 1f;
		Precedence = 60;
		Interruptible = false;
	}

	public override bool ValidateContextPreconditions(WorldState current, bool planning)
	{
		return true;
	}

	public override void SetPlanWSPreconditions(WorldState goalState)
	{
		base.SetPlanWSPreconditions(goalState);
		if (Owner.WeaponComponent.GetCurrentWeapon().WeaponAmmo > 0)
		{
			goalState.SetWSProperty(E_PropKey.WeaponLoaded, true);
		}
	}

	public override void Activate()
	{
		base.Activate();
		Action = AgentActionFactory.Create(AgentActionFactory.E_Type.CoverLeave) as AgentActionCoverLeave;
		Action.FinalViewDirection = Owner.BlackBoard.Cover.Forward;
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
		return !Action.IsActive();
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
