using System.Collections.Generic;
using UnityEngine;

public class LocalyticsGUIManager : MonoBehaviour
{
	private void OnGUI()
	{
		float num = 5f;
		float left = 5f;
		float width = ((Screen.width < 960 && Screen.height < 960) ? 160 : 320);
		float num2 = ((Screen.width < 960 && Screen.height < 960) ? 40 : 80);
		float num3 = num2 + 10f;
		if (GUI.Button(new Rect(left, num, width, num2), "Start Session"))
		{
			LocalyticsBinding.startSession("YOUR_API_KEY");
		}
		if (GUI.Button(new Rect(left, num += num3, width, num2), "Tag Event"))
		{
			LocalyticsBinding.tagEvent("somethingHappened");
		}
		if (GUI.Button(new Rect(left, num += num3, width, num2), "Tag Event + Attributes"))
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("aParam", "with a value");
			dictionary.Add("otherImportant", "data");
			LocalyticsBinding.tagEventWithAttributes("eventWithData", dictionary);
		}
	}
}
