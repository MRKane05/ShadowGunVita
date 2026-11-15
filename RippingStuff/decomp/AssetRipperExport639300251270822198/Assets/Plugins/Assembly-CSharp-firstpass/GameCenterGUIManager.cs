using System.Collections.Generic;
using UnityEngine;

public class GameCenterGUIManager : MonoBehaviour
{
	private List<GameCenterLeaderboard> leaderboards;

	private List<GameCenterAchievementMetadata> achievementMetadata;

	private void Start()
	{
		GameCenterManager.categoriesLoaded += delegate(List<GameCenterLeaderboard> leaderboards)
		{
			this.leaderboards = leaderboards;
		};
		GameCenterManager.achievementMetadataLoaded += delegate(List<GameCenterAchievementMetadata> achievementMetadata)
		{
			this.achievementMetadata = achievementMetadata;
		};
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
		if (GUI.Button(new Rect(num2, num += num4, num3, height), "Load Achievement Metadata"))
		{
			GameCenterBinding.retrieveAchievementMetadata();
		}
		if (GUI.Button(new Rect(num2, num += num4, num3, height), "Get Raw Achievements"))
		{
			GameCenterBinding.getAchievements();
		}
		if (GUI.Button(new Rect(num2, num += num4, num3, height), "Post Achievement") && achievementMetadata != null && achievementMetadata.Count > 0)
		{
			int num5 = Random.Range(2, 60);
			Debug.Log("sending percentComplete: " + num5);
			GameCenterBinding.reportAchievement(achievementMetadata[0].identifier, num5);
		}
		if (GUI.Button(new Rect(num2, num += num4, num3, height), "Show Achievements"))
		{
			GameCenterBinding.showAchievements();
		}
		if (GUI.Button(new Rect(num2, num += num4, num3, height), "Reset Achievements"))
		{
			GameCenterBinding.resetAchievements();
		}
		if (GUI.Button(new Rect(num2, num += num4, num3, height), "Multiplayer Scene"))
		{
			Application.LoadLevel("GameCenterMultiplayerTestScene");
		}
		num = 5f;
		num2 = num3 + num2 * 2f;
		if (GUI.Button(new Rect(num2, num, num3, height), "Get Player Alias"))
		{
			string text = GameCenterBinding.playerAlias();
			Debug.Log("Player alias: " + text);
		}
		if (GUI.Button(new Rect(num2, num += num4, num3, height), "Load Leaderboard Data"))
		{
			GameCenterBinding.loadLeaderboardCategoryTitles();
		}
		if (GUI.Button(new Rect(num2, num += num4, num3, height), "Post Score") && leaderboards != null && leaderboards.Count > 0)
		{
			Debug.Log("about to report a random score for category: " + leaderboards[0].categoryId);
			GameCenterBinding.reportScore(Random.Range(1, 99999), leaderboards[0].categoryId);
		}
		if (GUI.Button(new Rect(num2, num += num4, num3, height), "Show Leaderboards"))
		{
			GameCenterBinding.showLeaderboardWithTimeScope(GameCenterLeaderboardTimeScope.AllTime);
		}
		if (GUI.Button(new Rect(num2, num += num4, num3, height), "Get Raw Score Data"))
		{
			if (leaderboards != null && leaderboards.Count > 0)
			{
				GameCenterBinding.retrieveScores(false, GameCenterLeaderboardTimeScope.AllTime, 1, 25, leaderboards[0].categoryId);
			}
			else
			{
				Debug.Log("Load leaderboard data before attempting to retrieve scores");
			}
		}
		if (GUI.Button(new Rect(num2, num += num4, num3, height), "Get Scores for Me"))
		{
			if (leaderboards != null && leaderboards.Count > 0)
			{
				GameCenterBinding.retrieveScoresForPlayerId(GameCenterBinding.playerIdentifier(), leaderboards[0].categoryId);
			}
			else
			{
				Debug.Log("Load leaderboard data before attempting to retrieve scores");
			}
		}
		if (GUI.Button(new Rect(num2, num += num4, num3, height), "Retrieve Friends"))
		{
			GameCenterBinding.retrieveFriends();
		}
	}
}
