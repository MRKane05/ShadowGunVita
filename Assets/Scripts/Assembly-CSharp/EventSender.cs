using System;
using System.Collections.Generic;

[Serializable]
public class EventSender
{
	public List<GameEvent> GameEvents = new List<GameEvent>();

	public void Send()
	{
		foreach (GameEvent gameEvent in GameEvents)
		{
			Mission.Instance.SendGameEvent(gameEvent.Name, gameEvent.State, gameEvent.Delay);
		}
	}
}
