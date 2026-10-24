using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[Serializable]
public class AgentHuman : Agent, IProjectileHitReceiver
{
	[Serializable]
	public class SoundInfo
	{
		public AudioClip[] Steps = new AudioClip[0];

		public AudioClip[] StepsMetal = new AudioClip[0];

		public AudioClip[] CoverIn = new AudioClip[0];

		public AudioClip[] CoverJump = new AudioClip[0];

		public AudioClip[] Injuries = new AudioClip[0];

		public AudioClip[] Deaths = new AudioClip[0];

		public AudioClip[] Teleport = new AudioClip[0];
	}

	public static float AgentUpdateTime = 0.1f;

	public BlackBoard BlackBoard = new BlackBoard();

	public SoundInfo SoundSetup = new SoundInfo();

	public Material FadeoutMaterial;

	public Material DiffuseMaterial;

	public Transform TransformEye;

	public Transform TransformTarget;

	private GOAPManager GoalManager;

	private Hashtable Actions = new Hashtable();

	private float FadeoutFXDuration = 2f;

	private float TimeToUpdateAgent;

	private float StepTime;

	public float m_PlasmaDamageMultiplier = 1f;

	public override bool IsPlayer
	{
		get
		{
			return BlackBoard.IsPlayer;
		}
	}

	public override bool IsAlive
	{
		get
		{
			return BlackBoard.Health > 0f && base.GameObject.active;
		}
	}

	public override bool IsVisible
	{
		get
		{
			return Renderer.isVisible;
		}
	}

	public override bool IsInvulnerable
	{
		get
		{
			return BlackBoard.DamageSetup.Invulnerable || BlackBoard.Invulnerable;
		}
	}

	public override bool IsInCover
	{
		get
		{
			return BlackBoard.Cover != null;
		}
	}

	public override bool IsEnteringToCover
	{
		get
		{
			return GoalManager.CurrentGoal != null && GoalManager.CurrentGoal.GoalType == E_GOAPGoals.CoverEnter;
		}
	}

	public bool IsLeavingToCover
	{
		get
		{
			return GoalManager.CurrentGoal != null && GoalManager.CurrentGoal.GoalType == E_GOAPGoals.CoverLeave;
		}
	}

	public bool IsComlinkOn
	{
		get
		{
			return BlackBoard.ComlinkOn;
		}
	}

	public override Vector3 ChestPosition
	{
		get
		{
			return base.Transform.position + base.transform.up * 1.5f;
		}
	}

	public AnimSet AnimSet { get; private set; }

	public WorldState WorldState { get; private set; }

	public Memory Memory { get; private set; }

	public CharacterController CharacterController { get; private set; }

	public CapsuleCollider CollisionController { get; private set; }

	public ComponentWeapons WeaponComponent { get; private set; }

	public ComponentSensors SensorsComponent { get; private set; }

	public AnimComponent AnimComponent { get; private set; }

	public NavMeshAgent NavMeshAgent { get; private set; }

	public SkinnedMeshRenderer Renderer { get; private set; }

	public Vector3 EyePosition
	{
		get
		{
			return TransformEye.position;
		}
	}

	public AudioClip StepSound
	{
		get
		{
			if (SoundSetup.Steps.Length == 0)
			{
				return null;
			}
			return SoundSetup.Steps[Random.Range(0, SoundSetup.Steps.Length)];
		}
	}

	public AudioClip StepMetalSound
	{
		get
		{
			if (SoundSetup.StepsMetal.Length == 0)
			{
				return null;
			}
			return SoundSetup.StepsMetal[Random.Range(0, SoundSetup.StepsMetal.Length)];
		}
	}

	public AudioClip CoverInSound
	{
		get
		{
			if (SoundSetup.CoverIn.Length == 0)
			{
				return null;
			}
			return SoundSetup.CoverIn[Random.Range(0, SoundSetup.CoverIn.Length)];
		}
	}

	public AudioClip CoverJumpSound
	{
		get
		{
			if (SoundSetup.CoverJump.Length == 0)
			{
				return null;
			}
			return SoundSetup.CoverJump[Random.Range(0, SoundSetup.CoverJump.Length)];
		}
	}

	public AudioClip InjuriesSound
	{
		get
		{
			if (SoundSetup.Injuries.Length == 0)
			{
				return null;
			}
			return SoundSetup.Injuries[Random.Range(0, SoundSetup.Injuries.Length)];
		}
	}

