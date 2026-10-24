using UnityEngine;

internal class GOAPGoalFactory
{
	public static GOAPGoal Create(E_GOAPGoals type, AgentHuman owner)
	{
		GOAPGoal gOAPGoal;
		switch (type)
		{
		case E_GOAPGoals.SpawnAction:
			gOAPGoal = new GOAPGoalSpawnAction(owner);
			break;
		case E_GOAPGoals.Advance:
			gOAPGoal = new GOAPGoalAdvance(owner);
			break;
		case E_GOAPGoals.AdvanceToCover:
			gOAPGoal = new GOAPGoalAdvanceToCover(owner);
			break;
		case E_GOAPGoals.AdvanceTargetCovered:
			gOAPGoal = new GOAPGoalAdvanceToEnemyCover(owner);
			break;
		case E_GOAPGoals.FallBack:
			gOAPGoal = new GOAPGoalFallBack(owner);
			break;
		case E_GOAPGoals.RunAway:
			gOAPGoal = new GOAPGoalRunAway(owner);
			break;
		case E_GOAPGoals.KeepCombatRange:
			gOAPGoal = new GOAPGoalKeepCombatRange(owner);
			break;
		case E_GOAPGoals.KeepCombatRangeTargetCovered:
			gOAPGoal = new GOAPGoalKeepCombatRangeTargetCovered(owner);
			break;
		case E_GOAPGoals.Move:
			gOAPGoal = new GOAPGoalMove(owner);
			break;
		case E_GOAPGoals.HideInCover:
			gOAPGoal = new GOAPGoalHideInCover(owner);
			break;
		case E_GOAPGoals.GetOutOfCover:
			gOAPGoal = new GOAPGoalGetOutOfCover(owner);
			break;
		case E_GOAPGoals.LookAtTarget:
			gOAPGoal = new GOAPGoalLookAtTarget(owner);
			break;
		case E_GOAPGoals.CheckEvent:
			gOAPGoal = new GOAPGoalCheckEvent(owner);
			break;
		case E_GOAPGoals.KillTarget:
			gOAPGoal = new GOAPGoalKillTarget(owner);
			break;
		case E_GOAPGoals.SuppressiveFire:
			gOAPGoal = new GOAPGoalSuppressiveFire(owner);
			break;
		case E_GOAPGoals.CoverEnter:
			gOAPGoal = new GOAPGoalCoverEnter(owner);
			break;
		case E_GOAPGoals.CoverLeave:
			gOAPGoal = new GOAPGoalCoverLeave(owner);
			break;
		case E_GOAPGoals.CoverFire:
			gOAPGoal = new GOAPGoalCoverFire(owner);
			break;
		case E_GOAPGoals.CoverMove:
			gOAPGoal = new GOAPGoalCoverMove(owner);
			break;
		case E_GOAPGoals.CoverJumpOver:
			gOAPGoal = new GOAPGoalCoverJumpOver(owner);
			break;
		case E_GOAPGoals.WeaponReload:
			gOAPGoal = new GOAPGoalWeaponReload(owner);
			break;
		case E_GOAPGoals.WeaponReloadAI:
			gOAPGoal = new GOAPGoalWeaponReloadAI(owner);
			break;
		case E_GOAPGoals.WeaponChange:
			gOAPGoal = new GOAPGoalWeaponChange(owner);
			break;
		case E_GOAPGoals.Dodge:
			gOAPGoal = new GOAPGoalDodge(owner);
			break;
		case E_GOAPGoals.Roll:
			gOAPGoal = new GOAPGoalRoll(owner);
			break;
		case E_GOAPGoals.UseWorldObject:
			gOAPGoal = new GOAPGoalUseWorldObject(owner);
			break;
		case E_GOAPGoals.PlayAnim:
			gOAPGoal = new GOAPGoalPlayAnim(owner);
			break;
		case E_GOAPGoals.CriticalInjury:
			gOAPGoal = new GOAPGoalCriticalInjury(owner);
			break;
		case E_GOAPGoals.Suppressed:
			gOAPGoal = new GOAPGoalSuppressed(owner);
			break;
		case E_GOAPGoals.Teleport:
			gOAPGoal = new GOAPGoalTeleport(owner);
			break;
		default:
			Debug.LogError(string.Concat("GOAPGoalFactory Unknow goal ", type, " for ", owner.name));
			return null;
		}
		gOAPGoal.InitGoal();
		return gOAPGoal;
	}
}
