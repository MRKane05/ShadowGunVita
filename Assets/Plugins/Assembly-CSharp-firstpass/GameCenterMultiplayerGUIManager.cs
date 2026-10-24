using UnityEngine;

public class GameCenterMultiplayerGUIManager : MonoBehaviour
{
	public GUIText statusText;

	private void Start()
	{
		GameCenterMultiplayerManager.playerConnected += playerConnected;
		GameCenterMultiplayerManager.playerDisconnected += playerDisconnected;
	}

	private void OnDisable()
	{
		GameCenterMultiplayerManager.playerConnected -= playerConnected;
		GameCenterMultiplayerManager.playerDisconnected -= playerDisconnected;
	}

	public void playerConnected(string playerId)
	{
		statusText.text = "player connected: " + playerId;
	}

	private void playerDisconnected(string playerId)
	{
		statusText.text = "player disconnected: " + playerId;
	}

	private void OnGUI()
	{
		float num = 5f;
		float num2 = 10f;
		float num3 = 220f;
		float height = 40f;
		float num4 = 45f;
		if (GUI.Button(new Rect(num2, num, num3, height), "Authenticate"))
		{
			GameCenterBinding.authenticateLocalPlayer();
		}
		if (GUI.Button(new Rect(num2, num += num4, num3, height), "Show Matchmaker"))
		{
			GameCenterMultiplayerBinding.showMatchmakerWithMinMaxPlayers(2, 4);
		}
		if (GUI.Button(new Rect(num2, num += num4, num3, height), "Send To All"))
		{
			GameCenterMultiplayerBinding.sendMessageToAllPeers("GCTestObject", "receiveTime", Time.timeSinceLevelLoad.ToString(), false);
		}
		if (GUI.Button(new Rect(num2, num += num4, num3, height), "Disconnect"))
		{
			GameCenterMultiplayerBinding.disconnectFromMatch();
		}
		if (GUI.Button(new Rect(num2, num += num4, num3, height), "Show Friend Request (iOS 4.2)"))
		{
			GameCenterMultiplayerBinding.showFriendRequestController();
		}
		if (GUI.Button(new Rect(num2, num += num4 * 2f, num3, height), "Advanced Multiplayer"))
		{
			GameCenterMultiplayerBinding.disconnectFromMatch();
			Application.LoadLevel("GameCenterMultiplayerTestSceneTwo");
		}
		num = 5f;
		num2 = num3 + num2 * 2f;
		if (GUI.Button(new Rect(num2, num, num3, height), "Find Match (no GUI)"))
		{
			GameCenterMultiplayerBinding.findMatchProgrammaticallyWithMinMaxPlayers(2, 4);
		}
		if (GUI.Button(new Rect(num2, num += num4, num3, height), "Cancel Find Match"))
		{
			GameCenterMultiplayerBinding.cancelProgrammaticMatchRequest();
		}
		if (GUI.Button(new Rect(num2, num += num4, num3, height), "Find Activity"))
		{
			GameCenterMultiplayerBinding.findAllActivity();
		}
		if (GUI.Button(new Rect(num2, num += num4, num3, height), "Enable Voicechat"))
		{
			GameCenterMultiplayerBinding.enableVoiceChat(true);
		}
		if (GUI.Button(new Rect(num2, num += num4, num3, height), "Start Voicechat"))
		{
			GameCenterMultiplayerBinding.addAndStartVoiceChatChannel("testChannel");
		}
		if (GUI.Button(new Rect(num2, num += num4, num3, height), "Receive VC Updates"))
		{
			GameCenterMultiplayerBinding.receiveUpdates("testChannel", true);
		}
	}
}
