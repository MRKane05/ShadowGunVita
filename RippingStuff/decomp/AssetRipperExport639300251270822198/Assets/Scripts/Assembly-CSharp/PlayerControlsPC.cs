using UnityEngine;

public class PlayerControlsPC
{
	public class MouseCameraControl
	{
		private float sensitivityX = 7.4f;

		private float sensitivityY = 3.7f;

		public bool Changed;

		public float OutYaw;

		public float OutPitch;

		private bool CursorLocked
		{
			get
			{
				return Screen.lockCursor;
			}
			set
			{
				Screen.lockCursor = value;
			}
		}

		public void SwitchCursor()
		{
			CursorLocked = !CursorLocked;
		}

		private float GetSensitivityX()
		{
			return sensitivityX * GuiOptions.sensitivity;
		}

		private float GetSensitivityY()
		{
			return sensitivityY * GuiOptions.sensitivity;
		}

		public void Update()
		{
			OutYaw = 0f;
			OutPitch = 0f;
			if (CursorLocked)
			{
				float num = Input.GetAxis("MouseX") * GetSensitivityX();
				float num2 = Input.GetAxis("MouseY") * GetSensitivityY();
				Changed = Mathf.Abs(num) > 0.001f || Mathf.Abs(num2) > 0.001f;
				if (Changed)
				{
					float angle = num;
					float angle2 = 0f - num2;
					OutYaw = ClampAngle(angle, -160f, 160f);
					OutPitch = ClampAngle(angle2, -160f, 160f);
				}
			}
		}
	}

	private bool Enabled = true;

	private PlayerControlStates States;

	public MouseCameraControl MouseCameraCtrl = new MouseCameraControl();

	private bool lockCursorAfterStart = true;

	public PlayerControlsPC(PlayerControlStates inStates)
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

	private void UpdateMouseInteractionTouch()
	{
		if (Input.GetMouseButtonDown(0) && !Screen.lockCursor)
		{
			Vector2 position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			InteractionObject interactionObject = PlayerControlsTouch.TouchedInteractionIcon(position);
			if (interactionObject != null)
			{
				States.UseObjectDelegate(interactionObject);
			}
		}
	}

	public void Update()
	{
		if (!Enabled)
		{
			return;
		}
		if (Time.timeSinceLevelLoad > 0f && lockCursorAfterStart)
		{
			Screen.lockCursor = true;
			lockCursorAfterStart = false;
		}
		if (Input.GetKeyDown("return"))
		{
			MouseCameraCtrl.SwitchCursor();
		}
		UpdateMouseInteractionTouch();
		if (States.Move.Enabled)
		{
			Vector2 vector = default(Vector2);
			vector.x = Input.GetAxis("HorizontalMove");
			vector.y = Input.GetAxis("VerticalMove");
			float magnitude = vector.magnitude;
			if (magnitude > 0.001f)
			{
				States.Move.Direction.x = vector.x;
				States.Move.Direction.z = vector.y;
				States.Move.Direction.Normalize();
				States._Temp.eulerAngles = new Vector3(0f, Player.Instance.Owner.Transform.rotation.eulerAngles.y, 0f);
				States.Move.Direction = States._Temp.TransformDirection(States.Move.Direction);
				States.Move.Force = magnitude;
			}
		}
		if (States.View.Enabled)
		{
			MouseCameraCtrl.Update();
			if (MouseCameraCtrl.Changed)
			{
				States.View.SetNewRotation(MouseCameraCtrl.OutYaw, MouseCameraCtrl.OutPitch);
			}
		}
		if (!States.ActionsEnabled)
		{
			return;
		}
		if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
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
		if ((Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space)) && !Player.Instance.InUseMode)
		{
			States.FireUpDelegate();
		}
		if (Input.GetMouseButtonDown(1) || Input.GetKeyDown("r"))
		{
			States.ReloadDelegate();
		}
		if (Input.GetKeyDown(KeyCode.RightControl) || Input.GetKeyDown(KeyCode.LeftControl))
		{
			States.RollDelegate();
		}
		if (Input.GetKeyDown("escape") && Game.Instance.GameState != E_GameState.IngameMenu && !GuiHUD.Instance.IsHidden && MFGuiManager.Instance.FadeState == MFGuiManager.E_Fading.None && (bool)GuiIngameMenu.Instance && GuiIngameMenu.Instance.CanShowAgain())
		{
			GuiHUD.Instance.SwitchToIngameMenu();
		}
		if (Input.GetKeyDown("1"))
		{
			ChangeWeapon(E_WeaponType.SMG);
		}
		if (Input.GetKeyDown("2"))
		{
			ChangeWeapon(E_WeaponType.Shotgun);
		}
		if (Input.GetKeyDown("3"))
		{
			ChangeWeapon(E_WeaponType.GrenadeLauncher);
		}
		if (Input.GetKeyDown("4"))
		{
			ChangeWeapon(E_WeaponType.RocketLauncher);
		}
		if (Input.GetKeyDown("5"))
		{
			ChangeWeapon(E_WeaponType.PlasmaRifle);
		}
	}

	private void ChangeWeapon(E_WeaponType type)
	{
		States.ChangeWeaponDelegate(type);
		WeaponBase weapon = Player.Instance.Owner.WeaponComponent.GetWeapon(type);
		if ((bool)weapon)
		{
			GuiHUD.Instance.OnCurrentWeaponChanged(type);
		}
	}
}
