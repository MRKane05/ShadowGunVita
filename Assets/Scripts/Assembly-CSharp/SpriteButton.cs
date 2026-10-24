using UnityEngine;

public class SpriteButton
{
	public int TouchCount;

	private Vector2 _screenPosition;

	private Vector2 _LeftDown;

	private Vector2 _RightUp;

	private Sprite _sprite;

	public Sprite sprite
	{
		get
		{
			return _sprite;
		}
		set
		{
			_sprite = value;
		}
	}

	public Vector2 screenPosition
	{
		get
		{
			return _screenPosition;
		}
		set
		{
			_screenPosition = value;
		}
	}

	public Vector2 RightUp
	{
		get
		{
			return _RightUp;
		}
		set
		{
			_RightUp = value;
		}
	}

	public Vector2 LeftDown
	{
		get
		{
			return _LeftDown;
		}
		set
		{
			_LeftDown = value;
		}
	}

	public bool Hidden
	{
		get
		{
			return sprite.hidden;
		}
		set
		{
			sprite.hidden = value;
		}
	}
}
