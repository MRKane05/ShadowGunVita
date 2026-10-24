using UnityEngine;

public class AnimStateInjury : AnimState
{
	private float MoveTime;

	private float CurrentMoveTime;

	private bool PositionOK;

	private Vector3 Impuls;

	private AgentActionInjury Action;

	private float EndOfStateTime;

	private float PlayAnimTime;

	public AnimStateInjury(Animation anims, AgentHuman owner)
		: base(anims, owner)
	{
	}

	public override void OnActivate(AgentAction action)
	{
		base.OnActivate(action);
		Owner.BlackBoard.MotionType = E_MotionType.None;
		Owner.BlackBoard.MoveDir = Vector3.zero;
		Owner.BlackBoard.Speed = 0f;
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
		if (!PositionOK)
		{
			CurrentMoveTime += Time.deltaTime;
			if (CurrentMoveTime >= MoveTime)
			{
				CurrentMoveTime = MoveTime;
				PositionOK = true;
			}
			float t = Mathf.Max(0f, Mathf.Min(1f, CurrentMoveTime / MoveTime));
			Vector3 vector = Vector3.Lerp(Impuls, Vector3.zero, t);
			if (!MoveEx(vector * Time.deltaTime))
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
		if (action is AgentActionInjury)
		{
			if (Action != null)
			{
				Action.SetSuccess();
			}
			SetFinished(false);
			Initialize(action);
			return true;
		}
		return false;
	}

	protected override void Initialize(AgentAction action)
	{
		base.Initialize(action);
		Action = action as AgentActionInjury;
		if (PlayAnimTime < Time.timeSinceLevelLoad)
		{
			string injuryAnim = Owner.AnimSet.GetInjuryAnim(Action.FromWeapon);
			PlayAnimTime = Time.timeSinceLevelLoad + Animation[injuryAnim].length * 0.35f;
			CrossFade(injuryAnim, 0.25f, PlayMode.StopSameLayer);
			EndOfStateTime = Animation[injuryAnim].length + Time.timeSinceLevelLoad;
		}
		else
		{
			EndOfStateTime = 0.2f + Time.timeSinceLevelLoad;
		}
		Owner.BlackBoard.MotionType = E_MotionType.None;
		MoveTime = Random.Range(0.05f, 0.09f);
		CurrentMoveTime = 0f;
		Impuls = Action.Impuls;
		PositionOK = Impuls == Vector3.zero;
		Owner.BlackBoard.MotionType = E_MotionType.Injury;
	}
}
