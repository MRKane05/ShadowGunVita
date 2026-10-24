using UnityEngine;

public abstract class JoystickBase
{
	public int FingerID = -1;

	public Vector2 Center;

	public Vector2 Dir;

	public float Force;

	public bool Updated;

	public bool On
	{
		get
		{
			return FingerID != -1;
		}
	}

	public float Radius
	{
		get
		{
			return 0.07f * (float)Screen.width;
		}
	}

	public void SetCenter(Vector2 center)
	{
		Center = center;
	}

	public abstract void OnTouchBegin(Touch touch);

	public abstract void OnTouchUpdate(Touch touch);

	public abstract void OnTouchEnd(Touch touch);

	public abstract bool IsInside(Touch touch);
}
