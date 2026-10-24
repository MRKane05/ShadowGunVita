using UnityEngine;

[AddComponentMenu("GUI/Widgets/Button")]
public class GUIBase_Button : GUIBase_Callback
{
	public enum E_ButtonState
	{
		E_BS_NONE = -1,
		E_BS_IDLE = 0,
		E_BS_DISABLED = 1,
		E_BS_MOUSE_OVER = 2,
		E_BS_BUTTON_DOWN = 3,
		E_BS_LAST_IDX = 4
	}

	private enum E_ButtonSubstate
	{
		E_BSS_IN = 0,
		E_BSS_LOOP = 1,
		E_BSS_OUT = 2
	}

	public struct S_SpriteUV
	{
		public bool m_IsReady;

		public Vector2 m_LowerLeftUV;

		public Vector2 m_UvDimensions;
	}

	public delegate void TouchDelegate();

	public delegate void ReleaseDelegate(bool inside);

	public bool initStateDisabled;

	public float m_TouchableAreaWidthScale = 1f;

	public float m_TouchableAreaHeightScale = 1f;

	public AnimationClip idleAnimationLoop;

	public GUIBase_Widget idleLabel;

	public GUIBase_Sprite disabledSprite;

	public AnimationClip disabledAnimationLoop;

	public GUIBase_Widget disabledLabel;

	public GUIBase_Sprite mouseOverSprite;

	public AnimationClip mouseOverAnimationLoop;

	public AudioClip mouseOverSoundIn;

	public GUIBase_Widget mouseOverLabel;

	public GUIBase_Sprite buttonDownSprite;

	public AnimationClip buttonDownAnimationLoop;

	public AudioClip buttonDownSoundIn;

	public AudioClip buttonDownSoundOut;

	public GUIBase_Widget buttonDownLabel;

	public GUIBase_Callback m_ParentWidget;

	public float m_ValueChangedInParent;

	public string inputButton;

	private TouchDelegate m_TouchDelegate;

	private ReleaseDelegate m_ReleaseDelegate;

	private GUIBase_Widget m_Widget;

	private MFGuiRenderer m_GuiRenderer;

	private E_ButtonState m_CurrentState = E_ButtonState.E_BS_NONE;

	private E_ButtonState m_NextState = E_ButtonState.E_BS_NONE;

	private E_ButtonState m_PrevState = E_ButtonState.E_BS_NONE;

	private E_ButtonSubstate m_Substate = E_ButtonSubstate.E_BSS_LOOP;

	private S_SpriteUV[] m_UsedSpritesUV = new S_SpriteUV[4];

	private Animation m_Anim;

	private bool m_WasTouched;

	private GUIBase_Widget[] m_Labels = new GUIBase_Widget[4];

	private bool m_Disabled;

	private bool m_ForceHighlight;

	public GUIBase_Widget Widget
	{
		get
		{
			return m_Widget;
		}
	}

	public bool IsDisabled()
	{
		return m_Disabled;
	}

	public void Start()
	{
		m_Widget = GetComponent<GUIBase_Widget>();
		m_Anim = GetComponent<Animation>();
		int num = 15;
		if ((bool)buttonDownSprite)
		{
			num += 16;
		}
		if ((bool)mouseOverSprite)
		{
			num += 384;
		}
		m_Widget.RegisterCallback(this, num);
		m_Widget.RegisterUpdateDelegate(UpdateButton);
		m_Disabled = initStateDisabled;
	}

	private void SetLabel(GUIBase_Widget w, int stateIdx)
	{
		if ((bool)w && ((bool)w.GetComponent<GUIBase_Label>() || (bool)w.GetComponent<GUIBase_Sprite>()))
		{
			m_Labels[stateIdx] = w;
		}
		else
		{
			m_Labels[stateIdx] = null;
		}
	}

	public void SetDisabled(bool disabled)
	{
		bool flag = m_Disabled != disabled;
		m_Disabled = disabled;
		if (m_Widget.IsVisible() && flag)
		{
			if (m_Disabled)
			{
				ChangeButtonState(E_ButtonState.E_BS_DISABLED);
			}
			else
			{
				ChangeButtonState(E_ButtonState.E_BS_IDLE);
			}
		}
	}

	public void ForceHighlight(bool on)
	{
		if (m_ForceHighlight == on)
		{
			return;
		}
		if (m_Widget.IsVisible())
		{
			if (on)
			{
				ChangeButtonState(E_ButtonState.E_BS_MOUSE_OVER);
			}
			else
			{
				ChangeButtonState(E_ButtonState.E_BS_IDLE);
			}
		}
		m_ForceHighlight = on;
	}

