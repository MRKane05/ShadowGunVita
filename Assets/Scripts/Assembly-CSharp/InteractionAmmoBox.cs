using UnityEngine;

[AddComponentMenu("Interaction/Ammo Box")]
public class InteractionAmmoBox : InteractionObject
{
	public int RefreshTime;

	public GameObject GameObject { get; private set; }

	private void Awake()
	{
		Transform = base.transform;
		GameObject = base.gameObject;
	}

	private void Start()
	{
		Initialize();
		base.DisableDuringFight = false;
	}

	private void OnDestroy()
	{
		GameObject = null;
	}

	public override void Reset()
	{
		base.Reset();
		CancelInvoke("Refreshed");
	}

	public override void DoInteraction()
	{
		base.DoInteraction();
		base.InteractionObjectUsable = false;
		Disable();
		Invoke("Refreshed", RefreshTime);
		Player.Instance.LoadAllWeapon();
		GuiHUD.Instance.ShowMessageTimed(GuiHUD.E_HudMessageType.E_010_SupplyCrate, GuiHUD.InfoMessageTime);
	}

	private void Refreshed()
	{
		base.InteractionObjectUsable = true;
		Enable();
	}
}
