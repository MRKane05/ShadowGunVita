using UnityEngine;

public class AnimStateMove : AnimState
{
	private AgentActionMove Action;

	private float MaxSpeed;

	private string AnimNameBase;

	private string AnimNameDown;

	private string AnimNameUp;

	private float BlendUp;

	private float BlendDown;

	public AnimStateMove(Animation anims, AgentHuman owner)
		: base(anims, owner)
	{
	}

	public override void OnActivate(AgentAction action)
	{
		base.OnActivate(action);
	}

	public override void OnDeactivate()
	{
		Owner.BlackBoard.MotionType = E_MotionType.None;
		Owner.BlackBoard.MoveDir = Vector3.zero;
		Action.SetSuccess();
		Action = null;
		if (Owner.BlackBoard.AimAnimationsEnabled)
		{
			Animation[AnimNameUp].weight = 0f;
			Animation[AnimNameDown].weight = 0f;
			Animation.Stop(AnimNameUp);
			Animation.Stop(AnimNameDown);
		}
		base.OnDeactivate();
	}

	public override void Reset()
	{
		if (Owner.BlackBoard.AimAnimationsEnabled)
		{
			Animation[AnimNameUp].weight = 0f;
			Animation[AnimNameDown].weight = 0f;
			Animation.Stop(AnimNameUp);
			Animation.Stop(AnimNameDown);
		}
		Action.SetSuccess();
		Action = null;
		base.Reset();
	}

	public override void Update()
	{
		if (!Action.IsActive())
		{
			Release();
		}
		else if (!(Owner.BlackBoard.Desires.MoveDirection == Vector3.zero))
		{
			if (Owner.BlackBoard.MotionType == E_MotionType.Comlink)
			{
				MaxSpeed = Owner.BlackBoard.BaseSetup.MaxWalkSpeed;
			}
			else
			{
				MaxSpeed = Mathf.Max(Owner.BlackBoard.BaseSetup.MaxWalkSpeed, Owner.BlackBoard.BaseSetup.MaxRunSpeed * Owner.BlackBoard.Desires.MoveSpeedModifier);
			}
			float t = Owner.BlackBoard.BaseSetup.SpeedSmooth * Time.deltaTime;
			Owner.BlackBoard.Speed = Mathf.Lerp(Owner.BlackBoard.Speed, MaxSpeed, t);
			Owner.BlackBoard.MoveDir = Owner.BlackBoard.Desires.MoveDirection;
			if (!Move(Owner.BlackBoard.Desires.MoveDirection * Owner.BlackBoard.Speed * Time.deltaTime))
			{
				Release();
			}
			PlayMoveAnim(false);
			if (Owner.BlackBoard.AimAnimationsEnabled)
			{
				UpdateBlendValues();
				UpdateBlendedAnims();
			}
		}
	}

	public override bool HandleNewAction(AgentAction action)
	{
		if (action is AgentActionMove)
		{
			if (Action != null)
			{
				Action.SetSuccess();
			}
			SetFinished(false);
			Initialize(action);
			return true;
		}
		if (action is AgentActionIdle)
		{
			action.SetSuccess();
			SetFinished(true);
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
		if (action is AgentActionInjury)
		{
			PlayInjuryAnimation(action as AgentActionInjury);
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

	private void PlayMoveAnim(bool force)
	{
		Owner.BlackBoard.MotionType = GetMotionType();
		Owner.BlackBoard.MoveType = GetMoveType();
		AnimNameBase = Owner.AnimSet.GetMoveAnim();
		if (force || !Animation.IsPlaying(AnimNameBase))
		{
			if (Owner.BlackBoard.MotionType == E_MotionType.Comlink)
			{
				CrossFade(AnimNameBase, 0.5f, PlayMode.StopSameLayer);
			}
			else
			{
				CrossFade(AnimNameBase, 0.4f, PlayMode.StopSameLayer);
			}
		}
	}

	private E_MoveType GetMoveType()
	{
		Vector2 vector = new Vector2(Transform.forward.x, Transform.forward.z);
		Vector2 vector2 = new Vector2(Transform.right.x, Transform.right.z);
		Vector2 to = new Vector2(Owner.BlackBoard.Desires.MoveDirection.x, Owner.BlackBoard.Desires.MoveDirection.z);
		float num = Vector2.Angle(vector, to);
		float num2 = Vector2.Angle(vector2, to);
		if (num <= 45f)
		{
			return E_MoveType.Forward;
		}
		if (num > 135f)
		{
			return E_MoveType.Backward;
		}
		if (num2 < 90f)
		{
			return E_MoveType.StrafeRight;
		}
		return E_MoveType.StrafeLeft;
	}

	private E_MotionType GetMotionType()
	{
		if (Owner.BlackBoard.ComlinkOn)
		{
			return E_MotionType.Comlink;
		}
		if (Owner.BlackBoard.Speed > Owner.BlackBoard.BaseSetup.MaxWalkSpeed * 1.5f)
		{
			return E_MotionType.Run;
		}
		return E_MotionType.Walk;
	}

	protected override void Initialize(AgentAction action)
	{
		base.Initialize(action);
		Action = action as AgentActionMove;
		PlayMoveAnim(true);
		if (Owner.BlackBoard.AimAnimationsEnabled)
		{
			AnimNameUp = Owner.AnimSet.GetAimAnim(E_AimDirection.Up);
			AnimNameDown = Owner.AnimSet.GetAimAnim(E_AimDirection.Down);
			UpdateBlendValues();
			Animation[AnimNameUp].time = 0f;
			Animation[AnimNameDown].time = 0f;
			Animation[AnimNameUp].weight = BlendUp;
			Animation[AnimNameDown].weight = BlendDown;
			Animation.Blend(AnimNameUp, BlendUp, 0f);
			Animation.Blend(AnimNameDown, BlendDown, 0f);
		}
	}

	private void UpdateBlendValues()
	{
		if (Owner.IsInCover || Owner.IsComlinkOn)
		{
			BlendUp = 0f;
			BlendDown = 0f;
			return;
		}
		Quaternion rotation = Owner.BlackBoard.Desires.Rotation;
		rotation.SetLookRotation(Owner.BlackBoard.FireDir);
		Vector3 eulerAngles = rotation.eulerAngles;
		Vector3 eulerAngles2 = Owner.Transform.rotation.eulerAngles;
		Vector3 vector = eulerAngles - eulerAngles2;
		if (vector.x > 180f)
		{
			vector.x -= 360f;
		}
		else if (vector.x < -180f)
		{
			vector.x += 360f;
		}
		float num = ((!(vector.x > 0f)) ? (0f - vector.x) : 0f);
		float num2 = ((!(vector.x < 0f)) ? vector.x : 0f);
		BlendUp = Mathf.Min(1f, num / 70f);
		BlendDown = Mathf.Min(1f, num2 / 50f);
	}

	private void UpdateBlendedAnims()
	{
		float num = 10f;
		Animation[AnimNameUp].weight = Mathf.Lerp(Animation[AnimNameUp].weight, BlendUp, Time.deltaTime * num);
		Animation[AnimNameDown].weight = Mathf.Lerp(Animation[AnimNameDown].weight, BlendDown, Time.deltaTime * num);
	}
}
