using UnityEngine;

[AddComponentMenu("Weapons/WeaponRocketLauncher")]
public class WeaponRocketLauncher : WeaponBase
{
	private void Awake()
	{
		Initialize(E_WeaponType.RocketLauncher, E_ProjectileType.Rocket);
		base.IsPlayerWeapon = true;
	}
}
