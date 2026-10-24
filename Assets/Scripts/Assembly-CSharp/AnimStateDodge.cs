using UnityEngine;
using UnityEngine.AI;

public class AnimStateDodge : AnimState
{
	private AgentActionDodgeStrafe Action;

	private float EndOfStateTime;

	public AnimStateDodge(Animation anims, AgentHuman owner)
		: base(anims, owner)
	{
	}

	public override void OnActivate(AgentAction action)
	{
		base.OnActivate(action);
		Owner.BlackBoard.BusyAction = true;
	}

	public override void OnDeactivate()
	{
		Owner.BlackBoard.BusyAction = false;
		if (Owner.NavMeshAgent.enabled)
		{
			Owner.NavMeshAgent.Stop();
		}
		Action.SetSuccess();
		Action = null;
		base.OnDeactivate();
	}

	public override void Reset()
	{
		Owner.BlackBoard.BusyAction = false;
		Action.SetSuccess();
		Action = null;
		base.Reset();
	}

	public override void Update()
	{
		if (Action.AnimationDriven)
		{
			if (EndOfStateTime <= Time.timeSinceLevelLoad)
			{
				Release();
			}
		}
		else if (Owner.NavMeshAgent.remainingDistance < 0.2f)
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
		if (action is AgentActionInjury)
		{
			PlayInjuryAnimation(action as AgentActionInjury);
			return true;
		}
		return false;
	}

	protected override void Initialize(AgentAction action)
	{
		base.Initialize(action);
		Action = action as AgentActionDodgeStrafe;
		Vector3 targetLocation = ((Action.StrafeDirection != E_StrafeDirection.Right) ? (Transform.position - Owner.Right * Owner.BlackBoard.BaseSetup.RollDistance) : (Transform.position + Owner.Right * Owner.BlackBoard.BaseSetup.RollDistance));
		if (!SetTargetLocation(targetLocation))
		{
			Release();
			return;
		}
		Owner.BlackBoard.MotionType = Action.Motion;
		if (Action.Motion == E_MotionType.Run)
		{
			Owner.NavMeshAgent.speed = Owner.BlackBoard.BaseSetup.MaxRunSpeed;
		}
		else
		{
			Owner.NavMeshAgent.speed = Owner.BlackBoard.BaseSetup.MaxWalkSpeed;
		}
		string dodgeAnim = Owner.AnimSet.GetDodgeAnim(Action.StrafeDirection);
		CrossFade(dodgeAnim, 0.1f, PlayMode.StopSameLayer);
		EndOfStateTime = Animation[dodgeAnim].length * 0.9f + Time.timeSinceLevelLoad;
	}
}
