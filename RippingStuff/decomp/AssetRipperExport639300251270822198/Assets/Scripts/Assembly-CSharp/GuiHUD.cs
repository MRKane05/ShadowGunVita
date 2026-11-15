using System;
using System.Collections;
using UnityEngine;

[AddComponentMenu("GUI/Menu/GuiHUD")]
public class GuiHUD : MonoBehaviour
{
	public enum E_ActionButton
	{
		None = 0,
		Fire = 1,
		Use = 2
	}

	private enum E_HudWidget
	{
		Pause = 0,
		Crosshair = 1,
		Weapon = 2,
		Move = 3,
		Fire = 4,
		Use = 5,
		Reload = 6,
		Roll = 7,
		COUNT = 8
	}

	private class Ammo
	{
		public GUIBase_Number m_Clip;

		public GUIBase_Number m_Supply;

		public GUIBase_Number m_SelectionAll;

		public GUIBase_Number m_ClipRed;
	}

	private enum E_CrosshairType
	{
		None = -1,
		Normal = 0,
		Target = 1
	}

	public enum E_HudMessageType
	{
		E_NONE = -1,
		E_DEATH = 0,
		E_FAILED_SAVE_SARA = 1,
		E_001_GameLoaded = 2,
		E_002_Checkpoint = 3,
		E_003_PickedAmmoSMG = 4,
		E_004_PickedAmmoShotg = 5,
		E_005_PickedAmmoGrnd = 6,
		E_006_PickedAmmoRPG = 7,
		E_007_NewWeaponShotg = 8,
		E_008_NewWeaponGrnd = 9,
		E_009_NewWeaponRPG = 10,
		E_010_SupplyCrate = 11,
		E_011_Collectible = 12,
		E_012_PickedAmmoPlasma = 13,
		E_013_NewWeaponPlasma = 14,
		E_101_DoorsUnlocked = 15,
		E_102_ElevatorActivated = 16,
		E_103_LadderActivated = 17,
		E_104_ExcavatorProgram = 18,
		E_105_Forcefield = 19,
		E_106_TerminalMalfunction = 20,
		E_107_PowernodeRestarted = 21,
		E_108_PlatformActivated = 22,
		E_109_IgnitorDeactivated = 23,
		E_110_IgnitorActivated = 24,
		E_112_CargoPlatform = 25,
		E_113_CargoGate = 26,
		E_114_DeactivateMissile = 27,
		E_115_MissileDeactivated = 28,
		E_116_BulkheadOpened = 29,
		E_117_PowerDeactivated = 30,
		E_118_CraneActivated = 31,
		E_201_TutorialHint01 = 32,
		E_202_TutorialHint02 = 33,
		E_203_TutorialHint03 = 34,
		E_204_TutorialHint04 = 35,
		E_205_TutorialHint05 = 36,
		E_119_KeycardPickedUp = 37,
		E_120_FindKeycard = 38,
		E_300_ActivateBeacon = 39,
		E_301_FindWayFacility = 40,
		E_302_Investigate = 41,
		E_303_FanBlock = 42,
		E_304_HotCircuit = 43,
		E_305_Pursue = 44,
		E_306_RestorePower = 45,
		E_307_ElevatorPower = 46,
		E_308_FindWayDestroy = 47,
		E_309_UnlockEject = 48,
		E_310_FirstUnitUnlock = 49,
		E_311_FirstUnitEject = 50,
		E_312_SecondUnitUnlock = 51,
		E_313_SecondUnitEject = 52,
		E_314_ThirdUnitUnlock = 53,
		E_315_ThirdUnitEject = 54,
		E_316_FindWayEject = 55,
		E_317_InjectFuel = 56,
		E_318_Overheating = 57,
		E_319_Escape = 58,
		COUNT = 59
	}

	private const float hideControlsInactivityTime = 20f;

	public static GuiHUD Instance;

	public static float InfoMessageTime = 5f;

	private Mission m_Mission;

	private int m_GuiSelectedWeaponIndex;

	public bool Initialised;

	private static GUIBase_Pivot m_PivotMain;

	private static GUIBase_Pivot m_PivotWeaponSelector;

	private static GUIBase_Layout m_LayoutMain;

	private static GUIBase_Layout m_LayoutWeaponSelector;

	private static GUIBase_Button m_PauseButton;

	private static GUIBase_Sprite m_DPad;

	private static GUIBase_Sprite m_DPadjoy;

	private static GUIBase_Button[] m_WeaponButton;

	private static GUIBase_Button m_AttackButton;

	private static GUIBase_Button m_UseButton;

	private static GUIBase_Button m_ReloadButton;

	private static GUIBase_Button m_RollButton;

	private static GUIBase_Button[] m_SelectWeapon;

	private static Vector2[] m_OrigSelectWeaponPos;

	private static GUIBase_Pivot m_PivotMessages;

	private static GUIBase_Layout[] m_LayoutMessage;

	private static GUIBase_Sprite[] m_Crosshair = new GUIBase_Sprite[2];

	private static GUIBase_Pivot m_PivotMemoryWarning;

	private static GUIBase_Layout m_LayoutMemoryWarning;

	private static GUIBase_Pivot m_PivotMissionSuccess;

	private static GUIBase_Layout m_LayoutContinueButton;

	private GUIBase_Button m_ContinueButton;

	private static GUIBase_Layout m_LayoutGameEndText;

	private static GUIBase_Layout m_LayoutMissionText;

	private GUIBase_Button.TouchDelegate m_OnMissionSuccessDelegate;

	public bool m_MissionSuccessScreenOn;

	private AgentAction m_WaitForEndOfWeaponChange;

	private MenuController MenuCtrl = new MenuController();

