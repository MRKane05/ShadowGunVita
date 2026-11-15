using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ComponentWeapons : MonoBehaviour
{
	public E_WeaponType[] WeaponStart = new E_WeaponType[0];

	public Transform Hand;

	protected AgentHuman Owner;

	public Dictionary<E_WeaponType, WeaponBase> Weapons { get; protected set; }

	public E_WeaponType CurrentWeapon { get; protected set; }

	protected WeaponBase DefaultWeapon
	{
		get
		{
			return Weapons[WeaponStart[0]];
		}
	}

	public WeaponBase GetCurrentWeapon()
	{
		return (CurrentWeapon == E_WeaponType.None) ? null : Weapons[CurrentWeapon];
	}

	public WeaponBase GetWeapon(E_WeaponType t)
	{
		return (!Weapons.ContainsKey(t)) ? null : Weapons[t];
	}

	private void Awake()
	{
		Owner = GetComponent<AgentHuman>();
		BlackBoard blackBoard = Owner.BlackBoard;
		blackBoard.ActionHandler = (BlackBoard.AgentActionHandler)Delegate.Combine(blackBoard.ActionHandler, new BlackBoard.AgentActionHandler(HandleAction));
		Weapons = new Dictionary<E_WeaponType, WeaponBase>();
	}

	public virtual void Initialize()
	{
		CurrentWeapon = E_WeaponType.None;
	}

	private void LateUpdate()
	{
		if (CurrentWeapon != E_WeaponType.None)
		{
			WeaponBase weaponBase = Weapons[CurrentWeapon];
			if (!weaponBase.IsBusy() && Owner.BlackBoard.Desires.WeaponTriggerOn && weaponBase.ClipAmmo > 0)
			{
				AgentActionAttack agentActionAttack = AgentActionFactory.Create(AgentActionFactory.E_Type.Attack) as AgentActionAttack;
				agentActionAttack.AttackDir = Owner.BlackBoard.FireDir;
				Owner.BlackBoard.ActionAdd(agentActionAttack);
			}
		}
	}

	public void AddWeapon(E_WeaponType weapon)
	{
		if (Weapons.ContainsKey(weapon))
		{
			Weapons[weapon].AddAmmo(-1);
			return;
		}
		WeaponBase weapon2 = WeaponManager.Instance.GetWeapon(weapon);
		Weapons.Add(weapon2.WeaponType, weapon2);
		Owner.BlackBoard.Desires.Weapon = weapon;
	}

	protected void AddWeaponToOwnerHand(WeaponBase weapon)
	{
		CurrentWeapon = weapon.WeaponType;
		Owner.WorldState.SetWSProperty(E_PropKey.WeaponLoaded, weapon.ClipAmmo > 0);
		weapon.LinkToOwner(Owner, Hand);
	}

	public void SwitchWeapons(E_WeaponType weaponType)
	{
		if (Weapons.ContainsKey(weaponType))
		{
			Weapons[CurrentWeapon].UnlinkFromOwner();
			CurrentWeapon = weaponType;
			Weapons[weaponType].LinkToOwner(Owner, Hand);
			Owner.WorldState.SetWSProperty(E_PropKey.WeaponLoaded, Weapons[weaponType].ClipAmmo > 0);
			Weapons[weaponType].WeaponArm();
		}
	}

	public void HandleAction(AgentAction action)
	{
		if (action.IsFailed())
		{
			return;
		}
		if (action is AgentActionAttack)
		{
			WeaponBase weaponBase = Weapons[CurrentWeapon];
			weaponBase.Fire((action as AgentActionAttack).AttackDir);
			if (weaponBase.ClipAmmo == 0)
			{
				Owner.WorldState.SetWSProperty(E_PropKey.WeaponLoaded, false);
			}
		}
		else if (action is AgentActionReload)
		{
			WeaponBase weaponBase2 = Weapons[CurrentWeapon];
			weaponBase2.Reload();
			Owner.WorldState.SetWSProperty(E_PropKey.WeaponLoaded, true);
		}
	}

	public void DropAmmo()
	{
		if (Owner.IsInCover)
		{
			Mission.Instance.CurrentGameZone.RegisterDroppedAmmoClip(CurrentWeapon, Owner.Position + Owner.BlackBoard.Cover.Forward * -0.5f + Vector3.up * 0.7f);
		}
		else
		{
			Mission.Instance.CurrentGameZone.RegisterDroppedAmmoClip(CurrentWeapon, Owner.Position + Vector3.up * 0.7f);
		}
	}

	public void AddAmmoToWeapon(AmmoBox a)
	{
		if (Weapons.ContainsKey(a.ForWeaponType) && !Weapons[a.ForWeaponType].IsFull)
		{
			Weapons[a.ForWeaponType].AddAmmo(a.Ammo);
			a.Disable();
			if (a.Dropped)
			{
				Mission.Instance.CurrentGameZone.UnRegisterDroppedAmmoClip(a);
			}
			Owner.SoundPlay(Player.Instance.SoundTakeAmmo);
			if ((bool)GuiHUD.Instance)
			{
				GuiHUD.Instance.ShowMessageTimed(GuiHUD.WeaponAmmoMessage(a.ForWeaponType), GuiHUD.InfoMessageTime);
			}
		}
	}

	public void AddAmmoToAllWeapon()
	{
		foreach (KeyValuePair<E_WeaponType, WeaponBase> weapon in Weapons)
		{
			weapon.Value.AddAmmo(-1);
		}
		Owner.SoundPlay(Player.Instance.SoundTakeAmmoFromBox);
	}

	public void DisableCurrentWeapon(float disableTime)
	{
		if (CurrentWeapon != E_WeaponType.None)
		{
			Weapons[CurrentWeapon].SetBusy(disableTime);
		}
	}
}
