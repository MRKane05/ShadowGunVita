using UnityEngine;

public class CamRelativePos : MonoBehaviour
{
	public Vector3 m_Offset = new Vector3(0f, 0f, 0f);

	public bool m_HorizontalOnly;

	private Transform m_OwnerTransform;

	private void Awake()
	{
		m_OwnerTransform = base.transform;
	}

	private void LateUpdate()
	{
		if ((bool)m_OwnerTransform && (bool)Camera.main)
		{
			Vector3 position = Camera.main.transform.position + m_Offset;
			if (m_HorizontalOnly)
			{
				position.y = m_OwnerTransform.position.y;
			}
			m_OwnerTransform.position = position;
		}
	}
}
