using System.Collections;
using UnityEngine;

public class GuiAchievements : MonoBehaviour
{
	public static GuiAchievements Instance;

	private static GUIBase_Pivot m_PivotAchievements;

	private static GUIBase_Layout[] m_LayoutAchievement;

	private static string[] s_AchievementsName = new string[17]
	{
		"ALayout01", "ALayout02", "ALayout03", "ALayout04", "ALayout05", "ALayout06", "ALayout07", "ALayout08", "ALayout09", "ALayout10_Easy",
		"ALayout11_Medium", "ALayout12_Hard", "ALayout13_Extras", "ALayout14_Dlc1", "ALayout15_Dlc2", "ALayout16_Dlc3", "ALayout17_Dlc_Finished"
	};

	private int showingID = -1;

	private float hideRealTime;

	private void Awake()
	{
		Instance = this;
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
		CancelInvoke();
		Instance = null;
	}

	public void InitGui()
	{
		InitAchievements();
	}

	private void InitAchievements()
	{
		m_LayoutAchievement = new GUIBase_Layout[s_AchievementsName.Length];
		m_PivotAchievements = MFGuiManager.Instance.GetPivot("Achievements");
		if (!m_PivotAchievements)
		{
			Debug.LogError("Pivot 'Achievements' not found! ");
			return;
		}
		for (int i = 0; i < m_LayoutAchievement.Length; i++)
		{
			m_LayoutAchievement[i] = GuiBaseUtils.GetLayout(s_AchievementsName[i], m_PivotAchievements);
		}
	}

	private void Update()
	{
		if (showingID >= 0 && Time.realtimeSinceStartup >= hideRealTime)
		{
			_ShowAchievement(showingID, false);
			showingID = -1;
			hideRealTime = 0f;
		}
	}

	private void _ShowAchievement(int index, bool show)
	{
		if (index >= 0 && index < m_LayoutAchievement.Length)
		{
			GUIBase_Layout gUIBase_Layout = m_LayoutAchievement[index];
			if ((bool)gUIBase_Layout)
			{
				MFGuiManager.Instance.ShowLayout(gUIBase_Layout, show);
			}
		}
	}

	public void ShowAchievement(int id, float time = 3.5f)
	{
		showingID = id;
		hideRealTime = Time.realtimeSinceStartup + time;
		_ShowAchievement(showingID, true);
	}

	private IEnumerator DebugShowAllAchievements()
	{
		for (int i = 0; i < 13; i++)
		{
			ShowAchievement(i);
			yield return new WaitForSeconds(4f);
		}
	}
}
