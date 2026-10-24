internal class AStarEngine
{
	private AStarGoal Goal;

	private AStarMap Map;

	private AStarStorage Storage;

	public AStarNode CurrentNode;

	public short Start;

	public short End;

	public void Setup(AStarGoal _goal, AStarStorage _storage, AStarMap _aStarMap)
	{
		Goal = _goal;
		Storage = _storage;
		Map = _aStarMap;
		Storage.ResetStorage(Map);
	}

	public void RunAStar(AgentHuman ai)
	{
		int num = 0;
		CurrentNode = Map.CreateANode(End);
		Storage.AddToOpenList(CurrentNode, Map);
		float heuristicDistance = Goal.GetHeuristicDistance(ai, CurrentNode, true);
		CurrentNode.G = 0f;
		CurrentNode.H = heuristicDistance;
		CurrentNode.F = heuristicDistance;
		while (true)
		{
			CurrentNode = Storage.RemoveCheapestOpenNode(Map);
			if (CurrentNode == null)
			{
				break;
			}
			Storage.AddToClosedList(CurrentNode, Map);
			if (Goal.IsAStarFinished(CurrentNode))
			{
				break;
			}
			num = Map.GetNumAStarNeighbours(CurrentNode);
			for (short num2 = 0; num2 < num; num2++)
			{
				short aStarNeighbour = Map.GetAStarNeighbour(CurrentNode, num2);
				if (aStarNeighbour == -1)
				{
					break;
				}
				AStarNode.E_AStarFlags aStarFlags = Map.GetAStarFlags(aStarNeighbour);
				AStarNode aStarNode;
				switch (aStarFlags)
				{
				case AStarNode.E_AStarFlags.Open:
					aStarNode = Storage.FindInOpenList(aStarNeighbour);
					break;
				case AStarNode.E_AStarFlags.Closed:
					aStarNode = Storage.FindInClosedList(aStarNeighbour);
					break;
				default:
					if (Goal.IsAStarNodePassable(aStarNeighbour))
					{
						aStarNode = Map.CreateANode(aStarNeighbour);
						break;
					}
					continue;
				}
				if (aStarNode == null || CurrentNode.Parent == aStarNode)
				{
					continue;
				}
				float num3 = CurrentNode.G + Goal.GetActualCost(CurrentNode, aStarNode);
				heuristicDistance = Goal.GetHeuristicDistance(ai, aStarNode, false);
				float num4 = num3 + heuristicDistance;
				if (!(num4 >= aStarNode.F))
				{
					aStarNode.F = num4;
					aStarNode.G = num3;
					aStarNode.H = heuristicDistance;
					if (aStarFlags == AStarNode.E_AStarFlags.Closed)
					{
						Storage.RemoveFromClosedList(aStarNode.NodeID, Map);
					}
					Storage.AddToOpenList(aStarNode, Map);
					aStarNode.Parent = CurrentNode;
				}
			}
		}
	}

	public void Cleanup()
	{
		Goal = null;
		Map = null;
		Storage = null;
		CurrentNode = null;
		Start = 0;
		End = 0;
	}
}
