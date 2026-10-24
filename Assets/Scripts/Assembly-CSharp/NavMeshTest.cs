using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshTest : MonoBehaviour
{
	public bool UseRotationFromNavMeshAgent = true;

	internal Vector3 m_StartPos;

	internal int m_CurrentPosIdx = -1;

	internal Vector3[] m_TargetPos = new Vector3[4];

	internal NavMeshAgent m_NavMeshAgent;

	public Vector3 getTargetPos
	{
		get
		{
			return (m_CurrentPosIdx >= 0) ? m_TargetPos[m_CurrentPosIdx] : m_StartPos;
		}
	}

	public void NextPos()
	{
		m_CurrentPosIdx = ++m_CurrentPosIdx % m_TargetPos.Length;
	}

	private void Start()
	{
		m_StartPos = base.transform.position;
		m_TargetPos[0] = m_StartPos + 3f * Vector3.forward + 3f * Vector3.right;
		m_TargetPos[1] = m_StartPos + 3f * Vector3.forward - 3f * Vector3.right;
		m_TargetPos[2] = m_StartPos - 3f * Vector3.forward - 3f * Vector3.right;
		m_TargetPos[3] = m_StartPos - 3f * Vector3.forward + 3f * Vector3.right;
		m_NavMeshAgent = GetComponent<NavMeshAgent>();
	}

	private void Update()
	{
		Vector3 vector = getTargetPos - base.transform.position;
		vector.y = 0f;
		if (vector.magnitude < 0.5f)
		{
			NextPos();
			m_NavMeshAgent.SetDestination(getTargetPos);
		}
		if (m_NavMeshAgent.updateRotation != UseRotationFromNavMeshAgent)
		{
			m_NavMeshAgent.updateRotation = UseRotationFromNavMeshAgent;
		}
		if (!UseRotationFromNavMeshAgent)
		{
			Quaternion to = default(Quaternion);
			to.SetLookRotation(vector.normalized);
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, to, 180f * Time.deltaTime);
		}
		int i = 0;
		for (int num = m_TargetPos.Length; i < num; i++)
		{
			Debug.DrawLine(m_TargetPos[i], m_TargetPos[(i + 1) % num], Color.magenta);
		}
	}
}
