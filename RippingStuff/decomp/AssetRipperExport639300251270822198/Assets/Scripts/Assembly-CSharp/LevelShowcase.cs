using System.Collections;
using UnityEngine;

public class LevelShowcase : MonoBehaviour
{
	private GUIBase_Pivot m_PivotMain;

	private GUIBase_Layout m_LayoutMain;

	private static string m_MainMenuDemoName = "MainMenuDemo";

	private GUIBase_Pivot m_PivotPause;

	private GUIBase_Layout m_LayoutPause;

	private float Left;

	private float Bottom;

	private float Right;

	private float Top;

	private bool exiting;

	private bool paused;

	private void Awake()
	{
	}

	private void Start()
	{
		Left = (float)Screen.width * 0.1f;
		Bottom = (float)Screen.height * 0.1f;
		Right = (float)Screen.width * 0.9f;
		Top = (float)Screen.height * 0.9f;
		MFGuiManager.Instance.SetFadeOut(1f);
		StartCoroutine(LateInit());
	}

	private bool IsInsidePauseArea(Touch touch)
	{
		if (touch.position.x > Left && touch.position.x < Right && touch.position.y > Bottom && touch.position.y < Top)
		{
			return true;
		}
		return false;
	}

	private IEnumerator LateInit()
	{
		yield return new WaitForSeconds(0.5f);
		m_PivotMain = MFGuiManager.Instance.GetPivot("Main");
		m_LayoutMain = GetLayout("LayoutMain", m_PivotMain);
		MFGuiManager.Instance.ShowPivot(m_PivotMain, true);
		MFGuiManager.Instance.ShowLayout(m_LayoutMain, true);
		m_PivotPause = MFGuiManager.Instance.GetPivot("IngameMenu");
		m_LayoutPause = GetLayout("Menu_Layout", m_PivotPause);
		PrepareButton(m_LayoutPause, "ResumeButton", null, ResumeButtonDelegate);
		PrepareButton(m_LayoutPause, "ExitDemoButton", null, ExitButtonDelegate);
		MFGuiManager.Instance.FadeIn(2f);
	}

	private GUIBase_Button PrepareButton(GUIBase_Layout layout, string name, GUIBase_Button.TouchDelegate touch, GUIBase_Button.ReleaseDelegate release)
	{
		GUIBase_Button gUIBase_Button = null;
		GUIBase_Widget widget = layout.GetWidget(name);
		if ((bool)widget)
		{
			gUIBase_Button = widget.GetComponent<GUIBase_Button>();
			if ((bool)gUIBase_Button)
			{
				gUIBase_Button.RegisterTouchDelegate(touch);
				gUIBase_Button.RegisterReleaseDelegate(release);
			}
		}
		return gUIBase_Button;
	}

	private GUIBase_Layout GetLayout(string layoutName, GUIBase_Pivot pivot)
	{
		if ((bool)pivot)
		{
			GUIBase_Layout[] componentsInChildren = pivot.GetComponentsInChildren<GUIBase_Layout>();
			GUIBase_Layout[] array = componentsInChildren;
			foreach (GUIBase_Layout gUIBase_Layout in array)
			{
				if (gUIBase_Layout.name == layoutName)
				{
					return gUIBase_Layout;
				}
			}
		}
		Debug.LogError("Can't find layout '" + layoutName + "'");
		return null;
	}

	private void ResumeButtonDelegate(bool inside)
	{
		if (inside)
		{
			paused = false;
			AudioListener.pause = false;
			Time.timeScale = 1f;
			m_PivotPause.Show(false);
		}
	}

	private void ExitButtonDelegate(bool inside)
	{
		if (inside && !exiting)
		{
			exiting = true;
			m_PivotPause.Show(false);
			Time.timeScale = 1f;
			StartCoroutine(ExitToMainMenu());
		}
	}

	private void PauseButtonDelegate()
	{
		if (!paused)
		{
			paused = true;
			m_PivotPause.Show(true);
			AudioListener.pause = true;
			Time.timeScale = 0f;
		}
	}

	private IEnumerator ExitToMainMenu()
	{
		Time.timeScale = 1f;
		MFGuiManager.Instance.FadeOut(0.5f);
		yield return new WaitForSeconds(0.6f);
		Application.LoadLevel(m_MainMenuDemoName);
	}

	private void Update()
	{
		if (exiting)
		{
			return;
		}
		bool flag = false;
		if (Input.GetKeyDown("escape"))
		{
			flag = true;
		}
		if (Input.touchCount != 0)
		{
			Touch touch = Input.GetTouch(0);
			if (touch.phase == TouchPhase.Began && IsInsidePauseArea(touch))
			{
				flag = true;
			}
		}
		if (flag)
		{
			PauseButtonDelegate();
		}
	}
}
