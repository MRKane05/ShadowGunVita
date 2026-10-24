using System.Collections;
using UnityEngine;

[AddComponentMenu("GUI/Menu/GuiMainMenu")]
public class GuiMainMenu : MonoBehaviour
{
	private enum E_MenuState
	{
		E_MS_NONE = 0,
		E_MS_MAIN = 1,
		E_MS_ABOUT = 2,
		E_MS_MORE = 3,
		E_MS_MORE_GAMES = 4,
		E_MS_OPT = 5,
		E_MS_HELP = 6,
		E_MS_HELP_MOGA = 7,
		E_MS_PLAY = 8,
		E_MS_GAME = 9,
		E_MS_PLAY_DIF = 10,
		E_MS_PLAY_LEV = 11,
		E_MS_PLAY_LEV_DLC = 12,
		E_MS_LOADING = 13
	}

	private const int dlcCnt = 2;

	private const int numExtras = 20;

	public static GuiMainMenu Instance;

	private bool m_IsInitialized;

	private E_MenuState m_CurrentState;

	private static string s_PivotMainName = "Main";

	private static string s_PivotAboutName = "MainAbout";

	private static string s_PivotMoreName = "MainMore";

	private static string s_PivotExtrasName = "MainMoreButtons";

	private static string s_PivotPlayName = "MainPlay";

	private static string s_PivotPlayDifName = "MainPlayDif";

	private static string[] s_PivotPlayLevName = new string[2] { "MainPlayLev", "MainPlayLevGame02" };

	private static string s_PivotLoadingName = "Loading";

	private static string s_PivotAlwaysName = "AlwaysVisible";

	private static string s_PivotMoreGamesName = "MainSale";

	private static string s_PivotGameName = "MainGame";

	private static string s_PivotHelpName = "GamestickHelp_Pivot";

	private static string s_LayoutHelpButtonsName = "GamestickHelp_Layout";

	private static string s_LayoutMainButtonsName = "01Buttons_Layout";

	private static string s_LayoutAboutButtonsName = "01Buttons_Layout";

	private static string s_LayoutMoreButtonsName = "01Buttons_Layout";

	private static string s_LayoutPlayButtonsName = "01Buttons_Layout";

	private static string s_LayoutGameButtonsName = "01Buttons_Layout";

	private static string s_LayoutPlayDifButtonsName = "01Buttons_Layout";

	private static string s_LayoutPlayLevButtonsName = "01Buttons_Layout";

	private static string s_LayoutPlayConfirmName = "00Confirm_Layout";

	private static string s_ButtonPlayName = "Play_Button";

	private static string s_ButtonPlayDemoName = "PlayDemo_Button";

	private static string s_ButtonShowcaseName = "Showcase_Button";

	private static string s_ButtonAboutName = "About_Button";

	private static string s_ButtonMoreName = "More_Button";

	private static string s_ButtonOptName = "Opt_Button";

	private static string s_ButtonHelpName = "Help_Button";

	private static string s_ButtonBackName = "Back_Button";

	private static string s_ButtonResumeName = "Res_Button";

	private static string s_ButtonNewGameName = "New_Button";

	private static string s_ButtonPlayLevelName = "Lev_Button";

	private static string s_ButtonPlayConfirmYesName = "Yes_Button";

	private static string s_ButtonPlayConfirmNoName = "No_Button";

	private static string s_ButtonPlayDif1ButtonName = "Easy_Button";

	private static string s_ButtonPlayDif2ButtonName = "Norm_Button";

	private static string s_ButtonPlayDif3ButtonName = "Hard_Button";

	private static string s_ButtonMoreGames = "MoreGames_Button";

	private static string s_ButtonExitName = "Exit_Button";

	private GUIBase_Pivot m_PivotMain;

	private GUIBase_Pivot m_PivotAbout;

	private GUIBase_Pivot m_PivotMore;

	private GUIBase_Pivot m_PivotExtrasLayouts;

	private GUIBase_Pivot m_PivotHelp;

	private GUIBase_Pivot m_PivotPlay;

	private GUIBase_Pivot m_PivotGame;

	private GUIBase_Pivot m_PivotPlayDif;

	private GUIBase_Pivot[] m_PivotPlayLev = new GUIBase_Pivot[2];

	private GUIBase_Pivot m_PivotLoading;

	private GUIBase_Pivot m_PivotAlwaysVisible;

	private GUIBase_Pivot m_PivotMoreGames;

	private GUIBase_Layout m_LayoutMainButtons;

	private GUIBase_Layout m_LayoutAboutButtons;

	private GUIBase_Layout m_LayoutMoreButtons;

	private GUIBase_Layout m_LayoutHelpButtons;

	private GUIBase_Layout m_LayoutPlayButtons;

	private GUIBase_Layout m_LayoutGameButtons;

	private GUIBase_Layout m_LayoutPlayDifButtons;

	private GUIBase_Layout[] m_LayoutPlayLevButtons = new GUIBase_Layout[2];

	private GUIBase_Layout m_LayoutPlayConfirm;

	private static string s_PivotHelpMogaName = "MogaHelp_Pivot";

	private static string s_LayoutHelpMogaName = "MogaHelp_Layout";

	private static string s_MogaProName = "MogaPro";

	private static string s_MogaPocketName = "MogaPocket";

	private GUIBase_Pivot m_PivotHelpMoga;

	private GUIBase_Layout m_LayoutHelpMogaButtons;

	private GUIBase_Widget m_MogaPocket_Widget;

	private GUIBase_Widget m_MogaPro_Widget;

	private static GUIBase_Button[][] m_LevelsButtons = new GUIBase_Button[2][];

	private static GUIBase_Button m_ResumePlayButton = null;

	private static GUIBase_Button m_PlayNewButton = null;

	private static GUIBase_Button m_PlayLevelButton = null;

	private static GUIBase_Button m_BackButtonPlay = null;

	private static GUIBase_Button m_GameOrig = null;

	private static GUIBase_Button m_GameDlc = null;

	private static GUIBase_Switch m_UnlockAllLevels;

	private static GUIBase_Switch m_UnlockAllExtras;

	private GUIBase_Button[] m_ExtrasButtons;

	private GUIBase_Layout[] m_ExtrasLayouts;

	private GUIBase_Button[] m_ExtrasBackButton;

	private static GUIBase_Button.TouchDelegate[] s_ExtrasDelg = new GUIBase_Button.TouchDelegate[20]
	{
		TouchExtra01, TouchExtra02, TouchExtra03, TouchExtra04, TouchExtra05, TouchExtra06, TouchExtra07, TouchExtra08, TouchExtra09, TouchExtra10,
		TouchExtra11, TouchExtra12, TouchExtra13, TouchExtra14, TouchExtra15, TouchExtra16, TouchExtra17, TouchExtra18, TouchExtra19, TouchExtra20
	};

	private GUIBase_Pivot m_PrevPivot;

	private int m_LevelIdx;

	private int m_Dlc;

