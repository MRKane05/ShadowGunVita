using System.Collections;
using UnityEngine;

[AddComponentMenu("GUI/Hiearchy/Platform")]
public class GUIBase_Platform : MonoBehaviour
{
	public enum E_SpecialAnimIdx
	{
		E_SAI_INANIM = 0,
		E_SAI_OUTANIM = 1,
		E_SAI_FIRSTCUSTOM = 10,
		E_SAI_BUTTONANIM = 11
	}

	private struct S_AnimDscr
	{
		public Animation m_Animation;

		public float m_StartTime;

		public float m_Length;

		public int m_CustomIdx;

		public AnimFinishedDelegate m_AnimFinishedDelegate;

		public GUIBase_Widget m_Widget;
	}

	public delegate void AnimFinishedDelegate(int animIdx);

	public int m_Width = 320;

	public int m_Height = 480;

	private MFGuiManager m_GuiManager;

	private ArrayList m_PlayingAnims;

	private ArrayList m_AnimsToRemove;

	private bool m_IsInitialized;

	public void Start()
	{
		m_GuiManager = MFGuiManager.Instance;
		m_IsInitialized = false;
		if ((bool)m_GuiManager)
		{
			m_GuiManager.RegisterPlatform(this);
			m_PlayingAnims = new ArrayList();
			m_AnimsToRemove = new ArrayList();
		}
		else
		{
			Debug.LogError("GuiManager prefab is not present!");
		}
	}

	public bool IsInitialized()
	{
		return m_IsInitialized;
	}

	public void Update()
	{
		m_IsInitialized = true;
		ProcessAnimations();
	}

	public void PlayAnim(Animation animation, GUIBase_Widget widget, AnimFinishedDelegate finishDelegate = null, int customIdx = -1)
	{
		S_AnimDscr s_AnimDscr = default(S_AnimDscr);
		s_AnimDscr.m_Animation = animation;
		s_AnimDscr.m_StartTime = Time.realtimeSinceStartup;
		s_AnimDscr.m_Length = animation.clip.length;
		s_AnimDscr.m_CustomIdx = customIdx;
		s_AnimDscr.m_AnimFinishedDelegate = finishDelegate;
		s_AnimDscr.m_Widget = widget;
		s_AnimDscr.m_Animation.wrapMode = s_AnimDscr.m_Animation.clip.wrapMode;
		int count = m_PlayingAnims.Count;
		m_PlayingAnims.Add(s_AnimDscr);
		ProcessAnim(s_AnimDscr, 0f, count);
	}

	public void StopAnim(Animation animation)
	{
		if (!animation)
		{
			return;
		}
		animation.Stop();
		animation.Sample();
		for (int i = 0; i < m_PlayingAnims.Count; i++)
		{
			S_AnimDscr anim = (S_AnimDscr)m_PlayingAnims[i];
			if (anim.m_Animation == animation)
			{
				ProcessAnim(anim, 1f, i);
				if ((bool)anim.m_Widget)
				{
					anim.m_Widget.SetModify();
				}
				m_PlayingAnims.RemoveAt(i);
				break;
			}
		}
	}

	private void ProcessAnimations()
	{
		if (m_PlayingAnims != null && m_PlayingAnims.Count > 0)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			for (int i = 0; i < m_PlayingAnims.Count; i++)
			{
				S_AnimDscr anim = (S_AnimDscr)m_PlayingAnims[i];
				float deltaTime = realtimeSinceStartup - anim.m_StartTime;
				ProcessAnim(anim, deltaTime, i);
			}
		}
		if (m_AnimsToRemove == null || m_AnimsToRemove.Count <= 0)
		{
			return;
		}
		for (int num = m_AnimsToRemove.Count - 1; num >= 0; num--)
		{
			S_AnimDscr s_AnimDscr = (S_AnimDscr)m_PlayingAnims[(int)m_AnimsToRemove[num]];
			if (s_AnimDscr.m_CustomIdx != -1)
			{
				AnimationRemoved(s_AnimDscr.m_CustomIdx, s_AnimDscr.m_AnimFinishedDelegate);
			}
			m_PlayingAnims.RemoveAt((int)m_AnimsToRemove[num]);
		}
		m_AnimsToRemove.RemoveRange(0, m_AnimsToRemove.Count);
	}

	private void ProcessAnim(S_AnimDscr anim, float deltaTime, int idx)
	{
		anim.m_Animation.Play();
		foreach (AnimationState item in anim.m_Animation)
		{
			item.enabled = true;
			item.time = deltaTime;
		}
		anim.m_Animation.Sample();
		foreach (AnimationState item2 in anim.m_Animation)
		{
			item2.enabled = false;
		}
		if ((anim.m_Animation.wrapMode == WrapMode.Once || anim.m_Animation.wrapMode == WrapMode.Default) && deltaTime > anim.m_Length)
		{
			m_AnimsToRemove.Add(idx);
		}
		if ((bool)anim.m_Widget)
		{
			anim.m_Widget.SetModify();
		}
	}

	private void AnimationRemoved(int customIdx, AnimFinishedDelegate finishDelegate)
	{
		if (finishDelegate != null)
		{
			finishDelegate(customIdx);
		}
	}
}
