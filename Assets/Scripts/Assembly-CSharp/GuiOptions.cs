using UnityEngine;

public class GuiOptions
{
	public enum E_ControlScheme
	{
		Scheme1 = 0,
		Scheme2 = 1
	}

	public enum E_XperiPad
	{
		Joypad = 0,
		Slide = 1
	}

	public enum E_Language
	{
		English = 0,
		German = 1,
		French = 2,
		Italian = 3,
		Japanese = 4,
		Chinese = 5,
		Korean = 6
	}

	public class ControlPos
	{
		public Vector2 OrigPos;

		public Vector2 Offset;

		public Vector2 Positon
		{
			get
			{
				return OrigPos + Offset;
			}
		}
	}

	public static float sensitivity = 1f;

	public static float musicVolume = 1f;

	public static bool subtitles = true;

	public static bool invertYAxis = false;

	public static bool leftHandAiming = false;

	public static E_ControlScheme m_ControlScheme = E_ControlScheme.Scheme1;

	public static float ListenerVolume = 1f;

	public static E_XperiPad m_XperiaPad = E_XperiPad.Slide;

	public static E_Language m_Language = E_Language.English;

	private static float DefaultMusicVolume = 1f;

	private static bool DefaultSubtitles = true;

	private static bool DefaultInvertYAxis = false;

	private static bool DefaultLeftHandAiming = false;

	private static E_ControlScheme DefaultScheme = E_ControlScheme.Scheme1;

	private static E_XperiPad DefaultXperiPad = E_XperiPad.Slide;

	private static E_Language DefaultLanguage = E_Language.English;

	public static bool leftHandControlsNeedUpdate = false;

	public static ControlPos FireUseButton = new ControlPos();

	public static ControlPos PauseButton = new ControlPos();

	public static ControlPos WeaponButton = new ControlPos();

	public static ControlPos MoveStick = new ControlPos();

	public static ControlPos ReloadButton = new ControlPos();

	public static ControlPos RollButton = new ControlPos();

	private static float DefaultSensitivity
	{
		get
		{
			if (Game.IsHDResolution())
			{
				return 2.5f;
			}
			return 1.5f;
		}
	}

	public static void SetNewLeftHandAiming(bool newVal)
	{
		if (leftHandAiming != newVal)
		{
			leftHandAiming = newVal;
			if ((bool)GuiHUD.Instance && GuiHUD.Instance.Initialised)
			{
				SwitchLeftHandAimingControls();
			}
			else
			{
				leftHandControlsNeedUpdate = true;
			}
		}
	}

	public static void ResetToDefaults()
	{
		sensitivity = DefaultSensitivity;
		musicVolume = DefaultMusicVolume;
		subtitles = DefaultSubtitles;
		invertYAxis = DefaultInvertYAxis;
		m_ControlScheme = DefaultScheme;
		m_XperiaPad = DefaultXperiPad;
		SetLanguage(DefaultLanguage);
		SetNewLeftHandAiming(DefaultLeftHandAiming);
	}

	public static void SetLanguage(E_Language l)
	{
		m_Language = l;
		Debug.Log("Language set: " + m_Language);
	}

	public static void SwitchLeftHandAimingControls()
	{
		if ((leftHandAiming && MoveStick.Positon.x < (float)(Screen.width / 2)) || (!leftHandAiming && MoveStick.Positon.x > (float)(Screen.width / 2)))
		{
			float num = (float)Screen.width - MoveStick.Positon.x;
			float x = num - MoveStick.OrigPos.x;
			MoveStick.Offset.x = x;
		}
		if ((leftHandAiming && FireUseButton.Positon.x > (float)(Screen.width / 2)) || (!leftHandAiming && FireUseButton.Positon.x < (float)(Screen.width / 2)))
		{
			float num2 = (float)Screen.width - FireUseButton.Positon.x;
			float x2 = num2 - FireUseButton.OrigPos.x;
			FireUseButton.Offset.x = x2;
		}
		GuiHUD.Instance.UpdateControlsPosition();
		Player.Instance.Controls.TouchControls.OnControlSchemeChange();
		leftHandControlsNeedUpdate = false;
	}

