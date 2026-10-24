using UnityEngine;

internal class GOAPActionCoverFire : GOAPAction
{
	private AgentActionCoverFire Action;

	private AgentActionCoverFireCancel ActionCancel;

	public GOAPActionCoverFire(AgentHuman owner)
		: base(E_GOAPAction.CoverFire, owner)
	{
	}

	public override void InitAction()
	{
		if (!Owner.IsPlayer)
		{
			Debug.LogError(ToString() + " is only for player not for " + Owner.name);
		}
		WorldEffects.SetWSProperty(E_PropKey.KillTarget, true);
		WorldPreconditions.SetWSProperty(E_PropKey.WeaponLoaded, true);
		Cost = 5f;
		Precedence = 30;
	}

	public override bool ValidateContextPreconditions(WorldState current, bool planning)
	{
		if (!Owner.BlackBoard.Desires.WeaponTriggerOn)
		{
			return false;
		}
		if (Owner.WeaponComponent.GetCurrentWeapon().ClipAmmo == 0)
		{
			return false;
		}
		return Owner.WorldState.GetWSProperty(E_PropKey.CoverState).GetCoverState() != E_CoverState.None;
	}

	public override void Update()
	{
		if ((!Owner.WorldState.GetWSProperty(E_PropKey.WeaponLoaded).GetBool() || !Owner.BlackBoard.Desires.WeaponTriggerOn) && ActionCancel == null && !Owner.WeaponComponent.GetCurrentWeapon().IsBusy())
		{
			Owner.BlackBoard.Desires.WeaponTriggerOn = false;
			ActionCancel = AgentActionFactory.Create(AgentActionFactory.E_Type.CoverFireCancel) as AgentActionCoverFireCancel;
			Owner.BlackBoard.ActionAdd(ActionCancel);
		}
	}

	public override void Activate()
	{
		base.Activate();
		Action = AgentActionFactory.Create(AgentActionFactory.E_Type.CoverFire) as AgentActionCoverFire;
		ActionCancel = null;
		Owner.BlackBoard.ActionAdd(Action);
	}

	public override void Deactivate()
	{
		base.Deactivate();
		Owner.WorldState.SetWSProperty(E_PropKey.KillTarget, false);
	}

	public override bool IsActionComplete()
	{
		if (Action != null && !Action.IsActive() && ActionCancel != null && !ActionCancel.IsActive())
		{
			return true;
		}
		return false;
	}

	public override bool ValidateAction()
	{
		if (Action != null && Action.IsFailed())
		{
			Debug.Log(ToString() + " not valid anymore !");
			return false;
		}
		return true;
	}
}
