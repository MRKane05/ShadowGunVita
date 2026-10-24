using System.Collections;
using UnityEngine;

[AddComponentMenu("GUI/Game/BossHealth")]
public class BossHealth : MonoBehaviour
{
	public enum E_BossName
	{
		Robolobster = 0,
		Driller = 1,
		DrSimon = 2,
		Dropship = 3
	}

	private const float minDelayTime = 0.5f;

	private static string[] s_BossLabelNames = new string[4] { "Label_Robolobster", "Label_Driller", "Label_DrSimon", "Label_Dropship" };

	public float ShowDelay = 2.5f;

	public E_BossName BossName;

	public AnimationClip hitAnimClip;

	private IBossHealth boss;

	private bool m_Initialised;

	private bool m_Visible;

	private GUIBase_Pivot m_Pivot;

	private GUIBase_Layout m_Layout;

	private GUIBase_ProgressBar m_ProgressBar;

	private float currentHP = 1f;

	private float animStartHP = 1f;

	private float animTargetHP;

	private float animSpeed = 0.6f;

	private float curAnimTime;

	private float animTimeLength;

	private float showTime;

	private bool m_Suspended;

	private static ArrayList Instancies = new ArrayList();

	private static void RegisterInstance(BossHealth s)
	{
		Instancies.Add(s);
	}

	private static void UnregisterInstance(BossHealth s)
	{
		Instancies.Remove(s);
	}

	public static void SuspendAllRunning(bool suspend)
	{
		foreach (BossHealth instancy in Instancies)
		{
			instancy.Suspend(suspend);
		}
	}

	public void Awake()
	{
		boss = GameObjectUtils.GetComponentWithInterface<IBossHealth>(base.gameObject);
		if (boss == null)
		{
			Debug.LogError("BossHealth :: There have to be the component which implements IBossHealth interface");
			Debug.LogError("BossHealth :: Health-bar will not be updated ... ");
		}
		RegisterInstance(this);
	}

	private void OnDestroy()
	{
		HideHealthBar();
		CancelInvoke();
		UnregisterInstance(this);
	}

	public void OnDisable()
	{
		HideHealthBar();
		CancelInvoke();
	}

	public void OnEnable()
	{
		if (boss != null)
		{
			InvokeRepeating("HealhBarUpdate", (!(ShowDelay > 0.5f)) ? 0.5f : ShowDelay, 0.1f);
			currentHP = 1f;
			animTargetHP = currentHP;
			animStartHP = currentHP;
			curAnimTime = 0f;
			m_Suspended = false;
			if ((bool)m_ProgressBar)
			{
				m_ProgressBar.SetValue(currentHP);
			}
		}
	}

	private void HealhBarUpdate()
	{
		if (!m_Initialised)
		{
			InitHealthBar();
		}
		if (!m_Visible)
		{
			ShowHealthBar();
		}
		if (currentHP <= 0f)
		{
			HideHealthBar();
			CancelInvoke();
			return;
		}
		float healthPercent = boss.HealthPercent;
		if (animTargetHP > healthPercent)
		{
			animTargetHP = healthPercent;
			float num = 0f;
			if (curAnimTime > 0f)
			{
				num = animTimeLength - curAnimTime;
			}
			animTimeLength = (animStartHP - animTargetHP) / animSpeed;
			curAnimTime = animTimeLength - num;
			while (currentHP < Mathfx.Hermite(animStartHP, animTargetHP, Mathf.Clamp(1f - curAnimTime / animTimeLength, 0f, 1f)))
			{
				curAnimTime -= 0.05f;
			}
			if ((bool)hitAnimClip)
			{
				m_ProgressBar.PlayAnimClip(hitAnimClip);
			}
		}
	}

	private void Update()
	{
		if (!m_Suspended)
		{
			UpdateBarAnim();
		}
	}

	private void UpdateBarAnim()
	{
		if (currentHP != animTargetHP && curAnimTime > 0f)
		{
			curAnimTime -= Time.deltaTime * animSpeed;
			float value = Mathf.Clamp(1f - curAnimTime / animTimeLength, 0f, 1f);
			currentHP = Mathfx.Hermite(animStartHP, animTargetHP, value);
			UpdateBar();
			if (curAnimTime <= 0f)
			{
				curAnimTime = 0f;
				animStartHP = currentHP;
			}
		}
	}

	internal void InitHealthBar()
	{
		m_Pivot = MFGuiManager.Instance.GetPivot("HealthBar");
		m_Layout = GuiBaseUtils.GetLayout("HealthBar_Layout", m_Pivot);
		m_ProgressBar = GuiBaseUtils.PrepareProgressBar(m_Layout, "GUIBase_ProgressBar");
		m_Initialised = true;
	}

	internal void ShowHealthBar()
	{
		m_Visible = true;
		MFGuiManager.Instance.ShowLayout(m_Layout, true);
		ShowName();
	}

	internal void HideHealthBar()
	{
		if (m_Visible)
		{
			m_Visible = false;
			if (MFGuiManager.Instance != null && m_Layout != null)
			{
				MFGuiManager.Instance.ShowLayout(m_Layout, false);
			}
		}
	}

	internal void ShowName()
	{
		string wName = s_BossLabelNames[(int)BossName];
		GUIBase_Widget widget = m_Layout.GetWidget(wName);
		MFGuiManager.Instance.ShowWidget(widget, true, true);
	}

	internal void UpdateBar()
	{
		m_ProgressBar.SetValue(currentHP);
	}

	private void Suspend(bool suspend)
	{
		if (m_Initialised && m_Visible)
		{
			m_Layout.ShowImmediate(!suspend, false);
			m_Suspended = suspend;
		}
	}
}
