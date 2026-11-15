using UnityEngine;

[AddComponentMenu("Weapons/WeaponSMG")]
public class WeaponSMG : WeaponBase
{
	private void Awake()
	{
		Initialize(E_WeaponType.SMG, E_ProjectileType.SMG);
		base.IsPlayerWeapon = true;
	}
}