	private int m_ShowingExtras = -1;

	public AudioSource m_MenuMusic;

	private float m_MusicVolume = 1f;

	private ArrayList[] MenuItems = new ArrayList[14];

	private int MenuSelection;

	private MenuController MenuCtrl = new MenuController();

	private int m_SkipInputFrames;

	private bool HighlightButton;

	public float MusicVolume
	{
		get
		{
			return m_MusicVolume;
		}
		set
		{
			m_MusicVolume = value;
			if ((bool)m_MenuMusic)
			{
				m_MenuMusic.volume = m_MusicVolume;
			}
		}
	}

	private void Awake()
	{
		Instance = this;
		m_IsInitialized = false;
		m_LevelIdx = -1;
		Time.timeScale = 1f;
	}

	private void Start()
	{
		MFGuiManager.Instance.SetFadeOut(1f);
		MFGuiManager.Instance.FadeIn(0.5f);
		GuiOptionsMenu.Instance.m_OnHideDelegate = OnOptionsHide;
		MusicVolume = GuiOptions.musicVolume;
		if (custom_inputs.IsNVidiaShield())
		{
			s_PivotHelpName = "ShieldHelp_Pivot";
			s_LayoutHelpButtonsName = "ShieldHelp_Layout";
		}
		HighlightButton = true;
	}

	private void LateUpdate()
	{
		if (m_IsInitialized)
		{
			ControlsUpdate();
			if ((bool)m_PrevPivot && !m_PrevPivot.IsVisible())
			{
				ImmediateChangeState();
				m_PrevPivot = null;
			}
			return;
		}
		PrecacheObjects();
		RegisterDelegates();
		RegisterExtras();
		InitMenuItems();
		UnlockLevels(false);
		UnlockExtras(false);
		ChangeState(E_MenuState.E_MS_MAIN);
		MFGuiManager.Instance.ShowPivot(m_PivotAlwaysVisible, true);
		Screen.lockCursor = false;
		GuiOptionsMenu.Instance.LateInitialize();
		m_IsInitialized = true;
	}

	private void PrecacheObjects()
	{
		m_PivotMain = MFGuiManager.Instance.GetPivot(s_PivotMainName);
		m_PivotAbout = MFGuiManager.Instance.GetPivot(s_PivotAboutName);
		m_PivotMore = MFGuiManager.Instance.GetPivot(s_PivotMoreName);
		m_PivotExtrasLayouts = MFGuiManager.Instance.GetPivot(s_PivotExtrasName);
		m_PivotHelp = MFGuiManager.Instance.GetPivot(s_PivotHelpName);
		m_PivotPlay = MFGuiManager.Instance.GetPivot(s_PivotPlayName);
		m_PivotGame = MFGuiManager.Instance.GetPivot(s_PivotGameName);
		m_PivotPlayDif = MFGuiManager.Instance.GetPivot(s_PivotPlayDifName);
		m_PivotLoading = MFGuiManager.Instance.GetPivot(s_PivotLoadingName);
		m_PivotAlwaysVisible = MFGuiManager.Instance.GetPivot(s_PivotAlwaysName);
		m_PivotMoreGames = MFGuiManager.Instance.GetPivot(s_PivotMoreGamesName);
		for (int i = 0; i < 2; i++)
		{
			m_PivotPlayLev[i] = MFGuiManager.Instance.GetPivot(s_PivotPlayLevName[i]);
			m_LayoutPlayLevButtons[i] = GuiBaseUtils.GetLayout(s_LayoutPlayLevButtonsName, m_PivotPlayLev[i]);
		}
		GuiBaseUtils.RegisterButtonDelegate(m_LayoutPlayLevButtons[0], s_ButtonBackName, null, OnTouchButtonPlayLevBack0);
		GuiBaseUtils.RegisterButtonDelegate(m_LayoutPlayLevButtons[1], s_ButtonBackName, null, OnTouchButtonPlayLevBack1);
		m_LayoutMainButtons = GuiBaseUtils.GetLayout(s_LayoutMainButtonsName, m_PivotMain);
		m_LayoutAboutButtons = GuiBaseUtils.GetLayout(s_LayoutAboutButtonsName, m_PivotAbout);
		m_LayoutMoreButtons = GuiBaseUtils.GetLayout(s_LayoutMoreButtonsName, m_PivotMore);
		m_LayoutHelpButtons = GuiBaseUtils.GetLayout(s_LayoutHelpButtonsName, m_PivotHelp);
		m_LayoutPlayButtons = GuiBaseUtils.GetLayout(s_LayoutPlayButtonsName, m_PivotPlay);
		m_LayoutGameButtons = GuiBaseUtils.GetLayout(s_LayoutGameButtonsName, m_PivotGame);
		m_LayoutPlayDifButtons = GuiBaseUtils.GetLayout(s_LayoutPlayDifButtonsName, m_PivotPlayDif);
		m_LayoutPlayConfirm = GuiBaseUtils.GetLayout(s_LayoutPlayConfirmName, m_PivotPlay);
		m_PivotHelpMoga = MFGuiManager.Instance.GetPivot(s_PivotHelpMogaName);
		m_LayoutHelpMogaButtons = GuiBaseUtils.GetLayout(s_LayoutHelpMogaName, m_PivotHelpMoga);
		m_MogaPro_Widget = m_LayoutHelpMogaButtons.GetWidget(s_MogaProName);
		m_MogaPocket_Widget = m_LayoutHelpMogaButtons.GetWidget(s_MogaPocketName);
	}

