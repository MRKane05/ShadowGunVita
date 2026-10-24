using UnityEngine;

public class SensorCoverAI : SensorBase
{
	private enum E_Test
	{
		Near = 0,
		Advance = 1,
		Fallback = 2,
		FlankLeft = 3,
		FlankRight = 4
	}

	private E_Test TestingCover;

	public SensorCoverAI(AgentHuman owner)
		: base(owner)
	{
	}

	public override void Reset()
	{
		TestingCover = E_Test.Near;
		base.Owner.BlackBoard.Desires.CoverAdvance.Reset();
		base.Owner.BlackBoard.Desires.CoverFallback.Reset();
		base.Owner.BlackBoard.Desires.CoverFlankLeft.Reset();
		base.Owner.BlackBoard.Desires.CoverFlankRight.Reset();
		base.Owner.BlackBoard.Desires.CoverNear.Reset();
	}

	public override void Update()
	{
		AgentHuman agentHuman = base.Owner.BlackBoard.VisibleTarget;
		if (agentHuman == null)
		{
			agentHuman = base.Owner.BlackBoard.DangerousEnemy;
		}
		if (agentHuman == null)
		{
			base.Owner.BlackBoard.Desires.CoverNear.Reset();
			base.Owner.BlackBoard.Desires.CoverAdvance.Reset();
			base.Owner.BlackBoard.Desires.CoverFallback.Reset();
			return;
		}
		switch (TestingCover)
		{
		case E_Test.Near:
			UpdateNear(agentHuman);
			TestingCover++;
			break;
		case E_Test.Advance:
			UpdateAdvance(agentHuman);
			TestingCover++;
			break;
		case E_Test.Fallback:
			UpdateFallback(agentHuman);
			TestingCover++;
			break;
		case E_Test.FlankLeft:
			UpdateFlankLeft(agentHuman);
			TestingCover++;
			break;
		case E_Test.FlankRight:
			UpdateFlankRight(agentHuman);
			TestingCover = E_Test.Near;
			break;
		}
	}

	private void UpdateNear(AgentHuman enemy)
	{
		base.Owner.BlackBoard.Desires.CoverNear.Reset();
		Cover coverAgainst = Mission.Instance.CurrentGameZone.GetCoverAgainst(base.Owner, enemy, base.Owner.Forward, 15f);
		if (!(coverAgainst == null))
		{
			SetValidity(coverAgainst, base.Owner.BlackBoard.Desires.CoverNear, enemy);
		}
	}

	private void UpdateAdvance(AgentHuman enemy)
	{
		base.Owner.BlackBoard.Desires.CoverAdvance.Reset();
		Cover coverForAdvance = Mission.Instance.CurrentGameZone.GetCoverForAdvance(base.Owner, enemy, 3f, base.Owner.BlackBoard.CombatRange * 0.4f);
		if (!(coverForAdvance == null))
		{
			SetValidity(coverForAdvance, base.Owner.BlackBoard.Desires.CoverAdvance, enemy);
		}
	}

	private void UpdateFallback(AgentHuman enemy)
	{
		base.Owner.BlackBoard.Desires.CoverFallback.Reset();
		Cover coverForFallback = Mission.Instance.CurrentGameZone.GetCoverForFallback(base.Owner, enemy, 3f, base.Owner.BlackBoard.WeaponRange);
		if (!(coverForFallback == null))
		{
			SetValidity(coverForFallback, base.Owner.BlackBoard.Desires.CoverFallback, enemy);
		}
	}

	private void UpdateFlankLeft(AgentHuman enemy)
	{
		base.Owner.BlackBoard.Desires.CoverFlankLeft.Reset();
		Cover coverForFlank = Mission.Instance.CurrentGameZone.GetCoverForFlank(base.Owner, true, enemy, base.Owner.BlackBoard.CombatRange * 0.5f, base.Owner.BlackBoard.WeaponRange * 0.8f);
		if (!(coverForFlank == null))
		{
			SetValidity(coverForFlank, base.Owner.BlackBoard.Desires.CoverFlankLeft, enemy);
		}
	}

	private void UpdateFlankRight(AgentHuman enemy)
	{
		base.Owner.BlackBoard.Desires.CoverFlankRight.Reset();
		Cover coverForFlank = Mission.Instance.CurrentGameZone.GetCoverForFlank(base.Owner, false, enemy, base.Owner.BlackBoard.CombatRange * 0.5f, base.Owner.BlackBoard.WeaponRange * 0.8f);
		if (!(coverForFlank == null))
		{
			SetValidity(coverForFlank, base.Owner.BlackBoard.Desires.CoverFlankRight, enemy);
		}
	}

	private void SetValidity(Cover c, CoverAIInfo info)
	{
		info.Cover = c;
		if (c.IsRightAllowed && (c.RightAgent == null || c.RightAgent == base.Owner))
		{
			info.RightCoverValidity = 0.9f;
		}
		if (c.IsLeftAllowed && (c.LeftAgent == null || c.LeftAgent == base.Owner))
		{
			info.LeftCoverValidity = 0.9f;
		}
		if (c.IsMiddleAllowed && (c.MiddleAgent == null || c.MiddleAgent == base.Owner))
		{
			info.MiddleCoverValidity = 1f;
		}
		if (info.MiddleCoverValidity == 0f && info.LeftCoverValidity == 0f && info.RightCoverValidity == 0f)
		{
			info.Cover = null;
		}
	}

