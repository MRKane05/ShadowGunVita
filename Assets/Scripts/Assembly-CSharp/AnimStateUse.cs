using UnityEngine;

public class AnimStateUse : AnimState
{
	private enum E_State
	{
		E_PREPARING_FOR_USE = 0,
		E_USING = 1
	}

	private AgentActionUse Action;

	private InteractionObject InterObj;

	private Quaternion FinalRotation;

	private Quaternion StartRotation;

	private Vector3 StartPosition;

	private Vector3 FinalPosition;

	private float MoveTime;

	private float CurrentMoveTime;

	private bool PositionOK;

	private E_State State;

	public AnimStateUse(Animation anims, AgentHuman owner)
		: base(anims, owner)
	{
	}

	public override void OnActivate(AgentAction action)
	{
		base.OnActivate(action);
		Owner.BlackBoard.ReactOnHits = false;
		Owner.BlackBoard.BusyAction = true;
		Owner.BlackBoard.Desires.WeaponTriggerOn = false;
		Owner.BlackBoard.MotionType = E_MotionType.None;
		Owner.BlackBoard.MoveDir = Vector3.zero;
		Owner.BlackBoard.Speed = 0f;
	}

	public override void OnDeactivate()
	{
		Owner.BlackBoard.ReactOnHits = true;
		Owner.BlackBoard.BusyAction = false;
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
		if (State == E_State.E_PREPARING_FOR_USE && !PositionOK)
		{
			CurrentMoveTime += Time.deltaTime;
			if (CurrentMoveTime >= MoveTime)
			{
				CurrentMoveTime = MoveTime;
				PositionOK = true;
			}
			float num = Mathf.Min(1f, CurrentMoveTime / MoveTime);
			Owner.BlackBoard.Desires.Rotation = Quaternion.Lerp(StartRotation, FinalRotation, num);
			Vector3 vector = Mathfx.Sinerp(StartPosition, FinalPosition, num);
			if (!Move(vector - Transform.position))
			{
				PositionOK = true;
			}
		}
		if (State == E_State.E_PREPARING_FOR_USE && PositionOK)
		{
			State = E_State.E_USING;
			PlayAnim();
		}
		if (State == E_State.E_USING && Action.InterObj.IsInteractionFinished)
		{
			Release();
		}
	}

	public override void Release()
	{
		Transform.parent = null;
		base.Release();
	}

	public override bool HandleNewAction(AgentAction action)
	{
		if (action is AgentActionUse && Action != null)
		{
			action.SetFailed();
		}
		return false;
	}

	private void PlayAnim()
	{
		if (Animation.GetClip(Action.InterObj.UserAnimationClip.name) == null)
		{
			Animation.AddClip(Action.InterObj.UserAnimationClip, Action.InterObj.UserAnimationClip.name);
		}
		CrossFade(Action.InterObj.GetUserAnimation(), 0.3f, PlayMode.StopSameLayer);
		Action.InterObj.DoInteraction();
		Owner.BlackBoard.MotionType = E_MotionType.None;
	}

	protected override void Initialize(AgentAction action)
	{
		base.Initialize(action);
		Action = action as AgentActionUse;
		if (Action.InterObj is InteractionObjectCutscene)
		{
			Owner.Transform.position = Action.InterObj.GetEntryTransform().position;
			Owner.Transform.rotation = Action.InterObj.GetEntryTransform().rotation;
			Owner.BlackBoard.Desires.Rotation = Owner.Transform.rotation;
			PositionOK = true;
		}
		else if ((bool)Action.InterObj.GetEntryTransform())
		{
			StartPosition = Transform.position;
			StartRotation = Transform.rotation;
			FinalRotation.SetLookRotation(Action.InterObj.GetEntryTransform().forward);
			FinalPosition = Action.InterObj.GetEntryTransform().position;
			CurrentMoveTime = 0f;
			MoveTime = 0.2f;
			PositionOK = false;
		}
		else
		{
			PositionOK = true;
		}
		State = E_State.E_PREPARING_FOR_USE;
	}
}
