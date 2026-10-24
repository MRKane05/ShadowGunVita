using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Game/Load Next Level")]
public class LoadNextLevel : MonoBehaviour
{
	public string NextLevel;

	public bool LastMission;

	public List<OnGameEvent> OnGameEvents = new List<OnGameEvent>();

	private void Start()
	{
		foreach (OnGameEvent onGameEvent in OnGameEvents)
		{
			GameBlackboard.Instance.GameEvents.AddEventChangeHandler(onGameEvent.Name, EventHandler);
		}
	}

	public void EventHandler(string name, GameEvents.E_State state)
	{
		foreach (OnGameEvent onGameEvent in OnGameEvents)
		{
			if (GameBlackboard.Instance.GameEvents.GetState(onGameEvent.Name) != onGameEvent.State)
			{
				return;
			}
		}
		Game.Instance.UnlockMission(Application.loadedLevelName);
		Player.Instance.StopMove(true);
		LoadLevel();
	}

	private void LoadLevel()
	{
		if (LastMission && Game.Instance.GameType == E_GameType.SinglePlayer)
		{
			Game.Instance.Save_Clear();
		}
		Mission.Instance.EndOfMissionSuccess(OnMissionSuccessTap, LastMission);
	}

	private void OnMissionSuccessTap()
	{
		if (Game.Instance.GameType == E_GameType.ChapterOnly || LastMission)
		{
			Game.Instance.LoadMainMenu();
		}
		else
		{
			Game.Instance.LoadNextLevel(NextLevel, 0);
		}
	}
}
