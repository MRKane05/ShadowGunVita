using UnityEngine;
using UnityEngine.AI;

public class AnimStateCoverEnter : AnimState
{
	private AgentActionCoverEnter Action;

	private Vector3 StartPosition;

	private Vector3 FinalPosition;

	private float CurrentMoveTime;

	private float MoveTime;

	private float EndOfStateTime;

	private float BlockEndTime;

	private bool PositionOK;

	private bool EntryCompleted;

	public AnimStateCoverEnter(Animation anims, AgentHuman owner)
		: base(anims, owner)
	{
	}

	public override void OnActivate(AgentAction action)
	{
		base.OnActivate(action);
		Owner.BlackBoard.ReactOnHits = false;
		Owner.BlackBoard.BusyAction = true;
		Owner.BlackBoard.Desires.WeaponTriggerOn = false;
		if ((bool)Owner.NavMeshAgent)
		{
			Owner.NavMeshAgent.updatePosition = false;
		}
	}

	public override void OnDeactivate()
	{
		Owner.BlackBoard.ReactOnHits = true;
		Owner.BlackBoard.BusyAction = false;
		// If the entry was cut short and the AI has already given up on this cover (its GOAP
		// CoverEnter action cleared BlackBoard.Cover), the old code still put the cover back and
		// marked the entry a success. The enemy then carried on standing/moving with a crouch-sized
		// capsule and NavMeshAgent.updatePosition still false (body frozen = "huffing"). Abort cleanly
		// instead. When BlackBoard.Cover is still set (e.g. the player pressed something during the
		// entry), keep the original behaviour.
		if (!EntryCompleted && Owner.BlackBoard.Cover == null)
		{
			Action.Cover.LockPosition(Owner.BlackBoard.Desires.CoverPosition, null);
			Owner.WorldState.SetWSProperty(E_PropKey.CoverState, E_CoverState.None);
			Owner.BlackBoard.CoverPosition = E_CoverDirection.Unknown;
			ResumeAgentFollowing();
			Action.SetFailed();
			Action = null;
			// Lets ComponentEnemyMutant / ComponentPlayer restore the standing capsule.
			Owner.BlackBoard.ActionAdd(AgentActionFactory.Create(AgentActionFactory.E_Type.CoverTerminated));
			base.OnDeactivate();
			return;
		}
		Owner.BlackBoard.Cover = Action.Cover;
		Owner.BlackBoard.CoverPose = ((!Action.Cover.IsStandAllowed) ? E_CoverPose.Crouch : E_CoverPose.Stand);
		if (Owner.BlackBoard.Desires.CoverPosition == E_CoverDirection.Unknown)
		{
			Owner.BlackBoard.CoverPosition = E_CoverDirection.Middle;
		}
		else
		{
			Owner.BlackBoard.CoverPosition = Owner.BlackBoard.Desires.CoverPosition;
		}
		if (Owner.BlackBoard.CoverPosition == E_CoverDirection.Left || Owner.BlackBoard.CoverPosition == E_CoverDirection.Right)
		{
			Owner.WorldState.SetWSProperty(E_PropKey.CoverState, E_CoverState.Edge);
		}
		else
		{
			Owner.WorldState.SetWSProperty(E_PropKey.CoverState, E_CoverState.Middle);
		}
		Action.SetSuccess();
		Action = null;
		base.OnDeactivate();
	}

	public override void Reset()
	{
		Owner.BlackBoard.ReactOnHits = true;
		Owner.BlackBoard.BusyAction = false;
		Action.SetSuccess();
		Action = null;
		base.Reset();
	}

	public override void Update()
	{
		if (!PositionOK)
		{
			CurrentMoveTime += Time.deltaTime;
			if (CurrentMoveTime >= MoveTime)
			{
				CurrentMoveTime = MoveTime;
				PositionOK = true;
			}
			float value = CurrentMoveTime / MoveTime;
			Owner.Transform.position = Mathfx.Hermite(StartPosition, FinalPosition, value);
		}
		if (EndOfStateTime <= Time.timeSinceLevelLoad)
		{
			EntryCompleted = true;
			Release();
		}
	}

	public override bool HandleNewAction(AgentAction action)
	{
		if (action is AgentActionCoverEnter)
		{
			if (Action != null)
			{
				Debug.LogError(ToString() + ": Second action is not allowed");
				action.SetFailed();
				return true;
			}
			Initialize(action);
			return true;
		}
		return false;
	}

	protected override void Initialize(AgentAction action)
	{
		base.Initialize(action);
		Action = action as AgentActionCoverEnter;
		CurrentMoveTime = 0f;
		EntryCompleted = false;
		StartPosition = Transform.position;
		string coverAnim = Owner.AnimSet.GetCoverAnim(AnimSet.E_CoverAnim.Enter, (!Action.Cover.IsStandAllowed) ? E_CoverPose.Crouch : E_CoverPose.Stand, E_CoverDirection.Middle);
		CrossFade(coverAnim, 0.1f, PlayMode.StopSameLayer);
		if (Owner.BlackBoard.Desires.CoverPosition == E_CoverDirection.Unknown)
		{
			FinalPosition = Action.Cover.GetNearestPointOnCover(Owner.Position);
		}
		else if (Owner.BlackBoard.Desires.CoverPosition == E_CoverDirection.Left)
		{
			FinalPosition = Action.Cover.LeftEdge;
		}
		else if (Owner.BlackBoard.Desires.CoverPosition == E_CoverDirection.Right)
		{
			FinalPosition = Action.Cover.RightEdge;
		}
		else if (Owner.BlackBoard.Desires.CoverPosition == E_CoverDirection.Middle)
		{
			FinalPosition = Action.Cover.Transform.position;
		}
		MoveTime = Animation[coverAnim].length * 0.3f;
		EndOfStateTime = Animation[coverAnim].length * 0.7f + Time.timeSinceLevelLoad;
		PositionOK = false;
		Owner.BlackBoard.MotionType = E_MotionType.None;
	}
}
