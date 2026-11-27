using UnityEngine;

// The player's Gatling. A copy of the enemy WeaponGatling (WeaponGatlingMutant) with its own weapon
// type, so the player version can be tuned (ammo, position, scale) without touching the original.
// The prefab (Resources/weapons/WeaponGatlingPlayer) keeps the gun model, muzzle and shoot point
// on a "Model" child: weapons are snapped to the hand bone at zero position/rotation when equipped,
// so adjust the Model child to change how the gun sits in the hands.
[AddComponentMenu("Weapons/Player Weapon Gatling")]
public class WeaponGatlingPlayer : WeaponBase
{
	private void Awake()
	{
		Initialize(E_WeaponType.Gatling, E_ProjectileType.SMG);
		base.IsPlayerWeapon = true;
	}
}