	public static void Save()
	{
		PlayerPrefs.SetFloat("OptionsSensitivity", sensitivity);
		PlayerPrefs.SetFloat("OptionsMusicVolume", musicVolume);
		PlayerPrefs.SetInt("OptionsSubtitles", subtitles ? 1 : 0);
		PlayerPrefs.SetInt("OptionsInvertYAxis", invertYAxis ? 1 : 0);
		PlayerPrefs.SetInt("OptionsLeftHandAiming", leftHandAiming ? 1 : 0);
		PlayerPrefs.SetInt("OptionsControlScheme", (int)m_ControlScheme);
		PlayerPrefs.SetInt("OptionsLanguage", (int)m_Language);
		PlayerPrefs.SetInt("OptionsXperiaViewPad", (int)m_XperiaPad);
		PlayerPrefs.SetFloat("OptionsFireButtonX", FireUseButton.Offset.x);
		PlayerPrefs.SetFloat("OptionsFireButtonY", FireUseButton.Offset.y);
		PlayerPrefs.SetFloat("OptionsPauseButtonX", PauseButton.Offset.x);
		PlayerPrefs.SetFloat("OptionsPauseButtonY", PauseButton.Offset.y);
		PlayerPrefs.SetFloat("OptionsWeaponButtonX", WeaponButton.Offset.x);
		PlayerPrefs.SetFloat("OptionsWeaponButtonY", WeaponButton.Offset.y);
		PlayerPrefs.SetFloat("OptionsMoveStickX", MoveStick.Offset.x);
		PlayerPrefs.SetFloat("OptionsMoveStickY", MoveStick.Offset.y);
		PlayerPrefs.SetFloat("OptionsReloadButtonX", ReloadButton.Offset.x);
		PlayerPrefs.SetFloat("OptionsReloadButtonY", ReloadButton.Offset.y);
		PlayerPrefs.SetFloat("OptionsRollButtonX", RollButton.Offset.x);
		PlayerPrefs.SetFloat("OptionsRollButtonY", RollButton.Offset.y);
		PlayerPrefs.Save();
	}

	public static void Load()
	{
		sensitivity = PlayerPrefs.GetFloat("OptionsSensitivity", DefaultSensitivity);
		musicVolume = PlayerPrefs.GetFloat("OptionsMusicVolume", DefaultMusicVolume);
		subtitles = PlayerPrefs.GetInt("OptionsSubtitles", DefaultSubtitles ? 1 : 0) != 0;
		invertYAxis = PlayerPrefs.GetInt("OptionsInvertYAxis", DefaultInvertYAxis ? 1 : 0) != 0;
		leftHandAiming = PlayerPrefs.GetInt("OptionsLeftHandAiming", DefaultLeftHandAiming ? 1 : 0) != 0;
		m_ControlScheme = (E_ControlScheme)PlayerPrefs.GetInt("OptionsControlScheme", (int)DefaultScheme);
		SetLanguage((E_Language)PlayerPrefs.GetInt("OptionsLanguage", (int)DefaultLanguage));
		m_XperiaPad = (E_XperiPad)PlayerPrefs.GetInt("OptionsXperiaViewPad", (int)DefaultXperiPad);
		FireUseButton.Offset.x = PlayerPrefs.GetFloat("OptionsFireButtonX", 0f);
		FireUseButton.Offset.y = PlayerPrefs.GetFloat("OptionsFireButtonY", 0f);
		PauseButton.Offset.x = PlayerPrefs.GetFloat("OptionsPauseButtonX", 0f);
		PauseButton.Offset.y = PlayerPrefs.GetFloat("OptionsPauseButtonY", 0f);
		WeaponButton.Offset.x = PlayerPrefs.GetFloat("OptionsWeaponButtonX", 0f);
		WeaponButton.Offset.y = PlayerPrefs.GetFloat("OptionsWeaponButtonY", 0f);
		MoveStick.Offset.x = PlayerPrefs.GetFloat("OptionsMoveStickX", 0f);
		MoveStick.Offset.y = PlayerPrefs.GetFloat("OptionsMoveStickY", 0f);
		ReloadButton.Offset.x = PlayerPrefs.GetFloat("OptionsReloadButtonX", 0f);
		ReloadButton.Offset.y = PlayerPrefs.GetFloat("OptionsReloadButtonY", 0f);
		RollButton.Offset.x = PlayerPrefs.GetFloat("OptionsRollButtonX", 0f);
		RollButton.Offset.y = PlayerPrefs.GetFloat("OptionsRollButtonY", 0f);
	}
}
