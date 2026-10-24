using System.Collections;
using UnityEngine;

[AddComponentMenu("GUI/Menu/GuiCustomizeControls")]
public class GuiCustomizeControls : MonoBehaviour
{
	private enum E_Dragging
	{
		None = 0,
		FireButton = 1,
		PauseButton = 2,
		WeaponButton = 3,
		DpadMove = 4,
		Reload = 5,
		Roll = 6
	}

	public delegate void CustomizeHideDelegate();

	public static GuiCustomizeControls Instance;

	private GUIBase_Pivot m_CutomizePivot;

	private GUIBase_Layout m_CutomizeLayout;

	private static string s_CutomizePivotName = "Customize";

	private static string s_CutomizeLayoutName = "Customize_Layout";

	private static string s_ResetButtonName = "ResetButton";

	private static string s_ConfirmButtonName = "BackButton";

	private GUIBase_Widget m_FireButtonDummy;

	private GUIBase_Widget m_WeaponButtonDummy;

	private GUIBase_Widget m_PauseButtonDummy;

	private GUIBase_Widget m_DpadMoveDummy;

	private GUIBase_Widget m_ReloadDummy;

	private GUIBase_Widget m_RollDummy;

	private Transform FireTransform;

	private Transform WeaponTransform;

	private Transform PauseTransform;

	private Transform MoveTransform;

	private Transform ReloadTransform;

	private Transform RollTransform;

	private Vector3 FireOrigScale;

	private Vector3 WeaponOrigScale;

	private Vector3 PauseOrigScale;

	private Vector3 MoveOrigScale;

	private Vector3 ReloadOrigScale;

	private Vector3 RollOrigScale;

	private Vector2 TempFireOffset;

	private Vector2 TempWeaponOffset;

	private Vector2 TempPauseOffset;

	private Vector2 TempMoveOffset;

	private Vector2 TempReloadOffset;

	private Vector2 TempRollOffset;

	private Vector2 DragBeginPos;

	private Vector2 DragBeginOffset;

	private GUIBase_Button m_BackButton;

	private E_Dragging m_Dragging;

	private bool isOn;

	public CustomizeHideDelegate m_OnHideDelegate;

	private ArrayList MenuItems = new ArrayList();

	private int MenuSelection = -1;

	private MenuController MenuCtrl = new MenuController();

	private void Awake()
	{
		Instance = this;
	}

	public void LateInitialize()
	{
		m_CutomizePivot = MFGuiManager.Instance.GetPivot(s_CutomizePivotName);
		if (!m_CutomizePivot)
		{
			Debug.LogError("Pivot '" + s_CutomizePivotName + "'  not found");
		}
		m_CutomizeLayout = m_CutomizePivot.GetLayout(s_CutomizeLayoutName);
		if (!m_CutomizeLayout)
		{
			Debug.LogError("Layout '" + s_CutomizeLayoutName + "'  not found");
		}
		GuiBaseUtils.RegisterButtonDelegate(m_CutomizeLayout, s_ResetButtonName, null, ResetButtonDelegate);
		m_BackButton = GuiBaseUtils.RegisterButtonDelegate(m_CutomizeLayout, s_ConfirmButtonName, null, ConfirmButtonDelegate);
		GUIBase_Sprite gUIBase_Sprite = GuiBaseUtils.PrepareSprite(m_CutomizeLayout, "FireDummy");
		m_FireButtonDummy = gUIBase_Sprite.Widget;
		FireTransform = m_FireButtonDummy.gameObject.transform;
		gUIBase_Sprite = GuiBaseUtils.PrepareSprite(m_CutomizeLayout, "WeaponsDummy");
		m_WeaponButtonDummy = gUIBase_Sprite.Widget;
		WeaponTransform = m_WeaponButtonDummy.gameObject.transform;
		gUIBase_Sprite = GuiBaseUtils.PrepareSprite(m_CutomizeLayout, "PauseDummy");
		m_PauseButtonDummy = gUIBase_Sprite.Widget;
		PauseTransform = m_PauseButtonDummy.gameObject.transform;
		gUIBase_Sprite = GuiBaseUtils.PrepareSprite(m_CutomizeLayout, "DpadMoveDummy");
		m_DpadMoveDummy = gUIBase_Sprite.Widget;
		MoveTransform = m_DpadMoveDummy.gameObject.transform;
		gUIBase_Sprite = GuiBaseUtils.PrepareSprite(m_CutomizeLayout, "ReloadDummy");
		m_ReloadDummy = gUIBase_Sprite.Widget;
		ReloadTransform = m_ReloadDummy.gameObject.transform;
		gUIBase_Sprite = GuiBaseUtils.PrepareSprite(m_CutomizeLayout, "RollDummy");
		m_RollDummy = gUIBase_Sprite.Widget;
		RollTransform = m_RollDummy.gameObject.transform;
		MenuItems = new ArrayList();
		MenuItems.Add(GuiBaseUtils.GetButton(m_CutomizeLayout, s_ResetButtonName));
		MenuItems.Add(GuiBaseUtils.GetButton(m_CutomizeLayout, s_ConfirmButtonName));
		InitOrigScale();
	}

