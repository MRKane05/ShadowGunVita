using UnityEngine;
using UnityEngine.AI;

internal class GOAPGoalSpawnAction : GOAPGoal
{
	public GOAPGoalSpawnAction(AgentHuman owner)
		: base(E_GOAPGoals.SpawnAction, owner)
	{
	}

	public override void InitGoal()
	{
		base.FailChance = 0;
	}

	public override float GetMaxRelevancy()
	{
		return 0.1f;
	}

	public override void CalculateGoalRelevancy()
	{
		base.GoalRelevancy = 0f;
		if (base.Owner.WorldState.GetWSProperty(E_PropKey.DoSpawnAction).GetBool())
		{
			base.GoalRelevancy = GetMaxRelevancy();
		}
	}

	public override void SetDisableTime()
	{
		base.NextEvaluationTime = 1000f + Time.timeSinceLevelLoad;
	}

	public override void ChangeCurrentWSForPlanning(WorldState worldState)
	{
		SpawnPointEnemy spawnPointEnemy = base.Owner.BlackBoard.SpawnPointEnemy;
		switch (spawnPointEnemy.InitState)
		{
		case SpawnPointEnemy.E_InitState.Goto:
			worldState.SetWSProperty(E_PropKey.AtTargetPos, false);
			worldState.SetWSProperty(E_PropKey.TargetNode, spawnPointEnemy.GotoPosition);
			break;
		case SpawnPointEnemy.E_InitState.Ambush:
			worldState.SetWSProperty(E_PropKey.AtTargetPos, false);
			worldState.SetWSProperty(E_PropKey.TargetNode, spawnPointEnemy.GotoPosition);
			break;
		case SpawnPointEnemy.E_InitState.Patrol:
			worldState.SetWSProperty(E_PropKey.Patrol, true);
			break;
		case SpawnPointEnemy.E_InitState.Cover:
			worldState.SetWSProperty(E_PropKey.AtTargetPos, false);
			worldState.SetWSProperty(E_PropKey.TargetNode, spawnPointEnemy.InitCover.MiddleEntryPos);
			break;
		case SpawnPointEnemy.E_InitState.PlayAnim:
			worldState.SetWSProperty(E_PropKey.PlayAnim, true);
			break;
		}
	}

	public override void SetWSSatisfactionForPlanning(WorldState worldState)
	{
		SpawnPointEnemy spawnPointEnemy = base.Owner.BlackBoard.SpawnPointEnemy;
		switch (spawnPointEnemy.InitState)
		{
		case SpawnPointEnemy.E_InitState.Goto:
			worldState.SetWSProperty(E_PropKey.AtTargetPos, true);
			worldState.SetWSProperty(E_PropKey.TargetNode, spawnPointEnemy.GotoPosition);
			break;
		case SpawnPointEnemy.E_InitState.Ambush:
			worldState.SetWSProperty(E_PropKey.AtTargetPos, true);
			worldState.SetWSProperty(E_PropKey.TargetNode, spawnPointEnemy.GotoPosition);
			break;
		case SpawnPointEnemy.E_InitState.Patrol:
			worldState.SetWSProperty(E_PropKey.Patrol, false);
			break;
		case SpawnPointEnemy.E_InitState.Cover:
			worldState.SetWSProperty(E_PropKey.AtTargetPos, false);
			worldState.SetWSProperty(E_PropKey.CoverState, E_CoverState.Middle);
			worldState.SetWSProperty(E_PropKey.TargetNode, spawnPointEnemy.InitCover.MiddleEntryPos);
			break;
		case SpawnPointEnemy.E_InitState.PlayAnim:
			worldState.SetWSProperty(E_PropKey.PlayAnim, false);
			break;
		}
	}

	public override bool IsWSSatisfiedForPlanning(WorldState worldState)
	{
		SpawnPointEnemy spawnPointEnemy = base.Owner.BlackBoard.SpawnPointEnemy;
		switch (spawnPointEnemy.InitState)
		{
		case SpawnPointEnemy.E_InitState.Ambush:
			return worldState.GetWSProperty(E_PropKey.AtTargetPos).GetBool();
		case SpawnPointEnemy.E_InitState.Goto:
			return worldState.GetWSProperty(E_PropKey.AtTargetPos).GetBool();
		case SpawnPointEnemy.E_InitState.Patrol:
			return !worldState.GetWSProperty(E_PropKey.Patrol).GetBool();
		case SpawnPointEnemy.E_InitState.Cover:
			return worldState.GetWSProperty(E_PropKey.CoverState).GetCoverState() == E_CoverState.Middle;
		case SpawnPointEnemy.E_InitState.PlayAnim:
			return !worldState.GetWSProperty(E_PropKey.PlayAnim).GetBool();
		default:
			return false;
		}
	}

