using UnityEngine;

[AddComponentMenu("Weapons/WeaponPistol")]
public class WeaponPistol : WeaponBase
{
	private void Awake()
	{
		Initialize(E_WeaponType.Pistol, E_ProjectileType.SMG);
		base.IsPlayerWeapon = true;
	}
}
