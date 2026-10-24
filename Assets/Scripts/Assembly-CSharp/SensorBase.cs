public abstract class SensorBase
{
	private AgentHuman _Owner;

	public bool Active;

	public AgentHuman Owner
	{
		get
		{
			return _Owner;
		}
	}

	public SensorBase(AgentHuman owner)
	{
		_Owner = owner;
	}

	public abstract void Update();

	public abstract void Reset();

	public virtual void DebugDraw()
	{
	}
}
