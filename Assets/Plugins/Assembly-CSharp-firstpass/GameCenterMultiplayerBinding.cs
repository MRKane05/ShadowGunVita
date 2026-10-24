using System;
using System.Runtime.InteropServices;
using UnityEngine;

// _gameCenterMultiplayer* native symbols only exist in the iOS build - keep this class out of PSP2/other builds to avoid unresolved-symbol link errors.
#if UNITY_IOS
public class GameCenterMultiplayerBinding : MonoBehaviour
{
	[DllImport("__Internal")]
	private static extern void _gameCenterMultiplayerShowMatchmakerWithMinMaxPlayers(int minPlayers, int maxPlayers);

	public static void showMatchmakerWithMinMaxPlayers(int minPlayers, int maxPlayers)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_gameCenterMultiplayerShowMatchmakerWithMinMaxPlayers(minPlayers, maxPlayers);
		}
	}

	[DllImport("__Internal")]
	private static extern void _gameCenterMultiplayerShowMatchmakerWithFilters(int minPlayers, int maxPlayers, uint playerGroup, uint playerAttributes);

	public static void showMatchmakerWithFilters(int minPlayers, int maxPlayers, uint playerGroup, uint playerAttributes)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_gameCenterMultiplayerShowMatchmakerWithFilters(minPlayers, maxPlayers, playerGroup, playerAttributes);
		}
	}

	[DllImport("__Internal")]
	private static extern void _gameCenterMultiplayerShowFriendRequestController();

	public static void showFriendRequestController()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_gameCenterMultiplayerShowFriendRequestController();
		}
	}

	[DllImport("__Internal")]
	private static extern string _gameCenterMultiplayerSendMessageToAllPeers(string gameObject, string method, string param, bool reliably);

	public static string sendMessageToAllPeers(string gameObject, string method, string param, bool reliably)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return _gameCenterMultiplayerSendMessageToAllPeers(gameObject, method, param, reliably);
		}
		return string.Empty;
	}

	[DllImport("__Internal")]
	private static extern string _gameCenterMultiplayerSendMessageToPeers(string playerIds, string gameObject, string method, string param, bool reliably);

	public static string sendMessageToPeers(string[] playerIds, string gameObject, string method, string param, bool reliably)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return _gameCenterMultiplayerSendMessageToPeers(string.Join(",", playerIds), gameObject, method, param, reliably);
		}
		return string.Empty;
	}

	[DllImport("__Internal")]
	private static extern string _gameCenterMultiplayerSendRawMessageToAllPeers(string gameObject, string method, byte[] param, int length, bool reliably);

	public static string sendRawMessageToAllPeers(string gameObject, string method, byte[] param, bool reliably)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return _gameCenterMultiplayerSendRawMessageToAllPeers(gameObject, method, param, param.Length, reliably);
		}
		return string.Empty;
	}

	[DllImport("__Internal")]
	private static extern string _gameCenterMultiplayerSendRawMessageToPeers(string playerIds, string gameObject, string method, byte[] param, int length, bool reliably);

	public static string sendRawMessageToPeers(string[] playerIds, string gameObject, string method, byte[] param, bool reliably)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return _gameCenterMultiplayerSendRawMessageToPeers(string.Join(",", playerIds), gameObject, method, param, param.Length, reliably);
		}
		return string.Empty;
	}

	[DllImport("__Internal")]
	private static extern string _gameCenterGetAllConnectedPlayerIds();

	public static string[] getAllConnectedPlayerIds()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			string text = _gameCenterGetAllConnectedPlayerIds();
			return text.Split(new string[1] { "," }, StringSplitOptions.None);
		}
		return new string[0];
	}

	[DllImport("__Internal")]
	private static extern void _gameCenterMultiplayerDisconnectFromMatch();

	public static void disconnectFromMatch()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_gameCenterMultiplayerDisconnectFromMatch();
		}
	}

	[DllImport("__Internal")]
	private static extern bool _gameCenterMultiplayerIsVOIPAllowed();

	public static bool isVOIPAllowed()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return _gameCenterMultiplayerIsVOIPAllowed();
		}
		return false;
	}

	[DllImport("__Internal")]
	private static extern void _gameCenterMultiplayerEnableVoiceChat(bool isEnabled);

	public static void enableVoiceChat(bool isEnabled)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_gameCenterMultiplayerEnableVoiceChat(isEnabled);
		}
	}

	[DllImport("__Internal")]
	private static extern void _gameCenterMultiplayerCloseAllOpenVoiceChats();

	public static void closeAllOpenVoiceChats()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_gameCenterMultiplayerCloseAllOpenVoiceChats();
		}
	}

	[DllImport("__Internal")]
	private static extern void _gameCenterMultiplayerAddAndStartVoiceChatChannel(string channelName);

	public static void addAndStartVoiceChatChannel(string channelName)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_gameCenterMultiplayerAddAndStartVoiceChatChannel(channelName);
		}
	}

	[DllImport("__Internal")]
	private static extern void _gameCenterMultiplayerStartVoiceChat(string channelName, bool shouldEnable);

	public static void startVoiceChat(string channelName, bool shouldEnable)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_gameCenterMultiplayerStartVoiceChat(channelName, shouldEnable);
		}
	}

	[DllImport("__Internal")]
	private static extern void _gameCenterMultiplayerEnableMicrophone(string channelName, bool shouldEnable);

	public static void enableMicrophone(string channelName, bool shouldEnable)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_gameCenterMultiplayerEnableMicrophone(channelName, shouldEnable);
		}
	}

	[DllImport("__Internal")]
	private static extern void _gameCenterMultiplayerSetVolume(string channelName, float volume);

	public static void setVolume(string channelName, float volume)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_gameCenterMultiplayerSetVolume(channelName, volume);
		}
	}

	[DllImport("__Internal")]
	private static extern void _gameCenterMultiplayerSetMute(string channelName, string playerId, bool shouldMute);

	public static void setMute(string channelName, string playerId, bool shouldMute)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_gameCenterMultiplayerSetMute(channelName, playerId, shouldMute);
		}
	}

	[DllImport("__Internal")]
	private static extern void _gameCenterMultiplayerReceiveUpdates(string channelName, bool shouldReceiveUpdates);

	public static void receiveUpdates(string channelName, bool shouldReceiveUpdates)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_gameCenterMultiplayerReceiveUpdates(channelName, shouldReceiveUpdates);
		}
	}

	[DllImport("__Internal")]
	private static extern void _gameCenterMultiplayerFindMatchProgrammaticallyWithMinMaxPlayers(int minPlayers, int maxPlayers);

	public static void findMatchProgrammaticallyWithMinMaxPlayers(int minPlayers, int maxPlayers)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_gameCenterMultiplayerFindMatchProgrammaticallyWithMinMaxPlayers(minPlayers, maxPlayers);
		}
	}

	[DllImport("__Internal")]
	private static extern void _gameCenterMultiplayerFindMatchProgrammaticallyWithFilters(int minPlayers, int maxPlayers, uint playerGroup, uint playerAttributes);

	public static void findMatchProgrammaticallyWithFilters(int minPlayers, int maxPlayers, uint playerGroup, uint playerAttributes)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_gameCenterMultiplayerFindMatchProgrammaticallyWithFilters(minPlayers, maxPlayers, playerGroup, playerAttributes);
		}
	}

	[DllImport("__Internal")]
	private static extern void _gameCenterMultiplayerCancelProgrammaticMatchRequest();

	public static void cancelProgrammaticMatchRequest()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_gameCenterMultiplayerCancelProgrammaticMatchRequest();
		}
	}

	[DllImport("__Internal")]
	private static extern void _gameCenterMultiplayerFindAllActivity();

	public static void findAllActivity()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_gameCenterMultiplayerFindAllActivity();
		}
	}
}
#else
public class GameCenterMultiplayerBinding : MonoBehaviour
{
	public static void showMatchmakerWithMinMaxPlayers(int minPlayers, int maxPlayers) { }

