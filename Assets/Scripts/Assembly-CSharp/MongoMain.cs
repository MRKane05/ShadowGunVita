#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[AddComponentMenu("Enemies/Mongo/Main")]
[RequireComponent(typeof(MongoWeapons))]
public class MongoMain : Agent, IGameZoneControledObject, IBossHealth, IHitZoneOwner
{
	[Serializable]
	public class SpawningWave
	{
		internal enum E_State
		{
			Inactive = 0,
			Active = 1,
			InProgress = 2,
			Done = 3
		}

		internal E_State m_State;

		private string m_Event;

		private GameEvents.E_State m_EventState;

		public SpawnZoneOnEvent m_SpawnZone;

		private string m_DoneEvent;

		private GameEvents.E_State m_DoneEventState;

		public bool isReadyForUse
		{
			get
			{
				return m_State == E_State.Active;
			}
		}

		public bool isInProgress
		{
			get
			{
				return m_State == E_State.InProgress;
			}
		}

		public bool isDone
		{
			get
			{
				return m_State == E_State.Done;
			}
		}

		public void Prepare()
		{
			DebugUtils.Assert(m_SpawnZone.GameEvents.Count == 1);
			m_Event = m_SpawnZone.GameEvents[0].Name;
			m_EventState = m_SpawnZone.GameEvents[0].State;
			DebugUtils.Assert(m_SpawnZone.GameEventsToSend.Count == 1);
			DebugUtils.Assert(m_SpawnZone.GameEventsToSend[0].Delay == 0f);
			m_DoneEvent = m_SpawnZone.GameEventsToSend[0].Name;
			m_DoneEventState = m_SpawnZone.GameEventsToSend[0].State;
		}

		public void Init()
		{
			GameBlackboard.Instance.GameEvents.AddEventChangeHandler(m_DoneEvent, AllEnemiesAreDeath_EventHandler);
			m_State = E_State.Active;
		}

		public void Reset()
		{
			m_SpawnZone.Reset();
			m_State = E_State.Active;
		}

		public void DeInit()
		{
			if (m_State != E_State.Inactive)
			{
				GameBlackboard.Instance.GameEvents.RemoveEventChangeHandler(m_DoneEvent, AllEnemiesAreDeath_EventHandler);
				m_State = E_State.Inactive;
			}
		}

		public void Activate_SpawnZone()
		{
			GameBlackboard.Instance.GameEvents.Update(m_Event, m_EventState);
			m_State = E_State.InProgress;
		}

		public void AllEnemiesAreDeath_EventHandler(string name, GameEvents.E_State state)
		{
			if (m_DoneEventState == state || m_State == E_State.InProgress)
			{
				m_State = E_State.Done;
			}
		}
	}

	[Serializable]
	public class AnimInfo
	{
		public AnimationClip Spawn;

		public AnimationClip Idle;

		public AnimationClip Howling;

		public AnimationClip Damage;

		public AnimationClip GunFire;

		public AnimationClip GrenadeLaunch;

		public AnimationClip RocketLaunch;

		public AnimationClip Destroy;
	}

	[Serializable]
	public class AudioInfo
	{
		public AudioClip SpawnSpeech;

		public AudioClip[] IdleSpeeches;

		public AudioClip HowlingSpeech;

		public AudioClip DamageSpeech;

		public AudioClip GunFireSpeech;

		public AudioClip GrenadeLaunchSpeech;

		public AudioClip RocketLaunchSpeech;

		public AudioClip DeathSpeech;

		public AudioClip IdleSpeechRandom
		{
			get
			{
				return (IdleSpeeches.Length > 0) ? IdleSpeeches[Random.Range(0, IdleSpeeches.Length)] : null;
			}
		}
	}

	private int m_CurrentBossFightPhase = -1;

	private float m_DefaultHitPoints;

	public float m_HitPoints = 1000f;

	public float m_2ndPhaseHitPoints = 500f;

	public float m_MinWaitTimeInPhase1 = 5f;

	public float m_MinWaitTimeInPhase2 = 5f;

	public float m_MaxWaitTimeInPhase1 = 10f;

	public float m_MaxWaitTimeInPhase2 = 10f;

	public bool m_TestAnims;

	public AnimInfo m_AnimInfo;

	private Animation m_Animations;

	private Transform m_AnimMixinRoot;