	private void InitOrigScale()
	{
		FireOrigScale = FireTransform.localScale;
		WeaponOrigScale = WeaponTransform.localScale;
		PauseOrigScale = PauseTransform.localScale;
		MoveOrigScale = MoveTransform.localScale;
		ReloadOrigScale = ReloadTransform.localScale;
		RollOrigScale = RollTransform.localScale;
	}

	public void Show()
	{
		isOn = true;
		InitTempPositions();
		MFGuiManager.Instance.ShowPivot(m_CutomizePivot, true);
		ShowSchemeSpecificSprites();
		MenuSelection = -1;
	}

	private void Hide()
	{
		isOn = false;
		MFGuiManager.Instance.ShowPivot(m_CutomizePivot, false);
		HideSchemeSpecificSprites();
	}

	private void InitTempPositions()
	{
		TempFireOffset = GuiOptions.FireUseButton.Offset;
		TempWeaponOffset = GuiOptions.WeaponButton.Offset;
		TempPauseOffset = GuiOptions.PauseButton.Offset;
		TempMoveOffset = GuiOptions.MoveStick.Offset;
		TempReloadOffset = GuiOptions.ReloadButton.Offset;
		TempRollOffset = GuiOptions.RollButton.Offset;
		UpdateSpritesPos();
	}

	private void CancelButtonDelegate(bool inside)
	{
		if (inside)
		{
			Hide();
			if (m_OnHideDelegate != null)
			{
				m_OnHideDelegate();
			}
		}
	}

	private void ResetButtonDelegate(bool inside)
	{
		if (inside)
		{
			TempWeaponOffset = Vector2.zero;
			TempPauseOffset = Vector2.zero;
			TempFireOffset = Vector2.zero;
			TempMoveOffset = Vector2.zero;
			TempReloadOffset = Vector2.zero;
			TempRollOffset = Vector2.zero;
			if (GuiOptions.leftHandAiming)
			{
				TempFireOffset.x = (float)Screen.width - GuiOptions.FireUseButton.OrigPos.x - GuiOptions.FireUseButton.OrigPos.x;
				TempMoveOffset.x = (float)Screen.width - GuiOptions.MoveStick.OrigPos.x - GuiOptions.MoveStick.OrigPos.x;
			}
			UpdateSpritesPos();
			RedrawSprites();
		}
	}

	private void ConfirmButtonDelegate(bool inside)
	{
		if (inside)
		{
			GuiOptions.FireUseButton.Offset = TempFireOffset;
			GuiOptions.WeaponButton.Offset = TempWeaponOffset;
			GuiOptions.PauseButton.Offset = TempPauseOffset;
			GuiOptions.MoveStick.Offset = TempMoveOffset;
			GuiOptions.ReloadButton.Offset = TempReloadOffset;
			GuiOptions.RollButton.Offset = TempRollOffset;
			GuiHUD.Instance.UpdateControlsPosition();
			Player.Instance.Controls.TouchControls.OnControlSchemeChange();
			GuiOptions.Save();
			Hide();
			if (m_OnHideDelegate != null)
			{
				m_OnHideDelegate();
			}
		}
	}