	private void RegisterDelegates()
	{
		GuiBaseUtils.RegisterButtonDelegate(m_LayoutMainButtons, s_ButtonPlayName, null, OnTouchButtonPlay);
		GuiBaseUtils.RegisterButtonDelegate(m_LayoutMainButtons, s_ButtonAboutName, null, OnTouchButtonAbout);
		GuiBaseUtils.RegisterButtonDelegate(m_LayoutMainButtons, s_ButtonMoreName, null, OnTouchButtonMore);
		GuiBaseUtils.RegisterButtonDelegate(m_LayoutMainButtons, s_ButtonOptName, null, OnTouchButtonOpt);
		GuiBaseUtils.RegisterButtonDelegate(m_LayoutMainButtons, s_ButtonHelpName, null, OnTouchButtonHelp);
		GuiBaseUtils.RegisterButtonDelegate(m_LayoutMainButtons, s_ButtonExitName, null, OnTouchButtonExit);
		GUIBase_Widget widget = m_LayoutMainButtons.GetWidget(s_ButtonMoreGames);
		if (widget != null)
		{
			widget.m_VisibleOnLayoutShow = false;
			GUIBase_Widget[] componentsInChildren = widget.GetComponentsInChildren<GUIBase_Widget>();
			foreach (GUIBase_Widget gUIBase_Widget in componentsInChildren)
			{
				gUIBase_Widget.m_VisibleOnLayoutShow = false;
			}
		}
		GuiBaseUtils.RegisterButtonDelegate(m_LayoutAboutButtons, s_ButtonBackName, null, OnAboutButtonBack);
		GuiBaseUtils.RegisterButtonDelegate(m_LayoutMoreButtons, s_ButtonBackName, null, OnMoreButtonBack);
		GuiBaseUtils.RegisterButtonDelegate(m_LayoutHelpButtons, s_ButtonBackName, null, OnHelpButtonBack);
		GuiBaseUtils.RegisterButtonDelegate(m_LayoutHelpMogaButtons, s_ButtonBackName, null, OnHelpMogaButtonBack);
		m_BackButtonPlay = GuiBaseUtils.RegisterButtonDelegate(m_LayoutPlayButtons, s_ButtonBackName, null, OnPlayButtonBack);
		GuiBaseUtils.RegisterButtonDelegate(m_LayoutGameButtons, s_ButtonBackName, null, OnGameButtonBack);
		m_GameOrig = GuiBaseUtils.RegisterButtonDelegate(m_LayoutGameButtons, "Game01_Button", null, OnTouchButtonGameOrig);
		m_GameDlc = GuiBaseUtils.RegisterButtonDelegate(m_LayoutGameButtons, "Game02_Button", null, OnTouchButtonGameDlc1);
		m_ResumePlayButton = GuiBaseUtils.RegisterButtonDelegate(m_LayoutPlayButtons, s_ButtonResumeName, null, OnTouchButtonResume);
		m_PlayNewButton = GuiBaseUtils.RegisterButtonDelegate(m_LayoutPlayButtons, s_ButtonNewGameName, null, OnTouchButtonNewGame);
		m_PlayLevelButton = GuiBaseUtils.RegisterButtonDelegate(m_LayoutPlayButtons, s_ButtonPlayLevelName, null, OnTouchButtonPlayLevel);
		GuiBaseUtils.RegisterButtonDelegate(m_LayoutPlayConfirm, s_ButtonPlayConfirmYesName, null, OnTouchButtonPlayYes);
		GuiBaseUtils.RegisterButtonDelegate(m_LayoutPlayConfirm, s_ButtonPlayConfirmNoName, null, OnTouchButtonPlayNo);
		GuiBaseUtils.RegisterButtonDelegate(m_LayoutPlayDifButtons, s_ButtonPlayDif1ButtonName, null, OnTouchButtonPlayDif1);
		GuiBaseUtils.RegisterButtonDelegate(m_LayoutPlayDifButtons, s_ButtonPlayDif2ButtonName, null, OnTouchButtonPlayDif2);
		GuiBaseUtils.RegisterButtonDelegate(m_LayoutPlayDifButtons, s_ButtonPlayDif3ButtonName, null, OnTouchButtonPlayDif3);
		GuiBaseUtils.RegisterButtonDelegate(m_LayoutPlayDifButtons, s_ButtonBackName, null, OnTouchButtonPlayDifBack);
		GuiBaseUtils.RegisterFocusDelegate(m_PivotMain, s_LayoutMainButtonsName, OnFocusDelegate);
		RegisterLevelDelegates();
		if (m_LayoutMainButtons.GetWidget(s_ButtonPlayDemoName, false) != null)
		{
			GuiBaseUtils.RegisterButtonDelegate(m_LayoutMainButtons, s_ButtonPlayDemoName, null, OnTouchButtonPlayDemo);
		}
		if (m_LayoutMainButtons.GetWidget(s_ButtonShowcaseName, false) != null)
		{
			GuiBaseUtils.RegisterButtonDelegate(m_LayoutMainButtons, s_ButtonShowcaseName, null, OnTouchButtonShowcase);
		}
	}

	private void RegisterLevelDelegates()
	{
		m_LevelsButtons[0] = new GUIBase_Button[10];
		m_LevelsButtons[0][0] = GuiBaseUtils.RegisterButtonDelegate(m_LayoutPlayLevButtons[0], "Lev01_Button", null, OnTouchButtonPlayLev1);
		m_LevelsButtons[0][1] = GuiBaseUtils.RegisterButtonDelegate(m_LayoutPlayLevButtons[0], "Lev02_Button", null, OnTouchButtonPlayLev2);
		m_LevelsButtons[0][2] = GuiBaseUtils.RegisterButtonDelegate(m_LayoutPlayLevButtons[0], "Lev03_Button", null, OnTouchButtonPlayLev3);
		m_LevelsButtons[0][3] = GuiBaseUtils.RegisterButtonDelegate(m_LayoutPlayLevButtons[0], "Lev04_Button", null, OnTouchButtonPlayLev4);
		m_LevelsButtons[0][4] = GuiBaseUtils.RegisterButtonDelegate(m_LayoutPlayLevButtons[0], "Lev05_Button", null, OnTouchButtonPlayLev5);
		m_LevelsButtons[0][5] = GuiBaseUtils.RegisterButtonDelegate(m_LayoutPlayLevButtons[0], "Lev06_Button", null, OnTouchButtonPlayLev6);
		m_LevelsButtons[0][6] = GuiBaseUtils.RegisterButtonDelegate(m_LayoutPlayLevButtons[0], "Lev07_Button", null, OnTouchButtonPlayLev7);
		m_LevelsButtons[0][7] = GuiBaseUtils.RegisterButtonDelegate(m_LayoutPlayLevButtons[0], "Lev08_Button", null, OnTouchButtonPlayLev8);
		m_LevelsButtons[0][8] = GuiBaseUtils.RegisterButtonDelegate(m_LayoutPlayLevButtons[0], "Lev09_Button", null, OnTouchButtonPlayLev9);
		m_LevelsButtons[0][9] = GuiBaseUtils.RegisterButtonDelegate(m_LayoutPlayLevButtons[0], "Lev10_Button", null, OnTouchButtonPlayLev10);
		m_LevelsButtons[1] = new GUIBase_Button[4];
		m_LevelsButtons[1][0] = GuiBaseUtils.RegisterButtonDelegate(m_LayoutPlayLevButtons[1], "Lev11_dlc_Button", null, OnTouchButtonPlayLev11);
		m_LevelsButtons[1][1] = GuiBaseUtils.RegisterButtonDelegate(m_LayoutPlayLevButtons[1], "Lev12_dlc_Button", null, OnTouchButtonPlayLev12);
		m_LevelsButtons[1][2] = GuiBaseUtils.RegisterButtonDelegate(m_LayoutPlayLevButtons[1], "Lev13_dlc_Button", null, OnTouchButtonPlayLev13);
		m_LevelsButtons[1][3] = GuiBaseUtils.RegisterButtonDelegate(m_LayoutPlayLevButtons[1], "Lev14_dlc_Button", null, OnTouchButtonPlayLev14);
	}

