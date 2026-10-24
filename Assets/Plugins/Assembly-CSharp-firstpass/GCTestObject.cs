using System.Text;
using UnityEngine;

public class GCTestObject : MonoBehaviour
{
	public GUIText statusText;

	private static GCTestObject _instance;

	private void Awake()
	{
		base.gameObject.name = GetType().ToString();
		_instance = this;
	}

	public void receiveTime(string data)
	{
		statusText.text = data;
	}

	public void sendRawBytes()
	{
		string s = "im a string that will be encoded to a byte array and sent across the wire using GameCenter";
		byte[] bytes = Encoding.UTF8.GetBytes(s);
		GameCenterMultiplayerBinding.sendRawMessageToAllPeers("GCTestObject", "receivedRawBytes", bytes, true);
	}

	public static void receivedRawBytes(byte[] bytes)
	{
		string text = Encoding.UTF8.GetString(bytes);
		Debug.Log("receivedRawBytes: " + text);
		_instance.statusText.text = text;
	}
}
