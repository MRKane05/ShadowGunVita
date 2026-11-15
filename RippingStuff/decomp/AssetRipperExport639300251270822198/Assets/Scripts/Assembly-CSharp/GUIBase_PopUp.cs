using UnityEngine;

[AddComponentMenu("GUI/Widgets/Popup")]
public class GUIBase_PopUp : GUIBase_Callback
{
	public delegate void PopUpDelegate(int i);

	public GUIBase_Button[] m_PopUpButtons = new GUIBase_Button[1];

	private GUIBase_Widget m_Widget;

	private PopUpDelegate m_PopUpDelegate;

	private bool m_PopUpButtonsVisible;

	public void Start()
	{
		m_Widget = GetComponent<GUIBase_Widget>();
		m_Widget.RegisterUpdateDelegate(UpdatePopUp);
	}

	public void RegisterPopUpDelegate(PopUpDelegate d)
	{
		m_PopUpDelegate = d;
	}

	public override void ChildButtonPressed(float v)
	{
		ShowPopUpButtons(true);
	}

	public override void ChildButtonReleased()
	{
		if (m_PopUpDelegate == null)
		{
			return;
		}
		Vector2 clickPos = default(Vector2);
		if (Input.touchCount != 0)
		{
			Touch touch = Input.touches[0];
			clickPos.x = touch.position.x;
			clickPos.y = touch.position.y;
		}
		else
		{
			clickPos.x = Input.mousePosition.x;
			clickPos.y = Input.mousePosition.y;
		}
		clickPos.y = (float)Screen.height - clickPos.y;
		for (int i = 0; i < m_PopUpButtons.Length; i++)
		{
			if ((bool)m_PopUpButtons[i])
			{
				GUIBase_Widget widget = m_PopUpButtons[i].Widget;
				if ((bool)widget && widget.IsMouseOver(clickPos))
				{
					m_PopUpDelegate(i);
					break;
				}
			}
		}
	}

	private void ShowPopUpButtons(bool v)
	{
		m_PopUpButtonsVisible = v;
		for (int i = 0; i < m_PopUpButtons.Length; i++)
		{
			if ((bool)m_PopUpButtons[i])
			{
				GUIBase_Widget widget = m_PopUpButtons[i].Widget;
				if ((bool)widget)
				{
					widget.Show(v, true);
				}
			}
		}
	}

	private void UpdatePopUp()
	{
		if (m_PopUpButtonsVisible)
		{
			bool flag = false;
			if (Input.touchCount != 0)
			{
				flag = true;
			}
			else if (Input.GetMouseButton(0))
			{
				flag = true;
			}
			if (!flag)
			{
				ShowPopUpButtons(false);
			}
		}
	}
}
