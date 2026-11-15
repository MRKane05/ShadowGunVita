using UnityEngine;

[AddComponentMenu("")]
public class GUIBase_Callback : MonoBehaviour
{
	public enum E_CallbackType
	{
		E_CT_NONE = 0,
		E_CT_INIT = 1,
		E_CT_SHOW = 2,
		E_CT_HIDE = 4,
		E_CT_ON_TOUCH_BEGIN = 8,
		E_CT_ON_TOUCH_END = 0x10,
		E_CT_ON_TOUCH_END_OUTSIDE = 0x20,
		E_CT_ON_TOUCH_END_KEYBOARD = 0x40,
		E_CT_ON_MOUSEOVER_BEGIN = 0x80,
		E_CT_ON_MOUSEOVER_END = 0x100
	}

	private int m_Flags;

	public virtual bool Callback(E_CallbackType type)
	{
		Debug.LogError("This method should be overriden !!!");
		return false;
	}

	public virtual void GetTouchAreaScale(out float scaleWidth, out float scaleHeight)
	{
		scaleWidth = 1f;
		scaleHeight = 1f;
	}

	public virtual void ChildButtonPressed(float v)
	{
		Debug.LogError("This method should be overriden !!!");
	}

	public virtual void ChildButtonReleased()
	{
		Debug.LogError("This method should be overriden !!!");
	}

	public void RegisterCallbackType(int clbkTypes)
	{
		m_Flags |= clbkTypes;
	}

	public bool TestFlag(int flagMask)
	{
		return (m_Flags & flagMask) != 0;
	}
}
