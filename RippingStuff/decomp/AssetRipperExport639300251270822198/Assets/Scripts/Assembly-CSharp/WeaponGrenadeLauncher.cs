using UnityEngine;

[AddComponentMenu("Weapons/WeaponGrenadeLauncher")]
public class WeaponGrenadeLauncher : WeaponBase
{
	private void Awake()
	{
		Initialize(E_WeaponType.GrenadeLauncher, E_ProjectileType.Grenade);
		base.IsPlayerWeapon = true;
	}
}
