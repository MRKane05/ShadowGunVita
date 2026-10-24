using UnityEngine;
using UnityEngine.AI;

internal class GOAPActionCheckLostEnemy : GOAPAction
{
	private AgentActionGoTo Action;

	private AgentHuman Target;

	private Fact CheckingFact;

	private NavMeshPath Path;

	private Vector3 FinalPos;

	private float EndOfAction;

	public GOAPActionCheckLostEnemy(AgentHuman owner)
		: base(E_GOAPAction.CheckLostEnemy, owner)
	{
	}

	public override void InitAction()
	{
		WorldEffects.SetWSProperty(E_PropKey.Event, E_EventTypes.None);
		WorldPreconditions.SetWSProperty(E_PropKey.CoverState, E_CoverState.None);
		Cost = 1f;
		Precedence = 70;
		Path = new NavMeshPath();
	}

	public override bool ValidateContextPreconditions(WorldState current, bool planning)
	{
		if (Owner.WorldState.GetWSProperty(E_PropKey.SeeEnemy).GetBool())
		{
			return false;
		}
		CheckingFact = Owner.Memory.GetValidFact(E_EventTypes.EnemyLost);
		if (CheckingFact == null)
		{
			return false;
		}
		if ((CheckingFact.Position - Owner.Position).sqrMagnitude < 1f)
		{
			return false;
		}
		if (!NavMesh.CalculatePath(Owner.Position, CheckingFact.Position, Owner.NavMeshAgent.walkableMask, Path))
		{
			return false;
		}
		FinalPos = Path.corners[Path.corners.Length - 1];
		return true;
	}

	public override void Activate()
	{
		base.Activate();
		Action = AgentActionFactory.Create(AgentActionFactory.E_Type.Goto) as AgentActionGoTo;
		Action.MoveType = E_MoveType.Forward;
		Action.Motion = E_MotionType.Walk;
		Action.FinalPosition = FinalPos;
		Owner.BlackBoard.ActionAdd(Action);
		EndOfAction = Time.timeSinceLevelLoad + Random.Range(3.2f, 5.4f);
	}

	public override void Deactivate()
	{
		WorldStateProp wSProperty = Owner.WorldState.GetWSProperty(E_PropKey.Event);
		if (wSProperty != null && CheckingFact != null && wSProperty.GetEvent() == CheckingFact.Type)
		{
			Owner.WorldState.SetWSProperty(E_PropKey.Event, E_EventTypes.None);
		}
		CheckingFact = null;
		AgentActionIdle action = AgentActionFactory.Create(AgentActionFactory.E_Type.Idle) as AgentActionIdle;
		Owner.BlackBoard.ActionAdd(action);
		base.Deactivate();
	}

	public override bool IsActionComplete()
	{
		if (EndOfAction < Time.timeSinceLevelLoad || (Action != null && Action.IsSuccess()))
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
		if (Action != null && Action.IsFailed())
		{
			return false;
		}
		return true;
	}
}
