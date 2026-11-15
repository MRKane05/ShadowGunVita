using UnityEngine;

[AddComponentMenu("GUI/Widgets/Slider")]
public class GUIBase_Slider : GUIBase_Callback
{
	public delegate void ChangeValueDelegate(float v);

	public float m_MinValue;

	public float m_MaxValue = 1f;

	public float m_InitValue = 0.5f;

	public GUIBase_Sprite m_BarSprite;

	public float m_TouchableAreaWidthScale = 1f;

	public float m_TouchableAreaHeightScale = 1f;

	private GUIBase_Widget m_Widget;

	private ChangeValueDelegate m_ChangeValueDelegate;

	private float m_CurrentValue;

	private bool m_WasTouched;

	public void Start()
	{
		m_Widget = GetComponent<GUIBase_Widget>();
		int clbkTypes = 9;
		m_Widget.RegisterCallback(this, clbkTypes);
		m_Widget.RegisterUpdateDelegate(UpdateSlider);
	}

	public void RegisterChangeValueDelegate(ChangeValueDelegate d)
	{
		m_ChangeValueDelegate = d;
	}

	public override bool Callback(E_CallbackType type)
	{
		switch (type)
		{
		case E_CallbackType.E_CT_INIT:
			CustomInit();
			break;
		case E_CallbackType.E_CT_ON_TOUCH_BEGIN:
			m_WasTouched = true;
			UpdateSlider();
			break;
		}
		return true;
	}

	public override void GetTouchAreaScale(out float scaleWidth, out float scaleHeight)
	{
		scaleWidth = m_TouchableAreaWidthScale;
		scaleHeight = m_TouchableAreaHeightScale;
	}

	public override void ChildButtonPressed(float v)
	{
		float num = v * 0.01f * (m_MaxValue - m_MinValue);
		float value = m_CurrentValue + num;
		SetValue(value);
		if (m_ChangeValueDelegate != null)
		{
			m_ChangeValueDelegate(m_CurrentValue);
		}
	}

	public override void ChildButtonReleased()
	{
	}

	private void CustomInit()
	{
		Vector3 position = base.gameObject.transform.position;
		Vector3 lossyScale = base.gameObject.transform.lossyScale;
		Vector3 eulerAngles = base.gameObject.transform.eulerAngles;
		float width = m_Widget.GetWidth();
		float height = m_Widget.GetHeight();
		Texture texture = m_Widget.GetTexture();
		int num = 1;
		int num2 = 1;
		if ((bool)texture)
		{
			num = texture.width;
			num2 = texture.height;
		}
		int num3 = 0;
		int num4 = 0;
		int num5 = 1;
		int num6 = 1;
		if ((bool)m_BarSprite)
		{
			GUIBase_Widget widget = m_BarSprite.Widget;
			if ((bool)widget)
			{
				num3 = (int)((float)num * widget.m_InTexPos.x);
				num4 = (int)((float)num2 * widget.m_InTexPos.y);
				num5 = (int)((float)num * widget.m_InTexSize.x);
				num6 = (int)((float)num2 * widget.m_InTexSize.y);
				m_Widget.AddSprite(new Vector2(position.x, position.y), width, height, lossyScale.x, lossyScale.y, eulerAngles.z, num3, num4 + num6, num5, num6);
			}
		}
		SetValue(m_InitValue);
		m_Widget.ShowSprite(1, false);
	}

	public void SetValue(float v)
	{
		m_Widget.ShowSprite(1, true);
		m_CurrentValue = Mathf.Clamp(v, m_MinValue, m_MaxValue);
		Vector3 position = base.gameObject.transform.position;
		Vector3 lossyScale = base.gameObject.transform.lossyScale;
		float num = (m_CurrentValue - m_MinValue) / (m_MaxValue - m_MinValue);
		float x = position.x;
		float y = position.y;
		float num2 = m_Widget.GetWidth() * num;
		float height = m_Widget.GetHeight();
		x -= m_Widget.GetWidth() * 0.5f * lossyScale.x;
		x += num2 * 0.5f * lossyScale.x;
		m_Widget.UpdateSpritePosAndSize(1, x, y, num2, height);
	}

	private void UpdateSlider()
	{
		if (!m_WasTouched)
		{
			return;
		}
		Vector2 vector = default(Vector2);
		bool flag = false;
		if (Input.touchCount != 0)
		{
			Touch touch = Input.touches[0];
			vector.x = touch.position.x;
			flag = true;
		}
		else if (Input.GetMouseButton(0))
		{
			vector.x = Input.mousePosition.x;
			flag = true;
		}
		if (flag)
		{
			float num = base.gameObject.transform.position.x - m_Widget.GetWidth() * 0.5f * base.gameObject.transform.lossyScale.x;
			float num2 = num + m_Widget.GetWidth() * base.gameObject.transform.lossyScale.x;
			float t = (vector.x - num) / (num2 - num);
			float num3 = Mathf.Lerp(m_MinValue, m_MaxValue, t);
			SetValue(num3);
			if (m_ChangeValueDelegate != null)
			{
				m_ChangeValueDelegate(num3);
			}
		}
		else
		{
			m_WasTouched = false;
		}
	}
}
