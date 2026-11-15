using UnityEngine;

public class PlayerControlStates
{
	public class MoveState
	{
		public bool Enabled = true;

		public Vector3 Direction = default(Vector3);

		public float Force;

		public void ZeroInput()
		{
			Direction = Vector3.zero;
			Force = 0f;
		}
	}

	public class ViewState
	{
		public bool Enabled = true;

		public float YawAdd;

		public float PitchAdd;

		public void ZeroInput()
		{
			YawAdd = 0f;
			PitchAdd = 0f;
		}

		public void SetNewRotation(float Yaw, float Pitch)
		{
			YawAdd = Yaw;
			PitchAdd = ((!GuiOptions.invertYAxis) ? Pitch : (0f - Pitch));
		}
	}

	public delegate void ButtonDelegate();

	public delegate void TouchDelegate(InteractionObject obj);

	public delegate void WeaponDelegate(E_WeaponType weaponType);

	public MoveState Move = new MoveState();

	public ViewState View = new ViewState();

	public bool ActionsEnabled = true;

	public bool Fire;

	public bool Use;

	public ButtonDelegate FireDownDelegate;

	public ButtonDelegate FireUpDelegate;

	public ButtonDelegate UseDelegate;

	public ButtonDelegate ReloadDelegate;

	public ButtonDelegate RollDelegate;

	public TouchDelegate UseObjectDelegate;

	public WeaponDelegate ChangeWeaponDelegate;

	public Transform _Temp;

	public Transform MainCameraTransfom;

	public PlayerControlsTouch TouchControls;

	private PlayerControlsPC PCControls;

	private PlayerControlsGamepad GamepadControls;

	private PlayerControlsMoga MogaControls;

	public void Start()
	{
		GameObject gameObject = new GameObject();
		_Temp = gameObject.transform;
		MainCameraTransfom = Camera.mainCamera.transform;
		TouchControls = new PlayerControlsTouch(this);
		TouchControls.Start();
		GamepadControls = new PlayerControlsGamepad(this);
		MogaControls = new PlayerControlsMoga(this);
	}

	public void SwitchToUseMode()
	{
		GuiHUD.Instance.ShowActionButton(GuiHUD.E_ActionButton.Use);
		Move.Enabled = true;
	}

	public void SwitchToCombatMode()
	{
		GuiHUD.Instance.ShowActionButton(GuiHUD.E_ActionButton.Fire);
		Move.Enabled = true;
	}

	public void DisableInput()
	{
		Reset();
		GuiHUD.Instance.ShowActionButton(GuiHUD.E_ActionButton.None);
		Move.Enabled = false;
		View.Enabled = false;
		ActionsEnabled = false;
	}

	public void EnableInput()
	{
		SwitchToCombatMode();
		Move.Enabled = true;
		View.Enabled = true;
		ActionsEnabled = true;
	}

	public void Reset()
	{
		Fire = false;
		Move.Direction = Vector3.zero;
		Move.Force = 0f;
		if (TouchControls != null)
		{
			TouchControls.Reset();
		}
	}

	public void Update()
	{
		Move.ZeroInput();
		View.ZeroInput();
		Use = false;
		if (Game.Instance.GameState == E_GameState.Game)
		{
			if (PCControls != null)
			{
				PCControls.Update();
			}
			if (MogaControls != null)
			{
				MogaControls.Update();
			}
			if (GamepadControls != null)
			{
				GamepadControls.Update();
			}
			if (TouchControls != null)
			{
				TouchControls.Update();
			}
		}
	}
}
