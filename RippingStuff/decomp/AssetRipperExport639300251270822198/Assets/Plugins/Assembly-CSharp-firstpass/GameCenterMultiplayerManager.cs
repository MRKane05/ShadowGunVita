using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameCenterMultiplayerManager : MonoBehaviour
{
	public delegate void GameCenterMultiplayerErrorEventHandler(string error);

	public delegate void GameCenterMultiplayerSimpleEventHandler(string data);

	public delegate void GameCenterMultiplayerIntEventHandler(int count);

	public delegate void GameCenterMultiplayerEventHandler();

	public delegate void ScoresLoadedEventHandler(List<GameCenterScore> scores);

	[method: MethodImpl(32)]
	public static event GameCenterMultiplayerEventHandler matchmakerCancelled;

	[method: MethodImpl(32)]
	public static event GameCenterMultiplayerIntEventHandler matchmakerFoundMatch;

	[method: MethodImpl(32)]
	public static event GameCenterMultiplayerEventHandler friendRequestControllerFinished;

	[method: MethodImpl(32)]
	public static event GameCenterMultiplayerSimpleEventHandler playerConnected;

	[method: MethodImpl(32)]
	public static event GameCenterMultiplayerSimpleEventHandler playerDisconnected;

	[method: MethodImpl(32)]
	public static event GameCenterMultiplayerErrorEventHandler playerConnectionFailed;

	[method: MethodImpl(32)]
	public static event GameCenterMultiplayerEventHandler inviteRequestWasReceived;

	[method: MethodImpl(32)]
	public static event GameCenterMultiplayerSimpleEventHandler playerBeganSpeaking;

	[method: MethodImpl(32)]
	public static event GameCenterMultiplayerSimpleEventHandler playerStoppedSpeaking;

	[method: MethodImpl(32)]
	public static event GameCenterMultiplayerErrorEventHandler findMatchFailed;

	[method: MethodImpl(32)]
	public static event GameCenterMultiplayerIntEventHandler findMatchFinished;

	[method: MethodImpl(32)]
	public static event GameCenterMultiplayerErrorEventHandler addPlayersToMatchFailed;

	[method: MethodImpl(32)]
	public static event GameCenterMultiplayerErrorEventHandler findActivityFailed;

	[method: MethodImpl(32)]
	public static event GameCenterMultiplayerIntEventHandler findActivityFinished;

	[method: MethodImpl(32)]
	public static event GameCenterMultiplayerErrorEventHandler findActivityForGroupFailed;

	[method: MethodImpl(32)]
	public static event GameCenterMultiplayerIntEventHandler findActivityForGroupFinished;

	[method: MethodImpl(32)]
	public static event GameCenterMultiplayerErrorEventHandler retrieveMatchesBestScoresFailed;

	[method: MethodImpl(32)]
	public static event ScoresLoadedEventHandler retrieveMathesBestScoresFinished;

	private void Awake()
	{
		base.gameObject.name = GetType().ToString();
		Object.DontDestroyOnLoad(this);
	}

	public void matchmakerWasCancelled(string empty)
	{
		if (GameCenterMultiplayerManager.matchmakerCancelled != null)
		{
			GameCenterMultiplayerManager.matchmakerCancelled();
		}
	}

	public void matchmakerFoundMatchWithExpectedPlayerCount(string playerCount)
	{
		int count = int.Parse(playerCount);
		if (GameCenterMultiplayerManager.matchmakerFoundMatch != null)
		{
			GameCenterMultiplayerManager.matchmakerFoundMatch(count);
		}
	}

	public void friendRequestComposeViewControllerDidFinish(string empty)
	{
		if (GameCenterMultiplayerManager.friendRequestControllerFinished != null)
		{
			GameCenterMultiplayerManager.friendRequestControllerFinished();
		}
	}

	public void playerDidConnectToMatch(string playerId)
	{
		if (GameCenterMultiplayerManager.playerConnected != null)
		{
			GameCenterMultiplayerManager.playerConnected(playerId);
		}
	}

	public void playerDidDisconnectFromMatch(string playerId)
	{
		if (GameCenterMultiplayerManager.playerDisconnected != null)
		{
			GameCenterMultiplayerManager.playerDisconnected(playerId);
		}
	}

	public void connectionWithPlayerFailed(string playerId)
	{
		if (GameCenterMultiplayerManager.playerConnectionFailed != null)
		{
			GameCenterMultiplayerManager.playerConnectionFailed(playerId);
		}
	}

	public void inviteRequestReceived(string emtpy)
	{
		if (GameCenterMultiplayerManager.inviteRequestWasReceived != null)
		{
			GameCenterMultiplayerManager.inviteRequestWasReceived();
		}
	}

	public void playerIsSpeaking(string playerId)
	{
		if (GameCenterMultiplayerManager.playerBeganSpeaking != null)
		{
			GameCenterMultiplayerManager.playerBeganSpeaking(playerId);
		}
	}

	public void playerIsSilent(string playerId)
	{
		if (GameCenterMultiplayerManager.playerStoppedSpeaking != null)
		{
			GameCenterMultiplayerManager.playerStoppedSpeaking(playerId);
		}
	}

	public void findMatchProgramaticallyFailed(string error)
	{
		if (GameCenterMultiplayerManager.findMatchFailed != null)
		{
			GameCenterMultiplayerManager.findMatchFailed(error);
		}
	}

	public void findMatchProgramaticallyFinishedWithExpectedPlayerCount(string playerCount)
	{
		int count = int.Parse(playerCount);
		if (GameCenterMultiplayerManager.findMatchFinished != null)
		{
			GameCenterMultiplayerManager.findMatchFinished(count);
		}
	}

	public void addPlayersToCurrentMatchFailed(string error)
	{
		if (GameCenterMultiplayerManager.addPlayersToMatchFailed != null)
		{
			GameCenterMultiplayerManager.addPlayersToMatchFailed(error);
		}
	}

	public void findAllActivityFailed(string error)
	{
		if (GameCenterMultiplayerManager.findActivityFailed != null)
		{
			GameCenterMultiplayerManager.findActivityFailed(error);
		}
	}

	public void findAllActivityFinished(string activity)
	{
		int count = int.Parse(activity);
		if (GameCenterMultiplayerManager.findActivityFinished != null)
		{
			GameCenterMultiplayerManager.findActivityFinished(count);
		}
	}

	public void findAllActivityForPlayerGroupFailed(string error)
	{
		if (GameCenterMultiplayerManager.findActivityForGroupFailed != null)
		{
			GameCenterMultiplayerManager.findActivityForGroupFailed(error);
		}
	}

	public void findAllActivityForPlayerGroupFinished(string activity)
	{
		int count = int.Parse(activity);
		if (GameCenterMultiplayerManager.findActivityForGroupFinished != null)
		{
			GameCenterMultiplayerManager.findActivityForGroupFinished(count);
		}
	}

	public void retrieveMatchesBestScoresDidFail(string error)
	{
		if (GameCenterMultiplayerManager.retrieveMatchesBestScoresFailed != null)
		{
			GameCenterMultiplayerManager.retrieveMatchesBestScoresFailed(error);
		}
	}

	public void retrieveMatchesBestScoresDidFinish(string jsonScores)
	{
		List<GameCenterScore> scores = GameCenterScore.fromJSON(jsonScores);
		if (GameCenterMultiplayerManager.retrieveMathesBestScoresFinished != null)
		{
			GameCenterMultiplayerManager.retrieveMathesBestScoresFinished(scores);
		}
	}
}
