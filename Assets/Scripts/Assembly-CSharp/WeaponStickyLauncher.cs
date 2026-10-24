using UnityEngine;

[AddComponentMenu("Weapons/WeaponStickyLauncher")]
public class WeaponStickyLauncher : WeaponBase
{
	private void Awake()
	{
		Initialize(E_WeaponType.StickyLauncher, E_ProjectileType.Sticky);
		base.IsPlayerWeapon = true;
	}
}