	private void RegisterExtras()
	{
		m_ExtrasButtons = new GUIBase_Button[20];
		m_ExtrasBackButton = new GUIBase_Button[20];
		for (int i = 0; i < 20; i++)
		{
			string text = "bonus";
			if (i + 1 < 10)
			{
				text += "0";
			}
			text += i + 1;
			text += "_Button";
			m_ExtrasButtons[i] = GuiBaseUtils.RegisterButtonDelegate(m_LayoutMoreButtons, text, s_ExtrasDelg[i], null);
		}
		m_ExtrasLayouts = new GUIBase_Layout[20];
		for (int j = 0; j < 20; j++)
		{
			string text2 = string.Empty;
			if (j + 1 < 10)
			{
				text2 += "0";
			}
			text2 += j + 1;
			text2 += "Bonus_Layout";
			m_ExtrasLayouts[j] = GuiBaseUtils.GetLayout(text2, m_PivotExtrasLayouts);
			m_ExtrasBackButton[j] = GuiBaseUtils.RegisterButtonDelegate(m_ExtrasLayouts[j], "Back_Button", null, ExtrasBack);
		}
	}

	private void RegisterDebugMenus()
	{
		GUIBase_Layout layout = GuiBaseUtils.GetLayout("AlwaysVisible_Layout", m_PivotAlwaysVisible);
		m_UnlockAllLevels = GuiBaseUtils.RegisterSwitchDelegate(layout, "Unlock_All_Levels", OnUnlockLevels);
		m_UnlockAllLevels.Widget.m_VisibleOnLayoutShow = true;
		GUIBase_Widget widget = layout.GetWidget("Label_Unlock_All_Levels");
		widget.m_VisibleOnLayoutShow = true;
	}

	private void InitMenuItems()
	{
		MenuItems[0] = new ArrayList();
		MenuItems[1] = new ArrayList();
		MenuItems[1].Add(GuiBaseUtils.GetButton(m_LayoutMainButtons, s_ButtonPlayName));
		MenuItems[1].Add(GuiBaseUtils.GetButton(m_LayoutMainButtons, s_ButtonHelpName));
		MenuItems[1].Add(GuiBaseUtils.GetButton(m_LayoutMainButtons, s_ButtonOptName));
		MenuItems[1].Add(GuiBaseUtils.GetButton(m_LayoutMainButtons, s_ButtonMoreName));
		MenuItems[1].Add(GuiBaseUtils.GetButton(m_LayoutMainButtons, s_ButtonAboutName));
		MenuItems[1].Add(GuiBaseUtils.GetButton(m_LayoutMainButtons, s_ButtonExitName));
		MenuItems[2] = new ArrayList();
		MenuItems[2].Add(GuiBaseUtils.GetButton(m_LayoutAboutButtons, s_ButtonBackName));
		MenuItems[3] = new ArrayList();
		GUIBase_Button[] extrasButtons = m_ExtrasButtons;
		foreach (GUIBase_Button value in extrasButtons)
		{
			MenuItems[3].Add(value);
		}
		MenuItems[3].Add(GuiBaseUtils.GetButton(m_LayoutMoreButtons, s_ButtonBackName));
		MenuItems[4] = new ArrayList();
		MenuItems[5] = new ArrayList();
		MenuItems[6] = new ArrayList();
		MenuItems[6].Add(GuiBaseUtils.GetButton(m_LayoutHelpButtons, s_ButtonBackName));
		MenuItems[7] = new ArrayList();
		MenuItems[7].Add(GuiBaseUtils.GetButton(m_LayoutHelpMogaButtons, s_ButtonBackName));
		MenuItems[8] = new ArrayList();
		MenuItems[8].Add(m_ResumePlayButton);
		MenuItems[8].Add(m_PlayNewButton);
		MenuItems[8].Add(m_PlayLevelButton);
		MenuItems[8].Add(GuiBaseUtils.GetButton(m_LayoutPlayButtons, s_ButtonBackName));
		MenuItems[8].Add(GuiBaseUtils.GetButton(m_LayoutPlayConfirm, s_ButtonPlayConfirmYesName));
		MenuItems[8].Add(GuiBaseUtils.GetButton(m_LayoutPlayConfirm, s_ButtonPlayConfirmNoName));
		MenuItems[9] = new ArrayList();
		MenuItems[9].Add(m_GameOrig);
		MenuItems[9].Add(m_GameDlc);
		MenuItems[9].Add(GuiBaseUtils.GetButton(m_LayoutGameButtons, s_ButtonBackName));
		MenuItems[10] = new ArrayList();
		MenuItems[10].Add(GuiBaseUtils.GetButton(m_LayoutPlayDifButtons, s_ButtonPlayDif1ButtonName));
		MenuItems[10].Add(GuiBaseUtils.GetButton(m_LayoutPlayDifButtons, s_ButtonPlayDif2ButtonName));
		MenuItems[10].Add(GuiBaseUtils.GetButton(m_LayoutPlayDifButtons, s_ButtonPlayDif3ButtonName));
		MenuItems[10].Add(GuiBaseUtils.GetButton(m_LayoutPlayDifButtons, s_ButtonBackName));
		MenuItems[11] = new ArrayList();
		GUIBase_Button[] array = m_LevelsButtons[0];
		foreach (GUIBase_Button value2 in array)
		{
			MenuItems[11].Add(value2);
		}
		MenuItems[11].Add(GuiBaseUtils.GetButton(m_LayoutPlayLevButtons[0], s_ButtonBackName));
		MenuItems[12] = new ArrayList();
		GUIBase_Button[] array2 = m_LevelsButtons[1];
		foreach (GUIBase_Button value3 in array2)
		{
			MenuItems[12].Add(value3);
		}
		MenuItems[12].Add(GuiBaseUtils.GetButton(m_LayoutPlayLevButtons[1], s_ButtonBackName));
		MenuItems[13] = new ArrayList();
	}

	private void OnUnlockLevels(bool unlock)
	{
		Debug.Log("OnUnlockLevels " + m_UnlockAllLevels.GetValue());
		bool flag = (bool)m_UnlockAllLevels && m_UnlockAllLevels.GetValue();
		UnlockLevels(flag);
		UnlockExtras(flag);
	}

