using System.Collections;
using UnityEngine;

[AddComponentMenu("GUI/Hiearchy/Pivot")]
public class GUIBase_Pivot : MonoBehaviour
{
	public AnimationClip m_InAnimation;

	public AnimationClip m_LoopAnimation;

	public AnimationClip m_OutAnimation;

	public float m_FadeAlpha = 1f;

	private MFGuiManager m_GuiManager;

	private Animation m_Anim;

	private bool m_IsVisible;

	private GUIBase_Widget m_ParentWidget;

	private GUIBase_Pivot m_ParentPivot;

	private GUIBase_Layout m_ParentLayout;

	private bool m_LayoutsInitialized;

	private Hashtable m_Layouts;

	public void Start()
	{
		m_GuiManager = MFGuiManager.Instance;
		m_Anim = GetComponent<Animation>();
		m_IsVisible = false;
		if (!m_Anim)
		{
			Debug.LogError("Missing Animation component.");
		}
		PrepareParent();
	}

	public bool IsVisible()
	{
		return m_IsVisible;
	}

	public void Show(bool show)
	{
		if (show)
		{
			ShowLayouts(true);
			if ((bool)m_InAnimation)
			{
				m_Anim.clip = m_InAnimation;
				m_GuiManager.GetPlatform(this).PlayAnim(m_Anim, null, PivotAnimFinished, 0);
			}
		}
		else if ((bool)m_OutAnimation)
		{
			m_Anim.clip = m_OutAnimation;
			m_GuiManager.GetPlatform(this).PlayAnim(m_Anim, null, PivotAnimFinished, 1);
		}
		else
		{
			ShowLayouts(false);
		}
	}

	private void PivotAnimFinished(int idx)
	{
		switch ((GUIBase_Platform.E_SpecialAnimIdx)idx)
		{
		case GUIBase_Platform.E_SpecialAnimIdx.E_SAI_INANIM:
			if ((bool)m_LoopAnimation)
			{
				m_Anim.clip = m_LoopAnimation;
				m_GuiManager.GetPlatform(this).PlayAnim(m_Anim, null);
			}
			break;
		case GUIBase_Platform.E_SpecialAnimIdx.E_SAI_OUTANIM:
			ShowLayouts(false);
			break;
		}
	}

	private void ShowLayouts(bool show)
	{
		if (!m_LayoutsInitialized)
		{
			InitLayouts();
		}
		if (m_Layouts != null)
		{
			IDictionaryEnumerator enumerator = m_Layouts.GetEnumerator();
			while (enumerator.MoveNext())
			{
				GUIBase_Layout layout = (GUIBase_Layout)((DictionaryEntry)enumerator.Current).Value;
				m_GuiManager.ShowLayout(layout, show);
			}
		}
		m_IsVisible = show;
	}

	private void InitLayouts()
	{
		m_LayoutsInitialized = true;
		GUIBase_Layout[] componentsInChildren = GetComponentsInChildren<GUIBase_Layout>();
		if (componentsInChildren != null)
		{
			m_Layouts = new Hashtable();
			GUIBase_Layout[] array = componentsInChildren;
			foreach (GUIBase_Layout gUIBase_Layout in array)
			{
				m_Layouts[gUIBase_Layout.name] = gUIBase_Layout;
			}
		}
	}

	public GUIBase_Layout GetLayout(string layoutName)
	{
		if (!m_LayoutsInitialized)
		{
			InitLayouts();
		}
		if (m_Layouts != null && m_Layouts.ContainsKey(layoutName))
		{
			return (GUIBase_Layout)m_Layouts[layoutName];
		}
		return null;
	}

	private float GetParentFadeAlpha()
	{
		float result = 1f;
		if ((bool)m_ParentWidget)
		{
			result = m_ParentWidget.GetFadeAlpha(true);
		}
		else if ((bool)m_ParentPivot)
		{
			result = m_ParentPivot.GetFadeAlpha(true);
		}
		else if ((bool)m_ParentLayout)
		{
			result = m_ParentLayout.GetFadeAlpha(true);
		}
		return result;
	}

	public float GetFadeAlpha(bool recursive)
	{
		return m_FadeAlpha * GetParentFadeAlpha();
	}

	private void PrepareParent()
	{
		Transform parent = base.gameObject.transform.parent;
		GameObject gameObject = parent.gameObject;
		if (!gameObject)
		{
			return;
		}
		m_ParentLayout = gameObject.GetComponent<GUIBase_Layout>();
		if (!m_ParentLayout)
		{
			m_ParentPivot = gameObject.GetComponent<GUIBase_Pivot>();
			if (!m_ParentPivot)
			{
				m_ParentWidget = gameObject.GetComponent<GUIBase_Widget>();
			}
		}
	}
}
