using UnityEngine;

internal class GOAPActionCheckEvent : GOAPAction
{
	private AgentHuman Target;

	private Fact CheckingFact;

	private float EndOfAction;

	public GOAPActionCheckEvent(AgentHuman owner)
		: base(E_GOAPAction.CheckEvent, owner)
	{
	}

	public override void InitAction()
	{
		WorldEffects.SetWSProperty(E_PropKey.Event, E_EventTypes.None);
		Cost = 1f;
		Precedence = 60;
	}

	public override bool ValidateContextPreconditions(WorldState current, bool planning)
	{
		if (Owner.WorldState.GetWSProperty(E_PropKey.SeeEnemy).GetBool())
		{
			return false;
		}
		CheckingFact = Owner.Memory.GetValidFact(E_EventTypes.EnemyInjuredMe);
		if (CheckingFact != null)
		{
			return true;
		}
		CheckingFact = Owner.Memory.GetValidFact(E_EventTypes.EnemyFire);
		if (CheckingFact != null)
		{
			return true;
		}
		CheckingFact = Owner.Memory.GetValidFact(E_EventTypes.EnemyHideInCover);
		if (CheckingFact != null)
		{
			return true;
		}
		CheckingFact = Owner.Memory.GetValidFact(E_EventTypes.EnemyStep);
		if (CheckingFact != null)
		{
			return true;
		}
		return false;
	}

	public override void Activate()
	{
		base.Activate();
		Vector3 lookRotation = CheckingFact.Agent.TransformTarget.position - Owner.TransformEye.position;
		lookRotation.Normalize();
		Owner.BlackBoard.Desires.Rotation.SetLookRotation(lookRotation);
		EndOfAction = Time.timeSinceLevelLoad + Random.Range(0.2f, 0.4f);
	}

	public override void Deactivate()
	{
		WorldStateProp wSProperty = Owner.WorldState.GetWSProperty(E_PropKey.Event);
		if (wSProperty != null && CheckingFact != null && wSProperty.GetEvent() == CheckingFact.Type)
		{
			Owner.WorldState.SetWSProperty(E_PropKey.Event, E_EventTypes.None);
		}
		CheckingFact = null;
		base.Deactivate();
	}

	public override bool IsActionComplete()
	{
		if (EndOfAction < Time.timeSinceLevelLoad)
		{
			return true;
		}
		return false;
	}

	public override bool ValidateAction()
	{
		if (Owner.WorldState.GetWSProperty(E_PropKey.SeeEnemy).GetBool())
		{
			return false;
		}
		return true;
	}

	protected override void DebugLogActivate()
	{
		Debug.Log(Time.timeSinceLevelLoad + " " + ToString() + " - Activated " + CheckingFact.Type);
	}
}
