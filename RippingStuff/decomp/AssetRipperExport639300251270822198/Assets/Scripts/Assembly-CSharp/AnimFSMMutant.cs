using UnityEngine;

public class AnimFSMMutant : AnimFSM
{
	public AnimFSMMutant(Animation anims, AgentHuman owner)
		: base(anims, owner)
	{
	}

	public override void Initialize()
	{
		AnimStates.Clear();
		AnimStates.Add(AgentActionFactory.E_Type.Idle, new AnimStateIdle(AnimEngine, Owner));
		AnimStates.Add(AgentActionFactory.E_Type.Goto, new AnimStateGoTo(AnimEngine, Owner));
		AnimStates.Add(AgentActionFactory.E_Type.CoverEnter, new AnimStateCoverEnter(AnimEngine, Owner));
		AnimStates.Add(AgentActionFactory.E_Type.CoverMove, new AnimStateCoverMove(AnimEngine, Owner));
		AnimStates.Add(AgentActionFactory.E_Type.CoverLeave, new AnimStateCoverLeave(AnimEngine, Owner));
		AnimStates.Add(AgentActionFactory.E_Type.CoverFire, new AnimStateCoverFire(AnimEngine, Owner));
		AnimStates.Add(AgentActionFactory.E_Type.Use, new AnimStateUse(AnimEngine, Owner));
		AnimStates.Add(AgentActionFactory.E_Type.DodgeStrafe, new AnimStateDodge(AnimEngine, Owner));
		AnimStates.Add(AgentActionFactory.E_Type.Death, new AnimStateDeath(AnimEngine, Owner));
		AnimStates.Add(AgentActionFactory.E_Type.PlayAnim, new AnimStatePlayAnim(AnimEngine, Owner));
		DefaultAnimState = AnimStates[AgentActionFactory.E_Type.Idle];
		base.Initialize();
	}
}
