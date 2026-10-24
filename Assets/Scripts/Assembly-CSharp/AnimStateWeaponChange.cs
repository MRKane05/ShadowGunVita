using UnityEngine;
using UnityEngine.AI;

public class AnimStateWeaponChange : AnimState
{
	private enum E_State
	{
		Prepare = 0,
		Hide = 1,
		Show = 2
	}

	private E_State State = E_State.Show;

	private float TimeToFinishState;

	private AgentActionWeaponChange Action;

	public AnimStateWeaponChange(Animation anims, AgentHuman owner)
		: base(anims, owner)
	{
	}

	public override void OnDeactivate()
	{
		if (Action != null)
		{
			Action.SetSuccess();
			Action = null;
		}
	}

	public override void Reset()
	{
		Action.SetSuccess();
		Action = null;
		base.Reset();
	}

	public override bool HandleNewAction(AgentAction action)
	{
		if (action is AgentActionInjury)
		{
			PlayInjuryAnimation(action as AgentActionInjury);
			return true;
		}
		return false;
	}

	public override void Update()
	{
		switch (State)
		{
		case E_State.Prepare:
			if (TimeToFinishState < Time.timeSinceLevelLoad)
			{
				string weaponAnim = Owner.AnimSet.GetWeaponAnim(E_WeaponAction.Disarm);
				CrossFade(weaponAnim, 0.2f, PlayMode.StopAll);
				TimeToFinishState = Animation[weaponAnim].length + Time.timeSinceLevelLoad;
				State = E_State.Hide;
				Owner.WeaponComponent.GetCurrentWeapon().SetBusy(Animation[weaponAnim].length + 0.1f);
			}
			break;
		case E_State.Hide:
			if (TimeToFinishState < Time.timeSinceLevelLoad)
			{
				string weaponAnim2 = Owner.AnimSet.GetWeaponAnim(E_WeaponAction.Arm);
				CrossFade(weaponAnim2, 0.1f, PlayMode.StopAll);
				TimeToFinishState = Animation[weaponAnim2].length + Time.timeSinceLevelLoad - 0.1f;
				State = E_State.Show;
				Owner.WeaponComponent.SwitchWeapons(Action.NewWeapon);
			}
			break;
		case E_State.Show:
			if (TimeToFinishState < Time.timeSinceLevelLoad)
			{
				string idleAnim = Owner.AnimSet.GetIdleAnim();
				CrossFade(idleAnim, 0.1f, PlayMode.StopAll);
				Release();
			}
			break;
		}
		if (Owner.NavMeshAgent == null && !Owner.IsInCover)
		{
			Move(Vector3.zero, false);
		}
	}

	private void PlayIdleAnim()
	{
		string idleAnim = Owner.AnimSet.GetIdleAnim();
		CrossFade(idleAnim, 0.2f, PlayMode.StopSameLayer);
	}

	protected override void Initialize(AgentAction action)
	{
		State = E_State.Prepare;
		base.Initialize(action);
		Owner.BlackBoard.MotionType = E_MotionType.None;
		Owner.BlackBoard.MoveDir = Vector3.zero;
		Owner.BlackBoard.Speed = 0f;
		Action = action as AgentActionWeaponChange;
		TimeToFinishState = 0.2f + Time.timeSinceLevelLoad;
		Owner.WeaponComponent.GetCurrentWeapon().SetBusy(0.3f);
		PlayIdleAnim();
	}
}
