using System;

[Serializable]
public class OnGameEvent
{
	public string Name;

	public GameEvents.E_State State;

	public OnGameEvent(string name, GameEvents.E_State state)
	{
		Name = name;
		State = state;
	}
}
