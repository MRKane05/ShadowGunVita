using UnityEngine;

public class AnimStateKnockdown : AnimState
{
	private enum E_State
	{
		Start = 0,
		Loop = 1,
		Fatality = 2,
		Death = 3,
		End = 4
	}

	private AgentActionKnockdown Action;

	private AgentActionDeath ActionDeath;

	private Quaternion FinalRotation;

	private Quaternion StartRotation;

	private Vector3 StartPosition;

	private Vector3 FinalPosition;

	private float CurrentRotationTime;

	private float RotationTime;

	private float CurrentMoveTime;

	private float MoveTime;

	private float EndOfStateTime;

	private float KnockdownEndTime;

	private bool RotationOk;

	private bool PositionOK;

	private E_State State;

	public AnimStateKnockdown(Animation anims, AgentHuman owner)
		: base(anims, owner)
	{
	}

	public override void OnActivate(AgentAction action)
	{
		base.OnActivate(action);
		Owner.BlackBoard.MotionType = E_MotionType.Knockdown;
		Owner.BlackBoard.MoveDir = Vector3.zero;
		Owner.BlackBoard.Speed = 0f;
	}

	public override void OnDeactivate()
	{
		if (ActionDeath != null)
		{
			ActionDeath.SetSuccess();
		}
		ActionDeath = null;
		Action.SetSuccess();
		Action = null;
		Owner.BlackBoard.MotionType = E_MotionType.None;
		base.OnDeactivate();
	}

	public override void Reset()
	{
		if (ActionDeath != null)
		{
			ActionDeath.SetSuccess();
		}
		ActionDeath = null;
		Action.SetSuccess();
		Action = null;
		base.Reset();
	}

	public override bool HandleNewAction(AgentAction action)
	{
		if (action is AgentActionKnockdown)
		{
			Debug.LogError("obsolete AgentActionBlock arrived");
			action.SetFailed();
			return true;
		}
		if (action is AgentActionDeath)
		{
			ActionDeath = action as AgentActionDeath;
			InitializeDeath();
			return true;
		}
		return false;
	}

	public override void Release()
	{
		SetFinished(true);
	}

	public override void Update()
	{
		if (State == E_State.Death)
		{
			return;
		}
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
		if (!PositionOK)
		{
			CurrentMoveTime += Time.deltaTime;
			if (CurrentMoveTime >= MoveTime)
			{
				CurrentMoveTime = MoveTime;
				PositionOK = true;
			}
			float value = CurrentMoveTime / MoveTime;
			Vector3 vector = Mathfx.Sinerp(StartPosition, FinalPosition, value);
			if (!Move(vector - Transform.position))
			{
				PositionOK = true;
			}
		}
		switch (State)
		{
		case E_State.Start:
			if (EndOfStateTime <= Time.timeSinceLevelLoad)
			{
				InitializeKnockdownLoop();
			}
			break;
		case E_State.Loop:
			if (EndOfStateTime <= Time.timeSinceLevelLoad)
			{
				InitializeKnockdownUp();
			}
			break;
		case E_State.Fatality:
			if (EndOfStateTime <= Time.timeSinceLevelLoad)
			{
				if (ActionDeath != null)
				{
					ActionDeath.SetSuccess();
					ActionDeath = null;
				}
				InitializeDeath();
			}
			break;
		case E_State.End:
			if (EndOfStateTime <= Time.timeSinceLevelLoad)
			{
				Release();
			}
			break;
		case E_State.Death:
			break;
		}
	}

	protected override void Initialize(AgentAction action)
	{
		base.Initialize(action);
		Action = action as AgentActionKnockdown;
		string knockdowAnim = Owner.AnimSet.GetKnockdowAnim(E_KnockdownState.Down);
		StartRotation = Transform.rotation;
		StartPosition = Transform.position;
		Vector3 vector = Action.Attacker.Position - Transform.position;
		float num = 0f;
		if (vector.sqrMagnitude > 0.010000001f)
		{
			vector.Normalize();
			num = Vector3.Angle(Transform.forward, vector);
		}
		else
		{
			vector = Transform.forward;
		}
		FinalRotation.SetLookRotation(vector);
		RotationTime = num / 500f;
		FinalPosition = StartPosition + Action.Impuls;
		MoveTime = Animation[knockdowAnim].length * 0.8f;
		RotationOk = RotationTime == 0f;
		PositionOK = MoveTime == 0f;
		CurrentRotationTime = 0f;
		CurrentMoveTime = 0f;
		CrossFade(knockdowAnim, 0.05f, PlayMode.StopSameLayer);
		EndOfStateTime = Time.timeSinceLevelLoad + Animation[knockdowAnim].length * 0.9f;
		KnockdownEndTime = EndOfStateTime + Action.Time;
		State = E_State.Start;
	}

	private void InitializeKnockdownLoop()
	{
		string knockdowAnim = Owner.AnimSet.GetKnockdowAnim(E_KnockdownState.Loop);
		CrossFade(knockdowAnim, 0.05f, PlayMode.StopSameLayer);
		EndOfStateTime = KnockdownEndTime;
		State = E_State.Loop;
		Owner.DisableCollisions();
	}

	private void InitializeDeath()
	{
		string knockdowAnim = Owner.AnimSet.GetKnockdowAnim(E_KnockdownState.Fatality);
		CrossFade(knockdowAnim, 0.1f, PlayMode.StopSameLayer);
		EndOfStateTime = Time.timeSinceLevelLoad + Animation[knockdowAnim].length * 0.9f;
		ActionDeath.SetSuccess();
		State = E_State.Death;
		Owner.BlackBoard.MotionType = E_MotionType.Death;
	}

	private void InitializeKnockdownUp()
	{
		string knockdowAnim = Owner.AnimSet.GetKnockdowAnim(E_KnockdownState.Up);
		CrossFade(knockdowAnim, 0.05f, PlayMode.StopSameLayer);
		EndOfStateTime = Time.timeSinceLevelLoad + Animation[knockdowAnim].length * 0.9f;
		State = E_State.End;
		Owner.BlackBoard.MotionType = E_MotionType.None;
		Owner.EnableCollisions();
	}

	private void UpdateFinalRotation()
	{
		Vector3 vector = Action.Attacker.Position - Owner.Position;
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