	private E_ActionButton m_ActionButtonState;

	private bool hideWeaponControls;

	private bool hideTouchControls;

	private bool hideCrosshair;

	private static Ammo[] m_Ammo;

	private static string s_PivotMainName = "MainHUD";

	private static string s_PivotWeaponSelectorName = "WeaponSelector";

	private static string s_LayoutMainName = "HUD_Layout";

	private static string s_LayoutWeaponSelectorName = "W_Layout";

	private static string s_PivotMissionSuccessName = "MissionSuccess";

	private static string s_PauseButtonName = "PauseButton";

	private static string s_DPadName = "Dpad";

	private static string s_DPadjoyName = "Dpadjoy";

	private static string s_AttackButtonName = "FireButton";

	private static string s_UseButtonName = "UseButton";

	private static string[] s_WeaponButtonName = new string[5] { "W_SMG_selected", "W_Grn_selected", "W_Sht_selected", "W_Rck_selected", "W_Plasma_selected" };

	private static string s_AmmoMagName = "Ammo_Mag";

	private static string s_AmmoMagRedName = "Ammo_Mag_Red";

	private static string s_AmmoSupplyName = "Ammo_Rest";

	private static string s_AmmoSelection = "Ammo_All";

	private static string s_ReloadButtonName = "ReloadButton";

	private static string s_RollButtonName = "RollButton";

	private static string[] s_CrosshairName = new string[2] { "Crosshair", "CrosshairTarget" };

	private static string s_PivotMemoryWarning = "MemoryWarning";

	private static string s_LayoutMemoryWarning = "MemoryWarning_Layout";

	private static E_WeaponType[] s_WeaponType = new E_WeaponType[5]
	{
		E_WeaponType.SMG,
		E_WeaponType.GrenadeLauncher,
		E_WeaponType.Shotgun,
		E_WeaponType.RocketLauncher,
		E_WeaponType.PlasmaRifle
	};

	private static int[] s_CriticalAmmo = new int[5] { 10, 0, 2, 0, 10 };

	private bool m_AmmoBlink;

	private bool m_PlayingCutscene;

	private static GUIBase_Button.TouchDelegate[] s_SelectWeaponDlgt = new GUIBase_Button.TouchDelegate[5] { TouchDelegateSMG, TouchDelegateGrn, TouchDelegateSht, TouchDelegateRck, TouchDelegatePlasma };

	private E_CrosshairType m_CrosshairType;

	private static string[] s_MessageName = new string[59]
	{
		"Death_Layout", "FailedSara_Layout", "01_GameLoaded", "02_Checkpoint", "03_PickedAmmoSMG", "04_PickedAmmoShotg", "05_PickedAmmoGrnd", "06_PickedAmmoRPG", "07_NewWeaponShotg", "08_NewWeaponGrnd",
		"09_NewWeaponRPG", "10_SupplyCrate", "11_Collectible", "12_PickedAmmoPlasma", "13_NewWeaponPlasma", "101_DoorsUnlocked", "102_ElevatorActivated", "103_LadderActivated", "104_ExcavatorProgram", "105_Forcefield",
		"106_TerminalMalfunction", "107_PowernodeRestarted", "108_PlatformActivated", "109_IgnitorDeactivated", "110_IgnitorActivated", "112_CargoPlatform", "113_CargoGate", "114_DeactivateMissile", "115_MissileDeactivated", "116_BulkheadOpened",
		"117_PowerDeactivated", "118_CraneActivated", "201_TutorialHint01", "202_TutorialHint02", "203_TutorialHint03", "204_TutorialHint04", "205_TutorialHint05", "119_KeycardPickedUp", "120_FindKeycard", "300_ActivateBeacon",
		"301_FindWayFacility", "302_Investigate", "303_FanBlock", "304_HotCircuit", "305_Pursue", "306_RestorePower", "307_ElevatorPower", "308_FindWayDestroy", "309_UnlockEject", "310_FirstUnitUnlock",
		"311_FirstUnitEject", "312_SecondUnitUnlock", "313_SecondUnitEject", "314_ThirdUnitUnlock", "315_ThirdUnitEject", "316_FindWayEject", "317_InjectFuel", "318_Overheating", "319_Escape"
	};

	public E_HudMessageType m_ShowingMessage = E_HudMessageType.E_NONE;

	public bool IsHidden { get; private set; }

	public bool IsWeaponChanging()
	{
		return m_WaitForEndOfWeaponChange != null && m_WaitForEndOfWeaponChange.IsActive();
	}

	private bool IsPartHidden(E_HudWidget part)
	{
		switch (part)
		{
		case E_HudWidget.Pause:
			return false;
		case E_HudWidget.Crosshair:
			return hideWeaponControls || hideCrosshair;
		case E_HudWidget.Weapon:
			return hideWeaponControls;
		case E_HudWidget.Move:
			return hideTouchControls;
		case E_HudWidget.Fire:
			return hideTouchControls || hideWeaponControls;
		case E_HudWidget.Use:
			return hideWeaponControls;
		case E_HudWidget.Reload:
			return hideTouchControls || hideWeaponControls;
		case E_HudWidget.Roll:
			return hideTouchControls || hideWeaponControls;
		default:
			return false;
		}
	}

	private static int WeaponIndex(E_WeaponType t)
	{
		switch (t)
		{
		case E_WeaponType.SMG:
			return 0;
		case E_WeaponType.GrenadeLauncher:
			return 1;
		case E_WeaponType.Shotgun:
			return 2;
		case E_WeaponType.RocketLauncher:
			return 3;
		case E_WeaponType.PlasmaRifle:
			return 4;
		default:
			Debug.LogWarning("Trying to set weapon unssupported for player: " + t);
			return 0;
		}
	}

