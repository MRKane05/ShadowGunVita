using UnityEngine;

[AddComponentMenu("Weapons/WeaponPlasma")]
public class WeaponPlasma : WeaponBase
{
	public override bool IsFull
	{
		get
		{
			return base.WeaponAmmo == MaxAmmoInClip;
		}
	}

	private void Awake()
	{
		Initialize(E_WeaponType.PlasmaRifle, E_ProjectileType.Plasma);
		base.IsPlayerWeapon = true;
	}

	public override void DecreaseAmmo()
	{
		if (Owner.IsPlayer)
		{
			_AmmoInClip--;
		}
	}

	public override void AddAmmo(int ammo)
	{
		if (ammo == -1)
		{
			_AmmoInClip = MaxAmmoInClip;
			_AmmoInWeapon = MaxAmmoInWeapon;
		}
		else
		{
			_AmmoInClip = Mathf.Min(_AmmoInClip + ammo, MaxAmmoInClip);
		}
	}
}
