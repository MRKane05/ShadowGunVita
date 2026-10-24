using UnityEngine;

[AddComponentMenu("Interaction/Show Message")]
public class ShowGameMessage : MonoBehaviour
{
	public enum E_GameMessage
	{
		NONE = -1,
		DoorsUnlocked = 0,
		ElevatorActivated = 1,
		LadderActivated = 2,
		ExcavatorProgram = 3,
		Forcefield = 4,
		TerminalMalfunction = 5,
		PowernodeRestarted = 6,
		PlatformActivated = 7,
		IgnitorDeactivated = 8,
		IgnitorActivated = 9,
		CargoPlatform = 10,
		CargoGate = 11,
		DeactivateMissile = 12,
		MissileDeactivated = 13,
		BulkheadOpened = 14,
		PowerDeactivated = 15,
		CraneActivated = 16,
		Tutorial01 = 17,
		Tutorial02 = 18,
		Tutorial03 = 19,
		Tutorial04 = 20,
		Tutorial05 = 21,
		KeycardPicked = 22,
		FindKeycard = 23,
		ActivateBeacon = 24,
		FindWayFacility = 25,
		Investigate = 26,
		FanBlock = 27,
		HotCircuit = 28,
		Pursue = 29,
		RestorePower = 30,
		ElevatorPower = 31,
		FindWayDestroy = 32,
		UnlockEject = 33,
		FirstUnitUnlock = 34,
		FirstUnitEject = 35,
		SecondUnitUnlock = 36,
		SecondUnitEject = 37,
		ThirdUnitUnlock = 38,
		ThirdUnitEject = 39,
		FindWayEject = 40,
		InjectFuel = 41,
		Overheating = 42,
		Escape = 43
	}

	public E_GameMessage message = E_GameMessage.NONE;

	public float time = 5f;

	public EventTarget onEvents = new EventTarget();

	private void Awake()
	{
		onEvents.OnActivate = OnActivate;
		onEvents.OnDeactivate = OnDeactivate;
	}

	private void Start()
	{
		onEvents.Initialize();
	}

	private void OnActivate()
	{
		int num = (int)(message + 15);
		GuiHUD.E_HudMessageType e_HudMessageType = (GuiHUD.E_HudMessageType)num;
		if (time > 0f)
		{
			GuiHUD.Instance.ShowMessageTimed(e_HudMessageType, time);
		}
		else
		{
			GuiHUD.Instance.ShowMessage(e_HudMessageType, true);
		}
	}

	private void OnDeactivate()
	{
		int num = (int)(message + 15);
		GuiHUD.E_HudMessageType e_HudMessageType = (GuiHUD.E_HudMessageType)num;
		GuiHUD.Instance.ShowMessage(e_HudMessageType, false);
	}
}
