using UnityEngine;

public class MenuController
{
	private class Button
	{
		public bool Pressed;

		public bool FirstQuery;

		public float LastRelaxPos;

		public float LastPressedPos;

		public void Reset()
		{
			Pressed = false;
			FirstQuery = true;
			LastRelaxPos = 0f;
			LastPressedPos = 0f;
		}
	}

	private enum E_Dir
	{
		LEFT = 0,
		RIGHT = 1,
		UP = 2,
		DOWN = 3
	}

	private Button[] Buttons = new Button[4]
	{
		new Button(),
		new Button(),
		new Button(),
		new Button()
	};

	private float DeadZone = 0.1f;

	private float MoveDelta = 0.4f;

	private bool m_MogaButtonDownA;

	private bool m_MogaButtonDownB;

	private bool m_DpadLeft;

	private bool m_DpadRight;

	private bool m_DpadUp;

	private bool m_DpadDown;

	public MenuController()
	{
		for (int i = 0; i < Buttons.Length; i++)
		{
			Buttons[i].Reset();
		}
	}

	public void Update()
	{
		Button button = Buttons[0];
		Button button2 = Buttons[1];
		float num = Input.GetAxisRaw("HorizontalMove");
		if (MogaGamepad.IsConnected())
		{
			num = MogaGamepad.GetAxis(0);
		}
		// Joystick_5b is joystick axis 4 - the d-pad's horizontal on the Android pads this
		// shipped with. On Vita that index is the right stick's vertical, so reading it
		// here let the right stick scroll menus sideways. The Vita d-pad is buttons.
		float axisRaw = ((!VitaInput.IsVita) ? Input.GetAxisRaw("Joystick_5b") : VitaInput.DpadX());
		if (Mathf.Abs(axisRaw) > Mathf.Abs(num))
		{
			num = axisRaw;
		}
		if (Game.Instance.NVidiaShiledCached)
		{
			float axisRaw2 = Input.GetAxisRaw("Joystick_5b");
			if (Mathf.Abs(axisRaw2) > Mathf.Abs(num))
			{
				num = axisRaw2;
			}
		}
		if (!button2.Pressed && num > button2.LastRelaxPos + MoveDelta)
		{
			button2.Pressed = true;
			button2.LastPressedPos = num;
		}
		else if (button2.Pressed && num < button2.LastPressedPos - MoveDelta)
		{
			button2.Reset();
			button2.LastRelaxPos = num;
		}
		else if (!button.Pressed && num < button.LastRelaxPos - MoveDelta)
		{
			button.Pressed = true;
			button.LastPressedPos = num;
		}
		else if (button.Pressed && num > button.LastPressedPos + MoveDelta)
		{
			button.Reset();
			button.LastRelaxPos = num;
		}
		if (button2.Pressed)
		{
			if (button2.LastPressedPos < num)
			{
				button2.LastPressedPos = num;
			}
		}
		else if (button2.LastRelaxPos > num)
		{
			if (num > DeadZone)
			{
				button2.LastRelaxPos = num;
			}
			else
			{
				button2.LastRelaxPos = DeadZone;
			}
		}
		if (button.Pressed)
		{
			if (button.LastPressedPos > num)
			{
				button.LastPressedPos = num;
			}
		}
		else if (button.LastRelaxPos < num)
		{
			if (num < 0f - DeadZone)
			{
				button.LastRelaxPos = num;
			}
			else
			{
				button.LastRelaxPos = 0f - DeadZone;
			}
		}
		Button button3 = Buttons[3];
		Button button4 = Buttons[2];
		float num2 = Input.GetAxisRaw("VerticalMove");
		if (MogaGamepad.IsConnected())
		{
			num2 = 0f - MogaGamepad.GetAxis(1);
		}
		// Joystick_6a is joystick axis 5 - the d-pad's vertical on those pads, but the
		// d-pad's horizontal on Vita. Same substitution as above.
		float axisRaw3 = ((!VitaInput.IsVita) ? Input.GetAxisRaw("Joystick_6a") : VitaInput.DpadY());
		if (Mathf.Abs(axisRaw3) > Mathf.Abs(num2))
		{
			num2 = axisRaw3;
		}
		if (Game.Instance.NVidiaShiledCached)
		{
			float axisRaw4 = Input.GetAxisRaw("Joystick_6a");
			if (Mathf.Abs(axisRaw4) > Mathf.Abs(num2))
			{
				num2 = axisRaw4;
			}
		}
		if (!button4.Pressed && num2 > button4.LastRelaxPos + MoveDelta)
		{
			button4.Pressed = true;
			button4.LastPressedPos = num2;
		}
		else if (button4.Pressed && num2 < button4.LastPressedPos - MoveDelta)
		{
			button4.Reset();
			button4.LastRelaxPos = num2;
		}
		else if (!button3.Pressed && num2 < button3.LastRelaxPos - MoveDelta)
		{
			button3.Pressed = true;
			button3.LastPressedPos = num2;
		}
		else if (button3.Pressed && num2 > button3.LastPressedPos + MoveDelta)
		{
			button3.Reset();
			button3.LastRelaxPos = num2;
		}
		if (button4.Pressed)
		{
			if (button4.LastPressedPos < num2)
			{
				button4.LastPressedPos = num2;
			}
		}
		else if (button4.LastRelaxPos > num2)
		{
			if (num2 > DeadZone)
			{
				button4.LastRelaxPos = num2;
			}
			else
			{
				button4.LastRelaxPos = DeadZone;
			}
		}
		if (button3.Pressed)
		{
			if (button3.LastPressedPos > num2)
			{
				button3.LastPressedPos = num2;
			}
		}
		else if (button3.LastRelaxPos < num2)
		{
			if (num2 < 0f - DeadZone)
			{
				button3.LastRelaxPos = num2;
			}
			else
			{
				button3.LastRelaxPos = 0f - DeadZone;
			}
		}
		MogaUpdate();
	}

