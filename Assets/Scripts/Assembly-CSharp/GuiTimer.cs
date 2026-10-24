using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("GUI/Game/Timer")]
[RequireComponent(typeof(AudioSource))]
public class GuiTimer : MonoBehaviour, IGameZoneControledObject
{
	[Serializable]
	public class WarningSetting
	{
		public float RemainingTime;

		public AnimationClip Anim;

		public AudioClip Sound;
	}

	public int minutes = 1;

	public int seconds = 30;

	public bool countDown = true;

	public List<WarningSetting> warnings = new List<WarningSetting>();

	public AudioSource Audio;

	public EventTarget onEvent = new EventTarget();

	public EventSender sendOnFinish = new EventSender();

	private float finishTime;

	private float lastUpdateTime;

	private bool m_Initialised;

	private bool m_Running;

	private bool m_Suspended;

	private GUIBase_Pivot m_Pivot;

	private GUIBase_Layout m_Layout;

	private GUIBase_Number m_NumberMinutes;

	private GUIBase_Number m_NumberSeconds;

	private static ArrayList Instancies = new ArrayList();

	public Color m_TextColor;

	public Font m_SubtitleFont;

	private GUIStyle m_SubtitleStyle;

	private GUIContent m_CurrentContent;

	private static void RegisterInstance(GuiTimer s)
	{
		Instancies.Add(s);
	}

	private static void UnregisterInstance(GuiTimer s)
	{
		Instancies.Remove(s);
	}

	public static void SuspendAllRunning(bool suspend)
	{
		foreach (GuiTimer instancy in Instancies)
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
			Audio.Stop();
			m_Running = false;
			lastUpdateTime = 0f;
			if (m_Layout != null)
			{
				StartCoroutine(StopAnimAndHide());
			}
		}
	}

	public void Reset()
	{
		Disable();
		Enable();
	}

	private void Start()
	{
		onEvent.Initialize();
		minutes = Mathf.Clamp(minutes, 0, 99);
		seconds = Mathf.Clamp(seconds, 0, 60);
	}

	private void RemainingMinutesAndSeconds(out int min, out int sec)
	{
		float num = finishTime - Time.time;
		min = Mathf.Clamp((int)num / 60, 0, 99);
		sec = Mathf.Clamp((int)num % 60, 0, 60);
	}

	private void Awake()
	{
		RegisterInstance(this);
		Audio = base.GetComponent<AudioSource>();
		if (Audio == null)
		{
			Debug.LogWarning("Requires AudioSource component for playing audio!");
		}
		Audio.playOnAwake = false;
		onEvent.OnActivate = OnActivate;
		onEvent.OnDeactivate = OnDeactivate;
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
		m_Pivot = MFGuiManager.Instance.GetPivot("Timer");
		m_Layout = GuiBaseUtils.GetLayout("Timer_Layout", m_Pivot);
		m_NumberMinutes = GuiBaseUtils.PrepareNumber(m_Layout, "Timer_Minutes");
		m_NumberSeconds = GuiBaseUtils.PrepareNumber(m_Layout, "Timer_Seconds");
		m_Initialised = true;
	}

	private void OnActivate()
	{
		if (!m_Initialised)
		{
			LateInit();
		}
		float num = (float)minutes * 60f + (float)seconds;
		finishTime = Time.time + num;
		m_Running = true;
		MFGuiManager.Instance.ShowLayout(m_Layout, true);
		UpdateNumbers();
	}

	private void OnDeactivate()
	{
		Disable();
	}

	private void UpdateNumbers()
	{
		int min;
		int sec;
		RemainingMinutesAndSeconds(out min, out sec);
		if (!countDown)
		{
			min = Mathf.Clamp(minutes - min, 0, 99);
			sec = Mathf.Clamp(seconds - sec, 0, 60);
		}
		m_NumberMinutes.SetNumber(min, 99);
		m_NumberSeconds.SetNumber(sec, 60);
	}

	private void Update()
	{
		if (!m_Running || m_Suspended)
		{
			return;
		}
		UpdateNumbers();
		foreach (WarningSetting warning in warnings)
		{
			float num = finishTime - warning.RemainingTime;
			if (lastUpdateTime < num && Time.time > num)
			{
				if ((bool)warning.Anim)
				{
					m_Layout.PlayAnimClip(warning.Anim);
				}
				if ((bool)warning.Sound)
				{
					MFGuiManager.Instance.PlayOneShot(warning.Sound);
				}
			}
		}
		lastUpdateTime = Time.time;
		if (Time.time >= finishTime)
		{
			sendOnFinish.Send();
			m_Running = false;
			StartCoroutine(StopAnimAndHide());
		}
	}

	private IEnumerator StopAnimAndHide()
	{
		m_Layout.StopCurrentAnimClip();
		yield return new WaitForEndOfFrame();
		MFGuiManager.Instance.ShowLayout(m_Layout, false);
	}

	private void Suspend(bool suspend)
	{
		if (m_Running)
		{
			m_Pivot.Show(!suspend);
			m_Suspended = suspend;
		}
	}
}
