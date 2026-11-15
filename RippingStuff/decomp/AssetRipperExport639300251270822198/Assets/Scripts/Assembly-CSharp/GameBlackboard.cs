using System;
using UnityEngine;

[Serializable]
[ExecuteInEditMode]
public class GameBlackboard : MonoBehaviour
{
	[SerializeField]
	private GameEvents _GameEvents = new GameEvents();

	public GameEvents GameEvents
	{
		get
		{
			return _GameEvents;
		}
	}

	public int NumberOfGameEvents
	{
		get
		{
			return GameEvents.Count;
		}
	}

	public static GameBlackboard Instance { get; set; }

	private void Awake()
	{
		if (Application.isPlaying && (bool)Instance)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		Instance = this;
		GameEvents.Add("RESET", GameEvents.E_State.False);
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void Save_Save(ref DictionaryFile df)
	{
		_GameEvents.Save_Save(ref df);
	}

	public void Save_Load()
	{
		_GameEvents.Save_Load();
	}

	public void Save_LoadOld()
	{
		_GameEvents.Save_LoadOld();
	}
}
