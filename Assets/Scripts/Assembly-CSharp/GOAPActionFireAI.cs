using UnityEngine;

internal class GOAPActionFireAI : GOAPAction
{
	private enum E_State
	{
		Waiting = 0,
		Firing = 1,
		End = 2
	}

	private float EndTime;

	private E_State State;

	public GOAPActionFireAI(AgentHuman owner)
		: base(E_GOAPAction.FireAI, owner)
	{
	}

	public override void InitAction()
	{
		WorldEffects.SetWSProperty(E_PropKey.KillTarget, true);
		WorldPreconditions.SetWSProperty(E_PropKey.LookingAtTarget, true);
		WorldPreconditions.SetWSProperty(E_PropKey.WeaponLoaded, true);
		Cost = 5f;
		Precedence = 30;
	}

	public override bool ValidateContextPreconditions(WorldState current, bool planning)
	{
		return Owner.WorldState.GetWSProperty(E_PropKey.CoverState).GetCoverState() == E_CoverState.None;
	}

	public override void Update()
	{
		if ((bool)Owner.BlackBoard.VisibleTarget)
		{
			Owner.BlackBoard.Desires.Rotation.SetLookRotation((Owner.BlackBoard.VisibleTarget.Position - Owner.Position).normalized);
		}
		UpdateState();
	}

	protected void UpdateState()
	{
		switch (State)
		{
		case E_State.Waiting:
			if (EndTime < Time.timeSinceLevelLoad)
			{
				ChangeStateToFiring();
			}
			break;
		case E_State.Firing:
			if (EndTime < Time.timeSinceLevelLoad)
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
		}
	}

	public override void Activate()
	{
		base.Activate();
		ChangeStateToFiring();
	}

	public override void Deactivate()
	{
		Owner.BlackBoard.Desires.WeaponTriggerOn = false;
		base.Deactivate();
	}

	public override bool IsActionComplete()
	{
		return State == E_State.End;
	}

	public override bool ValidateAction()
	{
		if (Owner.BlackBoard.VisibleTarget == null)
		{
			return false;
		}
		return Owner.IsAlive && Owner.WorldState.GetWSProperty(E_PropKey.WeaponLoaded).GetBool();
	}

	private void ChangeStateToFiring()
	{
		EndTime = Owner.WeaponComponent.GetCurrentWeapon().FireTime * (float)Random.Range(Owner.BlackBoard.AttackSetup.MinShortBurst, Owner.BlackBoard.AttackSetup.MaxShortBurst) + Time.timeSinceLevelLoad;
		Owner.BlackBoard.Desires.WeaponTriggerOn = true;
		State = E_State.Firing;
	}

	private void ChangeStateToWaiting()
	{
		EndTime = Random.Range(0.5f, 1.5f) + Time.timeSinceLevelLoad;
		Owner.BlackBoard.Desires.WeaponTriggerOn = false;
		State = E_State.Waiting;
	}

	private void ChangeStateToEnd()
	{
		Owner.BlackBoard.Desires.WeaponTriggerOn = false;
		State = E_State.End;
	}
}
