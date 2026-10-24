using System.Collections.Generic;

internal class AStarGOAPMap : AStarMap
{
	private AgentHuman Ai;

	private List<GOAPAction>[] m_EffectsTable = new List<GOAPAction>[19];

	private List<GOAPAction> m_Neighbours = new List<GOAPAction>();

	public void Initialise(AgentHuman ai)
	{
		Ai = ai;
		m_Neighbours.Clear();
	}

	public void BuildActionsEffectsTable()
	{
		for (E_GOAPAction e_GOAPAction = E_GOAPAction.AdvanceToCoveredTarget; e_GOAPAction < E_GOAPAction.Count; e_GOAPAction++)
		{
			GOAPAction action = Ai.GetAction(e_GOAPAction);
			if (action == null)
			{
				continue;
			}
			for (uint num = 0u; num < 19; num++)
			{
				if (m_EffectsTable[num] == null)
				{
					m_EffectsTable[num] = new List<GOAPAction>();
				}
				if (action.WorldEffects.IsWSPropertySet((E_PropKey)num))
				{
					m_EffectsTable[num].Add(action);
				}
			}
		}
	}

	public override int GetNumAStarNeighbours(AStarNode aStarNode)
	{
		if (aStarNode == null)
		{
			return 0;
		}
		m_Neighbours.Clear();
		AStarGOAPNode aStarGOAPNode = (AStarGOAPNode)aStarNode;
		for (E_PropKey e_PropKey = E_PropKey.Start; e_PropKey < E_PropKey.Count; e_PropKey++)
		{
			if (!aStarGOAPNode.CurrentState.IsWSPropertySet(e_PropKey) || !aStarGOAPNode.GoalState.IsWSPropertySet(e_PropKey))
			{
				continue;
			}
			WorldStateProp wSProperty = aStarGOAPNode.CurrentState.GetWSProperty(e_PropKey);
			WorldStateProp wSProperty2 = aStarGOAPNode.GoalState.GetWSProperty(e_PropKey);
			if (!(wSProperty != null) || !(wSProperty2 != null) || wSProperty == wSProperty2)
			{
				continue;
			}
			for (int i = 0; i < m_EffectsTable[(int)e_PropKey].Count; i++)
			{
				GOAPAction gOAPAction = m_EffectsTable[(int)e_PropKey][i];
				if (gOAPAction.ValidateContextPreconditions(aStarGOAPNode.CurrentState, true))
				{
					m_Neighbours.Add(gOAPAction);
				}
			}
		}
		PrecedenceComparer comparer = new PrecedenceComparer();
		m_Neighbours.Sort(comparer);
		return m_Neighbours.Count;
	}

	public override short GetAStarNeighbour(AStarNode AStarNode, short neighbourCount)
	{
		return (short)m_Neighbours[neighbourCount].Type;
	}

	public override AStarNode CreateANode(short id)
	{
		AStarGOAPNode aStarGOAPNode = new AStarGOAPNode();
		aStarGOAPNode.NodeID = id;
		return aStarGOAPNode;
	}

	public override AStarNode.E_AStarFlags GetAStarFlags(short node)
	{
		return AStarNode.E_AStarFlags.Unchecked;
	}

	public override bool CompareNodes(AStarNode node1, AStarNode node2)
	{
		GOAPAction action = GetAction(node1.NodeID);
		GOAPAction action2 = GetAction(node2.NodeID);
		return action.Precedence > action2.Precedence;
	}

	public GOAPAction GetAction(short nodeID)
	{
		return Ai.GetAction((E_GOAPAction)nodeID);
	}

	public override void Cleanup()
	{
	}
}
