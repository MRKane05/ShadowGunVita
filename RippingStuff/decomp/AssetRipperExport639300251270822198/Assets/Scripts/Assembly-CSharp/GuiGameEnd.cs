using System.Collections;
using UnityEngine;

[AddComponentMenu("GUI/Game/Game End")]
public class GuiGameEnd : MonoBehaviour, IGameZoneControledObject
{
	public EventTarget onEvents = new EventTarget();

	public EventSender setEventA = new EventSender();

	public EventSender setEventB = new EventSender();

	private GUIBase_Pivot m_PivotFinal;

	private bool m_IsInitialized;

	private bool m_IsRunning;

	private ArrayList MenuItems = new ArrayList();

	private int MenuSelection = -1;

	private MenuController MenuCtrl = new MenuController();

	public static GuiGameEnd Instance;

	public static void Suspend(bool suspend)
	{
		if ((bool)Instance && Instance.m_IsRunning)
		{
			Instance.Show(!suspend);
		}
	}

	public void Enable()
	{
		m_IsRunning = false;
	}

	public void Disable()
	{
		if (m_IsRunning)
		{
			Show(false);
			m_IsRunning = false;
		}
	}

	public void Reset()
	{
		Disable();
		Enable();
	}

	private void Awake()
	{
		Instance = this;
		onEvents.OnActivate = OnActivate;
		GameZone firstComponentUpward = base.gameObject.GetFirstComponentUpward<GameZone>();
		if (firstComponentUpward == null)
		{
			Debug.LogError("This object must be a child of GameZone: '" + base.name + "'");
		}
		else
		{
			firstComponentUpward.RegisterControllableObject(this);
		}
	}

	private void Start()
	{
		onEvents.Initialize();
	}

	private void OnDestroy()
	{
		Instance = null;
	}

	private void LateUpdate()
	{
		if (!m_IsInitialized)
		{
			LateInit();
		}
	}

	private void LateInit()
	{
		m_IsInitialized = true;
		m_PivotFinal = MFGuiManager.Instance.GetPivot("FinalDialog");
		if (!m_PivotFinal)
		{
			Debug.LogError("Pivot 'FinalDialog'  not found");
			return;
		}
		GUIBase_Layout layout = GuiBaseUtils.GetLayout("FinalDialog_Layout", m_PivotFinal);
		GuiBaseUtils.RegisterButtonDelegate(layout, "Button_EndingA", null, OnButtonEndingA);
		GuiBaseUtils.RegisterButtonDelegate(layout, "Button_EndingB", null, OnButtonEndingB);
		MenuItems = new ArrayList();
		MenuItems.Add(GuiBaseUtils.GetButton(layout, "Button_EndingB"));
		MenuItems.Add(GuiBaseUtils.GetButton(layout, "Button_EndingA"));
	}

	private void Show(bool show)
	{
		if (m_PivotFinal != null)
		{
			m_PivotFinal.Show(show);
		}
		if (show)
		{
			Screen.lockCursor = false;
			if ((bool)GuiHUD.Instance)
			{
				GuiHUD.Instance.Hide();
			}
		}
	}

	private void OnActivate()
	{
		m_IsRunning = true;
		Show(true);
		Time.timeScale = 0f;
	}

	private void OnButtonEndingA(bool inside)
	{
		if (inside)
		{
			Time.timeScale = 1f;
			Show(false);
			setEventA.Send();
			m_IsRunning = false;
		}
	}

	private void OnButtonEndingB(bool inside)
	{
		if (inside)
		{
			Time.timeScale = 1f;
			Show(false);
			setEventB.Send();
			m_IsRunning = false;
		}
	}

	private void Update()
	{
		if (m_IsInitialized && m_IsRunning)
		{
			MenuCtrl.Update();
			if (MenuCtrl.PressedLeft())
			{
				ChangeFocusLeft();
			}
			else if (MenuCtrl.PressedRight())
			{
				ChangeFocusRight();
			}
			else if (MenuCtrl.PressedOk())
			{
				PressFocusButton();
			}
		}
	}

	private void ChangeFocusLeft()
	{
		if (MenuSelection != 0)
		{
			int newSel = 0;
			if (IsMenuButtonEnabled(newSel))
			{
				UpdateSelection(newSel);
			}
		}
	}

	private void ChangeFocusRight()
	{
		if (MenuSelection != 1)
		{
			int newSel = 1;
			if (IsMenuButtonEnabled(newSel))
			{
				UpdateSelection(newSel);
			}
		}
	}

	private void PressFocusButton()
	{
		GUIBase_Button currentMenuItem = GetCurrentMenuItem(MenuSelection);
		if ((bool)currentMenuItem && !currentMenuItem.IsDisabled() && currentMenuItem.Widget.IsVisible())
		{
			currentMenuItem.Callback(GUIBase_Callback.E_CallbackType.E_CT_ON_TOUCH_BEGIN);
			currentMenuItem.Callback(GUIBase_Callback.E_CallbackType.E_CT_ON_TOUCH_END);
		}
	}

	private GUIBase_Button GetCurrentMenuItem(int index)
	{
		if (index < 0 || index >= MenuItems.Count)
		{
			return null;
		}
		return (GUIBase_Button)MenuItems[index];
	}

	private void UpdateSelection(int newSel)
	{
		GUIBase_Button currentMenuItem = GetCurrentMenuItem(MenuSelection);
		if ((bool)currentMenuItem)
		{
			currentMenuItem.Callback(GUIBase_Callback.E_CallbackType.E_CT_ON_MOUSEOVER_END);
		}
		currentMenuItem = GetCurrentMenuItem(newSel);
		if ((bool)currentMenuItem)
		{
			currentMenuItem.Callback(GUIBase_Callback.E_CallbackType.E_CT_ON_MOUSEOVER_BEGIN);
		}
		MenuSelection = newSel;
	}

	private bool IsMenuButtonEnabled(int newSel)
	{
		GUIBase_Button currentMenuItem = GetCurrentMenuItem(newSel);
		if (!currentMenuItem || currentMenuItem.IsDisabled())
		{
			return false;
		}
		return true;
	}
}
