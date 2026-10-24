using UnityEngine;

public class PredictPosition
{
	private int m_CurrentIndex;

	private Vector3[] m_Velocites;

	private float m_LastSampleTime;

	private Vector3 m_LastSamplePos = Vector3.zero;

	public PredictPosition(int inNumOfSamples)
	{
		m_Velocites = new Vector3[Mathf.Max(inNumOfSamples, 1)];
		for (int num = m_Velocites.Length - 1; num >= 0; num--)
		{
			m_Velocites[num] = Vector3.zero;
		}
	}

	public void AddSample(Vector3 inPosition)
	{
		float timeSinceLevelLoad = Time.timeSinceLevelLoad;
		float num = timeSinceLevelLoad - m_LastSampleTime;
		if (m_LastSampleTime > 0f && num > 0f)
		{
			Vector3 vector = inPosition - m_LastSamplePos;
			m_Velocites[m_CurrentIndex] = vector / num;
			m_CurrentIndex = ++m_CurrentIndex % m_Velocites.Length;
		}
		m_LastSampleTime = timeSinceLevelLoad;
		m_LastSamplePos = inPosition;
	}

	public Vector3 EstimatedMove(float inDeltaTime)
	{
		Vector3 zero = Vector3.zero;
		Vector3[] velocites = m_Velocites;
		foreach (Vector3 vector in velocites)
		{
			zero += vector;
		}
		return zero / m_Velocites.Length * inDeltaTime;
	}
}
