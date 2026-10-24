using UnityEngine;

// PS Vita button map - the one place the physical Vita layout is described.
//
// custom_inputs.VitaDefault builds the default control config from these constants, and
// MenuController uses the d-pad helpers below for menu navigation. If a button comes out
// wrong on device, correct it here and both follow.
//
// Verified Vita joystick indices (see VitaButtonProbe to re-check on hardware):
//     0 cross          1 circle          2 square         3 triangle
//     4 left shoulder  5 right shoulder  6 select         7 start
//     8 dpad up        9 dpad right     10 dpad down     11 dpad left
// Axes: 0/1 = left stick X/Y, 3/4 = right stick X/Y, 5 = d-pad horizontal.
// Note there is no axis 2 on Vita - the right stick sits one index higher than the
// 3rd/4th axis the Android pads in custom_inputs use, which is why the Vita defaults
// pick Joystick_4b / Joystick_5a where those pads pick Joystick_3b / Joystick_4a.
//
// The codes below are the "any joystick" KeyCodes (JoystickButtonN), not Joystick1ButtonN.
// The Vita pad is not guaranteed to enumerate as joystick 1, and the any-joystick codes
// match either way.
public static class VitaInput
{
	public const KeyCode BTN_CROSS = KeyCode.JoystickButton0;

	public const KeyCode BTN_CIRCLE = KeyCode.JoystickButton1;

	public const KeyCode BTN_SQUARE = KeyCode.JoystickButton2;

	public const KeyCode BTN_TRIANGLE = KeyCode.JoystickButton3;

	public const KeyCode BTN_L = KeyCode.JoystickButton4;

	public const KeyCode BTN_R = KeyCode.JoystickButton5;

	public const KeyCode BTN_SELECT = KeyCode.JoystickButton6;

	public const KeyCode BTN_START = KeyCode.JoystickButton7;

	public const KeyCode DPAD_UP = KeyCode.JoystickButton8;

	public const KeyCode DPAD_RIGHT = KeyCode.JoystickButton9;

	public const KeyCode DPAD_DOWN = KeyCode.JoystickButton10;

	public const KeyCode DPAD_LEFT = KeyCode.JoystickButton11;

	private static int isVita = -1;

	public static bool IsVita
	{
		get
		{
			if (isVita < 0)
			{
				isVita = ((Application.platform != RuntimePlatform.PSP2) ? 0 : 1);
			}
			return isVita == 1;
		}
	}

	// -1 left, +1 right, 0 centred. The Vita d-pad is buttons, not an axis, so menu code
	// that expects an axis reads it through these.
	public static float DpadX()
	{
		if (Input.GetKey(DPAD_LEFT))
		{
			return -1f;
		}
		if (Input.GetKey(DPAD_RIGHT))
		{
			return 1f;
		}
		return 0f;
	}

	// +1 up, -1 down, matching the sign of the VerticalMove axis (which is inverted in
	// the InputManager so that pushing the stick up reads positive).
	public static float DpadY()
	{
		if (Input.GetKey(DPAD_UP))
		{
			return 1f;
		}
		if (Input.GetKey(DPAD_DOWN))
		{
			return -1f;
		}
		return 0f;
	}
}
