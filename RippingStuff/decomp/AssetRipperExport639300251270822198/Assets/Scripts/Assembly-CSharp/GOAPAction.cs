using UnityEngine;

public abstract class GOAPAction
{
	public WorldState WorldPreconditions;

	public WorldState WorldEffects;

	public float Cost;

	public bool Interruptible = true;

	public int Precedence;

	public E_GOAPAction Type;

	public AgentHuman Owner;

	protected GOAPAction(E_GOAPAction type, AgentHuman owner)
	{
		WorldPreconditions = new WorldState();
		WorldEffects = new WorldState();
		Type = type;
		Owner = owner;
	}

	public abstract void InitAction();

	public virtual void Update()
	{
	}

	public virtual void SolvePlanWSVariable(WorldState currentState, WorldState goalState)
	{
		for (int i = 0; i < 19; i++)
		{
			WorldStateProp wSProperty = WorldEffects.GetWSProperty((E_PropKey)i);
			if (wSProperty != null)
			{
				WorldStateProp wSProperty2 = goalState.GetWSProperty(wSProperty.PropKey);
				if (wSProperty2 != null)
				{
					currentState.SetWSProperty(wSProperty2);
				}
			}
		}
	}

	public bool ValidateWSEffects(WorldState current, WorldState goal)
	{
		if (WorldEffects.GetNumUnsatisfiedWorldStateProps(current) == 0)
		{
			return true;
		}
		return false;
	}

	public virtual bool ValidateContextPreconditions(WorldState current, bool planning)
	{
		return true;
	}

	public bool ValidateWSPreconditions(WorldState current, WorldState goal)
	{
		if (WorldPreconditions.GetNumUnsatisfiedWorldStateProps(current) == 0)
		{
			return true;
		}
		return false;
	}

	public virtual void SetPlanWSPreconditions(WorldState goalState)
	{
		for (E_PropKey e_PropKey = E_PropKey.Start; e_PropKey < E_PropKey.Count; e_PropKey++)
		{
			WorldStateProp wSProperty = WorldPreconditions.GetWSProperty(e_PropKey);
			if (wSProperty != null)
			{
				goalState.SetWSProperty(wSProperty);
			}
		}
	}

	public void ApplyWSEffects(WorldState currentState, WorldState goalState)
	{
		for (E_PropKey e_PropKey = E_PropKey.Start; e_PropKey < E_PropKey.Count; e_PropKey++)
		{
			WorldStateProp wSProperty = WorldEffects.GetWSProperty(e_PropKey);
			if (wSProperty != null)
			{
				currentState.SetWSProperty(wSProperty);
			}
		}
	}

	public virtual bool ValidateAction()
	{
		return true;
	}

	public virtual bool IsActionComplete()
	{
		return false;
	}

	public virtual void Activate()
	{
	}

	public virtual void Deactivate()
	{
	}

	protected virtual void DebugLogActivate()
	{
		Debug.Log(Time.timeSinceLevelLoad + " " + ToString() + " - Activated");
	}
}
