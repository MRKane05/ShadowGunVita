using System;
using UnityEngine;

[RequireComponent(typeof(AnimSetMutantKamikaze))]
public class ComponentEnemyKamikaze : ComponentEnemy
{
	public float HeightStand = 2f;

	public float HeightCover = 1.3f;

	private new void Awake()
	{
		base.Awake();
		base.Owner.BlackBoard.AiRecon = new AiRecon(base.Owner, 16, 8f);
	}

	private new void Start()
	{
		base.Start();
		base.Owner.SensorsComponent.AddSensor(E_SensorType.EyesAi, true);
		base.Owner.SensorsComponent.AddSensor(E_SensorType.Positions, true);
		base.Owner.AddGOAPAction(E_GOAPAction.FireAI);
		base.Owner.AddGOAPAction(E_GOAPAction.LookatTarget);
		base.Owner.AddGOAPAction(E_GOAPAction.CheckEvent);
		base.Owner.AddGOAPAction(E_GOAPAction.PlayAnim);
		base.Owner.AddGOAPAction(E_GOAPAction.Patrol);
		base.Owner.AddGOAPAction(E_GOAPAction.WeaponReload);
		base.Owner.AddGOAPGoal(E_GOAPGoals.KillTarget);
		base.Owner.AddGOAPGoal(E_GOAPGoals.LookAtTarget);
		base.Owner.AddGOAPGoal(E_GOAPGoals.CheckEvent);
		base.Owner.AddGOAPGoal(E_GOAPGoals.RunAway);
		base.Owner.AddGOAPGoal(E_GOAPGoals.SpawnAction);
		base.Owner.AddGOAPGoal(E_GOAPGoals.WeaponReloadAI);
		base.Owner.InitializeGOAP();
		BlackBoard blackBoard = base.Owner.BlackBoard;
		blackBoard.ActionHandler = (BlackBoard.AgentActionHandler)Delegate.Combine(blackBoard.ActionHandler, new BlackBoard.AgentActionHandler(HandleAction));
	}

	private new void Activate(SpawnPoint spawn)
	{
		base.Activate(spawn);
		base.Owner.CollisionController.center = new Vector3(0f, HeightStand * 0.5f, 0f);
		base.Owner.CollisionController.height = HeightStand;
	}

	private new void Deactivate()
	{
		base.Deactivate();
	}

	private new void Update()
	{
		base.Update();
	}

	public new void HandleAction(AgentAction a)
	{
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
