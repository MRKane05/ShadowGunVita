using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(SpriteEffectsManager))]
public class Mission : MonoBehaviour
{
	public enum E_GuiState
	{
		E_GS_NONE = 0,
		E_GS_HUD = 1,
		E_GS_INGAME_MENU = 2,
		E_GS_SNIFF_MINIGAME = 3
	}

	public List<GameZoneBaze> GameZones = new List<GameZoneBaze>();

	public List<GameObject> ManagedGameObject = new List<GameObject>();

	public List<AgentsCache> AgentsCache = new List<AgentsCache>();

	public ExplosionCache ExplosionCache = new ExplosionCache();

	public int AchievementID;

	public int LeaderBoardID;

	[NonSerialized]
	public GameZoneBaze CurrentGameZone;

	public static Mission Instance;

	private E_GuiState m_CurrentGuiState;

	public int GameZoneIndex
	{
		get
		{
			for (int i = 0; i < GameZones.Count; i++)
			{
				if (GameZones[i] == CurrentGameZone)
				{
					return i;
				}
			}
			Debug.LogError("Unknow game zone");
			return 0;
		}
	}

	private void Awake()
	{
		Instance = this;
		Random.seed = DateTime.Now.Second;
	}

	private void Start()
	{
		if (Game.Instance.GameType == E_GameType.SinglePlayer && Game.Instance.CurrentGameZone != 0)
		{
			Save_Load();
		}
		MFGuiManager.Instance.SetFadeOut(1f);
		AudioListener.pause = true;
		AudioListener.volume = 0f;
		Game.Instance.GameState = E_GameState.Game;
		Game.Instance.CurrentLevel = Application.loadedLevelName;
		Player.Instance.Owner.GameObject.SetActiveRecursively(false);
		ExplosionCache.Init();
		for (int i = 0; i < AgentsCache.Count; i++)
		{
			AgentsCache[i].Init();
		}
		for (int j = 0; j < GameZones.Count; j++)
		{
			if (j == Game.Instance.CurrentGameZone)
			{
				CurrentGameZone = GameZones[j];
			}
			else
			{
				GameZones[j].Disable();
			}
		}
		Game.Instance.LeaderBoardID = LeaderBoardID;
		LoadGui();
		Shader.WarmupAllShaders();
		Invoke("WaitForLogGui", 0.1f);
	}

	private void OnDestroy()
	{
		if (Game.Instance.GameType == E_GameType.ChapterOnly)
		{
			Game.Instance.Save_Clear();
		}
		foreach (AgentsCache item in AgentsCache)
		{
			item.Clear();
		}
		ExplosionCache.Clear();
		ExplosionCache = null;
	}

	private void WaitForLogGui()
	{
		GUIBase_Platform gUIBase_Platform = MFGuiManager.Instance.FindPlatform("Gui_16_9");
		if ((bool)gUIBase_Platform && gUIBase_Platform.IsInitialized())
		{
			StartCoroutine(PrepareForStart());
		}
		else
		{
			Invoke("WaitForLogGui", 0.1f);
		}
	}

	private IEnumerator PrepareForStart()
	{
		StartGui();
		Player.Instance.Owner.GameObject.SetActiveRecursively(true);
		Player.Instance.Owner.SendMessage("Activate", CurrentGameZone.PlayerSpawn);
		Player.Instance.Owner.WeaponComponent.Initialize();
		CurrentGameZone.Enable();
		GameBlackboard.Instance.GameEvents.Update("RESET", GameEvents.E_State.True);
		GameBlackboard.Instance.GameEvents.Update("RESET", GameEvents.E_State.False);
		StartCoroutine(FadeInAudio_Corout(1f, GuiOptions.ListenerVolume));
		yield return new WaitForSeconds(1f);
		MFGuiManager.Instance.FadeIn(2f);
		if ((bool)GuiHUD.Instance)
		{
			GuiHUD.Instance.ShowMessageTimed(GuiHUD.E_HudMessageType.E_001_GameLoaded, GuiHUD.InfoMessageTime);
		}
	}

