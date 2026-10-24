using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[AddComponentMenu("GUI/Menu/Sniff Singleton")]
public class GuiSniffMinigame : MonoBehaviour
{
	private enum E_NodeState
	{
		interactive = 0,
		normal = 1,
		highlight = 2
	}

	private enum E_BgState
	{
		disabled = 0,
		interactive = 1,
		fail = 2,
		success = 3
	}

	private enum E_State
	{
		WaitingToStart = 0,
		PlayingSequence = 1,
		ScanningCode = 2,
		Success = 3,
		Failed = 4,
		GameOver = 5
	}

	private class SymbolWidgets
	{
		public GUIBase_Button button;

		public GUIBase_Sprite normal;

		public GUIBase_Sprite highlight;
	}

	[Serializable]
	public class Config
	{
		public int codeLength = 4;

		public float displayTime = 0.55f;

		public float delayTime = 0.55f;
	}

	public class PrototypeConfig
	{
		public int sniffCost = 1;

		public int tryCost = 2;

		public int alertLimit = 6;
	}

	private class Code
	{
		public ArrayList sequence = new ArrayList();

		public void NewCode(int length)
		{
			sequence.Clear();
			ArrayList arrayList = new ArrayList();
			arrayList.AddRange(seed);
			for (int i = 0; i < length; i++)
			{
				int index = Random.Range(0, arrayList.Count - 1);
				sequence.Add(arrayList[index]);
				arrayList.RemoveAt(index);
			}
		}
	}

	private const float barAnimTime = 0.6f;

	private const float failureTime = 0.5f;

	private const float successTime = 1.2f;

	public static GuiSniffMinigame Instance;

	private static int[] seed = new int[9] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

	private SymbolWidgets[] symbolsWidgets;

	private GUIBase_Pivot m_PivotAll;

	private GUIBase_Pivot Main_Pivot;

	private GUIBase_Pivot BG_Pivot;

	private GUIBase_Layout m_LayoutDisabled;

	private GUIBase_Layout m_LayoutInteractive;

	private GUIBase_Layout m_LayoutFailure;

	private GUIBase_Layout m_LayoutSuccess;

	private GUIBase_Layout m_LayoutButtons;

	private GUIBase_Layout m_LayoutNodes;

	private GUIBase_Layout m_LayoutStart;

	private GUIBase_Layout m_LayoutHelp;

	private GUIBase_Layout m_LayoutGameOver;

	private GUIBase_Layout m_LayoutGameOver2;

	private GUIBase_Layout m_LayoutNumbers;

	private GUIBase_Button m_SniffButton;

	private GUIBase_Button m_RestartButton;

	private GUIBase_Button m_ExitButton;

	private GUIBase_Button m_StartButton;

	private GUIBase_Button m_GameOverButton;

	private GUIBase_Button m_GameOverButton2;

	private GUIBase_Button m_HelpButton;

	private GUIBase_Button m_HelpCloseButton;

	private GUIBase_Number m_EnteredNumber;

	private GUIBase_ProgressBar m_AlertBar;

	private Code code = new Code();

	private InteractionSniffMinigame useObject;

	private E_State m_State;

	private int pressedCount;

	private bool pressedValid;

	private int enteredNumbers;

	private int alertLevel;

	private bool helpScreenOn;

	private bool restartButtonOn;

	private ArrayList MenuItems = new ArrayList();

	private int MenuSelection = -1;

	private MenuController MenuCtrl = new MenuController();

	public PrototypeConfig prototypeCfg = new PrototypeConfig();

