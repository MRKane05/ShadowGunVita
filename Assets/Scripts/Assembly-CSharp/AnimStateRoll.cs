using UnityEngine;

public class AnimStateRoll : AnimState
{
	private AgentActionRoll Action;

	private Vector3 StartPosition;

	private Vector3 FinalPosition;

	private float CurrentMoveTime;

	private float MoveTime;

	private bool PositionOK;

	private float EndOfStateTime;

	public AnimStateRoll(Animation anims, AgentHuman owner)
		: base(anims, owner)
	{
	}

	public override void OnActivate(AgentAction action)
	{
		base.OnActivate(action);
		Owner.BlackBoard.MotionType = E_MotionType.Roll;
		Owner.BlackBoard.BusyAction = true;
		Owner.BlackBoard.ReactOnHits = false;
	}

	public override void OnDeactivate()
	{
		Owner.BlackBoard.BusyAction = false;
		Owner.BlackBoard.ReactOnHits = true;
		Owner.BlackBoard.MotionType = E_MotionType.None;
		Action.SetSuccess();
		Action = null;
		base.OnDeactivate();
	}

	public override void Reset()
	{
		Owner.BlackBoard.MotionType = E_MotionType.None;
		Owner.BlackBoard.BusyAction = false;
		Owner.BlackBoard.ReactOnHits = true;
		Action.SetSuccess();
		Action = null;
		base.Reset();
	}

	public override void Update()
	{
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
		if (EndOfStateTime <= Time.timeSinceLevelLoad)
		{
			Release();
		}
	}

	public override bool HandleNewAction(AgentAction action)
	{
		if (action is AgentActionDodgeStrafe)
		{
			if (Action != null)
			{
				Action.SetSuccess();
			}
			Initialize(action);
			return true;
		}
		return false;
	}

	protected override void Initialize(AgentAction action)
	{
		base.Initialize(action);
		Action = action as AgentActionRoll;
		StartPosition = Transform.position;
		switch (Action.Direction)
		{
		case E_Direction.Forward:
			FinalPosition = StartPosition + Owner.Forward * Owner.BlackBoard.BaseSetup.RollDistance;
			break;
		case E_Direction.Right:
			FinalPosition = StartPosition + Owner.Right * Owner.BlackBoard.BaseSetup.RollDistance;
			break;
		case E_Direction.Left:
			FinalPosition = StartPosition - Owner.Right * Owner.BlackBoard.BaseSetup.RollDistance;
			break;
		case E_Direction.Backward:
			FinalPosition = StartPosition - Owner.Forward * Owner.BlackBoard.BaseSetup.RollDistance;
			break;
		}
		string rollAnim = Owner.AnimSet.GetRollAnim(Action.Direction);
		CrossFade(rollAnim, 0.1f, PlayMode.StopSameLayer);
		CurrentMoveTime = 0f;
		MoveTime = Animation[rollAnim].length * 0.8f;
		EndOfStateTime = Animation[rollAnim].length * 0.85f + Time.timeSinceLevelLoad;
		PositionOK = false;
	}
}
