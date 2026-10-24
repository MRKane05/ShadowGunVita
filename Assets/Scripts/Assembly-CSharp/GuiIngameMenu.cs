using System.Collections;
using UnityEngine;

[AddComponentMenu("GUI/Menu/GuiIngameMenu")]
public class GuiIngameMenu : MonoBehaviour
{
	private enum E_MenuState
	{
		Hidden = 0,
		IngameMenu = 1,
		HelpScreen = 2,
		HelpScreenMoga = 3
	}

	public static GuiIngameMenu Instance;

	private GUIBase_Pivot m_Pivot;

	private GUIBase_Layout m_Layout;

	private GUIBase_Pivot m_PivotLoading;

	private GUIBase_Pivot m_PivotHelp;

	private GUIBase_Layout m_LayoutHelp;

	private static string s_PivotName = "IngameMenu";

	private static string s_PivotLoadingName = "Loading";

	private static string s_LayoutName = "Menu_Layout";

	private static string s_ResumeButtonName = "ResumeButton";

	private static string s_OptButtonName = "OptButton";

	private static string s_HelpButtonName = "HelpButton";

	private static string s_RestartButtonName = "RestartButton";

	private static string s_ExitButtonName = "ExitButton";

	private static string s_PivotHelpName = "GamestickHelp_Pivot";

	private static string s_LayoutHelpName = "GamestickHelp_Layout";

	private GUIBase_Pivot m_PivotHelpMoga;

	private GUIBase_Layout m_LayoutHelpMoga;

	private static string s_PivotHelpMogaName = "MogaHelp_Pivot";

	private static string s_LayoutHelpMogaName = "MogaHelp_Layout";

	private static string s_MogaProName = "MogaPro";

	private static string s_MogaPocketName = "MogaPocket";

	private GUIBase_Widget m_MogaPocket_Widget;

	private GUIBase_Widget m_MogaPro_Widget;

	private E_MenuState menuState;

	private ArrayList[] MenuItems = new ArrayList[3];

	private int MenuSelection;

	private MenuController MenuCtrl = new MenuController();

	private bool HighlightButton;

	private GUIBase_Button m_RestartButton;

	private bool exiting;

	private float lastHideTime;

