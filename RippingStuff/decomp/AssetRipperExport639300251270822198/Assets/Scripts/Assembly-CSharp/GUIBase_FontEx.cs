using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[AddComponentMenu("")]
[ExecuteInEditMode]
public class GUIBase_FontEx : GUIBase_FontBase
{
	[Serializable]
	internal class BitmapCharacterSet
	{
		public int MaxCharHeight;

		public int LineHeight;

		public int Base;

		public int RenderedSize;

		public int Width;

		public int Height;

		public BitmapCharacter[] _CharTable = new BitmapCharacter[1];

		public Dictionary<int, BitmapCharacter> Characters = new Dictionary<int, BitmapCharacter>();
	}

	[Serializable]
	internal class BitmapCharacter
	{
		public int Char;

		public int X;

		public int Y;

		public int Width;

		public int Height;

		public int XOffset;

		public int YOffset;

		public int XAdvance;

		public List<Kerning> KerningList = new List<Kerning>();
	}

	[Serializable]
	internal class Kerning
	{
		public int Second;

		public int Amount;
	}

	[SerializeField]
	private BitmapCharacterSet m_CharSet = new BitmapCharacterSet();

	public Material m_Material;

	public TextAsset m_FontDescriptionFile;

	public float m_CorrectionFontSizeScale = 1f;

	private static float m_ErrorFontHeight = 50f;

	private static float m_ErrorFontBase = 25f;

	private List<int> m_MissingChars = new List<int>();

	private void Reset()
	{
		ProcessFontDescriptionAsset();
		m_CharSet._CharTable = new BitmapCharacter[m_CharSet.Characters.Count];
		m_CharSet.Characters.Values.CopyTo(m_CharSet._CharTable, 0);
	}

	private void Awake()
	{
		if (m_Material != null)
		{
			m_Material = UnityEngine.Object.Instantiate(m_Material) as Material;
		}
		if (m_Material == null)
		{
			Debug.LogError("Can not load or instatiate material for font...");
		}
		ProcessFontDescriptionAsset();
		m_CharSet._CharTable = new BitmapCharacter[m_CharSet.Characters.Count];
		m_CharSet.Characters.Values.CopyTo(m_CharSet._CharTable, 0);
	}

	private bool ProcessFontDescriptionAsset()
	{
		if (m_FontDescriptionFile == null)
		{
			Debug.Log("FontDescriptionFile -- was not found");
			return false;
		}
		StringReader stringReader = new StringReader(m_FontDescriptionFile.text);
		if (stringReader == null)
		{
			Debug.Log("FontDescriptionFile not found or is not readable");
			return false;
		}
		ParseFNTFile(stringReader);
		return true;
	}

	private void ParseFNTFile(StringReader inReader)
	{
		m_CharSet = new BitmapCharacterSet();
		m_CharSet.MaxCharHeight = 0;
		char[] separator = new char[2] { ' ', '=' };
		string text;
		while ((text = inReader.ReadLine()) != null)
		{
			string[] array = text.Split(separator);
			if (array[0] == "info")
			{
				for (int i = 1; i < array.Length; i++)
				{
					if (array[i] == "size")
					{
						m_CharSet.RenderedSize = int.Parse(array[i + 1]);
					}
				}
			}
			else if (array[0] == "common")
			{
				for (int j = 1; j < array.Length; j++)
				{
					if (array[j] == "lineHeight")
					{
						m_CharSet.LineHeight = int.Parse(array[j + 1]);
					}
					else if (array[j] == "base")
					{
						m_CharSet.Base = int.Parse(array[j + 1]);
					}
					else if (array[j] == "scaleW")
					{
						m_CharSet.Width = int.Parse(array[j + 1]);
					}
					else if (array[j] == "scaleH")
					{
						m_CharSet.Height = int.Parse(array[j + 1]);
					}
				}
			}
			else if (array[0] == "char")
			{
				int num = 0;
				for (int k = 1; k < array.Length; k++)
				{
					if (array[k] == "id")
					{
						num = int.Parse(array[k + 1]);
						m_CharSet.Characters[num] = new BitmapCharacter();
						m_CharSet.Characters[num].Char = num;
					}
					else if (array[k] == "x")
					{
						m_CharSet.Characters[num].X = int.Parse(array[k + 1]);
					}
					else if (array[k] == "y")
					{
						m_CharSet.Characters[num].Y = int.Parse(array[k + 1]);
					}
					else if (array[k] == "width")
					{
						m_CharSet.Characters[num].Width = int.Parse(array[k + 1]);
					}
					else if (array[k] == "height")
					{
						m_CharSet.Characters[num].Height = int.Parse(array[k + 1]);
						m_CharSet.MaxCharHeight = Mathf.Max(m_CharSet.MaxCharHeight, m_CharSet.Characters[num].Height);
					}
					else if (array[k] == "xoffset")
					{
						m_CharSet.Characters[num].XOffset = int.Parse(array[k + 1]);
					}
					else if (array[k] == "yoffset")
					{
						m_CharSet.Characters[num].YOffset = int.Parse(array[k + 1]);
					}
					else if (array[k] == "xadvance")
					{
						m_CharSet.Characters[num].XAdvance = int.Parse(array[k + 1]);
					}
				}
			}
			else
			{
				if (!(array[0] == "kerning"))
				{
					continue;
				}
				int key = 0;
				Kerning kerning = new Kerning();
				for (int l = 1; l < array.Length; l++)
				{
					if (array[l] == "first")
					{
						key = int.Parse(array[l + 1]);
					}
					else if (array[l] == "second")
					{
						kerning.Second = int.Parse(array[l + 1]);
					}
					else if (array[l] == "amount")
					{
						kerning.Amount = int.Parse(array[l + 1]);
					}
				}
				m_CharSet.Characters[key].KerningList.Add(kerning);
			}
		}
		inReader.Close();
	}

