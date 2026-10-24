using UnityEngine;
using UnityEngine.AI;

public class AnimStateGoToByAnim : AnimState
{
	private AgentActionGoToByAnim Action;

	private string AnimName;

	private Quaternion FinalRotation = default(Quaternion);

	private Quaternion StartRotation = default(Quaternion);

	private float CurrentRotationTime;

	private float RotationTime;

	private bool RotationOk;

	private float EndOfState;

	public AnimStateGoToByAnim(Animation anims, AgentHuman owner)
		: base(anims, owner)
	{
	}

	public override void OnActivate(AgentAction action)
	{
		CurrentRotationTime = 0f;
		RotationOk = true;
		base.OnActivate(action);
	}

	public override void OnDeactivate()
	{
		if (Owner.NavMeshAgent.enabled)
		{
			Owner.NavMeshAgent.Stop();
		}
		Owner.BlackBoard.MotionType = E_MotionType.None;
		Owner.BlackBoard.Velocity = (Owner.BlackBoard.MoveDir = Vector3.zero);
		Owner.BlackBoard.Speed = 0f;
		Action.SetSuccess();
		Action = null;
		base.OnDeactivate();
	}

	public override void Reset()
	{
		Action.SetSuccess();
		Action = null;
		base.Reset();
	}

	public override void Update()
	{
		if (!RotationOk)
		{
			CurrentRotationTime += Time.deltaTime;
			if (CurrentRotationTime >= RotationTime)
			{
				CurrentRotationTime = RotationTime;
				RotationOk = true;
			}
			float t = CurrentRotationTime / RotationTime;
			Quaternion rotation = Quaternion.Lerp(StartRotation, FinalRotation, t);
			Owner.Transform.rotation = rotation;
		}
		else
		{
			UpdateRotation();
			Owner.BlackBoard.Speed = Owner.NavMeshAgent.speed;
			Owner.BlackBoard.Velocity = Owner.NavMeshAgent.velocity;
		}
		if (Animation[AnimName].normalizedTime >= 0.9f)
		{
			Release();
		}
	}

	public override bool HandleNewAction(AgentAction action)
	{
		return false;
	}

	protected override void Initialize(AgentAction action)
	{
		base.Initialize(action);
		Action = action as AgentActionGoToByAnim;
		Owner.BlackBoard.MotionType = E_MotionType.AnimationDrive;
		Owner.BlackBoard.MoveType = Action.MoveType;
		InitializeRotation();
		InitializeMove();
	}

	private void InitializeRotation()
	{
		if (GetRotationType(Action.MoveType) != E_RotationType.None)
		{
			StartRotation = Owner.Transform.rotation;
			FinalRotation = StartRotation;
			RotationTime = 0.2f;
			if (Action.MoveType == E_MoveType.Forward)
			{
				FinalRotation.SetLookRotation((Action.FinalPosition - Owner.Position).normalized);
			}
			else if (Action.MoveType == E_MoveType.Backward)
			{
				FinalRotation.SetLookRotation((Owner.Position - Action.FinalPosition).normalized);
			}
			else if (Action.MoveType == E_MoveType.StrafeRight)
			{
				FinalRotation.SetLookRotation(Vector3.Cross((Action.FinalPosition - Owner.Position).normalized, Vector3.up));
			}
			else if (Action.MoveType == E_MoveType.Backward)
			{
				FinalRotation.SetLookRotation(-Vector3.Cross((Action.FinalPosition - Owner.Position).normalized, Vector3.up));
			}
			Owner.BlackBoard.Desires.Rotation = FinalRotation;
		}
	}

	private void InitializeMove()
	{
		Owner.NavMeshAgent.SetDestination(Action.FinalPosition);
		AnimName = Owner.AnimSet.GetMoveAnim();
		CrossFade(AnimName, 0.2f, PlayMode.StopSameLayer);
		Owner.NavMeshAgent.speed = (Owner.Position - Action.FinalPosition).magnitude / Animation[AnimName].length;
	}

	private void UpdateRotation()
	{
		if (Owner.NavMeshAgent.velocity != Vector3.zero)
		{
			if (Action.MoveType == E_MoveType.Forward)
			{
				Owner.BlackBoard.Desires.Rotation.SetLookRotation(Owner.NavMeshAgent.velocity.normalized);
			}
			else if (Action.MoveType == E_MoveType.Backward)
			{
				Owner.BlackBoard.Desires.Rotation.SetLookRotation(-Owner.NavMeshAgent.velocity.normalized);
			}
			else if (Action.MoveType == E_MoveType.StrafeRight)
			{
				Owner.BlackBoard.Desires.Rotation.SetLookRotation(Vector3.Cross(Owner.NavMeshAgent.velocity.normalized, Vector3.up));
			}
			else if (Action.MoveType == E_MoveType.StrafeLeft)
			{
				Owner.BlackBoard.Desires.Rotation.SetLookRotation(-Vector3.Cross(Owner.NavMeshAgent.velocity.normalized, Vector3.up));
			}
		}
	}

	private E_RotationType GetRotationType(E_MoveType moveType)
	{
		Vector2 vector = new Vector2(Transform.forward.x, Transform.forward.z);
		Vector2 vector2 = new Vector2(Transform.right.x, Transform.right.z);
		Vector3 normalized = (Action.FinalPosition - Owner.Position).normalized;
		Vector2 to = new Vector2(normalized.x, normalized.z);
		float num = Vector2.Angle(vector, to);
		float num2 = Vector2.Angle(vector2, to);
		switch (moveType)
		{
		case E_MoveType.Forward:
			if (num > 10f)
			{
				if (num2 < 90f)
				{
					return E_RotationType.Right;
				}
				return E_RotationType.Left;
			}
			return E_RotationType.None;
		case E_MoveType.Backward:
			if (num < 170f)
			{
				if (num2 < 90f)
				{
					return E_RotationType.Left;
				}
				return E_RotationType.Right;
			}
			return E_RotationType.None;
		case E_MoveType.StrafeRight:
			if (num2 > 10f)
			{
				if (num < 90f)
				{
					return E_RotationType.Left;
				}
				return E_RotationType.Right;
			}
			return E_RotationType.None;
		case E_MoveType.StrafeLeft:
			if (num2 > 10f)
			{
				if (num < 90f)
				{
					return E_RotationType.Right;
				}
				return E_RotationType.Left;
			}
			return E_RotationType.None;
		default:
			return E_RotationType.None;
		}
	}
}
