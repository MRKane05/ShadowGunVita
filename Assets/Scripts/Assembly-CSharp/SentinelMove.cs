using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
public class SentinelMove : MonoBehaviour
{
	[Serializable]
	public class AgentSettings
	{
		public float m_Speed = 5f;

		public float m_Acceleration = 10f;

		public float m_AngularSpeed = 120f;

		public float m_RotSpeed = 120f;

		public float m_MinEndDistance = 0.5f;
	}

	[Serializable]
	public class PatrolSettings
	{
		public enum PatrolMode
		{
			Subsequently = 0,
			Random = 1
		}

		public PatrolMode m_PatrolMode;

		public float m_AgentSpeed = 5f;

		public float m_AgentAcceleration = 10f;

		public float m_AgentAngularSpeed = 120f;

		public float m_RotSpeed = 120f;

		public float m_MinEndDistance = 0.5f;

		public float m_MinPause = 0.5f;

		public float m_MaxPause = 2.5f;

		public GameObject[] m_PatrolPoints;

		internal int m_CurrentPosIdx = -1;

		private float m_DontMoveUntil;

		internal bool m_Moving;

		public float getPause
		{
			get
			{
				return Random.Range(m_MinPause, m_MaxPause);
			}
		}

		public Vector3 getCurrPos
		{
			get
			{
				return (m_CurrentPosIdx >= 0) ? m_PatrolPoints[m_CurrentPosIdx].transform.position : Vector3.zero;
			}
		}

		public Vector3 getNextPos
		{
			get
			{
				if (m_PatrolPoints == null || m_PatrolPoints.Length == 0)
				{
					m_CurrentPosIdx = -1;
				}
				else if (m_PatrolPoints.Length == 1)
				{
					m_CurrentPosIdx = 0;
				}
				else if (m_PatrolMode == PatrolMode.Subsequently)
				{
					m_CurrentPosIdx = ++m_CurrentPosIdx % m_PatrolPoints.Length;
				}
				else if (m_PatrolMode == PatrolMode.Random)
				{
					m_CurrentPosIdx = Random.Range(0, m_PatrolPoints.Length);
				}
				return getCurrPos;
			}
		}

		public bool isWaiting
		{
			get
			{
				return Time.timeSinceLevelLoad < m_DontMoveUntil;
			}
		}

		public bool isMoving
		{
			get
			{
				return m_Moving;
			}
			set
			{
				m_Moving = value;
			}
		}

		public bool wantMove
		{
			get
			{
				return m_PatrolPoints != null && m_PatrolPoints.Length > 0 && !isWaiting;
			}
		}

		public void Wait()
		{
			m_DontMoveUntil = Time.timeSinceLevelLoad + getPause;
			isMoving = false;
		}

		public bool IsOnTargetPos(Vector3 inActualPos)
		{
			return m_MinEndDistance > Vector3.Distance(inActualPos, getCurrPos);
		}
	}

	[Serializable]
	public class AttackSettings
	{
		public float m_AgentSpeed = 10f;

		public float m_AgentAcceleration = 15f;

		public float m_AgentAngularSpeed = 180f;

		public float m_RotSpeed = 180f;

		public float m_MoveWaitTime = 2f;

		public float m_ActiveZoneMin = 3f;

		public float m_ActiveZoneMax = 7f;

		internal Vector3 m_MoveTargetPos;

		internal bool m_IsMoving;

		internal float m_LastMoveTime;

		public float dbg_getStopMoveDistance
		{
			get
			{
				return (m_ActiveZoneMax + m_ActiveZoneMin) * 0.4f;
			}
		}
	}

	[Serializable]
	public class DbgMoveSettings
	{
		internal Vector3 m_StartPos;

		internal int m_CurrentPosIdx = -1;

		internal Vector3[] m_TargetPos = new Vector3[4];

		internal bool m_IsMoving;

		internal float m_LastMoveTime;

		public float m_RotSpeed = 120f;

		public float m_MaxSpeed = 5f;

		public float m_ActSpeed;

		public float m_Acceleration = 2f;

		public Vector3 getCurrPos
		{
			get
			{
				return (m_CurrentPosIdx >= 0) ? m_TargetPos[m_CurrentPosIdx] : Vector3.zero;
			}
		}

