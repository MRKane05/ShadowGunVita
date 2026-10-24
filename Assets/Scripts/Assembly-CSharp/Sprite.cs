using System.Collections;
using UnityEngine;

public class Sprite : MonoBehaviour
{
	public delegate void AnimCompleteDelegate();

	protected float m_width;

	protected float m_height;

	protected Vector2 m_lowerLeftUV;

	protected Vector2 m_UVDimensions;

	protected GameObject m_client;

	protected SpriteManager m_manager;

	protected bool m_billboarded;

	public bool m_hidden___DoNotAccessExternally;

	protected Vector3[] meshVerts;

	protected Vector2[] UVs;

	public Transform clientTransform;

	public Vector3 offset = default(Vector3);

	public Color color;

	public int index;

	public Vector3 v1 = default(Vector3);

	public Vector3 v2 = default(Vector3);

	public Vector3 v3 = default(Vector3);

	public Vector3 v4 = default(Vector3);

	public int mv1;

	public int mv2;

	public int mv3;

	public int mv4;

	public int uv1;

	public int uv2;

	public int uv3;

	public int uv4;

	public int cv1;

	public int cv2;

	public int cv3;

	public int cv4;

	protected ArrayList animations = new ArrayList();

	protected UVAnimation curAnim;

	protected AnimCompleteDelegate animCompleteDelegate;

	public GameObject dummyGO;

	public SpriteManager manager
	{
		get
		{
			return m_manager;
		}
		set
		{
			m_manager = value;
		}
	}

	public GameObject client
	{
		get
		{
			return m_client;
		}
		set
		{
			m_client = value;
			if (m_client != null)
			{
				base.name = m_client.name;
			}
			if (m_client != null)
			{
				clientTransform = m_client.transform;
			}
			else
			{
				clientTransform = null;
			}
		}
	}

	public Vector2 lowerLeftUV
	{
		get
		{
			return m_lowerLeftUV;
		}
		set
		{
			m_lowerLeftUV = value;
			m_manager.UpdateUV(this);
		}
	}

	public Vector2 uvDimensions
	{
		get
		{
			return m_UVDimensions;
		}
		set
		{
			m_UVDimensions = value;
			m_manager.UpdateUV(this);
		}
	}

	public float width
	{
		get
		{
			return m_width;
		}
	}

	public float height
	{
		get
		{
			return m_height;
		}
	}

	public bool billboarded
	{
		get
		{
			return m_billboarded;
		}
		set
		{
			m_billboarded = value;
		}
	}

	public bool hidden
	{
		get
		{
			return m_hidden___DoNotAccessExternally;
		}
		set
		{
			if (value != m_hidden___DoNotAccessExternally)
			{
				m_hidden___DoNotAccessExternally = value;
				if (value)
				{
					m_manager.HideSprite(this);
				}
				else
				{
					m_manager.ShowSprite(this);
				}
			}
		}
	}

	public Sprite()
	{
		m_width = 0f;
		m_height = 0f;
		m_client = null;
		m_manager = null;
		clientTransform = null;
		index = 0;
		color = Color.white;
		offset = Vector3.zero;
	}

	~Sprite()
	{
	}

	public void SetWidthHeightXY(float width, float height)
	{
		m_width = width;
		m_height = height;
		v3 = offset + new Vector3(m_width / 2f, (0f - m_height) / 2f, 0f);
		v4 = offset + new Vector3(m_width / 2f, m_height / 2f, 0f);
		Transform();
	}

	public void SetSizeXY(float width, float height)
	{
		m_width = width;
		m_height = height;
		v1 = offset + new Vector3((0f - m_width) / 2f, m_height / 2f, 0f);
		v2 = offset + new Vector3((0f - m_width) / 2f, (0f - m_height) / 2f, 0f);
		v3 = offset + new Vector3(m_width / 2f, (0f - m_height) / 2f, 0f);
		v4 = offset + new Vector3(m_width / 2f, m_height / 2f, 0f);
		Transform();
	}

	public void SetSizeXZ(float width, float height)
	{
		m_width = width;
		m_height = height;
		v1 = offset + new Vector3((0f - m_width) / 2f, 0f, m_height / 2f);
		v2 = offset + new Vector3((0f - m_width) / 2f, 0f, (0f - m_height) / 2f);
		v3 = offset + new Vector3(m_width / 2f, 0f, (0f - m_height) / 2f);
		v4 = offset + new Vector3(m_width / 2f, 0f, m_height / 2f);
		Transform();
	}