	private void CustomInit()
	{
		m_GuiRenderer = m_Widget.GetGuiRenderer();
		for (int i = 0; i < 4; i++)
		{
			m_UsedSpritesUV[i].m_IsReady = false;
			m_Labels[i] = null;
		}
		PrepareUV(m_Widget, 0);
		SetLabel(idleLabel, 0);
		if ((bool)disabledSprite)
		{
			PrepareUV(disabledSprite.Widget, 1);
			SetLabel(disabledLabel, 1);
		}
		if ((bool)mouseOverSprite)
		{
			PrepareUV(mouseOverSprite.Widget, 2);
			SetLabel(mouseOverLabel, 2);
		}
		if ((bool)buttonDownSprite)
		{
			PrepareUV(buttonDownSprite.Widget, 3);
			SetLabel(buttonDownLabel, 3);
		}
	}

	private void PrepareUV(GUIBase_Widget w, int idx)
	{
		if (idx >= 0 && idx < 4)
		{
			Texture texture = w.GetTexture();
			if ((bool)texture)
			{
				float UVLeft;
				float UVTop;
				float UVWidth;
				float UVHeight;
				w.GetUV(out UVLeft, out UVTop, out UVWidth, out UVHeight);
				int width = texture.width;
				int height = texture.height;
				m_UsedSpritesUV[idx].m_IsReady = true;
				m_UsedSpritesUV[idx].m_LowerLeftUV = new Vector2(UVLeft, UVTop + UVHeight);
				m_UsedSpritesUV[idx].m_UvDimensions = new Vector2(UVWidth / (float)width, UVHeight / (float)height);
			}
		}
	}

	public void RegisterTouchDelegate(TouchDelegate d)
	{
		m_TouchDelegate = d;
	}

	public void RegisterReleaseDelegate(ReleaseDelegate d)
	{
		m_ReleaseDelegate = d;
	}

	public override bool Callback(E_CallbackType type)
	{
		switch (type)
		{
		case E_CallbackType.E_CT_INIT:
			CustomInit();
			return true;
		case E_CallbackType.E_CT_SHOW:
			if (m_Disabled)
			{
				ChangeButtonState(E_ButtonState.E_BS_DISABLED);
			}
			else if (m_ForceHighlight)
			{
				ChangeButtonState(E_ButtonState.E_BS_MOUSE_OVER);
			}
			else
			{
				ChangeButtonState(E_ButtonState.E_BS_IDLE);
			}
			m_WasTouched = false;
			return true;
		case E_CallbackType.E_CT_HIDE:
			m_CurrentState = E_ButtonState.E_BS_NONE;
			m_Substate = E_ButtonSubstate.E_BSS_LOOP;
			if (m_ReleaseDelegate != null)
			{
				m_ReleaseDelegate(false);
			}
			return true;
		case E_CallbackType.E_CT_ON_TOUCH_BEGIN:
			if (m_Disabled)
			{
				break;
			}
			m_WasTouched = true;
			if ((bool)m_ParentWidget)
			{
				m_ParentWidget.ChildButtonPressed(m_ValueChangedInParent);
			}
			if ((bool)buttonDownSprite)
			{
				ChangeButtonState(E_ButtonState.E_BS_BUTTON_DOWN);
			}
			else
			{
				MFGuiManager.Instance.PlayOneShot(buttonDownSoundIn);
			}
			if (m_TouchDelegate != null)
			{
				m_TouchDelegate();
			}
			return true;
		case E_CallbackType.E_CT_ON_TOUCH_END:
			if (m_Disabled)
			{
				break;
			}
			if (m_WasTouched)
			{
				m_WasTouched = false;
				if (m_ForceHighlight)
				{
					ChangeButtonState(E_ButtonState.E_BS_MOUSE_OVER);
				}
				else
				{
					ChangeButtonState(E_ButtonState.E_BS_IDLE);
				}
				if (m_ReleaseDelegate != null)
				{
					m_ReleaseDelegate(true);
				}
			}
			return true;
		case E_CallbackType.E_CT_ON_TOUCH_END_OUTSIDE:
			if (m_Disabled)
			{
				break;
			}
			if (m_WasTouched)
			{
				m_WasTouched = false;
				if (m_ReleaseDelegate != null)
				{
					m_ReleaseDelegate(true);
				}
			}
			else if (m_ReleaseDelegate != null)
			{
				m_ReleaseDelegate(false);
			}
			if (m_ForceHighlight)
			{
				ChangeButtonState(E_ButtonState.E_BS_MOUSE_OVER);
			}
			else
			{
				ChangeButtonState(E_ButtonState.E_BS_IDLE);
			}
			if ((bool)m_ParentWidget)
			{
				m_ParentWidget.ChildButtonReleased();
			}
			return true;
		case E_CallbackType.E_CT_ON_TOUCH_END_KEYBOARD:
			if (m_Disabled)
			{
				break;
			}
			ChangeButtonState(E_ButtonState.E_BS_IDLE);
			if (m_WasTouched)
			{
				m_WasTouched = false;
				if (m_ReleaseDelegate != null)
				{
					m_ReleaseDelegate(true);
				}
			}
			return true;
		case E_CallbackType.E_CT_ON_MOUSEOVER_BEGIN:
			if (m_Disabled)
			{
				break;
			}
			if ((bool)mouseOverSprite)
			{
				ChangeButtonState(E_ButtonState.E_BS_MOUSE_OVER);
			}
			return true;
		case E_CallbackType.E_CT_ON_MOUSEOVER_END:
			if (m_Disabled)
			{
				break;
			}
			if (!m_ForceHighlight)
			{
				ChangeButtonState(E_ButtonState.E_BS_IDLE);
			}
			return true;
		}
		return false;
	}