	public AudioInfo m_Speeches;

	private AudioSource m_Audio;

	private int m_CurrentWave;

	private int m_CurrentWavePhase;

	public List<SpawningWave> m_Phase1_Waves = new List<SpawningWave>();

	public List<SpawningWave> m_Phase2_Waves = new List<SpawningWave>();

	public GameEvent m_DeathEvent;

	public ParticleSystem[] m_DestroyParticles;

	private float m_DamageAnimStartTime;

	private MongoWeapons m_WeaponComp;

	private Transform m_DZHearth;

	private Transform m_DZHowling;

	private bool isPlayingHowlingAnim
	{
		get
		{
			return m_Animations.IsPlaying(m_AnimInfo.Howling.name);
		}
	}

	private bool isPlayingDamageAnim
	{
		get
		{
			return m_Animations.IsPlaying(m_AnimInfo.Damage.name);
		}
	}

	public override bool IsPlayer
	{
		get
		{
			return false;
		}
	}

	public override bool IsAlive
	{
		get
		{
			return m_HitPoints > 0f;
		}
	}

	public override bool IsVisible
	{
		get
		{
			return true;
		}
	}

	public override bool IsInvulnerable
	{
		get
		{
			return false;
		}
	}

	public override bool IsInCover
	{
		get
		{
			return false;
		}
	}

	public override Vector3 ChestPosition
	{
		get
		{
			return GetTargetPosForAutoAim();
		}
	}

	public override bool IsEnteringToCover
	{
		get
		{
			return false;
		}
	}

	public override float HealthPercent
	{
		get
		{
			return m_HitPoints / m_DefaultHitPoints;
		}
	}

	private void Awake()
	{
		Initialize();
		m_DefaultHitPoints = m_HitPoints;
		m_Animations = base.GetComponent<Animation>();
		CheckObjectValidity();
		SetupAnimations();
		m_Audio = base.GetComponent<AudioSource>();
		m_WeaponComp = GetComponent<MongoWeapons>();
		if (m_WeaponComp == null)
		{
			Debug.LogError("Neccessary script missing - MongoWeapons");
		}
		GameZone firstComponentUpward = base.gameObject.GetFirstComponentUpward<GameZone>();
		firstComponentUpward.RegisterControllableObject(this);
		foreach (SpawningWave phase1_Wave in m_Phase1_Waves)
		{
			phase1_Wave.Prepare();
		}
		foreach (SpawningWave phase2_Wave in m_Phase2_Waves)
		{
			phase2_Wave.Prepare();
		}
		m_DZHearth = base.transform.Find("mover/pelvis/Hearth");
		m_DZHowling = base.transform.Find("mover/pelvis/body/head/howling");
		base.gameObject.SetActiveRecursively(false);
	}

	private void Start()
	{
		if (m_DeathEvent.Name != string.Empty && !GameBlackboard.Instance.GameEvents.Exist(m_DeathEvent.Name))
		{
			Debug.LogError("MongoMain :: Death event is not registered in GlobalBlackboard");
			Debug.LogError("MongoMain :: Please add event " + m_DeathEvent.Name + " into Mission.GlobalBlackboard");
			m_DeathEvent = null;
		}
	}

	private void Update()
	{
	}

	public void Enable()
	{
		foreach (SpawningWave phase1_Wave in m_Phase1_Waves)
		{
			phase1_Wave.Init();
		}
		foreach (SpawningWave phase2_Wave in m_Phase2_Waves)
		{
			phase2_Wave.Init();
		}
		m_WeaponComp.enabled = false;
		m_DamageAnimStartTime = 0f;
		m_CurrentBossFightPhase = 0;
		m_CurrentWave = 0;
		m_CurrentWavePhase = 0;
		m_HitPoints = m_DefaultHitPoints;
		m_Animations.Stop();
		m_Animations.playAutomatically = false;
		m_AnimInfo.Spawn.SampleAnimation(base.gameObject, 0.01f);
		base.gameObject.SetActiveRecursively(true);
		GameZone firstComponentUpward = base.gameObject.GetFirstComponentUpward<GameZone>();
		firstComponentUpward.AddEnemy(this);
		StartCoroutine(UpdateState());
	}