	private void ChangeState(E_MenuState newState)
	{
		switch (m_CurrentState)
		{
		case E_MenuState.E_MS_MAIN:
			MFGuiManager.Instance.ShowPivot(m_PivotMain, false);
			m_PrevPivot = m_PivotMain;
			break;
		case E_MenuState.E_MS_ABOUT:
			MFGuiManager.Instance.ShowPivot(m_PivotAbout, false);
			m_PrevPivot = m_PivotAbout;
			break;
		case E_MenuState.E_MS_MORE:
			MFGuiManager.Instance.ShowPivot(m_PivotMore, false);
			m_PrevPivot = m_PivotMore;
			break;
		case E_MenuState.E_MS_MORE_GAMES:
			MFGuiManager.Instance.ShowPivot(m_PivotMoreGames, false);
			m_PrevPivot = m_PivotMoreGames;
			break;
		case E_MenuState.E_MS_OPT:
			Debug.LogError("Z GuiOptions se nevracime nastavenim statu ale voolame delegate ktery se prepne rovnou do main menu.");
			break;
		case E_MenuState.E_MS_HELP:
			MFGuiManager.Instance.ShowPivot(m_PivotHelp, false);
			m_PrevPivot = m_PivotHelp;
			break;
		case E_MenuState.E_MS_HELP_MOGA:
			MFGuiManager.Instance.ShowPivot(m_PivotHelpMoga, false);
			m_PrevPivot = m_PivotHelpMoga;
			break;
		case E_MenuState.E_MS_PLAY:
			MFGuiManager.Instance.ShowPivot(m_PivotPlay, false);
			m_PrevPivot = m_PivotPlay;
			break;
		case E_MenuState.E_MS_GAME:
			MFGuiManager.Instance.ShowPivot(m_PivotGame, false);
			m_PrevPivot = m_PivotGame;
			break;
		case E_MenuState.E_MS_PLAY_DIF:
			MFGuiManager.Instance.ShowPivot(m_PivotPlayDif, false);
			m_PrevPivot = m_PivotPlayDif;
			break;
		case E_MenuState.E_MS_PLAY_LEV:
			MFGuiManager.Instance.ShowPivot(m_PivotPlayLev[0], false);
			m_PrevPivot = m_PivotPlayLev[0];
			break;
		case E_MenuState.E_MS_PLAY_LEV_DLC:
			MFGuiManager.Instance.ShowPivot(m_PivotPlayLev[1], false);
			m_PrevPivot = m_PivotPlayLev[1];
			break;
		case E_MenuState.E_MS_LOADING:
			MFGuiManager.Instance.ShowPivot(m_PivotLoading, false);
			m_PrevPivot = m_PivotLoading;
			break;
		default:
			m_PrevPivot = null;
			break;
		}
		m_CurrentState = newState;
		if (!(m_PrevPivot != null))
		{
			ImmediateChangeState();
		}
	}

	private void ImmediateChangeState()
	{
		switch (m_CurrentState)
		{
		case E_MenuState.E_MS_MAIN:
			MFGuiManager.Instance.ShowPivot(m_PivotMain, true);
			break;
		case E_MenuState.E_MS_ABOUT:
			MFGuiManager.Instance.ShowPivot(m_PivotAbout, true);
			break;
		case E_MenuState.E_MS_MORE:
			MFGuiManager.Instance.ShowPivot(m_PivotMore, true);
			break;
		case E_MenuState.E_MS_MORE_GAMES:
			MFGuiManager.Instance.ShowPivot(m_PivotMoreGames, true);
			break;
		case E_MenuState.E_MS_OPT:
			GuiOptionsMenu.Instance.Show();
			break;
		case E_MenuState.E_MS_HELP:
			MFGuiManager.Instance.ShowPivot(m_PivotHelp, true);
			break;
		case E_MenuState.E_MS_HELP_MOGA:
			MFGuiManager.Instance.ShowPivot(m_PivotHelpMoga, true);
			if (MogaGamepad.IsMogaPro())
			{
				m_MogaPro_Widget.Show(true, true);
			}
			else
			{
				m_MogaPocket_Widget.Show(true, true);
			}
			break;
		case E_MenuState.E_MS_PLAY:
			MFGuiManager.Instance.ShowPivot(m_PivotPlay, true);
			MFGuiManager.Instance.ShowWidget(m_ResumePlayButton.Widget, Game.Instance.IsResumePossible(E_GameType.SinglePlayer, m_Dlc), true);
			MFGuiManager.Instance.ShowLayout(m_LayoutPlayConfirm, false);
			break;
		case E_MenuState.E_MS_GAME:
			MFGuiManager.Instance.ShowPivot(m_PivotGame, true);
			break;
		case E_MenuState.E_MS_PLAY_DIF:
			MFGuiManager.Instance.ShowPivot(m_PivotPlayDif, true);
			break;
		case E_MenuState.E_MS_PLAY_LEV:
			MFGuiManager.Instance.ShowPivot(m_PivotPlayLev[0], true);
			break;
		case E_MenuState.E_MS_PLAY_LEV_DLC:
			MFGuiManager.Instance.ShowPivot(m_PivotPlayLev[1], true);
			break;
		case E_MenuState.E_MS_LOADING:
			MFGuiManager.Instance.ShowPivot(m_PivotAlwaysVisible, false);
			MFGuiManager.Instance.ShowPivot(m_PivotLoading, true);
			break;
		}
		ResetSelectionButton();
	}

	private void OnOptionsHide()
	{
		m_CurrentState = E_MenuState.E_MS_MAIN;
		ImmediateChangeState();
	}

	private void OnTouchButtonPlay(bool inside)
	{
		if (inside)
		{
			ChangeState(E_MenuState.E_MS_GAME);
		}
	}

	private void OnTouchButtonPlayDemo(bool inside)
	{
		if (!inside)
		{
		}
	}

	private void OnTouchButtonShowcase(bool inside)
	{
		if (inside)
		{
			ChangeState(E_MenuState.E_MS_LOADING);
			Invoke("LoadShowcase", 0.5f);
		}
	}

	private void LoadShowcase()
	{
		Application.LoadLevel("level_showcase");
	}

	private void OnTouchButtonAbout(bool inside)
	{
		if (!inside)
		{
			return;
		}
		GameObject gameObject = GameObject.Find("Credits_Sprite");
		if ((bool)gameObject)
		{
			GUIBase_Widget gUIBase_Widget = gameObject.GetComponent("GUIBase_Widget") as GUIBase_Widget;
			if ((bool)gUIBase_Widget)
			{
				gUIBase_Widget.m_Material.SetFloat("_TimeOffs", 0f - Time.time);
			}
		}
		else
		{
			Debug.LogError("Cannot find Credits_Sprite object");
		}
		ChangeState(E_MenuState.E_MS_ABOUT);
	}

	private void OnTouchButtonMore(bool inside)
	{
		if (inside)
		{
			ChangeState(E_MenuState.E_MS_MORE);
		}
	}

	private void OnTouchButtonOpt(bool inside)
	{
		if (inside)
		{
			ChangeState(E_MenuState.E_MS_OPT);
		}
	}

	private void OnTouchButtonHelp(bool inside)
	{
		if (inside)
		{
			if (MogaGamepad.IsConnected())
			{
				ChangeState(E_MenuState.E_MS_HELP_MOGA);
			}
			else
			{
				ChangeState(E_MenuState.E_MS_HELP);
			}
		}
	}

	private void OnTouchButtonExit(bool inside)
	{
		if (inside)
		{
			Application.Quit();
		}
	}

	private void OnAboutButtonBack(bool inside)
	{
		if (inside)
		{
			ChangeState(E_MenuState.E_MS_MAIN);
		}
	}

	private void OnHelpButtonBack(bool inside)
	{
		if (inside)
		{
			ChangeState(E_MenuState.E_MS_MAIN);
		}
	}

