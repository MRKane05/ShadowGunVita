using UnityEngine;

[AddComponentMenu("Interaction/Mission Fail")]
public class MissionFail : MonoBehaviour
{
	public enum E_Messages
	{
		NONE = -1,
		Death = 0,
		FailedSaveSara = 1
	}

	public E_Messages messageType = E_Messages.NONE;

	public EventTarget onEvents = new EventTarget();

	private void Awake()
	{
		onEvents.OnActivate = OnActivate;
	}

	private void Start()
	{
		onEvents.Initialize();
	}

	private void OnActivate()
	{
		if (Player.Instance.Owner.IsAlive)
		{
			Player.Instance.StopMove(true);
		}
		int failMsg = (int)messageType;
		Mission.Instance.EndOfMissionFail((GuiHUD.E_HudMessageType)failMsg);
	}
}
