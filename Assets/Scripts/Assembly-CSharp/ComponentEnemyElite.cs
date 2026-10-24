using System;
using UnityEngine;

[RequireComponent(typeof(AnimSetMutantElite))]
public class ComponentEnemyElite : ComponentEnemy
{
	public float HeightStand = 2f;

	public float HeightCover = 1.3f;

	public LightningFX Lighting;

	private AgentAction CriticalInjury;

	private new void Awake()
	{
		base.Awake();
		base.Owner.BlackBoard.AiRecon = new AiRecon(base.Owner, 16, 8f);
		base.Owner.BlackBoard.DontDeathAnimMove = true;
		base.Owner.BlackBoard.GrenadesExplodeOnHit = false;
	}

	private new void Start()
	{
		base.Start();
		base.Owner.SensorsComponent.AddSensor(E_SensorType.EyesAi, true);
		base.Owner.SensorsComponent.AddSensor(E_SensorType.Positions, true);
		base.Owner.AddGOAPAction(E_GOAPAction.Goto);
		base.Owner.AddGOAPAction(E_GOAPAction.Fallback);
		base.Owner.AddGOAPAction(E_GOAPAction.AdvanceToCoveredTarget);
		base.Owner.AddGOAPAction(E_GOAPAction.WeaponReload);
		base.Owner.AddGOAPAction(E_GOAPAction.FireAI);
		base.Owner.AddGOAPAction(E_GOAPAction.LookatTarget);
		base.Owner.AddGOAPAction(E_GOAPAction.CheckLostEnemy);
		base.Owner.AddGOAPAction(E_GOAPAction.CheckEvent);
		base.Owner.AddGOAPAction(E_GOAPAction.FireAISuppressive);
		base.Owner.AddGOAPAction(E_GOAPAction.PlayAnim);
		base.Owner.AddGOAPAction(E_GOAPAction.Patrol);
		base.Owner.AddGOAPAction(E_GOAPAction.CritInjury);
		base.Owner.AddGOAPGoal(E_GOAPGoals.KillTarget);
		base.Owner.AddGOAPGoal(E_GOAPGoals.WeaponReloadAI);
		base.Owner.AddGOAPGoal(E_GOAPGoals.LookAtTarget);
		base.Owner.AddGOAPGoal(E_GOAPGoals.CheckEvent);
		base.Owner.AddGOAPGoal(E_GOAPGoals.SuppressiveFire);
		base.Owner.AddGOAPGoal(E_GOAPGoals.KeepCombatRange);
		base.Owner.AddGOAPGoal(E_GOAPGoals.KeepCombatRangeTargetCovered);
		base.Owner.AddGOAPGoal(E_GOAPGoals.SpawnAction);
		base.Owner.AddGOAPGoal(E_GOAPGoals.CriticalInjury);
		base.Owner.InitializeGOAP();
		BlackBoard blackBoard = base.Owner.BlackBoard;
		blackBoard.ActionHandler = (BlackBoard.AgentActionHandler)Delegate.Combine(blackBoard.ActionHandler, new BlackBoard.AgentActionHandler(HandleActions));
	}

	private new void Activate(SpawnPoint spawn)
	{
		base.Activate(spawn);
		base.Owner.CollisionController.center = new Vector3(0f, HeightStand * 0.5f, 0f);
		base.Owner.CollisionController.height = HeightStand;
		base.Owner.BlackBoard.Invulnerable = true;
		Lighting.enabled = false;
	}

	private new void Deactivate()
	{
		CriticalInjury = null;
		base.Deactivate();
	}

	private new void Update()
	{
		if (CriticalInjury != null && !CriticalInjury.IsActive())
		{
			CriticalInjury = null;
			base.Owner.BlackBoard.ReactOnHits = true;
			base.Owner.BlackBoard.Invulnerable = true;
			Lighting.enabled = false;
		}
		base.Update();
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
		}
		else
		{
			blackBoard.SetFear(blackBoard.Fear - blackBoard.CombatSetup.FearModificator * Time.deltaTime);
		}
		blackBoard.SetDodge(blackBoard.Dodge - blackBoard.CombatSetup.DodgeModificator * Time.deltaTime);
		if (base.Owner.BlackBoard.AbsorbedDamage >= 300f)
		{
			base.Owner.WorldState.SetWSProperty(E_PropKey.CriticalInjury, true);
			base.Owner.BlackBoard.AbsorbedDamage = 0f;
		}
		if (base.Owner.BlackBoard.AbsorbedDamage > 0f)
		{
			base.Owner.BlackBoard.AbsorbedDamage -= 10f * Time.deltaTime;
		}
	}

	public void HandleActions(AgentAction a)
	{
		if (a is AgentActionInjuryCrit)
		{
			CriticalInjury = a;
			base.Owner.BlackBoard.ReactOnHits = false;
			base.Owner.BlackBoard.Invulnerable = false;
			Lighting.enabled = true;
			base.Owner.SoundPlay(base.Fear);
		}
		else if (a is AgentActionDeath)
		{
			Lighting.enabled = false;
		}
	}
}
