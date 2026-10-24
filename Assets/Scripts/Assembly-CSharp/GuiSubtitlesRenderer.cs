using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[AddComponentMenu("")]
public class GuiSubtitlesRenderer : MonoBehaviour
{
	public GUIBase_Layout m_Background;

	private GUIBase_Label m_Label;

	private AudioSource m_Audio;

	private GuiSubtitles m_CurrentSubtitles;

	private bool wasAudioPlaying;

	public static GuiSubtitlesRenderer Instance;

	private void Awake()
	{
		base.useGUILayout = false;
		m_Audio = base.GetComponent<AudioSource>();
		m_Audio.playOnAwake = false;
		Instance = this;
	}

	private void Start()
	{
		base.enabled = false;
		m_Label = m_Background.GetComponentInChildren<GUIBase_Label>();
		wasAudioPlaying = false;
	}

	public static void Deactivate()
	{
		if (Instance != null)
		{
			Instance.DeactivateInternal();
		}
	}

	public static void ShowSubtitles(GuiSubtitles inSubtitles)
	{
		if (Instance != null)
		{
			Instance.ShowSubtitlesInternal(inSubtitles);
		}
	}

	public static void ShowAllRunning(bool show)
	{
		if (Instance != null)
		{
			Instance.ShowAllRunningInternal(show);
		}
	}

	private void DeactivateInternal()
	{
		if (base.enabled)
		{
			StopAllCoroutines();
			if (m_Background != null)
			{
				MFGuiManager.Instance.ShowLayout(m_Background, false);
			}
			if ((bool)m_Audio && (bool)m_Audio.clip)
			{
				m_Audio.Stop();
			}
			OnSequenceEnd();
		}
	}

	internal void ShowSubtitlesInternal(GuiSubtitles inSubtitles)
	{
		if (m_CurrentSubtitles != null)
		{
			Deactivate();
		}
		m_CurrentSubtitles = inSubtitles;
		if (m_CurrentSubtitles != null)
		{
			base.enabled = true;
			if ((bool)m_Label)
			{
				m_Label.Clear();
			}
			StartCoroutine("RunSubtitlesSequence");
		}
	}

	internal void ShowAllRunningInternal(bool show)
	{
		if (!show || CanShowBackground())
		{
			MFGuiManager.Instance.ShowLayout(m_Background, show);
			base.enabled = show;
		}
		else if (m_CurrentSubtitles != null)
		{
			base.enabled = show;
		}
	}

	private IEnumerator RunSubtitlesSequence()
	{
		OnSequenceBegin();
		if (CanShowBackground())
		{
			yield return StartCoroutine(ShowBackGround());
		}
		if ((bool)m_CurrentSubtitles.Voice)
		{
			m_Audio.clip = m_CurrentSubtitles.Voice;
			m_Audio.Play();
		}
		if (GuiOptions.subtitles || m_CurrentSubtitles.ForceShow)
		{
			GuiSubtitles.SubtitleLineEx[] sequenceEx = m_CurrentSubtitles.SequenceEx;
			foreach (GuiSubtitles.SubtitleLineEx l in sequenceEx)
			{
				m_Label.SetNewText(l.TextID);
				yield return new WaitForSeconds(l.Time);
				m_Label.Clear();
				yield return new WaitForSeconds(0.3f);
			}
		}
		if ((bool)m_Background)
		{
			yield return StartCoroutine(HideBackGround());
		}
		while (m_Audio.isPlaying)
		{
			yield return new WaitForSeconds(0.2f);
		}
		OnSequenceEnd();
	}

	private void OnSequenceBegin()
	{
		if (m_CurrentSubtitles.ForceWalkOnPlayer)
		{
			Player.Instance.Owner.BlackBoard.ComlinkOn = true;
			Player.Instance.Owner.BlackBoard.Desires.WeaponTriggerOn = false;
			GuiHUD.Instance.HideWeaponControls();
		}
	}

	private void OnSequenceEnd()
	{
		if (m_CurrentSubtitles.ForceWalkOnPlayer)
		{
			Player.Instance.Owner.BlackBoard.ComlinkOn = false;
			GuiHUD.Instance.ShowWeaponControls();
		}
		if ((bool)m_Background && m_Background.IsVisible())
		{
			MFGuiManager.Instance.ShowLayout(m_Background, false);
		}
		m_CurrentSubtitles = null;
		base.enabled = false;
		wasAudioPlaying = false;
	}

	internal bool CanShowBackground()
	{
		return (bool)m_Background && (bool)m_CurrentSubtitles && m_CurrentSubtitles.hasAnyText && (GuiOptions.subtitles || m_CurrentSubtitles.ForceShow);
	}

	internal IEnumerator ShowBackGround()
	{
		MFGuiManager.Instance.ShowLayout(m_Background, true);
		yield return new WaitForSeconds(0.1f);
		while (!m_Background.ShowDone)
		{
			yield return new WaitForSeconds(0.1f);
		}
	}

	internal IEnumerator HideBackGround()
	{
		MFGuiManager.Instance.ShowLayout(m_Background, false);
		yield return new WaitForSeconds(0.1f);
		while (!m_Background.HideDone)
		{
			yield return new WaitForSeconds(0.1f);
		}
	}

	public static void Suspend(bool suspend)
	{
		if (Instance != null)
		{
			Instance.SuspendInternal(suspend);
		}
	}

	private void SuspendInternal(bool suspend)
	{
		if (!m_Audio || !m_Audio.clip)
		{
			return;
		}
		if (suspend)
		{
			if (m_Audio.isPlaying)
			{
				wasAudioPlaying = true;
				m_Audio.Pause();
			}
		}
		else if (wasAudioPlaying)
		{
			wasAudioPlaying = false;
			m_Audio.Play();
		}
	}
}
