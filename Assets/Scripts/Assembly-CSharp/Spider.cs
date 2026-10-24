using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animation))]
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class Spider : Agent, IProjectileHitReceiver
{
	[Serializable]
	public class AnimInfo
	{
		public AnimationClip Idle;

		public AnimationClip Move;
	}

	[Serializable]
	public class SoundInfo
	{
		public AudioClip Moving;

		public AudioClip Activate;
	}

	public enum State
	{
		None = 0,
		Spawning = 1,
		Waiting = 2,
		Attack = 3,
		Destroyed = 4
	}

	private State m_State;

	private AgentHuman m_Target;

	public float m_HitPoints = 50f;

	private float m_DefaultHitPoints;

	[NonSerialized]
	public float m_DetectionDistance = 500f;

	public float m_ActivationDistance = 5f;

	public float m_ExplosionTimeout = 5f;

	public Explosion m_Explosion;

	public Vector3 m_ExplosionOffset = Vector3.zero;

	public AnimInfo m_AnimInfo;

	public SoundInfo m_SoundInfo;

	private NavMeshAgent m_NavMeshAgent;

	private Vector3 m_MoveTargetPos;

	private float m_AgentRadius = 0.5f;

	private Animation m_Animations;

	private AudioSource m_Audio;

	private static string m_SpawnAnimName = "SpawnAnim";

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

	private void Awake()
	{
		Initialize();
		m_DefaultHitPoints = m_HitPoints;
		m_DetectionDistance = 500f;
		m_NavMeshAgent = GetComponentInChildren<NavMeshAgent>();
		m_Animations = base.GetComponent<Animation>();
		m_Animations.SyncLayer(0);
		m_Audio = base.GetComponent<AudioSource>();
		if (m_SoundInfo.Moving != null)
		{
			m_Audio.clip = m_SoundInfo.Moving;
			m_Audio.loop = true;
		}
	}

	private void Start()
	{
		if (m_Target == null)
		{
			m_Target = Player.Instance.Owner;
		}
		if (Game.Instance.GameDifficulty == E_GameDifficulty.Easy)
		{
			m_NavMeshAgent.speed *= 0.8f;
		}
		else if (Game.Instance.GameDifficulty == E_GameDifficulty.Hard)
		{
			m_NavMeshAgent.speed *= 1.2f;
		}
	}

	private void Activate(SpawnPoint spawn)
	{
		base.Transform.position = spawn.Transform.position;
		base.Transform.rotation = spawn.Transform.rotation;
		m_HitPoints = m_DefaultHitPoints;
		m_Animations.playAutomatically = false;
		SpawnPointEnemy spawnPointEnemy = spawn as SpawnPointEnemy;
		if (spawnPointEnemy != null && spawnPointEnemy.InitState == SpawnPointEnemy.E_InitState.PlayAnim)
		{
			if (base.GetComponent<Animation>().GetClip(m_SpawnAnimName) == null)
			{
				base.GetComponent<Animation>().AddClip(spawnPointEnemy.SpawnAnimation, m_SpawnAnimName);
			}
			m_Animations.Play(m_SpawnAnimName);
			m_State = State.Spawning;
		}
		else
		{
			m_State = State.Waiting;
		}
	}

	private void Deactivate()
	{
		base.GetComponent<AudioSource>().Stop();
		base.GetComponent<Animation>().Stop();
		base.GetComponent<Animation>().playAutomatically = false;
		m_NavMeshAgent.enabled = false;
		m_State = State.None;
		m_HitPoints = 0f;
		StopAllCoroutines();
		CancelInvoke();
	}

	private void Update()
	{
		if (m_Target == null || Time.deltaTime <= float.Epsilon)
		{
			return;
		}
		Vector3 position = m_Target.transform.position;
		Vector3 vector = position - base.transform.position;
		vector.y = 0f;
		if (m_State == State.Spawning)
		{
			if (!m_Animations[m_SpawnAnimName].enabled || !(m_Animations[m_SpawnAnimName].length - m_Animations[m_SpawnAnimName].time > 0.3f))
			{
				m_State = State.Waiting;
			}
		}
		else if (m_State == State.Waiting)
		{
			if (vector.magnitude - m_AgentRadius < m_DetectionDistance)
			{
				m_MoveTargetPos = position;
				m_NavMeshAgent.enabled = true;
				m_NavMeshAgent.SetDestination(m_MoveTargetPos);
				m_State = State.Attack;
			}
		}
		else
		{
			if (m_State != State.Attack)
			{
				return;
			}
			if (!IsInvoking("SpawnExplosion") && vector.magnitude - m_AgentRadius < m_ActivationDistance)
			{
				Invoke("SpawnExplosion", m_ExplosionTimeout);
				if (m_SoundInfo.Activate != null)
				{
					base.GetComponent<AudioSource>().PlayOneShot(m_SoundInfo.Activate);
				}
			}
			else if (vector.magnitude - m_AgentRadius < 0.5f)
			{
				SpawnExplosion(false);
			}
			if (m_State != State.Attack)
			{
				return;
			}
			Vector3 vector2 = position - m_MoveTargetPos;
			vector2.y = 0f;
			if (vector2.magnitude > 0.5f)
			{
				m_MoveTargetPos = position;
				m_NavMeshAgent.SetDestination(m_MoveTargetPos);
			}
			Vector3 velocity = m_NavMeshAgent.velocity;
			velocity.y = 0f;
			float magnitude = velocity.magnitude;
			if (magnitude > 0.1f)
			{
				if (!m_Audio.isPlaying)
				{
					m_Audio.Play();
				}
				if (!m_Animations.IsPlaying(m_AnimInfo.Move.name))
				{
					m_Animations.CrossFade(m_AnimInfo.Move.name);
				}
				return;
			}
			if (!m_Audio.isPlaying)
			{
				m_Audio.Stop();
			}
			if (m_AnimInfo.Idle == null)
			{
				m_Animations.Stop(m_AnimInfo.Move.name);
			}
			else if (!m_Animations.IsPlaying(m_AnimInfo.Idle.name))
			{
				m_Animations.CrossFade(m_AnimInfo.Idle.name);
			}
		}
	}

	private void OnDestroy()
	{
	}

	private void OnDrawGizmos()
	{
	}

	public override void OnReceiveRangeDamage(Agent attacker, float damage, Vector3 impuls, E_WeaponType weaponType)
	{
		if (m_HitPoints <= 0f || m_State == State.Destroyed || m_State == State.Spawning)
		{
			return;
		}
		m_HitPoints -= damage;
		if (m_HitPoints > 0f)
		{
			if (m_State != State.Attack)
			{
				m_MoveTargetPos = m_Target.transform.position;
				m_NavMeshAgent.enabled = true;
				m_NavMeshAgent.SetDestination(m_MoveTargetPos);
				m_State = State.Attack;
			}
		}
		else
		{
			SpawnExplosion(true);
		}
	}

	public void OnProjectileHit(Projectile projectile)
	{
		if (m_HitPoints <= 0f || m_State == State.Destroyed || m_State == State.Spawning)
		{
			return;
		}
		m_HitPoints -= projectile.Damage;
		if (m_HitPoints > 0f)
		{
			if (m_State != State.Attack)
			{
				m_MoveTargetPos = m_Target.transform.position;
				m_NavMeshAgent.enabled = true;
				m_NavMeshAgent.SetDestination(m_MoveTargetPos);
				m_State = State.Attack;
			}
		}
		else
		{
			SpawnExplosion(true);
		}
	}

	private void SpawnExplosion()
	{
		SpawnExplosion(false);
	}

	private void SpawnExplosion(bool inClearDamage)
	{
		CancelInvoke("SpawnExplosion");
		if (m_Explosion != null)
		{
			Explosion explosion = Mission.Instance.ExplosionCache.Get(m_Explosion, base.transform.position + m_ExplosionOffset, base.transform.rotation);
			if (inClearDamage)
			{
				explosion.damage = 0f;
			}
		}
		m_State = State.Destroyed;
		base.GetComponent<Animation>().Stop();
		base.GetComponent<Animation>().playAutomatically = false;
		Mission.Instance.ReturnAgent(base.gameObject);
	}
}