	private void OnHelpMogaButtonBack(bool inside)
	{
		if (inside)
		{
			ChangeState(E_MenuState.E_MS_MAIN);
		}
	}

	private void OnMoreButtonBack(bool inside)
	{
		if (inside)
		{
			ChangeState(E_MenuState.E_MS_MAIN);
		}
	}

	private void OnPlayButtonBack(bool inside)
	{
		if (inside)
		{
			ChangeState(E_MenuState.E_MS_GAME);
		}
	}

	private void OnGameButtonBack(bool inside)
	{
		if (inside)
		{
			ChangeState(E_MenuState.E_MS_MAIN);
		}
	}

	private void OnTouchButtonNewGame(bool inside)
	{
		if (inside)
		{
			if (Game.Instance.IsResumePossible(E_GameType.SinglePlayer, m_Dlc))
			{
				m_LevelIdx = -1;
				DisablePlayButtons(true);
				MFGuiManager.Instance.ShowLayout(m_LayoutPlayConfirm, true);
				ResetSelectionButton();
			}
			else
			{
				m_LevelIdx = -1;
				ChangeState(E_MenuState.E_MS_PLAY_DIF);
			}
		}
	}

	private void DisablePlayButtons(bool disable)
	{
		m_ResumePlayButton.SetDisabled(disable);
		m_PlayNewButton.SetDisabled(disable);
		m_PlayLevelButton.SetDisabled(disable);
		m_BackButtonPlay.Widget.Show(!disable, true);
	}

	private void OnTouchButtonPlayLevel(bool inside)
	{
		if (inside)
		{
			ChangeState((m_Dlc != 0) ? E_MenuState.E_MS_PLAY_LEV_DLC : E_MenuState.E_MS_PLAY_LEV);
		}
	}

	private void OnTouchButtonPlayYes(bool inside)
	{
		if (inside)
		{
			DisablePlayButtons(false);
			ChangeState(E_MenuState.E_MS_PLAY_DIF);
		}
	}

	private void OnTouchButtonPlayNo(bool inside)
	{
		if (inside)
		{
			DisablePlayButtons(false);
			ChangeState(E_MenuState.E_MS_PLAY);
			ResetSelectionButton();
		}
	}

	private void OnTouchButtonPlayLevBack0(bool inside)
	{
		if (inside)
		{
			ChangeState(E_MenuState.E_MS_PLAY);
		}
	}

	private void OnTouchButtonPlayLevBack1(bool inside)
	{
		if (inside)
		{
			ChangeState(E_MenuState.E_MS_PLAY);
		}
	}

	private void OnTouchButtonPlayLev1(bool inside)
	{
		if (inside)
		{
			m_LevelIdx = 0;
			ChangeState(E_MenuState.E_MS_PLAY_DIF);
		}
	}

	private void OnTouchButtonPlayLev2(bool inside)
	{
		if (inside)
		{
			m_LevelIdx = 1;
			ChangeState(E_MenuState.E_MS_PLAY_DIF);
		}
	}

	private void OnTouchButtonPlayLev3(bool inside)
	{
		if (inside)
		{
			m_LevelIdx = 2;
			ChangeState(E_MenuState.E_MS_PLAY_DIF);
		}
	}

	private void OnTouchButtonPlayLev4(bool inside)
	{
		if (inside)
		{
			m_LevelIdx = 3;
			ChangeState(E_MenuState.E_MS_PLAY_DIF);
		}
	}

	private void OnTouchButtonPlayLev5(bool inside)
	{
		if (inside)
		{
			m_LevelIdx = 4;
			ChangeState(E_MenuState.E_MS_PLAY_DIF);
		}
	}

	private void OnTouchButtonPlayLev6(bool inside)
	{
		if (inside)
		{
			m_LevelIdx = 5;
			ChangeState(E_MenuState.E_MS_PLAY_DIF);
		}
	}

	private void OnTouchButtonPlayLev7(bool inside)
	{
		if (inside)
		{
			m_LevelIdx = 6;
			ChangeState(E_MenuState.E_MS_PLAY_DIF);
		}
	}

	private void OnTouchButtonPlayLev8(bool inside)
	{
		if (inside)
		{
			m_LevelIdx = 7;
			ChangeState(E_MenuState.E_MS_PLAY_DIF);
		}
	}

	private void OnTouchButtonPlayLev9(bool inside)
	{
		if (inside)
		{
			m_LevelIdx = 8;
			ChangeState(E_MenuState.E_MS_PLAY_DIF);
		}
	}

	private void OnTouchButtonPlayLev10(bool inside)
	{
		if (inside)
		{
			m_LevelIdx = 9;
			ChangeState(E_MenuState.E_MS_PLAY_DIF);
		}
	}

	private void OnTouchButtonPlayLev11(bool inside)
	{
		if (inside)
		{
			m_LevelIdx = 10;
			ChangeState(E_MenuState.E_MS_PLAY_DIF);
		}
	}

	private void OnTouchButtonPlayLev12(bool inside)
	{
		if (inside)
		{
			m_LevelIdx = 11;
			ChangeState(E_MenuState.E_MS_PLAY_DIF);
		}
	}

	private void OnTouchButtonPlayLev13(bool inside)
	{
		if (inside)
		{
			m_LevelIdx = 12;
			ChangeState(E_MenuState.E_MS_PLAY_DIF);
		}
	}

	private void OnTouchButtonPlayLev14(bool inside)
	{
		if (inside)
		{
			m_LevelIdx = 13;
			ChangeState(E_MenuState.E_MS_PLAY_DIF);
		}
	}

	private void OnTouchButtonPlayDifBack(bool inside)
	{
		if (inside)
		{
			ChangeState(E_MenuState.E_MS_PLAY);
		}
	}

	private void OnTouchMoreGamesBack(bool inside)
	{
		if (inside)
		{
			ChangeState(E_MenuState.E_MS_MAIN);
		}
	}

	private void OnTouchButtonPlayDif1(bool inside)
	{
		if (inside)
		{
			StartCoroutine(StartGame(E_GameDifficulty.Easy));
		}
	}

	private void OnTouchButtonPlayDif2(bool inside)
	{
		if (inside)
		{
			StartCoroutine(StartGame(E_GameDifficulty.Normal));
		}
	}

	private void OnTouchButtonPlayDif3(bool inside)
	{
		if (inside)
		{
			StartCoroutine(StartGame(E_GameDifficulty.Hard));
		}
	}

	private void OnTouchButtonResume(bool inside)
	{
		if (inside && Game.Instance.IsResumePossible(E_GameType.SinglePlayer, m_Dlc))
		{
			StartCoroutine(ResumePlay());
		}
	}

	private void OnTouchButtonGameOrig(bool inside)
	{
		if (inside)
		{
			m_Dlc = 0;
			ChangeState(E_MenuState.E_MS_PLAY);
		}
	}

	private void OnTouchButtonGameDlc1(bool inside)
	{
		if (inside)
		{
			m_Dlc = 1;
			ChangeState(E_MenuState.E_MS_PLAY);
		}
	}

