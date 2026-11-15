internal class GOAPActionPatrol : GOAPAction
{
	private AgentActionGoTo Action;

	private int CurrentPatrolPoint;

	public GOAPActionPatrol(AgentHuman owner)
		: base(E_GOAPAction.Patrol, owner)
	{
	}

	public override void InitAction()
	{
		WorldEffects.SetWSProperty(E_PropKey.Patrol, false);
		Cost = 5f;
		Precedence = 70;
	}

	public override bool ValidateContextPreconditions(WorldState current, bool planning)
	{
		if (!planning)
		{
			return true;
		}
		if (Owner.BlackBoard.Desires.PatrolPoints.Count == 0)
		{
			return false;
		}
		return true;
	}

	public override void Activate()
	{
		base.Activate();
		MoveToNextPatrolPoint();
	}

	public override void Deactivate()
	{
		WorldEffects.SetWSProperty(E_PropKey.AtTargetPos, true);
		if (Action != null && Action.IsActive())
		{
			AgentActionIdle action = AgentActionFactory.Create(AgentActionFactory.E_Type.Idle) as AgentActionIdle;
			Owner.BlackBoard.ActionAdd(action);
		}
		base.Deactivate();
	}

	public override void Update()
	{
		if (Action.IsSuccess())
		{
			MoveToNextPatrolPoint();
		}
	}

	public override bool IsActionComplete()
	{
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

	private void MoveToNextPatrolPoint()
	{
		Action = AgentActionFactory.Create(AgentActionFactory.E_Type.Goto) as AgentActionGoTo;
		Action.MoveType = E_MoveType.Forward;
		Action.Motion = E_MotionType.Walk;
		Action.FinalPosition = Owner.BlackBoard.Desires.PatrolPoints[CurrentPatrolPoint].transform.position;
		Action.DontChangeParameters = true;
		Action.UseNavMeshAgentRotation = true;
		CurrentPatrolPoint++;
		if (CurrentPatrolPoint == Owner.BlackBoard.Desires.PatrolPoints.Count)
		{
			CurrentPatrolPoint = 0;
		}
		Owner.BlackBoard.ActionAdd(Action);
	}
}
