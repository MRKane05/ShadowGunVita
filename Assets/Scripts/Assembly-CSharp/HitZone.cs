using UnityEngine;

public class HitZone : MonoBehaviour, IProjectileHitReceiver
{
	private IHitZoneOwner m_HitZoneOwner;

	public float DamageModifier = 1f;

	private void Awake()
	{
		Collider component = GetComponent<Collider>();
		if (component == null)
		{
			Debug.LogError("Invalid HitZone [ " + DebugUtils.GetFullName(base.gameObject) + " ] . Some collider must exist on gameObject");
		}
		m_HitZoneOwner = base.gameObject.GetFirstComponentUpwardWithInterface<IHitZoneOwner>();
		if (m_HitZoneOwner == null)
		{
			Debug.LogError("Invalid HitZone [ " + DebugUtils.GetFullName(base.gameObject) + " ] . HIt Zone Owner have to be defined");
			Debug.LogError("Invalid HitZone [ " + DebugUtils.GetFullName(base.gameObject) + " ] . HIt Zone Owner must implement IHitZoneOwner interface ");
		}
	}

	public void OnProjectileHit(Projectile projectile)
	{
		if (m_HitZoneOwner != null)
		{
			m_HitZoneOwner.OnProjectileHit(projectile, this);
		}
	}

	public void OnReceiveRangeDamage(Agent attacker, float damage, Vector3 impuls, E_WeaponType weaponType)
	{
		if (m_HitZoneOwner != null)
		{
			m_HitZoneOwner.OnReceiveRangeDamage(damage, this);
		}
	}
}
