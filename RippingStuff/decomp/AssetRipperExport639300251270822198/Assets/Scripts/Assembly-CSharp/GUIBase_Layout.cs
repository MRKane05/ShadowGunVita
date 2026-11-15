using UnityEngine;

[AddComponentMenu("GUI/Hiearchy/Layout")]
public class GUIBase_Layout : MonoBehaviour
{
	public enum E_FocusChangeDir
	{
		E_FCD_RIGHT = 0,
		E_FCD_LEFT = 1,
		E_FCD_UP = 2,
		E_FCD_DOWN = 3
	}

	public delegate int FocusDelegate(int currentFocusID, E_FocusChangeDir dir);

	public delegate void LayoutTouchDelegate();

	public AnimationClip m_InAnimation;

	public AnimationClip m_OutAnimation;

	public AudioClip m_InSound;

	public AudioClip m_OutSound;

	public float m_FadeAlpha = 1f;

	public int m_LayoutLayer;

	private MFGuiManager m_GuiManager;

	private bool m_IsVisible;

	private bool m_Initialized;

	private int m_FingerId = -1;

	private GUIBase_Widget[] m_Widgets;

	private GUIBase_Widget[] m_TouchSensitiveWidgets;

	private GUIBase_Widget m_TouchedWidget;

	private GUIBase_Widget m_MouseOverWidget;

	private Vector2 m_LayoutScale = default(Vector2);

	private Vector2 m_PlatformSize = default(Vector2);

	private Animation m_Anim;

	private bool m_IsPlayingAnimation;

	private GUIBase_Pivot m_ParentPivot;

	private int MAX_LAYERS = 10;

	private int m_LayoutUniqueId;

	private static int ms_LayoutUniqueIdCnt = 1;

	private LayoutTouchDelegate m_LayoutTouchDelegate;

	public bool ShowDone
	{
		get
		{
			return !m_IsPlayingAnimation && m_IsVisible;
		}
	}

	public bool HideDone
	{
		get
		{
			return !m_IsPlayingAnimation && !m_IsVisible;
		}
	}

	private GUIBase_Layout()
	{
		m_LayoutUniqueId = ms_LayoutUniqueIdCnt++;
	}

	public bool FingerIdInUse(int fingerId)
	{
		return m_FingerId == fingerId;
	}

	public void Start()
	{
		m_GuiManager = MFGuiManager.Instance;
		if ((bool)m_GuiManager)
		{
			m_GuiManager.RegisterLayout(this);
			m_Initialized = false;
			m_Anim = GetComponent<Animation>();
			if (!m_Anim)
			{
				Debug.LogError("Missing Animation component.");
			}
			m_IsPlayingAnimation = false;
			PrepareParent();
		}
		else
		{
			Debug.LogError("GuiManager prefab is not present!");
		}
	}

	private void OnDestroy()
	{
		m_InAnimation = null;
		m_OutAnimation = null;
		m_InSound = null;
		m_OutSound = null;
	}

	public void RegisterFocusDelegate(FocusDelegate f)
	{
	}

	public void RegisterLayoutTouchDelegate(LayoutTouchDelegate d)
	{
		m_LayoutTouchDelegate = d;
	}

	public void SetPlatformSize(int width, int height, float scaleX, float scaleY)
	{
		m_PlatformSize.x = width;
		m_PlatformSize.y = height;
		m_LayoutScale.x = scaleX;
		m_LayoutScale.y = scaleY;
	}

	public void GUIUpdate(float parentAlpha)
	{
		if (m_Initialized)
		{
			if (m_IsVisible)
			{
				float parentAlpha2 = m_FadeAlpha * parentAlpha;
				GUIBase_Widget[] widgets = m_Widgets;
				foreach (GUIBase_Widget gUIBase_Widget in widgets)
				{
					gUIBase_Widget.GUIUpdate(parentAlpha2);
				}
			}
		}
		else
		{
			LateInit();
		}
		UpdateGUIInput();
	}

