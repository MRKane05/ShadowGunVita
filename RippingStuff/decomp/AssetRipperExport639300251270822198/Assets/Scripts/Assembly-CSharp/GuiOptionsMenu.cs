using System.Collections;
using UnityEngine;

[AddComponentMenu("GUI/Menu/GuiOptionsMenu")]
public class GuiOptionsMenu : MonoBehaviour
{
	public delegate void OptionsHideDelegate();

	private const int OPT_PAGE_COUNT = 3;

	public static GuiOptionsMenu Instance;

	private bool m_IsInitialized;

	private int LastOptPageIndex = 1;

	private int m_CurrentOptPage;

	private static string s_PivotOptName = "MainOpt";

	private static string s_LayoutOptSoundsName = "00Sounds_Layout";

	private static string s_LayoutOptControlsName = "00Controls_Layout";

	private static string s_LayoutOptXperiaName = "00Xperia_Layout";

	private static string s_LayoutOptButtonsName = "01Buttons_Layout";

	private static string s_ButtonOptPrevButtonName = "Prev_Button";

	private static string s_ButtonOptNextButtonName = "Next_Button";

	private static string s_ButtonOptResetButtonName = "Reset_Button";

	private static string s_SliderMusicName = "Music_Slider";

	private static string s_SliderSensitivityName = "Sensitivity_Slider";

	private static string s_SwitchSubtitlesName = "Subtitles_Switch";

	private static string s_SwitchInvertYName = "InvertY_Switch";

	private static string s_SwitchLefthandedName = "Lefthanded_Switch";

	private static string s_ControlsSchemeName = "ControlScheme_Enum";

	private static string s_LanguageEnumName = "Language_Enum";

	private static string s_LanguageLeftName = "GUI_enum_left";

	private static string s_LanguageRightName = "GUI_enum_right";

	private static string s_LanguageLabelName = "Language_Label";

	private static string s_XperiaViewEnumName = "ViewPad_Enum";

	private static string s_CustomiseButtonName = "CustomiseButton";

	private static string s_ConfigButtonName = "ConfigButton";

	private static string s_ButtonBackName = "Back_Button";

	private GUIBase_Pivot m_PivotOpt;

	private GUIBase_Pivot m_PivotGpad;

	private GUIBase_Layout m_LayoutOptButtons;

	private GUIBase_Layout m_LayoutOptSounds;

	private GUIBase_Layout m_LayoutOptControls;

	private GUIBase_Layout m_LayoutOptXperia;

	private GUIBase_Layout m_LayoutGpadMessage;

	private GUIBase_Switch m_SwitchYAxis;

	private GUIBase_Switch m_SwitchSubtitles;

	private GUIBase_Switch m_SwitchLefthanded;

	private GUIBase_Slider m_SliderMusic;

	private GUIBase_Slider m_SliderSensitivity;

	private GUIBase_Label m_ControlSchemeLabel;

	private GUIBase_Label m_LeftHandLabel;

	private GUIBase_Enum m_ControlSchemeEnum;

	private GUIBase_Enum m_LanguageEnum;

	private GUIBase_Enum m_XperiaViewEnum;

	private GUIBase_Button m_BackButton;

	private GUIBase_Button m_CustomiseButton;

	private GUIBase_Button m_GpadConfigButton;

	public OptionsHideDelegate m_OnHideDelegate;

	private bool m_RunDelegateAfterPivotHide;

	private bool m_IsShown;

	private ArrayList[] MenuItems = new ArrayList[3];

	private int MenuSelection;

	private MenuController MenuCtrl = new MenuController();

	private bool needUpdateSelection;

	private int[] PrevPageButtonIndex = new int[3];

	private int[] NextPageButtonIndex = new int[3];

	private bool lastGpadConnected;

	private float gpadCheckTime;

	private float m_GpadMessageTimeout;

	private int m_SkipInputFrames = 1;

	private bool HighlightButton;

	private bool showTouchOptions = true;

	private void Awake()
	{
		Instance = this;
		m_IsInitialized = false;
		LastOptPageIndex = ((!Game.DetectXperiaPlayModel()) ? 1 : 2);
	}

