using UnityEngine;

[AddComponentMenu("GUI/Widgets/Label")]
public class GUIBase_Label : GUIBase_Callback
{
	public int m_TextID;

	private int m_TextIDGenerated;

	[SerializeField]
	private string m_Text;

	[SerializeField]
	private string m_TextDyn;

	private string m_TextGenerated = string.Empty;

	[SerializeField]
	private GUIBase_Font m_Font;

	private Material m_FontMat;

	public TextAnchor m_AnchorPoint = TextAnchor.MiddleCenter;

	private int m_AnchorPointGen = -1;

	public TextAlignment m_Alignment;

	private int m_AlignmentGen = -1;

	private GUIBase_Widget m_Widget;

	[SerializeField]
	private float m_LineSpace2;

	private float m_LineSpaceGenerated;

	private Vector2 m_LineSize;

	private bool m_CustomInitCalled;

	private bool m_RegenerationNeeded;

	public static GUIBase_FontEx m_FontEx;

	public bool m_UseFontExGen;

	public string text
	{
		get
		{
			return (m_TextID <= 0 || m_TextDyn == null) ? m_Text : m_TextDyn;
		}
	}

	public bool isValid
	{
		get
		{
			return text != null && text != string.Empty && (bool)font;
		}
	}

	public Vector2 textSize
	{
		get
		{
			return GetTextSize();
		}
	}

	public GUIBase_Widget Widget
	{
		get
		{
			return m_Widget;
		}
	}

	public Vector2 lineSize
	{
		get
		{
			return m_LineSize;
		}
	}

	public float lineSpace
	{
		get
		{
			return m_LineSpace2 * 0.01f;
		}
	}

	public TextAlignment alignment
	{
		get
		{
			return m_Alignment;
		}
	}

	public static bool useFontEx
	{
		get
		{
			return m_FontEx != null;
		}
	}

	public GUIBase_FontBase font
	{
		get
		{
			return (!useFontEx) ? ((GUIBase_FontBase)m_Font) : ((GUIBase_FontBase)m_FontEx);
		}
	}

	public Texture2D fontTexture
	{
		get
		{
			if (useFontEx)
			{
				if ((bool)m_FontEx.m_Material)
				{
					return (Texture2D)m_FontEx.m_Material.mainTexture;
				}
			}
			else
			{
				if (m_Widget != null)
				{
					return (Texture2D)m_Widget.GetTexture();
				}
				GUIBase_Widget component = GetComponent<GUIBase_Widget>();
				if (component != null)
				{
					return (Texture2D)component.GetTexture();
				}
			}
			return null;
		}
	}

	public void Start()
	{
		m_Widget = GetComponent<GUIBase_Widget>();
		int clbkTypes = 3;
		m_Widget.RegisterCallback(this, clbkTypes);
		m_FontMat = m_Widget.m_Material;
		m_TextDyn = null;
		m_TextIDGenerated = 0;
		m_TextGenerated = string.Empty;
		m_AnchorPointGen = -1;
		m_AlignmentGen = -1;
		m_LineSpaceGenerated = 0f;
		if (m_TextID != 0)
		{
			m_TextDyn = TextDatabase.instance[m_TextID];
		}
	}

	public override bool Callback(E_CallbackType type)
	{
		switch (type)
		{
		case E_CallbackType.E_CT_INIT:
			return true;
		case E_CallbackType.E_CT_SHOW:
			if (!m_CustomInitCalled)
			{
				CustomInit();
			}
			return true;
		default:
			return false;
		}
	}

