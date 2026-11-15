using System.Collections.Generic;
using UnityEngine;

public class GameCenterMultiplayerEventListener : MonoBehaviour
{
	private void Start()
	{
		GameCenterMultiplayerManager.matchmakerCancelled += matchmakerCancelled;
		GameCenterMultiplayerManager.matchmakerFoundMatch += matchmakerFoundMatch;
		GameCenterMultiplayerManager.friendRequestControllerFinished += friendRequestControllerFinished;
		GameCenterMultiplayerManager.playerConnected += playerConnected;
		GameCenterMultiplayerManager.playerDisconnected += playerDisconnected;
		GameCenterMultiplayerManager.playerConnectionFailed += playerConnectionFailed;
		GameCenterMultiplayerManager.inviteRequestWasReceived += inviteRequestWasReceived;
		GameCenterMultiplayerManager.playerBeganSpeaking += playerBeganSpeaking;
		GameCenterMultiplayerManager.playerStoppedSpeaking += playerStoppedSpeaking;
		GameCenterMultiplayerManager.findMatchFailed += findMatchFailed;
		GameCenterMultiplayerManager.findMatchFinished += findMatchFinished;
		GameCenterMultiplayerManager.addPlayersToMatchFailed += addPlayersToMatchFailed;
		GameCenterMultiplayerManager.findActivityFailed += findActivityFailed;
		GameCenterMultiplayerManager.findActivityFinished += findActivityFinished;
		GameCenterMultiplayerManager.findActivityForGroupFailed += findActivityForGroupFailed;
		GameCenterMultiplayerManager.findActivityForGroupFinished += findActivityForGroupFinished;
		GameCenterMultiplayerManager.retrieveMatchesBestScoresFailed += retrieveMatchesBestScoresFailed;
		GameCenterMultiplayerManager.retrieveMathesBestScoresFinished += retrieveMatchesBestScoresFinished;
	}

	private void OnDisable()
	{
		GameCenterMultiplayerManager.matchmakerCancelled -= matchmakerCancelled;
		GameCenterMultiplayerManager.matchmakerFoundMatch -= matchmakerFoundMatch;
		GameCenterMultiplayerManager.friendRequestControllerFinished -= friendRequestControllerFinished;
		GameCenterMultiplayerManager.playerConnected -= playerConnected;
		GameCenterMultiplayerManager.playerDisconnected -= playerDisconnected;
		GameCenterMultiplayerManager.playerConnectionFailed -= playerConnectionFailed;
		GameCenterMultiplayerManager.inviteRequestWasReceived -= inviteRequestWasReceived;
		GameCenterMultiplayerManager.playerBeganSpeaking -= playerBeganSpeaking;
		GameCenterMultiplayerManager.playerStoppedSpeaking -= playerStoppedSpeaking;
		GameCenterMultiplayerManager.findMatchFailed -= findMatchFailed;
		GameCenterMultiplayerManager.findMatchFinished -= findMatchFinished;
		GameCenterMultiplayerManager.addPlayersToMatchFailed -= addPlayersToMatchFailed;
		GameCenterMultiplayerManager.findActivityFailed -= findActivityFailed;
		GameCenterMultiplayerManager.findActivityFinished -= findActivityFinished;
		GameCenterMultiplayerManager.findActivityForGroupFailed -= findActivityForGroupFailed;
		GameCenterMultiplayerManager.findActivityForGroupFinished -= findActivityForGroupFinished;
		GameCenterMultiplayerManager.retrieveMatchesBestScoresFailed -= retrieveMatchesBestScoresFailed;
		GameCenterMultiplayerManager.retrieveMathesBestScoresFinished -= retrieveMatchesBestScoresFinished;
	}

	public void matchmakerCancelled()
	{
		Debug.Log("matchmakerCancelled");
	}

	public void playerConnectionFailed(string error)
	{
		Debug.Log("playerConnectionFailed: " + error);
	}

	public void inviteRequestWasReceived()
	{
		Debug.Log("inviteRequestWasReceived");
	}

	private void playerDisconnected(string playerId)
	{
		Debug.Log("playerDisconnected: " + playerId);
	}

	private void playerConnected(string playerId)
	{
		Debug.Log("playerConnected: " + playerId);
	}

	private void matchmakerFoundMatch(int count)
	{
		Debug.Log("matchmakerFoundMatch with expected player count: " + count);
	}

	private void friendRequestControllerFinished()
	{
		Debug.Log("friendRequestControllerFinished");
	}

	public void playerBeganSpeaking(string playerId)
	{
		Debug.Log("playerBeganSpeaking: " + playerId);
	}

	public void playerStoppedSpeaking(string playerId)
	{
		Debug.Log("playerStoppedSpeaking: " + playerId);
	}

	public void findMatchFailed(string error)
	{
		Debug.Log("findMatchFailed: " + error);
	}

	public void findMatchFinished(int count)
	{
		Debug.Log("findMatchFinished with expected count: " + count);
	}

	public void addPlayersToMatchFailed(string error)
	{
		Debug.Log("addPlayersToMatchFailed: " + error);
	}

	public void findActivityFailed(string error)
	{
		Debug.Log("findActivityFailed: " + error);
	}

	public void findActivityFinished(int count)
	{
		Debug.Log("findActivityFinished with count: " + count);
	}

	public void findActivityForGroupFailed(string error)
	{
		Debug.Log("findActivityForGroupFailed: " + error);
	}

	public void findActivityForGroupFinished(int count)
	{
		Debug.Log("findActivityForGroupFinished with count: " + count);
	}

	public void retrieveMatchesBestScoresFailed(string error)
	{
		Debug.Log("retrieveMatchesBestScoresFailed: " + error);
	}

	private void retrieveMatchesBestScoresFinished(List<GameCenterScore> scores)
	{
		foreach (GameCenterScore score in scores)
		{
			Debug.Log(score);
		}
	}
}