		public void NextPos()
		{
			if (m_TargetPos == null || m_TargetPos.Length == 0)
			{
				m_CurrentPosIdx = -1;
			}
			else if (m_TargetPos.Length == 1)
			{
				m_CurrentPosIdx = 0;
			}
			else
			{
				m_CurrentPosIdx = ++m_CurrentPosIdx % m_TargetPos.Length;
			}
		}
	}

	[Serializable]
	public class GotoSettings
	{
		internal Vector3 m_TargetPos = Vector3.zero;

		internal Vector3 m_TargetDir = Vector3.zero;

		public float m_RotSpeed = 120f;

		public float m_MaxSpeed = 5f;

		public float m_ActSpeed;

		public float m_Acceleration = 10f;

		public Vector3 getTargetPos
		{
			get
			{
				return m_TargetPos;
			}
		}
	}

	public enum MoveMode
	{
		None = 0,
		DebugMove = 1,
		Goto = 2,
		Patrol = 3,
		MoveNear = 4,
		FireingMove = 5,
		Warning = 6,
		Attack = 7,
		Destroyed = 8
	}

	private MoveMode m_ReqMoveMode;

	private MoveMode m_ActMoveMode;

	private MoveMode m_DbgMoveMode;

	private Sentinel m_Sentinel;

	public GameObject m_Mover;

	public PatrolSettings m_PatrolSettings;

	public AttackSettings m_AttackSettings;

	private DbgMoveSettings m_DbgMoveSettings = new DbgMoveSettings();

	public GotoSettings m_GotoSettings = new GotoSettings();

	private NavMeshAgent m_NavMeshAgent;

	private AgentHuman m_Target;

	private Vector3 m_LastVelocity = Vector3.zero;

	private bool m_IsRotatingToTarget;

	private Vector3Smoothing m_VelocitySmoother = new Vector3Smoothing(6);

	public MoveMode moveMode
	{
		get
		{
			return m_ActMoveMode;
		}
		set
		{
			m_ReqMoveMode = value;
		}
	}

	public bool isMoveModeDifferent
	{
		get
		{
			return m_ActMoveMode != m_ReqMoveMode;
		}
	}

	private void Awake()
	{
		if (m_Mover == null)
		{
			m_Mover = base.gameObject;
		}
		m_NavMeshAgent = m_Mover.GetComponentInChildren<NavMeshAgent>();
		m_NavMeshAgent.updateRotation = false;
		m_Sentinel = GetComponent<Sentinel>();
		if (m_Sentinel == null)
		{
			Debug.LogError("Neccessary script missing - Sentinel");
		}
	}

	private void Start()
	{
		if (m_Target == null)
		{
			m_Target = Player.Instance.Owner;
		}
	}

	private void Activate(SpawnPoint spawn)
	{
		SpawnPointEnemy spawnPointEnemy = spawn as SpawnPointEnemy;
		if (spawnPointEnemy != null && (spawnPointEnemy.InitState == SpawnPointEnemy.E_InitState.Patrol || spawnPointEnemy.InitState == SpawnPointEnemy.E_InitState.Goto))
		{
			m_PatrolSettings.m_PatrolPoints = spawnPointEnemy.PatrolPoints;
		}
	}

	private void Deactivate()
	{
		m_PatrolSettings.m_PatrolPoints = null;
		m_PatrolSettings.m_CurrentPosIdx = -1;
		m_PatrolSettings.m_Moving = false;
		m_AttackSettings.m_MoveTargetPos = Vector3.zero;
		m_AttackSettings.m_IsMoving = false;
		m_AttackSettings.m_LastMoveTime = 0f;
		m_ReqMoveMode = MoveMode.None;
		m_ActMoveMode = MoveMode.None;
		m_DbgMoveMode = MoveMode.None;
		m_LastVelocity = Vector3.zero;
		m_IsRotatingToTarget = false;
		m_NavMeshAgent.enabled = false;
		StopAllCoroutines();
		CancelInvoke();
	}

	private void Update()
	{
		if (!(Time.deltaTime <= float.Epsilon))
		{
			if (m_DbgMoveMode != MoveMode.None)
			{
				UpdateMove(m_DbgMoveMode);
			}
			else
			{
				UpdateMove(m_ReqMoveMode);
			}
		}
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
		CancelInvoke();
	}

