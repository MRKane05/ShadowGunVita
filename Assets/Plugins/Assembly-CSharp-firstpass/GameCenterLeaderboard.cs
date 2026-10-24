using System.Collections;
using System.Collections.Generic;

public class GameCenterLeaderboard
{
	public string categoryId;

	public string title;

	public GameCenterLeaderboard(string categoryId, string title)
	{
		this.categoryId = categoryId;
		this.title = title;
	}

	public static List<GameCenterLeaderboard> fromJSON(string json)
	{
		List<GameCenterLeaderboard> list = new List<GameCenterLeaderboard>();
		Hashtable hashtable = json.hashtableFromJson();
		foreach (DictionaryEntry item in hashtable)
		{
			list.Add(new GameCenterLeaderboard(item.Value as string, item.Key as string));
		}
		return list;
	}

	public override string ToString()
	{
		return string.Format("<Leaderboard> categoryId: {0}, title: {1}", categoryId, title);
	}
}
