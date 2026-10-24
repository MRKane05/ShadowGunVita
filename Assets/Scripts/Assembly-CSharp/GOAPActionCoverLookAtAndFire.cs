using UnityEngine;

internal class GOAPActionCoverLookAtAndFire : GOAPAction
{
	private AgentActionCoverFire Action;

	private AgentActionCoverFireCancel ActionCancel;

	private float TimeEndOfAction;

	private float TimeEndOfFireTime;

	public GOAPActionCoverLookAtAndFire(AgentHuman owner)
		: base(E_GOAPAction.CoverLookAtAndFire, owner)
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
		Cost = 5f;
		Precedence = 30;
		Interruptible = false;
	}

	public override bool ValidateContextPreconditions(WorldState current, bool planning)
	{
		if (Owner.BlackBoard.Rage < 50f)
		{
			return false;
		}
		if ((bool)Owner.BlackBoard.VisibleTarget)
		{
			return false;
		}
		if (Owner.Memory.HaveValidFact(E_EventTypes.EnemyFire, 0.1f))
		{
			return false;
		}
		return Owner.WorldState.GetWSProperty(E_PropKey.CoverState).GetCoverState() != E_CoverState.None && Owner.WorldState.GetWSProperty(E_PropKey.CoverState).GetCoverState() != E_CoverState.Aim;
	}

	public override void Update()
	{
		if ((bool)Owner.BlackBoard.VisibleTarget && Owner.BlackBoard.Rage > 70f && TimeEndOfFireTime < Time.timeSinceLevelLoad)
		{
			Owner.BlackBoard.Desires.WeaponTriggerOn = true;
			TimeEndOfFireTime = Time.timeSinceLevelLoad + Random.Range(0.6f, 1.2f);
		}
		if (TimeEndOfAction < Time.timeSinceLevelLoad && ActionCancel == null)
		{
			Owner.BlackBoard.Desires.WeaponTriggerOn = false;
			ActionCancel = AgentActionFactory.Create(AgentActionFactory.E_Type.CoverFireCancel) as AgentActionCoverFireCancel;
			Owner.BlackBoard.ActionAdd(ActionCancel);
		}
		if (TimeEndOfFireTime < Time.timeSinceLevelLoad)
		{
			Owner.BlackBoard.Desires.WeaponTriggerOn = false;
		}
		if (Owner.BlackBoard.Desires.WeaponTriggerOn && !Owner.WorldState.GetWSProperty(E_PropKey.WeaponLoaded).GetBool())
		{
			Owner.BlackBoard.Desires.WeaponTriggerOn = false;
			TimeEndOfAction = Time.timeSinceLevelLoad + Random.Range(0.2f, 1f);
		}
	}

	public override void Activate()
	{
		base.Activate();
		TimeEndOfAction = Random.Range(1.5f, 2f) + Time.timeSinceLevelLoad;
		TimeEndOfFireTime = Random.Range(0.3f, 1.2f) + Time.timeSinceLevelLoad;
		Action = AgentActionFactory.Create(AgentActionFactory.E_Type.CoverFire) as AgentActionCoverFire;
		ActionCancel = null;
		Owner.BlackBoard.ActionAdd(Action);
	}

	public override void Deactivate()
	{
		Owner.WorldState.SetWSProperty(E_PropKey.KillTarget, false);
		Owner.BlackBoard.Desires.WeaponTriggerOn = false;
		if (ActionCancel == null)
		{
			ActionCancel = AgentActionFactory.Create(AgentActionFactory.E_Type.CoverFireCancel) as AgentActionCoverFireCancel;
			Owner.BlackBoard.ActionAdd(ActionCancel);
		}
		base.Deactivate();
	}

	public override bool IsActionComplete()
	{
		if (Action != null && Action.IsSuccess() && ActionCancel != null && ActionCancel.IsSuccess())
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
}