	public bool IsOnGotoTargetPos_Hack()
	{
		if (m_ActMoveMode == MoveMode.Goto)
		{
			return (m_GotoSettings.getTargetPos - m_Mover.transform.position).magnitude <= 0.1f;
		}
		if (base.enabled && m_ReqMoveMode == MoveMode.Goto)
		{
			return false;
		}
		return true;
	}

	private void PatrolMove_Init()
	{
		m_NavMeshAgent.enabled = true;
		m_NavMeshAgent.updateRotation = false;
		m_NavMeshAgent.speed = m_PatrolSettings.m_AgentSpeed;
		m_NavMeshAgent.angularSpeed = m_PatrolSettings.m_AgentAngularSpeed;
		m_NavMeshAgent.acceleration = m_PatrolSettings.m_AgentAcceleration;
		m_Sentinel.SetPatrolAnim();
		m_VelocitySmoother.Reset();
		Vector3 getNextPos = m_PatrolSettings.getNextPos;
		if (getNextPos != Vector3.zero && m_PatrolSettings.IsOnTargetPos(m_Mover.transform.position))
		{
			m_PatrolSettings.Wait();
		}
	}

	private void PatrolMove_Update()
	{
		if (m_PatrolSettings.wantMove)
		{
			if (!m_PatrolSettings.isMoving)
			{
				Vector3 getNextPos = m_PatrolSettings.getNextPos;
				m_NavMeshAgent.SetDestination(getNextPos);
				m_PatrolSettings.isMoving = true;
			}
			else if (m_PatrolSettings.IsOnTargetPos(m_Mover.transform.position))
			{
				m_PatrolSettings.Wait();
			}
		}
		Update_Orientation();
	}

	private void PatrolMove_DeInit()
	{
	}

	private void Goto_Init()
	{
		m_NavMeshAgent.enabled = false;
		m_Sentinel.SetPatrolAnim();
		m_GotoSettings.m_TargetPos = ((m_PatrolSettings.m_PatrolPoints.Length <= 0) ? m_Mover.transform.position : m_PatrolSettings.m_PatrolPoints[0].transform.position);
		m_GotoSettings.m_TargetDir = ((m_PatrolSettings.m_PatrolPoints.Length <= 0) ? m_Mover.transform.forward : m_PatrolSettings.m_PatrolPoints[0].transform.forward);
	}

	private void Goto_Update()
	{
		Vector3 vector = m_GotoSettings.getTargetPos - m_Mover.transform.position;
		Vector3 normalized = vector.normalized;
		float magnitude = vector.magnitude;
		m_GotoSettings.m_ActSpeed = Mathf.Clamp(m_GotoSettings.m_ActSpeed + m_GotoSettings.m_Acceleration * Time.deltaTime, 0f, m_GotoSettings.m_MaxSpeed);
		float a = m_GotoSettings.m_ActSpeed * Time.deltaTime;
		a = Mathf.Min(a, magnitude);
		m_Mover.transform.Translate(a * normalized, Space.World);
		if (!m_IsRotatingToTarget)
		{
			float num = magnitude / m_GotoSettings.m_MaxSpeed;
			float num2 = Vector3.Angle(base.transform.forward, m_GotoSettings.m_TargetDir);
			float num3 = num2 / m_GotoSettings.m_RotSpeed;
			if (num < num3)
			{
				StartCoroutine(RotateToTarget(m_GotoSettings.m_RotSpeed, m_GotoSettings.m_TargetDir));
			}
			else
			{
				Update_Orientation(m_GotoSettings.m_ActSpeed * normalized);
			}
		}
		Debug.DrawLine(m_GotoSettings.m_TargetPos, m_GotoSettings.m_TargetPos + 5f * m_GotoSettings.m_TargetDir, Color.magenta);
	}

	private void Goto_DeInit()
	{
	}

	private void MoveNear_Init()
	{
		m_NavMeshAgent.enabled = true;
		m_NavMeshAgent.speed = m_AttackSettings.m_AgentSpeed;
		m_NavMeshAgent.angularSpeed = m_AttackSettings.m_AgentAngularSpeed;
		m_NavMeshAgent.acceleration = m_AttackSettings.m_AgentAcceleration;
	}