	private void Start()
	{
		GuiOptions.Load();
		if ((bool)GuiCustomizeControls.Instance)
		{
			GuiCustomizeControls.Instance.m_OnHideDelegate = OnCustomizeHide;
		}
		if ((bool)custom_inputs.Instance)
		{
			custom_inputs.Instance.m_OnHideDelegate = OnGamepadConfigHide;
		}
		HighlightButton = true;
		showTouchOptions = false;
	}

	public void LateInitialize()
	{
		PrecacheObjects();
		RegisterDelegates();
		ApplyOptionsToWidgets();
		InitMenuItems();
		m_IsInitialized = true;
	}

	private void PrecacheObjects()
	{
		m_PivotOpt = MFGuiManager.Instance.GetPivot(s_PivotOptName);
		m_LayoutOptButtons = GuiBaseUtils.GetLayout(s_LayoutOptButtonsName, m_PivotOpt);
		m_LayoutOptSounds = GuiBaseUtils.GetLayout(s_LayoutOptSoundsName, m_PivotOpt);
		m_LayoutOptXperia = GuiBaseUtils.GetLayout(s_LayoutOptXperiaName, m_PivotOpt);
		m_LayoutOptControls = GuiBaseUtils.GetLayout(s_LayoutOptControlsName, m_PivotOpt);
		m_PivotGpad = MFGuiManager.Instance.GetPivot("GpagMessage_Pivot");
		if ((bool)m_PivotGpad)
		{
			m_LayoutGpadMessage = GuiBaseUtils.GetLayout("GpagMessage_Layout", m_PivotGpad);
		}
	}

	private void RegisterDelegates()
	{
		m_BackButton = GuiBaseUtils.RegisterButtonDelegate(m_LayoutOptButtons, s_ButtonBackName, null, OnTouchOptionsButtonBack);
		GuiBaseUtils.RegisterButtonDelegate(m_LayoutOptButtons, s_ButtonOptPrevButtonName, null, OnTouchButtonOptPrev);
		GuiBaseUtils.RegisterButtonDelegate(m_LayoutOptButtons, s_ButtonOptNextButtonName, null, OnTouchButtonOptNext);
		GuiBaseUtils.RegisterButtonDelegate(m_LayoutOptButtons, s_ButtonOptResetButtonName, null, OnTouchButtonOptReset);
		m_SliderMusic = GuiBaseUtils.RegisterSliderDelegate(m_LayoutOptSounds, s_SliderMusicName, OnMusicSliderChange);
		m_SliderSensitivity = GuiBaseUtils.RegisterSliderDelegate(m_LayoutOptControls, s_SliderSensitivityName, OnSensitivitySliderChange);
		m_SwitchSubtitles = GuiBaseUtils.RegisterSwitchDelegate(m_LayoutOptSounds, s_SwitchSubtitlesName, OnSubtitlesChange);
		m_SwitchYAxis = GuiBaseUtils.RegisterSwitchDelegate(m_LayoutOptControls, s_SwitchInvertYName, OnInvertYChange);
		m_SwitchLefthanded = GuiBaseUtils.RegisterSwitchDelegate(m_LayoutOptControls, s_SwitchLefthandedName, OnLefthandedChange);
		m_ControlSchemeLabel = GuiBaseUtils.PrepareLabel(m_LayoutOptControls, "Control_Label");
		m_LeftHandLabel = GuiBaseUtils.PrepareLabel(m_LayoutOptControls, "Lefthanded_Label");
		m_ControlSchemeEnum = GuiBaseUtils.PrepareEnum(m_LayoutOptControls, s_ControlsSchemeName, OnControlSchemeChanged);
		m_ControlSchemeEnum.SetValue((int)GuiOptions.m_ControlScheme);
		m_LanguageEnum = GuiBaseUtils.PrepareEnum(m_LayoutOptSounds, s_LanguageEnumName, OnLanguageChanged);
		m_LanguageEnum.SetValue((int)GuiOptions.m_Language);
		m_LayoutOptSounds.GetWidget(s_LanguageEnumName).m_VisibleOnLayoutShow = false;
		m_LayoutOptSounds.GetWidget(s_LanguageLeftName).m_VisibleOnLayoutShow = false;
		m_LayoutOptSounds.GetWidget(s_LanguageRightName).m_VisibleOnLayoutShow = false;
		m_LayoutOptSounds.GetWidget(s_LanguageLabelName).m_VisibleOnLayoutShow = false;
		m_XperiaViewEnum = GuiBaseUtils.PrepareEnum(m_LayoutOptXperia, s_XperiaViewEnumName, OnXperiaViewChanged);
		m_XperiaViewEnum.SetValue((int)GuiOptions.m_XperiaPad);
		GUIBase_Widget widget = m_LayoutOptControls.GetWidget(s_CustomiseButtonName, false);
		if ((bool)widget)
		{
			m_CustomiseButton = widget.GetComponent<GUIBase_Button>();
			if ((bool)m_CustomiseButton)
			{
				m_CustomiseButton.RegisterReleaseDelegate(CustomiseButtonDelegate);
			}
		}
		m_GpadConfigButton = GuiBaseUtils.RegisterButtonDelegate(m_LayoutOptControls, s_ConfigButtonName, null, ConfigButtonDelegate);
	}