	public static E_HudMessageType WeaponAmmoMessage(E_WeaponType t)
	{
		switch (t)
		{
		case E_WeaponType.SMG:
			return E_HudMessageType.E_003_PickedAmmoSMG;
		case E_WeaponType.Shotgun:
			return E_HudMessageType.E_004_PickedAmmoShotg;
		case E_WeaponType.GrenadeLauncher:
			return E_HudMessageType.E_005_PickedAmmoGrnd;
		case E_WeaponType.RocketLauncher:
			return E_HudMessageType.E_006_PickedAmmoRPG;
		case E_WeaponType.PlasmaRifle:
			return E_HudMessageType.E_012_PickedAmmoPlasma;
		default:
			return E_HudMessageType.E_NONE;
		}
	}

	private void Awake()
	{
		Instance = this;
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
		CancelInvoke();
		Instance = null;
	}

	private void Start()
	{
		BlackBoard blackBoard = Player.Instance.Owner.BlackBoard;
		blackBoard.ActionHandler = (BlackBoard.AgentActionHandler)Delegate.Combine(blackBoard.ActionHandler, new BlackBoard.AgentActionHandler(HandleAction));
	}

	public void Init(Mission mission)
	{
		m_Mission = mission;
		IsHidden = true;
		Initialised = true;
		m_PivotMain = MFGuiManager.Instance.GetPivot(s_PivotMainName);
		if (!m_PivotMain)
		{
			Debug.LogError("'" + s_PivotMainName + "' not found!!! Assert should come now");
			return;
		}
		m_PivotWeaponSelector = MFGuiManager.Instance.GetPivot(s_PivotWeaponSelectorName);
		if (!m_PivotWeaponSelector)
		{
			Debug.LogError("'" + s_PivotWeaponSelectorName + "' not found!!! Assert should come now");
			return;
		}
		m_LayoutMain = m_PivotMain.GetLayout(s_LayoutMainName);
		if (!m_LayoutMain)
		{
			Debug.LogError("'" + s_LayoutMainName + "' not found!!! Assert should come now");
			return;
		}
		m_LayoutWeaponSelector = m_PivotWeaponSelector.GetLayout(s_LayoutWeaponSelectorName);
		if (!m_LayoutWeaponSelector)
		{
			Debug.LogError("'" + s_LayoutWeaponSelectorName + "' not found!!! Assert should come now");
			return;
		}
		m_LayoutWeaponSelector.RegisterLayoutTouchDelegate(OnWeaponSelectTouchOutside);
		m_PivotMissionSuccess = MFGuiManager.Instance.GetPivot(s_PivotMissionSuccessName);
		if (!m_PivotMissionSuccess)
		{
			Debug.LogError("'" + s_PivotMissionSuccessName + "' not found!");
			return;
		}
		m_LayoutContinueButton = GuiBaseUtils.GetLayout("Button_Layout", m_PivotMissionSuccess);
		m_LayoutGameEndText = GuiBaseUtils.GetLayout("GameEndText_Layout", m_PivotMissionSuccess);
		m_LayoutMissionText = GuiBaseUtils.GetLayout("MissionText_Layout", m_PivotMissionSuccess);
		m_PivotMemoryWarning = MFGuiManager.Instance.GetPivot(s_PivotMemoryWarning);
		m_LayoutMemoryWarning = GuiBaseUtils.GetLayout(s_LayoutMemoryWarning, m_PivotMemoryWarning);
		GuiBaseUtils.RegisterButtonDelegate(m_LayoutMemoryWarning, "Button_OK", null, OnMemoryWarningOKTouch);
		m_ContinueButton = GuiBaseUtils.RegisterButtonDelegate(m_LayoutContinueButton, "Button_Continue", OnMissionSuccessTouch, null);
		m_PauseButton = GuiBaseUtils.RegisterButtonDelegate(m_LayoutMain, s_PauseButtonName, null, PauseButtonDelegate);
		m_AttackButton = GuiBaseUtils.RegisterButtonDelegate(m_LayoutMain, s_AttackButtonName, AttackButtonBeginDelegate, AttackButtonEndDelegate);
		m_UseButton = GuiBaseUtils.RegisterButtonDelegate(m_LayoutMain, s_UseButtonName, UseButtonDelegate, null);
		m_ReloadButton = GuiBaseUtils.RegisterButtonDelegate(m_LayoutMain, s_ReloadButtonName, OnReloadButton, null);
		m_RollButton = GuiBaseUtils.RegisterButtonDelegate(m_LayoutMain, s_RollButtonName, OnRollButtonPress, null);
		if (s_WeaponButtonName.Length > 0)
		{
			m_WeaponButton = new GUIBase_Button[s_WeaponButtonName.Length];
			m_Ammo = new Ammo[s_WeaponButtonName.Length];
			for (int i = 0; i < s_WeaponButtonName.Length; i++)
			{
				m_WeaponButton[i] = GuiBaseUtils.RegisterButtonDelegate(m_LayoutMain, s_WeaponButtonName[i], WeaponButtonDelegate, null);
				m_Ammo[i] = new Ammo();
				m_Ammo[i].m_Clip = GetChildNumber(m_WeaponButton[i], s_AmmoMagName);
				m_Ammo[i].m_ClipRed = GetChildNumber(m_WeaponButton[i], s_AmmoMagRedName);
				m_Ammo[i].m_Supply = GetChildNumber(m_WeaponButton[i], s_AmmoSupplyName);
			}
		}
		m_SelectWeapon = new GUIBase_Button[s_WeaponButtonName.Length];
		m_OrigSelectWeaponPos = new Vector2[s_WeaponButtonName.Length];
		if (s_WeaponButtonName.Length > 0)
		{
			for (int j = 0; j < s_WeaponButtonName.Length; j++)
			{
				m_SelectWeapon[j] = GuiBaseUtils.RegisterButtonDelegate(m_LayoutWeaponSelector, s_WeaponButtonName[j], s_SelectWeaponDlgt[j], null);
				m_OrigSelectWeaponPos[j] = m_SelectWeapon[j].transform.position;
				m_Ammo[j].m_SelectionAll = GetChildNumber(m_SelectWeapon[j], s_AmmoSelection);
			}
		}
		m_DPad = PrepareSprite(m_LayoutMain, s_DPadName);
		m_DPadjoy = PrepareSprite(m_LayoutMain, s_DPadjoyName);
		m_Crosshair[0] = PrepareSprite(m_LayoutMain, s_CrosshairName[0]);
		m_Crosshair[1] = PrepareSprite(m_LayoutMain, s_CrosshairName[1]);
		StoreControlsOrigPositions();
		UpdateControlsPosition();
		Player.Instance.Controls.TouchControls.OnControlSchemeChange();
		m_GuiSelectedWeaponIndex = 0;
		InitMessages();
		InvokeRepeating("UpdateCrosshair", 0f, 0.1f);
		StartCoroutine(UpdateAmmoBlinking());
	}

