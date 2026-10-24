using UnityEngine;

public class Armor : MonoBehaviour, IHitZoneOwner
{
	public delegate void HitDelegate(float damage);

	public delegate void DestroyDelegate();

	public float hitPoints = 400f;

	private float origHitPoints;

	public HitDelegate OnHit;

	public DestroyDelegate OnDestroy;

	public Explosion m_Explosion;

	private void Awake()
	{
		origHitPoints = hitPoints;
	}

	public void Enable()
	{
		if (hitPoints > 0f)
		{
			base.gameObject.active = true;
		}
		else
		{
			base.gameObject.active = false;
		}
	}

	public void Disable()
	{
		base.gameObject.active = false;
	}

	public void Reset()
	{
		hitPoints = origHitPoints;
	}

	public void OnProjectileHit(Projectile inProjectile, HitZone inHitZone)
	{
		DoHitDamage(inProjectile.Damage * inHitZone.DamageModifier);
	}

	public void OnReceiveRangeDamage(float inDamage, HitZone inHitZone)
	{
		DoHitDamage(inDamage * inHitZone.DamageModifier);
	}

	private void DoHitDamage(float damage)
	{
		if (!(hitPoints <= 0f) && base.gameObject.active)
		{
			hitPoints -= damage;
			if (OnHit != null)
			{
				OnHit(damage);
			}
			if (hitPoints <= 0f)
			{
				Destroy();
			}
		}
	}

	private void Destroy()
	{
		OnDestroy();
		if (m_Explosion != null)
		{
			Explosion explosion = Mission.Instance.ExplosionCache.Get(m_Explosion, base.transform.position, base.transform.rotation);
			if ((bool)explosion)
			{
				explosion.damage = 0f;
			}
		}
		base.gameObject.SetActiveRecursively(false);
	}
}
