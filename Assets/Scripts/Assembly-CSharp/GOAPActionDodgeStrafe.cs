using UnityEngine;
using UnityEngine.AI;

internal class GOAPActionDodgeStrafe : GOAPAction
{
	private AgentActionDodgeStrafe Action;

	private E_StrafeDirection StrafeDirection;

	public GOAPActionDodgeStrafe(AgentHuman owner)
		: base(E_GOAPAction.DodgeStrafe, owner)
	{
	}

	public override void InitAction()
	{
		WorldEffects.SetWSProperty(E_PropKey.InDodge, true);
		Cost = 5f;
		Precedence = 70;
	}

	public override bool ValidateContextPreconditions(WorldState current, bool planning)
	{
		if (Owner.IsInCover || !Owner.NavMeshAgent.enabled)
		{
			return false;
		}
		if (!Owner.WorldState.GetWSProperty(E_PropKey.LookingAtTarget).GetBool())
		{
			return false;
		}
		if (!Owner.WorldState.GetWSProperty(E_PropKey.EnemyLookingAtMe).GetBool() && Owner.Memory.GetValidFact(E_EventTypes.EnemyInjuredMe) == null)
		{
			return false;
		}
		NavMeshHit hit;
		if ((bool)Owner.BlackBoard.VisibleTarget)
		{
			if (Vector3.Dot(Owner.Right, Owner.BlackBoard.VisibleTarget.Forward) > 0f && !Owner.NavMeshAgent.Raycast(Owner.Position - Owner.Right * 2f, out hit))
			{
				StrafeDirection = E_StrafeDirection.Left;
			}
			else
			{
				if (Owner.NavMeshAgent.Raycast(Owner.Position + Owner.Right * 2f, out hit))
				{
					return false;
				}
				StrafeDirection = E_StrafeDirection.Right;
			}
		}
		else if (Random.Range(0, 100) > 50 && !Owner.NavMeshAgent.Raycast(Owner.Position - Owner.Right * 2f, out hit))
		{
			StrafeDirection = E_StrafeDirection.Left;
		}
		else
		{
			if (Owner.NavMeshAgent.Raycast(Owner.Position + Owner.Right * 2f, out hit))
			{
				return false;
			}
			StrafeDirection = E_StrafeDirection.Right;
		}
		return true;
	}

	public override void Activate()
	{
		base.Activate();
		Owner.BlackBoard.Desires.WeaponTriggerOn = false;
		Owner.WorldState.SetWSProperty(E_PropKey.InDodge, true);
		ActionStrafe();
	}

	public override void Deactivate()
	{
		Owner.WorldState.SetWSProperty(E_PropKey.InDodge, false);
		base.Deactivate();
	}

	public override bool IsActionComplete()
	{
		if (Action != null && !Action.IsActive())
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

	private void ActionStrafe()
	{
		Action = AgentActionFactory.Create(AgentActionFactory.E_Type.DodgeStrafe) as AgentActionDodgeStrafe;
		Action.Motion = E_MotionType.Run;
		Action.StrafeDirection = StrafeDirection;
		Action.AnimationDriven = true;
		Owner.BlackBoard.ActionAdd(Action);
	}
}