	private void InitMessages()
	{
		m_LayoutMessage = new GUIBase_Layout[59];
		m_PivotMessages = MFGuiManager.Instance.GetPivot("Messages");
		if (!m_PivotMessages)
		{
			Debug.LogError("Pivot 'Messages' not found! ");
			return;
		}
		for (int i = 0; i < 59; i++)
		{
			m_LayoutMessage[i] = GuiBaseUtils.GetLayout(s_MessageName[i], m_PivotMessages);
		}
	}

	public bool FingerIdInUse(int fingerId)
	{
		if (m_LayoutMain.FingerIdInUse(fingerId))
		{
			return true;
		}
		if (m_LayoutWeaponSelector.FingerIdInUse(fingerId))
		{
			return true;
		}
		return false;
	}

	public void ReleaseFinger(Touch touch)
	{
		m_LayoutMain.ReleaseFinger(touch);
	}

	private void StoreControlsOrigPositions()
	{
		GuiOptions.FireUseButton.OrigPos = new Vector2(m_AttackButton.transform.position.x, m_AttackButton.transform.position.y);
		GuiOptions.PauseButton.OrigPos = new Vector2(m_PauseButton.transform.position.x, m_PauseButton.transform.position.y);
		GuiOptions.WeaponButton.OrigPos = new Vector2(m_WeaponButton[0].transform.position.x, m_WeaponButton[0].transform.position.y);
		GuiOptions.MoveStick.OrigPos = new Vector2(m_DPad.transform.position.x, m_DPad.transform.position.y);
		GuiOptions.ReloadButton.OrigPos = new Vector2(m_ReloadButton.transform.position.x, m_ReloadButton.transform.position.y);
		GuiOptions.RollButton.OrigPos = new Vector2(m_RollButton.transform.position.x, m_RollButton.transform.position.y);
		if (GuiOptions.leftHandControlsNeedUpdate)
		{
			GuiOptions.SwitchLeftHandAimingControls();
		}
	}

	private GUIBase_Number GetChildNumber(GUIBase_Button btn, string name)
	{
		Transform transform = btn.transform.Find(name);
		return (!(transform != null)) ? null : transform.GetComponent<GUIBase_Number>();
	}

	public void Hide()
	{
		MFGuiManager.Instance.ShowPivot(m_PivotMain, false);
		HideWeaponsSelection();
		ClearWaitingOnWeaponChange();
		IsHidden = true;
	}

	public void Show()
	{
		if (!m_PlayingCutscene)
		{
			MFGuiManager.Instance.ShowPivot(m_PivotMain, true);
			HideAllWeaponsSprites();
			HideWeaponsSelection();
			ShowWeaponSprite();
			ShowJoystickUp();
			Player.Instance.UpdateUseModeHACK();
			UpdateActiveParts();
			IsHidden = false;
		}
	}

	private void LateUpdate()
	{
		if (!IsHidden && Initialised)
		{
			UpdateAmmoDisplay(m_GuiSelectedWeaponIndex);
			if (m_WaitForEndOfWeaponChange != null && !m_WaitForEndOfWeaponChange.IsActive())
			{
				ClearWaitingOnWeaponChange();
			}
		}
	}

	private void DisableWeaponSelection(bool disable)
	{
		GUIBase_Button[] weaponButton = m_WeaponButton;
		foreach (GUIBase_Button gUIBase_Button in weaponButton)
		{
			gUIBase_Button.SetDisabled(disable);
		}
	}

	private void UpdateCrosshair()
	{
		bool flag = false;
		if (!IsHidden && !(Camera.main == null) && !IsPartHidden(E_HudWidget.Crosshair))
		{
			Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
			GameObject gameObject = CollisionUtils.FirstCollisionOnRay(ray, 70f, Player.Instance.Owner.GameObject);
			if ((bool)gameObject && (gameObject.GetComponent<Agent>() != null || gameObject.GetComponent<BrooderMain>() != null || gameObject.GetComponent<MongoMain>() != null || gameObject.GetComponent<Armor>() != null || gameObject.GetComponent<BreakableObject>() != null || gameObject.GetComponent<DamageEvent>() != null || gameObject.GetComponent<DamageSwitch>() != null || gameObject.GetComponent<DestroyObject>() != null || gameObject.GetComponent<HitZone>() != null))
			{
				flag = true;
			}
			if (flag)
			{
				ShowCrosshair(E_CrosshairType.Target);
			}
			else
			{
				ShowCrosshair(E_CrosshairType.Normal);
			}
		}
	}

