using UnityEngine;

[AddComponentMenu("GUI/Widgets/ProgressBar")]
public class GUIBase_ProgressBar : GUIBase_Callback
{
	private const float m_MinValue = 0f;

	private const float m_MaxValue = 1f;

	public float m_InitValue = 1f;

	private float m_CurrentValue;

	private Animation m_Anim;

	public GUIBase_Sprite m_BarSprite;

	private float barWidth;

	private float barHeight;

	private GUIBase_Widget m_Widget;

	public float CurentValue
	{
		get
		{
			return m_CurrentValue;
		}
	}

	public void Start()
	{
		m_Widget = GetComponent<GUIBase_Widget>();
		m_Anim = GetComponent<Animation>();
		int clbkTypes = 1;
		m_Widget.RegisterCallback(this, clbkTypes);
	}

	public override bool Callback(E_CallbackType type)
	{
		if (type == E_CallbackType.E_CT_INIT)
		{
			CustomInit();
		}
		return true;
	}

	private void CustomInit()
	{
		Vector3 position = base.gameObject.transform.position;
		Vector3 lossyScale = base.gameObject.transform.lossyScale;
		Vector3 eulerAngles = base.gameObject.transform.eulerAngles;
		barWidth = m_Widget.GetWidth();
		barHeight = m_Widget.GetHeight();
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
				barWidth = widget.GetWidth();
				barHeight = widget.GetHeight();
				num3 = (int)((float)num * widget.m_InTexPos.x);
				num4 = (int)((float)num2 * widget.m_InTexPos.y);
				num5 = (int)((float)num * widget.m_InTexSize.x);
				num6 = (int)((float)num2 * widget.m_InTexSize.y);
				m_Widget.AddSprite(new Vector2(position.x, position.y), barWidth, barHeight, lossyScale.x, lossyScale.y, eulerAngles.z, num3, num4 + num6, num5, num6);
			}
		}
		SetValue(m_InitValue);
		m_Widget.ShowSprite(1, false);
	}

	public void SetValue(float v)
	{
		m_Widget.ShowSprite(1, true);
		m_CurrentValue = Mathf.Clamp(v, 0f, 1f);
		Vector3 position = base.gameObject.transform.position;
		Vector3 lossyScale = base.gameObject.transform.lossyScale;
		float num = m_CurrentValue / 1f;
		float x = position.x;
		float y = position.y;
		float num2 = barWidth * num;
		x -= barWidth * 0.5f * lossyScale.x;
		x += num2 * 0.5f * lossyScale.x;
		m_Widget.UpdateSpritePosAndSize(1, x, y, num2, barHeight);
	}

	public void SetBarColor(Color c)
	{
		m_Widget.m_Color = c;
	}

	public void PlayAnimClip(AnimationClip clip)
	{
		if (clip != null)
		{
			m_Anim.clip = clip;
			m_Widget.PlayAnim(m_Anim, m_Widget);
		}
	}

	public void StopAnimClip()
	{
		if (m_Anim.clip != null)
		{
			m_Widget.StopAnim(m_Anim);
		}
	}
}
