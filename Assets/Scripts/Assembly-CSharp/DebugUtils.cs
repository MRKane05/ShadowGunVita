using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class DebugUtils
{
	[Conditional("DEBUG")]
	public static void Assert(bool condition)
	{
		if (!condition)
		{
			StackTrace stackTrace = new StackTrace(true);
			StackFrame[] frames = stackTrace.GetFrames();
			foreach (StackFrame stackFrame in frames)
			{
				Debug.LogError(string.Concat("File :", stackFrame.GetFileName(), ", ", stackFrame.GetMethod(), " ", stackFrame.GetFileLineNumber()));
			}
			throw new Exception();
		}
	}

	public static string GetFullName(GameObject inObject)
	{
		if ((bool)inObject)
		{
			if ((bool)inObject.transform.parent)
			{
				return GetFullName(inObject.transform.parent.gameObject) + "/" + inObject.name;
			}
			return inObject.name;
		}
		return string.Empty;
	}
}