	private void ShowCrosshair(E_CrosshairType type)
	{
		if (m_CrosshairType != type)
		{
			if (m_CrosshairType != E_CrosshairType.None)
			{
				m_Crosshair[(int)m_CrosshairType].Widget.Show(false, true);
			}
			if (type != E_CrosshairType.None)
			{
				m_Crosshair[(int)type].Widget.Show(true, true);
			}
			m_CrosshairType = type;
		}
	}

	private static Vector3 ScreenToWidget(Vector2 pos)
	{
		pos.y = (float)Screen.height - pos.y;
		return new Vector3(pos.x, pos.y, 0f);
	}

	public void JoystickBaseShow(Vector2 center)
	{
		if ((bool)m_DPad)
		{
			m_DPad.Widget.Show(true, true);
			Vector3 position = ScreenToWidget(center);
			m_DPad.transform.position = position;
		}
	}

	public void JoystickBaseHide()
	{
		if ((bool)m_DPad)
		{
			m_DPad.Widget.Show(false, true);
		}
	}

	public void JoystickDown(Vector2 center)
	{
		if ((bool)m_DPadjoy)
		{
			m_DPadjoy.Widget.Show(true, true);
			Vector3 position = ScreenToWidget(center);
			m_DPadjoy.transform.position = position;
		}
	}

	public void JoystickUpdate(Vector2 center)
	{
		if ((bool)m_DPadjoy)
		{
			Vector3 position = ScreenToWidget(center);
			m_DPadjoy.transform.position = position;
		}
	}

	public void JoystickUp()
	{
		if ((bool)m_DPadjoy)
		{
			m_DPadjoy.Widget.Show(false, true);
		}
	}

	private void ShowJoystickUp()
	{
		switch (GuiOptions.m_ControlScheme)
		{
		case GuiOptions.E_ControlScheme.Scheme1:
			if ((bool)m_DPad)
			{
				m_DPad.Widget.Show(false, true);
			}
			if ((bool)m_DPadjoy)
			{
				m_DPadjoy.Widget.Show(false, true);
			}
			break;
		case GuiOptions.E_ControlScheme.Scheme2:
			if ((bool)m_DPad)
			{
				m_DPad.Widget.Show(true, true);
				Vector3 position = new Vector3(GuiOptions.MoveStick.Positon.x, GuiOptions.MoveStick.Positon.y, 0f);
				m_DPad.transform.position = position;
			}
			if ((bool)m_DPadjoy)
			{
				m_DPadjoy.Widget.Show(false, true);
			}
			break;
		}
	}

	private GUIBase_Sprite PrepareSprite(GUIBase_Layout layout, string name)
	{
		GUIBase_Sprite result = null;
		GUIBase_Widget widget = layout.GetWidget(name);
		if ((bool)widget)
		{
			result = widget.GetComponent<GUIBase_Sprite>();
		}
		return result;
	}

	private GUIBase_Number PrepareNumber(GUIBase_Layout layout, string name)
	{
		GUIBase_Number result = null;
		GUIBase_Widget widget = layout.GetWidget(name);
		if ((bool)widget)
		{
			result = widget.GetComponent<GUIBase_Number>();
		}
		return result;
	}

	private void PauseButtonDelegate(bool inside)
	{
		if (inside)
		{
			SwitchToIngameMenu();
		}
	}

	public void SwitchToIngameMenu()
	{
		if (!(m_Mission == null) && Game.Instance.GameState != E_GameState.IngameMenu && MFGuiManager.Instance.FadeState == MFGuiManager.E_Fading.None)
		{
			m_Mission.ChangeGuiState(Mission.E_GuiState.E_GS_INGAME_MENU);
			GuiSubtitles.ShowAllRunning(false);
			SuspendCurrentMessage(true);
			GuiTimer.SuspendAllRunning(true);
			TimeBar.SuspendAllRunning(true);
			BossHealth.SuspendAllRunning(true);
			GuiGameEnd.Suspend(true);
			GuiSubtitlesRenderer.Suspend(true);
			Game.Instance.GameState = E_GameState.IngameMenu;
			AudioListener.pause = true;
			Time.timeScale = 0f;
			Game.Instance.TryToCleanSomeMemory();
		}
	}

	public void SwitchBackFromIngameMenu()
	{
		if ((bool)GuiSniffMinigame.Instance && GuiSniffMinigame.Instance.InProgress)
		{
			Screen.lockCursor = false;
			m_Mission.ChangeGuiState(Mission.E_GuiState.E_GS_SNIFF_MINIGAME);
		}
		else
		{
			Screen.lockCursor = true;
			GuiSubtitles.ShowAllRunning(true);
			m_Mission.ChangeGuiState(Mission.E_GuiState.E_GS_HUD);
		}
		SuspendCurrentMessage(false);
		GuiTimer.SuspendAllRunning(false);
		TimeBar.SuspendAllRunning(false);
		BossHealth.SuspendAllRunning(false);
		GuiGameEnd.Suspend(false);
		GuiSubtitlesRenderer.Suspend(false);
		Game.Instance.GameState = E_GameState.Game;
		Time.timeScale = 1f;
		AudioListener.pause = false;
	}

	private void AttackButtonBeginDelegate()
	{
		Player.Instance.Controls.FireDownDelegate();
	}

	private void AttackButtonEndDelegate(bool inside)
	{
		Player.Instance.Controls.FireUpDelegate();
	}

	private void UseButtonDelegate()
	{
		Player.Instance.Controls.UseDelegate();
	}