	public AudioClip DeathSound
	{
		get
		{
			if (SoundSetup.Deaths.Length == 0)
			{
				return null;
			}
			return SoundSetup.Deaths[Random.Range(0, SoundSetup.Deaths.Length)];
		}
	}

	public AudioClip TeleportSound
	{
		get
		{
			if (SoundSetup.Teleport.Length == 0)
			{
				return null;
			}
			return SoundSetup.Teleport[Random.Range(0, SoundSetup.Teleport.Length)];
		}
	}

	public GOAPAction GetAction(E_GOAPAction type)
	{
		return (GOAPAction)Actions[type];
	}

	public int GetNumberOfActions()
	{
		return Actions.Count;
	}

	private void Awake()
	{
		Initialize();
		CollisionController = base.Transform.GetComponent<CapsuleCollider>();
		CharacterController = base.Transform.GetComponent<CharacterController>();
		SensorsComponent = GetComponent<ComponentSensors>();
		WeaponComponent = GetComponent<ComponentWeapons>();
		AnimComponent = GetComponent<AnimComponent>();
		NavMeshAgent = GetComponent<NavMeshAgent>();
		if (NavMeshAgent != null)
		{
			NavMeshAgent.updateRotation = false;
		}
		BlackBoard.Owner = this;
		BlackBoard.GameObject = base.GameObject;
		AnimSet = GetComponent<AnimSet>();
		WorldState = new WorldState();
		Memory = new Memory(this);
		GoalManager = new GOAPManager(this);
		ResetAgent();
		BlackBoard.DontUpdate = true;
		TimeToUpdateAgent = 0f;
		base.GameObject.layer = 31;
		if ((bool)CollisionController && Game.Instance.GameDifficulty == E_GameDifficulty.Hard)
		{
			CollisionController.radius *= 0.9f;
		}
	}

	private void OnDestroy()
	{
		SoundSetup = null;
		BlackBoard.Reset();
		BlackBoard = null;
	}

