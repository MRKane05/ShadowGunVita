using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animation))]
public class Sentinel : Agent, IProjectileHitReceiver
{
	public class InternalState<T> where T : IComparable
	{
		private T m_Act;

		private T m_Req;

		public T state
		{
			get
			{
				return m_Act;
			}
			set
			{
				m_Req = value;
			}
		}

		public T actual
		{
			get
			{
				return m_Act;
			}
		}

		public T requested
		{
			get
			{
				return m_Req;
			}
		}

		public bool isDifferent
		{
			get
			{
				return m_Act.CompareTo(m_Req) != 0;
			}
		}

		public InternalState(T inInitialActValue, T inInitialReqValue)
		{
			m_Act = inInitialActValue;
			m_Req = inInitialReqValue;
		}

		public void Flush()
		{
			m_Act = m_Req;
		}
	}

	[Serializable]
	public class AnimInfo
	{
		public AnimationClip Patrol;

		public AnimationClip Attack;

		public AnimationClip OnHit;

		public AnimationClip Destroy;
	}

	[Serializable]
	public class SoundInfo
	{
		public AudioClip Flying;

		public AudioClip Destroy;

		public AudioClip[] Fire = new AudioClip[0];
	}

	private enum E_State
	{
		None = 0,
		Goto = 1,
		Patrol = 2,
		Warning = 3,
		Attack = 4,
		Destroyed = 5
	}

	private InternalState<E_State> m_State = new InternalState<E_State>(E_State.None, E_State.None);

	private AgentHuman m_Target;

	private SentinelMove m_MoveComp;

	private SentinelWeapon m_WeaponComp;

	public float m_HitPoints = 300f;

	private float m_DefaultHitPoints;

	public float m_DetectionDistance = 16f;

	private float m_DefaultDetectionDistance;

	public float m_FireDistance = 16f;

	public float m_WarningTime = 0.5f;

	private float m_WarningInitTime;

	public AnimInfo m_AnimInfo;

	private Animation m_Animations;

	public SoundInfo m_SoundInfo;

	private AudioSource m_Audio;

	public Explosion m_Explosion;

	private static bool m_HACK_SpawnExplosionAnimEventWasSet = false;

	private static List<Sentinel> _ActiveSentinels = new List<Sentinel>();

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

