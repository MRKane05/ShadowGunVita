using UnityEngine;

[AddComponentMenu("")]
public class MFGuiSprite
{
	protected float m_width;

	protected float m_height;

	protected Vector2 m_lowerLeftUV;

	protected Vector2 m_UVDimensions;

	protected bool m_HasSlient;

	protected MFGuiRenderer m_GuiRenderer;

	public bool m_hidden___DoNotAccessExternally;

	protected Vector3[] meshVerts;

	protected Vector2[] UVs;

	public Matrix4x4 matrix;

	public Vector3 offset;

	public Color m_Color;

	public int index;

	public Vector3 v1;

	public Vector3 v2;

	public Vector3 v3;

	public Vector3 v4;

	public MFGuiRenderer guiRenderer
	{
		set
		{
			m_GuiRenderer = value;
		}
	}

	public bool client
	{
		get
		{
			return m_HasSlient;
		}
		set
		{
			m_HasSlient = value;
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
			m_GuiRenderer.UpdateUV(this);
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
			m_GuiRenderer.UpdateUV(this);
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
					m_GuiRenderer.HideSprite(this);
				}
				else
				{
					m_GuiRenderer.ShowSprite(this);
				}
			}
		}
	}

	public MFGuiSprite()
	{
		m_width = 0f;
		m_height = 0f;
		m_HasSlient = false;
		m_GuiRenderer = null;
		index = 0;
		m_Color = Color.black;
		offset = Vector3.zero;
	}

	~MFGuiSprite()
	{
		m_GuiRenderer = null;
	}

	public void SetWidthHeightXY(float width, float height)
	{
		m_width = width;
		m_height = height;
		v3 = offset + new Vector3(m_width / 2f, (0f - m_height) / 2f, 0f);
		v4 = offset + new Vector3(m_width / 2f, m_height / 2f, 0f);
		Transform();
	}

	public void SetSizeXY(float width, float height, float depth)
	{
		m_width = width;
		m_height = height;
		v1 = offset + new Vector3((0f - m_width) / 2f, m_height / 2f, depth);
		v2 = offset + new Vector3((0f - m_width) / 2f, (0f - m_height) / 2f, depth);
		v3 = offset + new Vector3(m_width / 2f, (0f - m_height) / 2f, depth);
		v4 = offset + new Vector3(m_width / 2f, m_height / 2f, depth);
		Transform();
	}

	public void SetSizeXZ(float width, float height, float depth)
	{
		m_width = width;
		m_height = height;
		v1 = offset + new Vector3((0f - m_width) / 2f, depth, m_height / 2f);
		v2 = offset + new Vector3((0f - m_width) / 2f, depth, (0f - m_height) / 2f);
		v3 = offset + new Vector3(m_width / 2f, depth, (0f - m_height) / 2f);
		v4 = offset + new Vector3(m_width / 2f, depth, m_height / 2f);
		Transform();
	}

	public void SetSizeYZ(float width, float height, float depth)
	{
		m_width = width;
		m_height = height;
		v1 = offset + new Vector3(depth, m_height / 2f, (0f - m_width) / 2f);
		v2 = offset + new Vector3(depth, (0f - m_height) / 2f, (0f - m_width) / 2f);
		v3 = offset + new Vector3(depth, (0f - m_height) / 2f, m_width / 2f);
		v4 = offset + new Vector3(depth, m_height / 2f, m_width / 2f);
		Transform();
	}

	public void SetBuffers(Vector3[] v, Vector2[] uv)
	{
		meshVerts = v;
		UVs = uv;
	}

	public void Transform()
	{
		int num = 4 * index;
		meshVerts[num] = matrix.MultiplyPoint(v1);
		meshVerts[num + 1] = matrix.MultiplyPoint(v2);
		meshVerts[num + 2] = matrix.MultiplyPoint(v3);
		meshVerts[num + 3] = matrix.MultiplyPoint(v4);
		m_GuiRenderer.UpdatePositions();
	}

	public void TransformBillboarded(Transform t)
	{
		Vector3 vector = Mathfx.Matrix_GetPos(matrix);
		int num = 4 * index;
		meshVerts[num] = vector + t.InverseTransformDirection(v1);
		meshVerts[num + 1] = vector + t.InverseTransformDirection(v2);
		meshVerts[num + 2] = vector + t.InverseTransformDirection(v3);
		meshVerts[num + 3] = vector + t.InverseTransformDirection(v4);
		m_GuiRenderer.UpdatePositions();
	}

	public void SetColor(Color c)
	{
		m_Color = c;
		m_GuiRenderer.UpdateColors(this);
	}

	public void SetAlpha(float alpha)
	{
		m_Color.a = alpha;
		m_GuiRenderer.UpdateColors(this);
	}
}
