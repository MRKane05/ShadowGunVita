using UnityEngine;

public class AnimFSMPlayer : AnimFSM
{
	public AnimFSMPlayer(Animation anims, AgentHuman owner)
		: base(anims, owner)
	{
	}

	public override void Initialize()
	{
		AnimStates.Add(AgentActionFactory.E_Type.Idle, new AnimStateIdle(AnimEngine, Owner));
		AnimStates.Add(AgentActionFactory.E_Type.Goto, new AnimStateGoToWithoutNavmesh(AnimEngine, Owner));
		AnimStates.Add(AgentActionFactory.E_Type.Move, new AnimStateMove(AnimEngine, Owner));
		AnimStates.Add(AgentActionFactory.E_Type.CoverEnter, new AnimStateCoverEnter(AnimEngine, Owner));
		AnimStates.Add(AgentActionFactory.E_Type.CoverMove, new AnimStateCoverMove(AnimEngine, Owner));
		AnimStates.Add(AgentActionFactory.E_Type.CoverLeave, new AnimStateCoverLeave(AnimEngine, Owner));
		AnimStates.Add(AgentActionFactory.E_Type.CoverFire, new AnimStateCoverFire(AnimEngine, Owner));
		AnimStates.Add(AgentActionFactory.E_Type.Use, new AnimStateUse(AnimEngine, Owner));
		AnimStates.Add(AgentActionFactory.E_Type.Teleport, new AnimStateTeleport(AnimEngine, Owner));
		AnimStates.Add(AgentActionFactory.E_Type.Injury, new AnimStateInjury(AnimEngine, Owner));
		AnimStates.Add(AgentActionFactory.E_Type.Death, new AnimStateDeath(AnimEngine, Owner));
		AnimStates.Add(AgentActionFactory.E_Type.WeaponChange, new AnimStateWeaponChange(AnimEngine, Owner));
		AnimStates.Add(AgentActionFactory.E_Type.PlayAnim, new AnimStatePlayAnim(AnimEngine, Owner));
		AnimStates.Add(AgentActionFactory.E_Type.Roll, new AnimStateRoll(AnimEngine, Owner));
		DefaultAnimState = AnimStates[AgentActionFactory.E_Type.Idle];
		base.Initialize();
	}
}
