using UnityEngine;
using UnityEngine.AI;

public class AnimStateGoTo : AnimState
{
	private AgentActionGoTo Action;

	private float MaxSpeed;

	private string AnimName;

	private float RotationProgress;

	public AnimStateGoTo(Animation anims, AgentHuman owner)
		: base(anims, owner)
	{
	}

	public override void OnActivate(AgentAction action)
	{
		Owner.BlackBoard.Velocity = Vector3.zero;
		base.OnActivate(action);
		AnimName = null;
	}

	public override void OnDeactivate()
	{
		if (Owner.NavMeshAgent.enabled)
		{
			Owner.NavMeshAgent.Stop();
		}
		Owner.NavMeshAgent.updateRotation = false;
		if (Owner.IsAlive)
		{
			Owner.BlackBoard.MotionType = E_MotionType.None;
		}
		Owner.BlackBoard.MoveDir = Vector3.zero;
		Owner.BlackBoard.Speed = 0f;
		Owner.BlackBoard.Velocity = Vector3.zero;
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
		UpdateRotation();
		Owner.BlackBoard.Speed = Owner.NavMeshAgent.speed;
		Owner.BlackBoard.Velocity = Owner.NavMeshAgent.velocity;
		Owner.BlackBoard.MotionType = GetMotionType();
		Owner.BlackBoard.MoveType = GetMoveType();
		PlayAnim();
		if (Owner.NavMeshAgent.remainingDistance < Action.MinDistance)
		{
			Release();
		}
	}

	public override bool HandleNewAction(AgentAction action)
	{
		if (action is AgentActionGoTo)
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
		if (action is AgentActionAttack)
		{
			string weaponAnim = Owner.AnimSet.GetWeaponAnim(E_WeaponAction.Fire);
			Animation[weaponAnim].blendMode = AnimationBlendMode.Additive;
			if (Animation.IsPlaying(weaponAnim))
			{
				Animation[weaponAnim].time = 0f;
			}
			else
			{
				Animation.Blend(weaponAnim, 1f, 0.1f);
			}
			action.SetSuccess();
			return true;
		}
		if (action is AgentActionReload)
		{
			string weaponAnim2 = Owner.AnimSet.GetWeaponAnim(E_WeaponAction.Reload);
			Animation[weaponAnim2].layer = 2;
			Animation[weaponAnim2].blendMode = AnimationBlendMode.Additive;
			Blend(weaponAnim2, 0.1f);
			action.SetSuccess();
			return true;
		}
		return false;
	}

	private void PlayAnim()
	{
		AnimName = Owner.AnimSet.GetMoveAnim();
		if (!Animation.IsPlaying(AnimName))
		{
			CrossFade(AnimName, 0.2f, PlayMode.StopSameLayer);
		}
	}

	private E_MotionType GetMotionType()
	{
		if (Action.DontChangeParameters)
		{
			return Action.Motion;
		}
		if (Owner.BlackBoard.Speed > Owner.BlackBoard.BaseSetup.MaxWalkSpeed * 1.5f)
		{
			return E_MotionType.Run;
		}
		return E_MotionType.Walk;
	}

	private E_MoveType GetMoveType()
	{
		if (Action.DontChangeParameters)
		{
			return Action.MoveType;
		}
		Vector2 vector = new Vector2(Transform.forward.x, Transform.forward.z);
		Vector2 vector2 = new Vector2(Transform.right.x, Transform.right.z);
		Vector3 vector3 = Owner.BlackBoard.Velocity.normalized;
		if (vector3 == Vector3.zero)
		{
			vector3 = Owner.Forward;
		}
		Vector2 to = new Vector2(vector3.x, vector3.z);
		float num = Vector2.Angle(vector, to);
		float num2 = Vector2.Angle(vector2, to);
		if (num < 45f)
		{
			return E_MoveType.Forward;
		}
		if (num > 135f)
		{
			return E_MoveType.Backward;
		}
		if (num2 < 45f)
		{
			return E_MoveType.StrafeRight;
		}
		return E_MoveType.StrafeLeft;
	}

	protected override void Initialize(AgentAction action)
	{
		base.Initialize(action);
		Action = action as AgentActionGoTo;
		if (!SetTargetLocation(Action.FinalPosition))
		{
			Debug.LogWarning(ToString() + " SetTargetLocation Failed - " + Action.FinalPosition);
			Release();
			return;
		}
		Owner.NavMeshAgent.updateRotation = Action.UseNavMeshAgentRotation;
		Owner.BlackBoard.MotionType = GetMotionType();
		Owner.BlackBoard.MoveType = GetMoveType();
		if (Action.Motion == E_MotionType.Run)
		{
			Owner.NavMeshAgent.speed = Owner.BlackBoard.BaseSetup.MaxRunSpeed;
		}
		else
		{
			Owner.NavMeshAgent.speed = Owner.BlackBoard.BaseSetup.MaxWalkSpeed;
		}
	}

	private void UpdateRotation()
	{
		if (Action.UseNavMeshAgentRotation)
		{
			Owner.BlackBoard.Desires.Rotation = Owner.Transform.rotation;
		}
		else if ((bool)Action.LookTarget)
		{
			Owner.BlackBoard.Desires.Rotation.SetLookRotation((Action.LookTarget.position - Owner.Position).normalized);
		}
		else if (Owner.NavMeshAgent.velocity != Vector3.zero)
		{
			if (Owner.BlackBoard.MoveType == E_MoveType.Forward)
			{
				Owner.BlackBoard.Desires.Rotation.SetLookRotation(Owner.NavMeshAgent.velocity.normalized);
			}
			else if (Owner.BlackBoard.MoveType == E_MoveType.Backward)
			{
				Owner.BlackBoard.Desires.Rotation.SetLookRotation(-Owner.NavMeshAgent.velocity.normalized);
			}
		}
	}
}
