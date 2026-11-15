using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("GUI/Game/GuiSubtitles")]
[RequireComponent(typeof(AudioSource))]
public class GuiSubtitles : MonoBehaviour
{
	[Serializable]
	public class SubtitleLineEx
	{
		public int TextID;

		public float Time;
	}

	[Serializable]
	public class GameEvent
	{
		public string Name;

		public GameEvents.E_State State;

		public GameEvent(string name, GameEvents.E_State state)
		{
			Name = name;
			State = state;
		}
	}

	public AudioClip Voice;

	public SubtitleLineEx[] SequenceEx = new SubtitleLineEx[0];

	public bool ForceWalkOnPlayer;

	public bool ForceShow;

	public List<GameEvent> GameEvents = new List<GameEvent>();

	public bool hasAnyText
	{
		get
		{
			return SequenceEx.Length > 0;
		}
	}

	public static void DeactivateAllRuning()
	{
		GuiSubtitlesRenderer.Deactivate();
	}

	public static void ShowAllRunning(bool show)
	{
		GuiSubtitlesRenderer.ShowAllRunning(show);
	}

	private void Start()
	{
		InitializeEvents();
		base.enabled = false;
	}

	private void InitializeEvents()
	{
		foreach (GameEvent gameEvent in GameEvents)
		{
			GameBlackboard.Instance.GameEvents.AddEventChangeHandler(gameEvent.Name, EventHandler);
		}
	}

	private void Activate()
	{
		GuiSubtitlesRenderer.ShowSubtitles(this);
	}

	public void EventHandler(string name, GameEvents.E_State state)
	{
		foreach (GameEvent gameEvent in GameEvents)
		{
			if (GameBlackboard.Instance.GameEvents.GetState(gameEvent.Name) != gameEvent.State)
			{
				return;
			}
		}
		Activate();
	}
}