	public override bool IsEnteringToCover
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
			return base.Position;
		}
	}

	public static List<Sentinel> activeSentinels
	{
		get
		{
			return _ActiveSentinels;
		}
	}

	private Vector3 dirToTarget
	{
		get
		{
			return base.Transform.position - m_Target.ChestPosition;
		}
	}

	public static void HACK_ClearStaticFields()
	{
		m_HACK_SpawnExplosionAnimEventWasSet = false;
	}

	private void Awake()
	{
		Initialize();
		m_DefaultHitPoints = m_HitPoints;
		m_DefaultDetectionDistance = m_DetectionDistance;
		m_Animations = base.GetComponent<Animation>();
		m_Animations.SyncLayer(0);
		if (m_Animations[m_AnimInfo.OnHit.name] != null)
		{
			m_Animations[m_AnimInfo.OnHit.name].layer = 1;
		}
		if (m_Animations[m_AnimInfo.Destroy.name] != null)
		{
			m_Animations[m_AnimInfo.Destroy.name].layer = 1;
			m_Animations[m_AnimInfo.Destroy.name].wrapMode = WrapMode.ClampForever;
			if (!m_HACK_SpawnExplosionAnimEventWasSet)
			{
				AnimationClip clip = m_Animations[m_AnimInfo.Destroy.name].clip;
				AnimationEvent animationEvent = new AnimationEvent();
				animationEvent.time = clip.length;
				animationEvent.functionName = "SpawnExplosion";
				animationEvent.messageOptions = SendMessageOptions.DontRequireReceiver;
				clip.AddEvent(animationEvent);
				m_HACK_SpawnExplosionAnimEventWasSet = true;
			}
		}
		m_Audio = base.GetComponent<AudioSource>();
		m_MoveComp = GetComponent<SentinelMove>();
		if (m_MoveComp == null)
		{
			Debug.LogError("Neccessary script missing - SentinelMove");
		}
		m_WeaponComp = GetComponent<SentinelWeapon>();
		if (m_WeaponComp == null)
		{
			Debug.LogError("Neccessary script missing - SentinelWeapon");
			return;
		}
		m_WeaponComp.detectionDistancet = m_DetectionDistance;
		m_WeaponComp.fireDistance = m_FireDistance;
	}

	private void Start()
	{
		if (m_Target == null)
		{
			m_Target = Player.Instance.Owner;
		}
	}

	private void Log()
	{
		UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(Explosion));
		Debug.Log("number of textures " + array.Length);
		for (int i = 0; i < array.Length; i++)
		{
			Debug.Log(array[i].name);
		}
	}

	private void Activate(SpawnPoint spawn)
	{
		base.Transform.position = spawn.Transform.position + Vector3.up;
		base.Transform.rotation = spawn.Transform.rotation;
		foreach (Transform item in base.Transform)
		{
			item.localRotation = Quaternion.identity;
		}
		m_WarningInitTime = float.MinValue;
		m_State.state = E_State.None;
		SpawnPointEnemy spawnPointEnemy = spawn as SpawnPointEnemy;
		if (spawnPointEnemy != null)
		{
			if (spawnPointEnemy.InitState == SpawnPointEnemy.E_InitState.Patrol)
			{
				m_State.state = E_State.Patrol;
			}
			else if (spawnPointEnemy.InitState == SpawnPointEnemy.E_InitState.Goto)
			{
				m_State.state = E_State.Goto;
			}
		}
		m_DetectionDistance = Mathf.Max(spawnPointEnemy.InitSightRange, m_DefaultDetectionDistance);
		m_WeaponComp.detectionDistancet = m_DetectionDistance;
		m_HitPoints = m_DefaultHitPoints;
		base.GetComponent<Animation>().playAutomatically = false;
		if (m_Audio != null && m_SoundInfo.Flying != null)
		{
			m_Audio.Stop();
			m_Audio.clip = m_SoundInfo.Flying;
			m_Audio.loop = true;
			m_Audio.Play();
		}
	}

	private void Deactivate()
	{
		base.GetComponent<Animation>().Stop();
		base.GetComponent<Animation>().playAutomatically = false;
		m_Audio.Stop();
		m_State.state = E_State.None;
		m_State.Flush();
		m_HitPoints = 0f;
		StopAllCoroutines();
		CancelInvoke();
	}

	private void OnEnable()
	{
		if (!_ActiveSentinels.Contains(this))
		{
			_ActiveSentinels.Add(this);
		}
	}

	private void OnDisable()
	{
		_ActiveSentinels.Remove(this);
	}

	private void OnDestroy()
	{
		_ActiveSentinels.Remove(this);
		StopAllCoroutines();
		CancelInvoke();
	}

	public override void OnShootSound(AgentHuman inAgent)
	{
		if (!(m_HitPoints <= 0f) && m_State.state == E_State.Patrol)
		{
			m_State.state = E_State.Warning;
		}
	}

	public override void OnReceiveRangeDamage(Agent attacker, float damage, Vector3 impuls, E_WeaponType weaponType)
	{
		Takedamage(damage);
	}

	public void OnProjectileHit(Projectile projectile)
	{
		Takedamage(projectile.Damage);
	}

	internal void Takedamage(float inDamage)
	{
		if (m_HitPoints <= 0f)
		{
			return;
		}
		m_HitPoints -= inDamage;
		if (m_HitPoints > 0f)
		{
			if (m_State.state == E_State.Patrol)
			{
				m_State.state = E_State.Warning;
			}
			m_Animations.CrossFade(m_AnimInfo.OnHit.name);
			return;
		}
		m_State.state = E_State.Destroyed;
		if (m_AnimInfo.Destroy.name != null && m_Animations[m_AnimInfo.Destroy.name] != null)
		{
			m_Animations.CrossFade(m_AnimInfo.Destroy.name, 0.3f, PlayMode.StopAll);
			if (m_SoundInfo.Destroy != null)
			{
				m_Audio.Stop();
				m_Audio.clip = m_SoundInfo.Destroy;
				m_Audio.loop = false;
				m_Audio.Play();
			}
		}
		else
		{
			SpawnExplosion();
		}
	}

	public void PlayFireSound()
	{
		if (m_SoundInfo.Fire.Length > 0)
		{
			AudioClip clip = m_SoundInfo.Fire[Random.Range(0, m_SoundInfo.Fire.Length)];
			base.GetComponent<AudioSource>().PlayOneShot(clip);
		}
	}

	public void SetPatrolAnim()
	{
		if (!m_Animations.IsPlaying(m_AnimInfo.Patrol.name))
		{
			m_Animations.CrossFade(m_AnimInfo.Patrol.name);
		}
	}

	public void SetAttackAnim()
	{
		if (!m_Animations.IsPlaying(m_AnimInfo.Attack.name))
		{
			m_Animations.CrossFade(m_AnimInfo.Attack.name);
		}
	}

	private void SpawnExplosion()
	{
		CancelInvoke("SpawnExplosion");
		if (m_Explosion != null)
		{
			Mission.Instance.ExplosionCache.Get(m_Explosion, base.transform.position, base.transform.rotation);
		}
		base.GetComponent<Animation>().Stop();
		base.GetComponent<Animation>().playAutomatically = false;
		Invoke("ReturnToCache_Invoke", 0.001f);
	}

	private void ReturnToCache_Invoke()
	{
		Mission.Instance.ReturnAgent(base.gameObject);
	}

	private void UpdateState()
	{
		if (m_State.isDifferent)
		{
			switch (m_State.requested)
			{
			case E_State.Patrol:
				m_WeaponComp.enabled = false;
				m_MoveComp.enabled = true;
				m_MoveComp.moveMode = SentinelMove.MoveMode.Patrol;
				break;
			case E_State.Goto:
				m_WeaponComp.enabled = false;
				m_MoveComp.enabled = true;
				m_MoveComp.moveMode = SentinelMove.MoveMode.Goto;
				break;
			case E_State.Warning:
				m_WarningInitTime = Time.realtimeSinceStartup;
				m_WeaponComp.enabled = true;
				m_MoveComp.enabled = true;
				m_MoveComp.moveMode = SentinelMove.MoveMode.Warning;
				break;
			case E_State.Attack:
				m_WeaponComp.enabled = true;
				m_MoveComp.enabled = true;
				m_MoveComp.moveMode = SentinelMove.MoveMode.Attack;
				break;
			case E_State.None:
				m_WeaponComp.enabled = false;
				m_MoveComp.enabled = false;
				m_MoveComp.moveMode = SentinelMove.MoveMode.None;
				break;
			case E_State.Destroyed:
				m_WeaponComp.enabled = false;
				m_MoveComp.enabled = false;
				m_MoveComp.moveMode = SentinelMove.MoveMode.Destroyed;
				break;
			default:
				Debug.LogError("Sentinel : Unknown internal state  " + m_State.requested);
				break;
			}
			m_State.Flush();
		}
	}

	private void Update()
	{
		if (Time.deltaTime <= float.Epsilon)
		{
			return;
		}
		if (m_State.state != E_State.Destroyed && !m_State.isDifferent)
		{
			if (m_State.state == E_State.Goto)
			{
				if (m_MoveComp.IsOnGotoTargetPos_Hack())
				{
					m_State.state = E_State.Patrol;
				}
			}
			else if ((m_Target == null && m_State.state != E_State.Patrol) || m_State.state == E_State.None)
			{
				m_State.state = E_State.Patrol;
			}
			else if (m_State.state != E_State.Attack && m_State.state != E_State.Warning)
			{
				if (m_Target != null)
				{
					Vector3 vector = m_Target.transform.position - base.transform.position;
					vector.y = 0f;
					if (vector.magnitude < m_DetectionDistance)
					{
						m_State.state = ((!(m_WarningTime > 0f)) ? E_State.Attack : E_State.Warning);
					}
				}
			}
			else if (m_State.state == E_State.Warning && Time.realtimeSinceStartup - m_WarningInitTime > m_WarningTime)
			{
				m_State.state = E_State.Attack;
			}
		}
		UpdateState();
	}
}
