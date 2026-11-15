public interface IHitZoneOwner
{
	void OnProjectileHit(Projectile inProjectile, HitZone inHitZone);

	void OnReceiveRangeDamage(float inDamage, HitZone inHitZone);
}
