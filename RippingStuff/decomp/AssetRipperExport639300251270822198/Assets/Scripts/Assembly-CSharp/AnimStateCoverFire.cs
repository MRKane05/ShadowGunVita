using UnityEngine;

public class AnimStateCoverFire : AnimState
{
	private enum E_State
	{
		Start = 0,
		Fire = 1,
		End = 2
	}

	private AgentActionCoverFire Action;

	private AgentActionCoverFireCancel ActionCancel;

	private float EndOfStateTime;

	private E_State State;

	private E_CoverState OldCoverState;

	private string AnimNameLeft;

	private string AnimNameRight;

	private string AnimNameUp;

	private string AnimNameDown;

	private string AnimNameBase;

	private float BlendUp;

	private float BlendDown;

	private float BlendRight;

	private float BlendLeft;

	public AnimStateCoverFire(Animation anims, AgentHuman owner)
		: base(anims, owner)
	{
	}

	public override void OnActivate(AgentAction action)
	{
		base.OnActivate(action);
		Owner.BlackBoard.BusyAction = true;
		OldCoverState = Owner.WorldState.GetWSProperty(E_PropKey.CoverState).GetCoverState();
		Owner.WorldState.SetWSProperty(E_PropKey.CoverState, E_CoverState.Aim);
	}

	public override void OnDeactivate()
	{
		Owner.BlackBoard.BusyAction = false;
		if (Owner.IsAlive)
		{
			Owner.WorldState.SetWSProperty(E_PropKey.CoverState, OldCoverState);
		}
		if (ActionCancel != null)
		{
			ActionCancel.SetSuccess();
		}
		ActionCancel = null;
		Action.SetSuccess();
		Action = null;
		base.OnDeactivate();
	}

	public override void Reset()
	{
		if (ActionCancel != null)
		{
			ActionCancel.SetSuccess();
		}
		if (Owner.IsPlayer)
		{
			GameCamera.Instance.Reset();
		}
		ActionCancel = null;
		Action.SetSuccess();
		Action = null;
		base.Reset();
	}

	public override void Update()
	{
		switch (State)
		{
		case E_State.Start:
			UpdateAim();
			break;
		case E_State.Fire:
			UpdateFire();
			break;
		case E_State.End:
			UpdateEnd();
			break;
		}
		UpdateBlendedAnims();
	}

	public override bool HandleNewAction(AgentAction action)
	{
		if (action is AgentActionCoverFire)
		{
			Debug.LogError(ToString() + ": Second action is not allowed");
			action.SetFailed();
			return true;
		}
		if (action is AgentActionCoverFireCancel)
		{
			ActionCancel = action as AgentActionCoverFireCancel;
			StartEnd();
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
			Animation[weaponAnim].layer = 2;
			if (Animation.IsPlaying(weaponAnim))
			{
				Animation[weaponAnim].time = 0f;
			}
			else
			{
				Blend(weaponAnim, 0.1f);
			}
			action.SetSuccess();
			return true;
		}
		return false;
	}

	protected override void Initialize(AgentAction action)
	{
		base.Initialize(action);
		Action = action as AgentActionCoverFire;
		AnimNameBase = Owner.AnimSet.GetCoverAnim(AnimSet.E_CoverAnim.AimStart, Owner.BlackBoard.CoverPose, Owner.BlackBoard.CoverPosition);
		EndOfStateTime = Animation[AnimNameBase].length * 0.9f + Time.timeSinceLevelLoad;
		Owner.WeaponComponent.GetCurrentWeapon().SetBusy(Animation[AnimNameBase].length);
		Owner.BlackBoard.MotionType = E_MotionType.None;
		State = E_State.Start;
		AnimNameUp = Owner.AnimSet.GetAimAnim(E_AimDirection.Up);
		AnimNameDown = Owner.AnimSet.GetAimAnim(E_AimDirection.Down);
		AnimNameRight = Owner.AnimSet.GetAimAnim(E_AimDirection.Right);
		AnimNameLeft = Owner.AnimSet.GetAimAnim(E_AimDirection.Left);
		Animation[AnimNameUp].layer = 2;
		Animation[AnimNameDown].layer = 2;
		Animation[AnimNameRight].layer = 2;
		Animation[AnimNameLeft].layer = 2;
		Animation[AnimNameBase].wrapMode = WrapMode.ClampForever;
		CrossFade(AnimNameBase, 0.1f, PlayMode.StopSameLayer);
		float num = Animation[AnimNameBase].length * 0.9f;
		if (Owner.IsPlayer)
		{
			GameCamera.Instance.SetFov(35f, 60f);
		}
		UpdateBlendValues();
		Animation[AnimNameUp].weight = 0f;
		Animation[AnimNameDown].weight = 0f;
		Animation[AnimNameRight].weight = 0f;
		Animation[AnimNameLeft].weight = 0f;
		Animation.Blend(AnimNameUp, BlendUp, num);
		Animation.Blend(AnimNameDown, BlendDown, num);
		Animation.Blend(AnimNameRight, BlendRight, num);
		Animation.Blend(AnimNameLeft, BlendLeft, num);
		Owner.WeaponComponent.DisableCurrentWeapon(num);
	}