	public static void showMatchmakerWithFilters(int minPlayers, int maxPlayers, uint playerGroup, uint playerAttributes) { }

	public static void showFriendRequestController() { }

	public static string sendMessageToAllPeers(string gameObject, string method, string param, bool reliably) { return string.Empty; }

	public static string sendMessageToPeers(string[] playerIds, string gameObject, string method, string param, bool reliably) { return string.Empty; }

	public static string sendRawMessageToAllPeers(string gameObject, string method, byte[] param, bool reliably) { return string.Empty; }

	public static string sendRawMessageToPeers(string[] playerIds, string gameObject, string method, byte[] param, bool reliably) { return string.Empty; }

	public static string[] getAllConnectedPlayerIds() { return new string[0]; }

	public static void disconnectFromMatch() { }

	public static bool isVOIPAllowed() { return false; }

	public static void enableVoiceChat(bool isEnabled) { }

	public static void closeAllOpenVoiceChats() { }

	public static void addAndStartVoiceChatChannel(string channelName) { }

	public static void startVoiceChat(string channelName, bool shouldEnable) { }

	public static void enableMicrophone(string channelName, bool shouldEnable) { }

	public static void setVolume(string channelName, float volume) { }

	public static void setMute(string channelName, string playerId, bool shouldMute) { }

	public static void receiveUpdates(string channelName, bool shouldReceiveUpdates) { }

	public static void findMatchProgrammaticallyWithMinMaxPlayers(int minPlayers, int maxPlayers) { }

	public static void findMatchProgrammaticallyWithFilters(int minPlayers, int maxPlayers, uint playerGroup, uint playerAttributes) { }

	public static void cancelProgrammaticMatchRequest() { }

	public static void findAllActivity() { }
}
#endif
