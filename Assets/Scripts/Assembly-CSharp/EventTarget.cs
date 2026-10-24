using System;
using System.Collections.Generic;

[Serializable]
public class EventTarget
{
	public delegate void ActivateDelegate();

	public List<OnGameEvent> OnGameEvents = new List<OnGameEvent>();

	public ActivateDelegate OnActivate;

	public ActivateDelegate OnDeactivate;

	public void Initialize()
	{
		foreach (OnGameEvent onGameEvent in OnGameEvents)
		{
			GameBlackboard.Instance.GameEvents.AddEventChangeHandler(onGameEvent.Name, EventHandler);
		}
	}

	public void Reset()
	{
		foreach (OnGameEvent onGameEvent in OnGameEvents)
		{
			GameBlackboard.Instance.GameEvents.RemoveEventChangeHandler(onGameEvent.Name, EventHandler);
		}
	}

	public void EventHandler(string name, GameEvents.E_State state)
	{
		foreach (OnGameEvent onGameEvent in OnGameEvents)
		{
			if (GameBlackboard.Instance.GameEvents.GetState(onGameEvent.Name) != onGameEvent.State)
			{
				if (OnDeactivate != null)
				{
					OnDeactivate();
				}
				return;
			}
		}
		OnActivate();
	}
}
