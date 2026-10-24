using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
public class VelocityTest : MonoBehaviour
{
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

	public AttackSettings m_AttackSettings;

	private NavMeshAgent m_NavMeshAgent;

	private AgentHuman m_Target;

	private bool m_IsRotatingToTarget;

	private void Start()
	{
		m_NavMeshAgent = GetComponentInChildren<NavMeshAgent>();
		m_NavMeshAgent.updateRotation = false;
		m_NavMeshAgent.enabled = true;
		m_NavMeshAgent.speed = m_AttackSettings.m_AgentSpeed;
		m_NavMeshAgent.angularSpeed = m_AttackSettings.m_AgentAngularSpeed;
		m_NavMeshAgent.acceleration = m_AttackSettings.m_AgentAcceleration;
		if (m_Target == null)
		{
			m_Target = Player.Instance.Owner;
		}
	}

	private void Update()
	{
		Vector3 velocity = m_NavMeshAgent.velocity;
		velocity.y = 0f;
		Debug.Log(string.Concat("name ", base.gameObject.name, " vec_velocity = ", velocity, " time ", Time.timeSinceLevelLoad));
		float magnitude = velocity.magnitude;
		m_AttackSettings.m_IsMoving = magnitude > 0.1f;
		if (!Attack_IsTargetPosValid())
		{
			Debug.Log("Attack_Update :: m_NavMeshAgent.SetTargetLocation(m_AttackSettings.m_MoveTargetPos)");
			Attack_GenerateNewTargetPos();
			m_NavMeshAgent.SetDestination(m_AttackSettings.m_MoveTargetPos);
			m_AttackSettings.m_LastMoveTime = Time.timeSinceLevelLoad;
		}
		if (m_AttackSettings.m_IsMoving)
		{
			m_AttackSettings.m_LastMoveTime = Time.timeSinceLevelLoad;
		}
		Vector3 vector = m_Target.transform.position - base.transform.position;
		vector.y = 0f;
		if (!m_IsRotatingToTarget && Vector3.Angle(vector, base.transform.forward) > 20f)
		{
			StartCoroutine(RotateToTarget());
		}
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
		Vector3 vector = m_Target.transform.position - base.transform.position;
		vector.y = 0f;
		vector.Normalize();
		Quaternion quaternion = Quaternion.AngleAxis(60f, Vector3.up);
		Quaternion quaternion2 = Quaternion.AngleAxis(-60f, Vector3.up);
		Vector3 vector2 = m_Target.transform.position - m_AttackSettings.dbg_getStopMoveDistance * (quaternion * vector);
		Vector3 vector3 = m_Target.transform.position - m_AttackSettings.dbg_getStopMoveDistance * (quaternion2 * vector);
		m_AttackSettings.m_MoveTargetPos = ((Random.Range(0, 10) >= 5) ? vector3 : vector2);
	}

	internal Quaternion RotateToward(Quaternion inFrom, Quaternion inTo, float inRotSpeed, float inTime)
	{
		float num = Quaternion.Angle(inFrom, inTo);
		float num2 = num / inRotSpeed;
		float t = ((num2 != 0f) ? Mathf.Clamp(inTime / num2, 0f, 1f) : 0f);
		return Quaternion.Slerp(inFrom, inTo, t);
	}

	private IEnumerator RotateToTarget()
	{
		while (true)
		{
			m_IsRotatingToTarget = true;
			Vector3 dirToTarget = m_Target.transform.position - base.transform.position;
			dirToTarget.y = 0f;
			Quaternion reqRot = Quaternion.LookRotation(dirToTarget);
			Quaternion actRot = RotateToward(base.transform.rotation, reqRot, m_AttackSettings.m_RotSpeed, Time.deltaTime);
			base.transform.rotation = actRot;
			if (Quaternion.Angle(reqRot, actRot) < 5f)
			{
				break;
			}
			yield return null;
		}
		m_IsRotatingToTarget = false;
	}
}
