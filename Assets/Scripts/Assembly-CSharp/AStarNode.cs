internal class AStarNode
{
	public enum E_AStarFlags
	{
		Unchecked = 0,
		Open = 1,
		Closed = 2,
		NotPassable = 3
	}

	public short NodeID;

	public float G;

	public float H;

	public float F;

	public AStarNode Next;

	public AStarNode Previous;

	public AStarNode Parent;

	public E_AStarFlags Flag;

	public AStarNode()
	{
		NodeID = -1;
		G = 0f;
		H = 0f;
		F = float.MaxValue;
		Flag = E_AStarFlags.Unchecked;
	}
}