	private void LateInit()
	{
		m_Widgets = GetComponentsInChildren<GUIBase_Widget>();
		if (m_Widgets != null && m_Widgets.Length != 0)
		{
			int num = 0;
			GUIBase_Widget[] array = new GUIBase_Widget[m_Widgets.Length];
			for (int i = 0; i < MAX_LAYERS; i++)
			{
				GUIBase_Widget[] widgets = m_Widgets;
				foreach (GUIBase_Widget gUIBase_Widget in widgets)
				{
					if ((bool)gUIBase_Widget && gUIBase_Widget.m_GuiWidgetLayer == i)
					{
						gUIBase_Widget.Initialization(this, m_LayoutScale);
						if (gUIBase_Widget.IsTouchSensitive())
						{
							array[num] = gUIBase_Widget;
							num++;
						}
					}
				}
			}
			if (num != 0)
			{
				m_TouchSensitiveWidgets = new GUIBase_Widget[num];
				for (int k = 0; k < num; k++)
				{
					m_TouchSensitiveWidgets[k] = array[k];
				}
			}
		}
		m_Initialized = true;
	}

	private void UpdateGUIInput()
	{
		if (m_TouchSensitiveWidgets == null || m_TouchSensitiveWidgets.Length == 0)
		{
			return;
		}
		GUIBase_Widget.E_TouchPhase e_TouchPhase = GUIBase_Widget.E_TouchPhase.E_TP_NONE;
		Vector2 eventPos = default(Vector2);
		bool mouse = false;
		if (m_IsPlayingAnimation)
		{
			return;
		}
		if (Input.touchCount != 0)
		{
			if (m_FingerId == -1)
			{
				for (int i = 0; i < Input.touchCount; i++)
				{
					Touch touch = Input.GetTouch(i);
					if (touch.phase == TouchPhase.Began)
					{
						m_FingerId = touch.fingerId;
						eventPos.x = touch.position.x;
						eventPos.y = touch.position.y;
						e_TouchPhase = GUIBase_Widget.E_TouchPhase.E_TP_CLICK_BEGIN;
						HandleGuiInputEvent(e_TouchPhase, eventPos, mouse);
						break;
					}
				}
				return;
			}
			for (int j = 0; j < Input.touchCount; j++)
			{
				Touch touch2 = Input.GetTouch(j);
				if (touch2.fingerId != m_FingerId)
				{
					continue;
				}
				if (touch2.phase == TouchPhase.Ended)
				{
					eventPos.x = touch2.position.x;
					eventPos.y = touch2.position.y;
					e_TouchPhase = GUIBase_Widget.E_TouchPhase.E_TP_CLICK_RELEASE;
					HandleGuiInputEvent(e_TouchPhase, eventPos, mouse);
					if (m_FingerId != -1)
					{
						Debug.LogError("GUI error: fingerId still assigned after release: " + m_FingerId);
						m_FingerId = -1;
					}
				}
				break;
			}
		}
		else if (Input.GetMouseButtonDown(0))
		{
			e_TouchPhase = GUIBase_Widget.E_TouchPhase.E_TP_CLICK_BEGIN;
			eventPos.x = Input.mousePosition.x;
			eventPos.y = Input.mousePosition.y;
			mouse = true;
			HandleGuiInputEvent(e_TouchPhase, eventPos, mouse);
		}
		else if (Input.GetMouseButtonUp(0))
		{
			e_TouchPhase = GUIBase_Widget.E_TouchPhase.E_TP_CLICK_RELEASE;
			eventPos.x = Input.mousePosition.x;
			eventPos.y = Input.mousePosition.y;
			mouse = true;
			HandleGuiInputEvent(e_TouchPhase, eventPos, mouse);
		}
	}

	public Vector2 ScreenPosToLayoutSpace(Vector2 pos)
	{
		return new Vector2(pos.x / m_LayoutScale.x, m_PlatformSize.y - pos.y / m_LayoutScale.y);
	}

	public Vector2 ScreenDeltaToLayoutSpace(Vector2 offset)
	{
		return new Vector2(offset.x / m_LayoutScale.x, offset.y / m_LayoutScale.y);
	}

