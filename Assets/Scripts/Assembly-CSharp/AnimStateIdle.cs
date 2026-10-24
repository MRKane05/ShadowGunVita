using UnityEngine;
using UnityEngine.AI;

public class AnimStateIdle : AnimState
{
	private float TimeToFinishWeaponAction;

	private AgentAction WeaponAction;

	private float TimeToFinishRotateAction;

	private AgentActionRotate RotateAction;

	private float TimeToFinishReloadAction;

	private AgentActionReload ReloadAction;

	private string AnimNameUp;

	private string AnimNameDown;

	private string AnimNameBase;

	private float BlendUp;

	private float BlendDown;

	public AnimStateIdle(Animation anims, AgentHuman owner)
		: base(anims, owner)
	{
	}

	public override void OnDeactivate()
	{
		if (WeaponAction != null)
		{
			WeaponAction.SetSuccess();
			WeaponAction = null;
		}
		if (RotateAction != null)
		{
			RotateAction.SetSuccess();
			RotateAction = null;
		}
		if (ReloadAction != null)
		{
			ReloadAction.SetSuccess();
			ReloadAction = null;
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
		if (WeaponAction != null)
		{
			WeaponAction.SetSuccess();
			WeaponAction = null;
		}
		if (RotateAction != null)
		{
			RotateAction.SetSuccess();
			RotateAction = null;
		}
		if (ReloadAction != null)
		{
			ReloadAction.SetSuccess();
			ReloadAction = null;
		}
		base.Reset();
	}

	public override bool HandleNewAction(AgentAction action)
	{
		if (action is AgentActionAttack)
		{
			string weaponAnim = Owner.AnimSet.GetWeaponAnim(E_WeaponAction.Fire);
			TimeToFinishWeaponAction = Time.timeSinceLevelLoad + Animation[weaponAnim].length * 0.5f;
			Animation[weaponAnim].layer = 2;
			Animation[weaponAnim].blendMode = AnimationBlendMode.Additive;
			if (Animation.IsPlaying(weaponAnim))
			{
				Animation[weaponAnim].time = 0f;
			}
			else
			{
				Blend(weaponAnim, 0.05f);
			}
			if (WeaponAction != null)
			{
				WeaponAction.SetSuccess();
			}
			WeaponAction = action;
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
			if (Owner.IsPlayer)
			{
				TimeToFinishReloadAction = Time.timeSinceLevelLoad + 0.1f;
			}
			else
			{
				TimeToFinishReloadAction = Time.timeSinceLevelLoad + Animation[weaponAnim2].length * 0.9f;
			}
			ReloadAction = action as AgentActionReload;
			return true;
		}
		if (action is AgentActionRotate)
		{
			RotateAction = action as AgentActionRotate;
			string rotateAnim = Owner.AnimSet.GetRotateAnim(RotateAction.Rotation);
			if (rotateAnim != null && !Animation.IsPlaying(rotateAnim))
			{
				Animation[rotateAnim].blendMode = AnimationBlendMode.Additive;
				Animation[rotateAnim].layer = 1;
				TimeToFinishRotateAction = Time.timeSinceLevelLoad + Animation[rotateAnim].length + 0.3f;
				Blend(rotateAnim, 0.1f);
			}
		}
		return false;
	}

	public override void Update()
	{
		if (WeaponAction != null && TimeToFinishWeaponAction < Time.timeSinceLevelLoad)
		{
			WeaponAction.SetSuccess();
			WeaponAction = null;
			CrossFade(Owner.AnimSet.GetIdleAnim(), 0.4f, PlayMode.StopSameLayer);
		}
		if (RotateAction != null && TimeToFinishRotateAction < Time.timeSinceLevelLoad)
		{
			RotateAction.SetSuccess();
			RotateAction = null;
		}
		if (ReloadAction != null && TimeToFinishReloadAction < Time.timeSinceLevelLoad)
		{
			ReloadAction.SetSuccess();
			ReloadAction = null;
		}
		if (Owner.NavMeshAgent == null && !Owner.IsInCover)
		{
			Move(Vector3.zero, false);
		}
		PlayIdleAnim();
		if (Owner.BlackBoard.AimAnimationsEnabled && !Owner.IsInCover)
		{
			UpdateBlendValues();
			UpdateBlendedAnims();
		}
	}

	private void PlayIdleAnim()
	{
		AnimNameBase = Owner.AnimSet.GetIdleAnim();
		if (!Animation.IsPlaying(AnimNameBase))
		{
			if (Owner.IsInCover)
			{
				CrossFade(AnimNameBase, 0.3f, PlayMode.StopSameLayer);
			}
			else if (Owner.BlackBoard.ComlinkOn)
			{
				CrossFade(AnimNameBase, 0.5f, PlayMode.StopSameLayer);
			}
			else
			{
				CrossFade(AnimNameBase, 0.3f, PlayMode.StopSameLayer);
			}
		}
	}

	protected override void Initialize(AgentAction action)
	{
		base.Initialize(action);
		Owner.BlackBoard.MotionType = E_MotionType.None;
		Owner.BlackBoard.MoveDir = Vector3.zero;
		Owner.BlackBoard.Speed = 0f;
		if (WeaponAction == null)
		{
			PlayIdleAnim();
		}
		if (action != null)
		{
			action.SetSuccess();
		}
		if (Owner.BlackBoard.AimAnimationsEnabled && !Owner.IsInCover)
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
