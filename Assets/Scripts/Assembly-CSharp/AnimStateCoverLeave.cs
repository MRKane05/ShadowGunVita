using UnityEngine;
using UnityEngine.AI;

public class AnimStateCoverLeave : AnimState
{
	private AgentActionCoverLeave Action;

	private Quaternion FinalRotation = default(Quaternion);

	private Quaternion StartRotation = default(Quaternion);

	private Vector3 StartPosition;

	private Vector3 FinalPosition;

	private Vector3 StartPositionHelper;

	private Vector3 FinalPositionHelper;

	private float CurrentMoveTime;

	private float MoveTime;

	private float EndOfStateTime;

	private bool PositionOK;

	private bool RotationOK;

	public AnimStateCoverLeave(Animation anims, AgentHuman owner)
		: base(anims, owner)
	{
	}

	public override void OnActivate(AgentAction action)
	{
		base.OnActivate(action);
		Owner.BlackBoard.MotionType = E_MotionType.AnimationDrive;
		Owner.BlackBoard.ReactOnHits = false;
		Owner.BlackBoard.BusyAction = true;
		Owner.BlackBoard.Desires.WeaponTriggerOn = false;
		if ((bool)Owner.NavMeshAgent)
		{
			Owner.NavMeshAgent.enabled = false;
		}
	}

	public override void OnDeactivate()
	{
		Owner.BlackBoard.ReactOnHits = true;
		Owner.BlackBoard.BusyAction = false;
		if ((bool)Owner.BlackBoard.Cover)
		{
			Owner.BlackBoard.Cover.LockPosition(Owner.BlackBoard.CoverPosition, null);
			Owner.WorldState.SetWSProperty(E_PropKey.CoverState, E_CoverState.None);
			Owner.BlackBoard.CoverPosition = E_CoverDirection.Unknown;
			Owner.BlackBoard.Cover = null;
		}
		Action.SetSuccess();
		Action = null;
		if ((bool)Owner.NavMeshAgent)
		{
			Owner.NavMeshAgent.enabled = true;
			Owner.NavMeshAgent.updatePosition = true;
		}
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
		if (!PositionOK)
		{
			CurrentMoveTime += Time.deltaTime;
			if (CurrentMoveTime >= MoveTime)
			{
				CurrentMoveTime = MoveTime;
				PositionOK = true;
				Action.SetSuccess();
				Owner.BlackBoard.BusyAction = false;
				Owner.BlackBoard.Cover.LockPosition(Owner.BlackBoard.CoverPosition, null);
				Owner.WorldState.SetWSProperty(E_PropKey.CoverState, E_CoverState.None);
				Owner.BlackBoard.CoverPosition = E_CoverDirection.Unknown;
				Owner.BlackBoard.Cover = null;
				Owner.BlackBoard.Desires.CoverNear.Cover = null;
				Release();
			}
			if (CurrentMoveTime > 0f)
			{
				float num = CurrentMoveTime / MoveTime;
				if (Action.TypeOfLeave == AgentActionCoverLeave.E_Type.Right || Action.TypeOfLeave == AgentActionCoverLeave.E_Type.Left)
				{
					Transform.position = Mathfx.InterpolateCatmullRom(StartPositionHelper, StartPosition, FinalPosition, FinalPositionHelper, num);
				}
				else
				{
					Transform.position = Mathfx.Hermite(StartPosition, FinalPosition, num);
				}
				if (!RotationOK)
				{
					Owner.Transform.rotation = Quaternion.Lerp(StartRotation, FinalRotation, num);
				}
			}
		}
		if (EndOfStateTime <= Time.timeSinceLevelLoad)
		{
			Release();
		}
	}

	public override bool HandleNewAction(AgentAction action)
	{
		if (action is AgentActionCoverLeave)
		{
			if (Action != null)
			{
				Debug.LogError(ToString() + ": Second action is not allowed");
				action.SetFailed();
				return true;
			}
			Initialize(action);
			return true;
		}
		return false;
	}

	protected override void Initialize(AgentAction action)
	{
		base.Initialize(action);
		Action = action as AgentActionCoverLeave;
		StartPosition = Transform.position;
		string coverAnim;
		if (Action.TypeOfLeave == AgentActionCoverLeave.E_Type.Right)
		{
			coverAnim = Owner.AnimSet.GetCoverAnim(AnimSet.E_CoverAnim.LeaveRight, Owner.BlackBoard.CoverPose, E_CoverDirection.Right);
			FinalPosition = Owner.Position + Owner.BlackBoard.Cover.Right * 1f + Owner.Forward * 1.2f;
			StartPositionHelper = StartPosition - Owner.Right;
			FinalPositionHelper = FinalPosition + Owner.Forward;
			CurrentMoveTime = -0.1f;
			Animation[coverAnim].speed = 0.5f;
			MoveTime = 0.3f;
			EndOfStateTime = Animation[coverAnim].length * 1.6f + Time.timeSinceLevelLoad;
			Owner.BlackBoard.Speed = 5f;
			RotationOK = true;
		}
		else if (Action.TypeOfLeave == AgentActionCoverLeave.E_Type.Left)
		{
			coverAnim = Owner.AnimSet.GetCoverAnim(AnimSet.E_CoverAnim.LeaveLeft, Owner.BlackBoard.CoverPose, E_CoverDirection.Left);
			FinalPosition = Owner.Position - Owner.BlackBoard.Cover.Right * 1f + Owner.Forward * 1.2f;
			StartPositionHelper = StartPosition + Owner.Right;
			FinalPositionHelper = FinalPosition + Owner.Forward;
			CurrentMoveTime = -0.2f;
			Animation[coverAnim].speed = 0.5f;
			MoveTime = 0.2f;
			EndOfStateTime = Animation[coverAnim].length * 1.6f + Time.timeSinceLevelLoad;
			Owner.BlackBoard.Speed = 5f;
			RotationOK = true;
		}
		else if (Action.TypeOfLeave == AgentActionCoverLeave.E_Type.Jump)
		{
			coverAnim = Owner.AnimSet.GetCoverAnim(AnimSet.E_CoverAnim.JumpOver, Owner.BlackBoard.CoverPose, E_CoverDirection.Middle);
			FinalPosition = Owner.Position + Owner.Forward * 2f;
			CurrentMoveTime = -0.3f;
			MoveTime = 0.6f;
			EndOfStateTime = Animation[coverAnim].length * 0.95f + Time.timeSinceLevelLoad;
			Owner.BlackBoard.Speed = 5f;
			RotationOK = true;
		}
		else
		{
			coverAnim = Owner.AnimSet.GetCoverAnim(AnimSet.E_CoverAnim.Leave, Owner.BlackBoard.CoverPose, E_CoverDirection.Middle);
			FinalPosition = Owner.Position - Owner.BlackBoard.Cover.Forward * 1.1f;
			StartRotation = Owner.Transform.rotation;
			FinalRotation.eulerAngles = new Vector3(0f, Owner.BlackBoard.Desires.Rotation.eulerAngles.y, 0f);
			MoveTime = Animation[coverAnim].length * 0.7f;
			CurrentMoveTime = 0f;
			EndOfStateTime = Animation[coverAnim].length * 0.5f + Time.timeSinceLevelLoad;
			RotationOK = false;
		}
		CrossFade(coverAnim, 0.1f, PlayMode.StopSameLayer);
		Owner.WeaponComponent.DisableCurrentWeapon(Animation[coverAnim].length);
		PositionOK = false;
		Owner.BlackBoard.MotionType = E_MotionType.None;
	}
}
