using UnityEngine;

public static class WorldStatePropFactory
{
	private static Queue<WorldStateProp> m_UnusedProps = new Queue<WorldStateProp>();

	public static WorldStateProp Create(E_PropKey key, bool state)
	{
		WorldStateProp worldStateProp = null;
		if (m_UnusedProps.Count != 0)
		{
			worldStateProp = m_UnusedProps.Dequeue();
			worldStateProp.Bool = state;
			worldStateProp.PropType = E_PropType.Bool;
		}
		else
		{
			worldStateProp = new WorldStateProp(state);
		}
		worldStateProp.Time = Time.timeSinceLevelLoad;
		worldStateProp.PropKey = key;
		return worldStateProp;
	}

	public static WorldStateProp Create(E_PropKey key, int state)
	{
		WorldStateProp worldStateProp;
		if (m_UnusedProps.Count != 0)
		{
			worldStateProp = m_UnusedProps.Dequeue();
			worldStateProp.Int = state;
			worldStateProp.PropType = E_PropType.Int;
		}
		else
		{
			worldStateProp = new WorldStateProp(state);
		}
		worldStateProp.Time = Time.timeSinceLevelLoad;
		worldStateProp.PropKey = key;
		return worldStateProp;
	}

	public static WorldStateProp Create(E_PropKey key, float state)
	{
		WorldStateProp worldStateProp;
		if (m_UnusedProps.Count != 0)
		{
			worldStateProp = m_UnusedProps.Dequeue();
			worldStateProp.PropKey = key;
			worldStateProp.Float = state;
		}
		else
		{
			worldStateProp = new WorldStateProp(state);
		}
		worldStateProp.Time = Time.timeSinceLevelLoad;
		worldStateProp.PropType = E_PropType.Float;
		return worldStateProp;
	}

	public static WorldStateProp Create(E_PropKey key, AgentHuman state)
	{
		WorldStateProp worldStateProp = null;
		if (m_UnusedProps.Count != 0)
		{
			worldStateProp = m_UnusedProps.Dequeue();
			worldStateProp.Agent = state;
			worldStateProp.PropType = E_PropType.Agent;
		}
		else
		{
			worldStateProp = new WorldStateProp(state);
		}
		worldStateProp.Time = Time.timeSinceLevelLoad;
		worldStateProp.PropKey = key;
		return worldStateProp;
	}

	public static WorldStateProp Create(E_PropKey key, Vector3 vector)
	{
		WorldStateProp worldStateProp = null;
		if (m_UnusedProps.Count != 0)
		{
			worldStateProp = m_UnusedProps.Dequeue();
			worldStateProp.Vector = vector;
			worldStateProp.PropType = E_PropType.Vector;
		}
		else
		{
			worldStateProp = new WorldStateProp(vector);
		}
		worldStateProp.Time = Time.timeSinceLevelLoad;
		worldStateProp.PropKey = key;
		return worldStateProp;
	}

	public static WorldStateProp Create(E_PropKey key, E_EventTypes eventType)
	{
		WorldStateProp worldStateProp = null;
		if (m_UnusedProps.Count != 0)
		{
			worldStateProp = m_UnusedProps.Dequeue();
			worldStateProp.Event = eventType;
			worldStateProp.PropType = E_PropType.Event;
		}
		else
		{
			worldStateProp = new WorldStateProp(eventType);
		}
		worldStateProp.Time = Time.timeSinceLevelLoad;
		worldStateProp.PropKey = key;
		return worldStateProp;
	}

	public static WorldStateProp Create(E_PropKey key, E_CoverState state)
	{
		WorldStateProp worldStateProp = null;
		if (m_UnusedProps.Count != 0)
		{
			worldStateProp = m_UnusedProps.Dequeue();
			worldStateProp.CoverState = state;
			worldStateProp.PropType = E_PropType.CoverState;
		}
		else
		{
			worldStateProp = new WorldStateProp(state);
		}
		worldStateProp.Time = Time.timeSinceLevelLoad;
		worldStateProp.PropKey = key;
		return worldStateProp;
	}

	public static void Return(WorldStateProp prop)
	{
		prop.PropKey = E_PropKey.Count;
		m_UnusedProps.Enqueue(prop);
	}
}
