using UnityEngine;

[AddComponentMenu("Weapons/WeaponSMGMutant")]
public class WeaponSMGMutant : WeaponBase
{
	private void Awake()
	{
		Initialize(E_WeaponType.SMGMutant, E_ProjectileType.SMG);
		base.IsPlayerWeapon = false;
	}
}