	private void WeaponButtonDelegate()
	{
		HideWeaponSprite();
		ShowWeaponsSelection();
		Invoke("CancelWeaponSelection", 6f);
	}

	private void CancelWeaponSelection()
	{
		HideWeaponsSelection();
		ShowWeaponSprite();
	}

	private static void OnReloadButton()
	{
		Player.Instance.Controls.ReloadDelegate();
	}

	private static void OnRollButtonPress()
	{
		Player.Instance.Controls.RollDelegate();
	}

	private static void TouchDelegateGrn()
	{
		Instance.SelectWeapon(E_WeaponType.GrenadeLauncher);
	}

	private static void TouchDelegateRck()
	{
		Instance.SelectWeapon(E_WeaponType.RocketLauncher);
	}

	private static void TouchDelegateSht()
	{
		Instance.SelectWeapon(E_WeaponType.Shotgun);
	}

	private static void TouchDelegateSMG()
	{
		Instance.SelectWeapon(E_WeaponType.SMG);
	}

	private static void TouchDelegatePlasma()
	{
		Instance.SelectWeapon(E_WeaponType.PlasmaRifle);
	}

	private void OnWeaponSelectTouchOutside()
	{
		CancelInvoke("CancelWeaponSelection");
		HideWeaponsSelection();
		ShowWeaponSprite();
	}

	private void SelectWeapon(E_WeaponType type)
	{
		if (Player.Instance.CanChangeWeapon())
		{
			int num = WeaponIndex(type);
			if (num != m_GuiSelectedWeaponIndex)
			{
				m_GuiSelectedWeaponIndex = num;
				Player.Instance.Controls.ChangeWeaponDelegate(s_WeaponType[m_GuiSelectedWeaponIndex]);
			}
			CancelInvoke("CancelWeaponSelection");
			HideWeaponsSelection();
			ShowWeaponSprite();
		}
	}

	public void OnCurrentWeaponChanged(E_WeaponType Weapon)
	{
		HideWeaponSprite();
		int guiSelectedWeaponIndex = WeaponIndex(Weapon);
		m_GuiSelectedWeaponIndex = guiSelectedWeaponIndex;
		ShowWeaponSprite();
	}

	private void ShowWeaponSprite()
	{
		if (m_WeaponButton != null)
		{
			m_WeaponButton[m_GuiSelectedWeaponIndex].Widget.Show(true, true);
			Instance.UpdateAmmoDisplay(m_GuiSelectedWeaponIndex);
		}
	}

	private void HideWeaponSprite()
	{
		int guiSelectedWeaponIndex = m_GuiSelectedWeaponIndex;
		if (m_WeaponButton != null)
		{
			m_WeaponButton[guiSelectedWeaponIndex].Widget.Show(false, true);
		}
	}

	private void HideAllWeaponsSprites()
	{
		if (m_WeaponButton != null)
		{
			for (int i = 0; i < m_WeaponButton.Length; i++)
			{
				m_WeaponButton[i].Widget.Show(false, true);
			}
		}
	}

	private void ShowWeaponsSelection()
	{
		HighlightSelectedWeapon();
		UpdateAllAmmo();
		MFGuiManager.Instance.ShowPivot(m_PivotWeaponSelector, true);
		UpdatePlayerAvailableWeapons();
	}

	private void HideWeaponsSelection()
	{
		MFGuiManager.Instance.ShowPivot(m_PivotWeaponSelector, false);
	}

	private void UpdateAmmoDisplay(int idx)
	{
		if (Player.Instance == null || Player.Instance.Owner == null || Player.Instance.Owner.WeaponComponent == null || Player.Instance.Owner.WeaponComponent.CurrentWeapon == E_WeaponType.None)
		{
			return;
		}
		WeaponBase weapon = Player.Instance.Owner.WeaponComponent.GetWeapon(s_WeaponType[idx]);
		if (weapon == null)
		{
			m_ReloadButton.SetDisabled(true);
		}
		else
		{
			if (m_Ammo == null || idx >= m_Ammo.Length)
			{
				return;
			}
			Ammo ammo = m_Ammo[idx];
			bool flag = weapon.ClipAmmo > 0 && weapon.ClipAmmo <= s_CriticalAmmo[idx];
			if ((bool)ammo.m_Clip && (bool)ammo.m_ClipRed)
			{
				ammo.m_Clip.SetNumber(weapon.ClipAmmo, 999);
				if (flag)
				{
					ammo.m_Clip.Widget.Show(m_WeaponButton[idx].Widget.IsVisible() && m_AmmoBlink, true);
					ammo.m_ClipRed.SetNumber(weapon.ClipAmmo, 999);
					ammo.m_ClipRed.Widget.Show(m_WeaponButton[idx].Widget.IsVisible() && !m_AmmoBlink, true);
				}
				else
				{
					ammo.m_Clip.Widget.Show(m_WeaponButton[idx].Widget.IsVisible(), true);
					ammo.m_ClipRed.Widget.Show(false, true);
				}
			}
			if (ammo.m_Supply != null)
			{
				ammo.m_Supply.SetNumber(weapon.WeaponAmmo, 999);
			}
			if (ammo.m_SelectionAll != null)
			{
				ammo.m_SelectionAll.SetNumber(weapon.WeaponAmmo + weapon.ClipAmmo, 999);
			}
			if (weapon.WeaponAmmo > 0 && weapon.ClipAmmo < weapon.MaxAmmoInClip)
			{
				m_ReloadButton.SetDisabled(false);
			}
			else
			{
				m_ReloadButton.SetDisabled(true);
			}
		}
	}

	private void UpdateAllAmmo()
	{
		for (int i = 0; i < s_WeaponType.Length; i++)
		{
			UpdateAmmoDisplay(i);
		}
	}

