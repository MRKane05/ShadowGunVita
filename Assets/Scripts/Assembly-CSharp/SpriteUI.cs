using System.Collections;
using UnityEngine;

public class SpriteUI : SpriteManager
{
	public enum ZeroLocationEnum
	{
		LowerLeft = -1,
		UpperLeft = 1
	}

	public int DrawDepth = 100;

	public LayerMask UILayer = 0;

	public ZeroLocationEnum ZeroLocation = ZeroLocationEnum.LowerLeft;

	public ScreenOrientation ScreenOrientation = ScreenOrientation.Portrait;

	private Camera _uiCamera;

	private GameObject _uiCameraHolder;

	private float _xOffset;

	private float _yOffset;

	protected override void Awake()
	{
		base.Awake();
		_uiCameraHolder = new GameObject("UI Camera");
		_uiCameraHolder.AddComponent<Camera>();
		_uiCamera = _uiCameraHolder.GetComponent<Camera>();
		_uiCamera.clearFlags = CameraClearFlags.Depth;
		_uiCamera.nearClipPlane = 0.3f;
		_uiCamera.farClipPlane = 100f;
		_uiCamera.depth = DrawDepth;
		_uiCamera.rect = new Rect(0f, 0f, 1f, 1f);
		_uiCamera.orthographic = true;
		_uiCamera.orthographicSize = (float)Screen.height * 0.5f;
		_uiCamera.cullingMask = UILayer;
		_uiCamera.transform.position = new Vector3(0f, 0f, -10f);
		UpdateUISize();
	}

	public void UpdateUISize()
	{
		_xOffset = (float)(-Screen.width) / 2f;
		_yOffset = (float)Screen.height / 2f;
	}

	public Sprite AddElement(Vector2 leftDown, float width, float height, float depth, int leftPixelX, int bottomPixelY, int pixelWidth, int pixelHeight)
	{
		return AddElement(leftDown, width, height, depth, PixelCoordToUVCoord(leftPixelX, bottomPixelY), PixelSpaceToUVSpace(pixelWidth, pixelHeight));
	}

	public Sprite AddElement(Vector2 leftDown, float width, float height, float depth, Vector2 lowerLeftUV, Vector2 uvSize)
	{
		return AddUIElement(leftDown, width, height, depth, lowerLeftUV, uvSize, null, false);
	}

	private Sprite AddUIElement(Vector2 leftDown, float width, float height, float depth, Vector2 lowerLeftUV, Vector2 uvSize, UIManager manager, bool isActive)
	{
		UpdateUISize();
		GameObject gameObject = new GameObject("UI Element");
		Transform transform = gameObject.transform;
		float x = leftDown.x + _xOffset + width / 2f;
		float y = (float)ZeroLocation * (0f - leftDown.y + _yOffset - height / 2f);
		transform.position = new Vector3(x, y, depth);
		for (int i = 0; i < 32; i++)
		{
			if ((UILayer.value & (1 << i)) == 1 << i)
			{
				gameObject.layer = i;
				break;
			}
		}
		return AddSprite(gameObject, width, height, lowerLeftUV, uvSize, false);
	}

	public void UpdateSpriteSize(Sprite sprite, Vector2 upperLeft, float width, float height)
	{
		float x = upperLeft.x + _xOffset + width / 2f;
		float y = (float)ZeroLocation * (0f - upperLeft.y + _yOffset - height / 2f);
		sprite.clientTransform.position = new Vector3(x, y, sprite.clientTransform.position.z);
		sprite.SetSizeXY(width, height);
		Transform(sprite);
	}

	public void UpdateSpriteSize(Sprite sprite, Vector2 leftDown, float width, float height, int leftPixelX, int bottomPixelY, int pixelWidth, int pixelHeight)
	{
		UpdateSpriteSize(sprite, leftDown, width, height, PixelCoordToUVCoord(leftPixelX, bottomPixelY), PixelSpaceToUVSpace(pixelWidth, pixelHeight));
	}

	private void UpdateSpriteSize(Sprite sprite, Vector2 leftDown, float width, float height, Vector2 lowerLeftUV, Vector2 uvSize)
	{
		float x = leftDown.x + _xOffset + width / 2f;
		float y = (float)ZeroLocation * (0f - leftDown.y + _yOffset - height / 2f);
		sprite.clientTransform.position = new Vector3(x, y, sprite.clientTransform.position.z);
		sprite.SetSizeXY(width, height);
		sprite.uvDimensions = uvSize;
		sprite.lowerLeftUV = lowerLeftUV;
	}

	public void SetSpritePosition(Sprite s, Vector2 leftDown)
	{
		s.clientTransform.position = new Vector3(leftDown.x + _xOffset + s.width / 2f, (float)ZeroLocation * (0f - leftDown.y + _yOffset - s.height / 2f), 0f);
		Transform(s);
	}

	public void SetSpriteCenterPosition(Sprite s, Vector2 center)
	{
		s.clientTransform.position = new Vector3(center.x + _xOffset, (float)ZeroLocation * (0f - center.y + _yOffset), 0f);
		Transform(s);
	}

	public IEnumerator MoveSprite(Sprite s, float time, float delay, Vector2 leftDown)
	{
		yield return new WaitForSeconds(delay);
		float currentTime = 0f;
		Vector3 start = s.clientTransform.position;
		Vector3 end = new Vector3(leftDown.x + _xOffset + s.width / 2f, (float)ZeroLocation * (0f - leftDown.y + _yOffset - s.height / 2f), 0f);
		if (time == 0f)
		{
			s.clientTransform.position = end;
			Transform(s);
			yield break;
		}
		while (currentTime < time)
		{
			currentTime += Time.deltaTime;
			if (currentTime > time)
			{
				currentTime = time;
			}
			float progress = currentTime / time;
			s.clientTransform.position = Mathfx.Hermite(start, end, progress);
			Transform(s);
			yield return new WaitForEndOfFrame();
		}
	}
}