	private void MouseTouchEvent()
	{
		if (Input.GetMouseButtonDown(0))
		{
			TouchBegin(Input.mousePosition);
		}
		else if (Input.GetMouseButtonUp(0))
		{
			TouchEnd(Input.mousePosition);
		}
		else if (m_Dragging != E_Dragging.None)
		{
			TouchUpdate(Input.mousePosition);
		}
	}

	private void TouchBegin(Vector2 pos)
	{
		Vector2 clickPos = m_CutomizeLayout.ScreenPosToLayoutSpace(pos);
		if (m_FireButtonDummy.IsMouseOver(clickPos))
		{
			m_Dragging = E_Dragging.FireButton;
			DragBeginOffset = TempFireOffset;
		}
		else if (m_WeaponButtonDummy.IsMouseOver(clickPos))
		{
			m_Dragging = E_Dragging.WeaponButton;
			DragBeginOffset = TempWeaponOffset;
		}
		else if (m_PauseButtonDummy.IsMouseOver(clickPos))
		{
			m_Dragging = E_Dragging.PauseButton;
			DragBeginOffset = TempPauseOffset;
		}
		else if (m_DpadMoveDummy.IsVisible() && m_DpadMoveDummy.IsMouseOver(clickPos))
		{
			m_Dragging = E_Dragging.DpadMove;
			DragBeginOffset = TempMoveOffset;
		}
		else if (m_ReloadDummy.IsVisible() && m_ReloadDummy.IsMouseOver(clickPos))
		{
			m_Dragging = E_Dragging.Reload;
			DragBeginOffset = TempReloadOffset;
		}
		else if (m_RollDummy.IsVisible() && m_RollDummy.IsMouseOver(clickPos))
		{
			m_Dragging = E_Dragging.Roll;
			DragBeginOffset = TempRollOffset;
		}
		if (m_Dragging != E_Dragging.None)
		{
			DragBeginPos = pos;
		}
	}

	private void TouchUpdate(Vector2 pos)
	{
		Vector2 vector = pos - DragBeginPos;
		vector.y = 0f - vector.y;
		switch (m_Dragging)
		{
		case E_Dragging.None:
			return;
		case E_Dragging.FireButton:
			TempFireOffset = DragBeginOffset + vector;
			break;
		case E_Dragging.WeaponButton:
			TempWeaponOffset = DragBeginOffset + vector;
			break;
		case E_Dragging.PauseButton:
			TempPauseOffset = DragBeginOffset + vector;
			break;
		case E_Dragging.DpadMove:
			TempMoveOffset = DragBeginOffset + vector;
			break;
		case E_Dragging.Reload:
			TempReloadOffset = DragBeginOffset + vector;
			break;
		case E_Dragging.Roll:
			TempRollOffset = DragBeginOffset + vector;
			break;
		}
		UpdateSpritesPos();
		RedrawSprites();
	}

	private void TouchEnd(Vector2 pos)
	{
		m_Dragging = E_Dragging.None;
	}

	private void LateUpdate()
	{
		if (!isOn)
		{
			return;
		}
		MenuCtrl.Update();
		if (MenuCtrl.PressedLeft())
		{
			ChangeFocusLeft();
		}
		else if (MenuCtrl.PressedRight())
		{
			ChangeFocusRight();
		}
		else if (MenuCtrl.PressedOk())
		{
			PressFocusButton();
		}
		else if (MenuCtrl.PressedBack())
		{
			PressBackButton();
		}
		if (Input.touchCount != 0)
		{
			Touch touch = Input.GetTouch(0);
			if (touch.phase == TouchPhase.Began)
			{
				TouchBegin(touch.position);
			}
			else if (touch.phase == TouchPhase.Moved)
			{
				TouchUpdate(touch.position);
			}
			else if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
			{
				TouchEnd(touch.position);
			}
		}
	}

