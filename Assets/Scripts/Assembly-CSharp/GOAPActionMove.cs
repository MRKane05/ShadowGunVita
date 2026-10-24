using UnityEngine;

internal class GOAPActionMove : GOAPAction
{
	private AgentActionMove Action;

	private Vector3 FinalPos;

	public GOAPActionMove(AgentHuman owner)
		: base(E_GOAPAction.Move, owner)
	{
	}

	public override void InitAction()
	{
		WorldEffects.SetWSProperty(E_PropKey.AtTargetPos, true);
		Cost = 5f;
		Precedence = 30;
	}

	public override bool ValidateContextPreconditions(WorldState current, bool planning)
	{
		if (Owner.IsInCover)
		{
			return false;
		}
		return Owner.BlackBoard.Desires.MoveDirection != Vector3.zero;
	}

	public override void Update()
	{
		if (!Owner.BlackBoard.ComlinkOn)
		{
			WorldStateProp wSProperty = Owner.WorldState.GetWSProperty(E_PropKey.WeaponLoaded);
			if (wSProperty != null && !wSProperty.GetBool() && Owner.WeaponComponent.GetCurrentWeapon().WeaponAmmo > 0)
			{
				AgentAction action = AgentActionFactory.Create(AgentActionFactory.E_Type.Reload) as AgentActionReload;
				Owner.BlackBoard.ActionAdd(action);
			}
		}
	}

	public override void Activate()
	{
		base.Activate();
		Action = AgentActionFactory.Create(AgentActionFactory.E_Type.Move) as AgentActionMove;
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
			Debug.Log(ToString() + " not valid anymore !" + FinalPos.ToString());
			return false;
		}
		return true;
	}
}
