using UnityEngine;

internal class GOAPActionSuppressed : GOAPAction
{
	public float TimeToEnd;

	public GOAPActionSuppressed(AgentHuman owner)
		: base(E_GOAPAction.Suppressed, owner)
	{
	}

	public override void InitAction()
	{
		WorldEffects.SetWSProperty(E_PropKey.Start, true);
		Interruptible = true;
		Cost = 1f;
	}

	public override void SetPlanWSPreconditions(WorldState goalState)
	{
		base.SetPlanWSPreconditions(goalState);
		if (Owner.WorldState.GetWSProperty(E_PropKey.SeeEnemy).GetBool())
		{
			goalState.SetWSProperty(E_PropKey.LookingAtTarget, true);
		}
	}

	public override void Activate()
	{
		base.Activate();
		AgentActionIdle action = AgentActionFactory.Create(AgentActionFactory.E_Type.Idle) as AgentActionIdle;
		Owner.BlackBoard.ActionAdd(action);
		TimeToEnd = Random.Range(1.5f, 3f) + Time.timeSinceLevelLoad;
	}

	public override void Deactivate()
	{
		base.Deactivate();
	}

	public override bool IsActionComplete()
	{
		return TimeToEnd < Time.timeSinceLevelLoad || Owner.BlackBoard.Fear / 100f < 0.25f;
	}

	public override bool ValidateAction()
	{
		return true;
	}
}
