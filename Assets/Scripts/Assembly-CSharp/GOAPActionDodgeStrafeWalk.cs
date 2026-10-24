using UnityEngine;
using UnityEngine.AI;

internal class GOAPActionDodgeStrafeWalk : GOAPAction
{
	private AgentActionGoTo Action;

	private Vector3 FinalPos;

	public GOAPActionDodgeStrafeWalk(AgentHuman owner)
		: base(E_GOAPAction.DodgeStrafeWalk, owner)
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
		if (!Owner.WorldState.GetWSProperty(E_PropKey.LookingAtTarget).GetBool() && Owner.WorldState.GetWSProperty(E_PropKey.Event).GetEvent() != E_EventTypes.EnemyInjuredMe)
		{
			return false;
		}
		AiRecon.NearPositionData bestPositionInDirection = Owner.BlackBoard.AiRecon.GetBestPositionInDirection(Owner.Right, 3f, 3f);
		AiRecon.NearPositionData bestPositionInDirection2 = Owner.BlackBoard.AiRecon.GetBestPositionInDirection(-Owner.Right, 3f, 3f);
		if (bestPositionInDirection2 != null && bestPositionInDirection != null)
		{
			if (Random.Range(0, 100) < 50)
			{
				FinalPos = bestPositionInDirection.Position;
			}
			else
			{
				FinalPos = bestPositionInDirection2.Position;
			}
		}
		else if (bestPositionInDirection != null)
		{
			FinalPos = bestPositionInDirection.Position;
		}
		else
		{
			if (bestPositionInDirection2 == null)
			{
				return false;
			}
			FinalPos = bestPositionInDirection2.Position;
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
		Action = AgentActionFactory.Create(AgentActionFactory.E_Type.Goto) as AgentActionGoTo;
		Action.Motion = E_MotionType.Walk;
		Action.FinalPosition = FinalPos;
		Action.LookTarget = ((!Owner.BlackBoard.DangerousEnemy) ? null : Owner.BlackBoard.DangerousEnemy.Transform);
		Owner.BlackBoard.ActionAdd(Action);
	}
}
