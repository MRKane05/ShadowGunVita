using UnityEngine;

public class AnimStateCoverMove : AnimState
{
	private AgentActionCoverMove Action;

	private float MaxSpeed;

	private Vector3 MoveDirection;

	public AnimStateCoverMove(Animation anims, AgentHuman owner)
		: base(anims, owner)
	{
	}

	public override void OnActivate(AgentAction action)
	{
		Owner.BlackBoard.ReactOnHits = false;
		base.OnActivate(action);
	}

	public override void OnDeactivate()
	{
		if (Owner.BlackBoard.CoverPosition == E_CoverDirection.Middle)
		{
			if (Action.Direction == AgentActionCoverMove.E_Direction.Left && Owner.BlackBoard.Cover.IsLeftAllowed)
			{
				Vector3 leftEdge = Owner.BlackBoard.Cover.LeftEdge;
				leftEdge.y = Owner.Position.y;
				if (Mathf.Abs(Vector3.Dot(leftEdge - Owner.Position, Owner.BlackBoard.Cover.Left)) < 0.02f)
				{
					Owner.Transform.position = leftEdge;
					Owner.BlackBoard.CoverPosition = E_CoverDirection.Left;
					Owner.WorldState.SetWSProperty(E_PropKey.CoverState, E_CoverState.Edge);
				}
			}
			else if (Action.Direction == AgentActionCoverMove.E_Direction.Right && Owner.BlackBoard.Cover.IsRightAllowed)
			{
				Vector3 rightEdge = Owner.BlackBoard.Cover.RightEdge;
				rightEdge.y = Owner.Position.y;
				if (Mathf.Abs(Vector3.Dot(rightEdge - Owner.Position, Owner.BlackBoard.Cover.Right)) < 0.02f)
				{
					Owner.Transform.position = rightEdge;
					Owner.BlackBoard.CoverPosition = E_CoverDirection.Right;
					Owner.WorldState.SetWSProperty(E_PropKey.CoverState, E_CoverState.Edge);
				}
			}
		}
		Owner.BlackBoard.ReactOnHits = true;
		if (Action != null)
		{
			Action.SetSuccess();
			Action = null;
		}
		Owner.BlackBoard.Speed = 0f;
		base.OnDeactivate();
	}

	public override void Reset()
	{
		Action.SetSuccess();
		Action = null;
		base.Reset();
	}

	protected override void Initialize(AgentAction action)
	{
		base.Initialize(action);
		Action = action as AgentActionCoverMove;
		if (Action.Direction == AgentActionCoverMove.E_Direction.Right)
		{
			MoveDirection = Owner.BlackBoard.Cover.Right;
		}
		else
		{
			MoveDirection = Owner.BlackBoard.Cover.Left;
		}
		MaxSpeed = Owner.BlackBoard.BaseSetup.MaxCoverSpeed * Action.Speed;
		Owner.BlackBoard.MotionType = GetMotionType();
		Owner.BlackBoard.MoveType = GetMoveType();
	}

	public override void Update()
	{
		if (!Action.IsActive())
		{
			Release();
			return;
		}
		float t = Owner.BlackBoard.BaseSetup.SpeedSmooth * Time.deltaTime;
		Owner.BlackBoard.Speed = Mathf.Lerp(Owner.BlackBoard.Speed, MaxSpeed, t);
		Owner.BlackBoard.MoveDir = Owner.BlackBoard.Desires.MoveDirection;
		if (!Move())
		{
			if (Action.Direction == AgentActionCoverMove.E_Direction.Left && Owner.BlackBoard.Cover.IsLeftAllowed)
			{
				Owner.BlackBoard.CoverPosition = E_CoverDirection.Left;
				Owner.WorldState.SetWSProperty(E_PropKey.CoverState, E_CoverState.Edge);
			}
			else if (Action.Direction == AgentActionCoverMove.E_Direction.Right && Owner.BlackBoard.Cover.IsRightAllowed)
			{
				Owner.BlackBoard.CoverPosition = E_CoverDirection.Right;
				Owner.WorldState.SetWSProperty(E_PropKey.CoverState, E_CoverState.Edge);
			}
			Release();
		}
		else
		{
			Owner.BlackBoard.CoverPosition = E_CoverDirection.Middle;
			Owner.WorldState.SetWSProperty(E_PropKey.CoverState, E_CoverState.Middle);
		}
		UpdateMoveAnim();
	}

	public override bool HandleNewAction(AgentAction action)
	{
		if (action is AgentActionCoverMove)
		{
			if (Action != null)
			{
				Action.SetSuccess();
			}
			SetFinished(false);
			Initialize(action);
			return true;
		}
		if (action is AgentActionInjury)
		{
			PlayInjuryAnimation(action as AgentActionInjury);
			return true;
		}
		if (action is AgentActionIdle)
		{
			action.SetSuccess();
			SetFinished(true);
		}
		return false;
	}

	// The original test stopped only when the straight-line distance to the edge dropped below
	// one frame's step (a few millimetres). It relied on the player sitting exactly on the cover
	// line. In Unity 2018 the CharacterController gets pushed off the wall slightly every frame
	// (AnimComponent nudges it up/down and PhysX 3 resolves the wall contact), so that distance
	// never got small enough and the player slid past either edge. Now the player is held on the
	// cover line and the stop is measured along the cover only.
	protected bool Move()
	{
		Cover cover = Owner.BlackBoard.Cover;
		Vector3 vector = ((Action.Direction != AgentActionCoverMove.E_Direction.Left) ? cover.RightEdge : cover.LeftEdge);
		Vector3 position = Owner.Position;
		vector.y = position.y;
		Vector3 onLine = cover.GetNearestPointOnCover(position);
		onLine.y = position.y;
		Vector3 direction = MoveDirection;
		direction.y = 0f;
		direction.Normalize();
		float remaining = Vector3.Dot(vector - onLine, direction);
		float num = Owner.BlackBoard.Speed * Time.deltaTime;
		if (remaining <= num)
		{
			Transform.position = vector;
			return false;
		}
		Transform.position = onLine + direction * num;
		return true;
	}

	private void PlayMoveAnim(E_MotionType motion)
	{
		Owner.BlackBoard.MotionType = motion;
		Owner.BlackBoard.MoveType = GetMoveType();
		CrossFade(Owner.AnimSet.GetMoveAnim(), 0.2f, PlayMode.StopSameLayer);
	}

	private void UpdateMoveAnim()
	{
		Owner.BlackBoard.MotionType = GetMotionType();
		Owner.BlackBoard.MoveType = GetMoveType();
		string moveAnim = Owner.AnimSet.GetMoveAnim();
		if (!Animation.IsPlaying(moveAnim))
		{
			CrossFade(moveAnim, 0.2f, PlayMode.StopSameLayer);
		}
	}

	private E_MoveType GetMoveType()
	{
		Vector2 vector = new Vector2(Transform.right.x, Transform.right.z);
		float num = Vector2.Angle(to: new Vector2(Owner.BlackBoard.Desires.MoveDirection.x, Owner.BlackBoard.Desires.MoveDirection.z), from: vector);
		if (num < 45f)
		{
			return E_MoveType.StrafeRight;
		}
		return E_MoveType.StrafeLeft;
	}

	private E_MotionType GetMotionType()
	{
		if (Owner.BlackBoard.Speed > Owner.BlackBoard.BaseSetup.MaxWalkSpeed * 1.5f)
		{
			return E_MotionType.Run;
		}
		return E_MotionType.Walk;
	}
}