	private void OnTouchButtonAchievements(bool inside)
	{
		if (inside && !IsInvoking("ShowGCAchievementList"))
		{
			Invoke("ShowGCAchievementList", 0.2f);
		}
	}

	private void OnTouchButtonMoreGames(bool inside)
	{
	}

	private void OnTouchButtonBuySamuraiHD()
	{
		Application.OpenURL("http://click.linksynergy.com/fs-bin/stat?id=bUwtTpNkQk8&offerid=146261&type=3&subid=0&tmpid=1826&RD_PARM1=http%253A%252F%252Fitunes.apple.com%252Fus%252Fapp%252Fsamurai-way-of-the-warrior-hd%252Fid377386379%253Fmt%253D8%2526uo%253D4%2526partnerId%253D30");
	}

	private void OnTouchButtonBuySamuraiII()
	{
		Application.OpenURL("http://click.linksynergy.com/fs-bin/stat?id=bUwtTpNkQk8&offerid=146261&type=3&subid=0&tmpid=1826&RD_PARM1=http%253A%252F%252Fitunes.apple.com%252Fus%252Fapp%252Fsamurai-ii-vengeance%252Fid392486160%253Fmt%253D8%2526uo%253D4%2526partnerId%253D30");
	}

	private IEnumerator ResumePlay()
	{
		ChangeState(E_MenuState.E_MS_LOADING);
		StartCoroutine(FadeOutMenuMusic_Coroutine(1f));
		yield return new WaitForSeconds(1f);
		Game.Instance.ResumeSinglePlayer(m_Dlc);
	}

	private IEnumerator StartGame(E_GameDifficulty dif)
	{
		ChangeState(E_MenuState.E_MS_LOADING);
		StartCoroutine(FadeOutMenuMusic_Coroutine(1f));
		yield return new WaitForSeconds(1f);
		if (m_LevelIdx == -1 && m_Dlc == 0)
		{
			Game.PauseScreenTimeout(120f);
			yield return StartCoroutine(FullScreenVideo.Play("introHD.mp4"));
			ChangeState(E_MenuState.E_MS_LOADING);
			yield return new WaitForEndOfFrame();
			Game.Instance.StartNewGame(dif, m_Dlc);
		}
		else if (m_LevelIdx == -1 && m_Dlc == 1)
		{
			ChangeState(E_MenuState.E_MS_LOADING);
			yield return new WaitForEndOfFrame();
			Game.Instance.StartNewGame(dif, m_Dlc);
		}
		else
		{
			Game.Instance.StartChapterMode(m_LevelIdx, dif);
		}
	}

