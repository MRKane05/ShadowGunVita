using System.Collections;
using UnityEngine;

[AddComponentMenu("GUI/Game/TimeBar")]
public class TimeBar : MonoBehaviour, IGameZoneControledObject
{
	public float timer = 10f;

	public string labelName;

	public bool countDown;

	public EventSender sendOnFinish = new EventSender();

	public EventTarget onlyOnEvent = new EventTarget();

	private float finishTime;

	private bool m_Initialised;

	private bool m_Running;

	private bool m_Suspended;

	private GUIBase_Pivot m_Pivot;

	private GUIBase_Layout m_Layout;

	private GUIBase_ProgressBar m_ProgressBar;

	private GUIBase_Widget m_LabelWidget;

	private static ArrayList Instancies = new ArrayList();

	private static void RegisterInstance(TimeBar s)
	{
		Instancies.Add(s);
	}

	private static void UnregisterInstance(TimeBar s)
	{
		Instancies.Remove(s);
	}

	public static void SuspendAllRunning(bool suspend)
	{
		foreach (TimeBar instancy in Instancies)
		{
			instancy.Suspend(suspend);
		}
	}

	public void Enable()
	{
		m_Suspended = false;
	}

	public void Disable()
	{
		if (m_Running)
		{
			if (m_Layout != null)
			{
				MFGuiManager.Instance.ShowLayout(m_Layout, false);
			}
			m_Running = false;
		}
	}

	public void Reset()
	{
		Disable();
		Enable();
	}

	private void Start()
	{
		onlyOnEvent.Initialize();
	}

	private void Awake()
	{
		RegisterInstance(this);
		onlyOnEvent.OnActivate = Activate;
		GameZone firstComponentUpward = base.gameObject.GetFirstComponentUpward<GameZone>();
		if (firstComponentUpward == null)
		{
			Debug.LogError("This object must be a child of GameZone: '" + base.name + "' (TimeBar Component)");
		}
		else
		{
			firstComponentUpward.RegisterControllableObject(this);
		}
	}

	private void OnDestroy()
	{
		UnregisterInstance(this);
	}

	private void LateInit()
	{
		m_Pivot = MFGuiManager.Instance.GetPivot("TimeBar");
		m_Layout = GuiBaseUtils.GetLayout("TimeBar_Layout", m_Pivot);
		m_ProgressBar = GuiBaseUtils.PrepareProgressBar(m_Layout, "Time_ProgressBar");
		m_LabelWidget = m_Layout.GetWidget(labelName, false);
		m_Initialised = true;
	}

	private void Activate()
	{
		if (!m_Initialised)
		{
			LateInit();
		}
		finishTime = Time.time + timer;
		m_Running = true;
		MFGuiManager.Instance.ShowLayout(m_Layout, true);
		if (m_LabelWidget != null)
		{
			MFGuiManager.Instance.ShowWidget(m_LabelWidget, true, true);
		}
	}

	private void Update()
	{
		if (m_Running && !m_Suspended)
		{
			float num = (finishTime - Time.time) / timer;
			if (countDown)
			{
				num = 1f - num;
			}
			m_ProgressBar.SetValue(num);
			if (Time.time >= finishTime)
			{
				sendOnFinish.Send();
				m_Running = false;
				MFGuiManager.Instance.ShowLayout(m_Layout, false);
			}
		}
	}

	private void Suspend(bool suspend)
	{
		if (m_Initialised && m_Running)
		{
			m_Layout.ShowImmediate(!suspend, false);
			if (m_LabelWidget != null)
			{
				m_LabelWidget.Show(!suspend, true);
			}
			m_Suspended = suspend;
		}
	}
}
