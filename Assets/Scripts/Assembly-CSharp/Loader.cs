using System.Collections;
using UnityEngine;

public class Loader : MonoBehaviour
{
	private enum E_State
	{
		ConfirmDialog = 0,
		DataAvailable = 1,
		Downloading = 2,
		InvalidDataError = 3,
		DataDownloadError = 4,
		DeviceError = 5,
		PlatformError = 6,
		StorageError = 7,
		NetworkError = 8,
		ExitDialog = 9
	}

	private enum E_DataFileStatus
	{
		NoStorage = 0,
		Missing = 1,
		NotBind = 2,
		Bind = 3
	}

	public GUIStyle m_LabelStyle;

	public GUIStyle m_BackGroundStyle;

	private E_State m_State;

	private E_State oldState;

	private Rect m_ActiveScreen;

	private string m_FirstLevel = "MainMenuBackground";

	private bool m_isRunningOnAndroid;

	private bool m_DownloadInProggres;

	private float NextDataCheckTime;

	private static string m_LineTextOffsetBegin = "\n\n";

	private static string m_LineTextOffsetEnd = "\n";

	private static string m_IntroText = "Application is going to download data for ShadowGun .\nYou may want to wait with download until you are on wi-fi network.\nExpect aproximatly 300 MB of data.";

	private static string m_DataDownloadErrorText = "Failed to download ShadowGun data. \nCheck your internet connectivity and try again latter";

	private static string m_InvalidDataErrorText = "Data file is broken or in invalid version. \nPlease try reinstall application";

	private static string m_UnsupportedDeviceText = "Unable to start ShadowGun. \nYOUR DEVICE IS NOT SUPPORTED.";

	private static string m_NoAndroidDeviceText = "OBB files downloader works only on Android devices! \nApplication is not able to download data!";

	private static string m_NoStorageDeviceText = "External storage is not available! \nApplication is not able to download data!";

	private static string m_NoNetworkText = "Network is not reachable! \nApplication is not able to download data!";

	private static string m_ExitText = "Do you really want to exit application?";

	private E_State State
	{
		get
		{
			return m_State;
		}
		set
		{
			m_State = value;
			Debug.Log("Loader.SetState " + value);
		}
	}

	private static string introText
	{
		get
		{
			return m_LineTextOffsetBegin + m_IntroText + m_LineTextOffsetEnd;
		}
	}

	private static string dataDownloadErrorText
	{
		get
		{
			return m_LineTextOffsetBegin + m_DataDownloadErrorText + m_LineTextOffsetEnd;
		}
	}

	private static string dataErrorText
	{
		get
		{
			return m_LineTextOffsetBegin + m_InvalidDataErrorText + m_LineTextOffsetEnd;
		}
	}

	private static string unsupportedDeviceText
	{
		get
		{
			return m_LineTextOffsetBegin + m_UnsupportedDeviceText + m_LineTextOffsetEnd;
		}
	}

	private static string noAndroidDeviceText
	{
		get
		{
			return m_LineTextOffsetBegin + m_NoAndroidDeviceText + m_LineTextOffsetEnd;
		}
	}

	private static string noStorageDeviceText
	{
		get
		{
			return m_LineTextOffsetBegin + m_NoStorageDeviceText + m_LineTextOffsetEnd;
		}
	}

	private static string noNetworkText
	{
		get
		{
			return m_LineTextOffsetBegin + m_NoNetworkText + m_LineTextOffsetEnd;
		}
	}

	private static string exitText
	{
		get
		{
			return m_LineTextOffsetBegin + m_ExitText + m_LineTextOffsetEnd;
		}
	}

