using System.Collections.Generic;
using UnityEngine;

public class PlayerControlsMoga
{
	private enum E_InputAction
	{
		R1 = 0,
		R2 = 1,
		Reload = 2,
		Pause = 3,
		PrevWeapon = 4,
		NextWeapon = 5,
		Roll = 6,
		Use = 7,
		DpadLeft = 8,
		DpadRight = 9,
		COUNT = 10
	}

	private class KeyInput
	{
		public delegate void Delegate();

		public int key;

		public bool isDown;

		public Delegate onKeyDown;

		public Delegate onKeyUp;
	}

	private Dictionary<E_InputAction, KeyInput> m_KeyInput = new Dictionary<E_InputAction, KeyInput>();

	public bool Enabled = true;

	private PlayerControlStates States;

	private float ViewSensitivityX
	{
		get
		{
			return 1f * GuiOptions.sensitivity;
		}
	}

	private float ViewSensitivityY
	{
		get
		{
			return 0.75f * GuiOptions.sensitivity;
		}
	}

	public PlayerControlsMoga(PlayerControlStates inStates)
	{
		States = inStates;
		m_KeyInput.Add(E_InputAction.R1, new KeyInput
		{
			key = 103,
			onKeyDown = OnR1Down,
			onKeyUp = OnR1Up
		});
		m_KeyInput.Add(E_InputAction.R2, new KeyInput
		{
			key = 105,
			onKeyDown = OnR2Down,
			onKeyUp = OnR2Up
		});
		m_KeyInput.Add(E_InputAction.Roll, new KeyInput
		{
			key = 102,
			onKeyDown = OnRollDown
		});
		m_KeyInput.Add(E_InputAction.Reload, new KeyInput
		{
			key = 100,
			onKeyDown = OnReloadDown
		});
		m_KeyInput.Add(E_InputAction.Pause, new KeyInput
		{
			key = 108,
			onKeyDown = OnPauseDown
		});
		m_KeyInput.Add(E_InputAction.PrevWeapon, new KeyInput
		{
			key = 99,
			onKeyDown = OnPrevDown
		});
		m_KeyInput.Add(E_InputAction.NextWeapon, new KeyInput
		{
			key = 97,
			onKeyDown = OnNextDown
		});
		m_KeyInput.Add(E_InputAction.DpadLeft, new KeyInput
		{
			key = 21,
			onKeyDown = OnDpadLeft
		});
		m_KeyInput.Add(E_InputAction.DpadRight, new KeyInput
		{
			key = 22,
			onKeyDown = OnDpadRight
		});
		m_KeyInput.Add(E_InputAction.Use, new KeyInput
		{
			key = 96,
			onKeyDown = OnUseDown
		});
	}

	private void UpdateMove()
	{
		if (MogaGamepad.IsConnected())
		{
			Vector2 zero = Vector2.zero;
			zero.x = MogaGamepad.GetAxis(0);
			zero.y = 0f - MogaGamepad.GetAxis(1);
			float magnitude = zero.magnitude;
			if (magnitude > 0.001f)
			{
				States.Move.Direction.x = zero.x;
				States.Move.Direction.z = zero.y;
				States.Move.Direction.Normalize();
				States._Temp.eulerAngles = new Vector3(0f, Player.Instance.Owner.Transform.rotation.eulerAngles.y, 0f);
				States.Move.Direction = States._Temp.TransformDirection(States.Move.Direction);
				States.Move.Force = magnitude;
			}
		}
	}

	private void UpdateView()
	{
		if (MogaGamepad.IsConnected())
		{
			float num = MogaGamepad.GetAxis(11) * ViewSensitivityX;
			float num2 = MogaGamepad.GetAxis(14) * ViewSensitivityY;
			if (Mathf.Abs(num) > 0.001f || Mathf.Abs(num2) > 0.001f)
			{
				float num3 = 360f;
				float num4 = num3 * Time.deltaTime;
				float yaw = PlayerControlsGamepad.ClampAngle(num, 0f - num4, num4);
				float pitch = PlayerControlsGamepad.ClampAngle(num2, 0f - num4, num4);
				States.View.SetNewRotation(yaw, pitch);
			}
		}
	}

	private void UpdateKeys()
	{
		if (!MogaGamepad.IsConnected())
		{
			return;
		}
		foreach (E_InputAction key in m_KeyInput.Keys)
		{
			KeyInput keyInput = m_KeyInput[key];
			if (MogaGamepad.GetKeyCode(keyInput.key) == 0 && !keyInput.isDown)
			{
				keyInput.isDown = true;
				if (keyInput.onKeyDown != null)
				{
					keyInput.onKeyDown();
				}
			}
			else if (MogaGamepad.GetKeyCode(keyInput.key) == 1)
			{
				keyInput.isDown = false;
				if (keyInput.onKeyUp != null)
				{
					keyInput.onKeyUp();
				}
			}
		}
	}

	private bool ButtonDown(E_InputAction inp)
	{
		return m_KeyInput[inp].isDown;
	}

	private bool ButtonUp(E_InputAction inp)
	{
		return !m_KeyInput[inp].isDown;
	}

	private void OnR1Down()
	{
		if (MogaGamepad.IsMogaPro())
		{
			OnReloadDown();
		}
		else
		{
			OnFireDown();
		}
	}

	private void OnR1Up()
	{
		if (!MogaGamepad.IsMogaPro())
		{
			OnFireUp();
		}
	}

	private void OnR2Down()
	{
		if (MogaGamepad.IsMogaPro())
		{
			OnFireDown();
		}
	}

	private void OnR2Up()
	{
		if (MogaGamepad.IsMogaPro())
		{
			OnFireUp();
		}
	}

	private void OnFireDown()
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

	private void OnReloadDown()
	{
		States.ReloadDelegate();
	}

	private void OnPauseDown()
	{
		if (MFGuiManager.Instance.FadeState == MFGuiManager.E_Fading.None && (bool)GuiIngameMenu.Instance && GuiIngameMenu.Instance.CanShowAgain())
		{
			GuiHUD.Instance.SwitchToIngameMenu();
		}
	}

	private void OnFireUp()
	{
		if (!Player.Instance.InUseMode)
		{
			States.FireUpDelegate();
		}
	}

	private void OnUseDown()
	{
		if (Player.Instance.InUseMode)
		{
			States.UseDelegate();
		}
	}

	private void OnNextDown()
	{
		PlayerControlsGamepad.ChangeWeaponNext(States);
	}

	private void OnPrevDown()
	{
		PlayerControlsGamepad.ChangeWeaponPrev(States);
	}

	private void OnRollDown()
	{
		States.RollDelegate();
	}

	private void OnDpadLeft()
	{
		if (MogaGamepad.IsMogaPro())
		{
			OnPrevDown();
		}
	}

	private void OnDpadRight()
	{
		if (MogaGamepad.IsMogaPro())
		{
			OnNextDown();
		}
	}

	private void UpdateActions()
	{
		UpdateKeys();
	}

	public void Update()
	{
		if (Enabled)
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
}
