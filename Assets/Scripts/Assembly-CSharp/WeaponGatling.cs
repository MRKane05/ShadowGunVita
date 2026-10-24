using UnityEngine;

[AddComponentMenu("Weapons/Mutant Weapon Gatling")]
public class WeaponGatling : WeaponBase
{
	private void Awake()
	{
		Initialize(E_WeaponType.GatlingMutant, E_ProjectileType.SMG);
		base.IsPlayerWeapon = false;
	}
}
