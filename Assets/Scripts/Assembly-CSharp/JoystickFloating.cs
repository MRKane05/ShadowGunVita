using UnityEngine;

public class JoystickFloating : JoystickBase
{
	private Rect TouchArea;

	public JoystickFloating(Rect r)
	{
		SetTouchArea(r);
	}

	public void SetTouchArea(Rect r)
	{
		TouchArea.x = (float)Screen.width * r.x;
		TouchArea.y = (float)Screen.height * r.y;
		TouchArea.width = (float)Screen.width * r.width;
		TouchArea.height = (float)Screen.height * r.height;
	}

	public override bool IsInside(Touch touch)
	{
		return TouchArea.Contains(touch.position);
	}

	public override void OnTouchBegin(Touch touch)
	{
		FingerID = touch.fingerId;
		SetCenter(touch.position);
		Dir = Vector2.zero;
		Force = 0f;
		Updated = true;
		if ((bool)GuiHUD.Instance)
		{
			GuiHUD.Instance.JoystickBaseShow(touch.position);
			GuiHUD.Instance.JoystickDown(touch.position);
		}
	}

	public override void OnTouchUpdate(Touch touch)
	{
		if (FingerID != touch.fingerId)
		{
			Debug.LogError("inconsistent finger id in joystick update");
			return;
		}
		Dir = touch.position - Center;
		float magnitude = Dir.magnitude;
		if (magnitude > base.Radius)
		{
			Dir *= base.Radius / magnitude;
		}
		if ((bool)GuiHUD.Instance)
		{
			GuiHUD.Instance.JoystickUpdate(Center + Dir);
		}
		Force = Mathf.Clamp(magnitude / base.Radius, 0f, 1f);
		Updated = true;
	}

	public override void OnTouchEnd(Touch touch)
	{
		if (FingerID == touch.fingerId)
		{
			FingerID = -1;
			Dir = Vector2.zero;
			Force = 0f;
			if ((bool)GuiHUD.Instance)
			{
				GuiHUD.Instance.JoystickUp();
				GuiHUD.Instance.JoystickBaseHide();
			}
			Updated = false;
		}
		else
		{
			Debug.LogError("Inconsistent finger id");
		}
	}
}
