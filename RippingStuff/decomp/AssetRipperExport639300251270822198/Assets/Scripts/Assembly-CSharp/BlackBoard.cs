using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BlackBoard
{
	[Serializable]
	public class BaseSettings
	{
		public float SightRangeInCombat = 60f;

		public float SightFovInCombat = 120f;

		public float MaxRunSpeed = 4f;

		public float MaxWalkSpeed = 1.5f;

		public float MaxCoverSpeed = 1f;

		public float MaxHealth = 100f;

		public float SpeedSmooth = 2f;

		public float RollDistance = 3f;
	}

	[Serializable]
	public class CombatSettings
	{
		public const float MaxValue = 100f;

		public bool DontFireWhenRunning;

		public float RageMin;

		public float RageMax;

		public float RageModificator;

		public float DodgeMin;

		public float DodgeMax;

		public float DodgeModificator;

		public float FearMin;

		public float FearMax;

		public float FearModificator;

		public float RageInjuryModificator;

		public float DodgeInjuryModificator;

		public float FearInjuryModificator;

		public float RageAttackModificator;

		public float DodgeAttackModificator;

		public float FearAttackModificator;

		public float FearFriendDeathModificator;

		public float RageFriendDeathModificator;

		public float FearFriendInjuryModificator;

		public float RageFriendInjuryModificator;
	}

	[Serializable]
	public class GoapSettings
	{
		public float DodgeRelevancy = 0.9f;

		public float MoveRelevancy = 0.5f;

		public float GoToRelevancy = 0.5f;

		public float AdvanceRelevancy = 0.7f;

		public float AdvanceToEnemyCoverRelevancy = 0.7f;

		public float FallbackRelevancy = 0.6f;

		public float RunAwayRelevancy = 0.6f;

		public float KeepCombatRangeRelevancy = 0.6f;

		public float KeepCombatRangeTargetCoveredRelevancy = 0.6f;

		public float HideInCoverRelevancy = 0.5f;

		public float LookAtTargetRelevancy = 0.7f;

		public float CheckEventRelevancy = 0.7f;

		public float KillTargetRelevancy = 0.85f;

		public float SuppressiveFireRelevancy = 0.4f;

		public float PlayAnimRelevancy = 0.95f;

		public float UseWorlObjectRelevancy = 0.9f;

		public float IdleActionRelevancy = 0.4f;

		public float SuppressedRelevancy = 0.7f;

		public float CoverRelevancy = 0.8f;

		public float ReloadRelevancy = 0.8f;

		public float TeleportRelevancy = 0.9f;

		public float WeaponChangeRelevancy = 0.8f;

		public float CriticalInjuryRelevancy = 1f;

		public float DodgeDelay = 5f;

		public float MoveDelay = 0.1f;

		public float GoToDelay = 0.5f;

		public float AdvanceDelay = 0.5f;

		public float AdvanceToEnemyCoverDelay = 0.7f;

		public float FallbackDelay = 0.5f;

		public float RunAwayDelay = 0.5f;

		public float KeepCombatRangeDelay = 0.5f;

		public float KeepCombatRangeTargetCoveredDelay = 0.5f;

		public float HideInCoverDelay = 0.5f;

		public float HideInCoverRelevancyDelay = 0.5f;

		public float LookAtTargetDelay = 0.4f;

		public float CheckEventDelay = 0.8f;

		public float KillTargetDelay = 2.8f;

		public float SuppressiveFireDelay = 2.8f;

		public float CriticalInjuryDelay;

		public float UseWorlObjectDelay = 5f;

		public float IdleActionDelay = 10f;

		public float SuppressedDelay = 1f;

		public float CoverDelay = 1f;

		public float ReloadDelay = 1f;

		public float TeleportDelay = 4f;

		public float WeaponChangeDelay = 0.1f;
	}

	[Serializable]
	public class DamageSettings
	{
		public bool Invulnerable;

		public float DontAttackTimer = 2f;
	}

	[Serializable]
	public class CoverSettings
	{
		public float RightMaxUp = 40f;

		public float RightMaxDown = 40f;

		public float RightMaxRight = 70f;

		public float RightMaxLeft = 20f;

		public float LeftMaxUp = 40f;

		public float LeftMaxDown = 40f;

		public float LeftMaxRight = 20f;

		public float LeftMaxLeft = 70f;

		public float CenterMaxUp = 40f;

		public float CenterMaxDown = 20f;

		public float CenterMaxRight = 70f;

		public float CenterMaxLeft = 70f;
	}

	[Serializable]
	public class AttackSettings
	{
		public int MinShortBurst = 3;

		public int MaxShortBurst = 5;
	}

	[Serializable]
	public class StepsSettings
	{
		public float RunMinDelay = 0.35f;

		public float RunMaxDelay = 0.38f;

		public float WalkMinDelay = 0.64f;

		public float WalkMaxDelay = 0.73f;

		public float AnimMinDelay = 0.64f;

		public float AnimMaxDelay = 0.73f;
	}

	public class DesiredData
	{
		public Quaternion Rotation;

		public Vector3 MoveDirection;

		public Vector3 FireDirection;

		public bool WalkOnly;

		public float MoveSpeedModifier;

		public CoverAIInfo CoverNear = new CoverAIInfo();

		public CoverAIInfo CoverAdvance = new CoverAIInfo();

		public CoverAIInfo CoverFallback = new CoverAIInfo();

		public CoverAIInfo CoverFlankLeft = new CoverAIInfo();

		public CoverAIInfo CoverFlankRight = new CoverAIInfo();

		public Cover CoverSelected;

		public E_CoverDirection CoverPosition;

		public InteractionObject InteractionObject;

		public string Animation;

		public E_WeaponType Weapon;

		public bool WeaponTriggerOn;

		public bool LookAtTarget = true;

		public Vector3 TeleportDestination = default(Vector3);

		public Quaternion TeleportRotation = default(Quaternion);

		public List<GameObject> PatrolPoints = new List<GameObject>();

		public void Reset()
		{
			MoveDirection = Vector3.zero;
			FireDirection = Vector3.zero;
			WalkOnly = false;
			MoveSpeedModifier = 1f;
			CoverSelected = null;
			CoverPosition = E_CoverDirection.Unknown;
			CoverNear.Reset();
			CoverAdvance.Reset();
			CoverFallback.Reset();
			CoverFlankLeft.Reset();
			CoverFlankRight.Reset();
			InteractionObject = null;
			Animation = null;
			Weapon = E_WeaponType.None;
			WeaponTriggerOn = false;
			LookAtTarget = true;
			PatrolPoints.Clear();
		}
	}

	public delegate void AgentActionHandler(AgentAction a);

	private List<AgentAction> m_ActiveActions = new List<AgentAction>();

	[NonSerialized]
	public AgentHuman Owner;

	[NonSerialized]
	public GameObject GameObject;

	[NonSerialized]
	public bool IsPlayer;

	public float WeaponRange = 2f;

	public float CombatRange = 4f;

	public GoapSettings GoapSetup = new GoapSettings();

	public BaseSettings BaseSetup = new BaseSettings();

	public CombatSettings CombatSetup = new CombatSettings();

	public DamageSettings DamageSetup = new DamageSettings();

	public CoverSettings CoverSetup = new CoverSettings();

	public AttackSettings AttackSetup = new AttackSettings();

	public StepsSettings StepsSetup = new StepsSettings();

	[NonSerialized]
	public DesiredData Desires = new DesiredData();

	[NonSerialized]
	public AiRecon AiRecon;

	[NonSerialized]
	public E_MotionType MotionType;

	[NonSerialized]
	public E_MoveType MoveType = E_MoveType.None;

	[NonSerialized]
	public Vector3 MoveDir;

	[NonSerialized]
	public Vector3 FireDir;

	[NonSerialized]
	public Vector3 Velocity;

	[NonSerialized]
	public E_CoverPose CoverPose;

	[NonSerialized]
	public E_CoverDirection CoverPosition = E_CoverDirection.Middle;

	[NonSerialized]
	public Cover Cover;

	[NonSerialized]
	public float CoverTime;

	[NonSerialized]
	public bool Alarmed;

	[NonSerialized]
	public float SightRange;

	[NonSerialized]
	public float SightFov;

	[NonSerialized]
	public float Speed;

	[NonSerialized]
	public float Health = 30f;

	[NonSerialized]
	public float Rage;

	[NonSerialized]
	public float Fear;

	[NonSerialized]
	public float Dodge;

	[NonSerialized]
	public InteractionObject InteractionObject;

	[NonSerialized]
	public AgentHuman DangerousEnemy;

	[NonSerialized]
	public AgentHuman VisibleTarget;

	[NonSerialized]
	public float DistanceToTarget;

	[NonSerialized]
	public Vector3 DirToTarget;

	[NonSerialized]
	public bool DontUpdate = true;

	[NonSerialized]
	public bool ReactOnHits = true;

	[NonSerialized]
	public bool BusyAction;

	[NonSerialized]
	public bool Invulnerable;

	[NonSerialized]
	public float AbsorbedDamage;

	[NonSerialized]
	public float IdleTimer;

	[NonSerialized]
	public bool DontDeathAnimMove;

	[NonSerialized]
	public bool GrenadesExplodeOnHit = true;

	[NonSerialized]
	public bool ComlinkOn;

	[NonSerialized]
	public bool AimAnimationsEnabled;

	[NonSerialized]
	public SpawnPointEnemy SpawnPointEnemy;

	public AgentActionHandler ActionHandler;

	public float sqrWeaponRange
	{
		get
		{
			return WeaponRange * WeaponRange;
		}
	}

	public float sqrCombatRange
	{
		get
		{
			return CombatRange * CombatRange;
		}
	}

	public bool Stop { get; set; }

	public float RealMaxHealth
	{
		get
		{
			return BaseSetup.MaxHealth;
		}
	}

	public void Reset()
	{
		Desires.Reset();
		for (int i = 0; i < m_ActiveActions.Count; i++)
		{
			ActionDone(m_ActiveActions[i]);
		}
		if (AiRecon != null)
		{
			AiRecon.Reset();
		}
		m_ActiveActions.Clear();
		Stop = false;
		MotionType = E_MotionType.None;
		MoveType = E_MoveType.None;
		Speed = 0f;
		Health = RealMaxHealth;
		Rage = CombatSetup.RageMin;
		Dodge = CombatSetup.DodgeMin;
		Fear = CombatSetup.FearMin;
		IdleTimer = 0f;
		CoverTime = 0f;
		Cover = null;
		CoverPosition = E_CoverDirection.Unknown;
		MoveDir = Vector3.zero;
		FireDir = Owner.Transform.forward;
		Desires.Rotation = Owner.Transform.rotation;
		Desires.FireDirection = Owner.Transform.forward;
		InteractionObject = null;
		DangerousEnemy = null;
		Invulnerable = false;
		AbsorbedDamage = 0f;
		ReactOnHits = true;
		BusyAction = false;
		DontUpdate = false;
		SpawnPointEnemy = null;
		Alarmed = false;
	}

	public void ActionAdd(AgentAction action)
	{
		IdleTimer = 0f;
		m_ActiveActions.Add(action);
		ActionHandler(action);
	}

	public void Update()
	{
		if ((bool)VisibleTarget && !VisibleTarget.IsAlive)
		{
			VisibleTarget = null;
		}
		Owner.WorldState.SetWSProperty(E_PropKey.TargetNode, Owner.Position);
		DangerousEnemy = VisibleTarget;
		Fact validFact;
		if (DangerousEnemy == null && (validFact = Owner.Memory.GetValidFact(E_EventTypes.EnemyHideInCover)) != null)
		{
			DangerousEnemy = validFact.Agent;
		}
		if (DangerousEnemy == null && (validFact = Owner.Memory.GetValidFact(E_EventTypes.EnemyInjuredMe)) != null)
		{
			DangerousEnemy = validFact.Agent;
		}
		if (DangerousEnemy == null && (validFact = Owner.Memory.GetValidFact(E_EventTypes.EnemyFire)) != null)
		{
			DangerousEnemy = validFact.Agent;
		}
		if (DangerousEnemy == null && (validFact = Owner.Memory.GetValidFact(E_EventTypes.FriendDeath)) != null)
		{
			DangerousEnemy = validFact.Agent;
		}
		if (DangerousEnemy == null && (validFact = Owner.Memory.GetValidFact(E_EventTypes.FriendInjured)) != null)
		{
			DangerousEnemy = validFact.Agent;
		}
	}

	public void PostUpdate()
	{
		for (int i = 0; i < m_ActiveActions.Count; i++)
		{
			if (!m_ActiveActions[i].IsActive())
			{
				ActionDone(m_ActiveActions[i]);
				m_ActiveActions.RemoveAt(i);
				break;
			}
		}
	}

	private void ActionDone(AgentAction action)
	{
		AgentActionFactory.Return(action);
	}

	public void SetFear(float value)
	{
		Fear = value;
		if (Fear > CombatSetup.FearMax)
		{
			Fear = CombatSetup.FearMax;
		}
		else if (Fear < CombatSetup.FearMin)
		{
			Fear = CombatSetup.FearMin;
		}
	}

	public void SetRage(float value)
	{
		Rage = value;
		if (Rage > CombatSetup.RageMax)
		{
			Rage = CombatSetup.RageMax;
		}
		else if (Rage < CombatSetup.RageMin)
		{
			Rage = CombatSetup.RageMin;
		}
	}

	public void SetDodge(float value)
	{
		Dodge = value;
		if (Dodge > CombatSetup.DodgeMax)
		{
			Dodge = CombatSetup.DodgeMax;
		}
		else if (Dodge < CombatSetup.DodgeMin)
		{
			Dodge = CombatSetup.DodgeMin;
		}
	}

	public void UpdateCombatSetting(AgentAction a)
	{
		if (a is AgentActionInjury)
		{
			AgentActionInjury agentActionInjury = a as AgentActionInjury;
			if (agentActionInjury.FromWeapon == E_WeaponType.RocketLauncher || agentActionInjury.FromWeapon == E_WeaponType.GrenadeLauncher)
			{
				SetFear(Fear + CombatSetup.FearInjuryModificator * 2f);
			}
			else
			{
				SetFear(Fear + CombatSetup.FearInjuryModificator);
			}
			SetRage(Rage + CombatSetup.RageInjuryModificator);
			SetDodge(Dodge + CombatSetup.DodgeInjuryModificator);
		}
		else if (a is AgentActionAttack)
		{
			SetRage(Rage - CombatSetup.RageAttackModificator);
			SetDodge(Dodge + CombatSetup.DodgeAttackModificator);
			SetFear(Fear - CombatSetup.FearAttackModificator);
		}
	}

	public void UpdateCombatSetting(Fact fact)
	{
		if (!Alarmed)
		{
			SightRange = BaseSetup.SightRangeInCombat;
			SightFov = BaseSetup.SightFovInCombat;
			Alarmed = true;
			Mission.Instance.CurrentGameZone.SendFactToEnemies(Owner, null, E_EventTypes.Alert, 100f, 0.2f, true);
		}
		switch (fact.Type)
		{
		case E_EventTypes.EnemyFire:
			if (Owner.WorldState.GetWSProperty(E_PropKey.EnemyLookingAtMe).GetBool())
			{
				SetFear(Fear + CombatSetup.FearInjuryModificator * 0.5f);
				SetRage(Rage + CombatSetup.RageInjuryModificator * 0.25f);
			}
			else
			{
				SetFear(Fear + CombatSetup.FearInjuryModificator * 0.25f);
				SetRage(Rage + CombatSetup.RageInjuryModificator * 0.5f);
			}
			break;
		case E_EventTypes.FriendDeath:
			SetFear(Fear + CombatSetup.FearFriendDeathModificator);
			SetRage(Rage + CombatSetup.RageFriendDeathModificator);
			break;
		case E_EventTypes.FriendInjured:
			SetFear(Fear + CombatSetup.FearFriendInjuryModificator);
			SetRage(Rage + CombatSetup.RageFriendInjuryModificator);
			break;
		}
	}
}
