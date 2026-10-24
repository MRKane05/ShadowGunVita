using System.Collections.Generic;

public class SensorEyesPlayer : SensorBase
{
	public float EyeRange = 6f;

	public float FieldOfView = 120f;

	private float sqrEyeRange
	{
		get
		{
			return EyeRange * EyeRange;
		}
	}

	public SensorEyesPlayer(AgentHuman owner)
		: base(owner)
	{
	}

	public override void Update()
	{
		if (!base.Owner.IsVisible)
		{
			return;
		}
		if (Mission.Instance.CurrentGameZone == null)
		{
			base.Owner.WorldState.SetWSProperty(E_PropKey.SeeEnemy, false);
			return;
		}
		List<Agent> enemies = Mission.Instance.CurrentGameZone.Enemies;
		if (base.Owner.WorldState.GetWSProperty(E_PropKey.SeeEnemy).GetBool())
		{
			if (enemies.Count == 0)
			{
				base.Owner.WorldState.SetWSProperty(E_PropKey.SeeEnemy, false);
			}
			return;
		}
		for (int i = 0; i < enemies.Count; i++)
		{
			if ((base.Owner.Position - enemies[i].Position).sqrMagnitude < sqrEyeRange)
			{
				base.Owner.WorldState.SetWSProperty(E_PropKey.SeeEnemy, true);
				return;
			}
		}
		base.Owner.WorldState.SetWSProperty(E_PropKey.SeeEnemy, false);
	}

	public override void Reset()
	{
	}
}
