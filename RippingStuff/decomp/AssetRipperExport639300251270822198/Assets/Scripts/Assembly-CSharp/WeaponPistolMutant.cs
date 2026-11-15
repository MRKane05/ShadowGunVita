using UnityEngine;

public class WeaponPistolMutant : WeaponBase
{
	public int ProjectilesPerShot = 8;

	private void Awake()
	{
		Initialize(E_WeaponType.PistolMutant, E_ProjectileType.SMG);
		base.IsPlayerWeapon = false;
	}

	protected override void SpawnProjectile(Vector3 direction)
	{
		for (int i = 0; i < ProjectilesPerShot; i++)
		{
			float num = ((Game.Instance.GameDifficulty != E_GameDifficulty.Hard) ? Dispersion : (Dispersion * 0.5f));
			Temp.SetLookRotation(direction);
			Temp.eulerAngles = new Vector3(Temp.eulerAngles.x + Random.Range(0f - num, num), Temp.eulerAngles.y + Random.Range(0f - num, num), 0f);
			InitProjSettings.Agent = Owner;
			ProjectileManager.Instance.SpawnProjectile(base.ProjectileType, TransformShot.position, Temp * Vector3.forward, InitProjSettings);
		}
	}
}
