using System.Collections.Generic;
using UnityEngine;

public class PlayerControlsGamepad
{
	private class ViewJoypad
	{
		public int FingerID = -1;

		public Vector2 Center;

		public Vector2 LastTouchPosition;

		public bool FirstDelta;

		private List<Vector2> DeltaPositions = new List<Vector2>();

		private int FilterPositions = 5;

		public bool On
		{
			get
			{
				return FingerID != -1;
			}
		}

		public void SetCenter(Vector2 center)
		{
			Center = center;
			LastTouchPosition = center;
			FirstDelta = true;
			DeltaPositions.Clear();
		}

		public void AddDelta(Vector2 delta)
		{
			if (DeltaPositions.Count == FilterPositions)
			{
				DeltaPositions.RemoveAt(0);
			}
			DeltaPositions.Add(delta);
		}

		public Vector2 GetDelta()
		{
			if (DeltaPositions.Count == 0)
			{
				return Vector2.zero;
			}
			Vector2 zero = Vector2.zero;
			foreach (Vector2 deltaPosition in DeltaPositions)
			{
				zero += deltaPosition;
			}
			return zero / DeltaPositions.Count;
		}
	}

	public enum E_Input
	{
		Fire = 0,
		Reload = 1,
		Roll = 2,
		PrevWeapon = 3,
		NextWeapon = 4,
		Pause = 5,
		MenuOk = 6,
		MenuBack = 7,
		Axis_MoveRight = 8,
		Axis_MoveUp = 9,
		Axis_ViewRight = 10,
		Axis_ViewUp = 11,
		COUNT = 12
	}

	public bool Enabled = true;

	private PlayerControlStates States;

	private ViewJoypad ViewJoystick = new ViewJoypad();

	private static E_WeaponType[] wepons = new E_WeaponType[5]
	{
		E_WeaponType.SMG,
		E_WeaponType.Shotgun,
		E_WeaponType.GrenadeLauncher,
		E_WeaponType.RocketLauncher,
		E_WeaponType.PlasmaRifle
	};

	private float ViewSensitivityX
	{
		get
		{
			return 3.5f * GuiOptions.sensitivity;
		}
	}

	private float ViewSensitivityY
	{
		get
		{
			return 2.4f * GuiOptions.sensitivity;
		}
	}

	public PlayerControlsGamepad(PlayerControlStates inStates)
	{
		States = inStates;
	}

	public static float ClampAngle(float angle, float min, float max)
	{
		angle %= 360f;
		if (angle >= -360f && angle <= 360f)
		{
			if (angle < -360f)
			{
				angle += 360f;
			}
			if (angle > 360f)
			{
				angle -= 360f;
			}
		}
		return Mathf.Clamp(angle, min, max);
	}

	private void UpdateMove()
	{
		Vector2 joydir = Vector2.zero;
		joydir.x = GetGpadAxis(E_Input.Axis_MoveRight);
		joydir.y = GetGpadAxis(E_Input.Axis_MoveUp);
		if (Game.Instance.IsXperiaPlay)
		{
			GetTouchStickJoystick(ref joydir, true);
		}
		float magnitude = joydir.magnitude;
		if (magnitude > 0.001f)
		{
			States.Move.Direction.x = joydir.x;
			States.Move.Direction.z = joydir.y;
			States.Move.Direction.Normalize();
			States._Temp.eulerAngles = new Vector3(0f, Player.Instance.Owner.Transform.rotation.eulerAngles.y, 0f);
			States.Move.Direction = States._Temp.TransformDirection(States.Move.Direction);
			States.Move.Force = magnitude;
		}
	}

	private void UpdateView()
	{
		Vector2 joydir = Vector2.zero;
		if (Game.Instance.IsXperiaPlay)
		{
			if (GuiOptions.m_XperiaPad == GuiOptions.E_XperiPad.Slide)
			{
				UpdateXperiaView2();
				return;
			}
			GetTouchStickJoystick(ref joydir, false);
			joydir.x *= GuiOptions.sensitivity * 2f;
			joydir.y *= GuiOptions.sensitivity * 1.65f;
		}
		else
		{
			joydir.x = GetGpadAxis(E_Input.Axis_ViewRight) * ViewSensitivityX;
			joydir.y = GetGpadAxis(E_Input.Axis_ViewUp) * ViewSensitivityY;
		}
		float x = joydir.x;
		float y = joydir.y;
		if (Mathf.Abs(x) > 0.001f || Mathf.Abs(y) > 0.001f)
		{
			float num = 360f;
			float num2 = num * Time.deltaTime;
			float yaw = ClampAngle(x, 0f - num2, num2);
			float pitch = ClampAngle(y, 0f - num2, num2);
			States.View.SetNewRotation(yaw, pitch);
		}
	}

