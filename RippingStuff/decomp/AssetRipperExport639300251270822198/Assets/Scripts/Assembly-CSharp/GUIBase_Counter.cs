using UnityEngine;

[AddComponentMenu("GUI/Widgets/Counter")]
public class GUIBase_Counter : GUIBase_Callback
{
	public struct S_SpriteUV
	{
		public Vector2 m_LowerLeftUV;

		public Vector2 m_UvDimensions;
	}

	public int m_MaxCount = 3;

	public GUIBase_Sprite[] m_UsedSprites = new GUIBase_Sprite[1];

	private GUIBase_Widget m_Widget;

	private MFGuiRenderer m_GuiRenderer;

	private S_SpriteUV[] m_UsedSpritesUV = new S_SpriteUV[1];

	private static int MAX_COUNT = 10;

	public void Start()
	{
		m_Widget = GetComponent<GUIBase_Widget>();
		int clbkTypes = 1;
		m_Widget.RegisterCallback(this, clbkTypes);
	}

	public override bool Callback(E_CallbackType type)
	{
		if (type == E_CallbackType.E_CT_INIT)
		{
			CustomInit();
		}
		return true;
	}

	private void CustomInit()
	{
		m_GuiRenderer = m_Widget.GetGuiRenderer();
		m_MaxCount = Mathf.Clamp(m_MaxCount, 1, MAX_COUNT);
		Vector3 lossyScale = base.gameObject.transform.lossyScale;
		float width = m_Widget.GetWidth() / (float)m_MaxCount;
		float height = m_Widget.GetHeight();
		float x = m_Widget.GetWidth() * lossyScale.x / 2f;
		Vector3 vector = new Vector3(x, 0f, 0f);
		Vector2 vector2 = default(Vector2);
		vector2 = vector / ((float)m_MaxCount * 0.5f);
		Vector3 vector3 = default(Vector3);
		vector3 = base.gameObject.transform.position - vector;
		for (int i = 0; i < m_MaxCount; i++)
		{
			m_Widget.AddSprite(new Vector2(vector3.x + ((float)i + 0.5f) * vector2.x, vector3.y + ((float)i + 0.5f) * vector2.y), width, height, lossyScale.x, lossyScale.y, 0f, 0, 0, 1, 1);
		}
		m_Widget.SetSpriteProxyFlag(0);
		Texture texture = m_Widget.GetTexture();
		if (!texture || m_UsedSprites.Length <= 0)
		{
			return;
		}
		int width2 = texture.width;
		int height2 = texture.height;
		m_UsedSpritesUV = new S_SpriteUV[m_UsedSprites.Length];
		for (int j = 0; j < m_UsedSprites.Length; j++)
		{
			if ((bool)m_UsedSprites[j])
			{
				GUIBase_Widget component = m_UsedSprites[j].GetComponent<GUIBase_Widget>();
				if ((bool)component)
				{
					float UVLeft;
					float UVTop;
					float UVWidth;
					float UVHeight;
					component.GetUV(out UVLeft, out UVTop, out UVWidth, out UVHeight);
					m_UsedSpritesUV[j].m_LowerLeftUV = new Vector2(UVLeft, UVTop + UVHeight);
					m_UsedSpritesUV[j].m_UvDimensions = new Vector2(UVWidth / (float)width2, UVHeight / (float)height2);
				}
			}
		}
	}

	public void SetValue(int idx, int type)
	{
		if (idx >= 0 && idx < m_MaxCount)
		{
			MFGuiSprite sprite = m_Widget.GetSprite(idx + 1);
			if (sprite != null && type != -1 && type >= 0 && type < m_UsedSprites.Length)
			{
				sprite.lowerLeftUV = m_GuiRenderer.PixelCoordToUVCoord((int)m_UsedSpritesUV[type].m_LowerLeftUV.x, (int)m_UsedSpritesUV[type].m_LowerLeftUV.y);
				sprite.uvDimensions = m_UsedSpritesUV[type].m_UvDimensions;
			}
		}
	}

	public GUIBase_Widget GetSpriteWidget(int idx)
	{
		if (idx >= 0 && idx < m_UsedSprites.Length && (bool)m_UsedSprites[idx])
		{
			return m_UsedSprites[idx].Widget;
		}
		return null;
	}
}
