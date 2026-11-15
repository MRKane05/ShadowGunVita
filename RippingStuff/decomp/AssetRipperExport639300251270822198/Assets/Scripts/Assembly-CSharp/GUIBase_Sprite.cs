using UnityEngine;

[AddComponentMenu("GUI/Widgets/Sprite")]
public class GUIBase_Sprite : GUIBase_Callback
{
	public GUIBase_Widget Widget { get; private set; }

	public void Start()
	{
		GUIBase_Widget gUIBase_Widget = (Widget = GetComponent<GUIBase_Widget>());
		gUIBase_Widget.RegisterCallback(this, 0);
	}
}