	// Xperia Play secondary-touchpad input is Android-only and must not be referenced on PSP2.
#if !UNITY_PSP2
	private void UpdateXperiaView2()
	{
		if (!AndroidInput.secondaryTouchEnabled)
		{
			return;
		}
		int touchCountSecondary = AndroidInput.touchCountSecondary;
		for (int i = 0; i < touchCountSecondary; i++)
		{
			Touch secondaryTouch = AndroidInput.GetSecondaryTouch(i);
			if (secondaryTouch.phase == TouchPhase.Began)
			{
				ViewTouchBegin(secondaryTouch);
			}
			else if (secondaryTouch.phase == TouchPhase.Moved || secondaryTouch.phase == TouchPhase.Stationary)
			{
				ViewTouchUpdate(secondaryTouch);
			}
			else if (secondaryTouch.phase == TouchPhase.Ended || secondaryTouch.phase == TouchPhase.Canceled)
			{
				ViewTouchEnd(secondaryTouch);
			}
		}
	}

	private bool InsideRightPad(Touch touch)
	{
		Vector2 vector = new Vector2(786f, 180f);
		Vector2 vector2 = new Vector2(touch.position.x - vector.x, touch.position.y - vector.y);
		if (touch.position.x > (float)(AndroidInput.secondaryTouchWidth / 2) && vector2.sqrMagnitude < 40000f)
		{
			return true;
		}
		return false;
	}
#else
	private void UpdateXperiaView2()
	{
	}

	private bool InsideRightPad(Touch touch)
	{
		return false;
	}
#endif

	private void ViewTouchBegin(Touch touch)
	{
		if (States.View.Enabled && !ViewJoystick.On && InsideRightPad(touch))
		{
			ViewJoystick.FingerID = touch.fingerId;
			ViewJoystick.SetCenter(touch.position);
			States.View.ZeroInput();
		}
	}

	public void ViewTouchUpdate(Touch touch)
	{
		if (States.View.Enabled && ViewJoystick.FingerID == touch.fingerId)
		{
			Vector2 delta = touch.position - ViewJoystick.LastTouchPosition;
			if (ViewJoystick.FirstDelta)
			{
				delta *= 0.25f;
			}
			ViewJoystick.AddDelta(delta);
			Vector2 delta2 = ViewJoystick.GetDelta();
			ViewJoystick.LastTouchPosition = touch.position;
			if (delta.magnitude > 0.01f)
			{
				ViewJoystick.FirstDelta = false;
				float num = 0.225f;
				float num2 = 0.15f;
				float num3 = delta2.x * num;
				float num4 = delta2.y * num2;
				float sensitivity = GuiOptions.sensitivity;
				num3 *= sensitivity;
				num4 *= sensitivity;
				States.View.SetNewRotation(num3, num4);
			}
		}
	}

	public void ViewTouchEnd(Touch touch)
	{
		if (ViewJoystick.FingerID == touch.fingerId)
		{
			ViewJoystick.FingerID = -1;
		}
	}

	public static bool ButtonDown(E_Input inp)
	{
		if (Game.Instance.IsXperiaPlay)
		{
			switch (inp)
			{
			case E_Input.Fire:
				return Input.GetKeyDown(KeyCode.RightShift);
			case E_Input.Reload:
				return Input.GetKeyDown(KeyCode.LeftShift);
			case E_Input.Pause:
				return Input.GetKeyDown(KeyCode.Pause);
			case E_Input.Roll:
				return Input.GetKeyDown("joystick button 2");
			}
		}
		return custom_inputs.Instance.isInputDown[(int)inp];
	}

	public static bool ButtonUp(E_Input inp)
	{
		if (Game.Instance.IsXperiaPlay && inp == E_Input.Fire)
		{
			return Input.GetKeyUp(KeyCode.RightShift);
		}
		return custom_inputs.Instance.isInputUp[(int)inp];
	}

