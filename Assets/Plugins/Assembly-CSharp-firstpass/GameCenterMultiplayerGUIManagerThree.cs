using UnityEngine;

public class GameCenterMultiplayerGUIManagerThree : MonoBehaviour
{
	public GUIText statusText;

	private GCTestObject _testGameObject;

	private void Start()
	{
		GameCenterMultiplayerManager.playerConnected += playerConnected;
		GameCenterMultiplayerManager.playerDisconnected += playerDisconnected;
		GCTestObject[] array = Object.FindSceneObjectsOfType(typeof(GCTestObject)) as GCTestObject[];
		if (array.Length == 0)
		{
			Debug.Log("could not find any GCTestObjects in the scene!");
		}
		else
		{
			_testGameObject = array[0];
		}
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
			_testGameObject.sendRawBytes();
		}
		if (GUI.Button(new Rect(num2, num += num4 * 2f, num3, height), "Leaderboard Scene"))
		{
			GameCenterMultiplayerBinding.disconnectFromMatch();
			Application.LoadLevel("GameCenterTestScene");
		}
		num = 5f;
		num2 = num3 + num2 * 2f;
		if (GUI.Button(new Rect(num2, num, num3, height), "Find Match (no GUI)"))
		{
			GameCenterMultiplayerBinding.findMatchProgrammaticallyWithMinMaxPlayers(2, 4);
		}
	}
}