	public void Disable()
	{
		foreach (SpawningWave phase1_Wave in m_Phase1_Waves)
		{
			phase1_Wave.DeInit();
		}
		foreach (SpawningWave phase2_Wave in m_Phase2_Waves)
		{
			phase2_Wave.DeInit();
		}
		m_Audio.Stop();
		m_Animations.Stop();
		m_Animations.playAutomatically = false;
		m_WeaponComp.enabled = false;
		m_HitPoints = 0f;
		StopAllCoroutines();
		CancelInvoke();
		base.gameObject.SetActiveRecursively(false);
	}

	public void Reset()
	{
		Disable();
		Enable();
	}

	private void Activate(SpawnPoint spawn)
	{
	}

	private void Deactivate()
	{
	}

	public void OnProjectileHit(Projectile inProjectile, HitZone inHitZone)
	{
		if (IsAlive && base.enabled && base.gameObject.active)
		{
			float inDamage = inProjectile.Damage * inHitZone.DamageModifier;
			TakeDamage(inProjectile.Agent, inDamage, inHitZone);
		}
	}

	public void OnReceiveRangeDamage(float inDamage, HitZone inHitZone)
	{
		if (IsAlive && base.enabled && base.gameObject.active)
		{
			float inDamage2 = inDamage * inHitZone.DamageModifier;
			TakeDamage(null, inDamage2, inHitZone);
		}
	}

	internal void TakeDamage(Agent inAttacker, float inDamage, HitZone inHitZone)
	{
		DebugUtils.Assert(IsAlive && base.enabled && base.gameObject.active);
		m_HitPoints -= inDamage;
		if (m_HitPoints < 0f)
		{
			StopAllCoroutines();
			CancelInvoke();
			StartCoroutine(Death());
		}
		else if (isPlayingHowlingAnim && inHitZone.name == "howling" && Time.timeSinceLevelLoad - m_DamageAnimStartTime > 1f && !m_Animations[m_AnimInfo.Damage.name].enabled)
		{
			m_DamageAnimStartTime = Time.timeSinceLevelLoad;
			StartCoroutine(PlayDamage());
		}
	}

	private Vector3 GetTargetPosForAutoAim()
	{
		if (isPlayingDamageAnim)
		{
			return m_DZHearth.position;
		}
		return m_DZHowling.position;
	}

	public void GunFireStart()
	{
		StartCoroutine(PlayGunFire());
	}

	public void GrenadeLaunchStart()
	{
		StartCoroutine(PlayGrenadeLaunch());
	}

	public void RocketLaunchStart()
	{
		StartCoroutine(PlayRocketLaunch());
	}

	private IEnumerator UpdateState()
	{
		if (m_TestAnims)
		{
			yield return StartCoroutine(TestAnims());
			yield break;
		}
		yield return StartCoroutine(Spawn());
		yield return StartCoroutine(Phase_1());
		yield return StartCoroutine(Phase_2());
		yield return StartCoroutine(Death());
	}

	private IEnumerator Spawn()
	{
		m_WeaponComp.enabled = true;
		yield return StartCoroutine(PlaySpawn());
		m_Animations.CrossFadeQueued(m_AnimInfo.Idle.name, 0.3f, QueueMode.PlayNow).AddMixingTransform(m_AnimMixinRoot);
		yield return StartCoroutine(SpawnReinforcementPhase(false));
	}

	private IEnumerator Phase_1()
	{
		m_CurrentBossFightPhase = 1;
		if (m_WeaponComp.IsFireing())
		{
			Debug.LogError("It is imposible weapon can't run in this moment");
		}
		m_CurrentWave = 0;
		while (m_HitPoints > m_2ndPhaseHitPoints)
		{
			yield return StartCoroutine(Fight_Phase_1());
			yield return StartCoroutine(NoFight(m_2ndPhaseHitPoints, m_MinWaitTimeInPhase1, m_MaxWaitTimeInPhase1));
		}
	}

	private IEnumerator Phase_2()
	{
		m_CurrentBossFightPhase = 2;
		if (m_WeaponComp.IsFireing())
		{
			Debug.LogError("It is imposible weapon can't run in this moment");
		}
		m_CurrentWave = 0;
		while (m_HitPoints > 0f)
		{
			yield return StartCoroutine(Fight_Phase_2());
			yield return StartCoroutine(NoFight(0f, m_MinWaitTimeInPhase2, m_MaxWaitTimeInPhase2));
		}
	}

