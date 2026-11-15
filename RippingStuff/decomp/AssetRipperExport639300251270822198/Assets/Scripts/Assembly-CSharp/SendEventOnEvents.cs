using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Interaction/Send Event On Events")]
public class SendEventOnEvents : MonoBehaviour, IGameZoneControledObject
{
	public List<GameEvent> GameEvents = new List<GameEvent>();

	public List<OnGameEvent> OnGameEvents = new List<OnGameEvent>();

	private void Awake()
	{
	}

	private void Start()
	{
		GameZone firstComponentUpward = base.gameObject.GetFirstComponentUpward<GameZone>();
		firstComponentUpward.RegisterControllableObject(this);
		foreach (OnGameEvent onGameEvent in OnGameEvents)
		{
			GameBlackboard.Instance.GameEvents.AddEventChangeHandler(onGameEvent.Name, EventHandler);
		}
	}

	public void Enable()
	{
		TestEvents();
	}

	public void Reset()
	{
	}

	public void Disable()
	{
	}

	private void TestEvents()
	{
		foreach (OnGameEvent onGameEvent in OnGameEvents)
		{
			if (GameBlackboard.Instance.GameEvents.GetState(onGameEvent.Name) != onGameEvent.State)
			{
				return;
			}
		}
		foreach (GameEvent gameEvent in GameEvents)
		{
			if (GameBlackboard.Instance.GameEvents.GetState(gameEvent.Name) != gameEvent.State)
			{
				Mission.Instance.SendGameEvent(gameEvent.Name, gameEvent.State, gameEvent.Delay);
			}
		}
	}

	public void EventHandler(string name, GameEvents.E_State state)
	{
		TestEvents();
	}
}