	private IEnumerator FadeInAudio_Corout(float fadeTime, float targetVolume)
	{
		float beginFadeTime = Time.realtimeSinceStartup;
		AudioListener.volume = 0f;
		AudioListener.pause = false;
		while (Time.realtimeSinceStartup - beginFadeTime <= fadeTime)
		{
			float volume = targetVolume * Mathf.Max((Time.realtimeSinceStartup - beginFadeTime) / fadeTime, 1f);
			AudioListener.volume = volume;
			yield return new WaitForEndOfFrame();
		}
		AudioListener.volume = targetVolume;
	}

	public void Save_Save(ref DictionaryFile df)
	{
		GameBlackboard.Instance.Save_Save(ref df);
	}

	public void Save_Load()
	{
		if (Game.Instance.Save_LoadedFile != null)
		{
			GameBlackboard.Instance.Save_Load();
		}
		else
		{
			GameBlackboard.Instance.Save_LoadOld();
		}
	}

	private void LoadGui()
	{
		if (Game.IsHDResolution())
		{
			Application.LoadLevelAdditive("Gui_16_9_HD");
		}
		else
		{
			Application.LoadLevelAdditive("Gui_16_9");
		}
	}

	private void StartGui()
	{
		if (!GuiHUD.Instance)
		{
			Debug.LogError("Con't find GuiHUD script.");
			return;
		}
		if (!GuiIngameMenu.Instance)
		{
			Debug.LogError("Con't find GuiIngameMenu script.");
			return;
		}
		GuiHUD.Instance.Init(this);
		GuiIngameMenu.Instance.Init(this);
		ChangeGuiState(E_GuiState.E_GS_HUD);
		Player.Instance.OnGuiPrepared();
	}

	public void ChangeGuiState(E_GuiState newState)
	{
		switch (m_CurrentGuiState)
		{
		case E_GuiState.E_GS_HUD:
			GuiHUD.Instance.Hide();
			break;
		case E_GuiState.E_GS_INGAME_MENU:
			GuiIngameMenu.Instance.Hide();
			break;
		case E_GuiState.E_GS_SNIFF_MINIGAME:
			GuiSniffMinigame.Instance.SuspendAll();
			break;
		default:
			Debug.LogError("Unsupported case");
			break;
		case E_GuiState.E_GS_NONE:
			break;
		}
		m_CurrentGuiState = newState;
		switch (m_CurrentGuiState)
		{
		case E_GuiState.E_GS_NONE:
			break;
		case E_GuiState.E_GS_HUD:
			GuiHUD.Instance.Show();
			break;
		case E_GuiState.E_GS_INGAME_MENU:
			GuiIngameMenu.Instance.Show();
			break;
		case E_GuiState.E_GS_SNIFF_MINIGAME:
			GuiSniffMinigame.Instance.ResumeAll();
			break;
		default:
			Debug.LogError("Unsupported case");
			break;
		}
	}

	public void EndOfMissionSuccess(GUIBase_Button.TouchDelegate touchDelegate, bool finalLevel)
	{
		StopAllCoroutines();
		MusicManager.Instance.FadeOutMusic(1f);
		GuiHUD.Instance.ShowMissionSuccess(touchDelegate, finalLevel);
	}

	public void EndOfMissionFail(GuiHUD.E_HudMessageType failMsg)
	{
		GuiSubtitles.DeactivateAllRuning();
		GuiHUD.Instance.Hide();
		if (failMsg != GuiHUD.E_HudMessageType.E_NONE)
		{
			GuiHUD.Instance.ShowMessage(failMsg, true);
		}
		StopAllCoroutines();
		StartCoroutine(LoadLastSave(3f));
	}

	public void RestartCheckpoint()
	{
		GuiSubtitles.DeactivateAllRuning();
		StopAllCoroutines();
		StartCoroutine(LoadLastSave(0f));
	}