	private void InitMenuItems()
	{
		MenuItems[0] = new ArrayList();
		if (showTouchOptions)
		{
			MenuItems[0].Add(GuiBaseUtils.GetButton(m_LayoutOptControls, "GUI_enum_left"));
			MenuItems[0].Add(GuiBaseUtils.GetButton(m_LayoutOptControls, "GUI_enum_right"));
		}
		MenuItems[0].Add(GuiBaseUtils.GetButton(m_LayoutOptControls, "GUI_slider_left"));
		MenuItems[0].Add(GuiBaseUtils.GetButton(m_LayoutOptControls, "GUI_slider_right"));
		MenuItems[0].Add(m_SwitchYAxis.m_Buttons[0]);
		MenuItems[0].Add(m_SwitchYAxis.m_Buttons[1]);
		if (showTouchOptions)
		{
			MenuItems[0].Add(m_SwitchLefthanded.m_Buttons[0]);
			MenuItems[0].Add(m_SwitchLefthanded.m_Buttons[1]);
		}
		if (showTouchOptions && (bool)m_CustomiseButton)
		{
			MenuItems[0].Add(m_CustomiseButton);
		}
		MenuItems[1] = new ArrayList();
		if (m_LayoutOptSounds.GetWidget(s_LanguageEnumName).m_VisibleOnLayoutShow)
		{
			MenuItems[1].Add(GuiBaseUtils.GetButton(m_LayoutOptSounds, "GUI_enum_left"));
			MenuItems[1].Add(GuiBaseUtils.GetButton(m_LayoutOptSounds, "GUI_enum_right"));
		}
		MenuItems[1].Add(GuiBaseUtils.GetButton(m_LayoutOptSounds, "GUI_slider_left"));
		MenuItems[1].Add(GuiBaseUtils.GetButton(m_LayoutOptSounds, "GUI_slider_right"));
		MenuItems[1].Add(m_SwitchSubtitles.m_Buttons[0]);
		MenuItems[1].Add(m_SwitchSubtitles.m_Buttons[1]);
		MenuItems[2] = new ArrayList();
		MenuItems[2].Add(GuiBaseUtils.GetButton(m_LayoutOptXperia, "GUI_enum_left"));
		MenuItems[2].Add(GuiBaseUtils.GetButton(m_LayoutOptXperia, "GUI_enum_right"));
		for (int i = 0; i < MenuItems.Length; i++)
		{
			MenuItems[i].Add(GuiBaseUtils.GetButton(m_LayoutOptButtons, s_ButtonOptPrevButtonName));
			PrevPageButtonIndex[i] = MenuItems[i].Count - 1;
			MenuItems[i].Add(GuiBaseUtils.GetButton(m_LayoutOptButtons, s_ButtonOptNextButtonName));
			NextPageButtonIndex[i] = MenuItems[i].Count - 1;
			MenuItems[i].Add(GuiBaseUtils.GetButton(m_LayoutOptButtons, s_ButtonOptResetButtonName));
			MenuItems[i].Add(GuiBaseUtils.GetButton(m_LayoutOptButtons, s_ButtonBackName));
		}
	}