	private void MoveNear_Update()
	{
		Vector3 lookRotation = m_Target.transform.position - m_Mover.transform.position;
		lookRotation.y = 0f;
		if (lookRotation.magnitude > m_AttackSettings.dbg_getStopMoveDistance)
		{
			m_NavMeshAgent.SetDestination(m_Target.transform.position);
		}
		else
		{
			m_NavMeshAgent.SetDestination(m_Mover.transform.position);
		}
		lookRotation.Normalize();
		Vector3 velocity = m_NavMeshAgent.velocity;
		velocity.y = 0f;
		float magnitude = velocity.magnitude;
		if (magnitude > 0.1f)
		{
			Update_Orientation();
			return;
		}
		Quaternion to = default(Quaternion);
		to.SetLookRotation(lookRotation);
		m_Mover.transform.rotation = Quaternion.RotateTowards(m_Mover.transform.rotation, to, m_AttackSettings.m_RotSpeed * Time.deltaTime);
	}

	private void MoveNear_DeInit()
	{
		m_NavMeshAgent.enabled = false;
	}

	private void Warning_Init()
	{
		m_NavMeshAgent.enabled = false;
		m_Sentinel.SetAttackAnim();
	}

	private void Warning_Update()
	{
		Vector3 lookRotation = m_Target.transform.position - m_Mover.transform.position;
		lookRotation.y = 0f;
		lookRotation.Normalize();
		Quaternion to = default(Quaternion);
		to.SetLookRotation(lookRotation);
		m_Mover.transform.rotation = Quaternion.RotateTowards(m_Mover.transform.rotation, to, m_AttackSettings.m_RotSpeed * Time.deltaTime);
	}

	private void Warning_DeInit()
	{
		m_NavMeshAgent.enabled = false;
	}

	private void Attack_Init()
	{
		m_NavMeshAgent.enabled = true;
		m_NavMeshAgent.speed = m_AttackSettings.m_AgentSpeed;
		m_NavMeshAgent.angularSpeed = m_AttackSettings.m_AgentAngularSpeed;
		m_NavMeshAgent.acceleration = m_AttackSettings.m_AgentAcceleration;
		m_Sentinel.SetAttackAnim();
	}

	private void Attack_Update()
	{
		Vector3 velocity = m_NavMeshAgent.velocity;
		velocity.y = 0f;
		float magnitude = velocity.magnitude;
		m_AttackSettings.m_IsMoving = magnitude > 0.1f;
		if (!Attack_IsTargetPosValid())
		{
			Attack_GenerateNewTargetPos();
			m_NavMeshAgent.SetDestination(m_AttackSettings.m_MoveTargetPos);
		}
		if (m_AttackSettings.m_IsMoving)
		{
			m_AttackSettings.m_LastMoveTime = Time.timeSinceLevelLoad;
		}
		Vector3 vector = m_Target.transform.position - m_Mover.transform.position;
		vector.y = 0f;
		if (!m_IsRotatingToTarget && Vector3.Angle(vector, m_Mover.transform.forward) > 20f)
		{
			StartCoroutine(RotateToTarget(m_AttackSettings.m_RotSpeed));
		}
	}

	private void Attack_DeInit()
	{
		m_NavMeshAgent.enabled = false;
	}

	private bool Attack_IsTargetPosValid()
	{
		if (!m_AttackSettings.m_IsMoving && Time.timeSinceLevelLoad - m_AttackSettings.m_LastMoveTime > m_AttackSettings.m_MoveWaitTime)
		{
			return false;
		}
		return true;
	}

	private void Attack_GenerateNewTargetPos()
	{
		Vector3 vector = m_Target.transform.position - m_Mover.transform.position;
		vector.y = 0f;
		vector.Normalize();
		Quaternion quaternion = Quaternion.AngleAxis(60f, Vector3.up);
		Quaternion quaternion2 = Quaternion.AngleAxis(-60f, Vector3.up);
		Vector3 vector2 = m_Target.transform.position - m_AttackSettings.dbg_getStopMoveDistance * (quaternion * vector);
		Vector3 vector3 = m_Target.transform.position - m_AttackSettings.dbg_getStopMoveDistance * (quaternion2 * vector);
		m_AttackSettings.m_MoveTargetPos = ((Random.Range(0, 10) >= 5) ? vector3 : vector2);
	}