	private void CustomInit()
	{
		if (IsDataRegenerationNeaded())
		{
			m_Widget.HACK_Label_RemoveSprite();
			GenerateRunTimeData();
		}
		if (text != null && text.Length > 0)
		{
			Vector3 lossyScale = base.gameObject.transform.lossyScale;
			Texture texture = fontTexture;
			int num = ((!texture) ? 1 : texture.width);
			int num2 = ((!texture) ? 1 : texture.height);
			float num3 = m_LineSize.y * (float)num2;
			float x = m_LineSize.x;
			float num4 = m_Widget.GetWidth() / x;
			Vector3 inCursor = GetLeftUpPos();
			float x2 = inCursor.x;
			inCursor.y += num3 * lossyScale.y * 0.5f;
			bool flag = IsMultiline(text);
			if (flag)
			{
				inCursor = SetupCursorForTextAlign(inCursor, text, 0, m_Alignment, font, x2, x, num4 * lossyScale.x);
			}
			for (int i = 0; i < text.Length; i++)
			{
				int num5 = text[i];
				int num6 = num5;
				float outWidth;
				Rect outSprite;
				Rect outTexUV;
				if (num6 == 10)
				{
					if (flag)
					{
						inCursor = SetupCursorForTextAlign(inCursor, text, i + 1, m_Alignment, font, x2, x, num4 * lossyScale.x);
					}
					else
					{
						inCursor.x = x2;
					}
					inCursor.y += (num3 + lineSpace * (float)num2) * lossyScale.y;
					m_Widget.AddSprite(new Vector2(inCursor.x, inCursor.y), 0f, 0f, lossyScale.x, lossyScale.y, 0f, 0, 0, 0, 0);
				}
				else if (!useFontEx)
				{
					Vector2 inTexPos = default(Vector2);
					Vector2 inTexSize = default(Vector2);
					float width;
					m_Font.GetCharDscr(num5, out width, ref inTexPos, ref inTexSize);
					width *= num4;
					int texU = (int)((float)num * inTexPos.x);
					int num7 = (int)((float)num2 * inTexPos.y);
					int texW = (int)((float)num * inTexSize.x);
					int num8 = (int)((float)num2 * inTexSize.y);
					inCursor.x += 0.5f * width * lossyScale.x;
					m_Widget.AddSprite(new Vector2(inCursor.x, inCursor.y), width, num3, lossyScale.x, lossyScale.y, 0f, texU, num7 + num8, texW, num8);
					inCursor.x += 0.5f * width * lossyScale.x;
				}
				else if (m_FontEx.GetCharDescription(text[i], out outWidth, out outSprite, out outTexUV, false, false))
				{
					outWidth *= num4;
					Vector2 vector = new Vector2(outSprite.width, outSprite.height * (float)num2);
					Vector2 centerSpritePos = new Vector2(inCursor.x, inCursor.y) + new Vector2((outSprite.x + outSprite.width) * 0.5f * lossyScale.x, 0f);
					m_Widget.AddSprite(centerSpritePos, vector.x, vector.y, lossyScale.x, lossyScale.y, 0f, (int)outTexUV.x, (int)(outTexUV.y + outTexUV.height), (int)outTexUV.width, (int)outTexUV.height);
					inCursor.x += outWidth * lossyScale.x;
				}
			}
		}
		m_CustomInitCalled = true;
		m_Widget.SetModify();
	}

	public void SetNewText(string inText)
	{
		m_TextID = 0;
		m_TextIDGenerated = 0;
		m_Text = inText;
		if (m_CustomInitCalled)
		{
			CustomInit();
		}
	}

	public void SetNewText(int inTextID)
	{
		m_TextID = inTextID;
		m_TextIDGenerated = 0;
		if (m_CustomInitCalled)
		{
			CustomInit();
		}
	}

	public void Clear()
	{
		m_TextID = 0;
		m_TextIDGenerated = -1;
		m_TextDyn = string.Empty;
		m_Text = string.Empty;
		if (m_CustomInitCalled)
		{
			CustomInit();
		}
	}

	public static bool IsMultiline(string inText)
	{
		return inText.IndexOf('\n') >= 0;
	}

	private static float GetCurentTextLineWidth(string inText, int inStartIndex, GUIBase_FontBase inFont)
	{
		int num = inText.IndexOf('\n', inStartIndex);
		int num2 = ((num != -1) ? (num - inStartIndex) : (inText.Length - inStartIndex));
		if (num2 <= 0)
		{
			return -1f;
		}
		string subString = inText.Substring(inStartIndex, num2);
		return GetLineWidth(subString, inFont);
	}