	public bool PressedLeft()
	{
		if (Buttons[0].Pressed && Buttons[0].FirstQuery)
		{
			Buttons[0].FirstQuery = false;
			return true;
		}
		return MogaDpadLeft();
	}

	public bool PressedRight()
	{
		if (Buttons[1].Pressed && Buttons[1].FirstQuery)
		{
			Buttons[1].FirstQuery = false;
			return true;
		}
		return MogaDpadRight();
	}

	public bool PressedUp()
	{
		if (Buttons[2].Pressed && Buttons[2].FirstQuery)
		{
			Buttons[2].FirstQuery = false;
			return true;
		}
		return MogaDpadUp();
	}

	public bool PressedDown()
	{
		if (Buttons[3].Pressed && Buttons[3].FirstQuery)
		{
			Buttons[3].FirstQuery = false;
			return true;
		}
		return MogaDpadDown();
	}

	public bool MogaDpadLeft()
	{
		if (MogaGamepad.IsConnected() && MogaGamepad.IsMogaPro() && MogaGamepad.GetKeyCode(21) == 0 && !m_DpadLeft)
		{
			m_DpadLeft = true;
			return true;
		}
		return false;
	}

	public bool MogaDpadRight()
	{
		if (MogaGamepad.IsConnected() && MogaGamepad.IsMogaPro() && MogaGamepad.GetKeyCode(22) == 0 && !m_DpadRight)
		{
			m_DpadRight = true;
			return true;
		}
		return false;
	}

	public bool MogaDpadUp()
	{
		if (MogaGamepad.IsConnected() && MogaGamepad.IsMogaPro() && MogaGamepad.GetKeyCode(19) == 0 && !m_DpadUp)
		{
			m_DpadUp = true;
			return true;
		}
		return false;
	}

	public bool MogaDpadDown()
	{
		if (MogaGamepad.IsConnected() && MogaGamepad.IsMogaPro() && MogaGamepad.GetKeyCode(20) == 0 && !m_DpadDown)
		{
			m_DpadDown = true;
			return true;
		}
		return false;
	}

	public bool PressedOk()
	{
		if (PlayerControlsGamepad.ButtonDown(PlayerControlsGamepad.E_Input.MenuOk) || PlayerControlsGamepad.ButtonDown(PlayerControlsGamepad.E_Input.Fire) || Input.GetKeyDown("1") || Input.GetKeyDown("8") || MogaButtonOk())
		{
			return true;
		}
		return false;
	}

	public bool PressedBack()
	{
		if (PlayerControlsGamepad.ButtonDown(PlayerControlsGamepad.E_Input.MenuBack) || Input.GetKeyDown("escape") || MogaButtonBack())
		{
			return true;
		}
		return false;
	}

	private bool MogaButtonOk()
	{
		if (!MogaGamepad.IsConnected())
		{
			return false;
		}
		if (MogaGamepad.GetKeyCode(96) == 0 && !m_MogaButtonDownA)
		{
			m_MogaButtonDownA = true;
			return true;
		}
		return false;
	}

	private bool MogaButtonBack()
	{
		if (!MogaGamepad.IsConnected())
		{
			return false;
		}
		if (MogaGamepad.GetKeyCode(97) == 0 && !m_MogaButtonDownB)
		{
			m_MogaButtonDownB = true;
			return true;
		}
		return false;
	}

	private void MogaUpdate()
	{
		if (MogaGamepad.GetKeyCode(96) == 1)
		{
			m_MogaButtonDownA = false;
		}
		if (MogaGamepad.GetKeyCode(97) == 1)
		{
			m_MogaButtonDownB = false;
		}
		if (MogaGamepad.IsMogaPro() && MogaGamepad.GetKeyCode(20) == 1)
		{
			m_DpadDown = false;
		}
		if (MogaGamepad.IsMogaPro() && MogaGamepad.GetKeyCode(19) == 1)
		{
			m_DpadUp = false;
		}
		if (MogaGamepad.IsMogaPro() && MogaGamepad.GetKeyCode(21) == 1)
		{
			m_DpadLeft = false;
		}
		if (MogaGamepad.IsMogaPro() && MogaGamepad.GetKeyCode(22) == 1)
		{
			m_DpadRight = false;
		}
	}
}