	private void ApplyOptionsToWidgets()
	{
		m_SliderMusic.SetValue(GuiOptions.musicVolume);
		m_SliderSensitivity.SetValue(GuiOptions.sensitivity);
		m_SwitchSubtitles.SetValue(GuiOptions.subtitles);
		m_SwitchYAxis.SetValue(GuiOptions.invertYAxis);
		m_SwitchLefthanded.SetValue(GuiOptions.leftHandAiming);
		m_ControlSchemeEnum.SetValue((int)GuiOptions.m_ControlScheme);
		m_LanguageEnum.SetValue((int)GuiOptions.m_Language);
		m_XperiaViewEnum.SetValue((int)GuiOptions.m_XperiaPad);
		if ((bool)Player.Instance)
		{
			Player.Instance.Controls.TouchControls.OnControlSchemeChange();
		}
		if (GuiOptions.m_Language == GuiOptions.E_Language.English && TextDatabase.instance.databaseLangugae != SystemLanguage.English)
		{
			MFGuiManager.Instance.ChangeLanguage(GuiOptions.E_Language.English);
		}
		else if (GuiOptions.m_Language == GuiOptions.E_Language.Korean && TextDatabase.instance.databaseLangugae != SystemLanguage.Korean)
		{
			MFGuiManager.Instance.ChangeLanguage(GuiOptions.E_Language.Korean);
		}
	}

	private void ShowCurrentOptPage()
	{
		MFGuiManager.Instance.ShowPivot(m_PivotOpt, true);
		if (m_CurrentOptPage > LastOptPageIndex)
		{
			m_CurrentOptPage = 0;
		}
		else if (m_CurrentOptPage < 0)
		{
			m_CurrentOptPage = LastOptPageIndex;
		}
		MFGuiManager.Instance.ShowLayout(m_LayoutOptControls, m_CurrentOptPage == 0);
		MFGuiManager.Instance.ShowLayout(m_LayoutOptSounds, m_CurrentOptPage == 1);
		MFGuiManager.Instance.ShowLayout(m_LayoutOptXperia, m_CurrentOptPage == 2);
		if (!showTouchOptions)
		{
			if (m_CustomiseButton != null && m_CurrentOptPage == 0)
			{
				MFGuiManager.Instance.ShowWidget(m_CustomiseButton.Widget, false, true);
			}
			MFGuiManager.Instance.ShowWidget(m_ControlSchemeEnum.Widget, false, true);
			MFGuiManager.Instance.ShowWidget(m_SwitchLefthanded.Widget, false, true);
			MFGuiManager.Instance.ShowWidget(m_ControlSchemeLabel.Widget, false, true);
			MFGuiManager.Instance.ShowWidget(m_LeftHandLabel.Widget, false, true);
		}
		UpdateConfigButton();
	}

	private void UpdateConfigButton()
	{
		if (m_IsShown && m_GpadConfigButton != null && m_CurrentOptPage == 0)
		{
			MFGuiManager.Instance.ShowWidget(m_GpadConfigButton.Widget, false, true);
		}
	}

	private void OnTouchButtonOptPrev(bool inside)
	{
		if (inside)
		{
			m_CurrentOptPage--;
			ShowCurrentOptPage();
			int menuSelection = PrevPageButtonIndex[m_CurrentOptPage];
			MenuSelection = menuSelection;
			m_SkipInputFrames = 1;
			needUpdateSelection = true;
		}
	}

	private void OnTouchButtonOptNext(bool inside)
	{
		if (inside)
		{
			m_CurrentOptPage++;
			ShowCurrentOptPage();
			int menuSelection = NextPageButtonIndex[m_CurrentOptPage];
			MenuSelection = menuSelection;
			m_SkipInputFrames = 1;
			needUpdateSelection = true;
		}
	}

	private void OnMusicSliderChange(float val)
	{
		GuiOptions.musicVolume = val;
		if (GuiMainMenu.Instance != null)
		{
			GuiMainMenu.Instance.MusicVolume = val;
		}
	}

	private void OnSensitivitySliderChange(float val)
	{
		GuiOptions.sensitivity = val;
	}

	private void OnSubtitlesChange(bool switchValue)
	{
		GuiOptions.subtitles = switchValue;
		if (switchValue)
		{
			MenuSelection++;
		}
		else
		{
			MenuSelection--;
		}
		m_SkipInputFrames = 1;
		needUpdateSelection = true;
	}