	private void Start()
	{
		Object.DontDestroyOnLoad(this);
		PrintSystemInfo();
		m_ActiveScreen = new Rect(0f, 0f, Screen.width, Screen.height);
		GameObject gameObject = GameObject.Find("BackGroundImage");
		if (gameObject != null && gameObject.GetComponent<GUITexture>() != null && gameObject.GetComponent<GUITexture>().texture != null)
		{
			Vector2 vector = new Vector2(gameObject.GetComponent<GUITexture>().texture.width, gameObject.GetComponent<GUITexture>().texture.height);
			Vector2 vector2 = new Vector2(Screen.width, Screen.height);
			Vector2 vector3 = new Vector2(vector.x / vector2.x, vector.y / vector2.y);
			if (vector3.x > vector3.y)
			{
				float num = vector2.x / vector.x * vector.y;
				float num2 = vector2.y - num;
				gameObject.GetComponent<GUITexture>().pixelInset = new Rect(0f, num2 * 0.5f, 0f, 0f - num2);
			}
			else
			{
				float num3 = vector2.y / vector.y * vector.x;
				float num4 = vector2.x - num3;
				gameObject.GetComponent<GUITexture>().pixelInset = new Rect(num4 * 0.5f, 0f, 0f - num4, 0f);
			}
			m_ActiveScreen = new Rect(gameObject.GetComponent<GUITexture>().pixelInset.x, gameObject.GetComponent<GUITexture>().pixelInset.y, (float)Screen.width + gameObject.GetComponent<GUITexture>().pixelInset.width, (float)Screen.height + gameObject.GetComponent<GUITexture>().pixelInset.height);
		}
		m_isRunningOnAndroid = GooglePlayDownloader.RunningOnAndroid();
		if (!m_isRunningOnAndroid)
		{
			State = E_State.PlatformError;
			return;
		}
		string expansionFilePath = GooglePlayDownloader.GetExpansionFilePath();
		if (expansionFilePath == null)
		{
			State = E_State.StorageError;
			return;
		}
		string mainOBBPath = GooglePlayDownloader.GetMainOBBPath(expansionFilePath);
		bool flag = mainOBBPath != null;
		bool flag2 = false;
		if (mainOBBPath != null)
		{
			string value = mainOBBPath.Substring(expansionFilePath.Length);
			flag2 = Application.dataPath != null && Application.dataPath.EndsWith(value);
		}
		if (!flag)
		{
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				State = E_State.NetworkError;
			}
			else
			{
				State = E_State.ConfirmDialog;
			}
			return;
		}
		if (!flag2)
		{
			State = E_State.InvalidDataError;
			return;
		}
		if (gameObject != null)
		{
			gameObject.active = false;
		}
		State = E_State.DataAvailable;
		DataDownloaded();
	}

	private void Update()
	{
		if (State == E_State.DataAvailable)
		{
			return;
		}
		if (m_DownloadInProggres && NextDataCheckTime < Time.realtimeSinceStartup)
		{
			switch (GetDataFileStatus())
			{
			case E_DataFileStatus.NoStorage:
				m_DownloadInProggres = false;
				State = E_State.StorageError;
				break;
			case E_DataFileStatus.Missing:
				State = E_State.DataDownloadError;
				break;
			case E_DataFileStatus.NotBind:
				m_DownloadInProggres = false;
				State = E_State.InvalidDataError;
				break;
			case E_DataFileStatus.Bind:
				m_DownloadInProggres = false;
				State = E_State.DataAvailable;
				DataDownloaded();
				break;
			}
			NextDataCheckTime = Time.realtimeSinceStartup + 0.5f;
		}
		if (Input.GetKeyDown("escape"))
		{
			if (State == E_State.ExitDialog)
			{
				State = oldState;
				return;
			}
			oldState = State;
			State = E_State.ExitDialog;
		}
	}

	private void OnGUI()
	{
		if (State != E_State.DataAvailable)
		{
			int num = (int)(m_ActiveScreen.width * 0.48f);
			int num2 = (int)(m_ActiveScreen.height * 0.35f);
			Rect position = new Rect(m_ActiveScreen.xMin + (m_ActiveScreen.width - (float)num) * 0.5f, m_ActiveScreen.yMin + (m_ActiveScreen.height - (float)num2 - m_ActiveScreen.height * 0.25f), num, num2);
			GUI.BeginGroup(position);
			GUI.Box(new Rect(0f, 0f, num, num2), string.Empty, m_BackGroundStyle);
			switch (State)
			{
			case E_State.ConfirmDialog:
				Gui_ConfirmDownloadDialog(num);
				break;
			case E_State.InvalidDataError:
				Gui_ShowErrorDialog(num, dataErrorText);
				break;
			case E_State.DataDownloadError:
				Gui_ShowErrorDialog(num, dataDownloadErrorText);
				break;
			case E_State.DeviceError:
				Gui_ShowErrorDialog(num, unsupportedDeviceText);
				break;
			case E_State.PlatformError:
				Gui_ShowErrorDialog(num, noAndroidDeviceText);
				break;
			case E_State.StorageError:
				Gui_ShowErrorDialog(num, noStorageDeviceText);
				break;
			case E_State.NetworkError:
				Gui_ShowErrorDialog(num, noNetworkText);
				break;
			case E_State.ExitDialog:
				Gui_ExitDialog(num);
				break;
			}
			GUI.EndGroup();
		}
	}

	private void Gui_ConfirmDownloadDialog(int inWidth)
	{
		GUILayout.Label(introText, m_LabelStyle, GUILayout.Width(inWidth));
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Begin download", GUILayout.MaxWidth((float)inWidth * 0.5f)))
		{
			DownloadData();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}

	private void Gui_ShowErrorDialog(int inWidth, string inErrorlabel)
	{
		GUILayout.BeginVertical();
		GUILayout.Label(inErrorlabel, m_LabelStyle, GUILayout.Width(inWidth));
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Exit", GUILayout.MaxWidth((float)inWidth * 0.25f)))
		{
			Application.Quit();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
	}

	private void Gui_ExitDialog(int inWidth)
	{
		GUILayout.BeginVertical();
		GUILayout.Label(exitText, m_LabelStyle, GUILayout.Width(inWidth));
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Yes", GUILayout.MaxWidth((float)inWidth * 0.25f)))
		{
			Application.Quit();
		}
		if (GUILayout.Button("No", GUILayout.MaxWidth((float)inWidth * 0.25f)))
		{
			State = oldState;
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
	}

	private E_DataFileStatus GetDataFileStatus()
	{
		string expansionFilePath = GooglePlayDownloader.GetExpansionFilePath();
		if (expansionFilePath == null)
		{
			return E_DataFileStatus.NoStorage;
		}
		string mainOBBPath = GooglePlayDownloader.GetMainOBBPath(expansionFilePath);
		if (mainOBBPath == null)
		{
			return E_DataFileStatus.Missing;
		}
		string value = mainOBBPath.Substring(expansionFilePath.Length);
		if (Application.dataPath == null || !Application.dataPath.EndsWith(value))
		{
			return E_DataFileStatus.NotBind;
		}
		return E_DataFileStatus.Bind;
	}

	private void DownloadData()
	{
		GooglePlayDownloader.FetchOBB();
		State = E_State.Downloading;
		NextDataCheckTime = Time.realtimeSinceStartup + 1f;
		m_DownloadInProggres = true;
	}

	private void DataDownloaded(bool inWithEmpty = true)
	{
		StartCoroutine(LoadScene(m_FirstLevel, inWithEmpty));
		base.enabled = false;
	}

	private IEnumerator LoadScene(string scene, bool inWithEmpty)
	{
		if (inWithEmpty)
		{
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			Application.LoadLevel("empty");
		}
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		Application.LoadLevel(scene);
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
	}

	private void PrintSystemInfo()
	{
		string text = "SystemInfo :";
		text = text + "\n\t - SystemInfo.operatingSystem         - " + SystemInfo.operatingSystem;
		text = text + "\n\t - SystemInfo.processorType           - " + SystemInfo.processorType;
		text = text + "\n\t - SystemInfo.processorCount          - " + SystemInfo.processorCount;
		text = text + "\n\t - SystemInfo.systemMemorySize        - " + SystemInfo.systemMemorySize;
		text = text + "\n\t - SystemInfo.graphicsMemorySize      - " + SystemInfo.graphicsMemorySize;
		text = text + "\n\t - SystemInfo.graphicsDeviceName      - " + SystemInfo.graphicsDeviceName;
		text = text + "\n\t - SystemInfo.graphicsDeviceVendor    - " + SystemInfo.graphicsDeviceVendor;
		text = text + "\n\t - SystemInfo.graphicsDeviceID        - " + SystemInfo.graphicsDeviceID;
		text = text + "\n\t - SystemInfo.graphicsDeviceVendorID  - " + SystemInfo.graphicsDeviceVendorID;
		text = text + "\n\t - SystemInfo.graphicsDeviceVersion   - " + SystemInfo.graphicsDeviceVersion;
		text = text + "\n\t - SystemInfo.graphicsShaderLevel     - " + SystemInfo.graphicsShaderLevel;
		text = text + "\n\t - SystemInfo.graphicsPixelFillrate   - " + SystemInfo.graphicsPixelFillrate;
		text = text + "\n\t - SystemInfo.supportsShadows         - " + SystemInfo.supportsShadows;
		text = text + "\n\t - SystemInfo.supportsRenderTextures  - " + SystemInfo.supportsRenderTextures;
		text = text + "\n\t - SystemInfo.supportsImageEffects    - " + SystemInfo.supportsImageEffects;
		text = text + "\n\t - SystemInfo.deviceUniqueIdentifier  - " + SystemInfo.deviceUniqueIdentifier;
		text = text + "\n\t - SystemInfo.deviceName              - " + SystemInfo.deviceName;
		text = text + "\n\t - SystemInfo.deviceModel             - " + SystemInfo.deviceModel;
		Debug.Log(text);
	}
}
