using System.Collections.Generic;
using UnityEngine;

public abstract class AnimFSM
{
	protected Dictionary<AgentActionFactory.E_Type, AnimState> AnimStates = new Dictionary<AgentActionFactory.E_Type, AnimState>(0);

	protected AnimState NextAnimState;

	protected AnimState DefaultAnimState;

	protected Animation AnimEngine;

	protected AgentHuman Owner;

	public AnimState CurrentAnimState { get; private set; }

	public AnimFSM(Animation anims, AgentHuman owner)
	{
		AnimEngine = anims;
		Owner = owner;
	}

	public virtual void Initialize()
	{
	}

	public virtual void Activate()
	{
		CurrentAnimState = DefaultAnimState;
		CurrentAnimState.OnActivate(null);
		NextAnimState = null;
	}

	public void UpdateAnimStates()
	{
		if (CurrentAnimState.IsFinished())
		{
			CurrentAnimState.OnDeactivate();
			CurrentAnimState = DefaultAnimState;
			CurrentAnimState.OnActivate(null);
		}
		CurrentAnimState.Update();
	}

	public void Reset()
	{
		if (!CurrentAnimState.IsFinished())
		{
			CurrentAnimState.SetFinished(true);
			CurrentAnimState.Reset();
		}
	}

	public bool DoAction(AgentAction action)
	{
		if (CurrentAnimState.HandleNewAction(action))
		{
			NextAnimState = null;
			return true;
		}
		if (AnimStates.ContainsKey(action.Type))
		{
			NextAnimState = AnimStates[action.Type];
			SwitchToNewStage(action);
			return true;
		}
		return false;
	}

	protected void SwitchToNewStage(AgentAction action)
	{
		if (NextAnimState != null)
		{
			CurrentAnimState.Release();
			CurrentAnimState.OnDeactivate();
			CurrentAnimState = NextAnimState;
			CurrentAnimState.OnActivate(action);
			NextAnimState = null;
		}
	}
}