	public bool InProgress { get; private set; }

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
		m_PivotAll = MFGuiManager.Instance.GetPivot("CodeSniffer");
		if (!m_PivotAll)
		{
			Debug.LogError("Pivot 'CodeSniffer'  not found");
		}
		Main_Pivot = MFGuiManager.Instance.GetPivot("Main_Pivot");
		if (!Main_Pivot)
		{
			Debug.LogError("Pivot 'Main_Pivot'  not found");
		}
		BG_Pivot = MFGuiManager.Instance.GetPivot("BG_Pivot");
		if (!BG_Pivot)
		{
			Debug.LogError("Pivot 'BG_Pivot'  not found");
		}
		m_LayoutDisabled = GuiBaseUtils.GetLayout("BG_Disabled_Layout", m_PivotAll);
		m_LayoutInteractive = GuiBaseUtils.GetLayout("BG_Interactive_Layout", m_PivotAll);
		m_LayoutFailure = GuiBaseUtils.GetLayout("BG_Failure_Layout", m_PivotAll);
		m_LayoutSuccess = GuiBaseUtils.GetLayout("BG_Success_Layout", m_PivotAll);
		m_LayoutStart = GuiBaseUtils.GetLayout("Start_Layout", m_PivotAll);
		m_LayoutHelp = GuiBaseUtils.GetLayout("Help_Layout", m_PivotAll);
		m_LayoutGameOver = GuiBaseUtils.GetLayout("GameOver_Layout", m_PivotAll);
		m_LayoutGameOver2 = GuiBaseUtils.GetLayout("GameOver2_Layout", m_PivotAll);
		GuiBaseUtils.GetLayout("GFX_Layout", Main_Pivot);
		m_LayoutNumbers = GuiBaseUtils.GetLayout("Numbers_Layout", Main_Pivot);
		m_LayoutButtons = GuiBaseUtils.GetLayout("Buttons_Layout", Main_Pivot);
		m_LayoutNodes = GuiBaseUtils.GetLayout("Node_Sprites_Layout", Main_Pivot);
		m_LayoutNodes = GuiBaseUtils.GetLayout("Node_Sprites_Layout", Main_Pivot);
		m_EnteredNumber = GuiBaseUtils.PrepareNumber(m_LayoutNumbers, "SymbolNumber");
		m_AlertBar = GuiBaseUtils.PrepareProgressBar(m_LayoutNumbers, "AlertBar");
		RegisterSymbolsWidgets();
		m_ExitButton = GuiBaseUtils.RegisterButtonDelegate(m_LayoutButtons, "ExitButton", null, OnExitButton);
		m_SniffButton = GuiBaseUtils.RegisterButtonDelegate(m_LayoutButtons, "SniffButton", null, OnSniffButton);
		m_RestartButton = GuiBaseUtils.RegisterButtonDelegate(m_LayoutButtons, "RestartButton", null, OnRestartButton);
		m_StartButton = GuiBaseUtils.RegisterButtonDelegate(m_LayoutStart, "Button_Start", OnButtonStart, null);
		m_GameOverButton = GuiBaseUtils.RegisterButtonDelegate(m_LayoutGameOver, "Button_GameOver", OnButtonGameOver, null);
		m_GameOverButton2 = GuiBaseUtils.RegisterButtonDelegate(m_LayoutGameOver2, "Button_GameOver", OnButtonGameOver2, null);
		m_HelpCloseButton = GuiBaseUtils.RegisterButtonDelegate(m_LayoutHelp, "Button_Close_Help", OnButtonCloseHelp, null);
		m_HelpButton = GuiBaseUtils.RegisterButtonDelegate(m_LayoutButtons, "HelpButton", OnButtonHelp, null);
		InitMenuItems();
	}

	private void RegisterSymbolsWidgets()
	{
		symbolsWidgets = new SymbolWidgets[9];
		symbolsWidgets[0] = new SymbolWidgets();
		symbolsWidgets[0].button = GuiBaseUtils.RegisterButtonDelegate(m_LayoutButtons, "Node1", OnSymbol1, null);
		symbolsWidgets[0].normal = GuiBaseUtils.PrepareSprite(m_LayoutNodes, "Node1_Idle");
		symbolsWidgets[0].highlight = GuiBaseUtils.PrepareSprite(m_LayoutNodes, "Node1_Active");
		symbolsWidgets[1] = new SymbolWidgets();
		symbolsWidgets[1].button = GuiBaseUtils.RegisterButtonDelegate(m_LayoutButtons, "Node2", OnSymbol2, null);
		symbolsWidgets[1].normal = GuiBaseUtils.PrepareSprite(m_LayoutNodes, "Node2_Idle");
		symbolsWidgets[1].highlight = GuiBaseUtils.PrepareSprite(m_LayoutNodes, "Node2_Active");
		symbolsWidgets[2] = new SymbolWidgets();
		symbolsWidgets[2].button = GuiBaseUtils.RegisterButtonDelegate(m_LayoutButtons, "Node3", OnSymbol3, null);
		symbolsWidgets[2].normal = GuiBaseUtils.PrepareSprite(m_LayoutNodes, "Node3_Idle");
		symbolsWidgets[2].highlight = GuiBaseUtils.PrepareSprite(m_LayoutNodes, "Node3_Active");
		symbolsWidgets[3] = new SymbolWidgets();
		symbolsWidgets[3].button = GuiBaseUtils.RegisterButtonDelegate(m_LayoutButtons, "Node4", OnSymbol4, null);
		symbolsWidgets[3].normal = GuiBaseUtils.PrepareSprite(m_LayoutNodes, "Node4_Idle");
		symbolsWidgets[3].highlight = GuiBaseUtils.PrepareSprite(m_LayoutNodes, "Node4_Active");
		symbolsWidgets[4] = new SymbolWidgets();
		symbolsWidgets[4].button = GuiBaseUtils.RegisterButtonDelegate(m_LayoutButtons, "Node5", OnSymbol5, null);
		symbolsWidgets[4].normal = GuiBaseUtils.PrepareSprite(m_LayoutNodes, "Node5_Idle");
		symbolsWidgets[4].highlight = GuiBaseUtils.PrepareSprite(m_LayoutNodes, "Node5_Active");
		symbolsWidgets[5] = new SymbolWidgets();
		symbolsWidgets[5].button = GuiBaseUtils.RegisterButtonDelegate(m_LayoutButtons, "Node6", OnSymbol6, null);
		symbolsWidgets[5].normal = GuiBaseUtils.PrepareSprite(m_LayoutNodes, "Node6_Idle");
		symbolsWidgets[5].highlight = GuiBaseUtils.PrepareSprite(m_LayoutNodes, "Node6_Active");
		symbolsWidgets[6] = new SymbolWidgets();
		symbolsWidgets[6].button = GuiBaseUtils.RegisterButtonDelegate(m_LayoutButtons, "Node7", OnSymbol7, null);
		symbolsWidgets[6].normal = GuiBaseUtils.PrepareSprite(m_LayoutNodes, "Node7_Idle");
		symbolsWidgets[6].highlight = GuiBaseUtils.PrepareSprite(m_LayoutNodes, "Node7_Active");
		symbolsWidgets[7] = new SymbolWidgets();
		symbolsWidgets[7].button = GuiBaseUtils.RegisterButtonDelegate(m_LayoutButtons, "Node8", OnSymbol8, null);
		symbolsWidgets[7].normal = GuiBaseUtils.PrepareSprite(m_LayoutNodes, "Node8_Idle");
		symbolsWidgets[7].highlight = GuiBaseUtils.PrepareSprite(m_LayoutNodes, "Node8_Active");
		symbolsWidgets[8] = new SymbolWidgets();
		symbolsWidgets[8].button = GuiBaseUtils.RegisterButtonDelegate(m_LayoutButtons, "Node9", OnSymbol9, null);
		symbolsWidgets[8].normal = GuiBaseUtils.PrepareSprite(m_LayoutNodes, "Node9_Idle");
		symbolsWidgets[8].highlight = GuiBaseUtils.PrepareSprite(m_LayoutNodes, "Node9_Active");
	}

	private void InitMenuItems()
	{
		MenuItems = new ArrayList();
		SymbolWidgets[] array = symbolsWidgets;
		foreach (SymbolWidgets symbolWidgets in array)
		{
			MenuItems.Add(symbolWidgets.button);
		}
		MenuItems.Add(m_SniffButton);
		MenuItems.Add(m_RestartButton);
		MenuItems.Add(m_HelpButton);
		MenuItems.Add(m_ExitButton);
	}

	private void ShowHelp(bool show)
	{
		if (show)
		{
			StartCoroutine(SuspendHelp_Coroutine());
		}
		else
		{
			StartCoroutine(ResumeHelp_Coroutine());
		}
		MFGuiManager.Instance.ShowLayout(m_LayoutHelp, show);
		helpScreenOn = show;
	}

	public void Show(InteractionSniffMinigame fromUseObject)
	{
		useObject = fromUseObject;
		InProgress = true;
		m_State = E_State.WaitingToStart;
		Mission.Instance.ChangeGuiState(Mission.E_GuiState.E_GS_SNIFF_MINIGAME);
		GuiHUD.Instance.Hide();
		Screen.lockCursor = false;
		Player.Instance.StopMove(true);
		MFGuiManager.Instance.ShowPivot(Main_Pivot, true);
		MFGuiManager.Instance.ShowPivot(BG_Pivot, true);
		ShowStartButton(true);
		DisableSniffAndRestart(true);
		ShowBottomLineButtons(false);
		ResetInput();
		RegenereateCode();
		ResetSelectionButton();
		if (PlayerPrefs.GetInt("FirstTimeSniffMinigame", 0) == 0)
		{
			ShowHelp(true);
			PlayerPrefs.SetInt("FirstTimeSniffMinigame", 1);
			PlayerPrefs.Save();
		}
	}

	public void Hide()
	{
		StopAllCoroutines();
		MFGuiManager.Instance.ShowPivot(m_PivotAll, false);
		GuiHUD.Instance.Show();
		Screen.lockCursor = true;
		Player.Instance.StopMove(false);
		InProgress = false;
		Mission.Instance.ChangeGuiState(Mission.E_GuiState.E_GS_HUD);
	}

	public void SuspendAll()
	{
		StartCoroutine(SuspendAll_Coroutine());
	}

	private IEnumerator SuspendAll_Coroutine()
	{
		DisableAllButtons(true);
		if (helpScreenOn)
		{
			m_HelpCloseButton.SetDisabled(true);
		}
		float fadeTime = 0.4f;
		float beginFadeTime = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup - beginFadeTime <= fadeTime)
		{
			float currentFade = 1f - Mathf.Max((Time.realtimeSinceStartup - beginFadeTime) / fadeTime, 1f);
			m_PivotAll.m_FadeAlpha = currentFade;
			yield return new WaitForSeconds(0.05f);
		}
	}

	private IEnumerator SuspendHelp_Coroutine()
	{
		DisableAllButtons(true);
		float fadeTime = 0.4f;
		float beginFadeTime = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup - beginFadeTime <= fadeTime)
		{
			float currentFade = 1f - Mathf.Max((Time.realtimeSinceStartup - beginFadeTime) / fadeTime, 1f);
			Main_Pivot.m_FadeAlpha = currentFade;
			BG_Pivot.m_FadeAlpha = currentFade;
			m_LayoutStart.m_FadeAlpha = currentFade;
			m_LayoutGameOver.m_FadeAlpha = currentFade;
			m_LayoutGameOver2.m_FadeAlpha = currentFade;
			yield return new WaitForSeconds(0.05f);
		}
	}

	public void ResumeAll()
	{
		StartCoroutine(ResumeAll_Coroutine());
	}

	private IEnumerator ResumeAll_Coroutine()
	{
		float fadeTime = 0.4f;
		float beginFadeTime = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup - beginFadeTime <= fadeTime)
		{
			float currentFade = Mathf.Max((Time.realtimeSinceStartup - beginFadeTime) / fadeTime, 1f);
			m_PivotAll.m_FadeAlpha = currentFade;
			yield return new WaitForSeconds(0.05f);
		}
		if (helpScreenOn)
		{
			m_HelpCloseButton.SetDisabled(false);
		}
		if (m_State != E_State.Success && !helpScreenOn)
		{
			DisableAllButtons(false);
		}
		yield return new WaitForEndOfFrame();
		if (m_State == E_State.WaitingToStart || m_State == E_State.PlayingSequence || m_State == E_State.GameOver)
		{
			DisableSniffAndRestart(true);
		}
	}

	private IEnumerator ResumeHelp_Coroutine()
	{
		float fadeTime = 0.4f;
		float beginFadeTime = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup - beginFadeTime <= fadeTime)
		{
			float currentFade = Mathf.Max((Time.realtimeSinceStartup - beginFadeTime) / fadeTime, 1f);
			Main_Pivot.m_FadeAlpha = currentFade;
			BG_Pivot.m_FadeAlpha = currentFade;
			m_LayoutStart.m_FadeAlpha = currentFade;
			m_LayoutGameOver.m_FadeAlpha = currentFade;
			m_LayoutGameOver2.m_FadeAlpha = currentFade;
			yield return new WaitForSeconds(0.05f);
		}
		if (m_State != E_State.Success)
		{
			DisableAllButtons(false);
		}
		yield return new WaitForEndOfFrame();
		if (m_State == E_State.WaitingToStart || m_State == E_State.PlayingSequence || m_State == E_State.GameOver)
		{
			DisableSniffAndRestart(true);
		}
		if (m_State == E_State.WaitingToStart || m_State == E_State.GameOver)
		{
			ShowBottomLineButtons(false);
		}
	}

	private void RegenereateCode()
	{
		code.NewCode(useObject.config.codeLength);
		alertLevel = 0;
		AddAlertLevel(0, false);
	}

	private void OnButtonStart()
	{
		ShowStartButton(false);
		StartCoroutine(PlaySequence());
	}

	private void CheckEnteredCode()
	{
		if (pressedCount < useObject.config.codeLength)
		{
			return;
		}
		if (pressedValid)
		{
			m_State = E_State.Success;
			DisableAllButtons(true);
			StartCoroutine(ShowSuccess());
			return;
		}
		m_State = E_State.Failed;
		AddAlertLevel(prototypeCfg.tryCost, true);
		if (alertLevel >= prototypeCfg.alertLimit)
		{
			StartCoroutine(GameOver());
		}
		else
		{
			StartCoroutine(ShowFailure());
		}
	}

	private void UpdateAlertBar(float alert)
	{
		float num = (float)(prototypeCfg.alertLimit - prototypeCfg.tryCost) / (float)prototypeCfg.alertLimit;
		m_AlertBar.SetValue(alert);
		m_AlertBar.SetBarColor((!(alert >= num)) ? Color.white : Color.red);
	}

	private void AddAlertLevel(int alertPoints, bool animate)
	{
		alertLevel += alertPoints;
		if ((bool)m_AlertBar)
		{
			float num = (float)alertLevel / (float)prototypeCfg.alertLimit;
			if (animate)
			{
				StartCoroutine(AnimateBar(m_AlertBar.CurentValue, num, 0.6f));
			}
			else
			{
				UpdateAlertBar(num);
			}
		}
	}

	private IEnumerator AnimateBar(float fromVal, float toVal, float time)
	{
		float animTime = 0f;
		float curVal = fromVal;
		while (animTime < time)
		{
			animTime += Time.deltaTime;
			curVal = Mathfx.Hermite(fromVal, toVal, Mathf.Clamp(animTime / time, 0f, 1f));
			UpdateAlertBar(Mathf.Min(curVal, toVal));
			yield return new WaitForEndOfFrame();
		}
	}

	private void OnExitButton(bool inside)
	{
		if (inside)
		{
			useObject.OnExit();
			Hide();
		}
	}

	private void OnSniffButton(bool inside)
	{
		if (inside)
		{
			AddAlertLevel(prototypeCfg.sniffCost, true);
			ResetInput();
			StartCoroutine(PlaySequence());
		}
	}

	private void OnSymbol1()
	{
		SymbolPressed(1);
	}

	private void OnSymbol2()
	{
		SymbolPressed(2);
	}

	private void OnSymbol3()
	{
		SymbolPressed(3);
	}

	private void OnSymbol4()
	{
		SymbolPressed(4);
	}

	private void OnSymbol5()
	{
		SymbolPressed(5);
	}

	private void OnSymbol6()
	{
		SymbolPressed(6);
	}

	private void OnSymbol7()
	{
		SymbolPressed(7);
	}

	private void OnSymbol8()
	{
		SymbolPressed(8);
	}

	private void OnSymbol9()
	{
		SymbolPressed(9);
	}

	private void SymbolPressed(int id)
	{
		if (pressedCount < code.sequence.Count)
		{
			int num = (int)code.sequence[pressedCount];
			if (num != id)
			{
				pressedValid = false;
			}
			pressedCount++;
			enteredNumbers *= 10;
			enteredNumbers += id;
			UpdateEnteredSybmols();
			ShowNode(symbolsWidgets[id - 1], E_NodeState.highlight);
			CheckEnteredCode();
		}
	}

	private void ResetInput()
	{
		pressedValid = true;
		pressedCount = 0;
		enteredNumbers = 0;
		UpdateEnteredSybmols();
	}

	private void UpdateEnteredSybmols()
	{
		m_EnteredNumber.SetNumber(enteredNumbers, 999999999);
	}

	private void ShowAllNodes(E_NodeState state)
	{
		SymbolWidgets[] array = symbolsWidgets;
		foreach (SymbolWidgets sw in array)
		{
			ShowNode(sw, state);
		}
	}

	private void DisableAllButtons(bool disable)
	{
		SymbolWidgets[] array = symbolsWidgets;
		foreach (SymbolWidgets symbolWidgets in array)
		{
			symbolWidgets.button.SetDisabled(disable);
		}
		m_SniffButton.SetDisabled(disable);
		m_RestartButton.SetDisabled(disable);
		m_ExitButton.SetDisabled(disable);
		m_StartButton.SetDisabled(disable);
		m_GameOverButton.SetDisabled(disable);
		m_GameOverButton2.SetDisabled(disable);
		m_HelpButton.SetDisabled(disable);
	}

	private void ShowNode(SymbolWidgets sw, E_NodeState state)
	{
		MFGuiManager.Instance.ShowWidget(sw.button.Widget, state == E_NodeState.interactive, true);
		MFGuiManager.Instance.ShowWidget(sw.normal.Widget, state == E_NodeState.normal, true);
		MFGuiManager.Instance.ShowWidget(sw.highlight.Widget, state == E_NodeState.highlight, true);
	}

	private IEnumerator PlaySequence()
	{
		m_State = E_State.PlayingSequence;
		ShowAllNodes(E_NodeState.normal);
		ShowBackground(E_BgState.disabled);
		ShowAllNodes(E_NodeState.normal);
		DisableSniffAndRestart(true);
		m_SniffButton.ForceHighlight(true);
		ShowBottomLineButtons(false);
		yield return new WaitForSeconds(0.5f);
		for (int i = 0; i < code.sequence.Count; i++)
		{
			int symbol = (int)code.sequence[i];
			StartCoroutine(ShowSymbol(symbol));
			if (i < code.sequence.Count - 1)
			{
				yield return new WaitForSeconds(useObject.config.delayTime);
			}
		}
		yield return new WaitForSeconds(useObject.config.displayTime);
		ShowAllNodes(E_NodeState.interactive);
		ShowBackground(E_BgState.interactive);
		DisableSniffAndRestart(false);
		m_SniffButton.ForceHighlight(false);
		ShowBottomLineButtons(true);
		if (alertLevel + prototypeCfg.sniffCost >= prototypeCfg.alertLimit)
		{
			ShowRestartButton(true);
		}
		ResetSelectionButton();
		m_State = E_State.ScanningCode;
	}

	private IEnumerator ShowSymbol(int id)
	{
		ShowNode(symbolsWidgets[id - 1], E_NodeState.highlight);
		MFGuiManager.Instance.PlayOneShot(symbolsWidgets[id - 1].button.buttonDownSoundIn);
		yield return new WaitForSeconds(useObject.config.displayTime);
		ShowNode(symbolsWidgets[id - 1], E_NodeState.normal);
	}

	private IEnumerator ShowFailure()
	{
		ShowBackground(E_BgState.fail);
		yield return new WaitForSeconds(0.5f);
		ResetInput();
		StartCoroutine(PlaySequence());
	}

	private IEnumerator GameOver()
	{
		ShowBackground(E_BgState.fail);
		ShowAllNodes(E_NodeState.normal);
		ShowRestartButton(false);
		DisableSniffAndRestart(true);
		ShowBottomLineButtons(false);
		useObject.OnFailure();
		m_State = E_State.GameOver;
		yield return new WaitForSeconds(0.5f);
		ShowGameOverButton(true);
	}

	private IEnumerator ShowSuccess()
	{
		ShowBackground(E_BgState.success);
		yield return new WaitForSeconds(1.2f);
		useObject.OnSuccess();
		Hide();
	}

	private void ShowBackground(E_BgState state)
	{
		MFGuiManager.Instance.ShowLayout(m_LayoutDisabled, state == E_BgState.disabled);
		MFGuiManager.Instance.ShowLayout(m_LayoutInteractive, state == E_BgState.interactive);
		MFGuiManager.Instance.ShowLayout(m_LayoutFailure, state == E_BgState.fail);
		MFGuiManager.Instance.ShowLayout(m_LayoutSuccess, state == E_BgState.success);
	}

	private void ShowRestartButton(bool show)
	{
		restartButtonOn = show;
		MFGuiManager.Instance.ShowWidget(m_RestartButton.Widget, show, true);
		MFGuiManager.Instance.ShowWidget(m_SniffButton.Widget, !show, true);
	}

	private void OnRestartButton(bool inside)
	{
		if (inside)
		{
			RegenereateCode();
			ResetInput();
			ShowRestartButton(false);
			StartCoroutine(PlaySequence());
		}
	}

	private void OnButtonGameOver()
	{
		RegenereateCode();
		ResetInput();
		DisableSniffAndRestart(true);
		ShowGameOverButton(false);
		StartCoroutine(PlaySequence());
	}

	private void OnButtonGameOver2()
	{
		useObject.OnExit();
		Hide();
	}

	private void OnButtonHelp()
	{
		ShowHelp(true);
	}

	private void OnButtonCloseHelp()
	{
		ShowHelp(false);
	}

	private void ShowStartButton(bool show)
	{
		MFGuiManager.Instance.ShowLayout(m_LayoutStart, show);
	}

	private void ShowGameOverButton(bool show)
	{
		if (useObject.DisableOnFail)
		{
			MFGuiManager.Instance.ShowLayout(m_LayoutGameOver2, show);
		}
		else
		{
			MFGuiManager.Instance.ShowLayout(m_LayoutGameOver, show);
		}
	}

	private void DisableSniffAndRestart(bool disable)
	{
		m_SniffButton.SetDisabled(disable);
		m_RestartButton.SetDisabled(disable);
	}

	public static void ResetFirstTimeTutorial()
	{
		PlayerPrefs.SetInt("FirstTimeSniffMinigame", 0);
		PlayerPrefs.Save();
	}

	private void ShowBottomLineButtons(bool show)
	{
		m_SniffButton.Widget.Show(show && !restartButtonOn, true);
		m_RestartButton.Widget.Show(show && restartButtonOn, true);
		m_ExitButton.Widget.Show(show, true);
		m_HelpButton.Widget.Show(show, true);
	}

	private void LateUpdate()
	{
		if (!InProgress)
		{
			return;
		}
		MenuCtrl.Update();
		if (helpScreenOn)
		{
			if (MenuCtrl.PressedOk() && !m_HelpCloseButton.IsDisabled())
			{
				m_HelpCloseButton.Callback(GUIBase_Callback.E_CallbackType.E_CT_ON_TOUCH_BEGIN);
			}
		}
		else if (m_State == E_State.WaitingToStart)
		{
			if (MenuCtrl.PressedOk() && (bool)m_StartButton && !m_StartButton.IsDisabled())
			{
				m_StartButton.Callback(GUIBase_Callback.E_CallbackType.E_CT_ON_TOUCH_BEGIN);
			}
		}
		else
		{
			if (m_State == E_State.Success || m_State == E_State.Failed)
			{
				return;
			}
			if (m_State == E_State.GameOver)
			{
				if (MenuCtrl.PressedOk())
				{
					if (m_LayoutGameOver.IsVisible() && !m_GameOverButton.IsDisabled())
					{
						m_GameOverButton.Callback(GUIBase_Callback.E_CallbackType.E_CT_ON_TOUCH_BEGIN);
					}
					else if (m_LayoutGameOver2.IsVisible() && !m_GameOverButton2.IsDisabled())
					{
						m_GameOverButton2.Callback(GUIBase_Callback.E_CallbackType.E_CT_ON_TOUCH_BEGIN);
					}
				}
			}
			else if (MenuCtrl.PressedLeft())
			{
				ChangeFocusLeft();
			}
			else if (MenuCtrl.PressedRight())
			{
				ChangeFocusRight();
			}
			else if (MenuCtrl.PressedUp())
			{
				ChangeFocusUp();
			}
			else if (MenuCtrl.PressedDown())
			{
				ChangeFocusDown();
			}
			else if (MenuCtrl.PressedOk())
			{
				PressFocusButton();
			}
			else if (MenuCtrl.PressedBack())
			{
				PressBackButton();
			}
		}
	}

	private void ChangeFocusLeft()
	{
		if (MenuSelection == -1)
		{
			HighlightDefaultButton();
		}
		else if (MenuSelection > 0)
		{
			int num = MenuSelection - 1;
			while (!IsMenuButtonEnabled(num) && num > 0)
			{
				num--;
			}
			if (IsMenuButtonEnabled(num))
			{
				UpdateSelection(num);
			}
		}
	}

	private void ChangeFocusRight()
	{
		if (MenuSelection == -1)
		{
			HighlightDefaultButton();
			return;
		}
		int num = MenuItems.Count - 1;
		if (MenuSelection < num)
		{
			int i;
			for (i = MenuSelection + 1; !IsMenuButtonEnabled(i) && i < num; i++)
			{
			}
			if (IsMenuButtonEnabled(i))
			{
				UpdateSelection(i);
			}
		}
	}

	private void ChangeFocusUp()
	{
		if (MenuSelection == -1)
		{
			HighlightDefaultButton();
		}
		else if (MenuSelection >= 3)
		{
			int num = ((MenuSelection > 9) ? (MenuSelection - 4) : (MenuSelection - 3));
			while (!IsMenuButtonEnabled(num) && num > 0)
			{
				num--;
			}
			if (IsMenuButtonEnabled(num))
			{
				UpdateSelection(num);
			}
		}
	}

	private void ChangeFocusDown()
	{
		if (MenuSelection == -1)
		{
			HighlightDefaultButton();
		}
		else if (MenuSelection <= 8)
		{
			int i = ((MenuSelection >= 7) ? (MenuSelection + 4) : (MenuSelection + 3));
			for (int num = MenuItems.Count - 1; !IsMenuButtonEnabled(i) && i < num; i++)
			{
			}
			if (IsMenuButtonEnabled(i))
			{
				UpdateSelection(i);
			}
		}
	}

	private void PressFocusButton()
	{
		if (MenuSelection == -1)
		{
			HighlightDefaultButton();
			return;
		}
		GUIBase_Button currentMenuItem = GetCurrentMenuItem(MenuSelection);
		if ((bool)currentMenuItem && !currentMenuItem.IsDisabled() && currentMenuItem.Widget.IsVisible())
		{
			currentMenuItem.Callback(GUIBase_Callback.E_CallbackType.E_CT_ON_TOUCH_BEGIN);
			currentMenuItem.Callback(GUIBase_Callback.E_CallbackType.E_CT_ON_TOUCH_END);
		}
	}

	private void PressBackButton()
	{
	}

	private GUIBase_Button GetCurrentMenuItem(int index)
	{
		if (index < 0 || index >= MenuItems.Count)
		{
			return null;
		}
		return (GUIBase_Button)MenuItems[index];
	}

	private void ResetSelectionButton()
	{
		MenuSelection = -1;
	}

	private void HighlightDefaultButton()
	{
		MenuSelection = 0;
		GUIBase_Button currentMenuItem = GetCurrentMenuItem(MenuSelection);
		if ((bool)currentMenuItem)
		{
			currentMenuItem.Callback(GUIBase_Callback.E_CallbackType.E_CT_ON_MOUSEOVER_BEGIN);
		}
	}

	private void UpdateSelection(int newSel)
	{
		GUIBase_Button currentMenuItem = GetCurrentMenuItem(MenuSelection);
		if ((bool)currentMenuItem)
		{
			currentMenuItem.Callback(GUIBase_Callback.E_CallbackType.E_CT_ON_MOUSEOVER_END);
		}
		currentMenuItem = GetCurrentMenuItem(newSel);
		if ((bool)currentMenuItem)
		{
			currentMenuItem.Callback(GUIBase_Callback.E_CallbackType.E_CT_ON_MOUSEOVER_BEGIN);
		}
		MenuSelection = newSel;
	}

	private bool IsMenuButtonEnabled(int newSel)
	{
		GUIBase_Button currentMenuItem = GetCurrentMenuItem(newSel);
		if (!currentMenuItem || currentMenuItem.IsDisabled() || !currentMenuItem.Widget.IsVisible())
		{
			return false;
		}
		return true;
	}
}
