using System.Collections;
using UnityEngine;

public class BrooderCamera : MonoBehaviour
{
	private AgentHuman m_Target;

	public Transform m_HeadTransform;

	public float m_DetectionDistance = 10f;

	public float m_RotationSpeed = 40f;

	private float m_SqrDetectionDistance;

	private Quaternion m_InitialRotation;

	public AudioClip m_ActivationSound;

	public AudioClip m_RotationSound;

	public Explosion m_Explosion;

	private bool m_Active;

	private float m_LastSeenTime;

	private float m_LastCheckTime;

	private void Awake()
	{
		m_HeadTransform = ((!(m_HeadTransform == null)) ? m_HeadTransform : base.transform);
		m_InitialRotation = m_HeadTransform.rotation;
	}

	private void Start()
	{
		m_Target = ((!(m_Target == null)) ? m_Target : Player.Instance.Owner);
		m_SqrDetectionDistance = m_DetectionDistance * m_DetectionDistance;
		StartCoroutine(State_Inactive());
	}

	public bool IsEnemyVisible()
	{
		if (!IsEnemyInRange())
		{
			return false;
		}
		if (Time.timeSinceLevelLoad - m_LastSeenTime < 2f)
		{
			return true;
		}
		if (Time.timeSinceLevelLoad - m_LastCheckTime < 2f)
		{
			return false;
		}
		m_LastCheckTime = Time.timeSinceLevelLoad;
		Vector3 vector = DirToTarget(m_Target.ChestPosition);
		GameObject gameObject = CollisionUtils.FirstCollisionOnRay(new Ray(m_HeadTransform.position, vector.normalized), vector.magnitude, base.gameObject);
		if (gameObject != null && gameObject != m_Target.GameObject)
		{
			return false;
		}
		m_LastSeenTime = Time.timeSinceLevelLoad;
		return true;
	}

	private bool IsEnemyInRange()
	{
		if (m_Target == null)
		{
			return false;
		}
		Vector3 vector = DirToTarget(m_Target.ChestPosition);
		if (!m_Active)
		{
			return vector.sqrMagnitude < m_SqrDetectionDistance;
		}
		return vector.sqrMagnitude < m_SqrDetectionDistance * 1.5f;
	}

	private void OnEnable()
	{
		Reset();
	}

	private void OnDisable()
	{
		StopAllCoroutines();
		m_HeadTransform.rotation = m_InitialRotation;
		m_Active = false;
	}

	public void Reset()
	{
		m_LastSeenTime = 0f;
		m_LastCheckTime = 0f;
		StopAllCoroutines();
		m_HeadTransform.rotation = m_InitialRotation;
		base.gameObject.active = true;
		m_Active = false;
		StartCoroutine(State_Inactive());
	}

	public void Destroy()
	{
		StopAllCoroutines();
		if (m_Explosion != null)
		{
			Explosion explosion = Mission.Instance.ExplosionCache.Get(m_Explosion, base.transform.position, base.transform.rotation);
			explosion.damage = 0f;
		}
		base.gameObject.active = false;
		m_Active = false;
	}

	private IEnumerator State_Inactive()
	{
		m_Active = false;
		while (!IsEnemyInRange())
		{
			yield return new WaitForSeconds(0.5f);
		}
		StartCoroutine(State_Activating());
	}

	private IEnumerator State_Activating()
	{
		if (m_ActivationSound != null && base.GetComponent<AudioSource>() != null)
		{
			base.GetComponent<AudioSource>().PlayOneShot(m_ActivationSound);
			yield return new WaitForSeconds(m_ActivationSound.length);
		}
		StartCoroutine(State_Active());
	}

	private IEnumerator State_Active()
	{
		m_Active = true;
		while (IsEnemyInRange())
		{
			Vector3 dirToTarget = DirToTarget(m_Target.ChestPosition).normalized;
			if (Vector3.Angle(dirToTarget, m_HeadTransform.forward) > m_RotationSpeed * 0.25f)
			{
				yield return StartCoroutine(State_RotateToTarget());
			}
			yield return new WaitForSeconds(0.5f);
		}
		StartCoroutine(State_Deactivating());
	}

	private IEnumerator State_RotateToTarget()
	{
		base.GetComponent<AudioSource>().Stop();
		base.GetComponent<AudioSource>().clip = m_RotationSound;
		base.GetComponent<AudioSource>().loop = true;
		base.GetComponent<AudioSource>().Play();
		Vector3 dirToTarget = DirToTarget(m_Target.ChestPosition).normalized;
		while (Vector3.Angle(dirToTarget, m_HeadTransform.forward) > 5f)
		{
			LookAtTarget(m_Target.ChestPosition);
			yield return 0;
			dirToTarget = DirToTarget(m_Target.ChestPosition).normalized;
		}
		base.GetComponent<AudioSource>().Stop();
		base.GetComponent<AudioSource>().loop = false;
		base.GetComponent<AudioSource>().clip = null;
	}

	private IEnumerator State_Deactivating()
	{
		while (Quaternion.Angle(m_HeadTransform.rotation, m_InitialRotation) > 5f)
		{
			m_HeadTransform.rotation = RotateToward(m_HeadTransform.rotation, m_InitialRotation, m_RotationSpeed, Time.deltaTime);
			yield return 0;
		}
		StartCoroutine(State_Inactive());
	}

	private Vector3 DirToTarget(Vector3 inTargetPos)
	{
		return inTargetPos - m_HeadTransform.position;
	}

	private void LookAtTarget(Vector3 inTargetPos)
	{
		Vector3 lookRotation = inTargetPos - m_HeadTransform.position;
		Quaternion inTo = default(Quaternion);
		inTo.SetLookRotation(lookRotation);
		m_HeadTransform.rotation = RotateToward(m_HeadTransform.rotation, inTo, m_RotationSpeed, Time.deltaTime);
		Debug.DrawLine(m_HeadTransform.position, inTargetPos, Color.red);
	}

	internal Quaternion RotateToward(Quaternion inFrom, Quaternion inTo, float inRotSpeed, float inTime)
	{
		float num = Quaternion.Angle(inFrom, inTo);
		float num2 = num / inRotSpeed;
		float t = ((num2 != 0f) ? Mathf.Clamp(inTime / num2, 0f, 1f) : 0f);
		return Quaternion.Slerp(inFrom, inTo, t);
	}
}