	private static float GetLineWidth(string subString, GUIBase_FontBase inFont)
	{
		float num = 0f;
		GUIBase_FontEx gUIBase_FontEx = inFont as GUIBase_FontEx;
		GUIBase_Font gUIBase_Font = inFont as GUIBase_Font;
		if (gUIBase_FontEx != null)
		{
			for (int i = 0; i < subString.Length; i++)
			{
				num += gUIBase_FontEx.GetCharWidth(subString[i]);
			}
		}
		else
		{
			for (int j = 0; j < subString.Length; j++)
			{
				num += gUIBase_Font.GetCharWidth(subString[j]);
			}
		}
		return num;
	}

	private Vector2 GetTextSize()
	{
		Vector2 inTexPos = Vector2.zero;
		Vector2 zero = Vector2.zero;
		Vector2 zero2 = Vector2.zero;
		Vector2 zero3 = Vector2.zero;
		int num = 1;
		float num2 = 0f;
		for (int i = 0; i < text.Length; i++)
		{
			int num3 = text[i];
			int num4 = num3;
			if (num4 == 10)
			{
				zero3.x = Mathf.Max(zero3.x, zero2.x);
				zero2.x = 0f;
				num++;
			}
			else if (!useFontEx)
			{
				zero = Vector2.zero;
				num2 = 0f;
				if (m_Font.GetCharDscr(num3, out num2, ref inTexPos, ref zero))
				{
					zero2.x += num2;
					zero2.y = Mathf.Max(zero2.y, zero.y);
				}
			}
			else
			{
				num2 = 0f;
				Rect outSprite;
				Rect outTexUV;
				if (m_FontEx.GetCharDescription(text[i], out num2, out outSprite, out outTexUV, true, false))
				{
					zero2.x += num2;
					zero2.y = Mathf.Max(zero2.y, outSprite.height);
				}
			}
		}
		m_LineSize = zero2;
		zero3.x = Mathf.Max(zero3.x, zero2.x);
		m_LineSize.x = zero3.x;
		zero3.y = zero2.y * (float)num + (float)(num - 1) * lineSpace;
		return zero3;
	}

	internal bool IsDataRegenerationNeaded()
	{
		if (m_TextID != m_TextIDGenerated)
		{
			return true;
		}
		if (text != m_TextGenerated)
		{
			return true;
		}
		if (m_RegenerationNeeded)
		{
			return true;
		}
		return false;
	}

	public bool GenerateRunTimeData()
	{
		bool flag = IsDataRegenerationNeaded();
		if (flag)
		{
			if (font == null)
			{
				Debug.LogWarning("GUIBase_Label have not a font assigned " + DebugUtils.GetFullName(base.gameObject));
			}
			else
			{
				GUIBase_Widget component = GetComponent<GUIBase_Widget>();
				m_TextDyn = null;
				m_TextIDGenerated = 0;
				if (m_TextID != 0)
				{
					m_TextDyn = TextDatabase.instance[m_TextID];
					m_TextIDGenerated = m_TextID;
				}
				Texture2D texture2D = fontTexture;
				float num = texture2D.height;
				Vector2 vector = GetTextSize();
				component.SetScreenSize(vector.x, vector.y * num);
				m_AnchorPointGen = (int)m_AnchorPoint;
				m_AlignmentGen = (int)m_Alignment;
				m_LineSpaceGenerated = m_LineSpace2;
				m_UseFontExGen = useFontEx;
				m_RegenerationNeeded = false;
			}
		}
		return flag;
	}

	public Vector3 GetLeftUpPos()
	{
		return GetLeftUpPos(m_Widget);
	}