	public bool GetCharDescription(int inCharacter, out float outWidth, out Rect outSprite, out Rect outTexUV)
	{
		return GetCharDescription(inCharacter, out outWidth, out outSprite, out outTexUV, true, true);
	}

	public bool GetCharDescription(int inCharacter, out float outWidth, out Rect outSprite, out Rect outTexUV, bool inNormalizeTextCoord, bool inFliped)
	{
		outWidth = 0f;
		outSprite = default(Rect);
		outTexUV = default(Rect);
		BitmapCharacter characterInfo = GetCharacterInfo(inCharacter);
		if (characterInfo == null)
		{
			return false;
		}
		float left = (float)characterInfo.XOffset * m_CorrectionFontSizeScale;
		float top = (float)characterInfo.YOffset * m_CorrectionFontSizeScale / (float)m_CharSet.Height;
		float num = (float)characterInfo.XAdvance * m_CorrectionFontSizeScale;
		float width = (float)characterInfo.Width * m_CorrectionFontSizeScale;
		float height = (float)characterInfo.Height * m_CorrectionFontSizeScale / (float)m_CharSet.Height;
		outWidth = num;
		outSprite = new Rect(left, top, width, height);
		float left2 = characterInfo.X;
		float num2 = characterInfo.Y;
		float width2 = characterInfo.Width;
		float num3 = characterInfo.Height;
		if (inFliped)
		{
			outTexUV = new Rect(left2, 1f - (num2 + num3), width2, num3);
		}
		else
		{
			outTexUV = new Rect(left2, num2, width2, num3);
		}
		if (inNormalizeTextCoord)
		{
			outTexUV.x /= m_CharSet.Width;
			outTexUV.y /= m_CharSet.Height;
			outTexUV.width /= m_CharSet.Width;
			outTexUV.height /= m_CharSet.Height;
		}
		return true;
	}

	public float GetFontHeight()
	{
		if (m_CharSet == null)
		{
			Debug.LogError("FontEx - GetFontHeight : No charset !!!");
			return m_ErrorFontHeight;
		}
		return m_CharSet.MaxCharHeight;
	}

	public float GetFontBase()
	{
		if (m_CharSet == null)
		{
			Debug.LogError("FontEx - GetFontBase : No charset !!!");
			return m_ErrorFontBase;
		}
		return m_CharSet.Base;
	}

	public float GetCharWidth(int inCharacter)
	{
		BitmapCharacter characterInfo = GetCharacterInfo(inCharacter);
		if (characterInfo == null)
		{
			return 0f;
		}
		return (float)characterInfo.XAdvance * m_CorrectionFontSizeScale;
	}

	private BitmapCharacter GetCharacterInfo(int inCharacter)
	{
		if (m_CharSet == null)
		{
			Debug.LogWarning("FontEx - GetCharDescription : No charset !!!");
			return null;
		}
		if (!m_CharSet.Characters.ContainsKey(inCharacter))
		{
			if (m_CharSet.Characters.ContainsKey(-1))
			{
				inCharacter = -1;
			}
			else
			{
				if (!m_CharSet.Characters.ContainsKey(32))
				{
					return null;
				}
				inCharacter = 32;
			}
		}
		return m_CharSet.Characters[inCharacter];
	}
}
