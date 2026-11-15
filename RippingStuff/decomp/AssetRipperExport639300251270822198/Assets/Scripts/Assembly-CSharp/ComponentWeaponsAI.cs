using System;
using System.Collections.Generic;

[Serializable]
public class ComponentWeaponsAI : ComponentWeapons
{
	public override void Initialize()
	{
		base.Initialize();
		E_WeaponType[] weaponStart = WeaponStart;
		foreach (E_WeaponType e_WeaponType in weaponStart)
		{
			base.Weapons.Add(e_WeaponType, WeaponManager.Instance.GetWeapon(e_WeaponType));
		}
		base.CurrentWeapon = E_WeaponType.None;
		AddWeaponToOwnerHand(base.DefaultWeapon);
	}

	private void Activate()
	{
		GetCurrentWeapon().ShowWeapon(true);
		Owner.WorldState.SetWSProperty(E_PropKey.WeaponLoaded, GetCurrentWeapon().ClipAmmo > 0);
	}

	private void Deactivate()
	{
		foreach (KeyValuePair<E_WeaponType, WeaponBase> weapon in base.Weapons)
		{
			weapon.Value.Reset(false);
		}
	}
}