	private void UpdateAim()
	{
		if (EndOfStateTime <= Time.timeSinceLevelLoad)
		{
			State = E_State.Fire;
			Owner.BlackBoard.BusyAction = false;
			Action.SetSuccess();
		}
	}

	private void UpdateFire()
	{
		UpdateBlendValues();
		UpdateBlendedAnims();
	}

	private void StartEnd()
	{
		string coverAnim = Owner.AnimSet.GetCoverAnim(AnimSet.E_CoverAnim.AimEnd, Owner.BlackBoard.CoverPose, Owner.BlackBoard.CoverPosition);
		State = E_State.End;
		EndOfStateTime = Animation[coverAnim].length * 0.9f + Time.timeSinceLevelLoad;
		Owner.WeaponComponent.GetCurrentWeapon().SetBusy(Animation[coverAnim].length);
		Owner.WorldState.SetWSProperty(E_PropKey.CoverState, OldCoverState);
		CrossFade(coverAnim, 0.1f, PlayMode.StopSameLayer);
		if (Owner.IsPlayer)
		{
			GameCamera.Instance.SetDefaultFov(60f);
		}
		BlendUp = 0f;
		BlendDown = 0f;
		BlendRight = 0f;
		BlendLeft = 0f;
		Owner.BlackBoard.BusyAction = true;
		Owner.WeaponComponent.DisableCurrentWeapon(Animation[coverAnim].length);
	}

	private void UpdateEnd()
	{
		UpdateBlendedAnims();
		if (EndOfStateTime <= Time.timeSinceLevelLoad)
		{
			Animation.Stop(AnimNameUp);
			Animation.Stop(AnimNameDown);
			Animation.Stop(AnimNameRight);
			Animation.Stop(AnimNameLeft);
			Release();
		}
	}

	private void UpdateBlendValues()
	{
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
		if (vector.y > 180f)
		{
			vector.y -= 360f;
		}
		else if (vector.y < -180f)
		{
			vector.y += 360f;
		}
		float num = ((!(vector.x > 0f)) ? (0f - vector.x) : 0f);
		float num2 = ((!(vector.x < 0f)) ? vector.x : 0f);
		float num3 = ((!(vector.y < 0f)) ? vector.y : 0f);
		float num4 = ((!(vector.y > 0f)) ? (0f - vector.y) : 0f);
		if (Owner.BlackBoard.CoverPosition == E_CoverDirection.Right)
		{
			BlendUp = Mathf.Min(1f, num / Owner.BlackBoard.CoverSetup.RightMaxUp);
			BlendDown = Mathf.Min(1f, num2 / Owner.BlackBoard.CoverSetup.RightMaxDown);
			BlendRight = Mathf.Min(1f, num3 / Owner.BlackBoard.CoverSetup.RightMaxRight);
			BlendLeft = Mathf.Min(1f, num4 / Owner.BlackBoard.CoverSetup.RightMaxLeft);
		}
		else if (Owner.BlackBoard.CoverPosition == E_CoverDirection.Left)
		{
			BlendUp = Mathf.Min(1f, num / Owner.BlackBoard.CoverSetup.LeftMaxUp);
			BlendDown = Mathf.Min(1f, num2 / Owner.BlackBoard.CoverSetup.LeftMaxDown);
			BlendRight = Mathf.Min(1f, num3 / Owner.BlackBoard.CoverSetup.LeftMaxRight);
			BlendLeft = Mathf.Min(1f, num4 / Owner.BlackBoard.CoverSetup.LeftMaxLeft);
		}
		else
		{
			BlendUp = Mathf.Min(1f, num / Owner.BlackBoard.CoverSetup.CenterMaxUp);
			BlendDown = Mathf.Min(1f, num2 / Owner.BlackBoard.CoverSetup.CenterMaxDown);
			BlendRight = Mathf.Min(1f, num3 / Owner.BlackBoard.CoverSetup.CenterMaxRight);
			BlendLeft = Mathf.Min(1f, num4 / Owner.BlackBoard.CoverSetup.CenterMaxLeft);
		}
	}

	private void UpdateBlendedAnims()
	{
		float num = 15f;
		Animation[AnimNameUp].weight = Mathf.Lerp(Animation[AnimNameUp].weight, BlendUp, Time.deltaTime * num);
		Animation[AnimNameDown].weight = Mathf.Lerp(Animation[AnimNameDown].weight, BlendDown, Time.deltaTime * num);
		Animation[AnimNameRight].weight = Mathf.Lerp(Animation[AnimNameRight].weight, BlendRight, Time.deltaTime * num);
		Animation[AnimNameLeft].weight = Mathf.Lerp(Animation[AnimNameLeft].weight, BlendLeft, Time.deltaTime * num);
	}
}