	private void FireingMove_Init()
	{
		m_NavMeshAgent.speed = m_AttackSettings.m_AgentSpeed;
		m_NavMeshAgent.angularSpeed = m_AttackSettings.m_AgentAngularSpeed;
		m_NavMeshAgent.acceleration = m_AttackSettings.m_AgentAcceleration;
		m_NavMeshAgent.enabled = false;
		m_DbgMoveSettings.m_LastMoveTime = Time.timeSinceLevelLoad;
		m_DbgMoveSettings.m_IsMoving = false;
	}

	private void FireingMove_GenerateTargetPos()
	{
		if (!(m_Target == null))
		{
			Vector3 position = m_Target.transform.position;
			Vector3 vector = position - m_Mover.transform.position;
			vector.y = 0f;
			vector.Normalize();
			Vector3 vector2 = Vector3.Cross(vector, Vector3.up);
			Vector3[] array = (m_DbgMoveSettings.m_TargetPos = new Vector3[4]);
			float num = 4f;
			array[0] = position - num * vector;
			array[1] = position + num * vector2;
			array[2] = position + num * vector;
			array[3] = position - num * vector2;
			Debug.DrawLine(array[0], array[1], Color.magenta);
			Debug.DrawLine(array[1], array[2], Color.magenta);
			Debug.DrawLine(array[2], array[3], Color.magenta);
			Debug.DrawLine(array[3], array[0], Color.magenta);
		}
	}

	private void FireingMove_Update()
	{
		if (!m_DbgMoveSettings.m_IsMoving && Time.timeSinceLevelLoad - m_DbgMoveSettings.m_LastMoveTime > 2f)
		{
			FireingMove_GenerateTargetPos();
			m_DbgMoveSettings.NextPos();
			m_DbgMoveSettings.m_IsMoving = true;
		}
		if (m_DbgMoveSettings.m_IsMoving)
		{
			FireingMove_UpdateMove();
		}
		Vector3 vector = m_Target.transform.position - m_Mover.transform.position;
		vector.y = 0f;
		vector.Normalize();
		if (!m_IsRotatingToTarget && Vector3.Angle(vector, m_Mover.transform.forward) > 50f)
		{
			StartCoroutine(RotateToTarget(m_DbgMoveSettings.m_RotSpeed));
		}
	}

	private void FireingMove_UpdateMove()
	{
		m_DbgMoveSettings.m_LastMoveTime = Time.timeSinceLevelLoad;
		m_DbgMoveSettings.m_ActSpeed = Mathf.Clamp(m_DbgMoveSettings.m_ActSpeed + m_DbgMoveSettings.m_Acceleration * Time.deltaTime, 0f, m_DbgMoveSettings.m_MaxSpeed);
		Vector3 position = m_Mover.transform.position;
		Vector3 getCurrPos = m_DbgMoveSettings.getCurrPos;
		Debug.DrawLine(position, getCurrPos, Color.magenta);
		Vector3 vector = getCurrPos - position;
		vector.y = 0f;
		float magnitude = vector.magnitude;
		vector.Normalize();
		float num = m_DbgMoveSettings.m_ActSpeed * Time.deltaTime;
		if (magnitude < 0.5f)
		{
			m_DbgMoveSettings.m_IsMoving = false;
		}
		m_Mover.transform.Translate(num * vector, Space.World);
	}

	private void FireingMove_DeInit()
	{
	}

	private void DebugMove_Init()
	{
		m_NavMeshAgent.enabled = false;
		m_DbgMoveSettings.m_LastMoveTime = Time.timeSinceLevelLoad;
		m_DbgMoveSettings.m_StartPos = m_Mover.transform.position;
		DebugMove_ComputeTargetPos();
	}

	private void DebugMove_Update()
	{
		if (!m_DbgMoveSettings.m_IsMoving)
		{
			DebugMove_SetupMove();
		}
		if (m_DbgMoveSettings.m_IsMoving)
		{
			DebugMove_UpdateMove();
		}
		DebugMove_DebugDraw();
	}

	private void DebugMove_DeInit()
	{
		m_NavMeshAgent.enabled = false;
	}

