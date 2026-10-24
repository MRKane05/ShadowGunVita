using System;
using UnityEngine;

[Serializable]
public class WorldState
{
	internal WorldStateProp[] WorldStateProperties = new WorldStateProp[19];

	public WorldStateProp GetWSProperty(E_PropKey key)
	{
		return ((int)key >= WorldStateProperties.Length) ? null : WorldStateProperties[(int)key];
	}

	public bool IsWSPropertySet(E_PropKey key)
	{
		return (int)key < WorldStateProperties.Length && WorldStateProperties[(int)key] != null;
	}

	public void SetWSProperty(E_PropKey key, bool value)
	{
		if (IsWSPropertySet(key))
		{
			WorldStatePropFactory.Return(WorldStateProperties[(int)key]);
			WorldStateProperties[(int)key] = WorldStatePropFactory.Create(key, value);
		}
		else
		{
			WorldStateProperties[(int)key] = WorldStatePropFactory.Create(key, value);
		}
	}

	public void SetWSProperty(E_PropKey key, float value)
	{
		if (IsWSPropertySet(key))
		{
			WorldStatePropFactory.Return(WorldStateProperties[(int)key]);
			WorldStateProperties[(int)key] = WorldStatePropFactory.Create(key, value);
		}
		else
		{
			WorldStateProperties[(int)key] = WorldStatePropFactory.Create(key, value);
		}
	}

	public void SetWSProperty(E_PropKey key, int value)
	{
		if (IsWSPropertySet(key))
		{
			WorldStatePropFactory.Return(WorldStateProperties[(int)key]);
			WorldStateProperties[(int)key] = WorldStatePropFactory.Create(key, value);
		}
		else
		{
			WorldStateProperties[(int)key] = WorldStatePropFactory.Create(key, value);
		}
	}

	public void SetWSProperty(E_PropKey key, AgentHuman value)
	{
		if (IsWSPropertySet(key))
		{
			WorldStatePropFactory.Return(WorldStateProperties[(int)key]);
			WorldStateProperties[(int)key] = WorldStatePropFactory.Create(key, value);
		}
		else
		{
			WorldStateProperties[(int)key] = WorldStatePropFactory.Create(key, value);
		}
	}

	public void SetWSProperty(E_PropKey key, Vector3 value)
	{
		if (IsWSPropertySet(key))
		{
			WorldStatePropFactory.Return(WorldStateProperties[(int)key]);
			WorldStateProperties[(int)key] = WorldStatePropFactory.Create(key, value);
		}
		else
		{
			WorldStateProperties[(int)key] = WorldStatePropFactory.Create(key, value);
		}
	}

	public void SetWSProperty(E_PropKey key, E_EventTypes value)
	{
		if (IsWSPropertySet(key))
		{
			WorldStatePropFactory.Return(WorldStateProperties[(int)key]);
			WorldStateProperties[(int)key] = WorldStatePropFactory.Create(key, value);
		}
		else
		{
			WorldStateProperties[(int)key] = WorldStatePropFactory.Create(key, value);
		}
	}

	public void SetWSProperty(E_PropKey key, E_CoverState value)
	{
		if (IsWSPropertySet(key))
		{
			WorldStatePropFactory.Return(WorldStateProperties[(int)key]);
			WorldStateProperties[(int)key] = WorldStatePropFactory.Create(key, value);
		}
		else
		{
			WorldStateProperties[(int)key] = WorldStatePropFactory.Create(key, value);
		}
	}

	public void SetWSProperty(WorldStateProp other)
	{
		if (!(other == null))
		{
			switch (other.PropType)
			{
			case E_PropType.Bool:
				SetWSProperty(other.PropKey, other.GetBool());
				break;
			case E_PropType.Int:
				SetWSProperty(other.PropKey, other.GetInt());
				break;
			case E_PropType.Float:
				SetWSProperty(other.PropKey, other.GetFloat());
				break;
			case E_PropType.Vector:
				SetWSProperty(other.PropKey, other.GetVector());
				break;
			case E_PropType.Agent:
				SetWSProperty(other.PropKey, other.GetAgent());
				break;
			case E_PropType.Event:
				SetWSProperty(other.PropKey, other.GetEvent());
				break;
			case E_PropType.CoverState:
				SetWSProperty(other.PropKey, other.GetCoverState());
				break;
			default:
				Debug.LogError("error in SetWSProperty " + other.PropKey);
				break;
			}
		}
	}

	public void ResetWSProperty(E_PropKey key)
	{
		if (IsWSPropertySet(key))
		{
			WorldStatePropFactory.Return(WorldStateProperties[(int)key]);
			WorldStateProperties[(int)key] = null;
		}
	}

	public void Reset()
	{
		for (int i = 0; i < WorldStateProperties.Length; i++)
		{
			if (WorldStateProperties[i] != null)
			{
				WorldStatePropFactory.Return(WorldStateProperties[i]);
				WorldStateProperties[i] = null;
			}
		}
	}

	public void CopyWorldState(WorldState otherState)
	{
		Reset();
		for (int i = 0; i < otherState.WorldStateProperties.Length; i++)
		{
			if (otherState.WorldStateProperties[i] != null)
			{
				SetWSProperty(otherState.WorldStateProperties[i]);
			}
		}
	}

	public int GetNumWorldStateDifferences(WorldState otherState)
	{
		int num = 0;
		for (int i = 0; i < WorldStateProperties.Length; i++)
		{
			if (otherState.WorldStateProperties[i] != null && WorldStateProperties[i] != null)
			{
				if (!(WorldStateProperties[i] == otherState.WorldStateProperties[i]))
				{
					num++;
				}
			}
			else if (otherState.WorldStateProperties[i] != null || WorldStateProperties[i] != null)
			{
				num++;
			}
		}
		return num;
	}

	public int GetNumUnsatisfiedWorldStateProps(WorldState otherState)
	{
		int num = 0;
		for (E_PropKey e_PropKey = E_PropKey.Start; e_PropKey < E_PropKey.Count; e_PropKey++)
		{
			if (IsWSPropertySet(e_PropKey))
			{
				if (!otherState.IsWSPropertySet(e_PropKey))
				{
					num++;
				}
				if (!(GetWSProperty(e_PropKey) == otherState.GetWSProperty(e_PropKey)))
				{
					num++;
				}
			}
		}
		return num;
	}

	public override string ToString()
	{
		string text = "World state : ";
		for (E_PropKey e_PropKey = E_PropKey.Start; e_PropKey < E_PropKey.Count; e_PropKey++)
		{
			if (IsWSPropertySet(e_PropKey))
			{
				text = text + " " + GetWSProperty(e_PropKey).ToString();
			}
		}
		return text;
	}
}
