using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

// _localytics* native symbols only exist in the iOS build - keep this class out of PSP2/other builds to avoid unresolved-symbol link errors.
#if UNITY_IOS
public class LocalyticsBinding
{
	private static string dictionaryToString(Dictionary<string, string> dict)
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, string> item in dict)
		{
			list.Add(string.Format("{0}||{1}", item.Key, item.Value));
		}
		return string.Join("|||", list.ToArray());
	}

	[DllImport("__Internal")]
	private static extern void _localyticsStartSession(string apiKey);

	public static void startSession(string apiKey)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_localyticsStartSession(apiKey);
		}
	}

	[DllImport("__Internal")]
	private static extern void _localyticsTagEvent(string eventName);

	public static void tagEvent(string eventName)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_localyticsTagEvent(eventName);
		}
	}

	[DllImport("__Internal")]
	private static extern void _localyticsTagEventWithAttributes(string eventName, string parameters);

	public static void tagEventWithAttributes(string eventName, Dictionary<string, string> parameters)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_localyticsTagEventWithAttributes(eventName, dictionaryToString(parameters));
		}
	}

	[DllImport("__Internal")]
	private static extern void _localyticsTagScreen(string screenName);

	public static void tagScreen(string screenName)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_localyticsTagScreen(screenName);
		}
	}
}
#else
public class LocalyticsBinding
{
	public static void startSession(string apiKey) { }

	public static void tagEvent(string eventName) { }

	public static void tagEventWithAttributes(string eventName, Dictionary<string, string> parameters) { }

	public static void tagScreen(string screenName) { }
}
#endif