	private void UpdateSpritesPos()
	{
		FireTransform.position = GuiOptions.FireUseButton.OrigPos + TempFireOffset;
		WeaponTransform.position = GuiOptions.WeaponButton.OrigPos + TempWeaponOffset;
		PauseTransform.position = GuiOptions.PauseButton.OrigPos + TempPauseOffset;
		ReloadTransform.position = GuiOptions.ReloadButton.OrigPos + TempReloadOffset;
		RollTransform.position = GuiOptions.RollButton.OrigPos + TempRollOffset;
		if (GuiOptions.m_ControlScheme == GuiOptions.E_ControlScheme.Scheme2)
		{
			MoveTransform.position = GuiOptions.MoveStick.OrigPos + TempMoveOffset;
		}
	}

	private void RedrawSprites()
	{
		m_FireButtonDummy.ShowImmediate(false, true);
		m_FireButtonDummy.ShowImmediate(true, true);
		m_WeaponButtonDummy.ShowImmediate(false, true);
		m_WeaponButtonDummy.ShowImmediate(true, true);
		m_PauseButtonDummy.ShowImmediate(false, true);
		m_PauseButtonDummy.ShowImmediate(true, true);
		m_ReloadDummy.ShowImmediate(false, true);
		m_ReloadDummy.ShowImmediate(true, true);
		m_RollDummy.ShowImmediate(false, true);
		m_RollDummy.ShowImmediate(true, true);
		if (GuiOptions.m_ControlScheme == GuiOptions.E_ControlScheme.Scheme2)
		{
			m_DpadMoveDummy.ShowImmediate(false, true);
			m_DpadMoveDummy.ShowImmediate(true, true);
		}
	}

	private void ShowSchemeSpecificSprites()
	{
		if (GuiOptions.m_ControlScheme == GuiOptions.E_ControlScheme.Scheme1)
		{
			m_DpadMoveDummy.ShowImmediate(false, true);
		}
		else if (GuiOptions.m_ControlScheme == GuiOptions.E_ControlScheme.Scheme2)
		{
			m_DpadMoveDummy.ShowImmediate(true, true);
		}
	}

	private void HideSchemeSpecificSprites()
	{
		m_DpadMoveDummy.ShowImmediate(false, true);
	}

	private void AnimateControls()
	{
		float num = 1f + 0.025f * Mathf.Sin(Time.realtimeSinceStartup * 4f);
		FireTransform.localScale = FireOrigScale * num;
		WeaponTransform.localScale = WeaponOrigScale * num;
		PauseTransform.localScale = PauseOrigScale * num;
		MoveTransform.localScale = MoveOrigScale * num;
		ReloadTransform.localScale = ReloadOrigScale * num;
		RollTransform.localScale = RollOrigScale * num;
	}

	private void ChangeFocusLeft()
	{
		if (MenuSelection != 0)
		{
			int newSel = 0;
			UpdateSelection(newSel);
		}
	}

	private void ChangeFocusRight()
	{
		if (MenuSelection != 1)
		{
			int newSel = 1;
			UpdateSelection(newSel);
		}
	}

	private void PressFocusButton()
	{
		if (MenuSelection == -1)
		{
			UpdateSelection(1);
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
		if ((bool)m_BackButton && !m_BackButton.IsDisabled() && m_BackButton.Widget.IsVisible())
		{
			m_BackButton.Callback(GUIBase_Callback.E_CallbackType.E_CT_ON_TOUCH_BEGIN);
			m_BackButton.Callback(GUIBase_Callback.E_CallbackType.E_CT_ON_TOUCH_END);
		}
	}

	private GUIBase_Button GetCurrentMenuItem(int index)
	{
		if (index < 0 || index >= MenuItems.Count)
		{
			return null;
		}
		return (GUIBase_Button)MenuItems[index];
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
}
