using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class Fact
{
	public enum E_DataTypes
	{
		Event = 0,
		Pos = 1,
		Dir = 2,
		Agent = 3,
		Cover = 4,
		CoverDirection = 5,
		Count = 6
	}

	public float LiveTime = 0.2f;

	private BitArray m_DataTypesSet = new BitArray(6);

	public AgentHuman Causer;

	public float Belief = 1f;

	public float Delay;

	private E_EventTypes _EventType;

	private Vector3 _Pos;

	private Vector3 _Dir;

	private AgentHuman _Agent;

	private Cover _Cover;

	private E_CoverDirection _CoverDirection;

	public bool Deleted;

	public E_EventTypes Type
	{
		get
		{
			return _EventType;
		}
		set
		{
			_EventType = value;
			m_DataTypesSet.Set(0, true);
		}
	}

	public Vector3 Position
	{
		get
		{
			return _Pos;
		}
		set
		{
			_Pos = value;
			m_DataTypesSet.Set(1, true);
		}
	}

	public Vector3 Direction
	{
		get
		{
			return _Dir;
		}
		set
		{
			_Dir = value;
			m_DataTypesSet.Set(2, true);
		}
	}

	public AgentHuman Agent
	{
		get
		{
			return _Agent;
		}
		set
		{
			_Agent = value;
			m_DataTypesSet.Set(3, true);
		}
	}

	public Cover Cover
	{
		get
		{
			return _Cover;
		}
		set
		{
			_Cover = value;
			m_DataTypesSet.Set(4, true);
		}
	}

	public E_CoverDirection CoverDirection
	{
		get
		{
			return _CoverDirection;
		}
		set
		{
			_CoverDirection = value;
			m_DataTypesSet.Set(5, true);
		}
	}

	public void Reset()
	{
		Belief = 1f;
		Position = Vector3.zero;
		Direction = Vector3.zero;
		Agent = null;
		Deleted = false;
		Delay = 0f;
		m_DataTypesSet.SetAll(false);
	}

	public void Update()
	{
		if (Delay > 0f)
		{
			Delay = Mathf.Max(0f, Delay - AgentHuman.AgentUpdateTime);
		}
		else if (Belief > 0f)
		{
			Belief -= 1f / LiveTime * AgentHuman.AgentUpdateTime;
			Belief = Mathf.Max(0f, Belief);
		}
	}

	public override string ToString()
	{
		string text = base.ToString() + " : ";
		text = text + " " + Type;
		if (m_DataTypesSet.Get(3))
		{
			text = text + " " + Agent.name;
		}
		return text;
	}
}
