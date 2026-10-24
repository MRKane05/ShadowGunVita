using System.Collections.Generic;
using UnityEngine;

public class GameCenterManager : MonoBehaviour
{
	public delegate void GameCenterErrorEventHandler(string error);

	public delegate void GameCenterStringEventHandler(string param);

	public delegate void GameCenterEventHandler();

	public delegate void PlayerDataLoadedEventHandler(List<GameCenterPlayer> players);

	public delegate void CategoriesLoadedEventHandler(List<GameCenterLeaderboard> leaderboards);

	public delegate void ScoresLoadedEventHandler(List<GameCenterScore> scores);

	public delegate void AchievementLoadedEventHandler(List<GameCenterAchievement> achievements);

	public delegate void AchievementMetadataLoadedEventHandler(List<GameCenterAchievementMetadata> achievementMetadata);

	public static event GameCenterErrorEventHandler loadPlayerDataFailed;

	public static event PlayerDataLoadedEventHandler playerDataLoaded;

	public static event GameCenterEventHandler playerAuthenticated;

	public static event GameCenterErrorEventHandler playerFailedToAuthenticate;

	public static event GameCenterEventHandler playerLoggedOut;

	public static event GameCenterErrorEventHandler loadCategoryTitlesFailed;

	public static event CategoriesLoadedEventHandler categoriesLoaded;

	public static event GameCenterErrorEventHandler reportScoreFailed;

	public static event GameCenterStringEventHandler reportScoreFinished;

	public static event GameCenterStringEventHandler retrieveScoresFailed;

	public static event ScoresLoadedEventHandler scoresLoaded;

	public static event GameCenterErrorEventHandler retrieveScoresForPlayerIdFailed;

	public static event ScoresLoadedEventHandler scoresForPlayerIdLoaded;

	public static event GameCenterErrorEventHandler reportAchievementFailed;

	public static event GameCenterStringEventHandler reportAchievementFinished;

	public static event GameCenterErrorEventHandler loadAchievementsFailed;

	public static event AchievementLoadedEventHandler achievementsLoaded;

	public static event GameCenterErrorEventHandler resetAchievementsFailed;

	public static event GameCenterEventHandler resetAchievementsFinished;

	public static event GameCenterErrorEventHandler retrieveAchievementMetadataFailed;

	public static event AchievementMetadataLoadedEventHandler achievementMetadataLoaded;

	private void Awake()
	{
		base.gameObject.name = GetType().ToString();
		Object.DontDestroyOnLoad(this);
	}

	public void loadPlayerDataDidFail(string error)
	{
		if (GameCenterManager.loadPlayerDataFailed != null)
		{
			GameCenterManager.loadPlayerDataFailed(error);
		}
	}

	public void loadPlayerDataDidLoad(string jsonFriendList)
	{
		List<GameCenterPlayer> players = GameCenterPlayer.fromJSON(jsonFriendList);
		if (GameCenterManager.playerDataLoaded != null)
		{
			GameCenterManager.playerDataLoaded(players);
		}
	}

	public void playerDidLogOut()
	{
		if (GameCenterManager.playerLoggedOut != null)
		{
			GameCenterManager.playerLoggedOut();
		}
	}

	public void playerDidAuthenticate()
	{
		if (GameCenterManager.playerAuthenticated != null)
		{
			GameCenterManager.playerAuthenticated();
		}
	}

	public void playerAuthenticationFailed(string error)
	{
		if (GameCenterManager.playerFailedToAuthenticate != null)
		{
			GameCenterManager.playerFailedToAuthenticate(error);
		}
	}

	public void loadCategoryTitlesDidFail(string error)
	{
		if (GameCenterManager.loadCategoryTitlesFailed != null)
		{
			GameCenterManager.loadCategoryTitlesFailed(error);
		}
	}

	public void categoriesDidLoad(string jsonCategoryList)
	{
		List<GameCenterLeaderboard> leaderboards = GameCenterLeaderboard.fromJSON(jsonCategoryList);
		if (GameCenterManager.categoriesLoaded != null)
		{
			GameCenterManager.categoriesLoaded(leaderboards);
		}
	}

	public void reportScoreDidFail(string error)
	{
		if (GameCenterManager.reportScoreFailed != null)
		{
			GameCenterManager.reportScoreFailed(error);
		}
	}

	public void reportScoreDidFinish(string category)
	{
		if (GameCenterManager.reportScoreFinished != null)
		{
			GameCenterManager.reportScoreFinished(category);
		}
	}

	public void retrieveScoresDidFail(string category)
	{
		if (GameCenterManager.retrieveScoresFailed != null)
		{
			GameCenterManager.retrieveScoresFailed(category);
		}
	}

	public void retrieveScoresDidLoad(string jsonScoresList)
	{
		List<GameCenterScore> scores = GameCenterScore.fromJSON(jsonScoresList);
		if (GameCenterManager.scoresLoaded != null)
		{
			GameCenterManager.scoresLoaded(scores);
		}
	}

	public void retrieveScoresForPlayerIdDidFail(string error)
	{
		if (GameCenterManager.retrieveScoresForPlayerIdFailed != null)
		{
			GameCenterManager.retrieveScoresForPlayerIdFailed(error);
		}
	}

	public void retrieveScoresForPlayerIdDidLoad(string jsonScoresList)
	{
		List<GameCenterScore> scores = GameCenterScore.fromJSON(jsonScoresList);
		if (GameCenterManager.scoresForPlayerIdLoaded != null)
		{
			GameCenterManager.scoresForPlayerIdLoaded(scores);
		}
	}

	public void reportAchievementDidFail(string error)
	{
		if (GameCenterManager.reportAchievementFailed != null)
		{
			GameCenterManager.reportAchievementFailed(error);
		}
	}

	public void reportAchievementDidFinish(string identifier)
	{
		if (GameCenterManager.reportAchievementFinished != null)
		{
			GameCenterManager.reportAchievementFinished(identifier);
		}
	}

	public void loadAchievementsDidFail(string error)
	{
		if (GameCenterManager.loadAchievementsFailed != null)
		{
			GameCenterManager.loadAchievementsFailed(error);
		}
	}

	public void achievementsDidLoad(string jsonAchievmentList)
	{
		List<GameCenterAchievement> achievements = GameCenterAchievement.fromJSON(jsonAchievmentList);
		if (GameCenterManager.achievementsLoaded != null)
		{
			GameCenterManager.achievementsLoaded(achievements);
		}
	}

	public void resetAchievementsDidFail(string error)
	{
		if (GameCenterManager.resetAchievementsFailed != null)
		{
			GameCenterManager.resetAchievementsFailed(error);
		}
	}

	public void resetAchievementsDidFinish(string emptyString)
	{
		if (GameCenterManager.resetAchievementsFinished != null)
		{
			GameCenterManager.resetAchievementsFinished();
		}
	}

	public void retrieveAchievementsMetadataDidFail(string error)
	{
		if (GameCenterManager.retrieveAchievementMetadataFailed != null)
		{
			GameCenterManager.retrieveAchievementMetadataFailed(error);
		}
	}

	public void achievementMetadataDidLoad(string jsonAchievementDescriptionList)
	{
		List<GameCenterAchievementMetadata> achievementMetadata = GameCenterAchievementMetadata.fromJSON(jsonAchievementDescriptionList);
		if (GameCenterManager.achievementMetadataLoaded != null)
		{
			GameCenterManager.achievementMetadataLoaded(achievementMetadata);
		}
	}
}
