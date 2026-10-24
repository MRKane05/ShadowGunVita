using UnityEngine;

[AddComponentMenu("Weapons/WeaponGrenadeLauncherMutant")]
public class WeaponGrenadeLauncherMutant : WeaponBase
{
	private void Awake()
	{
		Initialize(E_WeaponType.GrenadeLauncherMutant, E_ProjectileType.Grenade);
		base.IsPlayerWeapon = false;
	}
}
