using System;
using System.Collections;
using UnityEngine;

[AddComponentMenu("")]
public class GUIBase_Font : GUIBase_FontBase
{
	[Serializable]
	public class C_CharDscr
	{
		public int m_Idx;

		public float m_Width;

		public float m_CX;

		public float m_CY;

		public float m_CW;

		public float m_CH;

		public C_CharDscr(int cIdx, float width, float cx, float cy, float cw, float ch)
		{
			m_Idx = cIdx;
			m_Width = width;
			m_CX = cx;
			m_CY = cy;
			m_CW = cw;
			m_CH = ch;
		}
	}

	[Serializable]
	public class C_FontDscr
	{
		public C_CharDscr[] m_CharTable;

		public int m_CharMaxWidth;

		private bool m_IsInitialized;

		private Hashtable m_CharLookUpTable;

		public C_FontDscr()
		{
			m_IsInitialized = false;
		}

		public void SetCharMaxWidth(int maxWidth)
		{
			m_CharMaxWidth = maxWidth;
		}

		public void AddChar(int cIdx, float width, float cx, float cy, float cw, float ch)
		{
			C_CharDscr c_CharDscr = new C_CharDscr(cIdx, width, cx, cy, cw, ch);
			int num = 0;
			if (m_CharTable != null)
			{
				num = m_CharTable.Length;
			}
			C_CharDscr[] array = new C_CharDscr[num + 1];
			if (num != 0)
			{
				m_CharTable.CopyTo(array, 0);
			}
			array[num] = c_CharDscr;
			m_CharTable = array;
		}

		public float GetCharWidth(int cIdx)
		{
			float result = 0f;
			if (Initialize() && m_CharLookUpTable != null && m_CharLookUpTable.ContainsKey(cIdx))
			{
				result = ((C_CharDscr)m_CharLookUpTable[cIdx]).m_Width * (float)m_CharMaxWidth;
			}
			return result;
		}

		public float GetCharHeight(int cIdx)
		{
			float result = 0f;
			if (Initialize() && m_CharLookUpTable != null && m_CharLookUpTable.ContainsKey(cIdx))
			{
				C_CharDscr c_CharDscr = (C_CharDscr)m_CharLookUpTable[cIdx];
				result = c_CharDscr.m_CH - c_CharDscr.m_CY;
			}
			return result;
		}

		public bool GetCharDscr(int cIdx, out float width, ref Vector2 inTexPos, ref Vector2 inTexSize)
		{
			bool result = false;
			width = 0f;
			if (Initialize() && m_CharLookUpTable.ContainsKey(cIdx))
			{
				C_CharDscr c_CharDscr = (C_CharDscr)m_CharLookUpTable[cIdx];
				width = c_CharDscr.m_Width * (float)m_CharMaxWidth;
				inTexPos.x = c_CharDscr.m_CX;
				inTexPos.y = c_CharDscr.m_CY;
				inTexSize.x = c_CharDscr.m_CW - c_CharDscr.m_CX;
				inTexSize.y = c_CharDscr.m_CH - c_CharDscr.m_CY;
				result = true;
			}
			return result;
		}

		public bool GetTextSize(string inText, out Vector2 outSize, bool inTreatSpecialChars, char inMissingChar)
		{
			outSize = Vector2.zero;
			if (!Initialize())
			{
				return false;
			}
			bool flag = inMissingChar > '\0' && m_CharLookUpTable.ContainsKey(inMissingChar);
			for (int i = 0; i < inText.Length; i++)
			{
				int num = inText[i];
				if (!m_CharLookUpTable.ContainsKey(num))
				{
					if (!flag)
					{
						continue;
					}
					num = inMissingChar;
				}
				C_CharDscr c_CharDscr = (C_CharDscr)m_CharLookUpTable[num];
				outSize.x += c_CharDscr.m_Width * (float)m_CharMaxWidth;
				float num2 = c_CharDscr.m_CH - c_CharDscr.m_CY;
				if (num2 > outSize.y)
				{
					outSize.y = num2;
				}
			}
			return true;
		}

		private bool Initialize()
		{
			if ((!m_IsInitialized || m_CharLookUpTable == null) && m_CharTable != null && m_CharTable.Length > 0)
			{
				m_CharLookUpTable = new Hashtable();
				for (int i = 0; i < m_CharTable.Length; i++)
				{
					C_CharDscr c_CharDscr = m_CharTable[i];
					m_CharLookUpTable[c_CharDscr.m_Idx] = c_CharDscr;
				}
				m_IsInitialized = true;
			}
			return m_IsInitialized;
		}
	}

	[SerializeField]
	public C_FontDscr m_FontDscr;

	public float GetCharWidth(int cIdx)
	{
		return m_FontDscr.GetCharWidth(cIdx);
	}

	public float GetCharHeight(int cIdx)
	{
		return m_FontDscr.GetCharHeight(cIdx);
	}

	public bool GetCharDscr(int cIdx, out float width, ref Vector2 inTexPos, ref Vector2 inTexSize)
	{
		return m_FontDscr.GetCharDscr(cIdx, out width, ref inTexPos, ref inTexSize);
	}

	public bool GetTextSize(string inText, out Vector2 inSize, bool inTreatSpecialChars)
	{
		return m_FontDscr.GetTextSize(inText, out inSize, inTreatSpecialChars, ' ');
	}
}
