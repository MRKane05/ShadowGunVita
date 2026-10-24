using System;

[Serializable]
public class GameEvent
{
	public string Name;

	public GameEvents.E_State State;

	public float Delay;

	public GameEvents.E_State invertedState
	{
		get
		{
			return (State != GameEvents.E_State.True) ? GameEvents.E_State.True : GameEvents.E_State.False;
		}
	}

	public GameEvent(string name, GameEvents.E_State state, float delay)
	{
		Name = name;
		State = state;
		Delay = delay;
	}
}
