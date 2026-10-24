using UnityEngine;

// Drop this on any GameObject in a Vita build to discover the real joystick button and
// axis indices. Every press is logged, so the constants in VitaInput - and the axis
// choices in custom_inputs.VitaDefault - can be corrected if something turns out to be
// mapped elsewhere on hardware.
//
// The axis names below are this project's InputManager entries, so the log reads back in
// the same vocabulary the control config uses:
//     Joystick_3a/3b = joystick axis 2    Joystick_6a/6b = joystick axis 5
//     Joystick_4a/4b = joystick axis 3    Joystick_7a/7b = joystick axis 6
//     Joystick_5a/5b = joystick axis 4    Joystick_8a/8b = joystick axis 7
// ("a" is the inverted variant of the same physical axis, "b" the direct one.)
public class VitaButtonProbe : MonoBehaviour
{
	public bool logAxes;

	private static readonly string[] AxisNames = new string[8]
	{
		"HorizontalMove",
		"VerticalMove",
		"Joystick_3b",
		"Joystick_4b",
		"Joystick_5b",
		"Joystick_6b",
		"Joystick_7b",
		"Joystick_8b"
	};

	private void Update()
	{
		for (int i = 0; i < 20; i++)
		{
			if (Input.GetKeyDown("joystick button " + i))
			{
				Debug.Log("[VitaProbe] joystick button " + i + " DOWN");
			}
		}
		if (!logAxes)
		{
			return;
		}
		for (int j = 0; j < AxisNames.Length; j++)
		{
			float v = Input.GetAxisRaw(AxisNames[j]);
			if (Mathf.Abs(v) > 0.35f)
			{
				Debug.Log("[VitaProbe] " + AxisNames[j] + " (joystick axis " + j + ") = " + v);
			}
		}
	}
}
