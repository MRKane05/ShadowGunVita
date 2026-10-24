using UnityEngine;

public class MogaGamepad
{
	public delegate void ConnectionDelegate(bool connect);

	public delegate void BatteryDelegate(bool low);

	private const float remindBatteryTime = 90f;

	private static MogaWrapper m_MogaController;

	private static bool IsMogaConnected;

	private static bool m_IsMogaPro;

	private static bool BatteryLow;

	private static float LastTimeBatteryCheck;

	public static ConnectionDelegate OnConnectionChange;

	public static BatteryDelegate OnBatteryLowChange;

	public static void Init()
	{
		Debug.Log("Moga Init");
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		if (androidJavaClass == null)
		{
			Debug.Log("unity player not found");
			return;
		}
		AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		if (androidJavaObject == null)
		{
			Debug.Log("current activity not found");
			return;
		}
		AndroidJavaObject instance = MogaWrapper.getInstance(androidJavaObject);
		if (instance == null)
		{
			Debug.Log("moga controller not found");
			return;
		}
		m_MogaController = new MogaWrapper(instance);
		m_MogaController.init();
	}

	public static void Done()
	{
		Debug.Log("Moga Done");
		if (m_MogaController != null)
		{
			m_MogaController.exit();
		}
		m_MogaController = null;
	}

	public static bool IsConnected()
	{
		return m_MogaController != null && IsMogaConnected;
	}

	public static bool IsMogaPro()
	{
		return m_MogaController != null && m_IsMogaPro;
	}

	public static void OnFocus(bool focused)
	{
		if (m_MogaController != null)
		{
			if (focused)
			{
				m_MogaController.onResume();
			}
			else
			{
				m_MogaController.onPause();
			}
		}
	}

	public static float GetAxis(int axis)
	{
		if (m_MogaController == null)
		{
			return 0f;
		}
		return m_MogaController.getAxisValue(axis);
	}

	public static int GetKeyCode(int keyCode)
	{
		if (m_MogaController == null)
		{
			return 1;
		}
		return m_MogaController.getKeyCode(keyCode);
	}

	public static void Update()
	{
		if (m_MogaController == null)
		{
			return;
		}
		int state = m_MogaController.getState(1);
		bool flag = state == 1;
		if (flag != IsMogaConnected)
		{
			if (OnConnectionChange != null)
			{
				OnConnectionChange(flag);
			}
			IsMogaConnected = flag;
			Debug.Log("Moga controller " + ((!flag) ? "disconected" : "connected"));
			m_IsMogaPro = m_MogaController.getState(4) == 1;
		}
		bool flag2 = m_MogaController.getState(2) == 1;
		if (flag2 != BatteryLow || (flag2 && Time.timeSinceLevelLoad > LastTimeBatteryCheck + 90f))
		{
			if (OnBatteryLowChange != null)
			{
				OnBatteryLowChange(flag2);
			}
			BatteryLow = flag2;
			LastTimeBatteryCheck = Time.timeSinceLevelLoad;
			Debug.Log("Moga gamepad battery is low");
		}
	}
}
