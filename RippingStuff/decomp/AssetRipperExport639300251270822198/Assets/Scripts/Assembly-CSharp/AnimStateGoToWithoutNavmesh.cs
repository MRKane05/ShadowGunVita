using UnityEngine;

public class AnimStateGoToWithoutNavmesh : AnimState
{
	private AgentActionGoTo Action;

	private float MaxSpeed;

	private string AnimName;

	private Quaternion FinalRotation = default(Quaternion);

	private Quaternion StartRotation = default(Quaternion);

	private float RotationProgress;

	public AnimStateGoToWithoutNavmesh(Animation anims, AgentHuman owner)
		: base(anims, owner)
	{
	}

	public override void OnActivate(AgentAction action)
	{
		base.OnActivate(action);
		AnimName = null;
		PlayAnim(Action.Motion);
	}

	public override void OnDeactivate()
	{
		Owner.BlackBoard.MotionType = E_MotionType.None;
		Owner.BlackBoard.MoveDir = Vector3.zero;
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
		float sqrMagnitude = (Action.FinalPosition - Transform.position).sqrMagnitude;
		if (sqrMagnitude < 2.25f)
		{
			MaxSpeed = Owner.BlackBoard.BaseSetup.MaxWalkSpeed;
		}
		RotationProgress += Time.deltaTime * 12f;
		RotationProgress = Mathf.Min(RotationProgress, 1f);
		Quaternion rotation = Quaternion.Slerp(StartRotation, FinalRotation, RotationProgress);
		Owner.Transform.rotation = rotation;
		float t = Owner.BlackBoard.BaseSetup.SpeedSmooth * Time.deltaTime;
		Owner.BlackBoard.Speed = Mathf.Lerp(Owner.BlackBoard.Speed, MaxSpeed, t);
		Vector3 moveDir = Action.FinalPosition - Transform.position;
		moveDir.y = 0f;
		moveDir.Normalize();
		Owner.BlackBoard.MoveDir = moveDir;
		if (!Move(Owner.BlackBoard.MoveDir * Owner.BlackBoard.Speed * Time.deltaTime))
		{
			Release();
			return;
		}
		if ((Action.FinalPosition - Transform.position).sqrMagnitude < 0.09f)
		{
			Release();
			return;
		}
		E_MotionType motionType = GetMotionType();
		if (motionType != Owner.BlackBoard.MotionType)
		{
			PlayAnim(motionType);
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
		if (action is AgentActionReload)
		{
			string weaponAnim = Owner.AnimSet.GetWeaponAnim(E_WeaponAction.Reload);
			Animation[weaponAnim].layer = 2;
			Animation[weaponAnim].blendMode = AnimationBlendMode.Additive;
			Blend(weaponAnim, 0.1f);
			action.SetSuccess();
			return true;
		}
		return false;
	}

	private void PlayAnim(E_MotionType motion)
	{
		Owner.BlackBoard.MotionType = motion;
		Owner.BlackBoard.MoveType = Action.MoveType;
		AnimName = Owner.AnimSet.GetMoveAnim();
		CrossFade(AnimName, 0.2f, PlayMode.StopSameLayer);
	}

	private E_MotionType GetMotionType()
	{
		if (Owner.BlackBoard.Speed > Owner.BlackBoard.BaseSetup.MaxWalkSpeed * 1.5f)
		{
			return E_MotionType.Run;
		}
		return E_MotionType.Walk;
	}

	protected override void Initialize(AgentAction action)
	{
		base.Initialize(action);
		Action = action as AgentActionGoTo;
		Vector3 vector = Action.FinalPosition - Owner.Transform.position;
		vector.y = 0f;
		vector.Normalize();
		StartRotation = Owner.Transform.rotation;
		if (vector != Vector3.zero)
		{
			if (Action.MoveType == E_MoveType.Forward)
			{
				FinalRotation.SetLookRotation(vector);
			}
			else if (Action.MoveType == E_MoveType.Backward)
			{
				FinalRotation.SetLookRotation(-vector);
			}
		}
		Owner.BlackBoard.MotionType = GetMotionType();
		Owner.BlackBoard.MoveType = Action.MoveType;
		if (Action.Motion == E_MotionType.Run)
		{
			MaxSpeed = Owner.BlackBoard.BaseSetup.MaxRunSpeed;
		}
		else
		{
			MaxSpeed = Owner.BlackBoard.BaseSetup.MaxWalkSpeed;
		}
		RotationProgress = 0f;
	}
}
