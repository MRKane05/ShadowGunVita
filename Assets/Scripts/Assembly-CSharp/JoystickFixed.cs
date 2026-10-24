using UnityEngine;

public class JoystickFixed : JoystickBase
{
	public float DeadZone
	{
		get
		{
			return 0.02f * (float)Screen.width;
		}
	}

	public JoystickFixed(float posX, float posY)
	{
		SetCenter(new Vector2(posX, posY));
	}

	public override void OnTouchBegin(Touch touch)
	{
		Vector2 vector = touch.position - Center;
		float magnitude = vector.magnitude;
		if (magnitude < base.Radius * 1.5f)
		{
			FingerID = touch.fingerId;
			if (magnitude > DeadZone)
			{
				Force = (magnitude - DeadZone) / (base.Radius - DeadZone);
				Dir = vector / magnitude * Force;
			}
			else
			{
				Dir = Vector2.zero;
				Force = 0f;
			}
			if ((bool)GuiHUD.Instance)
			{
				GuiHUD.Instance.JoystickDown(Center + Dir * base.Radius);
			}
			Updated = true;
		}
	}

	public override void OnTouchUpdate(Touch touch)
	{
		if (FingerID == touch.fingerId)
		{
			Vector2 vector = touch.position - Center;
			float magnitude = vector.magnitude;
			if (magnitude > base.Radius)
			{
				Dir = vector / magnitude;
				Force = 1f;
			}
			else if (magnitude > DeadZone)
			{
				Force = (magnitude - DeadZone) / (base.Radius - DeadZone);
				Dir = vector / magnitude * Force;
			}
			else
			{
				Dir = Vector2.zero;
				Force = 0f;
			}
			if ((bool)GuiHUD.Instance)
			{
				GuiHUD.Instance.JoystickUpdate(Center + Dir * base.Radius);
			}
			Updated = true;
		}
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
			}
			Updated = false;
		}
	}

	public override bool IsInside(Touch touch)
	{
		return (touch.position - Center).magnitude < base.Radius * 1.5f;
	}
}