	private IEnumerator Fight_Phase_1()
	{
		if (m_WeaponComp.IsFireing())
		{
			Debug.LogError("It is imposible weapon can't run in this moment");
		}
		m_WeaponComp.Phase1_StartFire();
		while (m_HitPoints > m_2ndPhaseHitPoints && m_WeaponComp.IsFireing())
		{
			yield return new WaitForSeconds(0.5f);
		}
	}

	private IEnumerator Fight_Phase_2()
	{
		if (m_WeaponComp.IsFireing())
		{
			Debug.LogError("It is imposible weapon can't run in this moment");
		}
		m_WeaponComp.Phase2_StartFire();
		while (m_HitPoints > 0f && m_WeaponComp.IsFireing())
		{
			yield return new WaitForSeconds(0.5f);
		}
	}

	private IEnumerator NoFight(float inHealthCheck, float inMinTime, float inMaxTime)
	{
		float endTimeMax = Time.timeSinceLevelLoad + inMaxTime;
		float endTimeMin = Time.timeSinceLevelLoad + inMinTime;
		if (Random.Range(0, 100) < 30 && !IsInvoking("PlayIdleSound"))
		{
			Invoke("PlayIdleSound", Random.Range(3.5f, 56.5f));
		}
		if (CanSpawnReinforcement())
		{
			yield return StartCoroutine(SpawnReinforcementPhase(true));
			SpawningWave wave = GetCurrentSpawningWave();
			while (m_HitPoints > inHealthCheck && !wave.isDone && endTimeMax > Time.timeSinceLevelLoad)
			{
				yield return new WaitForSeconds(0.5f);
			}
		}
		else
		{
			yield return StartCoroutine(SpawnReinforcementPhase(false));
		}
		while (m_HitPoints > inHealthCheck && endTimeMin > Time.timeSinceLevelLoad)
		{
			yield return new WaitForSeconds(0.5f);
		}
		CancelInvoke("PlayIdleSound");
	}

	private IEnumerator SpawnReinforcementPhase(bool inReinforcement)
	{
		yield return StartCoroutine(PlayHowling());
		if (inReinforcement)
		{
			SpawnReinforcement();
		}
		while (isPlayingDamageAnim)
		{
			yield return new WaitForSeconds(0.5f);
		}
	}

	private IEnumerator Death()
	{
		m_WeaponComp.enabled = false;
		m_CurrentBossFightPhase = 3;
		if (m_DeathEvent != null)
		{
			Mission.Instance.SendGameEvent(m_DeathEvent);
		}
		if ((bool)Mission.Instance && (bool)Mission.Instance.CurrentGameZone)
		{
			Mission.Instance.CurrentGameZone.CleanAllEnamies();
		}
		yield return StartCoroutine(PlayDeath());
		base.gameObject.SetActiveRecursively(false);
	}

	public void AnimNotify_PlayDestroyParticle(int inParticleIndex)
	{
		if (inParticleIndex < m_DestroyParticles.Length && m_DestroyParticles[inParticleIndex] != null)
		{
			ParticleSystem[] componentsInChildren = m_DestroyParticles[inParticleIndex].GetComponentsInChildren<ParticleSystem>();
			ParticleSystem[] array = componentsInChildren;
			foreach (ParticleSystem particleSystem in array)
			{
				particleSystem.Play();
			}
		}
		else
		{
			Debug.LogError("AnimNotify_PlayDestroyParticle : Destroy particle for index " + inParticleIndex + " is not defined !!");
		}
	}

	private bool CanSpawnReinforcement()
	{
		SpawningWave currentSpawningWave = GetCurrentSpawningWave();
		if (currentSpawningWave == null)
		{
			NextSpawningWave();
			currentSpawningWave = GetCurrentSpawningWave();
		}
		if (currentSpawningWave == null || currentSpawningWave.isInProgress)
		{
			return false;
		}
		if (currentSpawningWave.isDone)
		{
			currentSpawningWave.Reset();
			NextSpawningWave();
			currentSpawningWave = GetCurrentSpawningWave();
		}
		if (currentSpawningWave != null && currentSpawningWave.isReadyForUse)
		{
			return true;
		}
		return false;
	}

