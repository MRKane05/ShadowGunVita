using UnityEngine;

public class AnimStatePlayAnim : AnimState
{
	private AgentAction Action;

	private float EndOfStateTime;

	public bool LookAtTarget;

	private Quaternion FinalRotation;

	private Quaternion StartRotation;

	private float CurrentRotationTime;

	private float RotationTime;

	private bool RotationOk;

	private string AnimName;

	public AnimStatePlayAnim(Animation anims, AgentHuman owner)
		: base(anims, owner)
	{
	}

	public override void OnActivate(AgentAction action)
	{
		base.OnActivate(action);
		Owner.BlackBoard.Desires.WeaponTriggerOn = false;
		Owner.BlackBoard.MotionType = E_MotionType.None;
		Owner.BlackBoard.MoveDir = Vector3.zero;
		Owner.BlackBoard.Speed = 0f;
	}

	public override void OnDeactivate()
	{
		Animation[AnimName].layer = 0;
		LookAtTarget = false;
		Action.SetSuccess();
		Action = null;
		base.OnDeactivate();
	}

	public override void Reset()
	{
		Animation.Stop(AnimName);
		LookAtTarget = false;
		Action.SetSuccess();
		Action = null;
		base.Reset();
	}

	public override void Update()
	{
		UpdateFinalRotation();
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
		if (EndOfStateTime <= Time.timeSinceLevelLoad)
		{
			Release();
		}
	}

	public override bool HandleNewAction(AgentAction action)
	{
		if (action is AgentActionPlayAnim && Action != null)
		{
			action.SetFailed();
		}
		return false;
	}

	protected override void Initialize(AgentAction action)
	{
		base.Initialize(action);
		Action = action;
		if (Action is AgentActionPlayAnim)
		{
			AnimName = (Action as AgentActionPlayAnim).AnimName;
			LookAtTarget = false;
			Animation[AnimName].layer = 5;
		}
		else if (Action is AgentActionPlayIdleAnim)
		{
			AnimName = Owner.AnimSet.GetIdleActionAnim();
			LookAtTarget = true;
		}
		if (AnimName == null)
		{
			Action.SetFailed();
			Action = null;
			Release();
			return;
		}
		RotationOk = true;
		CrossFade(AnimName, 0.1f, PlayMode.StopAll);
		if (Animation[AnimName].wrapMode == WrapMode.Loop)
		{
			EndOfStateTime = 100000f + Time.timeSinceLevelLoad;
		}
		else
		{
			EndOfStateTime = Animation[AnimName].length + Time.timeSinceLevelLoad - 0.3f;
		}
	}

	private void UpdateFinalRotation()
	{
		if (!(Owner.BlackBoard.VisibleTarget == null))
		{
			Vector3 vector = Owner.BlackBoard.VisibleTarget.Position - Owner.Position;
			vector.Normalize();
			FinalRotation.SetLookRotation(vector);
			StartRotation = Owner.Transform.rotation;
			float num = Vector3.Angle(Transform.forward, vector);
			if (num > 0f)
			{
				RotationTime = num / 100f;
				RotationOk = false;
				CurrentRotationTime = 0f;
			}
		}
	}

	public override void HandleAnimationEvent(E_AnimEvent animEvent)
	{
	}
}
