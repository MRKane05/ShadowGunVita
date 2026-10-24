using System;
using System.Collections.Generic;

[Serializable]
public class Memory
{
	private Dictionary<E_EventTypes, Fact> Facts;

	public Memory(AgentHuman owner)
	{
		Facts = new Dictionary<E_EventTypes, Fact>();
	}

	private void DeleteFact(Fact f)
	{
		f.Deleted = false;
		FactsFactory.Return(f);
	}

	public void AddFact(Fact fact)
	{
		if (Facts.ContainsKey(fact.Type))
		{
			fact.Delay = Facts[fact.Type].Delay;
			DeleteFact(Facts[fact.Type]);
			Facts[fact.Type] = fact;
		}
		else
		{
			Facts.Add(fact.Type, fact);
		}
	}

	public void RemoveFact(E_EventTypes type)
	{
		if (Facts.ContainsKey(type))
		{
			Fact f = Facts[type];
			DeleteFact(f);
			Facts.Remove(type);
		}
	}

	public bool HaveValidFact(E_EventTypes type, float minBelief)
	{
		if (!Facts.ContainsKey(type))
		{
			return false;
		}
		if (Facts[type].Delay > 0f)
		{
			return false;
		}
		if (Facts[type].Belief < minBelief)
		{
			return false;
		}
		return true;
	}

	public Fact GetValidFact(E_EventTypes type)
	{
		if (!Facts.ContainsKey(type))
		{
			return null;
		}
		if (Facts[type].Delay > 0f)
		{
			return null;
		}
		return Facts[type];
	}

	public Fact GetFact(E_EventTypes type)
	{
		if (Facts == null || !Facts.ContainsKey(type))
		{
			return null;
		}
		return Facts[type];
	}

	public void Reset()
	{
		foreach (KeyValuePair<E_EventTypes, Fact> fact in Facts)
		{
			DeleteFact(fact.Value);
		}
		Facts.Clear();
	}

	public void Update()
	{
		foreach (KeyValuePair<E_EventTypes, Fact> fact in Facts)
		{
			fact.Value.Update();
			if (fact.Value.Belief == 0f)
			{
				RemoveFact(fact.Key);
				break;
			}
		}
	}

	private void CleanupFacts()
	{
	}

	public override string ToString()
	{
		string text = "Memory : ";
		foreach (KeyValuePair<E_EventTypes, Fact> fact in Facts)
		{
			string text2 = text;
			text = string.Concat(text2, " ", fact.Value.Type, " belief + ", fact.Value.Belief, " delay ", fact.Value.Delay);
		}
		return text;
	}
}