	private bool shownThisFrame;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		GuiOptionsMenu.Instance.m_OnHideDelegate = OnOptionsHide;
		if (custom_inputs.IsNVidiaShield())
		{
			s_PivotHelpName = "ShieldHelp_Pivot";
			s_LayoutHelpName = "ShieldHelp_Layout";
		}
		HighlightButton = true;
	}

	public void Init(Mission mission)
	{
		m_Pivot = MFGuiManager.Instance.GetPivot(s_PivotName);
		if (!m_Pivot)
		{
			Debug.LogError("'" + s_PivotName + "' not found!!! Assert should come now");
			return;
		}
		m_Layout = m_Pivot.GetLayout(s_LayoutName);
		if (!m_Layout)
		{
			Debug.LogError("'" + s_LayoutName + "' not found!!! Assert should come now");
			return;
		}
		m_PivotLoading = MFGuiManager.Instance.GetPivot(s_PivotLoadingName);
		if (!m_PivotLoading)
		{
			Debug.LogError("'" + s_PivotLoadingName + "' not found!");
			return;
		}
		m_PivotHelp = MFGuiManager.Instance.GetPivot(s_PivotHelpName);
		if (!m_PivotHelp)
		{
			Debug.LogError("'" + s_PivotHelpName + "' not found!");
			return;
		}
		m_LayoutHelp = GuiBaseUtils.GetLayout(s_LayoutHelpName, m_PivotHelp);
		GuiBaseUtils.RegisterButtonDelegate(m_LayoutHelp, "Back_Button", null, OnBackFromHelp);
		m_PivotHelpMoga = MFGuiManager.Instance.GetPivot(s_PivotHelpMogaName);
		if (!m_PivotHelpMoga)
		{
			Debug.LogError("'" + s_PivotHelpMogaName + "' not found!");
			return;
		}
		m_LayoutHelpMoga = GuiBaseUtils.GetLayout(s_LayoutHelpMogaName, m_PivotHelpMoga);
		GuiBaseUtils.RegisterButtonDelegate(m_LayoutHelpMoga, "Back_Button", null, OnBackFromHelpMoga);
		m_MogaPro_Widget = m_LayoutHelpMoga.GetWidget(s_MogaProName);
		m_MogaPocket_Widget = m_LayoutHelpMoga.GetWidget(s_MogaPocketName);
		PrepareButton(m_Layout, s_ResumeButtonName, null, ResumeButtonDelegate);
		PrepareButton(m_Layout, s_OptButtonName, null, OptButtonDelegate);
		PrepareButton(m_Layout, s_HelpButtonName, null, HelpButtonDelegate);
		m_RestartButton = PrepareButton(m_Layout, s_RestartButtonName, null, RestartButtonDelegate);
		PrepareButton(m_Layout, s_ExitButtonName, null, ExitButtonDelegate);
		GuiOptionsMenu.Instance.LateInitialize();
		GuiCustomizeControls.Instance.LateInitialize();
		InitMenuItems();
	}

	private void InitMenuItems()
	{
		MenuItems[0] = new ArrayList();
		MenuItems[0].Add(GuiBaseUtils.GetButton(m_Layout, s_ExitButtonName));
		MenuItems[0].Add(GuiBaseUtils.GetButton(m_Layout, s_RestartButtonName));
		MenuItems[0].Add(GuiBaseUtils.GetButton(m_Layout, s_HelpButtonName));
		MenuItems[0].Add(GuiBaseUtils.GetButton(m_Layout, s_OptButtonName));
		MenuItems[0].Add(GuiBaseUtils.GetButton(m_Layout, s_ResumeButtonName));
		MenuItems[1] = new ArrayList();
		MenuItems[1].Add(GuiBaseUtils.GetButton(m_LayoutHelp, "Back_Button"));
		MenuItems[2] = new ArrayList();
		MenuItems[2].Add(GuiBaseUtils.GetButton(m_LayoutHelpMoga, "Back_Button"));
	}

	public void Hide()
	{
		menuState = E_MenuState.Hidden;
		MFGuiManager.Instance.ShowPivot(m_Pivot, false);
		DisableRestartButton(false);
		lastHideTime = Time.realtimeSinceStartup;
	}

	public void Show()
	{
		menuState = E_MenuState.IngameMenu;
		MFGuiManager.Instance.ShowPivot(m_Pivot, true);
		ResetSelectionButton();
		shownThisFrame = true;
	}

	public bool CanShowAgain()
	{
		return Time.realtimeSinceStartup > lastHideTime + 0.5f;
	}

	public void DisableRestartButton(bool disable)
	{
		if ((bool)m_RestartButton)
		{
			m_RestartButton.SetDisabled(disable);
		}
	}

	private GUIBase_Button PrepareButton(GUIBase_Layout layout, string name, GUIBase_Button.TouchDelegate touch, GUIBase_Button.ReleaseDelegate release)
	{
		GUIBase_Button gUIBase_Button = null;
		GUIBase_Widget widget = layout.GetWidget(name);
		if ((bool)widget)
		{
			gUIBase_Button = widget.GetComponent<GUIBase_Button>();
			if ((bool)gUIBase_Button)
			{
				gUIBase_Button.RegisterTouchDelegate(touch);
				gUIBase_Button.RegisterReleaseDelegate(release);
			}
		}
		return gUIBase_Button;
	}

	private void ResumeButtonDelegate(bool inside)
	{
		if (inside)
		{
			GuiHUD.Instance.SwitchBackFromIngameMenu();
		}
	}

	private void OptButtonDelegate(bool inside)
	{
		if (inside)
		{
			Hide();
			GuiOptionsMenu.Instance.Show();
		}
	}

	private void OnOptionsHide()
	{
		Show();
	}

	private void CustomiseButtonDelegate(bool inside)
	{
		if (inside)
		{
			Hide();
			GuiCustomizeControls.Instance.Show();
		}
	}

	private void ExitButtonDelegate(bool inside)
	{
		if (inside && !exiting)
		{
			exiting = true;
			ExitToMainMenu();
		}
	}

	private void HelpButtonDelegate(bool inside)
	{
		if (!inside)
		{
			return;
		}
		Hide();
		if (MogaGamepad.IsConnected())
		{
			menuState = E_MenuState.HelpScreenMoga;
			MFGuiManager.Instance.ShowLayout(m_LayoutHelpMoga, true);
			if (MogaGamepad.IsMogaPro())
			{
				m_MogaPro_Widget.Show(true, true);
			}
			else
			{
				m_MogaPocket_Widget.Show(true, true);
			}
		}
		else
		{
			menuState = E_MenuState.HelpScreen;
			MFGuiManager.Instance.ShowPivot(m_PivotHelp, true);
		}
		ResetSelectionButton();
	}

	private void OnBackFromHelp(bool inside)
	{
		if (inside)
		{
			MFGuiManager.Instance.ShowPivot(m_PivotHelp, false);
			menuState = E_MenuState.IngameMenu;
			ResetSelectionButton();
			Show();
		}
	}

	private void OnBackFromHelpMoga(bool inside)
	{
		if (inside)
		{
			MFGuiManager.Instance.ShowLayout(m_LayoutHelpMoga, false);
			menuState = E_MenuState.IngameMenu;
			ResetSelectionButton();
			Show();
		}
	}

	private void RestartButtonDelegate(bool inside)
	{
		if (inside)
		{
			StartCoroutine(RestartCheckpoint());
		}
	}

	private IEnumerator RestartCheckpoint()
	{
		MFGuiManager.Instance.FadeOut(0.1f);
		Time.timeScale = 1f;
		yield return new WaitForSeconds(0.1f);
		GuiHUD.Instance.SwitchBackFromIngameMenu();
		Mission.Instance.RestartCheckpoint();
	}

	private void ExitToMainMenu()
	{
		Hide();
		Game.Instance.LoadMainMenu();
	}

	public void ShowLoadingScreen()
	{
		MFGuiManager.Instance.ShowPivot(m_PivotLoading, true);
	}

	private void Update()
	{
		if (menuState != E_MenuState.Hidden && !exiting)
		{
		}
	}

	private void LateUpdate()
	{
		if (shownThisFrame)
		{
			shownThisFrame = false;
			return;
		}
		if (HighlightButton)
		{
			if (menuState == E_MenuState.Hidden || exiting)
			{
				return;
			}
			if (MenuSelection == -1)
			{
				HighlightDefaultButton();
				return;
			}
		}
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
		else if (MenuCtrl.PressedBack())
		{
			PressBackButton();
		}
	}

	private void ChangeFocusLeft()
	{
		if (MenuSelection == -1)
		{
			HighlightDefaultButton();
		}
		else if (MenuSelection > 0)
		{
			int num = MenuSelection - 1;
			while (!IsMenuButtonEnabled(num) && num > 0)
			{
				num--;
			}
			if (IsMenuButtonEnabled(num))
			{
				UpdateSelection(num);
			}
		}
	}

	private int CurrentMenu()
	{
		if (menuState == E_MenuState.IngameMenu)
		{
			return 0;
		}
		if (menuState == E_MenuState.HelpScreen)
		{
			return 1;
		}
		if (menuState == E_MenuState.HelpScreenMoga)
		{
			return 2;
		}
		return -1;
	}

	private void ChangeFocusRight()
	{
		if (MenuSelection == -1)
		{
			HighlightDefaultButton();
			return;
		}
		int num = CurrentMenu();
		if (num == -1)
		{
			return;
		}
		int num2 = MenuItems[num].Count - 1;
		if (MenuSelection < num2)
		{
			int i;
			for (i = MenuSelection + 1; !IsMenuButtonEnabled(i) && i < num2; i++)
			{
			}
			if (IsMenuButtonEnabled(i))
			{
				UpdateSelection(i);
			}
		}
	}

	private void PressFocusButton()
	{
		if (MenuSelection == -1)
		{
			HighlightDefaultButton();
			return;
		}
		GUIBase_Button currentMenuItem = GetCurrentMenuItem(MenuSelection);
		if ((bool)currentMenuItem && !currentMenuItem.IsDisabled() && currentMenuItem.Widget.IsVisible())
		{
			currentMenuItem.Callback(GUIBase_Callback.E_CallbackType.E_CT_ON_TOUCH_BEGIN);
			currentMenuItem.Callback(GUIBase_Callback.E_CallbackType.E_CT_ON_TOUCH_END);
		}
	}

	private void PressBackButton()
	{
		int num = CurrentMenu();
		if (num != -1)
		{
			int index = MenuItems[num].Count - 1;
			GUIBase_Button gUIBase_Button = (GUIBase_Button)MenuItems[num][index];
			if ((bool)gUIBase_Button && !gUIBase_Button.IsDisabled() && gUIBase_Button.Widget.IsVisible())
			{
				gUIBase_Button.Callback(GUIBase_Callback.E_CallbackType.E_CT_ON_TOUCH_BEGIN);
				gUIBase_Button.Callback(GUIBase_Callback.E_CallbackType.E_CT_ON_TOUCH_END);
			}
		}
	}

	private GUIBase_Button GetCurrentMenuItem(int index)
	{
		int num = CurrentMenu();
		if (num == -1)
		{
			return null;
		}
		if (index < 0 || index >= MenuItems[num].Count)
		{
			return null;
		}
		return (GUIBase_Button)MenuItems[num][index];
	}

	private void ResetSelectionButton()
	{
		MenuSelection = -1;
	}

	private void HighlightDefaultButton()
	{
		MenuSelection = MenuDefaultButton(menuState);
		GUIBase_Button currentMenuItem = GetCurrentMenuItem(MenuSelection);
		if ((bool)currentMenuItem)
		{
			currentMenuItem.Callback(GUIBase_Callback.E_CallbackType.E_CT_ON_MOUSEOVER_BEGIN);
		}
	}

	private int MenuDefaultButton(E_MenuState s)
	{
		switch (s)
		{
		case E_MenuState.IngameMenu:
			return 4;
		case E_MenuState.HelpScreen:
			return 0;
		case E_MenuState.HelpScreenMoga:
			return 0;
		default:
			return 0;
		}
	}

	private void UpdateSelectionDelayed()
	{
		GUIBase_Button currentMenuItem = GetCurrentMenuItem(MenuSelection);
		if ((bool)currentMenuItem)
		{
			currentMenuItem.Callback(GUIBase_Callback.E_CallbackType.E_CT_ON_MOUSEOVER_BEGIN);
		}
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