	public void SetSizeYZ(float width, float height)
	{
		m_width = width;
		m_height = height;
		v1 = offset + new Vector3(0f, m_height / 2f, (0f - m_width) / 2f);
		v2 = offset + new Vector3(0f, (0f - m_height) / 2f, (0f - m_width) / 2f);
		v3 = offset + new Vector3(0f, (0f - m_height) / 2f, m_width / 2f);
		v4 = offset + new Vector3(0f, m_height / 2f, m_width / 2f);
		Transform();
	}

	public void SetBuffers(Vector3[] v, Vector2[] uv)
	{
		meshVerts = v;
		UVs = uv;
	}

	public void Transform()
	{
		meshVerts[mv1] = clientTransform.TransformPoint(v1);
		meshVerts[mv2] = clientTransform.TransformPoint(v2);
		meshVerts[mv3] = clientTransform.TransformPoint(v3);
		meshVerts[mv4] = clientTransform.TransformPoint(v4);
		m_manager.UpdatePositions();
	}

	public void TransformToGround()
	{
		Vector3 zero = Vector3.zero;
		RaycastHit hitInfo;
		if (Physics.Raycast(clientTransform.position, -Vector3.up * 2f, out hitInfo, 5f, 16384))
		{
			zero.y = 0f - hitInfo.distance;
		}
		meshVerts[mv1] = zero + clientTransform.TransformPoint(v1);
		meshVerts[mv2] = zero + clientTransform.TransformPoint(v2);
		meshVerts[mv3] = zero + clientTransform.TransformPoint(v3);
		meshVerts[mv4] = zero + clientTransform.TransformPoint(v4);
		m_manager.UpdatePositions();
	}

	public void TransformBillboarded(Transform t)
	{
		Vector3 position = clientTransform.position;
		meshVerts[mv1] = position + t.InverseTransformDirection(v1);
		meshVerts[mv2] = position + t.InverseTransformDirection(v2);
		meshVerts[mv3] = position + t.InverseTransformDirection(v3);
		meshVerts[mv4] = position + t.InverseTransformDirection(v4);
		m_manager.UpdatePositions();
	}

	public void SetColor(Color c)
	{
		color = c;
		m_manager.UpdateColors(this);
	}

	public void SetAnimCompleteDelegate(AnimCompleteDelegate del)
	{
		animCompleteDelegate = del;
	}

	public void AddAnimation(UVAnimation anim)
	{
		animations.Add(anim);
	}

	protected void StepAnim()
	{
		if (curAnim.GetNextFrame(ref m_lowerLeftUV))
		{
			m_manager.UpdateUV(this);
			return;
		}
		CancelInvoke("StepAnim");
		if (animCompleteDelegate != null)
		{
			animCompleteDelegate();
		}
	}

	public void PlayAnim(UVAnimation anim)
	{
		CancelInvoke("StepAnim");
		curAnim = anim;
		curAnim.Reset();
		StepAnim();
		InvokeRepeating("StepAnim", 1f / anim.framerate, 1f / anim.framerate);
	}

	public void PlayAnim(string name)
	{
		for (int i = 0; i < animations.Count; i++)
		{
			if (((UVAnimation)animations[i]).name == name)
			{
				PlayAnim((UVAnimation)animations[i]);
			}
		}
	}

	public void PlayAnimInReverse(UVAnimation anim)
	{
		CancelInvoke("StepAnim");
		curAnim = anim;
		curAnim.Reset();
		curAnim.PlayInReverse();
		StepAnim();
		InvokeRepeating("StepAnim", 1f / anim.framerate, 1f / anim.framerate);
	}

	public void PlayAnimInReverse(string name)
	{
		for (int i = 0; i < animations.Count; i++)
		{
			if (((UVAnimation)animations[i]).name == name)
			{
				((UVAnimation)animations[i]).PlayInReverse();
				PlayAnimInReverse((UVAnimation)animations[i]);
			}
		}
	}

	public void PauseAnim()
	{
		CancelInvoke("StepAnim");
	}

	public void UnpauseAnim()
	{
		if (curAnim != null)
		{
			StepAnim();
			InvokeRepeating("StepAnim", 1f / curAnim.framerate, 1f / curAnim.framerate);
		}
	}
}