	private void DebugMove_ComputeTargetPos()
	{
		if (!(m_Target == null))
		{
			Vector3 vector = m_Target.transform.position - m_DbgMoveSettings.m_StartPos;
			vector.y = 0f;
			vector.Normalize();
			Vector3 vector2 = Vector3.Cross(vector, Vector3.up);
			m_DbgMoveSettings.m_TargetPos[0] = m_DbgMoveSettings.m_StartPos + 3f * vector2 + 3f * vector;
			m_DbgMoveSettings.m_TargetPos[1] = m_DbgMoveSettings.m_StartPos + 3f * vector2 - 3f * vector;
			m_DbgMoveSettings.m_TargetPos[2] = m_DbgMoveSettings.m_StartPos - 3f * vector2 - 3f * vector;
			m_DbgMoveSettings.m_TargetPos[3] = m_DbgMoveSettings.m_StartPos - 3f * vector2 + 3f * vector;
		}
	}

	private void DebugMove_SetupMove()
	{
		m_DbgMoveSettings.NextPos();
		m_DbgMoveSettings.m_IsMoving = true;
	}

	private void DebugMove_UpdateMove()
	{
		m_DbgMoveSettings.m_LastMoveTime = Time.timeSinceLevelLoad;
		m_DbgMoveSettings.m_ActSpeed = Mathf.Clamp(m_DbgMoveSettings.m_ActSpeed + m_DbgMoveSettings.m_Acceleration * Time.deltaTime, 0f, m_DbgMoveSettings.m_MaxSpeed);
		Vector3 position = m_Mover.transform.position;
		Vector3 getCurrPos = m_DbgMoveSettings.getCurrPos;
		Vector3 vector = getCurrPos - position;
		float magnitude = vector.magnitude;
		vector.Normalize();
		float num = m_DbgMoveSettings.m_ActSpeed * Time.deltaTime;
		Vector3 axis = Vector3.Cross(Vector3.up, vector);
		Quaternion inTo = Quaternion.AngleAxis(20f, axis);
		inTo = RotateToward(m_Mover.transform.rotation, inTo, 90f, Time.deltaTime);
		if (magnitude < 0.5f)
		{
			m_DbgMoveSettings.m_IsMoving = false;
		}
		m_Mover.transform.Translate(num * vector, Space.World);
		m_Mover.transform.rotation = inTo;
	}

	private void DebugMove_DebugDraw()
	{
		int num = m_DbgMoveSettings.m_TargetPos.Length;
		for (int i = 0; i < num; i++)
		{
			Debug.DrawLine(m_DbgMoveSettings.m_TargetPos[i], m_DbgMoveSettings.m_TargetPos[i % num], Color.magenta);
		}
	}

	private void internalInitMove(MoveMode inMoveMode)
	{
		switch (inMoveMode)
		{
		case MoveMode.DebugMove:
			DebugMove_Init();
			break;
		case MoveMode.Patrol:
			PatrolMove_Init();
			break;
		case MoveMode.Goto:
			Goto_Init();
			break;
		case MoveMode.MoveNear:
			MoveNear_Init();
			break;
		case MoveMode.FireingMove:
			FireingMove_Init();
			break;
		case MoveMode.Warning:
			Warning_Init();
			break;
		case MoveMode.Attack:
			Attack_Init();
			break;
		case MoveMode.None:
			break;
		case MoveMode.Destroyed:
			break;
		default:
			Debug.LogError("Unknown MoveMode " + inMoveMode);
			break;
		}
	}

	private void internalUpdateMove(MoveMode inMoveMode)
	{
		switch (inMoveMode)
		{
		case MoveMode.DebugMove:
			DebugMove_Update();
			break;
		case MoveMode.Patrol:
			PatrolMove_Update();
			break;
		case MoveMode.Goto:
			Goto_Update();
			break;
		case MoveMode.MoveNear:
			MoveNear_Update();
			break;
		case MoveMode.FireingMove:
			FireingMove_Update();
			break;
		case MoveMode.Warning:
			Warning_Update();
			break;
		case MoveMode.Attack:
			Attack_Update();
			break;
		case MoveMode.None:
			break;
		case MoveMode.Destroyed:
			break;
		default:
			Debug.LogError("Unknown MoveMode " + inMoveMode);
			break;
		}
	}

	private void internalDeInitMove(MoveMode inMoveMode)
	{
		switch (inMoveMode)
		{
		case MoveMode.DebugMove:
			DebugMove_DeInit();
			break;
		case MoveMode.Patrol:
			PatrolMove_DeInit();
			break;
		case MoveMode.Goto:
			Goto_DeInit();
			break;
		case MoveMode.MoveNear:
			MoveNear_DeInit();
			break;
		case MoveMode.FireingMove:
			FireingMove_DeInit();
			break;
		case MoveMode.Warning:
			Warning_DeInit();
			break;
		case MoveMode.Attack:
			Attack_DeInit();
			break;
		case MoveMode.None:
			break;
		case MoveMode.Destroyed:
			break;
		default:
			Debug.LogError("Unknown MoveMode " + inMoveMode);
			break;
		}
	}

