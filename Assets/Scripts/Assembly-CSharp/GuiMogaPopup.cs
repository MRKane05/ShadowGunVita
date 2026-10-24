using UnityEngine;

public class GuiMogaPopup : MonoBehaviour
{
	public static GuiMogaPopup Instance;

	private GUIBase_Pivot m_Pivot;

	private GUIBase_Layout m_Layout;

	private GUIBase_Label m_TextLabel;

	private bool m_initialised;

	private void Awake()
	{
		Instance = this;
	}

	private void OnDestroy()
	{
		Hide();
		CancelInvoke();
		Instance = null;
	}

	public void Init()
	{
		m_initialised = true;
		m_Pivot = MFGuiManager.Instance.GetPivot("MogaGui_Pivot");
		m_Layout = m_Pivot.GetLayout("Connection_Layout");
		m_TextLabel = GuiBaseUtils.PrepareLabel(m_Layout, "Text_Label");
	}

	public void Show(int msgId, float hideTime)
	{
		if (m_initialised && MFGuiManager.Instance != null)
		{
			string newText = TextDatabase.instance[msgId];
			MFGuiManager.Instance.ShowLayout(m_Layout, true);
			m_TextLabel.SetNewText(newText);
			Invoke("Hide", hideTime);
		}
	}

	public void Hide()
	{
		if (m_initialised && MFGuiManager.Instance != null)
		{
			MFGuiManager.Instance.ShowLayout(m_Layout, false);
		}
	}

	public void LateUpdate()
	{
		if (!m_initialised)
		{
			Init();
		}
	}
}