	public override bool IsSatisfied()
	{
		return IsPlanFinished();
	}

	public override bool Activate(GOAPPlan plan)
	{
		SpawnPointEnemy spawnPointEnemy = base.Owner.BlackBoard.SpawnPointEnemy;
		base.Owner.BlackBoard.Desires.LookAtTarget = false;
		switch (spawnPointEnemy.InitState)
		{
		case SpawnPointEnemy.E_InitState.Goto:
			base.Owner.WorldState.SetWSProperty(E_PropKey.AtTargetPos, false);
			base.Owner.WorldState.SetWSProperty(E_PropKey.TargetNode, spawnPointEnemy.GotoPosition);
			base.GoalRelevancy = 1f;
			break;
		case SpawnPointEnemy.E_InitState.Ambush:
			base.Owner.WorldState.SetWSProperty(E_PropKey.AtTargetPos, false);
			base.Owner.WorldState.SetWSProperty(E_PropKey.TargetNode, spawnPointEnemy.GotoPosition);
			break;
		case SpawnPointEnemy.E_InitState.Patrol:
			base.Owner.WorldState.SetWSProperty(E_PropKey.Patrol, true);
			break;
		case SpawnPointEnemy.E_InitState.Cover:
			InitializeCoverAction(spawnPointEnemy.InitCover);
			break;
		case SpawnPointEnemy.E_InitState.PlayAnim:
			base.Owner.WorldState.SetWSProperty(E_PropKey.PlayAnim, true);
			base.Owner.BlackBoard.Desires.Animation = spawnPointEnemy.SpawnAnimation.name;
			base.Owner.Animation.Stop();
			spawnPointEnemy.SpawnAnimation.SampleAnimation(base.Owner.GameObject, 0f);
			if (spawnPointEnemy.SpawnAnimation.wrapMode != WrapMode.Loop)
			{
				base.GoalRelevancy = 1f;
			}
			break;
		}
		base.Owner.BlackBoard.SetFear(0f);
		base.Owner.BlackBoard.SetRage(0f);
		return base.Activate(plan);
	}

	public override void Deactivate()
	{
		base.Owner.BlackBoard.Desires.LookAtTarget = true;
		base.Owner.WorldState.SetWSProperty(E_PropKey.DoSpawnAction, false);
		base.Owner.NavMeshAgent.walkableMask &= ~(1 << NavMesh.GetNavMeshLayerFromName("WalkOnlyWhenSpawn"));
		SpawnPointEnemy spawnPointEnemy = base.Owner.BlackBoard.SpawnPointEnemy;
		if (spawnPointEnemy.InitState == SpawnPointEnemy.E_InitState.Goto && base.Owner.BlackBoard.DangerousEnemy == null)
		{
			base.Owner.BlackBoard.Desires.Rotation = spawnPointEnemy.InitPosition.transform.rotation;
		}
		base.Owner.WeaponComponent.GetCurrentWeapon().SetBusy(0.4f);
		base.Owner.BlackBoard.SightRange = base.Owner.BlackBoard.BaseSetup.SightRangeInCombat;
		base.Owner.BlackBoard.SightFov = base.Owner.BlackBoard.BaseSetup.SightFovInCombat;
		base.Deactivate();
	}

	private void InitializeCoverAction(Cover c)
	{
		E_CoverDirection coverPosition;
		Vector3 value;
		if (c.IsMiddleAllowed)
		{
			coverPosition = E_CoverDirection.Middle;
			value = c.Transform.position + c.Forward * -1f;
		}
		else if (c.IsLeftAllowed)
		{
			coverPosition = E_CoverDirection.Left;
			value = c.LeftEdge + c.Forward * -0.7f;
		}
		else
		{
			coverPosition = E_CoverDirection.Right;
			value = c.RightEdge + c.Forward * -0.7f;
		}
		base.Owner.WorldState.SetWSProperty(E_PropKey.CoverState, E_CoverState.Middle);
		base.Owner.WorldState.SetWSProperty(E_PropKey.AtTargetPos, false);
		base.Owner.WorldState.SetWSProperty(E_PropKey.TargetNode, value);
		base.Owner.BlackBoard.Fear = base.Owner.BlackBoard.CombatSetup.FearMax;
		base.Owner.BlackBoard.Desires.CoverSelected = c;
		base.Owner.BlackBoard.Desires.CoverPosition = coverPosition;
		base.GoalRelevancy = 1f;
	}
}