	private void SetValidity(Cover c, CoverAIInfo info, AgentHuman enemy)
	{
		info.Cover = c;
		if (c.IsRightAllowed && (c.RightAgent == null || c.RightAgent == base.Owner))
		{
			Vector3 rhs = enemy.Position - c.RightEdge;
			rhs.Normalize();
			info.RightCoverValidity = Vector3.Dot(c.Right, rhs);
			if (info.RightCoverValidity < -0.3f)
			{
				info.RightCoverValidity = 0f;
			}
			else if (info.RightCoverValidity > 1f)
			{
				info.RightCoverValidity = 0f;
			}
			else
			{
				info.RightCoverValidity = (info.RightCoverValidity + 0.3f) / 1.3f;
				info.RightCoverValidity = 1f - Mathf.Abs(info.RightCoverValidity - 0.5f) * 2f;
			}
		}
		if (c.IsLeftAllowed && (c.LeftAgent == null || c.LeftAgent == base.Owner))
		{
			Vector3 rhs2 = enemy.Position - c.LeftEdge;
			rhs2.Normalize();
			info.LeftCoverValidity = Vector3.Dot(c.Left, rhs2);
			if (info.LeftCoverValidity < -0.3f)
			{
				info.LeftCoverValidity = 0f;
			}
			else if (info.LeftCoverValidity > 1f)
			{
				info.LeftCoverValidity = 0f;
			}
			else
			{
				info.LeftCoverValidity = (info.LeftCoverValidity + 0.3f) / 1.3f;
				info.LeftCoverValidity = 1f - Mathf.Abs(info.LeftCoverValidity - 0.5f) * 2f;
			}
		}
		if (c.IsMiddleAllowed && (c.MiddleAgent == null || c.MiddleAgent == base.Owner))
		{
			Vector3 rhs3 = enemy.Position - c.Transform.position;
			rhs3.Normalize();
			info.MiddleCoverValidity = Vector3.Dot(c.Right, rhs3);
			if (info.MiddleCoverValidity < -0.8f)
			{
				info.MiddleCoverValidity = 0f;
			}
			else if (info.MiddleCoverValidity > 0.8f)
			{
				info.MiddleCoverValidity = 0f;
			}
			else
			{
				info.MiddleCoverValidity = (info.MiddleCoverValidity + 0.8f) / 1.6f;
				info.MiddleCoverValidity = 1f - Mathf.Abs(info.MiddleCoverValidity - 0.5f) * 2f;
			}
			info.MiddleCoverValidity *= 1.3f;
		}
		if (info.MiddleCoverValidity == 0f && info.LeftCoverValidity == 0f && info.RightCoverValidity == 0f)
		{
			info.Cover = null;
		}
	}

	public override void DebugDraw()
	{
		if ((bool)base.Owner.BlackBoard.Desires.CoverNear.Cover)
		{
			if (base.Owner.BlackBoard.Desires.CoverNear.LeftCoverValidity > 0f)
			{
				Debug.DrawLine(base.Owner.Position, base.Owner.BlackBoard.Desires.CoverNear.Cover.LeftEdge, Color.yellow);
			}
			if (base.Owner.BlackBoard.Desires.CoverNear.RightCoverValidity > 0f)
			{
				Debug.DrawLine(base.Owner.Position, base.Owner.BlackBoard.Desires.CoverNear.Cover.RightEdge, Color.yellow);
			}
			if (base.Owner.BlackBoard.Desires.CoverNear.MiddleCoverValidity > 0f)
			{
				Debug.DrawLine(base.Owner.Position, base.Owner.BlackBoard.Desires.CoverNear.Cover.Position, Color.yellow);
			}
		}
		if ((bool)base.Owner.BlackBoard.Desires.CoverAdvance.Cover)
		{
			if (base.Owner.BlackBoard.Desires.CoverAdvance.LeftCoverValidity > 0f)
			{
				Debug.DrawLine(base.Owner.Position, base.Owner.BlackBoard.Desires.CoverAdvance.Cover.LeftEdge, Color.red);
			}
			if (base.Owner.BlackBoard.Desires.CoverAdvance.RightCoverValidity > 0f)
			{
				Debug.DrawLine(base.Owner.Position, base.Owner.BlackBoard.Desires.CoverAdvance.Cover.RightEdge, Color.red);
			}
			if (base.Owner.BlackBoard.Desires.CoverAdvance.MiddleCoverValidity > 0f)
			{
				Debug.DrawLine(base.Owner.Position, base.Owner.BlackBoard.Desires.CoverAdvance.Cover.Position, Color.red);
			}
		}
		if ((bool)base.Owner.BlackBoard.Desires.CoverFallback.Cover)
		{
			if (base.Owner.BlackBoard.Desires.CoverFallback.LeftCoverValidity > 0f)
			{
				Debug.DrawLine(base.Owner.Position, base.Owner.BlackBoard.Desires.CoverFallback.Cover.LeftEdge, Color.grey);
			}
			if (base.Owner.BlackBoard.Desires.CoverFallback.RightCoverValidity > 0f)
			{
				Debug.DrawLine(base.Owner.Position, base.Owner.BlackBoard.Desires.CoverFallback.Cover.RightEdge, Color.grey);
			}
			if (base.Owner.BlackBoard.Desires.CoverFallback.MiddleCoverValidity > 0f)
			{
				Debug.DrawLine(base.Owner.Position, base.Owner.BlackBoard.Desires.CoverFallback.Cover.Position, Color.gray);
			}
		}
	}
}
