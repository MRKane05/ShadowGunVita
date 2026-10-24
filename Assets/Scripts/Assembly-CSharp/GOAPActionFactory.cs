using UnityEngine;

internal class GOAPActionFactory
{
	public static GOAPAction Create(E_GOAPAction type, AgentHuman owner)
	{
		GOAPAction gOAPAction;
		switch (type)
		{
		case E_GOAPAction.AdvanceToCoveredTarget:
			gOAPAction = new GOAPActionAdvanceToCoveredTarget(owner);
			break;
		case E_GOAPAction.Move:
			gOAPAction = new GOAPActionMove(owner);
			break;
		case E_GOAPAction.Goto:
			gOAPAction = new GOAPActionGoTo(owner);
			break;
		case E_GOAPAction.Fallback:
			gOAPAction = new GOAPActionFallback(owner);
			break;
		case E_GOAPAction.LookatTarget:
			gOAPAction = new GOAPActionLookAtTarget(owner);
			break;
		case E_GOAPAction.CheckEvent:
			gOAPAction = new GOAPActionCheckEvent(owner);
			break;
		case E_GOAPAction.CheckLostEnemy:
			gOAPAction = new GOAPActionCheckLostEnemy(owner);
			break;
		case E_GOAPAction.WeaponChange:
			gOAPAction = new GOAPActionWeaponChange(owner);
			break;
		case E_GOAPAction.WeaponReload:
			gOAPAction = new GOAPActionWeaponReload(owner);
			break;
		case E_GOAPAction.Use:
			gOAPAction = new GOAPActionUse(owner);
			break;
		case E_GOAPAction.PlayAnim:
			gOAPAction = new GOAPActionPlayAnim(owner);
			break;
		case E_GOAPAction.Knockdown:
			gOAPAction = new GOAPActionKnockdown(owner);
			break;
		case E_GOAPAction.Teleport:
			gOAPAction = new GOAPActionTeleport(owner);
			break;
		case E_GOAPAction.CoverEnter:
			gOAPAction = new GOAPActionCoverEnter(owner);
			break;
		case E_GOAPAction.CoverMove:
			gOAPAction = new GOAPActionCoverMove(owner);
			break;
		case E_GOAPAction.CoverFire:
			gOAPAction = new GOAPActionCoverFire(owner);
			break;
		case E_GOAPAction.CoverLeave:
			gOAPAction = new GOAPActionCoverLeave(owner);
			break;
		case E_GOAPAction.CoverFireAI:
			gOAPAction = new GOAPActionCoverFireAI(owner);
			break;
		case E_GOAPAction.CoverFireAISuppressive:
			gOAPAction = new GOAPActionCoverFireAISuppressive(owner);
			break;
		case E_GOAPAction.CoverLookAtAndFire:
			gOAPAction = new GOAPActionCoverLookAtAndFire(owner);
			break;
		case E_GOAPAction.CoverJumpOver:
			gOAPAction = new GOAPActionCoverJumpOver(owner);
			break;
		case E_GOAPAction.CoverJumpOverPlayer:
			gOAPAction = new GOAPActionCoverJumpOverPlayer(owner);
			break;
		case E_GOAPAction.CoverLeaveRightLeft:
			gOAPAction = new GOAPActionCoverLeaveRightLeft(owner);
			break;
		case E_GOAPAction.FireAI:
			gOAPAction = new GOAPActionFireAI(owner);
			break;
		case E_GOAPAction.FireAISuppressive:
			gOAPAction = new GOAPActionFireAISuppressive(owner);
			break;
		case E_GOAPAction.DodgeStrafe:
			gOAPAction = new GOAPActionDodgeStrafe(owner);
			break;
		case E_GOAPAction.DodgeStrafeWalk:
			gOAPAction = new GOAPActionDodgeStrafeWalk(owner);
			break;
		case E_GOAPAction.Roll:
			gOAPAction = new GOAPActionRoll(owner);
			break;
		case E_GOAPAction.Patrol:
			gOAPAction = new GOAPActionPatrol(owner);
			break;
		case E_GOAPAction.ShielderGoto:
			gOAPAction = new GOAPActionShielderGoTo(owner);
			break;
		case E_GOAPAction.CritInjury:
			gOAPAction = new GOAPActionShielderCritInjury(owner);
			break;
		case E_GOAPAction.Suppressed:
			gOAPAction = new GOAPActionSuppressed(owner);
			break;
		default:
			Debug.LogError("GOAPActionFactory -  unknow state " + type);
			return null;
		}
		gOAPAction.InitAction();
		return gOAPAction;
	}
}