	private IEnumerator LoadLastSave(float delay)
	{
		MusicManager.Instance.FadeOutMusic(2f);
		yield return new WaitForSeconds(delay);
		GuiHUD.Instance.Hide();
		yield return new WaitForEndOfFrame();
		MFGuiManager.Instance.FadeOut();
		yield return new WaitForSeconds(MFGuiManager.Instance.FadeRemainingTime);
		GuiHUD.Instance.HideAllMessages();
		Player.Instance.Owner.SendMessage("Deactivate");
		Player.Instance.Owner.Transform.position = new Vector3(0f, -1000f, 0f);
		yield return new WaitForSeconds(0.1f);
		ExplosionCache.Reset();
		CurrentGameZone.Reset();
		CurrentGameZone.Disable();
		ProjectileManager.Instance.Reset();
		if (CamExplosionFXMgr.Instance != null)
		{
			CamExplosionFXMgr.Instance.Reset();
		}
		for (int i = 0; i < ManagedGameObject.Count; i++)
		{
			ManagedGameObject[i].SetActiveRecursively(true);
		}
		SpriteEffectsManager.Instance.ReleaseBloodSprites();
		Game.Instance.Save_Load();
		Save_Load();
		yield return new WaitForEndOfFrame();
		CurrentGameZone = GameZones[Game.Instance.CurrentGameZone];
		Player.Instance.Owner.SendMessage("Activate", CurrentGameZone.PlayerSpawn);
		Player.Instance.Owner.WeaponComponent.Initialize();
		CurrentGameZone.Enable();
		GameBlackboard.Instance.GameEvents.Update("RESET", GameEvents.E_State.True);
		GuiHUD.Instance.Reset();
		GuiSniffMinigame.Instance.Hide();
		yield return new WaitForSeconds(0.1f);
		GameBlackboard.Instance.GameEvents.Update("RESET", GameEvents.E_State.False);
		yield return new WaitForSeconds(1f);
		MFGuiManager.Instance.FadeIn(2f);
		MusicManager.Instance.PlayDefaultMusic();
		yield return new WaitForSeconds(0.1f);
		GuiHUD.Instance.Show();
		GuiHUD.Instance.ShowMessageTimed(GuiHUD.E_HudMessageType.E_001_GameLoaded, GuiHUD.InfoMessageTime);
	}

	public GameObject GetAgendFromCache(E_EnemyType Type, Transform t)
	{
		int count = AgentsCache.Count;
		for (int i = 0; i < count; i++)
		{
			if (AgentsCache[i].EnemyType == Type)
			{
				return AgentsCache[i].Get();
			}
		}
		return null;
	}

	public void ReturnAgent(GameObject enemy)
	{
		enemy.SendMessage("Deactivate");
		int count = AgentsCache.Count;
		for (int i = 0; i < count && !AgentsCache[i].Return(enemy); i++)
		{
		}
	}

	public void UnlockNextGameZone()
	{
		int gameZoneIndex = GameZoneIndex;
		if (gameZoneIndex + 1 < GameZones.Count)
		{
			GameZones[gameZoneIndex + 1].Enable();
		}
	}

	public void LockPrevGameZone()
	{
		int gameZoneIndex = GameZoneIndex;
		if (gameZoneIndex > 0)
		{
			GameZones[gameZoneIndex - 1].Disable();
		}
	}

	private void OnApplicationFocus(bool focus)
	{
		if (GuiHUD.Instance != null && !GuiHUD.Instance.m_MissionSuccessScreenOn && Game.Instance.GameState != E_GameState.IngameMenu)
		{
			GuiIngameMenu.Instance.DisableRestartButton(true);
			GuiHUD.Instance.SwitchToIngameMenu();
		}
		AudioListener.pause = true;
	}

	private void OnApplicationPause(bool focus)
	{
		if (GuiHUD.Instance != null && !GuiHUD.Instance.m_MissionSuccessScreenOn && Game.Instance.GameState != E_GameState.IngameMenu)
		{
			GuiIngameMenu.Instance.DisableRestartButton(true);
			GuiHUD.Instance.SwitchToIngameMenu();
		}
		AudioListener.pause = true;
	}

	public void SendGameEvent(GameEvent inGameEvent)
	{
		StartCoroutine(SendGameEvent_Corutine(inGameEvent.Name, inGameEvent.State, inGameEvent.Delay));
	}

	public void SendGameEvent(string inEventName, GameEvents.E_State inState, float inDelay)
	{
		StartCoroutine(SendGameEvent_Corutine(inEventName, inState, inDelay));
	}

	private IEnumerator SendGameEvent_Corutine(string inEventName, GameEvents.E_State inState, float inDelay)
	{
		yield return new WaitForSeconds(inDelay);
		GameBlackboard.Instance.GameEvents.Update(inEventName, inState);
	}
}
