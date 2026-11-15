using UnityEngine;

public static class AgentActionFactory
{
	public enum E_Type
	{
		Idle = 0,
		Move = 1,
		Goto = 2,
		GotoByAnim = 3,
		CombatMove = 4,
		Attack = 5,
		Injury = 6,
		InjuryCrit = 7,
		DamageBlocked = 8,
		Block = 9,
		Roll = 10,
		IncommingAttack = 11,
		WeaponShow = 12,
		Rotate = 13,
		Use = 14,
		PlayAnim = 15,
		PlayIdleAnim = 16,
		Death = 17,
		Knockdown = 18,
		Teleport = 19,
		CoverEnter = 20,
		CoverLeave = 21,
		CoverMove = 22,
		CoverFire = 23,
		CoverFireCancel = 24,
		CoverTerminated = 25,
		Reload = 26,
		DodgeStrafe = 27,
		WeaponChange = 28,
		Count = 29
	}

	private static Queue<AgentAction>[] m_UnusedActions;

	static AgentActionFactory()
	{
		m_UnusedActions = new Queue<AgentAction>[29];
		for (E_Type e_Type = E_Type.Idle; e_Type < E_Type.Count; e_Type++)
		{
			m_UnusedActions[(int)e_Type] = new Queue<AgentAction>();
		}
	}

	public static AgentAction Create(E_Type type)
	{
		AgentAction agentAction;
		if (m_UnusedActions[(int)type].Count != 0)
		{
			agentAction = m_UnusedActions[(int)type].Dequeue();
		}
		else
		{
			switch (type)
			{
			case E_Type.Idle:
				agentAction = new AgentActionIdle();
				break;
			case E_Type.Move:
				agentAction = new AgentActionMove();
				break;
			case E_Type.Goto:
				agentAction = new AgentActionGoTo();
				break;
			case E_Type.GotoByAnim:
				agentAction = new AgentActionGoToByAnim();
				break;
			case E_Type.CombatMove:
				agentAction = new AgentActioCombatMove();
				break;
			case E_Type.Attack:
				agentAction = new AgentActionAttack();
				break;
			case E_Type.Injury:
				agentAction = new AgentActionInjury();
				break;
			case E_Type.InjuryCrit:
				agentAction = new AgentActionInjuryCrit();
				break;
			case E_Type.DamageBlocked:
				agentAction = new AgentActionDamageBlocked();
				break;
			case E_Type.Roll:
				agentAction = new AgentActionRoll();
				break;
			case E_Type.IncommingAttack:
				agentAction = new AgentActionIncommingAttack();
				break;
			case E_Type.WeaponChange:
				agentAction = new AgentActionWeaponChange();
				break;
			case E_Type.Rotate:
				agentAction = new AgentActionRotate();
				break;
			case E_Type.Use:
				agentAction = new AgentActionUse();
				break;
			case E_Type.PlayAnim:
				agentAction = new AgentActionPlayAnim();
				break;
			case E_Type.PlayIdleAnim:
				agentAction = new AgentActionPlayIdleAnim();
				break;
			case E_Type.Death:
				agentAction = new AgentActionDeath();
				break;
			case E_Type.Knockdown:
				agentAction = new AgentActionKnockdown();
				break;
			case E_Type.Teleport:
				agentAction = new AgentActionTeleport();
				break;
			case E_Type.CoverEnter:
				agentAction = new AgentActionCoverEnter();
				break;
			case E_Type.CoverMove:
				agentAction = new AgentActionCoverMove();
				break;
			case E_Type.CoverFire:
				agentAction = new AgentActionCoverFire();
				break;
			case E_Type.CoverFireCancel:
				agentAction = new AgentActionCoverFireCancel();
				break;
			case E_Type.CoverLeave:
				agentAction = new AgentActionCoverLeave();
				break;
			case E_Type.CoverTerminated:
				agentAction = new AgentActionCoverTerminated();
				break;
			case E_Type.Reload:
				agentAction = new AgentActionReload();
				break;
			case E_Type.DodgeStrafe:
				agentAction = new AgentActionDodgeStrafe();
				break;
			default:
				Debug.LogError("no AgentAction to create");
				return null;
			}
		}
		agentAction.Reset();
		agentAction.SetActive();
		return agentAction;
	}

	public static void Return(AgentAction action)
	{
		action.SetUnused();
		m_UnusedActions[(int)action.Type].Enqueue(action);
	}

	public static void Clear()
	{
		for (E_Type e_Type = E_Type.Idle; e_Type < E_Type.Count; e_Type++)
		{
			m_UnusedActions[(int)e_Type].Clear();
		}
	}
}