	public void UpdateControlsPosition()
	{
		m_AttackButton.transform.position = GuiOptions.FireUseButton.Positon;
		m_UseButton.transform.position = GuiOptions.FireUseButton.Positon;
		m_PauseButton.transform.position = GuiOptions.PauseButton.Positon;
		m_ReloadButton.transform.position = GuiOptions.ReloadButton.Positon;
		m_RollButton.transform.position = GuiOptions.RollButton.Positon;
		GUIBase_Button[] weaponButton = m_WeaponButton;
		foreach (GUIBase_Button gUIBase_Button in weaponButton)
		{
			gUIBase_Button.transform.position = GuiOptions.WeaponButton.Positon;
		}
		for (int j = 0; j < m_SelectWeapon.Length; j++)
		{
			m_SelectWeapon[j].transform.position = m_OrigSelectWeaponPos[j] + GuiOptions.WeaponButton.Offset;
		}
	}

	private void UpdatePlayerAvailableWeapons()
	{
		for (int i = 0; i < s_WeaponType.Length; i++)
		{
			WeaponBase weapon = Player.Instance.Owner.WeaponComponent.GetWeapon(s_WeaponType[i]);
			if ((bool)weapon)
			{
				m_SelectWeapon[i].Widget.Show(true, true);
				if (weapon.WeaponAmmo > 0 || weapon.ClipAmmo > 0)
				{
					m_SelectWeapon[i].SetDisabled(false);
				}
				else
				{
					m_SelectWeapon[i].SetDisabled(true);
				}
			}
			else
			{
				m_SelectWeapon[i].Widget.Show(false, true);
			}
		}
	}

	private void HighlightSelectedWeapon()
	{
		for (int i = 0; i < m_SelectWeapon.Length; i++)
		{
			if (i != m_GuiSelectedWeaponIndex)
			{
				m_SelectWeapon[i].ForceHighlight(false);
			}
		}
		m_SelectWeapon[m_GuiSelectedWeaponIndex].ForceHighlight(true);
	}

	public void ShowMessage(E_HudMessageType message, bool show)
	{
		if (message == E_HudMessageType.E_NONE || message == E_HudMessageType.COUNT || m_LayoutMessage == null)
		{
			return;
		}
		GUIBase_Layout gUIBase_Layout = m_LayoutMessage[(int)message];
		if (gUIBase_Layout == null)
		{
			Debug.LogWarning(string.Concat("Message  ", message, "not found"));
			return;
		}
		if (show)
		{
			if (m_ShowingMessage != E_HudMessageType.E_NONE)
			{
				GUIBase_Layout layout = m_LayoutMessage[(int)m_ShowingMessage];
				MFGuiManager.Instance.ShowLayout(layout, false);
			}
			m_ShowingMessage = message;
		}
		else if (m_ShowingMessage == message)
		{
			m_ShowingMessage = E_HudMessageType.E_NONE;
		}
		MFGuiManager.Instance.ShowLayout(gUIBase_Layout, show);
	}

	private void SuspendCurrentMessage(bool suspend)
	{
		if (m_ShowingMessage != E_HudMessageType.E_NONE)
		{
			GUIBase_Layout gUIBase_Layout = m_LayoutMessage[(int)m_ShowingMessage];
			gUIBase_Layout.ShowImmediate(!suspend, false);
		}
	}

	public void ShowMessageTimed(E_HudMessageType message, float time)
	{
		StartCoroutine(_ShowMessageTimed(message, time));
	}

	private IEnumerator _ShowMessageTimed(E_HudMessageType message, float time)
	{
		ShowMessage(message, true);
		yield return new WaitForSeconds(time);
		ShowMessage(message, false);
	}

	public void HideAllMessages()
	{
		GUIBase_Layout[] layoutMessage = m_LayoutMessage;
		foreach (GUIBase_Layout gUIBase_Layout in layoutMessage)
		{
			if ((bool)gUIBase_Layout)
			{
				MFGuiManager.Instance.ShowLayout(gUIBase_Layout, false);
			}
		}
		if ((bool)m_PivotMessages)
		{
			MFGuiManager.Instance.ShowPivot(m_PivotMessages, false);
		}
		m_ShowingMessage = E_HudMessageType.E_NONE;
	}

	public void HideWeaponControls()
	{
		hideWeaponControls = true;
		UpdateActiveParts();
	}

	public void ShowWeaponControls()
	{
		hideWeaponControls = false;
		UpdateActiveParts();
	}

	public void PlayCutsceneBegin()
	{
		m_PlayingCutscene = true;
	}

	public void PlayCutsceneEnd()
	{
		m_PlayingCutscene = false;
	}

	public bool IsPlayingCutscene()
	{
		return m_PlayingCutscene;
	}

	public void EnableCrosshair(bool on)
	{
		bool flag = !on;
		if (hideCrosshair != flag)
		{
			hideCrosshair = flag;
			if (IsPartHidden(E_HudWidget.Crosshair))
			{
				m_Crosshair[0].Widget.Show(false, true);
				m_Crosshair[1].Widget.Show(false, true);
				m_CrosshairType = E_CrosshairType.None;
			}
		}
	}

