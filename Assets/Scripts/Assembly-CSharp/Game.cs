using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Game : MonoBehaviour
{
	public class PlayerPersistantState
	{
		public class InventoryInfo
		{
			public class WeaponInfo
			{
				public int AmmoInClip = -1;

				public int AmmonInWeapon = -1;
			}

			public Dictionary<E_WeaponType, WeaponInfo> Weapons = new Dictionary<E_WeaponType, WeaponInfo>();

			public E_WeaponType WeaponSelected = E_WeaponType.None;

			public void Clear()
			{
				Weapons.Clear();
				WeaponSelected = E_WeaponType.None;
			}

			public void SetDefaults(int level)
			{
				Clear();
				switch (level)
				{
				case 0:
					Weapons.Add(E_WeaponType.SMG, new WeaponInfo
					{
						AmmoInClip = 30,
						AmmonInWeapon = 30
					});
					break;
				case 2:
				case 3:
					Weapons.Add(E_WeaponType.SMG, new WeaponInfo
					{
						AmmoInClip = 30,
						AmmonInWeapon = 250
					});
					Weapons.Add(E_WeaponType.Shotgun, new WeaponInfo
					{
						AmmoInClip = 6,
						AmmonInWeapon = 40
					});
					break;
				default:
					if (level >= 4 && level < 6)
					{
						Weapons.Add(E_WeaponType.SMG, new WeaponInfo
						{
							AmmoInClip = 30,
							AmmonInWeapon = 250
						});
						Weapons.Add(E_WeaponType.Shotgun, new WeaponInfo
						{
							AmmoInClip = 6,
							AmmonInWeapon = 40
						});
						Weapons.Add(E_WeaponType.GrenadeLauncher, new WeaponInfo
						{
							AmmoInClip = 1,
							AmmonInWeapon = 11
						});
					}
					else
					{
						Weapons.Add(E_WeaponType.SMG, new WeaponInfo
						{
							AmmoInClip = 30,
							AmmonInWeapon = 250
						});
						Weapons.Add(E_WeaponType.Shotgun, new WeaponInfo
						{
							AmmoInClip = 6,
							AmmonInWeapon = 40
						});
						Weapons.Add(E_WeaponType.GrenadeLauncher, new WeaponInfo
						{
							AmmoInClip = 1,
							AmmonInWeapon = 11
						});
						Weapons.Add(E_WeaponType.RocketLauncher, new WeaponInfo
						{
							AmmoInClip = 3,
							AmmonInWeapon = 3
						});
					}
					break;
				}
				WeaponSelected = E_WeaponType.SMG;
			}

			internal void Synchronize(ComponentPlayer player)
			{
				Clear();
				ComponentWeapons weaponComponent = player.Owner.WeaponComponent;
				foreach (KeyValuePair<E_WeaponType, WeaponBase> weapon in weaponComponent.Weapons)
				{
					Weapons.Add(weapon.Key, new WeaponInfo
					{
						AmmoInClip = weapon.Value.ClipAmmo,
						AmmonInWeapon = weapon.Value.WeaponAmmo
					});
				}
				WeaponSelected = weaponComponent.CurrentWeapon;
			}

			internal void Save_Save(ref DictionaryFile df)
			{
				df.SetInt(string.Concat("PlayerDataW", Instance.GameType, "Selected"), (int)WeaponSelected);
				foreach (KeyValuePair<E_WeaponType, WeaponInfo> weapon in Weapons)
				{
					df.SetInt(string.Concat("PlayerDataW", Instance.GameType, weapon.Key, "clip"), weapon.Value.AmmoInClip);
					df.SetInt(string.Concat("PlayerDataW", Instance.GameType, weapon.Key, "ammo"), weapon.Value.AmmonInWeapon);
				}
			}

			internal void Save_Load()
			{
				Clear();
				WeaponSelected = (E_WeaponType)Instance.Save_LoadedFile.GetInt(string.Concat("PlayerDataW", Instance.GameType, "Selected"), 0);
				foreach (int value in Enum.GetValues(typeof(E_WeaponType)))
				{
					if (Instance.Save_LoadedFile.HasKey(string.Concat("PlayerDataW", Instance.GameType, (E_WeaponType)value, "clip")))
					{
						int ammoInClip = Instance.Save_LoadedFile.GetInt(string.Concat("PlayerDataW", Instance.GameType, (E_WeaponType)value, "clip"), 0);
						int num = Instance.Save_LoadedFile.GetInt(string.Concat("PlayerDataW", Instance.GameType, (E_WeaponType)value, "ammo"), 0);
						if (value == 0 && num < 150)
						{
							num = 150;
						}
						Weapons.Add((E_WeaponType)value, new WeaponInfo
						{
							AmmoInClip = ammoInClip,
							AmmonInWeapon = num
						});
					}
				}
			}

			internal void Save_LoadOld()
			{
				Clear();
				WeaponSelected = (E_WeaponType)PlayerPrefs.GetInt(string.Concat("PlayerDataW", Instance.GameType, "Selected"), 0);
				foreach (int value in Enum.GetValues(typeof(E_WeaponType)))
				{
					if (PlayerPrefs.HasKey(string.Concat("PlayerDataW", Instance.GameType, (E_WeaponType)value, "clip")))
					{
						int ammoInClip = PlayerPrefs.GetInt(string.Concat("PlayerDataW", Instance.GameType, (E_WeaponType)value, "clip"), 0);
						int num = PlayerPrefs.GetInt(string.Concat("PlayerDataW", Instance.GameType, (E_WeaponType)value, "ammo"), 0);
						if (value == 0 && num < 150)
						{
							num = 150;
						}
						Weapons.Add((E_WeaponType)value, new WeaponInfo
						{
							AmmoInClip = ammoInClip,
							AmmonInWeapon = num
						});
					}
				}
			}

			internal void Save_Clear()
			{
				PlayerPrefs.DeleteKey(string.Concat("PlayerDataW", Instance.GameType, "Selected"));
				foreach (int value in Enum.GetValues(typeof(E_WeaponType)))
				{
					PlayerPrefs.DeleteKey(string.Concat("PlayerDataW", Instance.GameType, (E_WeaponType)value, "clip"));
					PlayerPrefs.DeleteKey(string.Concat("PlayerDataW", Instance.GameType, (E_WeaponType)value, "ammo"));
				}
				Clear();
			}
		}

		public InventoryInfo Inventory = new InventoryInfo();

		private bool SaveLoaded;

		public void SetDefaults(int level)
		{
			Inventory.SetDefaults(level);
			SaveLoaded = true;
		}

		public void Save_Save(ref DictionaryFile df)
		{
			Inventory.Save_Save(ref df);
		}

		internal void Save_Load()
		{
			Inventory.Save_Load();
			SaveLoaded = true;
		}

		internal void Save_LoadOld()
		{
			if (PlayerPrefs.HasKey(string.Concat("PlayerData", Instance.GameType, "SaveExist")))
			{
				Inventory.Save_LoadOld();
				SaveLoaded = true;
			}
		}

		internal void Save_Clear()
		{
			if (PlayerPrefs.HasKey(string.Concat("PlayerData", Instance.GameType, "SaveExist")))
			{
				PlayerPrefs.DeleteKey(string.Concat("PlayerData", Instance.GameType, "SaveExist"));
				Inventory.Save_Clear();
				SaveLoaded = false;
			}
		}

		internal void Clear()
		{
			SaveLoaded = false;
			Inventory.Clear();
		}

		public bool Save_IsLoaded()
		{
			return SaveLoaded;
		}

		public void SynchronizeDataFromPlayer(ComponentPlayer player)
		{
			Inventory.Synchronize(player);
		}
	}

	private const float checkSleepFrequency = 2f;

	private string _CurrentLevel;

	private int _CurrentGameZone;

	private int _Score;

	private float TimeToClearHits;

	private E_GameState _GameState = E_GameState.Game;

	private E_GameType _GameType = E_GameType.ChapterOnly;

	private static Game _Instance = null;

	public int LeaderBoardID;

	public E_GameDifficulty GameDifficulty = E_GameDifficulty.Normal;

	public PlayerPersistantState PlayerPersistantInfo = new PlayerPersistantState();

	private DictionaryFile _LoadedFile;

	private static float sleepDelay;

	public float LastTouchControlTime;

	private AndroidJavaObject currentConfig;

	private bool m_IsFocused = true;

	private static string[] s_Chapters = new string[14]
	{
		"level_01", "level_02", "level_03", "level_04", "level_05", "level_06", "level_07", "level_08", "level_09", "level_10",
		"level_11_dlc", "level_12_dlc", "level_13_dlc", "level_14_dlc"
	};

	public int Score
	{
		get
		{
			return _Score;
		}
		set
		{
			_Score = value;
		}
	}

	public string CurrentLevel
	{
		get
		{
			return _CurrentLevel;
		}
		set
		{
			_CurrentLevel = value;
		}
	}

	public int CurrentGameZone
	{
		get
		{
			return _CurrentGameZone;
		}
		set
		{
			_CurrentGameZone = value;
		}
	}

	public E_GameState GameState
	{
		get
		{
			return _GameState;
		}
		set
		{
			_GameState = value;
		}
	}

	public E_GameType GameType
	{
		get
		{
			return _GameType;
		}
	}

	public int Dlc { get; private set; }

	public float DontHealTime
	{
		get
		{
			return (GameDifficulty != E_GameDifficulty.Hard) ? 3 : 5;
		}
	}

	public float HealingModificator
	{
		get
		{
			return 10f;
		}
	}

	public static Game Instance
	{
		get
		{
			return _Instance;
		}
	}

	public DictionaryFile Save_LoadedFile
	{
		get
		{
			return _LoadedFile;
		}
	}

	public bool IsXperiaPlay { get; private set; }

	public bool IsAdreno205_Graphics { get; private set; }

	public bool KypadSlided { get; private set; }

	public bool GamepadConnected { get; private set; }

	public bool NVidiaShiledCached { get; private set; }

	public static string MainMenuLevelName
	{
		get
		{
			return "MainMenuBackground";
		}
	}

	private string GetChapter(int levelIndex)
	{
		return s_Chapters[levelIndex];
	}

	private int GetChapterId(string levelName)
	{
		for (int i = 0; i < s_Chapters.Length; i++)
		{
			if (s_Chapters[i] == levelName)
			{
				return i + 1;
			}
		}
		return -1;
	}

	public static bool DetectXperiaPlayModel()
	{
		return (SystemInfo.deviceModel.Contains("Sony Ericsson") && SystemInfo.deviceModel.Contains("R800")) || SystemInfo.deviceModel.Contains("Z1i");
	}

	public static bool DetectAdreno205()
	{
		return SystemInfo.graphicsDeviceName.Contains("Adreno 205") || (SystemInfo.graphicsDeviceName.Contains("Adreno") && SystemInfo.graphicsDeviceName.Contains("205"));
	}

	public static bool IsSmallDevice()
	{
		return Mathf.Max(Screen.width, Screen.height) < 1024;
	}

	public static void SetupQualityByDevice()
	{
		bool flag = DetectAdreno205();
		bool flag2 = DetectXperiaPlayModel();
		bool flag3 = IsSmallDevice();
		if (flag2 || flag3 || flag)
		{
			QualitySettings.masterTextureLimit = 1;
		}
		int num = 0;
	}

	private void Awake()
	{
		IsXperiaPlay = DetectXperiaPlayModel();
		IsAdreno205_Graphics = DetectAdreno205();
		//SetupQualityByDevice();
		CamExplosionFXMgr.PreloadResources();
		if ((bool)_Instance)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		_Instance = this;
		UnityEngine.Object.DontDestroyOnLoad(this);
		base.transform.parent = null;
		Application.targetFrameRate = 30;
		FlurryAndroid.enableAppCircle("com.madfingergames.Shadowgun");
		FlurryAndroid.onStartSession("83CRZ8YKRT1E1RCH6TYV");
		Screen.autorotateToPortrait = false;
		Screen.autorotateToPortraitUpsideDown = false;
		MogaGamepad.Init();
		MogaGamepad.OnConnectionChange = OnMogaConnectionChange;
		MogaGamepad.OnBatteryLowChange = OnMogaBatteryLowChange;
		if (m_IsFocused)
		{
			MogaGamepad.OnFocus(m_IsFocused);
		}
		if (custom_inputs.IsNVidiaShield())
		{
			NVidiaShiledCached = true;
		}
		else
		{
			NVidiaShiledCached = false;
		}
	}

	private void OnDestroy()
	{
		if (_Instance == this)
		{
			MogaGamepad.Done();
			MogaGamepad.OnConnectionChange = null;
		}
	}

	public void OnMogaConnectionChange(bool connected)
	{
		if (GuiMogaPopup.Instance != null)
		{
			GuiMogaPopup.Instance.Show((!connected) ? 98031 : 98030, 2f);
		}
		else
		{
			Debug.Log("GuiMogaPopup instance is not initialised");
		}
	}

	public void OnMogaBatteryLowChange(bool batteryLow)
	{
		if (GuiMogaPopup.Instance != null)
		{
			if (batteryLow)
			{
				GuiMogaPopup.Instance.Show(98032, 5f);
			}
			else
			{
				GuiMogaPopup.Instance.Hide();
			}
		}
	}

	private void LogMemory()
	{
		Debug.Log("memory " + GC.GetTotalMemory(false));
	}

	public void Start()
	{
		if (IsXperiaPlay)
		{
			InitAndroidConfigLink();
			StartCoroutine(CheckForPhoneSlidedStatus());
		}
		if (!IsSmallDevice())
		{
			StartCoroutine(CheckGamepadConnected());
		}
		InvokeRepeating("CheckSleepTimeout", 0f, 2f);
	}

	public static bool IsHDResolution()
	{
		int num = Mathf.Max(Screen.width, Screen.height);
		int num2 = Mathf.Min(Screen.width, Screen.height);
		if (num > 960 || num2 > 640)
		{
			return true;
		}
		return false;
	}

	public string GetSaveFileName()
	{
		return _GetSaveFileName(_GameType, Dlc);
	}

	private string _GetSaveFileName(E_GameType gt, int dlc)
	{
		// SavePath.Directory is persistentDataPath everywhere except PS Vita, where
		// persistentDataPath is unusable and saves live in ux0:data/ShadowGun instead.
		// Touching the property also creates the folder, so the write in GameZone.Save
		// has somewhere to land.
		return string.Concat(SavePath.Directory, "/SaveGame", gt, dlc);
	}

	public void Save_Save(ref DictionaryFile df)
	{
		df.SetString(string.Concat(GameType, "Level"), Application.loadedLevelName);
		df.SetInt(string.Concat(GameType, "GameZone"), Mission.Instance ? Mission.Instance.GameZoneIndex : 0);
		df.SetInt(string.Concat(GameType, "Difficulty"), (int)GameDifficulty);
		PlayerPersistantInfo.SynchronizeDataFromPlayer(Player.Instance);
		PlayerPersistantInfo.Save_Save(ref df);
	}

	public void Save_Clear()
	{
		if (Dlc == 0 && PlayerPrefs.GetInt(string.Concat(GameType, "SaveExist"), 0) == 1)
		{
			PlayerPrefs.DeleteKey(string.Concat(GameType, "SaveExist"));
			PlayerPrefs.DeleteKey(string.Concat(GameType, "Level"));
			PlayerPrefs.DeleteKey(string.Concat(GameType, "GameZone"));
			PlayerPrefs.DeleteKey(string.Concat(GameType, "Difficulty"));
			for (int i = 0; i < 100; i++)
			{
				if (PlayerPrefs.HasKey(string.Concat(Instance.GameType, "GB", i)))
				{
					PlayerPrefs.DeleteKey(string.Concat(Instance.GameType, "GB", i));
				}
			}
			PlayerPersistantInfo.Save_Clear();
		}
		if (SaveFileExists(GameType, Dlc))
		{
			File.Delete(GetSaveFileName());
		}
		PlayerPersistantInfo.Clear();
	}

	public void Save_Load()
	{
		if (SaveFileExists(_GameType, Dlc))
		{
			_LoadedFile = new DictionaryFile();
			_LoadedFile.Load(GetSaveFileName());
			CurrentLevel = Save_LoadedFile.GetString(string.Concat(GameType, "Level"), "level_01");
			CurrentGameZone = Save_LoadedFile.GetInt(string.Concat(GameType, "GameZone"), 0);
			GameDifficulty = (E_GameDifficulty)Save_LoadedFile.GetInt(string.Concat(GameType, "Difficulty"), 0);
			PlayerPersistantInfo.Save_Load();
		}
		else
		{
			Save_LoadOld();
		}
	}

	public void Save_LoadOld()
	{
		CurrentLevel = PlayerPrefs.GetString(string.Concat(GameType, "Level"), "level_01");
		CurrentGameZone = PlayerPrefs.GetInt(string.Concat(GameType, "GameZone"), 0);
		GameDifficulty = (E_GameDifficulty)PlayerPrefs.GetInt(string.Concat(GameType, "Difficulty"), 0);
		PlayerPersistantInfo.Save_LoadOld();
	}

	public void UnlockMission(string levelName)
	{
		int chapterId = GetChapterId(levelName);
		UnlockMission(chapterId);
	}

	public void UnlockMission(int count)
	{
		if (count > GetUnlockedMission(Dlc))
		{
			if (Dlc == 0)
			{
				PlayerPrefs.SetInt("MissionsUnlocked", count);
			}
			else
			{
				PlayerPrefs.SetInt("MissionsUnlocked" + Dlc, count - 10);
			}
			PlayerPrefs.Save();
		}
	}

	public int GetUnlockedMission(int inDlc)
	{
		int num = 0;
		num = ((inDlc != 0) ? PlayerPrefs.GetInt("MissionsUnlocked" + inDlc, 0) : PlayerPrefs.GetInt("MissionsUnlocked", 0));
		E_GameType e_GameType = E_GameType.SinglePlayer;
		string text = PlayerPrefs.GetString(string.Concat(e_GameType, "Level"), string.Empty);
		if (text != string.Empty)
		{
			int num2 = GetChapterId(text) - 1;
			if (num2 > num)
			{
				num = num2;
			}
		}
		return num;
	}

	public bool IsResumePossible(E_GameType gameType, int dlc)
	{
		if (dlc == 0 && PlayerPrefs.GetInt(string.Concat(gameType, "SaveExist"), 0) == 1)
		{
			return true;
		}
		return SaveFileExists(gameType, dlc);
	}

	private bool SaveFileExists(E_GameType gameType, int dlc)
	{
		return File.Exists(_GetSaveFileName(gameType, dlc));
	}

	public void LoadMainMenu()
	{
		if (GameState != E_GameState.MainMenu)
		{
			Time.timeScale = 1f;
			StartCoroutine(LoadScene(MainMenuLevelName));
		}
	}

	public void StartNewGame(E_GameDifficulty difficulty, int inDlc)
	{
		_GameType = E_GameType.SinglePlayer;
		Dlc = inDlc;
		Save_Clear();
		Save_Load();
		GameDifficulty = difficulty;
		if (Dlc == 0)
		{
			GuiSniffMinigame.ResetFirstTimeTutorial();
		}
		string[] array = new string[2] { "level_01", "level_11_dlc" };
		CurrentLevel = array[Dlc];
		StartCoroutine(LoadScene(CurrentLevel));
		PlayerPersistantInfo.SetDefaults(0);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Difficulty", Instance.GameDifficulty.ToString());
	}

	public void ResumeSinglePlayer(int inDlc)
	{
		_GameType = E_GameType.SinglePlayer;
		Dlc = inDlc;
		Save_Load();
		StartCoroutine(LoadScene(CurrentLevel));
	}

	public void StartChapterMode(int index, E_GameDifficulty difficulty)
	{
		_GameType = E_GameType.ChapterOnly;
		Save_Clear();
		Save_Load();
		GameDifficulty = difficulty;
		CurrentLevel = GetChapter(index);
		PlayerPersistantInfo.SetDefaults(index);
		StartCoroutine(LoadScene(CurrentLevel));
	}

	public void StartTutorial()
	{
		_GameType = E_GameType.Tutorial;
		_GameState = E_GameState.Tutorial;
		CurrentLevel = "tutorial";
		StartCoroutine(LoadScene(CurrentLevel));
	}

	public void StartSaleScreens()
	{
		FlurryAndroid.openCatalog("none");
	}

	public void LoadNextLevel(string nextLevel, int currentZone)
	{
		PlayerPersistantInfo.SynchronizeDataFromPlayer(Player.Instance);
		CurrentLevel = nextLevel;
		CurrentGameZone = currentZone;
		StartCoroutine(LoadScene(CurrentLevel));
	}

	private IEnumerator LoadScene(string scene)
	{
		PauseScreenTimeout(120f);
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		ClearInstances();
		Application.LoadLevel("empty");
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		ClearInstances();
		GC.Collect(100);
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return Resources.UnloadUnusedAssets();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		Application.LoadLevel(scene);
		GC.Collect(100);
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return Resources.UnloadUnusedAssets();
		GC.Collect(100);
	}

	public void TryToCleanSomeMemory()
	{
		GC.Collect(100);
		Resources.UnloadUnusedAssets();
	}

	private void ClearInstances()
	{
		AgentActionFactory.Clear();
		FactsFactory.Clear();
		if ((bool)Player.Instance)
		{
			Player.Instance.StopAllCoroutines();
			Player.Instance.CancelInvoke();
			Player.Instance = null;
		}
		if ((bool)SpriteEffectsManager.Instance)
		{
			SpriteEffectsManager.Instance.StopAllCoroutines();
			SpriteEffectsManager.Instance.CancelInvoke();
			SpriteEffectsManager.Instance = null;
		}
		if ((bool)GameCamera.Instance)
		{
			GameCamera.Instance.StopAllCoroutines();
			GameCamera.Instance.CancelInvoke();
			GameCamera.Instance = null;
		}
		if ((bool)CombatEffectsManager.Instance)
		{
			CombatEffectsManager.Instance.StopAllCoroutines();
			CombatEffectsManager.Instance.CancelInvoke();
			CombatEffectsManager.Instance = null;
		}
		if ((bool)ProjectileManager.Instance)
		{
			ProjectileManager.Instance.StopAllCoroutines();
			ProjectileManager.Instance.CancelInvoke();
			ProjectileManager.Instance = null;
		}
		if ((bool)Mission.Instance)
		{
			Mission.Instance.StopAllCoroutines();
			Mission.Instance.CancelInvoke();
			Mission.Instance = null;
		}
		if ((bool)MusicManager.Instance)
		{
			MusicManager.Instance.StopAllCoroutines();
			MusicManager.Instance.CancelInvoke();
			MusicManager.Instance = null;
		}
		if ((bool)BloodFXManager.Instance)
		{
			BloodFXManager.Instance.StopAllCoroutines();
			BloodFXManager.Instance.CancelInvoke();
			BloodFXManager.Instance = null;
		}
		if ((bool)CamExplosionFXMgr.Instance)
		{
			CamExplosionFXMgr.Instance.StopAllCoroutines();
			CamExplosionFXMgr.Instance.CancelInvoke();
			CamExplosionFXMgr.Instance = null;
		}
		if ((bool)GameBlackboard.Instance)
		{
			GameBlackboard.Instance.StopAllCoroutines();
			GameBlackboard.Instance.CancelInvoke();
			GameBlackboard.Instance = null;
		}
		if ((bool)WeaponManager.Instance)
		{
			WeaponManager.Instance.StopAllCoroutines();
			WeaponManager.Instance.CancelInvoke();
			WeaponManager.Instance = null;
		}
		if ((bool)GuiCustomizeControls.Instance)
		{
			GuiCustomizeControls.Instance.StopAllCoroutines();
			GuiCustomizeControls.Instance.CancelInvoke();
			GuiCustomizeControls.Instance = null;
		}
		if ((bool)GuiGameEnd.Instance)
		{
			GuiGameEnd.Instance.StopAllCoroutines();
			GuiGameEnd.Instance.CancelInvoke();
			GuiGameEnd.Instance = null;
		}
		if ((bool)GuiHUD.Instance)
		{
			GuiHUD.Instance.StopAllCoroutines();
			GuiHUD.Instance.CancelInvoke();
			GuiHUD.Instance = null;
		}
		if ((bool)GuiIngameMenu.Instance)
		{
			GuiIngameMenu.Instance.StopAllCoroutines();
			GuiIngameMenu.Instance.CancelInvoke();
			GuiIngameMenu.Instance = null;
		}
		if ((bool)GuiMainMenu.Instance)
		{
			GuiMainMenu.Instance.StopAllCoroutines();
			GuiMainMenu.Instance.CancelInvoke();
			GuiMainMenu.Instance = null;
		}
		if ((bool)GuiOptionsMenu.Instance)
		{
			GuiOptionsMenu.Instance.StopAllCoroutines();
			GuiOptionsMenu.Instance.CancelInvoke();
			GuiOptionsMenu.Instance = null;
		}
		if ((bool)GuiSniffMinigame.Instance)
		{
			GuiSniffMinigame.Instance.StopAllCoroutines();
			GuiSniffMinigame.Instance.CancelInvoke();
			GuiSniffMinigame.Instance = null;
		}
		if ((bool)GuiSubtitlesRenderer.Instance)
		{
			GuiSubtitlesRenderer.Instance.StopAllCoroutines();
			GuiSubtitlesRenderer.Instance.CancelInvoke();
			GuiSubtitlesRenderer.Instance = null;
		}
		if ((bool)MFGuiManager.Instance)
		{
			MFGuiManager.Instance.StopAllCoroutines();
			MFGuiManager.Instance.CancelInvoke();
			MFGuiManager.Instance = null;
		}
	}

	private void Update()
	{
		if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft && Screen.orientation != ScreenOrientation.LandscapeLeft)
		{
			Screen.orientation = ScreenOrientation.LandscapeLeft;
		}
		else if (Input.deviceOrientation == DeviceOrientation.LandscapeRight && Screen.orientation != ScreenOrientation.LandscapeRight)
		{
			Screen.orientation = ScreenOrientation.LandscapeRight;
		}
		MogaGamepad.Update();
	}

	private void OnApplicationQuit()
	{
		ClearInstances();
		FlurryAndroid.onEndSession();
	}

	private void OnApplicationFocus(bool focus)
	{
		m_IsFocused = focus;
		MogaGamepad.OnFocus(focus);
	}

	private void InitAndroidConfigLink()
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			if (androidJavaClass == null)
			{
				Debug.Log("player not found");
				return;
			}
			AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			if (androidJavaObject == null)
			{
				Debug.Log("activity not found");
				return;
			}
			currentConfig = androidJavaObject.Call<AndroidJavaObject>("getResources", new object[0]).Call<AndroidJavaObject>("getConfiguration", new object[0]);
			if (currentConfig == null)
			{
				Debug.Log("config not found");
			}
		}
	}

	private IEnumerator CheckForPhoneSlidedStatus()
	{
		while (true)
		{
			if (currentConfig != null)
			{
				int nav = 0;
				if (SystemInfo.deviceModel.Contains("R800") || SystemInfo.deviceModel.Contains("Z1i"))
				{
					nav = currentConfig.Get<int>("navigationHidden");
				}
				if (nav == 2 || nav == 0)
				{
					if (KypadSlided)
					{
						KypadSlided = false;
					}
				}
				else if (!KypadSlided)
				{
					KypadSlided = true;
				}
			}
			else
			{
				KypadSlided = false;
			}
			yield return new WaitForSeconds(0.5f);
		}
	}

	private IEnumerator CheckGamepadConnected()
	{
		while (true)
		{
			bool newConnection = DetectGpadConnection();
			if (GamepadConnected != newConnection)
			{
				GamepadConnected = newConnection;
			}
			yield return new WaitForSeconds(2.5f);
		}
	}

	private void CheckSleepTimeout()
	{
		if (sleepDelay > 0f)
		{
			sleepDelay -= 2f;
			if (sleepDelay <= 0f)
			{
				EnableScreenTimeout();
			}
		}
	}

	public static void PauseScreenTimeout(float pauseTime)
	{
		if (pauseTime > 0f && Screen.sleepTimeout > 0)
		{
			if (sleepDelay < pauseTime)
			{
				sleepDelay = pauseTime;
			}
			DisableScreenTimeout();
		}
	}

	private static void EnableScreenTimeout()
	{
		Screen.sleepTimeout = 600;
	}

	private static void DisableScreenTimeout()
	{
		Screen.sleepTimeout = -1;
	}

	public static bool DetectGpadConnection()
	{
		string[] joystickNames = Input.GetJoystickNames();
		return joystickNames != null && joystickNames.Length > 0;
	}

	public static string CurrentJoystickName()
	{
		string[] joystickNames = Input.GetJoystickNames();
		if (joystickNames != null && joystickNames.Length > 0)
		{
			if (joystickNames.Length > 1)
			{
				Debug.LogWarning("More then 1 gamepad connected, getting name of first.");
			}
			return joystickNames[0];
		}
		return null;
	}
}