	public Vector3 GetLeftUpPos(GUIBase_Widget inWidget)
	{
		Vector3 position = base.transform.position;
		Vector3 lossyScale = base.transform.lossyScale;
		Vector3 vector = new Vector3(inWidget.GetWidth() * lossyScale.x, inWidget.GetHeight() * lossyScale.y, 0f);
		Vector3 result = position;
		switch (m_AnchorPoint)
		{
		case TextAnchor.UpperLeft:
			result.x -= vector.x * 0f;
			result.y -= vector.y * 0f;
			break;
		case TextAnchor.UpperCenter:
			result.x -= vector.x * 0.5f;
			result.y -= vector.y * 0f;
			break;
		case TextAnchor.UpperRight:
			result.x -= vector.x * 1f;
			result.y -= vector.y * 0f;
			break;
		case TextAnchor.MiddleLeft:
			result.x -= vector.x * 0f;
			result.y -= vector.y * 0.5f;
			break;
		case TextAnchor.MiddleCenter:
			result.x -= vector.x * 0.5f;
			result.y -= vector.y * 0.5f;
			break;
		case TextAnchor.MiddleRight:
			result.x -= vector.x * 1f;
			result.y -= vector.y * 0.5f;
			break;
		case TextAnchor.LowerLeft:
			result.x -= vector.x * 0f;
			result.y -= vector.y * 1f;
			break;
		case TextAnchor.LowerCenter:
			result.x -= vector.x * 0.5f;
			result.y -= vector.y * 1f;
			break;
		case TextAnchor.LowerRight:
			result.x -= vector.x * 1f;
			result.y -= vector.y * 1f;
			break;
		}
		return result;
	}

	public static Vector3 SetupCursorForTextAlign(Vector3 inCursor, string inText, int inStartIndex, TextAlignment inAlignment, GUIBase_FontBase inFont, float inLineBegin, float inMaxlineWidth, float inScale)
	{
		Vector3 result = inCursor;
		result.x = inLineBegin;
		if (inAlignment != TextAlignment.Left)
		{
			float curentTextLineWidth = GetCurentTextLineWidth(inText, inStartIndex, inFont);
			float num = (inMaxlineWidth - curentTextLineWidth) * inScale;
			switch (inAlignment)
			{
			case TextAlignment.Center:
				result.x += num * 0.5f;
				break;
			case TextAlignment.Right:
				result.x += num;
				break;
			}
		}
		return result;
	}

	public static void Reload(string inLanguage)
	{
		if ("English.Old" == inLanguage)
		{
			m_FontEx = null;
		}
		else if ("English" == inLanguage)
		{
			m_FontEx = MFGuiManager.Instance.GetFontForLanguage(SystemLanguage.English);
		}
		else if ("Korean" == inLanguage)
		{
			m_FontEx = MFGuiManager.Instance.GetFontForLanguage(SystemLanguage.Korean);
		}
		GUIBase_Label[] array = Object.FindObjectsOfType(typeof(GUIBase_Label)) as GUIBase_Label[];
		GUIBase_Label[] array2 = array;
		foreach (GUIBase_Label gUIBase_Label in array2)
		{
			gUIBase_Label.Reload_Internal(inLanguage);
		}
	}

	private void Reload_Internal(string inLanguage)
	{
		Material material = null;
		if ("English.Old" == inLanguage)
		{
			material = m_FontMat;
		}
		else if ("English" == inLanguage)
		{
			material = m_FontEx.m_Material;
		}
		else if ("Korean" == inLanguage)
		{
			material = m_FontEx.m_Material;
		}
		if (material != null)
		{
			m_RegenerationNeeded = true;
			m_Widget.HACK_Label_RemoveSprite();
			m_Widget.ChangeMaterial(material);
			ConditionalInit();
		}
	}

	private void ConditionalInit()
	{
		if (m_CustomInitCalled)
		{
			CustomInit();
		}
	}

	private void ChangeMaterial_Internal(Material inMaterial)
	{
		if (m_Widget != null)
		{
			m_Widget.ChangeMaterial(inMaterial);
		}
	}
}