	private void UpdateMove(MoveMode inMoveMode)
	{
		if (inMoveMode != m_ActMoveMode)
		{
			internalDeInitMove(m_ActMoveMode);
			m_ActMoveMode = inMoveMode;
			internalInitMove(m_ActMoveMode);
		}
		internalUpdateMove(m_ActMoveMode);
	}

	private void Update_Orientation()
	{
		Update_Orientation_new();
	}

	private void Update_Orientation_new()
	{
		Vector3 velocity = m_NavMeshAgent.velocity;
		velocity.y = 0f;
		m_VelocitySmoother.AddSample(velocity);
		velocity = m_VelocitySmoother.GetSmoothedValue();
		Quaternion rotation = m_Mover.transform.rotation;
		Quaternion quaternion = default(Quaternion);
		Vector3 vector = velocity.normalized;
		if (vector == Vector3.zero)
		{
			vector = rotation * Vector3.forward;
			vector.y = 0f;
			vector.Normalize();
		}
		quaternion.SetLookRotation(vector);
		Quaternion quaternion2 = Quaternion.AngleAxis(40f * Mathf.Clamp(velocity.magnitude / m_NavMeshAgent.speed, 0f, 1f), Vector3.right);
		quaternion *= quaternion2;
		float num = Quaternion.Angle(rotation, quaternion);
		float num2 = num / m_PatrolSettings.m_RotSpeed;
		float t = ((num2 != 0f) ? Mathf.Clamp(Time.deltaTime / num2, 0f, 1f) : 1f);
		Quaternion rotation2 = Quaternion.Slerp(rotation, quaternion, t);
		m_Mover.transform.rotation = rotation2;
		m_LastVelocity = velocity;
	}

	private void Update_Orientation_old()
	{
		Vector3 velocity = m_NavMeshAgent.velocity;
		velocity.y = 0f;
		Vector3 vector = (velocity - m_LastVelocity) / Time.deltaTime;
		float magnitude = velocity.magnitude;
		float magnitude2 = vector.magnitude;
		Quaternion rotation = m_Mover.transform.rotation;
		Quaternion quaternion = default(Quaternion);
		if (magnitude > 0.3f)
		{
			Vector3 normalized = velocity.normalized;
			float num = 1f;
			if (magnitude2 > magnitude)
			{
				num = ((!(Vector3.Dot(normalized, vector.normalized) < -0.5f)) ? 1f : (-1f));
			}
			float num2 = 0.6f * num * magnitude / m_PatrolSettings.m_AgentSpeed;
			Vector3 lookRotation = normalized;
			Vector3 axis = Vector3.Cross(Vector3.up, normalized);
			Quaternion quaternion2 = Quaternion.AngleAxis(20f * num2, axis);
			quaternion.SetLookRotation(lookRotation);
			quaternion = quaternion2 * quaternion;
		}
		else
		{
			Vector3 lookRotation2 = rotation * Vector3.forward;
			lookRotation2.y = 0f;
			lookRotation2.Normalize();
			quaternion.SetLookRotation(lookRotation2);
		}
		float num3 = Quaternion.Angle(rotation, quaternion);
		float num4 = num3 / m_PatrolSettings.m_RotSpeed;
		float t = ((num4 != 0f) ? Mathf.Clamp(Time.deltaTime / num4, 0f, 1f) : 1f);
		Quaternion rotation2 = Quaternion.Slerp(rotation, quaternion, t);
		m_Mover.transform.rotation = rotation2;
		m_LastVelocity = velocity;
	}

