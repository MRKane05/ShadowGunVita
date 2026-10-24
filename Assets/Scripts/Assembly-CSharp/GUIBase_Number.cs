using UnityEngine;

[AddComponentMenu("GUI/Widgets/Number")]
public class GUIBase_Number : GUIBase_Callback
{
	private static int MAX_NUMBER_DIGITS = 9;

	public int numberDigits = 1;

	public int m_Value = int.MinValue;

	public bool m_KeepZeros;

	private GUIBase_Widget m_Widget;

	private MFGuiRenderer m_GuiRenderer;

	private float m_UvLeft;

	private float m_UvTop;

	private float m_UvWidth;

	private float m_UvHeight;

	public GUIBase_Widget Widget
	{
		get
		{
			return m_Widget;
		}
	}

	public void Start()
	{
		m_Widget = GetComponent<GUIBase_Widget>();
		int clbkTypes = 3;
		m_Widget.RegisterCallback(this, clbkTypes);
	}

	public override bool Callback(E_CallbackType type)
	{
		switch (type)
		{
		case E_CallbackType.E_CT_INIT:
			CustomInit();
			break;
		case E_CallbackType.E_CT_SHOW:
			SetNumber(m_Value, 999999);
			break;
		}
		return true;
	}

	private void CustomInit()
	{
		m_GuiRenderer = m_Widget.GetGuiRenderer();
		numberDigits = Mathf.Clamp(numberDigits, 1, MAX_NUMBER_DIGITS);
		Vector3 lossyScale = base.gameObject.transform.lossyScale;
		float width = m_Widget.GetWidth() / (float)numberDigits;
		float height = m_Widget.GetHeight();
		Texture texture = m_Widget.GetTexture();
		int num = 1;
		int num2 = 1;
		if ((bool)texture)
		{
			num = texture.width;
			num2 = texture.height;
		}
		int texU = (int)((float)num * m_Widget.m_InTexPos.x);
		int num3 = (int)((float)num2 * m_Widget.m_InTexPos.y);
		int texW = (int)((float)num * m_Widget.m_InTexSize.x);
		int num4 = (int)((float)num2 * m_Widget.m_InTexSize.y);
		float x = m_Widget.GetWidth() * lossyScale.x * 0.5f;
		Vector3 vector = new Vector3(x, 0f);
		Vector3 vector2 = default(Vector3);
		vector2 = vector / ((float)numberDigits * 0.5f);
		Vector3 vector3 = default(Vector3);
		vector3 = base.gameObject.transform.position + vector;
		for (int i = 0; i < numberDigits; i++)
		{
			m_Widget.AddSprite(new Vector2(vector3.x - ((float)i + 0.5f) * vector2.x, vector3.y - ((float)i + 0.5f) * vector2.y), width, height, lossyScale.x, lossyScale.y, 0f, texU, num3 + num4, texW, num4);
		}
		m_Widget.GetUV(out m_UvLeft, out m_UvTop, out m_UvWidth, out m_UvHeight);
		m_Widget.SetSpriteProxyFlag(0);
	}

	public void SetNumber(int number, int max)
	{
		if (m_Value == number)
		{
			return;
		}
		m_Widget.ShowSprite(0, false);
		int num = Mathf.Abs(number);
		if (num > max)
		{
			num = max;
		}
		m_Value = number;
		int num2 = 1;
		int num3 = 10;
		for (int i = 0; i < numberDigits; i++)
		{
			int num4 = num % num3 / num2;
			int num5 = i + 1;
			if (num > num2 - 1 || i == 0 || m_KeepZeros)
			{
				m_Widget.ClearSpriteProxyFlag(num5);
				m_Widget.ShowSprite(num5, true);
				MFGuiSprite sprite = m_Widget.GetSprite(num5);
				sprite.lowerLeftUV = m_GuiRenderer.PixelCoordToUVCoord((int)(m_UvLeft + m_UvWidth * (float)num4), (int)(m_UvTop + m_UvHeight));
			}
			else
			{
				m_Widget.ShowSprite(num5, false);
				m_Widget.SetSpriteProxyFlag(num5);
			}
			num2 = num3;
			num3 *= 10;
		}
	}
}
