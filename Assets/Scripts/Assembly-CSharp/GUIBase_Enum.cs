using UnityEngine;

[AddComponentMenu("GUI/Widgets/Enum")]
public class GUIBase_Enum : GUIBase_Callback
{
	public delegate void ChangeValueDelegate(int v);

	public GUIBase_Widget[] m_EnumWidgets = new GUIBase_Widget[1];

	public int m_InitValue;

	private GUIBase_Widget m_Widget;

	private int m_CurrentValue;

	private ChangeValueDelegate m_ChangeValueDelegate;

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
			SetValue(m_CurrentValue);
			break;
		}
		return true;
	}

	private void CustomInit()
	{
		if (m_EnumWidgets.Length <= 0)
		{
			return;
		}
		m_InitValue = Mathf.Clamp(m_InitValue, 0, m_EnumWidgets.Length - 1);
		for (int i = 0; i < m_EnumWidgets.Length; i++)
		{
			if ((bool)m_EnumWidgets[i])
			{
				GUIBase_Widget gUIBase_Widget = m_EnumWidgets[i];
				if ((bool)gUIBase_Widget)
				{
					gUIBase_Widget.ShowImmediate(false, true);
				}
			}
		}
	}

	public void SetValue(int v)
	{
		if (m_EnumWidgets.Length > 0)
		{
			int num = v;
			if (v > m_EnumWidgets.Length - 1)
			{
				num = 0;
			}
			else if (v < 0)
			{
				num = m_EnumWidgets.Length - 1;
			}
			if (m_Widget.IsVisible())
			{
				ShowValue(m_CurrentValue, false);
				ShowValue(num, true);
			}
			m_CurrentValue = num;
		}
	}

	private void ShowValue(int i, bool show)
	{
		if ((bool)m_EnumWidgets[i])
		{
			GUIBase_Widget gUIBase_Widget = m_EnumWidgets[i];
			if ((bool)gUIBase_Widget)
			{
				gUIBase_Widget.m_VisibleOnLayoutShow = show;
				gUIBase_Widget.ShowImmediate(show, true);
			}
		}
	}

	public override void ChildButtonPressed(float v)
	{
		int currentValue = m_CurrentValue;
		currentValue = ((!(v >= 0f)) ? (currentValue - 1) : (currentValue + 1));
		SetValue(currentValue);
		if (m_ChangeValueDelegate != null)
		{
			m_ChangeValueDelegate(m_CurrentValue);
		}
	}

	public override void ChildButtonReleased()
	{
	}

	public void RegisterDelegate(ChangeValueDelegate d)
	{
		m_ChangeValueDelegate = d;
	}
}
