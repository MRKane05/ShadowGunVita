using UnityEngine;
using UnityEngine.AI;

internal class GOAPGoalAdvance : GOAPGoal
{
	private Vector3 AdvancePos;

	private NavMeshPath Path;

	public GOAPGoalAdvance(AgentHuman owner)
		: base(E_GOAPGoals.Advance, owner)
	{
	}

	public override void InitGoal()
	{
		base.FailChance = 10;
		Path = new NavMeshPath();
	}

	public override float GetMaxRelevancy()
	{
		return base.Owner.BlackBoard.GoapSetup.AdvanceRelevancy;
	}

	public override void CalculateGoalRelevancy()
	{
		base.GoalRelevancy = 0f;
		if ((base.Owner.BlackBoard.CoverTime != 0f && base.Owner.BlackBoard.CoverTime < 7f) || !base.Owner.WorldState.GetWSProperty(E_PropKey.SeeEnemy).GetBool() || base.Owner.BlackBoard.VisibleTarget == null || base.Owner.BlackBoard.DistanceToTarget < base.Owner.BlackBoard.CombatRange * 0.8f)
		{
			return;
		}
		float num = 0f;
		if (base.Owner.BlackBoard.DistanceToTarget > base.Owner.BlackBoard.WeaponRange)
		{
			num = 1f;
		}
		else
		{
			float num2 = base.Owner.BlackBoard.DistanceToTarget - base.Owner.BlackBoard.CombatRange;
			num = num2 / (base.Owner.BlackBoard.WeaponRange - base.Owner.BlackBoard.CombatRange);
			num *= num;
			if (num < 0.5f)
			{
				return;
			}
		}
		if (NavMesh.CalculatePath(base.Owner.Position, base.Owner.BlackBoard.VisibleTarget.Position, base.Owner.NavMeshAgent.walkableMask, Path) && NavMeshUtils.IsPathValid(Path, base.Owner.BlackBoard.VisibleTarget.Position))
		{
			AdvancePos = Mathfx.GetBestPositionFromPath(base.Owner.Position, Path.corners, Path.corners.Length, 3f, 8f);
			base.GoalRelevancy = base.Owner.BlackBoard.GoapSetup.AdvanceRelevancy * num;
		}
	}

	public override void SetDisableTime()
	{
		base.NextEvaluationTime = base.Owner.BlackBoard.GoapSetup.AdvanceDelay + Time.timeSinceLevelLoad;
	}

	public override void ChangeCurrentWSForPlanning(WorldState worldState)
	{
		worldState.SetWSProperty(E_PropKey.AtTargetPos, false);
		worldState.SetWSProperty(E_PropKey.TargetNode, AdvancePos);
	}

	public override void SetWSSatisfactionForPlanning(WorldState worldState)
	{
		worldState.SetWSProperty(E_PropKey.AtTargetPos, true);
		worldState.SetWSProperty(E_PropKey.TargetNode, AdvancePos);
	}

	public override bool IsWSSatisfiedForPlanning(WorldState worldState)
	{
		WorldStateProp wSProperty = worldState.GetWSProperty(E_PropKey.AtTargetPos);
		if (wSProperty != null && wSProperty.GetBool())
		{
			return true;
		}
		return false;
	}

	public override bool IsPlanValid()
	{
		Vector3 position;
		if ((bool)base.Owner.BlackBoard.VisibleTarget)
		{
			position = base.Owner.BlackBoard.VisibleTarget.Position;
		}
		else
		{
			if (!base.Owner.BlackBoard.DangerousEnemy)
			{
				return IsPlanFinished();
			}
			position = base.Owner.BlackBoard.DangerousEnemy.Position;
		}
		if ((position - base.Owner.Position).sqrMagnitude < 9f)
		{
			return false;
		}
		if ((position - AdvancePos).sqrMagnitude < 4f)
		{
			return false;
		}
		return base.IsPlanValid();
	}

	public override bool IsSatisfied()
	{
		return IsPlanFinished();
	}

	public override bool Activate(GOAPPlan plan)
	{
		base.Owner.WorldState.SetWSProperty(E_PropKey.AtTargetPos, false);
		base.Owner.WorldState.SetWSProperty(E_PropKey.TargetNode, AdvancePos);
		base.Owner.BlackBoard.SetFear(0f);
		return base.Activate(plan);
	}

	public override void Deactivate()
	{
		base.Deactivate();
	}
}
