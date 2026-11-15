internal class AStarGOAPNode : AStarNode
{
	public WorldState CurrentState;

	public WorldState GoalState;

	public AStarGOAPNode()
	{
		CurrentState = new WorldState();
		GoalState = new WorldState();
	}
}