	private void OnInvertYChange(bool switchValue)
	{
		GuiOptions.invertYAxis = switchValue;
		if (switchValue)
		{
			MenuSelection++;
		}
		else
		{
			MenuSelection--;
		}
		m_SkipInputFrames = 1;
		needUpdateSelection = true;
	}

	private void OnLefthandedChange(bool switchValue)
	{
		GuiOptions.SetNewLeftHandAiming(switchValue);
		if (switchValue)
		{
			MenuSelection++;
		}
		else
		{
			MenuSelection--;
		}
		m_SkipInputFrames = 1;
		needUpdateSelection = true;
	}

	private void OnTouchButtonOptReset(bool inside)
	{
		if (inside)
		{
			GuiOptions.ResetToDefaults();
			ApplyOptionsToWidgets();
		}
	}

	private void OnTouchOptionsButtonBack(bool inside)
	{
		if (inside)
		{
			GuiOptions.Save();
			if (MusicManager.Instance != null)
			{
				MusicManager.Instance.ApplyVolumeChange();
			}
			Hide();
		}
	}

	private void OnControlSchemeChanged(int val)
	{
		GuiOptions.m_ControlScheme = (GuiOptions.E_ControlScheme)val;
		if ((bool)Player.Instance)
		{
			Player.Instance.Controls.TouchControls.OnControlSchemeChange();
		}
	}

	private void OnLanguageChanged(int val)
	{
		GuiOptions.SetLanguage((val != 0) ? GuiOptions.E_Language.Korean : GuiOptions.E_Language.English);
		MFGuiManager.Instance.ChangeLanguage(GuiOptions.m_Language);
	}

	private void OnXperiaViewChanged(int val)
	{
		GuiOptions.m_XperiaPad = (GuiOptions.E_XperiPad)val;
	}

	private void CustomiseButtonDelegate(bool inside)
	{
		if (inside)
		{
			MFGuiManager.Instance.ShowPivot(m_PivotOpt, false);
			GuiCustomizeControls.Instance.Show();
		}
	}

	private void OnCustomizeHide()
	{
		Show();
	}

	private void ConfigButtonDelegate(bool inside)
	{
		if (inside)
		{
			MFGuiManager.Instance.ShowPivot(m_PivotOpt, false);
			custom_inputs.Instance.ShowConfig();
		}
	}

	private void OnGamepadConfigHide()
	{
		Show();
	}

	private void Hide()
	{
		MFGuiManager.Instance.ShowPivot(m_PivotOpt, false);
		m_RunDelegateAfterPivotHide = true;
		m_IsShown = false;
	}

	public void Show()
	{
		m_IsShown = true;
		ResetSelectionButton();
		ShowCurrentOptPage();
	}

	private void Update()
	{
		if (!m_IsInitialized)
		{
			return;
		}
		if (Time.realtimeSinceStartup > gpadCheckTime)
		{
			bool flag = Game.DetectGpadConnection();
			if (flag != lastGpadConnected)
			{
				lastGpadConnected = flag;
				if (flag && custom_inputs.Instance != null)
				{
					string gpadName = Game.CurrentJoystickName();
					if (custom_inputs.Instance.HasStoredConfig(gpadName) || custom_inputs.Instance.HasDefaultConfig(gpadName))
					{
						custom_inputs.Instance.LoadConfig();
					}
				}
				else
				{
					HideGpadMessage();
				}
				if (m_IsShown)
				{
					UpdateConfigButton();
				}
			}
			gpadCheckTime = Time.realtimeSinceStartup + 2.5f;
		}
		if (m_GpadMessageTimeout > 0f && m_GpadMessageTimeout < Time.realtimeSinceStartup)
		{
			HideGpadMessage();
		}
	}

