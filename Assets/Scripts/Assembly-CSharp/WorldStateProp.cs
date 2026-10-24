using System;
using UnityEngine;

[Serializable]
public class WorldStateProp
{
	public E_PropKey PropKey;

	public E_PropType PropType;

	public float Time;

	public bool Bool;

	public int Int;

	public float Float;

	public Vector3 Vector;

	public AgentHuman Agent;

	public E_EventTypes Event;

	public E_CoverState CoverState;

	public string PropName
	{
		get
		{
			return PropKey.ToString();
		}
	}

	public WorldStateProp(bool state)
	{
		Bool = state;
		PropType = E_PropType.Bool;
	}

	public WorldStateProp(int state)
	{
		Int = state;
		PropType = E_PropType.Int;
	}

	public WorldStateProp(float state)
	{
		Float = state;
		PropType = E_PropType.Float;
	}

	public WorldStateProp(AgentHuman state)
	{
		Agent = state;
		PropType = E_PropType.Agent;
	}

	public WorldStateProp(Vector3 vector)
	{
		Vector = vector;
		PropType = E_PropType.Vector;
	}

	public WorldStateProp(E_EventTypes eventType)
	{
		Event = eventType;
		PropType = E_PropType.Event;
	}

	public WorldStateProp(E_CoverState state)
	{
		CoverState = state;
		PropType = E_PropType.CoverState;
	}

	public bool GetBool()
	{
		return Bool;
	}

	public int GetInt()
	{
		return Int;
	}

	public float GetFloat()
	{
		return Float;
	}

	public Vector3 GetVector()
	{
		return Vector;
	}

	public AgentHuman GetAgent()
	{
		return Agent;
	}

	public E_EventTypes GetEvent()
	{
		return Event;
	}

	public E_CoverState GetCoverState()
	{
		return CoverState;
	}

	public override bool Equals(object o)
	{
		WorldStateProp worldStateProp = o as WorldStateProp;
		if (worldStateProp != null)
		{
			if (PropType != worldStateProp.PropType)
			{
				return false;
			}
			switch (PropType)
			{
			case E_PropType.Bool:
				return Bool == worldStateProp.Bool;
			case E_PropType.Int:
				return Int == worldStateProp.Int;
			case E_PropType.Float:
				return Float == worldStateProp.Float;
			case E_PropType.Vector:
				return Vector == worldStateProp.Vector;
			case E_PropType.Agent:
				return Agent == worldStateProp.Agent;
			case E_PropType.Event:
				return Event == worldStateProp.Event;
			case E_PropType.CoverState:
				return CoverState == worldStateProp.CoverState;
			default:
				return false;
			}
		}
		return false;
	}

	public override int GetHashCode()
	{
		return GetHashCode();
	}

	public override string ToString()
	{
		string text = PropName + ": ";
		switch (PropType)
		{
		case E_PropType.Bool:
			text += Bool;
			break;
		case E_PropType.Int:
			text += Int;
			break;
		case E_PropType.Float:
			text += Float;
			break;
		case E_PropType.Vector:
			text += Vector;
			break;
		case E_PropType.Agent:
			text += Agent;
			break;
		case E_PropType.Event:
			text += Event;
			break;
		case E_PropType.CoverState:
			text += CoverState;
			break;
		}
		return text;
	}

	public static bool operator ==(WorldStateProp prop, WorldStateProp other)
	{
		if ((object)prop == null)
		{
			return (object)other == null;
		}
		return prop.Equals(other);
	}

	public static bool operator !=(WorldStateProp prop, WorldStateProp other)
	{
		return !(prop == other);
	}
}