	private void UpdateActiveParts()
	{
		if ((bool)m_PauseButton)
		{
			m_PauseButton.Widget.Show(!IsPartHidden(E_HudWidget.Pause), true);
		}
		if ((bool)m_ReloadButton)
		{
			m_ReloadButton.Widget.Show(!IsPartHidden(E_HudWidget.Reload), true);
		}
		if ((bool)m_RollButton)
		{
			m_RollButton.Widget.Show(!IsPartHidden(E_HudWidget.Roll), true);
		}
		UpdateActionButton();
		if ((bool)m_DPad && (GuiOptions.m_ControlScheme == GuiOptions.E_ControlScheme.Scheme2 || IsPartHidden(E_HudWidget.Move)))
		{
			m_DPad.Widget.Show(!IsPartHidden(E_HudWidget.Move), true);
		}
		if ((bool)m_DPadjoy && IsPartHidden(E_HudWidget.Move))
		{
			m_DPadjoy.Widget.Show(!IsPartHidden(E_HudWidget.Move), true);
		}
		HideWeaponSprite();
		if (!IsPartHidden(E_HudWidget.Weapon))
		{
			ShowWeaponSprite();
		}
		if (IsPartHidden(E_HudWidget.Crosshair))
		{
			if ((bool)m_Crosshair[0])
			{
				m_Crosshair[0].Widget.Show(false, true);
			}
			if ((bool)m_Crosshair[1])
			{
				m_Crosshair[1].Widget.Show(false, true);
			}
			m_CrosshairType = E_CrosshairType.None;
		}
	}

	private void UpdateActionButton()
	{
		if ((bool)m_AttackButton)
		{
			m_AttackButton.Widget.Show(!IsPartHidden(E_HudWidget.Fire) && m_ActionButtonState == E_ActionButton.Fire, true);
		}
		if ((bool)m_UseButton)
		{
			m_UseButton.Widget.Show(!IsPartHidden(E_HudWidget.Use) && m_ActionButtonState == E_ActionButton.Use, true);
		}
	}

	private IEnumerator UpdateAmmoBlinking()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.25f);
			m_AmmoBlink = true;
			yield return new WaitForSeconds(0.25f);
			m_AmmoBlink = false;
		}
	}

	public void ShowActionButton(E_ActionButton inState)
	{
		m_ActionButtonState = inState;
		UpdateActionButton();
	}

	public void Reset()
	{
		m_GuiSelectedWeaponIndex = 0;
		m_PlayingCutscene = false;
		hideWeaponControls = false;
		hideCrosshair = false;
		ClearWaitingOnWeaponChange();
		if (Player.Instance != null && Player.Instance.Owner != null && Player.Instance.Owner.WeaponComponent != null && Player.Instance.Owner.WeaponComponent.CurrentWeapon != E_WeaponType.None)
		{
			OnCurrentWeaponChanged(Player.Instance.Owner.WeaponComponent.CurrentWeapon);
		}
	}

	public void ShowMissionSuccess(GUIBase_Button.TouchDelegate touchDelegate, bool finalLevel)
	{
		Hide();
		MFGuiManager.Instance.ShowLayout(m_LayoutContinueButton, true);
		if (finalLevel)
		{
			MFGuiManager.Instance.ShowLayout(m_LayoutGameEndText, true);
		}
		else
		{
			MFGuiManager.Instance.ShowLayout(m_LayoutMissionText, true);
		}
		Screen.lockCursor = false;
		m_OnMissionSuccessDelegate = touchDelegate;
		m_MissionSuccessScreenOn = true;
	}

	private void Update()
	{
		if (!Initialised)
		{
			return;
		}
		if (m_MissionSuccessScreenOn)
		{
			MenuCtrl.Update();
			if (MenuCtrl.PressedOk() && (bool)m_ContinueButton && m_ContinueButton.Widget.IsVisible() && !m_ContinueButton.IsDisabled())
			{
				m_ContinueButton.Callback(GUIBase_Callback.E_CallbackType.E_CT_ON_TOUCH_BEGIN);
			}
		}
		if (!IsHidden)
		{
			HideControls(true);
		}
	}

	private bool IsMogaConnected()
	{
		return MogaGamepad.IsConnected();
	}

	private void HideMissionSuccess()
	{
		MFGuiManager.Instance.ShowLayout(m_LayoutContinueButton, false);
		MFGuiManager.Instance.ShowLayout(m_LayoutGameEndText, false);
		MFGuiManager.Instance.ShowLayout(m_LayoutMissionText, false);
		Screen.lockCursor = true;
		m_OnMissionSuccessDelegate = null;
		m_MissionSuccessScreenOn = false;
	}

	private void OnMissionSuccessTouch()
	{
		if (m_OnMissionSuccessDelegate != null)
		{
			m_OnMissionSuccessDelegate();
			Screen.lockCursor = true;
			m_OnMissionSuccessDelegate = null;
			m_MissionSuccessScreenOn = false;
		}
		else
		{
			Debug.LogError("No delegate for Mission success!");
		}
	}

	public void HandleAction(AgentAction a)
	{
		if (a is AgentActionWeaponChange)
		{
			DisableWeaponSelection(true);
			m_WaitForEndOfWeaponChange = a;
		}
		else if (a is AgentActionDeath)
		{
			ClearWaitingOnWeaponChange();
		}
	}

	private void ClearWaitingOnWeaponChange()
	{
		DisableWeaponSelection(false);
		m_WaitForEndOfWeaponChange = null;
	}

	private void HideControls(bool hide)
	{
		if (hideTouchControls != hide)
		{
			hideTouchControls = hide;
			UpdateActiveParts();
		}
	}

	private void ShowMemoryWarning(bool show)
	{
		if ((bool)MFGuiManager.Instance && (bool)m_LayoutMemoryWarning)
		{
			MFGuiManager.Instance.ShowLayout(m_LayoutMemoryWarning, show);
		}
		else
		{
			Debug.LogWarning("Gui for memory warning is not initialised.");
		}
	}

	private void OnMemoryWarningOKTouch(bool inside)
	{
		if (inside)
		{
			ShowMemoryWarning(false);
		}
	}
}
