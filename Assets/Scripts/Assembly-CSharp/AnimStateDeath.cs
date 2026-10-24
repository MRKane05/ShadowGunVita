using UnityEngine;
using UnityEngine.AI;

public class AnimStateDeath : AnimState
{
	private Vector3 StartPosition;

	private Vector3 FinalPosition;

	private Quaternion FinalRotation;

	private Quaternion StartRotation;

	private float RotationProgress;

	private float MoveTime;

	private float CurrentMoveTime;

	private AgentActionDeath Action;

	public AnimStateDeath(Animation anims, AgentHuman owner)
		: base(anims, owner)
	{
	}

	public override void OnActivate(AgentAction action)
	{
		base.OnActivate(action);
		Owner.BlackBoard.MotionType = E_MotionType.None;
		Owner.BlackBoard.MoveDir = Vector3.zero;
		Owner.BlackBoard.Speed = 0f;
	}

	public override void Update()
	{
	}

	public override void Reset()
	{
		Action.SetSuccess();
	}

	public override void Release()
	{
	}

	public override bool HandleNewAction(AgentAction action)
	{
		if (action is AgentActionDeath)
		{
			action.SetFailed();
			return true;
		}
		return false;
	}

	protected override void Initialize(AgentAction action)
	{
		base.Initialize(action);
		Action = action as AgentActionDeath;
		string deathAnim = Owner.AnimSet.GetDeathAnim(Action.FromWeapon);
		CrossFade(deathAnim, 0.1f, PlayMode.StopAll);
		if (!Owner.BlackBoard.DontDeathAnimMove && (bool)Owner.NavMeshAgent && (!Owner.IsInCover || Owner.WorldState.GetWSProperty(E_PropKey.CoverState).GetCoverState() == E_CoverState.Aim))
		{
			Vector3 destination = Action.Impuls + Owner.Position;
			Owner.NavMeshAgent.speed = Action.Impuls.magnitude / 0.2f;
			Owner.NavMeshAgent.acceleration = 100f;
			Owner.NavMeshAgent.updatePosition = true;
			Owner.NavMeshAgent.autoRepath = false;
			Owner.NavMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
			Owner.NavMeshAgent.SetDestination(destination);
		}
		Owner.BlackBoard.MotionType = E_MotionType.None;
		Owner.BlackBoard.MotionType = E_MotionType.Death;
		Owner.DisableCollisions();
	}
}