	private void HandleGuiInputEvent(GUIBase_Widget.E_TouchPhase touchPhase, Vector2 eventPos, bool mouse)
	{
		if (m_TouchSensitiveWidgets == null)
		{
			return;
		}
		eventPos.x /= m_LayoutScale.x;
		eventPos.y = m_PlatformSize.y - eventPos.y / m_LayoutScale.y;
		switch (touchPhase)
		{
		case GUIBase_Widget.E_TouchPhase.E_TP_CLICK_BEGIN:
		{
			m_TouchedWidget = null;
			GUIBase_Widget[] touchSensitiveWidgets2 = m_TouchSensitiveWidgets;
			foreach (GUIBase_Widget gUIBase_Widget2 in touchSensitiveWidgets2)
			{
				if ((bool)gUIBase_Widget2 && gUIBase_Widget2.IsVisible() && gUIBase_Widget2.IsMouseOver(eventPos) && gUIBase_Widget2.HandleTouchEvent(touchPhase))
				{
					m_TouchedWidget = gUIBase_Widget2;
					m_MouseOverWidget = gUIBase_Widget2;
					break;
				}
			}
			if (m_TouchedWidget == null)
			{
				m_FingerId = -1;
				if (m_IsVisible && m_LayoutTouchDelegate != null)
				{
					m_LayoutTouchDelegate();
				}
			}
			break;
		}
		case GUIBase_Widget.E_TouchPhase.E_TP_CLICK_RELEASE:
			if ((bool)m_TouchedWidget && m_TouchedWidget.IsVisible())
			{
				m_TouchedWidget.HandleTouchEvent(touchPhase, m_TouchedWidget.IsMouseOver(eventPos));
			}
			m_TouchedWidget = null;
			m_FingerId = -1;
			break;
		case GUIBase_Widget.E_TouchPhase.E_TP_NONE:
		{
			if (mouse && (bool)m_MouseOverWidget)
			{
				if (m_MouseOverWidget.IsVisible())
				{
					if (!m_MouseOverWidget.IsMouseOver(eventPos))
					{
						m_MouseOverWidget.HandleTouchEvent(GUIBase_Widget.E_TouchPhase.E_TP_MOUSEOVER_END);
						m_MouseOverWidget = null;
					}
				}
				else
				{
					m_MouseOverWidget = null;
				}
			}
			if (!mouse || (bool)m_MouseOverWidget)
			{
				break;
			}
			GUIBase_Widget[] touchSensitiveWidgets = m_TouchSensitiveWidgets;
			foreach (GUIBase_Widget gUIBase_Widget in touchSensitiveWidgets)
			{
				if ((bool)gUIBase_Widget && gUIBase_Widget.IsVisible() && gUIBase_Widget.IsMouseOver(eventPos))
				{
					gUIBase_Widget.HandleTouchEvent(GUIBase_Widget.E_TouchPhase.E_TP_MOUSEOVER_BEGIN);
					m_MouseOverWidget = gUIBase_Widget;
					break;
				}
			}
			break;
		}
		}
	}

	public void ShowWidget(string widgetID, bool v)
	{
		GUIBase_Widget[] widgets = m_Widgets;
		foreach (GUIBase_Widget gUIBase_Widget in widgets)
		{
			if (gUIBase_Widget.name == widgetID)
			{
				gUIBase_Widget.Show(v, false);
				break;
			}
		}
	}

	public GUIBase_Widget GetWidget(string wName, bool reportError = true)
	{
		if (m_Widgets != null)
		{
			GUIBase_Widget[] widgets = m_Widgets;
			foreach (GUIBase_Widget gUIBase_Widget in widgets)
			{
				if (gUIBase_Widget.name == wName)
				{
					return gUIBase_Widget;
				}
			}
		}
		if (reportError)
		{
			Debug.LogError("Cant find '" + wName + "' in layout '" + base.name + "' ");
		}
		return null;
	}

	public void RegisterButtonTouchDelegate(string widgetID, GUIBase_Button.TouchDelegate f)
	{
		if (m_Widgets == null)
		{
			return;
		}
		GUIBase_Widget[] widgets = m_Widgets;
		foreach (GUIBase_Widget gUIBase_Widget in widgets)
		{
			if (gUIBase_Widget.name == widgetID)
			{
				GUIBase_Button component = gUIBase_Widget.GetComponent<GUIBase_Button>();
				if ((bool)component)
				{
					component.RegisterTouchDelegate(f);
				}
				break;
			}
		}
	}

