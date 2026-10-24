internal class AStarGOAPGoal : AStarGoal
{
	private GOAPGoal Goal;

	private AStarGOAPMap Map;

	public void Initialise(AgentHuman ai, AStarGOAPMap map, GOAPGoal goal)
	{
		Map = map;
		Goal = goal;
	}

	public override float GetHeuristicDistance(AgentHuman ai, AStarNode aStarNode, bool firstRun)
	{
		AStarGOAPNode aStarGOAPNode = (AStarGOAPNode)aStarNode;
		if (firstRun)
		{
			aStarGOAPNode.GoalState.Reset();
			Goal.SetWSSatisfactionForPlanning(aStarGOAPNode.GoalState);
			Goal.ChangeCurrentWSForPlanning(aStarGOAPNode.CurrentState);
		}
		else
		{
			GOAPAction action = Map.GetAction(aStarNode.NodeID);
			action.SolvePlanWSVariable(aStarGOAPNode.CurrentState, aStarGOAPNode.GoalState);
			action.SetPlanWSPreconditions(aStarGOAPNode.GoalState);
		}
		MergeStates(ai, aStarGOAPNode.CurrentState, aStarGOAPNode.GoalState);
		return aStarGOAPNode.GoalState.GetNumWorldStateDifferences(aStarGOAPNode.CurrentState);
	}

	public override float GetActualCost(AStarNode nodeOne, AStarNode nodeTwo)
	{
		AStarGOAPNode aStarGOAPNode = (AStarGOAPNode)nodeOne;
		AStarGOAPNode aStarGOAPNode2 = (AStarGOAPNode)nodeTwo;
		aStarGOAPNode2.CurrentState.CopyWorldState(aStarGOAPNode.CurrentState);
		aStarGOAPNode2.GoalState.CopyWorldState(aStarGOAPNode.GoalState);
		GOAPAction action = Map.GetAction(nodeTwo.NodeID);
		if (action != null)
		{
			return action.Cost;
		}
		return float.MaxValue;
	}

	public override bool IsAStarFinished(AStarNode currNode)
	{
		if (currNode == null)
		{
			return true;
		}
		AStarGOAPNode currentNode = (AStarGOAPNode)currNode;
		if (IsPlanValid(currentNode))
		{
			return true;
		}
		return false;
	}

	public override bool IsAStarNodePassable(int node)
	{
		return true;
	}

	public bool IsPlanValid(AStarGOAPNode currentNode)
	{
		if (currentNode.GoalState.GetNumWorldStateDifferences(currentNode.CurrentState) == 0)
		{
			WorldState worldState = new WorldState();
			worldState.CopyWorldState(currentNode.GoalState);
			AStarGOAPNode aStarGOAPNode = currentNode;
			while (aStarGOAPNode.NodeID != -1)
			{
				GOAPAction action = Map.GetAction(aStarGOAPNode.NodeID);
				if (!action.ValidateWSPreconditions(worldState, null))
				{
					return false;
				}
				if (!action.ValidateContextPreconditions(worldState, true))
				{
					return false;
				}
				action.ApplyWSEffects(worldState, null);
				aStarGOAPNode = (AStarGOAPNode)aStarGOAPNode.Parent;
			}
			if (Goal.IsWSSatisfiedForPlanning(worldState))
			{
				return true;
			}
		}
		return false;
	}

	public void MergeStates(AgentHuman ai, WorldState currentState, WorldState goalState)
	{
		for (E_PropKey e_PropKey = E_PropKey.Start; e_PropKey < E_PropKey.Count; e_PropKey++)
		{
			if (goalState.IsWSPropertySet(e_PropKey) && !currentState.IsWSPropertySet(e_PropKey))
			{
				WorldStateProp wSProperty = ai.WorldState.GetWSProperty(e_PropKey);
				currentState.SetWSProperty(wSProperty);
			}
		}
	}

	public override void SetDestNode(AStarNode destNode)
	{
	}

	public override void Cleanup()
	{
	}
}
