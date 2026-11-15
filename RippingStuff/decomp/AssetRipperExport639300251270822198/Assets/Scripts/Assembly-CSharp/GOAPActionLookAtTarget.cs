using UnityEngine;

internal class GOAPActionLookAtTarget : GOAPAction
{
	private AgentHuman Target;

	private Vector3 TargetDir;

	public GOAPActionLookAtTarget(AgentHuman owner)
		: base(E_GOAPAction.LookatTarget, owner)
	{
	}

	public override void InitAction()
	{
		WorldEffects.SetWSProperty(E_PropKey.LookingAtTarget, true);
		Cost = 1f;
		Precedence = 95;
	}

	public override bool ValidateContextPreconditions(WorldState current, bool planning)
	{
		Target = Owner.BlackBoard.DangerousEnemy;
		if (Target == null)
		{
			Fact validFact = Owner.Memory.GetValidFact(E_EventTypes.EnemyLost);
			if (validFact == null)
			{
				validFact = Owner.Memory.GetValidFact(E_EventTypes.EnemyHideInCover);
				if (validFact == null)
				{
					return false;
				}
			}
			Target = validFact.Agent;
		}
		return true;
	}

	public override void Update()
	{
	}

	public override void Activate()
	{
		base.Activate();
		Vector3 position = Target.Position;
		Vector3 position2 = Owner.Position;
		position.y = 0f;
		position2.y = 0f;
		TargetDir = (position - position2).normalized;
		Owner.BlackBoard.Desires.Rotation.SetLookRotation(TargetDir);
	}

	public override void Deactivate()
	{
		base.Deactivate();
	}

	public override bool IsActionComplete()
	{
		if ((double)Vector3.Dot(TargetDir, Owner.Forward) > 0.95)
		{
			return true;
		}
		return false;
	}

	public override bool ValidateAction()
	{
		if (!Owner.WorldState.GetWSProperty(E_PropKey.SeeEnemy).GetBool())
		{
			return false;
		}
		return true;
	}
}
