using UnityEngine;

public class Vector3Smoothing
{
	private int m_CurrentIndex;

	private Vector3[] m_Samples;

	public Vector3Smoothing(int inNumOfSamples)
	{
		m_Samples = new Vector3[Mathf.Max(inNumOfSamples, 1)];
		for (int num = m_Samples.Length - 1; num >= 0; num--)
		{
			m_Samples[num] = Vector3.zero;
		}
	}

	public void Reset()
	{
		m_CurrentIndex = 0;
		for (int num = m_Samples.Length - 1; num >= 0; num--)
		{
			m_Samples[num] = Vector3.zero;
		}
	}

	public void AddSample(Vector3 inValue)
	{
		m_Samples[m_CurrentIndex] = inValue;
		m_CurrentIndex = ++m_CurrentIndex % m_Samples.Length;
	}

	public Vector3 GetSmoothedValue()
	{
		Vector3 zero = Vector3.zero;
		Vector3[] samples = m_Samples;
		foreach (Vector3 vector in samples)
		{
			zero += vector;
		}
		return zero / m_Samples.Length;
	}
}
