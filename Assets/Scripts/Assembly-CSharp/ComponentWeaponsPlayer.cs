using System;
using System.Collections.Generic;

[Serializable]
public class ComponentWeaponsPlayer : ComponentWeapons
{
	public override void Initialize()
	{
		base.Initialize();
		if (Game.Instance.PlayerPersistantInfo.Save_IsLoaded())
		{
			Game.PlayerPersistantState.InventoryInfo inventory = Game.Instance.PlayerPersistantInfo.Inventory;
			E_WeaponType weaponSelected = inventory.WeaponSelected;
			foreach (KeyValuePair<E_WeaponType, Game.PlayerPersistantState.InventoryInfo.WeaponInfo> weapon2 in inventory.Weapons)
			{
				WeaponBase weapon = WeaponManager.Instance.GetWeapon(weapon2.Key);
				weapon.SetAmmo(weapon2.Value.AmmoInClip, weapon2.Value.AmmonInWeapon);
				base.Weapons.Add(weapon2.Key, weapon);
			}
			AddWeaponToOwnerHand(base.Weapons[weaponSelected]);
		}
		else
		{
			E_WeaponType[] weaponStart = WeaponStart;
			foreach (E_WeaponType e_WeaponType in weaponStart)
			{
				base.Weapons.Add(e_WeaponType, WeaponManager.Instance.GetWeapon(e_WeaponType));
			}
			base.CurrentWeapon = E_WeaponType.None;
			AddWeaponToOwnerHand(base.DefaultWeapon);
		}
		Owner.WorldState.SetWSProperty(E_PropKey.WeaponLoaded, GetCurrentWeapon().ClipAmmo > 0);
		GuiHUD.Instance.OnCurrentWeaponChanged(base.CurrentWeapon);
	}

	private void Deactivate()
	{
		foreach (KeyValuePair<E_WeaponType, WeaponBase> weapon in base.Weapons)
		{
			WeaponManager.Instance.Return(weapon.Value);
		}
		base.Weapons.Clear();
		base.CurrentWeapon = E_WeaponType.None;
	}
}
