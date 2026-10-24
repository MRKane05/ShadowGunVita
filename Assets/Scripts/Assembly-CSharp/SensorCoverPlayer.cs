public class SensorCoverPlayer : SensorBase
{
	public SensorCoverPlayer(AgentHuman owner)
		: base(owner)
	{
	}

	public override void Update()
	{
		Cover coverForPlayer = Mission.Instance.CurrentGameZone.GetCoverForPlayer(base.Owner.Position, base.Owner.Forward, 0.85f);
		base.Owner.BlackBoard.Desires.CoverNear.Cover = coverForPlayer;
	}

	public override void Reset()
	{
	}
}
