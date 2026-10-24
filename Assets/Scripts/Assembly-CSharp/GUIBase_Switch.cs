using UnityEngine;

[AddComponentMenu("GUI/Widgets/Switch")]
public class GUIBase_Switch : GUIBase_Callback
{
	public delegate void SwitchDelegate(bool switchValue);

	public GUIBase_Button[] m_Buttons = new GUIBase_Button[2];

	public bool m_InitValue;

	private GUIBase_Widget m_Widget;

	private bool m_Value;

	private SwitchDelegate m_SwitchDelegate;

	public GUIBase_Widget Widget
	{
		get
		{
			return m_Widget;
		}
	}

	public void SetValue(bool val)
	{
		m_Value = val;
		if (m_Widget.IsVisible())
		{
			ShowSwitchButton();
		}
	}

	public bool GetValue()
	{
		return m_Value;
	}

	public void Start()
	{
		m_Widget = GetComponent<GUIBase_Widget>();
		int clbkTypes = 6;
		m_Widget.RegisterCallback(this, clbkTypes);
		m_Value = m_InitValue;
	}

	public override bool Callback(E_CallbackType type)
	{
		switch (type)
		{
		case E_CallbackType.E_CT_SHOW:
			ShowSwitchButton();
			break;
		case E_CallbackType.E_CT_HIDE:
			HideButtons();
			break;
		}
		return true;
	}

	public void RegisterDelegate(SwitchDelegate d)
	{
		m_SwitchDelegate = d;
	}

	public override void ChildButtonPressed(float v)
	{
		m_Value = v == 1f;
		ShowSwitchButton();
		if (m_SwitchDelegate != null)
		{
			m_SwitchDelegate(m_Value);
		}
	}

	private void ShowSwitchButton()
	{
		int num = (m_Value ? 1 : 0);
		for (int i = 0; i < m_Buttons.Length; i++)
		{
			if ((bool)m_Buttons[i])
			{
				GUIBase_Widget widget = m_Buttons[i].Widget;
				if ((bool)widget)
				{
					widget.Show(i == num, true);
				}
			}
		}
	}

	private void HideButtons()
	{
		for (int i = 0; i < m_Buttons.Length; i++)
		{
			if ((bool)m_Buttons[i])
			{
				GUIBase_Widget widget = m_Buttons[i].Widget;
				if ((bool)widget)
				{
					widget.Show(false, true);
				}
			}
		}
	}

	public override void ChildButtonReleased()
	{
	}
}
