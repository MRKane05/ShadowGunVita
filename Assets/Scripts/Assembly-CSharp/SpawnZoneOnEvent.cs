using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Game/SpawnZone On Event")]
public class SpawnZoneOnEvent : SpawnZoneBase
{
	public List<OnGameEvent> GameEvents = new List<OnGameEvent>();

	private void Start()
	{
		foreach (OnGameEvent gameEvent in GameEvents)
		{
			GameBlackboard.Instance.GameEvents.AddEventChangeHandler(gameEvent.Name, EventHandler);
		}
	}

	public void EventHandler(string name, GameEvents.E_State state)
	{
		if (base.State != E_State.E_WAITING_FOR_START)
		{
			return;
		}
		foreach (OnGameEvent gameEvent in GameEvents)
		{
			if (GameBlackboard.Instance.GameEvents.GetState(gameEvent.Name) != gameEvent.State)
			{
				return;
			}
		}
		StartSpawn();
	}
}