	private void SpawnReinforcement()
	{
		SpawningWave currentSpawningWave = GetCurrentSpawningWave();
		currentSpawningWave.Activate_SpawnZone();
	}

	private SpawningWave GetCurrentSpawningWave()
	{
		if (m_CurrentWavePhase == 1 && m_CurrentWave < m_Phase1_Waves.Count)
		{
			return m_Phase1_Waves[m_CurrentWave];
		}
		if (m_CurrentWavePhase == 2 && m_CurrentWave < m_Phase2_Waves.Count)
		{
			return m_Phase2_Waves[m_CurrentWave];
		}
		return null;
	}

	private void NextSpawningWave()
	{
		if (m_CurrentBossFightPhase != m_CurrentWavePhase)
		{
			m_CurrentWavePhase = m_CurrentBossFightPhase;
			m_CurrentWave = 0;
		}
		else if (m_CurrentWavePhase == 1)
		{
			m_CurrentWave = ++m_CurrentWave % m_Phase1_Waves.Count;
		}
		else if (m_CurrentWavePhase == 2)
		{
			m_CurrentWave = ++m_CurrentWave % m_Phase2_Waves.Count;
		}
	}

	private void SetupAnimations()
	{
		m_AnimMixinRoot = base.transform.Find("mover");
		m_Animations[m_AnimInfo.Idle.name].AddMixingTransform(m_AnimMixinRoot);
		m_Animations[m_AnimInfo.Idle.name].layer = 0;
		m_Animations[m_AnimInfo.Idle.name].wrapMode = WrapMode.Loop;
		m_Animations[m_AnimInfo.Spawn.name].AddMixingTransform(m_AnimMixinRoot);
		m_Animations[m_AnimInfo.Spawn.name].layer = 1;
		m_Animations[m_AnimInfo.Howling.name].AddMixingTransform(m_AnimMixinRoot);
		m_Animations[m_AnimInfo.Howling.name].layer = 1;
		m_Animations[m_AnimInfo.Damage.name].AddMixingTransform(m_AnimMixinRoot);
		m_Animations[m_AnimInfo.Damage.name].layer = 1;
		m_Animations[m_AnimInfo.GunFire.name].AddMixingTransform(m_AnimMixinRoot);
		m_Animations[m_AnimInfo.GunFire.name].layer = 1;
		m_Animations[m_AnimInfo.GrenadeLaunch.name].AddMixingTransform(m_AnimMixinRoot);
		m_Animations[m_AnimInfo.GrenadeLaunch.name].layer = 1;
		m_Animations[m_AnimInfo.RocketLaunch.name].AddMixingTransform(m_AnimMixinRoot);
		m_Animations[m_AnimInfo.RocketLaunch.name].layer = 1;
		m_Animations[m_AnimInfo.Destroy.name].AddMixingTransform(m_AnimMixinRoot);
		m_Animations[m_AnimInfo.Destroy.name].layer = 1;
		m_Animations[m_AnimInfo.Destroy.name].wrapMode = WrapMode.ClampForever;
	}

	private IEnumerator CrossFadeAnim(string inAnimName)
	{
		m_Animations.CrossFadeQueued(inAnimName, 0.3f, QueueMode.PlayNow).AddMixingTransform(m_AnimMixinRoot);
		yield return new WaitForSeconds(m_Animations[inAnimName].length - 0.3f);
	}

	private IEnumerator PlaySpawn()
	{
		if (m_Speeches.SpawnSpeech != null)
		{
			m_Audio.PlayOneShot(m_Speeches.SpawnSpeech);
		}
		m_Animations.Play(m_AnimInfo.Spawn.name);
		yield return new WaitForSeconds(m_Animations[m_AnimInfo.Spawn.name].length - 0.3f);
	}

	private void PlayIdleSound()
	{
		if (m_Speeches.IdleSpeeches.Length > 0)
		{
			m_Audio.PlayOneShot(m_Speeches.IdleSpeechRandom);
		}
	}

	private IEnumerator PlayIdle()
	{
		PlayIdleSound();
		return CrossFadeAnim(m_AnimInfo.Idle.name);
	}

	private IEnumerator PlayHowling()
	{
		if (m_Speeches.HowlingSpeech != null)
		{
			m_Audio.PlayOneShot(m_Speeches.HowlingSpeech);
		}
		return CrossFadeAnim(m_AnimInfo.Howling.name);
	}

