using System.Text;
using UnityEngine;

// Single source of truth for where save data lives.
//
// Application.persistentDataPath is not usable on PS Vita, so on that platform the game
// creates and uses a folder under ux0:data/. Everywhere else the original
// persistentDataPath behaviour is kept unchanged.
//
// Every save file the game writes goes through Game.GetSaveFileName (written by
// GameZone.Save, read by Game.Save_Load, removed by Game.Save_Clear), so redirecting
// here moves the whole save system.
//
// NOTE: this does not cover PlayerPrefs, which the game also uses for options, the
// gamepad config and mission-unlock progress. Unity's own PSP2 player stores those in
// the title's save-data slot, so they are already on device storage and are left alone.
public static class SavePath
{
	// Vita user storage. The trailing slash matters: paths are concatenated, not combined.
	public const string VITA_STORAGE = "ux0:data/";

	// Fallback folder name if the product name sanitises down to nothing.
	private const string DEFAULT_FOLDER = "ShadowGun";

	private static string cachedRoot;

	private static string cachedFolder;

	// Root with a trailing slash, for direct concatenation.
	public static string Root
	{
		get
		{
			if (cachedRoot == null)
			{
				cachedRoot = ResolveRoot();
			}
			return cachedRoot;
		}
	}

	// Root without the trailing slash, for callers that supply their own leading slash.
	public static string Directory
	{
		get
		{
			return Root.TrimEnd('/');
		}
	}

	// Product name reduced to characters that are legal in a Vita path. A colon is the
	// Vita device separator and spaces are best avoided, so anything that is not a
	// letter, digit, underscore or dash is dropped.
	public static string FolderName
	{
		get
		{
			if (cachedFolder != null)
			{
				return cachedFolder;
			}
			StringBuilder sb = new StringBuilder();
			string source = Application.productName;
			if (source != null)
			{
				for (int i = 0; i < source.Length; i++)
				{
					char c = source[i];
					if (char.IsLetterOrDigit(c) || c == '_' || c == '-')
					{
						sb.Append(c);
					}
				}
			}
			cachedFolder = ((sb.Length <= 0) ? DEFAULT_FOLDER : sb.ToString());
			return cachedFolder;
		}
	}

	private static string ResolveRoot()
	{
		string candidate;
		if (Application.platform == RuntimePlatform.PSP2)
		{
			candidate = VITA_STORAGE + FolderName + "/";
		}
		else
		{
			candidate = Application.persistentDataPath + "/";
		}
		if (EnsureExists(candidate))
		{
			Debug.Log("[SavePath] Save root: " + candidate);
			return candidate;
		}
		// If ux0:data is not writable for any reason, fall back rather than losing saves
		// silently - a wrong-but-working path beats every write throwing.
		string fallback = Application.persistentDataPath + "/";
		Debug.LogWarning("[SavePath] Could not create '" + candidate + "', falling back to " + fallback);
		EnsureExists(fallback);
		return fallback;
	}

	private static bool EnsureExists(string path)
	{
		try
		{
			if (!System.IO.Directory.Exists(path))
			{
				System.IO.Directory.CreateDirectory(path);
			}
			return System.IO.Directory.Exists(path);
		}
		catch (System.Exception e)
		{
			Debug.LogWarning("[SavePath] " + path + " : " + e.Message);
			return false;
		}
	}
}