	private void Start()
	{
		BlackBoard blackBoard = BlackBoard;
		blackBoard.ActionHandler = (BlackBoard.AgentActionHandler)Delegate.Combine(blackBoard.ActionHandler, new BlackBoard.AgentActionHandler(HandleAction));
		Renderer = base.GameObject.GetComponentInChildren(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
	}

	private void Activate(SpawnPoint spawn)
	{
		BlackBoard.DontUpdate = false;
		RaycastHit hitInfo;
		if (!Physics.Raycast(spawn.Transform.position + Vector3.up, -Vector3.up, out hitInfo, 5f, 16384))
		{
			base.Transform.position = spawn.Transform.position;
		}
		else
		{
			base.Transform.position = hitInfo.point;
		}
		base.Transform.rotation = spawn.Transform.rotation;
		BlackBoard.Desires.Rotation = spawn.Transform.rotation;
		BlackBoard.Desires.FireDirection = base.Transform.forward;
		StepTime = 0f;
		base.GameObject.active = true;
		if ((bool)NavMeshAgent)
		{
			NavMeshAgent.enabled = true;
			NavMeshAgent.updatePosition = true;
			NavMeshAgent.updateRotation = false;
		}
		base.GameObject.layer = 31;
	}

	private void Deactivate()
	{
		Reset();
	}

	private void LateUpdate()
	{
		if (!IsAlive)
		{
			return;
		}
		RaycastHit hitInfo;
		if ((BlackBoard.MotionType == E_MotionType.Run || BlackBoard.MotionType == E_MotionType.Walk || BlackBoard.MotionType == E_MotionType.AnimationDrive || BlackBoard.MotionType == E_MotionType.Comlink) && StepTime < Time.timeSinceLevelLoad && Physics.Raycast(base.Position + Vector3.up * 0.1f, -Vector3.up, out hitInfo))
		{
			AudioClip clip;
			switch (hitInfo.transform.gameObject.layer)
			{
			case 28:
				clip = StepSound;
				break;
			case 29:
				clip = StepMetalSound;
				break;
			default:
				clip = StepSound;
				break;
			}
			if (BlackBoard.MotionType == E_MotionType.Run)
			{
				SoundPlay(clip);
				StepTime = Time.timeSinceLevelLoad + Random.Range(BlackBoard.StepsSetup.RunMinDelay, BlackBoard.StepsSetup.RunMaxDelay);
			}
			else if (BlackBoard.MotionType == E_MotionType.Walk || BlackBoard.MotionType == E_MotionType.Comlink)
			{
				SoundPlay(clip);
				StepTime = Time.timeSinceLevelLoad + Random.Range(BlackBoard.StepsSetup.WalkMinDelay, BlackBoard.StepsSetup.WalkMaxDelay);
			}
			else if (BlackBoard.MotionType == E_MotionType.AnimationDrive)
			{
				SoundPlay(clip);
				StepTime = Time.timeSinceLevelLoad + Random.Range(BlackBoard.StepsSetup.AnimMinDelay, BlackBoard.StepsSetup.AnimMaxDelay);
			}
		}
		BlackBoard.IdleTimer += Time.deltaTime;
		if (BlackBoard.Cover != null)
		{
			BlackBoard.CoverTime += Time.deltaTime;
		}
		else
		{
			BlackBoard.CoverTime = 0f;
		}
		if (TimeToUpdateAgent < Time.timeSinceLevelLoad)
		{
			UpdateAgent();
			if (!IsPlayer)
			{
				TimeToUpdateAgent = Time.timeSinceLevelLoad + AgentUpdateTime;
			}
		}
	}

	private void UpdateAgent()
	{
		if (!BlackBoard.DontUpdate)
		{
			BlackBoard.Update();
			if (!BlackBoard.BusyAction)
			{
				GoalManager.UpdateCurrentGoal();
				GoalManager.ManageGoals();
			}
			Memory.Update();
			BlackBoard.PostUpdate();
			WorldState.SetWSProperty(E_PropKey.Start, GoalManager.CurrentGoal == null);
		}
	}

	public void HandleAction(AgentAction a)
	{
		if (a is AgentActionCoverEnter)
		{
			SoundPlay(CoverInSound);
		}
		else if (a is AgentActionCoverLeave && (a as AgentActionCoverLeave).TypeOfLeave == AgentActionCoverLeave.E_Type.Jump)
		{
			SoundPlay(CoverJumpSound);
		}
	}

	public void PrepareForStart()
	{
	}

	public void Reset()
	{
		ResetAgent();
		EnableCollisions();
	}

	public void Stop(bool stop)
	{
		BlackBoard.Stop = stop;
	}

	private void ResetAgent()
	{
		if (NavMeshAgent != null)
		{
			NavMeshAgent.enabled = false;
		}
		if (IsInCover)
		{
			BlackBoard.Cover.LockPosition(BlackBoard.CoverPosition, null);
		}
		StopAllCoroutines();
		GoalManager.Reset();
		BlackBoard.Reset();
		WorldState.Reset();
		WorldState.SetWSProperty(E_PropKey.Start, true);
		WorldState.SetWSProperty(E_PropKey.AtTargetPos, true);
		WorldState.SetWSProperty(E_PropKey.TargetNode, base.Position);
		WorldState.SetWSProperty(E_PropKey.KillTarget, false);
		WorldState.SetWSProperty(E_PropKey.LookingAtTarget, false);
		WorldState.SetWSProperty(E_PropKey.EnemyLookingAtMe, false);
		WorldState.SetWSProperty(E_PropKey.UseWorldObject, false);
		WorldState.SetWSProperty(E_PropKey.PlayAnim, false);
		WorldState.SetWSProperty(E_PropKey.InDodge, false);
		WorldState.SetWSProperty(E_PropKey.WeaponChange, false);
		WorldState.SetWSProperty(E_PropKey.WeaponLoaded, false);
		WorldState.SetWSProperty(E_PropKey.SeeEnemy, false);
		WorldState.SetWSProperty(E_PropKey.AheadOfEnemy, false);
		WorldState.SetWSProperty(E_PropKey.Teleport, false);
		WorldState.SetWSProperty(E_PropKey.CoverState, E_CoverState.None);
		WorldState.SetWSProperty(E_PropKey.Event, E_EventTypes.None);
		WorldState.SetWSProperty(E_PropKey.DoSpawnAction, false);
		WorldState.SetWSProperty(E_PropKey.Patrol, false);
		WorldState.SetWSProperty(E_PropKey.CriticalInjury, false);
		ResetMaterial();
		Memory.Reset();
		BlackBoard.DontUpdate = true;
	}

	public void AddGOAPAction(E_GOAPAction action)
	{
		Actions.Add(action, GOAPActionFactory.Create(action, this));
	}

	public void AddGOAPGoal(E_GOAPGoals goal)
	{
		GoalManager.AddGoal(goal);
	}

	public GOAPGoal GetGOAPGoal(E_GOAPGoals goal)
	{
		if (GoalManager == null)
		{
			return null;
		}
		return GoalManager.GetGoal(goal);
	}

	public void InitializeGOAP()
	{
		GoalManager.Initialize();
	}

	public bool IsFriend(AgentHuman friend)
	{
		if (friend == null)
		{
			return false;
		}
		return friend.IsPlayer == IsPlayer || !friend.IsPlayer == !IsPlayer;
	}

	public void OnProjectileHit(Projectile projectile)
	{
		if (!IsAlive)
		{
			return;
		}
		if (IsFriend(projectile.Agent))
		{
			projectile.ignoreThisHit = true;
		}
		else if (BlackBoard.Invulnerable)
		{
			if ((bool)projectile.Agent)
			{
				AddFactToMemory(E_EventTypes.EnemyInjuredMe, projectile.Agent, 1f, Random.Range(0.1f, 0.3f));
			}
			BlackBoard.AbsorbedDamage += projectile.Damage;
			projectile.ricochetThisHit = true;
		}
		else
		{
			TakeDamage(projectile.Agent, projectile.Damage, projectile.Transform.forward * projectile.Impuls, projectile.Weapon);
			if ((bool)BloodFXManager.Instance && IsPlayer)
			{
				BloodFXManager.Instance.SpawnBloodDrops((uint)Random.Range(1, 4));
			}
		}
	}

	public override void OnReceiveRangeDamage(Agent attacker, float damage, Vector3 impuls, E_WeaponType weaponType)
	{
		if (!IsAlive)
		{
			return;
		}
		AgentHuman friend = attacker as AgentHuman;
		if (IsFriend(friend))
		{
			return;
		}
		if (BlackBoard.Invulnerable)
		{
			if ((bool)attacker && attacker is AgentHuman)
			{
				AddFactToMemory(E_EventTypes.EnemyInjuredMe, attacker as AgentHuman, 1f, Random.Range(0.1f, 0.3f));
			}
			BlackBoard.AbsorbedDamage += damage;
		}
		else
		{
			TakeDamage(attacker, damage, impuls, weaponType);
			if ((bool)BloodFXManager.Instance && IsPlayer)
			{
				BloodFXManager.Instance.SpawnBloodDrops((uint)Random.Range(4, 6));
			}
		}
	}

	public void OnReceiveEnviromentDamage(float damage, Vector3 impuls)
	{
		if (IsAlive)
		{
			TakeDamage(null, damage, impuls, E_WeaponType.None);
			CombatEffectsManager.Instance.PlayHitEffect(base.GameObject, ChestPosition, impuls);
			if ((bool)BloodFXManager.Instance && IsPlayer)
			{
				BloodFXManager.Instance.SpawnBloodDrops((uint)Random.Range(1, 4));
			}
		}
	}

	internal void TakeDamage(Agent inAttacker, float inDamage, Vector3 inImpuls, E_WeaponType inWeaponType)
	{
		if (Game.Instance.GameDifficulty == E_GameDifficulty.Easy)
		{
			if ((bool)inAttacker && inAttacker.IsPlayer)
			{
				inDamage *= 2.5f;
			}
			else if (IsPlayer)
			{
				inDamage *= 0.5f;
			}
		}
		else if (Game.Instance.GameDifficulty == E_GameDifficulty.Hard)
		{
			if ((bool)inAttacker && inAttacker.IsPlayer)
			{
				inDamage *= 0.8f;
			}
			else if (IsPlayer)
			{
				inDamage *= 1.5f;
			}
		}
		if (inWeaponType == E_WeaponType.PlasmaRifle)
		{
			inDamage *= m_PlasmaDamageMultiplier;
		}
		AgentHuman agentHuman = inAttacker as AgentHuman;
		if (!IsInvulnerable)
		{
			BlackBoard.Health = Mathf.Max(0f, BlackBoard.Health - inDamage);
		}
		SpriteEffectsManager.Instance.CreateBlood(base.Transform.position + inImpuls * Random.Range(0.3f, 1.5f));
		if (IsAlive)
		{
			WorldState.SetWSProperty(E_PropKey.Event, E_EventTypes.EnemyInjuredMe);
			if (BlackBoard.ReactOnHits)
			{
				if (agentHuman != null)
				{
					AddFactToMemory(E_EventTypes.EnemyInjuredMe, agentHuman, 1f, Random.Range(0.1f, 0.3f));
				}
				if (Random.Range(0, 100) < 30)
				{
					SoundPlay(InjuriesSound);
				}
				AgentActionInjury agentActionInjury = AgentActionFactory.Create(AgentActionFactory.E_Type.Injury) as AgentActionInjury;
				agentActionInjury.Attacker = agentHuman;
				agentActionInjury.Damage = inDamage;
				agentActionInjury.FromWeapon = inWeaponType;
				agentActionInjury.Impuls = inImpuls;
				BlackBoard.ActionAdd(agentActionInjury);
			}
		}
		else
		{
			WorldState.SetWSProperty(E_PropKey.Event, E_EventTypes.Died);
			if (!IsPlayer)
			{
				StartCoroutine(Fadeout(2f));
			}
			BlackBoard.Desires.WeaponTriggerOn = false;
			AgentActionDeath agentActionDeath = AgentActionFactory.Create(AgentActionFactory.E_Type.Death) as AgentActionDeath;
			agentActionDeath.Attacker = agentHuman;
			agentActionDeath.FromWeapon = inWeaponType;
			agentActionDeath.Impuls = inImpuls;
			BlackBoard.ActionAdd(agentActionDeath);
			SoundPlay(DeathSound);
		}
	}

	public void PlayAnim(string animName)
	{
		if (animName != null)
		{
			BlackBoard.Desires.Animation = animName;
			WorldState.SetWSProperty(E_PropKey.PlayAnim, true);
		}
	}

	public void Teleport(Vector3 position, Quaternion rotation)
	{
		base.Transform.position = position;
		base.Transform.rotation = rotation;
		if ((bool)BlackBoard.Cover)
		{
			BlackBoard.Cover.LockPosition(BlackBoard.CoverPosition, null);
			WorldState.SetWSProperty(E_PropKey.CoverState, E_CoverState.None);
			BlackBoard.CoverPosition = E_CoverDirection.Unknown;
			BlackBoard.Cover = null;
			BlackBoard.ActionAdd(AgentActionFactory.Create(AgentActionFactory.E_Type.CoverTerminated));
		}
		BlackBoard.Desires.Reset();
		BlackBoard.Desires.Rotation = rotation;
		BlackBoard.Desires.FireDirection = rotation * Vector3.forward;
	}

	public void Teleport(Transform destination)
	{
		if ((bool)NavMeshAgent)
		{
			NavMeshAgent.enabled = false;
		}
		GoalManager.Reset();
		BlackBoard.Desires.Rotation = destination.rotation;
		BlackBoard.Desires.FireDirection = destination.rotation * Vector3.forward;
		BlackBoard.Desires.WeaponTriggerOn = false;
		BlackBoard.MoveDir = Vector3.zero;
		BlackBoard.FireDir = base.Transform.forward;
		BlackBoard.BusyAction = false;
		BlackBoard.InteractionObject = null;
		BlackBoard.DangerousEnemy = null;
		BlackBoard.Invulnerable = false;
		BlackBoard.ReactOnHits = true;
		if (BlackBoard.Cover != null)
		{
			BlackBoard.Cover.LockPosition(BlackBoard.CoverPosition, null);
			WorldState.SetWSProperty(E_PropKey.CoverState, E_CoverState.None);
			BlackBoard.CoverPosition = E_CoverDirection.Unknown;
			BlackBoard.Cover = null;
			BlackBoard.ActionAdd(AgentActionFactory.Create(AgentActionFactory.E_Type.CoverTerminated));
		}
		if ((bool)NavMeshAgent)
		{
			NavMeshAgent.enabled = true;
		}
		AnimComponent.OnTeleport();
		base.Transform.position = destination.position;
		base.Transform.rotation = destination.rotation;
	}

	protected IEnumerator Fadeout(float delay)
	{
		yield return new WaitForSeconds(2f);
		if (NavMeshAgent != null)
		{
			NavMeshAgent.enabled = false;
		}
		WeaponComponent.DropAmmo();
		yield return new WaitForSeconds(delay - 2f);
		if (FadeoutMaterial == null)
		{
			Mission.Instance.ReturnAgent(base.GameObject);
			yield break;
		}
		if ((bool)Renderer)
		{
			Renderer.material = FadeoutMaterial;
			Renderer.material.SetFloat("_TimeOffs", 0f - Time.time);
			Renderer.material.SetFloat("_Invert", 0f);
			Renderer.material.SetFloat("_Duration", FadeoutFXDuration);
		}
		yield return new WaitForSeconds(FadeoutFXDuration);
		Mission.Instance.ReturnAgent(base.GameObject);
	}

	public void TeleportFadeOut()
	{
		if ((bool)Renderer)
		{
			Renderer.material = FadeoutMaterial;
			Renderer.material.SetFloat("_TimeOffs", 0f - Time.time);
			Renderer.material.SetFloat("_Invert", 0f);
			Renderer.material.SetFloat("_Duration", 1f);
		}
	}

	public void TeleportFadeIn()
	{
		StartCoroutine(FadeIn());
	}

	protected IEnumerator FadeIn()
	{
		if ((bool)Renderer)
		{
			Renderer.material = FadeoutMaterial;
			Renderer.material.SetFloat("_TimeOffs", 0f - Time.time);
			Renderer.material.SetFloat("_Invert", 1f);
			Renderer.material.SetFloat("_Duration", 1f);
		}
		yield return new WaitForSeconds(1f);
		Renderer.material = DiffuseMaterial;
	}

	public void ResetMaterial()
	{
		if (Renderer == null)
		{
			Renderer = base.gameObject.GetComponentInChildren(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
		}
		Renderer.material = DiffuseMaterial;
	}

	public void SoundPlay(AudioClip clip)
	{
		if ((bool)clip)
		{
			base.Audio.PlayOneShot(clip);
		}
	}

	public void PlayLoopSound(AudioClip clip, float delay, float time, float fadeInTime, float fadeOutTime)
	{
		StartCoroutine(_PlayLoopSound(clip, delay, time, fadeInTime, fadeOutTime));
	}

	private IEnumerator _PlayLoopSound(AudioClip clip, float delay, float time, float fadeInTime, float fadeOutTime)
	{
		base.Audio.volume = 0f;
		base.Audio.loop = true;
		base.Audio.clip = clip;
		yield return new WaitForEndOfFrame();
		yield return new WaitForSeconds(delay);
		base.Audio.Play();
		float step = 1f / fadeInTime;
		while (base.Audio.volume < 1f)
		{
			base.Audio.volume = Mathf.Min(1f, base.Audio.volume + step * Time.deltaTime);
			yield return new WaitForEndOfFrame();
		}
		yield return new WaitForSeconds(time - fadeInTime - fadeOutTime);
		step = 1f / fadeInTime;
		while (base.Audio.volume > 0f)
		{
			base.Audio.volume = Mathf.Max(0f, base.Audio.volume - step * Time.deltaTime);
			yield return new WaitForEndOfFrame();
		}
		base.Audio.Stop();
		yield return new WaitForEndOfFrame();
		base.Audio.volume = 1f;
	}

	public void DisableCollisions()
	{
		if ((bool)CollisionController)
		{
			CollisionController.enabled = false;
		}
		if ((bool)CharacterController)
		{
			CharacterController.enabled = false;
		}
	}

	public void EnableCollisions()
	{
		if ((bool)CollisionController)
		{
			CollisionController.enabled = true;
		}
		if ((bool)CharacterController)
		{
			CharacterController.enabled = true;
		}
	}

	public void SetCoverCompromised(Cover cover)
	{
		Fact fact = FactsFactory.Create(E_EventTypes.CoverCompromised);
		fact.Cover = cover;
		fact.LiveTime = Random.Range(15, 25);
		fact.Agent = this;
		Memory.AddFact(fact);
		if (BlackBoard.Desires.CoverNear.Cover == cover)
		{
			BlackBoard.Desires.CoverNear.Reset();
		}
		if (BlackBoard.Desires.CoverAdvance.Cover == cover)
		{
			BlackBoard.Desires.CoverAdvance.Reset();
		}
		if (BlackBoard.Desires.CoverFallback.Cover == cover)
		{
			BlackBoard.Desires.CoverFallback.Reset();
		}
	}

	public void AddFactToMemory(E_EventTypes eventType, AgentHuman agent, float lifetime, float delay)
	{
		Fact fact = FactsFactory.Create(eventType);
		fact.Agent = agent;
		fact.Delay = delay;
		fact.LiveTime = lifetime;
		fact.Position = agent.Position;
		Memory.AddFact(fact);
		BlackBoard.UpdateCombatSetting(fact);
		WorldState.SetWSProperty(E_PropKey.Event, eventType);
	}

	public void AddFactToMemory(Fact fact)
	{
		Memory.AddFact(fact);
		BlackBoard.UpdateCombatSetting(fact);
		WorldState.SetWSProperty(E_PropKey.Event, fact.Type);
	}

	public bool CanFire()
	{
		if (BlackBoard.BusyAction)
		{
			return false;
		}
		if (BlackBoard.ComlinkOn)
		{
			return false;
		}
		if (WeaponComponent.GetCurrentWeapon().IsBusy())
		{
			return false;
		}
		if (WeaponComponent.GetCurrentWeapon().ClipAmmo == 0)
		{
			return false;
		}
		if ((bool)BlackBoard.Cover)
		{
			return BlackBoard.Cover.CanFire(this);
		}
		return true;
	}

	public E_Direction GetMoveDirectionType(Vector3 moveDirection)
	{
		if (BlackBoard.Desires.MoveDirection.sqrMagnitude < 0.1f)
		{
			return E_Direction.Forward;
		}
		Vector3 normalized = BlackBoard.Desires.MoveDirection.normalized;
		Vector2 vector = new Vector2(base.Transform.forward.x, base.Transform.forward.z);
		Vector2 vector2 = new Vector2(base.Transform.right.x, base.Transform.right.z);
		Vector2 to = new Vector2(normalized.x, normalized.z);
		float num = Vector2.Angle(vector, to);
		float num2 = Vector2.Angle(vector2, to);
		if (num <= 35f)
		{
			return E_Direction.Forward;
		}
		if (num > 145f)
		{
			return E_Direction.Backward;
		}
		if (num2 < 90f)
		{
			return E_Direction.Right;
		}
		return E_Direction.Left;
	}

	public bool CanProjectileHitTarget(AgentHuman target)
	{
		if (target == null)
		{
			return false;
		}
		Vector3 vector;
		if (IsInCover)
		{
			if (BlackBoard.CoverPosition == E_CoverDirection.Middle && !BlackBoard.Cover.CoverFlags.Get(Cover.E_CoverFlags.UpCrouch))
			{
				return false;
			}
			vector = ((BlackBoard.CoverPosition == E_CoverDirection.Left) ? BlackBoard.Cover.EyeLeftEdge : ((BlackBoard.CoverPosition != E_CoverDirection.Right) ? (base.Position + Vector3.up * 1.6f) : BlackBoard.Cover.EyeRightEdge));
		}
		else
		{
			vector = EyePosition;
		}
		Vector3 vector2 = ((!target.IsInCover) ? target.ChestPosition : ((target.BlackBoard.CoverPosition == E_CoverDirection.Left) ? target.BlackBoard.Cover.EyeLeftEdge : ((target.BlackBoard.CoverPosition != E_CoverDirection.Right) ? (target.Position + Vector3.up * 1.6f) : target.BlackBoard.Cover.EyeRightEdge)));
		Vector3 vector3 = vector2 - vector;
		RaycastHit[] array = Physics.RaycastAll(vector, vector3.normalized, vector3.magnitude);
		Debug.DrawLine(vector, vector2, Color.white, 3f);
		if (array.Length > 1)
		{
			Array.Sort(array, CollisionUtils.CompareHits);
		}
		RaycastHit[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			RaycastHit raycastHit = array2[i];
			if (!(raycastHit.transform == base.Transform) && !raycastHit.collider.isTrigger)
			{
				if (raycastHit.distance < BlackBoard.DistanceToTarget * 0.7f)
				{
					return false;
				}
				return true;
			}
		}
		return true;
	}
}