	private void UpdateActions()
	{
		if (Game.Instance.IsXperiaPlay)
		{
			XperiaControls();
			return;
		}
		if (ButtonDown(E_Input.Fire) || Input.GetKeyDown("8"))
		{
			if (Player.Instance.InUseMode)
			{
				States.UseDelegate();
			}
			else
			{
				States.FireDownDelegate();
			}
		}
		if ((ButtonUp(E_Input.Fire) || Input.GetKeyUp("8")) && !Player.Instance.InUseMode)
		{
			States.FireUpDelegate();
		}
		if ((Input.GetKeyDown("joystick button 0") || Input.GetKeyDown("1")) && Player.Instance.InUseMode)
		{
			States.UseDelegate();
		}
		if (ButtonDown(E_Input.Reload) || Input.GetKeyDown("7"))
		{
			States.ReloadDelegate();
		}
		if (ButtonDown(E_Input.Roll) || Input.GetKeyDown("4"))
		{
			States.RollDelegate();
		}
		bool flag = Input.GetKeyDown("escape") && Time.timeSinceLevelLoad > Game.Instance.LastTouchControlTime + 0.25f;
		if ((ButtonDown(E_Input.Pause) || flag) && Game.Instance.GameState != E_GameState.IngameMenu && !GuiHUD.Instance.IsHidden)
		{
			Debug.Log(Time.realtimeSinceStartup + "Pause menu: Escape pressed " + Input.GetKeyDown("escape") + ", last touch " + Game.Instance.LastTouchControlTime + " cur time: " + Time.timeSinceLevelLoad);
			if (MFGuiManager.Instance.FadeState == MFGuiManager.E_Fading.None && (bool)GuiIngameMenu.Instance && GuiIngameMenu.Instance.CanShowAgain())
			{
				GuiHUD.Instance.SwitchToIngameMenu();
			}
		}
		if (ButtonDown(E_Input.PrevWeapon) || Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown("a"))
		{
			ChangeWeaponPrev(States);
		}
		else if (ButtonDown(E_Input.NextWeapon) || Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown("d"))
		{
			ChangeWeaponNext(States);
		}
	}

	public void Update()
	{
		if (Enabled && !(GuiHUD.Instance == null) && !GuiHUD.Instance.IsHidden && (!GuiHUD.Instance || !GuiHUD.Instance.IsPlayingCutscene()))
		{
			if (States.Move.Enabled)
			{
				UpdateMove();
			}
			if (States.View.Enabled)
			{
				UpdateView();
			}
			if (States.ActionsEnabled)
			{
				UpdateActions();
			}
		}
	}

	private static void ChangeWeapon(E_WeaponType type, PlayerControlStates States)
	{
		if (CanChangeToWeapon(type) && (bool)GuiHUD.Instance && (bool)Player.Instance && (bool)Player.Instance.Owner && (bool)Player.Instance.Owner.WeaponComponent && !GuiHUD.Instance.IsWeaponChanging())
		{
			States.ChangeWeaponDelegate(type);
			if ((bool)Player.Instance.Owner.WeaponComponent.GetWeapon(type))
			{
				GuiHUD.Instance.OnCurrentWeaponChanged(type);
			}
		}
	}

	private static bool CanChangeToWeapon(E_WeaponType type)
	{
		ComponentWeapons weaponComponent = Player.Instance.Owner.WeaponComponent;
		WeaponBase weapon = weaponComponent.GetWeapon(type);
		if (weapon == null || !weapon.HasAnyAmmo || weaponComponent.CurrentWeapon == type)
		{
			return false;
		}
		return true;
	}

	private void XperiaControls()
	{
		if (ButtonDown(E_Input.Fire))
		{
			if (Player.Instance.InUseMode)
			{
				States.UseDelegate();
			}
			else
			{
				States.FireDownDelegate();
			}
		}
		if (ButtonUp(E_Input.Fire) && !Player.Instance.InUseMode)
		{
			States.FireUpDelegate();
		}
		if (Input.GetKeyDown("joystick button 0") && Player.Instance.InUseMode)
		{
			States.UseDelegate();
		}
		if (ButtonDown(E_Input.Reload))
		{
			States.ReloadDelegate();
		}
		if (ButtonDown(E_Input.Roll))
		{
			States.RollDelegate();
		}
		if (ButtonDown(E_Input.Pause) && Game.Instance.GameState != E_GameState.IngameMenu && !GuiHUD.Instance.IsHidden && MFGuiManager.Instance.FadeState == MFGuiManager.E_Fading.None && (bool)GuiIngameMenu.Instance && GuiIngameMenu.Instance.CanShowAgain())
		{
			GuiHUD.Instance.SwitchToIngameMenu();
		}
		if (Input.GetKeyDown("joystick button 1"))
		{
			ChangeWeaponPrev(States);
		}
		else if (Input.GetKeyDown("joystick button 3"))
		{
			ChangeWeaponNext(States);
		}
	}

	// Xperia Play secondary-touchpad input is Android-only and must not be referenced on PSP2.
#if !UNITY_PSP2
	public void GetTouchStickJoystick(ref Vector2 joydir, bool left)
	{
		if (!AndroidInput.secondaryTouchEnabled)
		{
			return;
		}
		Vector2 vector = ((!left) ? new Vector2(786f, 180f) : new Vector2(180f, 180f));
		int touchCountSecondary = AndroidInput.touchCountSecondary;
		for (int i = 0; i < touchCountSecondary; i++)
		{
			Touch secondaryTouch = AndroidInput.GetSecondaryTouch(i);
			Vector2 joydir2 = new Vector2(secondaryTouch.position.x - vector.x, secondaryTouch.position.y - vector.y);
			if (((left && secondaryTouch.position.x < (float)(AndroidInput.secondaryTouchWidth / 2)) || (!left && secondaryTouch.position.x > (float)(AndroidInput.secondaryTouchWidth / 2))) && joydir2.sqrMagnitude < 40000f)
			{
				if (left)
				{
					ComputeJoyVector(ref joydir2, 30f, 100f, 1f);
				}
				else
				{
					ComputeJoyVector(ref joydir2, 10f, 100f, 1.5f);
				}
				joydir = joydir2;
				if (left)
				{
					joydir.y = 0f - joydir.y;
				}
			}
		}
	}
#else
	public void GetTouchStickJoystick(ref Vector2 joydir, bool left)
	{
	}
#endif