	private void Update_Orientation(Vector3 inActVelocity)
	{
		Vector3 vector = inActVelocity;
		vector.y = 0f;
		Vector3 vector2 = (vector - m_LastVelocity) / Time.deltaTime;
		float magnitude = vector.magnitude;
		float magnitude2 = vector2.magnitude;
		Quaternion rotation = m_Mover.transform.rotation;
		Quaternion quaternion = default(Quaternion);
		if (magnitude > 0.3f)
		{
			Vector3 normalized = vector.normalized;
			float num = 1f;
			if (magnitude2 > magnitude)
			{
				num = ((!(Vector3.Dot(normalized, vector2.normalized) < -0.5f)) ? 1f : (-1f));
			}
			float num2 = 0.6f * num * magnitude / m_PatrolSettings.m_AgentSpeed;
			Vector3 lookRotation = normalized;
			Vector3 axis = Vector3.Cross(Vector3.up, normalized);
			Quaternion quaternion2 = Quaternion.AngleAxis(20f * num2, axis);
			quaternion.SetLookRotation(lookRotation);
			quaternion = quaternion2 * quaternion;
		}
		else
		{
			Vector3 lookRotation2 = rotation * Vector3.forward;
			lookRotation2.y = 0f;
			lookRotation2.Normalize();
			quaternion.SetLookRotation(lookRotation2);
		}
		float num3 = Quaternion.Angle(rotation, quaternion);
		float num4 = num3 / m_PatrolSettings.m_RotSpeed;
		float t = ((num4 != 0f) ? Mathf.Clamp(Time.deltaTime / num4, 0f, 1f) : 1f);
		Quaternion rotation2 = Quaternion.Slerp(rotation, quaternion, t);
		m_Mover.transform.rotation = rotation2;
		m_LastVelocity = vector;
		Vector3 position = m_Mover.transform.position;
		Quaternion rotation3 = m_Mover.transform.rotation;
		Vector3 vector3 = vector2;
		Vector3 vector4 = vector;
		Debug.DrawLine(position, position + 2f * vector4, Color.blue);
		Debug.DrawLine(position, position + rotation3 * Vector3.up, Color.blue);
		Debug.DrawLine(position, position + rotation3 * Vector3.up + vector3, Color.red);
	}

	internal Quaternion RotateToward(Quaternion inFrom, Quaternion inTo, float inRotSpeed, float inTime)
	{
		float num = Quaternion.Angle(inFrom, inTo);
		float num2 = num / inRotSpeed;
		float t = ((num2 != 0f) ? Mathf.Clamp(inTime / num2, 0f, 1f) : 0f);
		return Quaternion.Slerp(inFrom, inTo, t);
	}

	private IEnumerator RotateToTarget(float inRotationSpeed)
	{
		while (true)
		{
			m_IsRotatingToTarget = true;
			Vector3 dirToTarget = m_Target.transform.position - m_Mover.transform.position;
			dirToTarget.y = 0f;
			Quaternion reqRot = Quaternion.LookRotation(dirToTarget);
			Quaternion actRot = RotateToward(m_Mover.transform.rotation, reqRot, inRotationSpeed, Time.deltaTime);
			m_Mover.transform.rotation = actRot;
			if (Quaternion.Angle(reqRot, actRot) < 5f)
			{
				break;
			}
			yield return null;
		}
		m_IsRotatingToTarget = false;
	}

	private IEnumerator RotateToTarget(float inRotationSpeed, Vector3 inTargetDir)
	{
		while (true)
		{
			inTargetDir.y = 0f;
			m_IsRotatingToTarget = true;
			Quaternion reqRot = Quaternion.LookRotation(inTargetDir);
			Quaternion actRot = RotateToward(m_Mover.transform.rotation, reqRot, inRotationSpeed, Time.deltaTime);
			m_Mover.transform.rotation = actRot;
			if (Quaternion.Angle(reqRot, actRot) < 5f)
			{
				break;
			}
			yield return null;
		}
		m_IsRotatingToTarget = false;
	}

	internal Vector3 FixupAngles(Vector3 inValue)
	{
		inValue.x -= ((inValue.x > 180f) ? 360 : 0);
		inValue.y -= ((inValue.y > 180f) ? 360 : 0);
		inValue.z -= ((inValue.z > 180f) ? 360 : 0);
		return inValue;
	}

	internal Vector3 SmoothCD(Vector3 from, Vector3 to, ref Vector3 vel, float smoothTime, float timeDelta)
	{
		float num = 2f / smoothTime;
		float num2 = num * timeDelta;
		float num3 = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
		Vector3 vector = from - to;
		Vector3 vector2 = (vel + num * vector) * timeDelta;
		vel = (vel - num * vector2) * num3;
		return to + (vector + vector2) * num3;
	}
}
