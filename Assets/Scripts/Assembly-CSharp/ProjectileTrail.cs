using UnityEngine;

public class ProjectileTrail : MonoBehaviour
{
	public float m_BeamNoFadeDuration = 1f;

	public float m_BeamFadeDuration = 2f;

	private float m_InitTimer;

	private LineRenderer m_LineRenderer;

	private Vector3 m_TrailInitPos;

	private int m_VertexCount;

	private void Awake()
	{
		m_LineRenderer = GetComponent<LineRenderer>();
	}

	private void Update()
	{
		if (Time.timeSinceLevelLoad - m_InitTimer > m_BeamNoFadeDuration && m_BeamFadeDuration > 0f)
		{
			float num = Time.timeSinceLevelLoad - m_InitTimer - m_BeamNoFadeDuration;
			float w = 1f - num / m_BeamFadeDuration;
			Vector4 vector = m_LineRenderer.material.GetVector("_TintColor");
			vector.w = w;
			m_LineRenderer.material.SetVector("_TintColor", vector);
		}
	}

	public void InitTrail(Vector3 inPos)
	{
		m_InitTimer = Time.timeSinceLevelLoad;
		m_TrailInitPos = inPos;
		if (m_LineRenderer != null)
		{
			m_VertexCount = 2;
			m_LineRenderer.useWorldSpace = true;
			m_LineRenderer.SetVertexCount(m_VertexCount);
			m_LineRenderer.SetPosition(0, m_TrailInitPos);
			m_LineRenderer.SetPosition(1, m_TrailInitPos);
		}
	}

	public void AddTrailPos(Vector3 inPos)
	{
		if (!(m_LineRenderer == null))
		{
			m_LineRenderer.SetPosition(m_VertexCount - 1, inPos);
			m_VertexCount++;
			m_LineRenderer.SetVertexCount(m_VertexCount);
			m_LineRenderer.SetPosition(m_VertexCount - 1, inPos);
		}
	}

	public void UpdateTrailPos(Vector3 inPos)
	{
		if (!(m_LineRenderer == null))
		{
			m_LineRenderer.SetPosition(m_VertexCount - 1, inPos);
		}
	}

	public bool IsVisible()
	{
		return Time.timeSinceLevelLoad - m_InitTimer < m_BeamNoFadeDuration + m_BeamFadeDuration;
	}
}
