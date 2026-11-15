using System;
using UnityEngine;

[RequireComponent(typeof(AnimSetMutantShield))]
public class ComponentEnemyShield : ComponentEnemy
{
	public float HeightStand = 2f;

	public float HeightCover = 1.3f;

	private float NextInjuryTime;

	private AgentActionInjuryCrit CriticalAction;

	private new void Awake()
	{
		base.Awake();
		base.Owner.BlackBoard.AiRecon = new AiRecon(base.Owner, 16, 4f);
	}

	private new void Start()
	{
		base.Start();
		base.Owner.SensorsComponent.AddSensor(E_SensorType.EyesAi, true);
		base.Owner.SensorsComponent.AddSensor(E_SensorType.Positions, true);
		base.Owner.AddGOAPAction(E_GOAPAction.Goto);
		base.Owner.AddGOAPAction(E_GOAPAction.ShielderGoto);
		base.Owner.AddGOAPAction(E_GOAPAction.WeaponReload);
		base.Owner.AddGOAPAction(E_GOAPAction.FireAI);
		base.Owner.AddGOAPAction(E_GOAPAction.LookatTarget);
		base.Owner.AddGOAPAction(E_GOAPAction.CheckLostEnemy);
		base.Owner.AddGOAPAction(E_GOAPAction.CheckEvent);
		base.Owner.AddGOAPAction(E_GOAPAction.FireAISuppressive);
		base.Owner.AddGOAPAction(E_GOAPAction.PlayAnim);
		base.Owner.AddGOAPAction(E_GOAPAction.Patrol);
		base.Owner.AddGOAPAction(E_GOAPAction.CritInjury);
		base.Owner.AddGOAPAction(E_GOAPAction.Suppressed);
		base.Owner.AddGOAPGoal(E_GOAPGoals.KillTarget);
		base.Owner.AddGOAPGoal(E_GOAPGoals.WeaponReloadAI);
		base.Owner.AddGOAPGoal(E_GOAPGoals.LookAtTarget);
		base.Owner.AddGOAPGoal(E_GOAPGoals.CheckEvent);
		base.Owner.AddGOAPGoal(E_GOAPGoals.SuppressiveFire);
		base.Owner.AddGOAPGoal(E_GOAPGoals.KeepCombatRange);
		base.Owner.AddGOAPGoal(E_GOAPGoals.KeepCombatRangeTargetCovered);
		base.Owner.AddGOAPGoal(E_GOAPGoals.SpawnAction);
		base.Owner.AddGOAPGoal(E_GOAPGoals.CriticalInjury);
		base.Owner.AddGOAPGoal(E_GOAPGoals.Suppressed);
		base.Owner.InitializeGOAP();
		BlackBoard blackBoard = base.Owner.BlackBoard;
		blackBoard.ActionHandler = (BlackBoard.AgentActionHandler)Delegate.Combine(blackBoard.ActionHandler, new BlackBoard.AgentActionHandler(HandleAction));
	}

	private new void Activate(SpawnPoint spawn)
	{
		base.Activate(spawn);
		base.Owner.CollisionController.center = new Vector3(0f, HeightStand * 0.5f, 0f);
		base.Owner.CollisionController.height = HeightStand;
		base.Owner.BlackBoard.Desires.WalkOnly = true;
		NextInjuryTime = 0f;
	}

	private new void Deactivate()
	{
		base.Deactivate();
	}

	private new void Update()
	{
		if (CriticalAction != null && !CriticalAction.IsActive())
		{
			CriticalAction = null;
			NextInjuryTime = Time.timeSinceLevelLoad + 2f;
			base.Owner.BlackBoard.ReactOnHits = true;
		}
		base.Update();
	}

	public new void HandleAction(AgentAction a)
	{
		if (NextInjuryTime < Time.timeSinceLevelLoad && a is AgentActionInjury && CriticalAction == null)
		{
			base.Owner.WorldState.SetWSProperty(E_PropKey.CriticalInjury, true);
		}
		if (a is AgentActionInjuryCrit)
		{
			CriticalAction = a as AgentActionInjuryCrit;
			base.Owner.BlackBoard.ReactOnHits = false;
		}
	}

	protected override void UpdateCombatSetting()
	{
		bool flag = base.Owner.WorldState.GetWSProperty(E_PropKey.SeeEnemy).GetBool();
		BlackBoard blackBoard = base.Owner.BlackBoard;
		if (!flag && (base.Owner.Memory.GetValidFact(E_EventTypes.EnemyHideInCover) != null || base.Owner.Memory.GetValidFact(E_EventTypes.EnemyLost) != null))
		{
			flag = true;
		}
		if (flag)
		{
			blackBoard.SetRage(blackBoard.Rage + blackBoard.CombatSetup.RageModificator * Time.deltaTime);
		}
		else
		{
			blackBoard.SetRage(blackBoard.Rage - blackBoard.CombatSetup.RageModificator * Time.deltaTime * 0.5f);
		}
		if (base.Owner.WorldState.GetWSProperty(E_PropKey.EnemyLookingAtMe).GetBool())
		{
			blackBoard.SetFear(blackBoard.Fear - blackBoard.CombatSetup.FearModificator * Time.deltaTime * 0.5f);
			blackBoard.SetDodge(blackBoard.Dodge + blackBoard.CombatSetup.DodgeModificator * Time.deltaTime);
		}
		else
		{
			blackBoard.SetFear(blackBoard.Fear - blackBoard.CombatSetup.FearModificator * Time.deltaTime);
			blackBoard.SetDodge(blackBoard.Dodge - blackBoard.CombatSetup.DodgeModificator * Time.deltaTime);
		}
	}
}