	private IEnumerator FadeOutMenuMusic_Coroutine(float fadeTime)
	{
		if (!m_MenuMusic)
		{
			Debug.LogWarning("Music is not assigned");
			yield break;
		}
		float beginFadeTime = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup - beginFadeTime <= fadeTime)
		{
			float volume = 1f - Mathf.Max((Time.realtimeSinceStartup - beginFadeTime) / fadeTime, 1f);
			m_MenuMusic.volume = volume * MusicVolume;
			yield return new WaitForEndOfFrame();
		}
	}

	private int OnFocusDelegate(int currentFocusID, GUIBase_Layout.E_FocusChangeDir dir)
	{
		return -1;
	}

	private void UnlockLevels(bool unlockAll)
	{
		for (int i = 0; i < 2; i++)
		{
			int unlockedMission = Game.Instance.GetUnlockedMission(i);
			for (int j = 0; j < m_LevelsButtons[i].Length; j++)
			{
				bool flag = j + 1 <= unlockedMission || unlockAll;
				m_LevelsButtons[i][j].SetDisabled(!flag);
			}
		}
	}

	private void UnlockExtras(bool unlock)
	{
		for (int i = 0; i < 20; i++)
		{
			bool flag = InteractionObjectCollectible.CollectibleFound(i + 1) || unlock;
			m_ExtrasButtons[i].SetDisabled(!flag);
		}
	}

	private void ShowExtras(int idx)
	{
		MFGuiManager.Instance.ShowPivot(m_PivotMore, false);
		MFGuiManager.Instance.ShowLayout(m_ExtrasLayouts[idx], true);
		m_ShowingExtras = idx;
	}

	private void ReturnFromExtras()
	{
		if (m_ShowingExtras > -1)
		{
			MFGuiManager.Instance.ShowLayout(m_ExtrasLayouts[m_ShowingExtras], false);
			m_ShowingExtras = -1;
		}
		MFGuiManager.Instance.ShowPivot(m_PivotMore, true);
		Invoke("UpdateSelectionDelayed", 0.1f);
	}

	private static void ExtrasBack(bool inside)
	{
		if (inside)
		{
			Instance.ReturnFromExtras();
		}
	}

	private static void TouchExtra01()
	{
		Instance.ShowExtras(0);
	}

	private static void TouchExtra02()
	{
		Instance.ShowExtras(1);
	}

	private static void TouchExtra03()
	{
		Instance.ShowExtras(2);
	}

	private static void TouchExtra04()
	{
		Instance.ShowExtras(3);
	}

	private static void TouchExtra05()
	{
		Instance.ShowExtras(4);
	}

	private static void TouchExtra06()
	{
		Instance.ShowExtras(5);
	}

	private static void TouchExtra07()
	{
		Instance.ShowExtras(6);
	}

	private static void TouchExtra08()
	{
		Instance.ShowExtras(7);
	}

	private static void TouchExtra09()
	{
		Instance.ShowExtras(8);
	}

	private static void TouchExtra10()
	{
		Instance.ShowExtras(9);
	}

	private static void TouchExtra11()
	{
		Instance.ShowExtras(10);
	}

	private static void TouchExtra12()
	{
		Instance.ShowExtras(11);
	}

	private static void TouchExtra13()
	{
		Instance.ShowExtras(12);
	}

	private static void TouchExtra14()
	{
		Instance.ShowExtras(13);
	}

	private static void TouchExtra15()
	{
		Instance.ShowExtras(14);
	}

	private static void TouchExtra16()
	{
		Instance.ShowExtras(15);
	}

	private static void TouchExtra17()
	{
		Instance.ShowExtras(16);
	}

	private static void TouchExtra18()
	{
		Instance.ShowExtras(17);
	}

	private static void TouchExtra19()
	{
		Instance.ShowExtras(18);
	}

	private static void TouchExtra20()
	{
		Instance.ShowExtras(19);
	}

	private void ControlsUpdate()
	{
		if (!m_IsInitialized || m_CurrentState == E_MenuState.E_MS_NONE || m_CurrentState == E_MenuState.E_MS_LOADING)
		{
			return;
		}
		if (m_SkipInputFrames > 0)
		{
			m_SkipInputFrames--;
			return;
		}
		if (HighlightButton && MenuSelection == -1)
		{
			HighlightDefaultButton();
			return;
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
		else if ((m_CurrentState != E_MenuState.E_MS_MORE || m_ShowingExtras == -1) && MenuSelection > 0)
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
		}
		else
		{
			if ((m_CurrentState == E_MenuState.E_MS_MORE && m_ShowingExtras != -1) || (m_CurrentState == E_MenuState.E_MS_MAIN && MenuSelection >= MenuItems[1].Count - 1))
			{
				return;
			}
			int num = MenuItems[(int)m_CurrentState].Count - 1;
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
	}

	private void ChangeFocusUp()
	{
		if (MenuSelection == -1)
		{
			HighlightDefaultButton();
		}
		else if (m_CurrentState == E_MenuState.E_MS_PLAY_LEV)
		{
			if (MenuSelection >= 5)
			{
				int num = ((MenuSelection != 10) ? (MenuSelection - 5) : (MenuSelection - 1));
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
		else if (m_CurrentState == E_MenuState.E_MS_MORE && m_ShowingExtras == -1 && MenuSelection >= 4)
		{
			int num2 = ((MenuSelection != 16) ? (MenuSelection - 4) : (MenuSelection - 1));
			while (!IsMenuButtonEnabled(num2) && num2 > 0)
			{
				num2--;
			}
			if (IsMenuButtonEnabled(num2))
			{
				UpdateSelection(num2);
			}
		}
	}

	private void ChangeFocusDown()
	{
		if (MenuSelection == -1)
		{
			HighlightDefaultButton();
		}
		else if (m_CurrentState == E_MenuState.E_MS_PLAY_LEV)
		{
			if (MenuSelection <= 4 || MenuSelection >= 9)
			{
				int num = ((MenuSelection != 9) ? (MenuSelection + 5) : (MenuSelection + 1));
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
		else if (m_CurrentState == E_MenuState.E_MS_MORE && m_ShowingExtras == -1 && (MenuSelection <= 11 || MenuSelection >= 15))
		{
			int num2 = ((MenuSelection != 15) ? (MenuSelection + 4) : (MenuSelection + 1));
			while (!IsMenuButtonEnabled(num2) && num2 > 0)
			{
				num2--;
			}
			if (IsMenuButtonEnabled(num2))
			{
				UpdateSelection(num2);
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
		GUIBase_Button currentMenuBackButton = GetCurrentMenuBackButton();
		if ((bool)currentMenuBackButton && !currentMenuBackButton.IsDisabled() && currentMenuBackButton.Widget.IsVisible())
		{
			currentMenuBackButton.Callback(GUIBase_Callback.E_CallbackType.E_CT_ON_TOUCH_BEGIN);
			currentMenuBackButton.Callback(GUIBase_Callback.E_CallbackType.E_CT_ON_TOUCH_END);
		}
	}

	private GUIBase_Button GetCurrentMenuItem(int index)
	{
		if (m_CurrentState == E_MenuState.E_MS_MORE && m_ShowingExtras != -1)
		{
			return m_ExtrasBackButton[m_ShowingExtras];
		}
		if (index < 0 || index >= MenuItems[(int)m_CurrentState].Count)
		{
			return null;
		}
		return (GUIBase_Button)MenuItems[(int)m_CurrentState][index];
	}

	private int MenuDefaultButton(E_MenuState menu)
	{
		switch (menu)
		{
		case E_MenuState.E_MS_PLAY:
			if (m_LayoutPlayConfirm.IsVisible())
			{
				return 5;
			}
			if (Game.Instance.IsResumePossible(E_GameType.SinglePlayer, m_Dlc))
			{
				return 0;
			}
			return 1;
		default:
			if (menu != E_MenuState.E_MS_PLAY_LEV)
			{
				if (m_CurrentState == E_MenuState.E_MS_MORE)
				{
					if (m_ShowingExtras != -1)
					{
						return -1;
					}
					for (int i = 0; i < 20; i++)
					{
						if (InteractionObjectCollectible.CollectibleFound(i + 1))
						{
							return i;
						}
					}
					return 20;
				}
				return 0;
			}
			goto case E_MenuState.E_MS_PLAY_LEV;
		case E_MenuState.E_MS_PLAY_LEV:
		{
			int unlockedMission = Game.Instance.GetUnlockedMission(m_Dlc);
			if (unlockedMission > 0)
			{
				return 0;
			}
			return m_LevelsButtons[m_Dlc].Length;
		}
		}
	}

	private GUIBase_Button GetCurrentMenuBackButton()
	{
		switch (m_CurrentState)
		{
		case E_MenuState.E_MS_PLAY:
			return m_BackButtonPlay;
		case E_MenuState.E_MS_ABOUT:
		case E_MenuState.E_MS_MORE_GAMES:
		case E_MenuState.E_MS_HELP:
		case E_MenuState.E_MS_HELP_MOGA:
		case E_MenuState.E_MS_GAME:
		case E_MenuState.E_MS_PLAY_DIF:
		case E_MenuState.E_MS_PLAY_LEV:
		case E_MenuState.E_MS_PLAY_LEV_DLC:
		{
			int index2 = MenuItems[(int)m_CurrentState].Count - 1;
			return (GUIBase_Button)MenuItems[(int)m_CurrentState][index2];
		}
		case E_MenuState.E_MS_MORE:
			if (m_ShowingExtras == -1)
			{
				int index = MenuItems[(int)m_CurrentState].Count - 1;
				return (GUIBase_Button)MenuItems[(int)m_CurrentState][index];
			}
			return m_ExtrasBackButton[m_ShowingExtras];
		default:
			return null;
		}
	}

	private void ResetSelectionButton()
	{
		MenuSelection = -1;
		m_SkipInputFrames = 1;
	}

	private void HighlightDefaultButton()
	{
		MenuSelection = MenuDefaultButton(m_CurrentState);
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
		if (m_CurrentState == E_MenuState.E_MS_PLAY)
		{
			if (m_LayoutPlayConfirm.IsVisible())
			{
				return newSel > 3;
			}
			if (newSel == 0)
			{
				return Game.Instance.IsResumePossible(E_GameType.SinglePlayer, m_Dlc);
			}
			return newSel <= 3;
		}
		if (m_CurrentState == E_MenuState.E_MS_PLAY_LEV || m_CurrentState == E_MenuState.E_MS_PLAY_LEV_DLC)
		{
			if (newSel == m_LevelsButtons[m_Dlc].Length)
			{
				return true;
			}
			if (newSel < 0 || newSel > m_LevelsButtons[m_Dlc].Length)
			{
				return false;
			}
			return !m_LevelsButtons[m_Dlc][newSel].IsDisabled();
		}
		if (m_CurrentState == E_MenuState.E_MS_MORE)
		{
			if (newSel == m_ExtrasButtons.Length)
			{
				return true;
			}
			if (newSel < 0 || newSel > m_ExtrasButtons.Length)
			{
				return false;
			}
			return !m_ExtrasButtons[newSel].IsDisabled();
		}
		return true;
	}
}
