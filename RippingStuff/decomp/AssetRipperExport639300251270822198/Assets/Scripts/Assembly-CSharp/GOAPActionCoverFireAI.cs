using UnityEngine;

internal class GOAPActionCoverFireAI : GOAPAction
{
	protected enum E_State
	{
		Start = 0,
		Waiting = 1,
		Firing = 2,
		End = 3
	}

	private AgentActionCoverFire Action;

	private AgentActionCoverFireCancel ActionCancel;

	private float EndTime;

	protected E_State State;

	public GOAPActionCoverFireAI(AgentHuman owner)
		: base(E_GOAPAction.CoverFireAI, owner)
	{
	}

	public override void InitAction()
	{
		if (Owner.IsPlayer)
		{
			Debug.LogError(ToString() + " is only for AI not for " + Owner.name);
		}
		WorldEffects.SetWSProperty(E_PropKey.KillTarget, true);
		WorldPreconditions.SetWSProperty(E_PropKey.WeaponLoaded, true);
		Cost = 2f;
		Precedence = 60;
		Interruptible = false;
	}

	public override bool ValidateContextPreconditions(WorldState current, bool planning)
	{
		if (Owner.BlackBoard.Rage < Owner.BlackBoard.CombatSetup.RageMax * 0.7f)
		{
			return false;
		}
		if (Owner.BlackBoard.VisibleTarget == null)
		{
			return false;
		}
		return Owner.IsInCover;
	}

	public override void Update()
	{
		if (State == E_State.Waiting && (Owner.BlackBoard.VisibleTarget == null || Vector3.Dot(Owner.Forward, (Owner.BlackBoard.VisibleTarget.Position - Owner.Position).normalized) < 0.7f || !Owner.IsAlive || !Owner.BlackBoard.VisibleTarget.IsAlive))
		{
			ChangeStateToEnd();
		}
		UpdateStates();
	}

	protected void UpdateStates()
	{
		switch (State)
		{
		case E_State.Start:
			if (!Action.IsActive())
			{
				ChangeStateToFiring();
			}
			break;
		case E_State.Waiting:
			if (EndTime < Time.timeSinceLevelLoad)
			{
				ChangeStateToFiring();
			}
			break;
		case E_State.Firing:
			if (!Owner.WorldState.GetWSProperty(E_PropKey.WeaponLoaded).GetBool())
			{
				ChangeStateToEnd();
			}
			else if (EndTime < Time.timeSinceLevelLoad)
			{
				if (Random.Range(0, 100) < 30 || Owner.BlackBoard.Fear > 70f)
				{
					ChangeStateToEnd();
				}
				else
				{
					ChangeStateToWaiting();
				}
			}
			break;
		case E_State.End:
			break;
		}
	}

	public override void Activate()
	{
		base.Activate();
		Owner.BlackBoard.Desires.WeaponTriggerOn = false;
		Action = AgentActionFactory.Create(AgentActionFactory.E_Type.CoverFire) as AgentActionCoverFire;
		ActionCancel = null;
		Owner.BlackBoard.ActionAdd(Action);
		State = E_State.Start;
	}

	public override void Deactivate()
	{
		base.Deactivate();
		if (ActionCancel == null)
		{
			ActionCancel = AgentActionFactory.Create(AgentActionFactory.E_Type.CoverFireCancel) as AgentActionCoverFireCancel;
			Owner.BlackBoard.ActionAdd(ActionCancel);
		}
		Owner.BlackBoard.Desires.WeaponTriggerOn = false;
		Owner.WorldState.SetWSProperty(E_PropKey.KillTarget, false);
	}

	public override bool IsActionComplete()
	{
		if (ActionCancel != null && !ActionCancel.IsActive())
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

	private void ChangeStateToFiring()
	{
		int num = Random.Range(Owner.BlackBoard.AttackSetup.MinShortBurst, Owner.BlackBoard.AttackSetup.MaxShortBurst);
		EndTime = Owner.WeaponComponent.GetCurrentWeapon().FireTime * (float)num + Time.timeSinceLevelLoad;
		Owner.BlackBoard.Desires.WeaponTriggerOn = true;
		State = E_State.Firing;
	}

	private void ChangeStateToWaiting()
	{
		EndTime = Random.Range(0.5f, 1.5f) + Time.timeSinceLevelLoad;
		Owner.BlackBoard.Desires.WeaponTriggerOn = false;
		State = E_State.Waiting;
	}

	protected void ChangeStateToEnd()
	{
		Owner.BlackBoard.Desires.WeaponTriggerOn = false;
		ActionCancel = AgentActionFactory.Create(AgentActionFactory.E_Type.CoverFireCancel) as AgentActionCoverFireCancel;
		Owner.BlackBoard.ActionAdd(ActionCancel);
		State = E_State.End;
	}
}