	public void ShowImmediate(bool showFlag, bool playAnimAndSound = true)
	{
		if (showFlag)
		{
			m_IsVisible = true;
			if ((bool)m_InAnimation && playAnimAndSound)
			{
				StopAnim(m_Anim);
				m_Anim.clip = m_InAnimation;
				PlayAnim(m_Anim, null, LayoutAnimFinished, 0);
				m_IsPlayingAnimation = true;
			}
			if ((bool)m_InSound && playAnimAndSound)
			{
				MFGuiManager.Instance.PlayOneShot(m_InSound);
			}
			if (m_Widgets != null)
			{
				GUIBase_Widget[] widgets = m_Widgets;
				foreach (GUIBase_Widget gUIBase_Widget in widgets)
				{
					gUIBase_Widget.ShowImmediate(gUIBase_Widget.m_VisibleOnLayoutShow || gUIBase_Widget.IsVisible(), false);
				}
			}
			return;
		}
		if ((bool)m_OutAnimation && playAnimAndSound)
		{
			StopAnim(m_Anim);
			m_Anim.clip = m_OutAnimation;
			PlayAnim(m_Anim, null, LayoutAnimFinished, 1);
			m_IsPlayingAnimation = true;
		}
		if ((bool)m_OutSound && playAnimAndSound)
		{
			MFGuiManager.Instance.PlayOneShot(m_OutSound);
		}
		if ((bool)m_OutAnimation && playAnimAndSound)
		{
			return;
		}
		m_IsVisible = false;
		if (m_Widgets != null)
		{
			GUIBase_Widget[] widgets2 = m_Widgets;
			foreach (GUIBase_Widget gUIBase_Widget2 in widgets2)
			{
				gUIBase_Widget2.ShowImmediate(false, false);
			}
		}
	}

	public bool IsVisible()
	{
		return m_IsVisible;
	}

	public int GetUniqueId()
	{
		return m_LayoutUniqueId;
	}

	public void PlayAnim(Animation animation, GUIBase_Widget widget, GUIBase_Platform.AnimFinishedDelegate finishDelegate = null, int customIdx = -1)
	{
		m_GuiManager.GetPlatform(this).PlayAnim(animation, widget, finishDelegate, customIdx);
	}

	public void StopAnim(Animation animation)
	{
		m_GuiManager.GetPlatform(this).StopAnim(animation);
	}

	public void PlayAnimClip(AnimationClip clip)
	{
		m_Anim.clip = clip;
		PlayAnim(m_Anim, null);
	}

	public void StopCurrentAnimClip()
	{
		StopAnim(m_Anim);
	}

	private void LayoutAnimFinished(int idx)
	{
		switch ((GUIBase_Platform.E_SpecialAnimIdx)idx)
		{
		case GUIBase_Platform.E_SpecialAnimIdx.E_SAI_INANIM:
			m_IsPlayingAnimation = false;
			break;
		case GUIBase_Platform.E_SpecialAnimIdx.E_SAI_OUTANIM:
			m_IsPlayingAnimation = false;
			m_IsVisible = false;
			if (m_Widgets != null)
			{
				GUIBase_Widget[] widgets = m_Widgets;
				foreach (GUIBase_Widget gUIBase_Widget in widgets)
				{
					gUIBase_Widget.Show(false, false);
				}
			}
			break;
		}
	}

	public float GetParentFadeAlpha()
	{
		if ((bool)m_ParentPivot)
		{
			return m_ParentPivot.GetFadeAlpha(true);
		}
		return 1f;
	}

	public float GetFadeAlpha(bool recursive)
	{
		return m_FadeAlpha * GetParentFadeAlpha();
	}

	private void PrepareParent()
	{
		Transform parent = base.gameObject.transform.parent;
		GameObject gameObject = parent.gameObject;
		if ((bool)gameObject)
		{
			m_ParentPivot = gameObject.GetComponent<GUIBase_Pivot>();
		}
	}

	public void ReleaseFinger(Touch touch)
	{
		if (touch.fingerId == m_FingerId)
		{
			HandleGuiInputEvent(GUIBase_Widget.E_TouchPhase.E_TP_CLICK_RELEASE, touch.position, false);
			m_FingerId = -1;
		}
	}
}
