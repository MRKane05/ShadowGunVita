using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Weapons/WeaponManager")]
internal class WeaponManager : MonoBehaviour
{
	public static WeaponManager Instance;

	private Dictionary<E_WeaponType, ResourceCache> WeaponsCache = new Dictionary<E_WeaponType, ResourceCache>();

	private Dictionary<E_WeaponType, ResourceCache> AmmoCache = new Dictionary<E_WeaponType, ResourceCache>();

	private void Awake()
	{
		Instance = this;
		WeaponsCache[E_WeaponType.SMG] = new ResourceCache("Weapons/WeaponSMG", 1);
		WeaponsCache[E_WeaponType.SMGMutant] = new ResourceCache("Weapons/WeaponSMGMutant", 1);
		WeaponsCache[E_WeaponType.PistolMutant] = new ResourceCache("Weapons/WeaponPistolMutant", 1);
		WeaponsCache[E_WeaponType.Shotgun] = new ResourceCache("Weapons/WeaponShotgun", 1);
		WeaponsCache[E_WeaponType.GrenadeLauncher] = new ResourceCache("Weapons/WeaponGrenadeLauncher", 1);
		WeaponsCache[E_WeaponType.GrenadeLauncherMutant] = new ResourceCache("Weapons/WeaponGrenadeLauncherMutant", 1);
		WeaponsCache[E_WeaponType.RocketLauncher] = new ResourceCache("Weapons/WeaponRocketLauncher", 1);
		WeaponsCache[E_WeaponType.GatlingMutant] = new ResourceCache("Weapons/WeaponGatlingMutant", 1);
		WeaponsCache[E_WeaponType.PlasmaRifle] = new ResourceCache("Weapons/WeaponPlasma", 1);
		AmmoCache[E_WeaponType.SMG] = new ResourceCache("Weapons/AmmoSMG", 1);
		AmmoCache[E_WeaponType.SMGMutant] = new ResourceCache("Weapons/AmmoSMG", 1);
		AmmoCache[E_WeaponType.Shotgun] = new ResourceCache("Weapons/AmmoShells", 1);
		AmmoCache[E_WeaponType.PistolMutant] = new ResourceCache("Weapons/AmmoShells", 1);
		AmmoCache[E_WeaponType.GrenadeLauncher] = new ResourceCache("Weapons/AmmoGrenades", 1);
		AmmoCache[E_WeaponType.GrenadeLauncherMutant] = new ResourceCache("Weapons/AmmoGrenades", 1);
		AmmoCache[E_WeaponType.RocketLauncher] = new ResourceCache("Weapons/AmmoRockets", 1);
		AmmoCache[E_WeaponType.GatlingMutant] = new ResourceCache("Weapons/AmmoRockets", 1);
		AmmoCache[E_WeaponType.PlasmaRifle] = new ResourceCache("Weapons/AmmoPlasma", 1);
	}

	public WeaponBase GetWeapon(E_WeaponType type)
	{
		if (!WeaponsCache.ContainsKey(type))
		{
			Debug.LogError("WeaponManager: unknown type " + type);
			return null;
		}
		if (WeaponsCache[type] == null)
		{
			Debug.LogError(string.Concat("WeaponManager: For this type ", type, " we don't have resource"));
			return null;
		}
		GameObject gameObject = WeaponsCache[type].Get();
		return (!gameObject) ? null : gameObject.GetComponent<WeaponBase>();
	}

	public void Return(WeaponBase weapon)
	{
		if (weapon == null)
		{
			Debug.LogError("WeaponManager: sombody is trying return null object to cache");
			return;
		}
		if (!WeaponsCache.ContainsKey(weapon.WeaponType))
		{
			Debug.LogError("WeaponManager: unknown type " + weapon.WeaponType);
			return;
		}
		if (WeaponsCache[weapon.WeaponType] == null)
		{
			Debug.LogError("WeaponManager: We don't have cache for this weapon type. This object was not created by this manager");
			return;
		}
		weapon.Reset(true);
		WeaponsCache[weapon.WeaponType].Return(weapon.gameObject);
	}

	public AmmoBox GetAmmo(E_WeaponType type)
	{
		if (!AmmoCache.ContainsKey(type))
		{
			Debug.LogError("AmmoFactory: unknown type " + type);
			return null;
		}
		if (AmmoCache[type] == null)
		{
			Debug.LogError(string.Concat("AmmoFactory: For this type ", type, " we don't have resource"));
			return null;
		}
		GameObject gameObject = AmmoCache[type].Get();
		AmmoBox ammoBox = ((!gameObject) ? null : gameObject.GetComponent<AmmoBox>());
		if (ammoBox != null)
		{
			ammoBox.cacheKey = type;
		}
		return ammoBox;
	}

	public void Return(AmmoBox ammo)
	{
		if (ammo == null)
		{
			Debug.LogError("AmmoFactory: sombody is trying return null object to cache");
			return;
		}
		if (!AmmoCache.ContainsKey(ammo.cacheKey))
		{
			Debug.LogError("AmmoFactory: unknown type " + ammo.cacheKey);
			return;
		}
		if (AmmoCache[ammo.cacheKey] == null)
		{
			Debug.LogError("AmmoFactory: We don't have cache for this weapon type. This object was not created by this manager");
			return;
		}
		ammo.Reset();
		AmmoCache[ammo.cacheKey].Return(ammo.gameObject);
	}

	public void Clear()
	{
		WeaponsCache.Clear();
		AmmoCache.Clear();
	}
}
