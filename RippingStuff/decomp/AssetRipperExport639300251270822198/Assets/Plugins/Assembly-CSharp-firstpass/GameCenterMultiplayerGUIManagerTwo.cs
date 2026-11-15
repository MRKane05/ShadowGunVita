using UnityEngine;

public class GameCenterMultiplayerGUIManagerTwo : MonoBehaviour
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
		float num3 = 180f;
		float height = 40f;
		float num4 = 45f;
		if (GUI.Button(new Rect(num2, num, num3, height), "Authenticate"))
		{
			GameCenterBinding.authenticateLocalPlayer();
		}
		if (GUI.Button(new Rect(num2, num += num4, num3, height), "Matchmaker for Game One"))
		{
			GameCenterMultiplayerBinding.showMatchmakerWithFilters(2, 4, 1u, 0u);
		}
		if (GUI.Button(new Rect(num2, num += num4, num3, height), "Matchmaker for Game Two"))
		{
			GameCenterMultiplayerBinding.showMatchmakerWithFilters(2, 4, 2u, 0u);
		}
		if (GUI.Button(new Rect(num2, num += num4, num3, height), "Send To All"))
		{
			GameCenterMultiplayerBinding.sendMessageToAllPeers("GCTestObject", "receiveTime", Time.timeSinceLevelLoad.ToString(), false);
		}
		if (GUI.Button(new Rect(num2, num += num4, num3, height), "Disconnect"))
		{
			GameCenterMultiplayerBinding.disconnectFromMatch();
		}
		if (GUI.Button(new Rect(num2, num += num4, num3, height), "Get Connected PlayerIds"))
		{
			string[] allConnectedPlayerIds = GameCenterMultiplayerBinding.getAllConnectedPlayerIds();
			string[] array = allConnectedPlayerIds;
			foreach (string text in array)
			{
				Debug.Log("connected to: " + text);
			}
		}
		if (GUI.Button(new Rect(num2, num += num4, num3, height), "Raw Data Send Scene"))
		{
			GameCenterMultiplayerBinding.disconnectFromMatch();
			Application.LoadLevel("GameCenterMultiplayerTestSceneThree");
		}
		num = 5f;
		num2 = num3 + num2 * 2f;
		if (GUI.Button(new Rect(num2, num, num3, height), "Find Match as Alien"))
		{
			GameCenterMultiplayerBinding.findMatchProgrammaticallyWithFilters(2, 4, 0u, 4294901760u);
		}
		if (GUI.Button(new Rect(num2, num += num4, num3, height), "Find Match as Predator"))
		{
			GameCenterMultiplayerBinding.findMatchProgrammaticallyWithFilters(2, 4, 0u, 65535u);
		}
		if (GUI.Button(new Rect(num2, num += num4, num3, height), "Cancel Find Match"))
		{
			GameCenterMultiplayerBinding.cancelProgrammaticMatchRequest();
		}
	}
}
