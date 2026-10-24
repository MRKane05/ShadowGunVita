using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlsTouch
{
	public class JoystickBaseOld
	{
		public int FingerID = -1;

		public Vector2 Center;

		public float Left;

		public float Bottom;

		public float Right;

		public float Top;

		public Vector2 LastTouchPosition;

		public bool FirstDelta;

		private List<Vector2> DeltaPositions = new List<Vector2>();

		private int FilterPositions = 10;

		public bool On
		{
			get
			{
				return FingerID != -1;
			}
		}

		public JoystickBaseOld(float left, float bottom, float width, float height)
		{
			Left = (float)Screen.width * left;
			Bottom = (float)Screen.height * bottom;
			Right = Left + (float)Screen.width * width;
			Top = Bottom + (float)Screen.height * height;
		}

		public void SetCenter(Vector2 center)
		{
			Center = center;
			LastTouchPosition = center;
			FirstDelta = true;
			DeltaPositions.Clear();
		}

		public bool IsInside(Touch touch)
		{
			if (touch.position.x > Left && touch.position.x < Right && touch.position.y > Bottom && touch.position.y < Top)
			{
				return true;
			}
			return false;
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

	public class ViewControl : JoystickBaseOld
	{
		public Quaternion Rotation;

		public float Yaw;

		public float Pitch;

		public bool Updated;

		public float minimumYaw = -360f;

		public float maximumYaw = 360f;

		public float minimumPitch = 60f;

		public float maximumPitch = 60f;

		public ViewControl(float left, float bottom, float width, float height)
			: base(left, bottom, width, height)
		{
		}

		public void OnTouchEnd(Touch touch)
		{
			FingerID = -1;
		}
	}

	private static float useRayTestDistance = 10f;

	private static float usePlayerDistance = 2f;

	private static Rect leftArea = new Rect(0f, 0f, 0.5f, 0.82f);

	private static Rect rightArea = new Rect(0.5f, 0f, 0.5f, 0.82f);

	private Transform _Temp;

	public float lastTouchControlTime;

	public ViewControl ViewJoystick;

	private JoystickBase MoveJoystick;

	private PlayerControlStates _States;

	public PlayerControlsTouch(PlayerControlStates inStates)
	{
		_States = inStates;
	}

	private void CreateDefaultJoystics()
	{
		MoveJoystick = new JoystickFloating(leftArea);
		ViewJoystick = new ViewControl(rightArea.x, rightArea.y, rightArea.width, rightArea.height);
		OnControlSchemeChange();
	}

	public void OnControlSchemeChange()
	{
		switch (GuiOptions.m_ControlScheme)
		{
		case GuiOptions.E_ControlScheme.Scheme1:
			MoveJoystick = new JoystickFloating((!GuiOptions.leftHandAiming) ? leftArea : rightArea);
			break;
		case GuiOptions.E_ControlScheme.Scheme2:
			MoveJoystick = new JoystickFixed(GuiOptions.MoveStick.Positon.x, (float)Screen.height - GuiOptions.MoveStick.Positon.y);
			break;
		}
		Rect rect = ((!GuiOptions.leftHandAiming) ? rightArea : leftArea);
		ViewJoystick = new ViewControl(rect.x, rect.y, rect.width, rect.height);
	}

	public void Start()
	{
		GameObject gameObject = new GameObject();
		_Temp = gameObject.transform;
		CreateDefaultJoystics();
	}

	public void Update()
	{
		MoveJoystick.Updated = false;
		ViewJoystick.Updated = false;
		if (Input.touchCount == 0 || Game.Instance.KypadSlided || Game.Instance.NVidiaShiledCached)
		{
			return;
		}
		Game.Instance.LastTouchControlTime = Time.timeSinceLevelLoad;
		for (int i = 0; i < Input.touchCount; i++)
		{
			Touch touch = Input.GetTouch(i);
			if (touch.phase == TouchPhase.Began)
			{
				TouchBegin(touch);
			}
			else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
			{
				TouchUpdate(touch);
			}
			else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
			{
				TouchEnd(touch);
			}
		}
		if (MoveJoystick.Updated)
		{
			_States.Move.Direction.x = MoveJoystick.Dir.x;
			_States.Move.Direction.z = MoveJoystick.Dir.y;
			_States.Move.Direction.y = 0f;
			_States.Move.Direction.Normalize();
			_Temp.transform.eulerAngles = new Vector3(0f, Player.Instance.Owner.Transform.rotation.eulerAngles.y, 0f);
			_States.Move.Direction = _Temp.transform.TransformDirection(_States.Move.Direction);
			_States.Move.Force = MoveJoystick.Force;
		}
		if (ViewJoystick.Updated)
		{
			_States.View.SetNewRotation(ViewJoystick.Yaw, ViewJoystick.Pitch);
		}
	}

	public static InteractionObject TouchedInteractionIcon(Vector2 position)
	{
		if (GuiHUD.Instance.IsHidden)
		{
			return null;
		}
		Camera main = Camera.main;
		if (main == null)
		{
			return null;
		}
		Ray ray = main.ScreenPointToRay(position);
		RaycastHit[] array = Physics.RaycastAll(ray, useRayTestDistance);
		if (array.Length > 1)
		{
			Array.Sort(array, CollisionUtils.CompareHits);
		}
		RaycastHit[] array2 = array;
		foreach (RaycastHit raycastHit in array2)
		{
			GameObject gameObject = raycastHit.collider.gameObject;
			if (!(gameObject == Player.Instance.Owner.GameObject))
			{
				if (gameObject.layer != InteractionObject.UseLayer)
				{
					break;
				}
				InteractionObject interactionObject = Mission.Instance.CurrentGameZone.InteractionObjectWithIcon(gameObject);
				if (interactionObject != null)
				{
					float sqrMagnitude = (interactionObject.Position - Player.Instance.Owner.Position).sqrMagnitude;
					if (sqrMagnitude <= usePlayerDistance * usePlayerDistance)
					{
						return interactionObject;
					}
					break;
				}
				break;
			}
		}
		return null;
	}

	private float GetSensitivity()
	{
		return GuiOptions.sensitivity;
	}

	public void TouchBegin(Touch touch)
	{
		if (FingerIdInUse(touch, false))
		{
			if (MoveJoystick.FingerID == touch.fingerId)
			{
				MoveJoystick.OnTouchEnd(touch);
			}
			else if (ViewJoystick.FingerID == touch.fingerId)
			{
				ViewJoystick.OnTouchEnd(touch);
			}
			if ((bool)GuiHUD.Instance && GuiHUD.Instance.FingerIdInUse(touch.fingerId))
			{
				GuiHUD.Instance.ReleaseFinger(touch);
			}
		}
		InteractionObject interactionObject = TouchedInteractionIcon(touch.position);
		if ((bool)interactionObject)
		{
			_States.UseObjectDelegate(interactionObject);
		}
		else if (_States.Move.Enabled && MoveJoystick.FingerID == -1 && MoveJoystick.IsInside(touch))
		{
			MoveJoystick.OnTouchBegin(touch);
		}
		else if (_States.View.Enabled && !ViewJoystick.On && ViewJoystick.IsInside(touch))
		{
			ViewJoystick.FingerID = touch.fingerId;
			ViewJoystick.SetCenter(touch.position);
			ViewJoystick.Rotation = Player.Instance.Owner.BlackBoard.Desires.Rotation;
			_States.View.ZeroInput();
		}
	}

	public void TouchUpdate(Touch touch)
	{
		if (_States.Move.Enabled)
		{
			if (MoveJoystick.FingerID == touch.fingerId)
			{
				Rect rect = ((!GuiOptions.leftHandAiming) ? leftArea : rightArea);
				if (new Rect((float)Screen.width * rect.x, (float)Screen.height * rect.y, (float)Screen.width * rect.width, (float)Screen.height * rect.height).Contains(touch.position))
				{
					MoveJoystick.OnTouchUpdate(touch);
				}
				else
				{
					MoveJoystick.OnTouchEnd(touch);
				}
				return;
			}
			if (MoveJoystick.FingerID == -1 && MoveJoystick.IsInside(touch) && !FingerIdInUse(touch, false))
			{
				MoveJoystick.OnTouchBegin(touch);
			}
		}
		ViewJoystick.Yaw = 0f;
		ViewJoystick.Pitch = 0f;
		if (_States.View.Enabled && ViewJoystick.FingerID == touch.fingerId)
		{
			if (ViewJoystick.IsInside(touch))
			{
				ViewJoystickUpdate(touch);
			}
			else
			{
				ViewJoystick.OnTouchEnd(touch);
			}
		}
	}

	private void ViewJoystickUpdate(Touch touch)
	{
		Vector2 vector;
		if (ViewJoystick.FirstDelta)
		{
			ViewJoystick.AddDelta(touch.position - ViewJoystick.LastTouchPosition);
			vector = ViewJoystick.GetDelta();
		}
		else
		{
			vector = touch.position - ViewJoystick.LastTouchPosition;
		}
		ViewJoystick.LastTouchPosition = touch.position;
		if (vector != Vector2.zero)
		{
			ViewJoystick.FirstDelta = false;
			float num = 45f / ((float)Screen.width * 0.25f);
			float num2 = 30f / ((float)Screen.height * 0.25f);
			float num3 = vector.x * num;
			float num4 = vector.y * num2;
			float sensitivity = GetSensitivity();
			num3 *= sensitivity;
			num4 *= sensitivity;
			ViewJoystick.Yaw = num3;
			ViewJoystick.Pitch = 0f - num4;
			ViewJoystick.Updated = true;
		}
	}

	public void TouchEnd(Touch touch)
	{
		if (MoveJoystick.FingerID == touch.fingerId)
		{
			MoveJoystick.OnTouchEnd(touch);
		}
		else if (ViewJoystick.FingerID == touch.fingerId)
		{
			ViewJoystick.OnTouchEnd(touch);
		}
	}

	private void JoystickDown(Vector2 pos)
	{
		if ((bool)GuiHUD.Instance)
		{
			GuiHUD.Instance.JoystickDown(pos);
		}
	}

	private void JoystickUpdate(Vector2 pos)
	{
		if ((bool)GuiHUD.Instance)
		{
			GuiHUD.Instance.JoystickUpdate(pos);
		}
	}

	private void JoystickUp()
	{
		if ((bool)GuiHUD.Instance)
		{
			GuiHUD.Instance.JoystickUp();
		}
	}

	private bool FingerIdInUse(Touch touch, bool joysticksOnly)
	{
		if (MoveJoystick.FingerID == touch.fingerId)
		{
			return true;
		}
		if (ViewJoystick.FingerID == touch.fingerId)
		{
			return true;
		}
		if ((bool)GuiHUD.Instance && GuiHUD.Instance.FingerIdInUse(touch.fingerId) && !joysticksOnly)
		{
			return true;
		}
		return false;
	}

	public void Reset()
	{
		MoveJoystick.FingerID = -1;
		MoveJoystick.Dir = Vector3.zero;
		MoveJoystick.Force = 0f;
	}
}
