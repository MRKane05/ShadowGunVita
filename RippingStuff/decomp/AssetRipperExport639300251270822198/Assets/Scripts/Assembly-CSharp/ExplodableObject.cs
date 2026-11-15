using UnityEngine;

public class ExplodableObject : MonoBehaviour, IGameZoneControledObject, IHitZoneOwner
{
	private float m_DefaultHitPoints;

	public float m_HitPoints = 100f;

	public Explosion m_Explosion;

	public Transform m_ExplosionOrigin;

	public float m_ExplosionDamage = -1f;

	public GameObject[] m_HideObjects;

	public GameObject[] m_ShowObjects;

	private GameObject m_GameObject;

	private void Awake()
	{
		m_GameObject = base.gameObject;
		m_DefaultHitPoints = m_HitPoints;
		m_ExplosionOrigin = ((!(m_ExplosionOrigin != null)) ? base.transform : m_ExplosionOrigin);
		GameZone firstComponentUpward = base.gameObject.GetFirstComponentUpward<GameZone>();
		firstComponentUpward.RegisterControllableObject(this);
	}

	private void OnDestroy()
	{
		m_Explosion = null;
	}

	public void OnProjectileHit(Projectile inProjectile, HitZone inHitZone)
	{
		if (!(m_HitPoints <= 0f))
		{
			m_HitPoints -= inProjectile.Damage * inHitZone.DamageModifier;
			if (m_HitPoints <= 0f)
			{
				Break();
			}
		}
	}

	public void OnReceiveRangeDamage(float inDamage, HitZone inHitZone)
	{
		if (!(m_HitPoints <= 0f))
		{
			m_HitPoints -= inDamage * inHitZone.DamageModifier;
			if (m_HitPoints <= 0f)
			{
				Break();
			}
		}
	}

	public virtual void Reset()
	{
		m_HitPoints = m_DefaultHitPoints;
		GameObject[] hideObjects = m_HideObjects;
		foreach (GameObject gameObject in hideObjects)
		{
			gameObject.active = true;
		}
		GameObject[] showObjects = m_ShowObjects;
		foreach (GameObject gameObject2 in showObjects)
		{
			gameObject2.active = false;
		}
	}

	public void Enable()
	{
		m_GameObject.SetActiveRecursively(true);
		GameObject[] showObjects = m_ShowObjects;
		foreach (GameObject gameObject in showObjects)
		{
			gameObject.active = false;
		}
	}

	public void Disable()
	{
	}

	protected virtual void Break()
	{
		if (m_Explosion != null)
		{
			Explosion explosion = Mission.Instance.ExplosionCache.Get(m_Explosion, m_ExplosionOrigin.position, m_ExplosionOrigin.rotation);
			if (!explosion && m_ExplosionDamage >= 0f)
			{
				explosion.damage = m_ExplosionDamage;
			}
		}
		GameObject[] hideObjects = m_HideObjects;
		foreach (GameObject gameObject in hideObjects)
		{
			gameObject.active = false;
		}
		GameObject[] showObjects = m_ShowObjects;
		foreach (GameObject gameObject2 in showObjects)
		{
			gameObject2.active = true;
		}
	}
}
