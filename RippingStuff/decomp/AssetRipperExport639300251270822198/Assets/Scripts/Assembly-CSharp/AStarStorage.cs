internal class AStarStorage
{
	private AStarNode headOfOpenList;

	private AStarNode headOfClosedList;

	private ushort openSize;

	public void AddToOpenList(AStarNode node, AStarMap map)
	{
		AStarNode aStarNode = headOfOpenList;
		AStarNode aStarNode2 = null;
		AStarNode aStarNode3 = null;
		if (map.GetAStarFlags(node.NodeID) == AStarNode.E_AStarFlags.Open)
		{
			return;
		}
		if (aStarNode == null)
		{
			headOfOpenList = node;
			openSize = 1;
			map.SetAStarFlags(node.NodeID, AStarNode.E_AStarFlags.Open);
			node.Flag = AStarNode.E_AStarFlags.Open;
			return;
		}
		bool flag = false;
		while (!flag)
		{
			aStarNode3 = aStarNode.Next;
			aStarNode2 = aStarNode.Previous;
			if (aStarNode.F > node.F)
			{
				if (aStarNode2 == null)
				{
					aStarNode.Previous = node;
					node.Next = aStarNode;
					node.Previous = null;
					headOfOpenList = node;
					node.Flag = AStarNode.E_AStarFlags.Open;
					map.SetAStarFlags(node.NodeID, AStarNode.E_AStarFlags.Open);
					flag = true;
					openSize++;
				}
				else
				{
					aStarNode2.Next = node;
					node.Previous = aStarNode2;
					node.Next = aStarNode;
					aStarNode.Previous = node;
					map.SetAStarFlags(node.NodeID, AStarNode.E_AStarFlags.Open);
					node.Flag = AStarNode.E_AStarFlags.Open;
					flag = true;
					openSize++;
				}
			}
			else if (aStarNode.F == node.F)
			{
				if (map.CompareNodes(aStarNode, node))
				{
					if (aStarNode2 != null)
					{
						aStarNode2.Next = node;
						aStarNode.Previous = node;
						node.Previous = aStarNode2;
						node.Next = aStarNode;
						map.SetAStarFlags(node.NodeID, AStarNode.E_AStarFlags.Open);
						node.Flag = AStarNode.E_AStarFlags.Open;
						flag = true;
						openSize++;
					}
					else if (aStarNode2 == null)
					{
						node.Previous = null;
						node.Next = aStarNode;
						aStarNode.Previous = node;
						map.SetAStarFlags(node.NodeID, AStarNode.E_AStarFlags.Open);
						node.Flag = AStarNode.E_AStarFlags.Open;
						flag = true;
						openSize++;
						headOfOpenList = node;
					}
				}
				else if (aStarNode3 == null)
				{
					aStarNode.Next = node;
					node.Previous = aStarNode;
					node.Next = null;
					map.SetAStarFlags(node.NodeID, AStarNode.E_AStarFlags.Open);
					node.Flag = AStarNode.E_AStarFlags.Open;
					flag = true;
					openSize++;
				}
				else
				{
					aStarNode = aStarNode3;
				}
			}
			else if (aStarNode3 == null)
			{
				aStarNode.Next = node;
				node.Previous = aStarNode;
				node.Next = null;
				map.SetAStarFlags(node.NodeID, AStarNode.E_AStarFlags.Open);
				node.Flag = AStarNode.E_AStarFlags.Open;
				flag = true;
				openSize++;
			}
			else
			{
				aStarNode = aStarNode3;
			}
		}
	}

	public void AddToClosedList(AStarNode node, AStarMap map)
	{
		AStarNode aStarNode = headOfClosedList;
		if (aStarNode != null)
		{
			node.Next = aStarNode;
			node.Previous = null;
			aStarNode.Previous = node;
			headOfClosedList = node;
			map.SetAStarFlags(node.NodeID, AStarNode.E_AStarFlags.Closed);
			node.Flag = AStarNode.E_AStarFlags.Closed;
		}
		else
		{
			headOfClosedList = node;
			node.Next = null;
			node.Previous = null;
			map.SetAStarFlags(node.NodeID, AStarNode.E_AStarFlags.Closed);
			node.Flag = AStarNode.E_AStarFlags.Closed;
		}
	}

	public AStarNode FindInOpenList(short node)
	{
		for (AStarNode next = headOfOpenList; next != null; next = next.Next)
		{
			if (next.NodeID == node)
			{
				return next;
			}
		}
		return null;
	}

	public AStarNode FindInClosedList(short node)
	{
		for (AStarNode next = headOfClosedList; next != null; next = next.Next)
		{
			if (next.NodeID == node)
			{
				return next;
			}
		}
		return null;
	}

	public void RemoveFromClosedList(short nodeId, AStarMap map)
	{
		AStarNode aStarNode = FindInClosedList(nodeId);
		if (aStarNode != null)
		{
			AStarNode next = aStarNode.Next;
			AStarNode previous = aStarNode.Previous;
			if (next != null)
			{
				next.Previous = previous;
			}
			if (previous != null)
			{
				previous.Next = next;
			}
			if (headOfClosedList == aStarNode)
			{
				headOfClosedList = next;
			}
			aStarNode.Next = null;
			aStarNode.Previous = null;
			aStarNode.Flag = AStarNode.E_AStarFlags.Unchecked;
			map.SetAStarFlags(aStarNode.NodeID, AStarNode.E_AStarFlags.Unchecked);
		}
	}

	public void RemoveFromOpenList(short nodeId, AStarMap map)
	{
		AStarNode aStarNode = FindInOpenList(nodeId);
		if (aStarNode != null)
		{
			AStarNode next = aStarNode.Next;
			AStarNode previous = aStarNode.Previous;
			if (next != null)
			{
				next.Previous = previous;
			}
			if (previous != null)
			{
				previous.Next = next;
			}
			if (headOfOpenList == aStarNode)
			{
				headOfOpenList = next;
			}
			aStarNode.Next = null;
			aStarNode.Previous = null;
			aStarNode.Flag = AStarNode.E_AStarFlags.Unchecked;
			map.SetAStarFlags(aStarNode.NodeID, AStarNode.E_AStarFlags.Unchecked);
			openSize--;
		}
	}

	public AStarNode RemoveCheapestOpenNode(AStarMap map)
	{
		if (openSize == 0)
		{
			return null;
		}
		AStarNode aStarNode = headOfOpenList;
		if (openSize == 1)
		{
			headOfOpenList.Next = null;
			headOfOpenList.Previous = null;
			headOfOpenList = null;
			aStarNode.Flag = AStarNode.E_AStarFlags.Unchecked;
			map.SetAStarFlags(aStarNode.NodeID, AStarNode.E_AStarFlags.Unchecked);
			openSize--;
		}
		else if (openSize > 1)
		{
			AStarNode aStarNode2 = headOfOpenList;
			AStarNode next = headOfOpenList.Next;
			aStarNode2.Next = null;
			aStarNode2.Previous = null;
			aStarNode2.Flag = AStarNode.E_AStarFlags.Unchecked;
			map.SetAStarFlags(aStarNode2.NodeID, AStarNode.E_AStarFlags.Unchecked);
			headOfOpenList = next;
			headOfOpenList.Previous = null;
			aStarNode = aStarNode2;
			openSize--;
		}
		return aStarNode;
	}

	public bool CheckStorage()
	{
		if (headOfOpenList != null && headOfOpenList.Next == null && openSize > 1)
		{
			return true;
		}
		return false;
	}

	public void Cleanup()
	{
		headOfOpenList = null;
		headOfClosedList = null;
	}

	public void ResetStorage(AStarMap map)
	{
		AStarNode aStarNode = headOfOpenList;
		AStarNode next = headOfOpenList;
		while (next != null)
		{
			map.SetAStarFlags(next.NodeID, AStarNode.E_AStarFlags.Unchecked);
			aStarNode = next;
			next = aStarNode.Next;
			aStarNode.Flag = AStarNode.E_AStarFlags.Unchecked;
			aStarNode.Parent = null;
			aStarNode.Previous = null;
			aStarNode.Next = null;
			aStarNode = null;
		}
		aStarNode = headOfClosedList;
		next = headOfClosedList;
		while (next != null)
		{
			map.SetAStarFlags(next.NodeID, AStarNode.E_AStarFlags.Unchecked);
			aStarNode = next;
			next = aStarNode.Next;
			aStarNode.Parent = null;
			map.SetAStarFlags(aStarNode.NodeID, AStarNode.E_AStarFlags.Unchecked);
			aStarNode.Previous = null;
			aStarNode.Next = null;
			aStarNode = null;
		}
		headOfOpenList = null;
		headOfClosedList = null;
		openSize = 0;
	}
}