	private static void ComputeJoyVector(ref Vector2 joydir, float deadzone, float padRadius, float clamp)
	{
		if (joydir.x > 0f - deadzone && joydir.x < deadzone)
		{
			joydir.x = 0f;
		}
		if (joydir.y > 0f - deadzone && joydir.y < deadzone)
		{
			joydir.y = 0f;
		}
		joydir /= padRadius;
		joydir.x = Mathf.Clamp(joydir.x, 0f - clamp, clamp);
		joydir.y = Mathf.Clamp(joydir.y, 0f - clamp, clamp);
		joydir.x = EaseJoyValue(joydir.x);
		joydir.y = EaseJoyValue(joydir.y);
		joydir.x = DropSmallJoyValue(joydir.x);
		joydir.y = DropSmallJoyValue(joydir.y);
	}

	private static float EaseJoyValue(float val)
	{
		if (val >= 0f && val <= 1f)
		{
			return Mathfx.Hermite(0f, 1f, val);
		}
		if (val < 0f && val >= -1f)
		{
			return Mathfx.Hermite(0f, -1f, 0f - val);
		}
		return val;
	}

	private static float DropSmallJoyValue(float val)
	{
		if ((val > 0f && val < 0.01f) || (val < 0f && val > -0.01f))
		{
			return 0f;
		}
		return val;
	}

	private static int GetWeponIndex(E_WeaponType wt)
	{
		for (int i = 0; i < wepons.Length; i++)
		{
			if (wt == wepons[i])
			{
				return i;
			}
		}
		return 0;
	}

	public static void ChangeWeaponPrev(PlayerControlStates states)
	{
		if ((bool)GuiHUD.Instance && (bool)Player.Instance && (bool)Player.Instance.Owner && (bool)Player.Instance.Owner.WeaponComponent && !GuiHUD.Instance.IsWeaponChanging())
		{
			E_WeaponType currentWeapon = Player.Instance.Owner.WeaponComponent.CurrentWeapon;
			int weponIndex = GetWeponIndex(currentWeapon);
			int num = weponIndex;
			do
			{
				num = ((num > 0) ? (num - 1) : (wepons.Length - 1));
			}
			while (!CanChangeToWeapon(wepons[num]) && num != weponIndex);
			E_WeaponType e_WeaponType = wepons[num];
			if (e_WeaponType != currentWeapon)
			{
				ChangeWeapon(e_WeaponType, states);
			}
		}
	}

	public static void ChangeWeaponNext(PlayerControlStates states)
	{
		if ((bool)GuiHUD.Instance && (bool)Player.Instance && (bool)Player.Instance.Owner && (bool)Player.Instance.Owner.WeaponComponent && !GuiHUD.Instance.IsWeaponChanging())
		{
			E_WeaponType currentWeapon = Player.Instance.Owner.WeaponComponent.CurrentWeapon;
			int weponIndex = GetWeponIndex(currentWeapon);
			int num = weponIndex;
			do
			{
				num = (num + 1) % wepons.Length;
			}
			while (!CanChangeToWeapon(wepons[num]) && num != weponIndex);
			E_WeaponType e_WeaponType = wepons[num];
			if (e_WeaponType != currentWeapon)
			{
				ChangeWeapon(e_WeaponType, states);
			}
		}
	}

	private float GetGpadAxis(E_Input inp)
	{
		string axisName = custom_inputs.Instance.GetAxisName((int)inp);
		if (axisName != string.Empty)
		{
			if (inp == E_Input.Axis_ViewUp)
			{
				return 0f - Input.GetAxisRaw(axisName);
			}
			return Input.GetAxisRaw(axisName);
		}
		switch (inp)
		{
		case E_Input.Axis_MoveRight:
			return Input.GetAxisRaw("HorizontalMove");
		case E_Input.Axis_MoveUp:
			return Input.GetAxisRaw("VerticalMove");
		case E_Input.Axis_ViewRight:
			return Input.GetAxisRaw("HorizontalView");
		case E_Input.Axis_ViewUp:
			return Input.GetAxisRaw("VerticalView");
		default:
			return 0f;
		}
	}
}