	private IEnumerator PlayDamage()
	{
		if (m_Speeches.DamageSpeech != null)
		{
			m_Audio.PlayOneShot(m_Speeches.DamageSpeech);
		}
		return CrossFadeAnim(m_AnimInfo.Damage.name);
	}

	private IEnumerator PlayGunFire()
	{
		if (m_Speeches.GunFireSpeech != null)
		{
			m_Audio.PlayOneShot(m_Speeches.GunFireSpeech);
		}
		return CrossFadeAnim(m_AnimInfo.GunFire.name);
	}

	private IEnumerator PlayGrenadeLaunch()
	{
		if (m_Speeches.GrenadeLaunchSpeech != null)
		{
			m_Audio.PlayOneShot(m_Speeches.GrenadeLaunchSpeech);
		}
		return CrossFadeAnim(m_AnimInfo.GrenadeLaunch.name);
	}

	private IEnumerator PlayRocketLaunch()
	{
		if (m_Speeches.RocketLaunchSpeech != null)
		{
			m_Audio.PlayOneShot(m_Speeches.RocketLaunchSpeech);
		}
		return CrossFadeAnim(m_AnimInfo.RocketLaunch.name);
	}

	private IEnumerator PlayDeath()
	{
		if (m_Speeches.DeathSpeech != null)
		{
			m_Audio.PlayOneShot(m_Speeches.DeathSpeech);
		}
		m_Animations.CrossFadeQueued(m_AnimInfo.Destroy.name, 1f, QueueMode.PlayNow).AddMixingTransform(m_AnimMixinRoot);
		yield return new WaitForSeconds(m_Animations[m_AnimInfo.Destroy.name].length - 0.3f);
	}

	private IEnumerator TestAnims()
	{
		yield return new WaitForSeconds(2f);
		yield return StartCoroutine(PlaySpawn());
		yield return StartCoroutine(PlayIdle());
		yield return StartCoroutine(PlayHowling());
		yield return StartCoroutine(PlayDamage());
		yield return StartCoroutine(PlayGunFire());
		yield return StartCoroutine(PlayGrenadeLaunch());
		yield return StartCoroutine(PlayRocketLaunch());
		yield return StartCoroutine(PlayDeath());
	}

	private bool CheckObjectValidity()
	{
		if (m_Animations == null)
		{
			Debug.LogError("MongoMain : Missing animation component !!!");
			return false;
		}
		if (m_AnimInfo.Spawn == null || m_Animations[m_AnimInfo.Spawn.name] == null)
		{
			Debug.LogError("MongoMain : Missing AnimationClip 'spawn' !!!");
			return false;
		}
		if (m_AnimInfo.Idle == null || m_Animations[m_AnimInfo.Idle.name] == null)
		{
			Debug.LogError("MongoMain : Missing AnimationClip 'idle' !!!");
			return false;
		}
		if (m_AnimInfo.Howling == null || m_Animations[m_AnimInfo.Howling.name] == null)
		{
			Debug.LogError("MongoMain : Missing AnimationClip 'Howling' !!!");
			return false;
		}
		if (m_AnimInfo.Damage == null || m_Animations[m_AnimInfo.Damage.name] == null)
		{
			Debug.LogError("MongoMain : Missing AnimationClip 'Damage' !!!");
			return false;
		}
		if (m_AnimInfo.GunFire == null || m_Animations[m_AnimInfo.GunFire.name] == null)
		{
			Debug.LogError("MongoMain : Missing AnimationClip 'GunFire' !!!");
			return false;
		}
		if (m_AnimInfo.GrenadeLaunch == null || m_Animations[m_AnimInfo.GrenadeLaunch.name] == null)
		{
			Debug.LogError("MongoMain : Missing AnimationClip 'GrenadeLaunch' !!!");
			return false;
		}
		if (m_AnimInfo.RocketLaunch == null || m_Animations[m_AnimInfo.RocketLaunch.name] == null)
		{
			Debug.LogError("MongoMain : Missing AnimationClip 'RocketLaunch' !!!");
			return false;
		}
		if (m_AnimInfo.Destroy == null || m_Animations[m_AnimInfo.Destroy.name] == null)
		{
			Debug.LogError("MongoMain : Missing AnimationClip 'Destroy' !!!");
			return false;
		}
		return true;
	}
}
