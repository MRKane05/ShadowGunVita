using UnityEngine;
using UnityEngine.AI;

public class AnimStateInjuryCrit : AnimState
{
	private float MoveTime;

	private float CurrentMoveTime;

	private Vector3 Impuls;

	private AgentActionInjuryCrit Action;

	private float EndOfStateTime;

	public AnimStateInjuryCrit(Animation anims, AgentHuman owner)
		: base(anims, owner)
	{
	}

	public override void OnActivate(AgentAction action)
	{
		base.OnActivate(action);
		if (Owner.NavMeshAgent.enabled)
		{
			Owner.NavMeshAgent.Stop();
		}
		Owner.BlackBoard.MotionType = E_MotionType.None;
		Owner.BlackBoard.MoveDir = Vector3.zero;
		Owner.BlackBoard.Speed = 0f;
	}

	public override void OnDeactivate()
	{
		Owner.BlackBoard.MotionType = E_MotionType.None;
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
		if (EndOfStateTime <= Time.timeSinceLevelLoad)
		{
			Release();
		}
	}

	public override bool HandleNewAction(AgentAction action)
	{
		if (action is AgentActionInjury)
		{
			SetFinished(false);
			PlayInjuryAnimation(action as AgentActionInjury);
			return true;
		}
		return false;
	}

	protected override void Initialize(AgentAction action)
	{
		base.Initialize(action);
		Action = action as AgentActionInjuryCrit;
		string injuryCritAnim = Owner.AnimSet.GetInjuryCritAnim();
		CrossFade(injuryCritAnim, 0.25f, PlayMode.StopSameLayer);
		EndOfStateTime = Animation[injuryCritAnim].length * 0.95f + Time.timeSinceLevelLoad;
		Owner.BlackBoard.MotionType = E_MotionType.None;
		Owner.BlackBoard.MotionType = E_MotionType.Injury;
	}
}
