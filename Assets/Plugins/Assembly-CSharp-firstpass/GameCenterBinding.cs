using System.Runtime.InteropServices;
using UnityEngine;

// _gameCenter* native symbols only exist in the iOS build - keep this class out of PSP2/other builds to avoid unresolved-symbol link errors.
#if UNITY_IOS
public class GameCenterBinding
{
	[DllImport("__Internal")]
	private static extern bool _gameCenterIsGameCenterAvailable();

	public static bool isGameCenterAvailable()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return _gameCenterIsGameCenterAvailable();
		}
		return false;
	}

	[DllImport("__Internal")]
	private static extern void _gameCenterAuthenticateLocalPlayer();

	public static void authenticateLocalPlayer()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_gameCenterAuthenticateLocalPlayer();
		}
	}

	[DllImport("__Internal")]
	private static extern bool _gameCenterIsPlayerAuthenticated();

	public static bool isPlayerAuthenticated()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return _gameCenterIsPlayerAuthenticated();
		}
		return false;
	}

	[DllImport("__Internal")]
	private static extern string _gameCenterPlayerAlias();

	public static string playerAlias()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return _gameCenterPlayerAlias();
		}
		return string.Empty;
	}

	[DllImport("__Internal")]
	private static extern string _gameCenterPlayerIdentifier();

	public static string playerIdentifier()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return _gameCenterPlayerIdentifier();
		}
		return string.Empty;
	}

	[DllImport("__Internal")]
	private static extern bool _gameCenterIsUnderage();

	public static bool isUnderage()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return _gameCenterIsUnderage();
		}
		return false;
	}

	[DllImport("__Internal")]
	private static extern void _gameCenterRetrieveFriends();

	public static void retrieveFriends()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_gameCenterRetrieveFriends();
		}
	}

	[DllImport("__Internal")]
	private static extern void _gameCenterLoadPlayerData(string playerIds);

	public static void loadPlayerData(string[] playerIdArray)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_gameCenterLoadPlayerData(string.Join(",", playerIdArray));
		}
	}

	[DllImport("__Internal")]
	private static extern void _gameCenterLoadLeaderboardCategoryTitles();

	public static void loadLeaderboardCategoryTitles()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_gameCenterLoadLeaderboardCategoryTitles();
		}
	}

	[DllImport("__Internal")]
	private static extern void _gameCenterReportScore(long score, string categoryId);

	public static void reportScore(long score, string categoryId)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_gameCenterReportScore(score, categoryId);
		}
	}

	[DllImport("__Internal")]
	private static extern void _gameCenterShowLeaderboardWithTimeScope(int timeScope);

	public static void showLeaderboardWithTimeScope(GameCenterLeaderboardTimeScope timeScope)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_gameCenterShowLeaderboardWithTimeScope((int)timeScope);
		}
	}

	[DllImport("__Internal")]
	private static extern void _gameCenterShowLeaderboardWithTimeScopeAndCategoryId(int timeScope, string categoryId);

	public static void showLeaderboardWithTimeScopeAndCategory(GameCenterLeaderboardTimeScope timeScope, string categoryId)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_gameCenterShowLeaderboardWithTimeScopeAndCategoryId((int)timeScope, categoryId);
		}
	}

	[DllImport("__Internal")]
	private static extern void _gameCenterRetrieveScores(bool friendsOnly, int timeScope, int start, int end);

	public static void retrieveScores(bool friendsOnly, GameCenterLeaderboardTimeScope timeScope, int start, int end)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_gameCenterRetrieveScores(friendsOnly, (int)timeScope, start, end);
		}
	}

	[DllImport("__Internal")]
	private static extern void _gameCenterRetrieveScoresForCategory(bool friendsOnly, int timeScope, int start, int end, string category);

	public static void retrieveScores(bool friendsOnly, GameCenterLeaderboardTimeScope timeScope, int start, int end, string category)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_gameCenterRetrieveScoresForCategory(friendsOnly, (int)timeScope, start, end, category);
		}
	}

	[DllImport("__Internal")]
	private static extern void _gameCenterRetrieveScoresForPlayerId(string playerId);

	public static void retrieveScoresForPlayerId(string playerId)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_gameCenterRetrieveScoresForPlayerId(playerId);
		}
	}

	[DllImport("__Internal")]
	private static extern void _gameCenterRetrieveScoresForPlayerIdAndCategory(string playerId, string category);

	public static void retrieveScoresForPlayerId(string playerId, string category)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_gameCenterRetrieveScoresForPlayerIdAndCategory(playerId, category);
		}
	}

	[DllImport("__Internal")]
	private static extern void _gameCenterReportAchievement(string identifier, float percent);

	public static void reportAchievement(string identifier, float percent)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_gameCenterReportAchievement(identifier, percent);
		}
	}

	[DllImport("__Internal")]
	private static extern void _gameCenterGetAchievements();

	public static void getAchievements()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_gameCenterGetAchievements();
		}
	}

	[DllImport("__Internal")]
	private static extern void _gameCenterResetAchievements();

	public static void resetAchievements()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_gameCenterResetAchievements();
		}
	}

	[DllImport("__Internal")]
	private static extern void _gameCenterShowAchievements();

	public static void showAchievements()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_gameCenterShowAchievements();
		}
	}

	[DllImport("__Internal")]
	private static extern void _gameCenterRetrieveAchievementMetadata();

	public static void retrieveAchievementMetadata()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_gameCenterRetrieveAchievementMetadata();
		}
	}
}
#else
public class GameCenterBinding
{
	public static bool isGameCenterAvailable() { return false; }

	public static void authenticateLocalPlayer() { }

	public static bool isPlayerAuthenticated() { return false; }

	public static string playerAlias() { return string.Empty; }

	public static string playerIdentifier() { return string.Empty; }

	public static bool isUnderage() { return false; }

	public static void retrieveFriends() { }

	public static void loadPlayerData(string[] playerIdArray) { }

	public static void loadLeaderboardCategoryTitles() { }

	public static void reportScore(long score, string categoryId) { }

	public static void showLeaderboardWithTimeScope(GameCenterLeaderboardTimeScope timeScope) { }

	public static void showLeaderboardWithTimeScopeAndCategory(GameCenterLeaderboardTimeScope timeScope, string categoryId) { }

	public static void retrieveScores(bool friendsOnly, GameCenterLeaderboardTimeScope timeScope, int start, int end) { }

	public static void retrieveScores(bool friendsOnly, GameCenterLeaderboardTimeScope timeScope, int start, int end, string category) { }

	public static void retrieveScoresForPlayerId(string playerId) { }

	public static void retrieveScoresForPlayerId(string playerId, string category) { }

	public static void reportAchievement(string identifier, float percent) { }

	public static void getAchievements() { }

	public static void resetAchievements() { }

	public static void showAchievements() { }

	public static void retrieveAchievementMetadata() { }
}
#endif