	public override void GetTouchAreaScale(out float scaleWidth, out float scaleHeight)
	{
		scaleWidth = m_TouchableAreaWidthScale;
		scaleHeight = m_TouchableAreaHeightScale;
	}

	private void SwitchButtonSprite(int idx, int prevState)
	{
		if (idx >= 0 && idx < 4)
		{
			MFGuiSprite sprite = m_Widget.GetSprite(0);
			if (sprite != null && m_UsedSpritesUV[idx].m_IsReady)
			{
				sprite.lowerLeftUV = m_GuiRenderer.PixelCoordToUVCoord((int)m_UsedSpritesUV[idx].m_LowerLeftUV.x, (int)m_UsedSpritesUV[idx].m_LowerLeftUV.y);
				sprite.uvDimensions = m_UsedSpritesUV[idx].m_UvDimensions;
			}
			if (prevState >= 0 && prevState < 4 && (bool)m_Labels[prevState])
			{
				m_Labels[prevState].ShowImmediate(false, true);
			}
			if ((bool)m_Labels[idx])
			{
				m_Labels[idx].ShowImmediate(m_Widget.IsVisible(), true);
			}
			else if ((bool)m_Labels[0])
			{
				m_Labels[0].ShowImmediate(m_Widget.IsVisible(), true);
			}
		}
	}

	public void ChangeButtonState(E_ButtonState s)
	{
		if (s == E_ButtonState.E_BS_NONE)
		{
			m_CurrentState = s;
			m_Substate = E_ButtonSubstate.E_BSS_LOOP;
			m_Widget.StopAnim(m_Anim);
		}
		else if (m_CurrentState != s)
		{
			switch (m_CurrentState)
			{
			case E_ButtonState.E_BS_NONE:
				SwitchButtonSprite((int)s, (int)m_CurrentState);
				m_CurrentState = s;
				m_Substate = E_ButtonSubstate.E_BSS_IN;
				break;
			case E_ButtonState.E_BS_IDLE:
			case E_ButtonState.E_BS_DISABLED:
			case E_ButtonState.E_BS_MOUSE_OVER:
			case E_ButtonState.E_BS_BUTTON_DOWN:
				m_Substate = E_ButtonSubstate.E_BSS_OUT;
				m_NextState = s;
				break;
			}
			while (m_Substate != E_ButtonSubstate.E_BSS_LOOP)
			{
				UpdateButton();
			}
		}
	}

	private void UpdateButton()
	{
		switch (m_CurrentState)
		{
		case E_ButtonState.E_BS_NONE:
			break;
		case E_ButtonState.E_BS_IDLE:
			UpdateSubstate(idleAnimationLoop, null, null);
			break;
		case E_ButtonState.E_BS_DISABLED:
			UpdateSubstate(disabledAnimationLoop, null, null);
			break;
		case E_ButtonState.E_BS_MOUSE_OVER:
			UpdateSubstate(mouseOverAnimationLoop, mouseOverSoundIn, null);
			break;
		case E_ButtonState.E_BS_BUTTON_DOWN:
			UpdateSubstate(buttonDownAnimationLoop, buttonDownSoundIn, buttonDownSoundOut);
			break;
		}
	}

	private void UpdateSubstate(AnimationClip animLoop, AudioClip sndIn, AudioClip sndOut)
	{
		switch (m_Substate)
		{
		case E_ButtonSubstate.E_BSS_IN:
			if (m_PrevState != E_ButtonState.E_BS_BUTTON_DOWN && (bool)sndIn)
			{
				MFGuiManager.Instance.PlayOneShot(sndIn);
			}
			m_Substate = E_ButtonSubstate.E_BSS_LOOP;
			if ((bool)animLoop)
			{
				m_Anim.clip = animLoop;
				m_Widget.StopAnim(m_Anim);
				m_Widget.PlayAnim(m_Anim, Widget);
			}
			break;
		case E_ButtonSubstate.E_BSS_LOOP:
			break;
		case E_ButtonSubstate.E_BSS_OUT:
			if ((bool)animLoop)
			{
				m_Widget.StopAnim(m_Anim);
			}
			if ((bool)sndOut)
			{
				MFGuiManager.Instance.PlayOneShot(sndOut);
			}
			SwitchButtonSprite((int)m_NextState, (int)m_CurrentState);
			m_PrevState = m_CurrentState;
			m_CurrentState = m_NextState;
			m_Substate = E_ButtonSubstate.E_BSS_IN;
			break;
		}
	}
}