	private void LateUpdate()
	{
		if (m_IsInitialized && m_RunDelegateAfterPivotHide && !m_PivotOpt.IsVisible())
		{
			m_RunDelegateAfterPivotHide = false;
			if (m_OnHideDelegate != null)
			{
				m_OnHideDelegate();
			}
		}
		if (!m_IsInitialized || !m_IsShown)
		{
			return;
		}
		if (m_SkipInputFrames > 0)
		{
			m_SkipInputFrames--;
			return;
		}
		if (HighlightButton)
		{
			if (MenuSelection == -1)
			{
				HighlightDefaultButton();
				return;
			}
			if (needUpdateSelection)
			{
				UpdateSelectionDelayed();
				needUpdateSelection = false;
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
		else if (MenuCtrl.PressedUp())
		{
			ChangeFocusUp();
		}
		else if (MenuCtrl.PressedDown())
		{
			ChangeFocusDown();
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

	private void ChangeFocusRight()
	{
		if (MenuSelection == -1)
		{
			HighlightDefaultButton();
			return;
		}
		int num = MenuItems[m_CurrentOptPage].Count - 1;
		if (MenuSelection < num)
		{
			int i;
			for (i = MenuSelection + 1; !IsMenuButtonEnabled(i) && i < num; i++)
			{
			}
			if (IsMenuButtonEnabled(i))
			{
				UpdateSelection(i);
			}
		}
	}

	private void ChangeFocusUp()
	{
		if (MenuSelection == -1)
		{
			HighlightDefaultButton();
		}
		else if (MenuSelection >= 1)
		{
			int num = 0;
			if (m_CurrentOptPage == 0)
			{
				int menuSelection = MenuSelection;
				num = MenuSelection - 1;
			}
			else if (m_CurrentOptPage == 1)
			{
				int menuSelection = MenuSelection;
				num = MenuSelection - 1;
			}
			else if (m_CurrentOptPage == 2)
			{
				num = MenuSelection - 1;
			}
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

	private void ChangeFocusDown()
	{
		if (MenuSelection == -1)
		{
			HighlightDefaultButton();
			return;
		}
		int num = MenuItems[m_CurrentOptPage].Count - 1;
		if (MenuSelection < num)
		{
			int i = 0;
			if (m_CurrentOptPage == 0)
			{
				int menuSelection = MenuSelection;
				i = MenuSelection + 1;
			}
			else if (m_CurrentOptPage == 1)
			{
				int menuSelection = MenuSelection;
				i = MenuSelection + 1;
			}
			else if (m_CurrentOptPage == 2)
			{
				i = MenuSelection + 1;
			}
			for (; !IsMenuButtonEnabled(i) && i < num; i++)
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
			m_SkipInputFrames = 1;
			needUpdateSelection = true;
		}
	}

	private void PressBackButton()
	{
		if ((bool)m_BackButton && !m_BackButton.IsDisabled() && m_BackButton.Widget.IsVisible())
		{
			m_BackButton.Callback(GUIBase_Callback.E_CallbackType.E_CT_ON_TOUCH_BEGIN);
			m_BackButton.Callback(GUIBase_Callback.E_CallbackType.E_CT_ON_TOUCH_END);
		}
	}

	private GUIBase_Button GetCurrentMenuItem(int index)
	{
		if (index < 0 || index >= MenuItems[m_CurrentOptPage].Count)
		{
			return null;
		}
		return (GUIBase_Button)MenuItems[m_CurrentOptPage][index];
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

	private void ResetSelectionButton()
	{
		MenuSelection = -1;
		m_SkipInputFrames = 3;
	}

	private void HighlightDefaultButton()
	{
		MenuSelection = 0;
		GUIBase_Button currentMenuItem = GetCurrentMenuItem(MenuSelection);
		if ((bool)currentMenuItem)
		{
			currentMenuItem.Callback(GUIBase_Callback.E_CallbackType.E_CT_ON_MOUSEOVER_BEGIN);
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

	private bool IsMenuButtonEnabled(int newSel)
	{
		GUIBase_Button currentMenuItem = GetCurrentMenuItem(newSel);
		if (!currentMenuItem || !currentMenuItem.Widget.IsVisible())
		{
			return false;
		}
		return true;
	}

	private void ShowGpadMessage()
	{
		if ((bool)m_LayoutGpadMessage)
		{
			MFGuiManager.Instance.ShowLayout(m_LayoutGpadMessage, true);
		}
		m_GpadMessageTimeout = Time.realtimeSinceStartup + 90f;
	}

	public void HideGpadMessage()
	{
		if ((bool)m_LayoutGpadMessage)
		{
			MFGuiManager.Instance.ShowLayout(m_LayoutGpadMessage, false);
			m_GpadMessageTimeout = 0f;
		}
	}
}
