using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[ExecuteInEditMode]
public class GameEvents
{
	public enum E_State
	{
		False = 0,
		True = 1
	}

	public delegate void EventHandler(string Name, E_State state);

	[SerializeField]
	private List<string> _Names = new List<string>();

	[SerializeField]
	private List<E_State> _States = new List<E_State>();

	private Dictionary<string, EventHandler> EventHandlers = new Dictionary<string, EventHandler>();

	public List<string> Names
	{
		get
		{
			return _Names;
		}
	}

	public int Count
	{
		get
		{
			return _Names.Count;
		}
	}

	private List<string> EventNames
	{
		get
		{
			return _Names;
		}
	}

	public void Clear()
	{
		_Names.Clear();
		_States.Clear();
		Add("RESET", E_State.False);
	}

	public bool Add(string name, E_State state)
	{
		if (_Names.Contains(name))
		{
			return false;
		}
		_Names.Add(name);
		_States.Add(state);
		return true;
	}

	public void Remove(string name)
	{
		int num = FindIndex(name);
		if (num != -1)
		{
			_Names.RemoveAt(num);
			_States.RemoveAt(num);
		}
	}

	public void Update(string name, E_State state)
	{
		int num = FindIndex(name);
		if (num != -1)
		{
			_States[num] = state;
			if (Application.isPlaying && EventHandlers.ContainsKey(name))
			{
				EventHandlers[name](name, state);
			}
		}
	}

	public E_State GetState(string name)
	{
		int num = FindIndex(name);
		if (num == -1)
		{
			return E_State.False;
		}
		if (num > _States.Count - 1)
		{
			return E_State.False;
		}
		return _States[num];
	}

	private int FindIndex(string s)
	{
		for (int i = 0; i < _Names.Count; i++)
		{
			if (_Names[i] == s)
			{
				return i;
			}
		}
		return -1;
	}

	public bool Exist(string inEventName)
	{
		return FindIndex(inEventName) != -1;
	}

	public void AddEventChangeHandler(string name, EventHandler handler)
	{
		if (!_Names.Contains(name))
		{
			Debug.LogError("GameEvents dont contact event " + name);
		}
		else if (EventHandlers.ContainsKey(name))
		{
			Dictionary<string, EventHandler> eventHandlers;
			Dictionary<string, EventHandler> dictionary = (eventHandlers = EventHandlers);
			string key2;
			string key = (key2 = name);
			EventHandler a = eventHandlers[key2];
			dictionary[key] = (EventHandler)Delegate.Combine(a, handler);
		}
		else
		{
			EventHandlers.Add(name, handler);
		}
	}

	public void RemoveEventChangeHandler(string name, EventHandler handler)
	{
		if (!_Names.Contains(name))
		{
			Debug.LogError("GameEvents dont contact event " + name);
			return;
		}
		if (EventHandlers.ContainsKey(name))
		{
			Dictionary<string, EventHandler> eventHandlers;
			Dictionary<string, EventHandler> dictionary = (eventHandlers = EventHandlers);
			string key2;
			string key = (key2 = name);
			EventHandler source = eventHandlers[key2];
			dictionary[key] = (EventHandler)Delegate.Remove(source, handler);
		}
		if (EventHandlers[name] == null)
		{
			EventHandlers.Remove(name);
		}
	}

	public void Save_Save(ref DictionaryFile df)
	{
		for (int i = 0; i < _States.Count; i++)
		{
			df.SetInt(string.Concat(Game.Instance.GameType, "GB", i), (int)_States[i]);
		}
	}

	public void Save_Load()
	{
		for (int i = 0; i < _States.Count; i++)
		{
			_States[i] = (E_State)Game.Instance.Save_LoadedFile.GetInt(string.Concat(Game.Instance.GameType, "GB", i), 0);
		}
	}

	public void Save_LoadOld()
	{
		for (int i = 0; i < _States.Count; i++)
		{
			_States[i] = (E_State)PlayerPrefs.GetInt(string.Concat(Game.Instance.GameType, "GB", i), 0);
		}
	}
}
