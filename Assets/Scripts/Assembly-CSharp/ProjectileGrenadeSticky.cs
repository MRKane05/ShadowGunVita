using UnityEngine;

[AddComponentMenu("Weapons/ProjectileGrenadeSticky")]
public class ProjectileGrenadeSticky : Projectile
{
	public Explosion m_Explosion;

	public float m_ExplodeAfter = 5f;

	public int m_PlayHitSoundCount = 3;

	private int m_PlayHitSound;

	public override void ProjectileInit(Vector3 pos, Vector3 dir, InitSettings inSettings)
	{
		base.Transform.position = pos;
		base.Transform.forward = dir;
		Settings = inSettings;
		m_PlayHitSound = m_PlayHitSoundCount;
		Invoke("Explode", m_ExplodeAfter);
		base.GetComponent<Rigidbody>().velocity = base.transform.forward * base.Speed;
		base.GetComponent<Rigidbody>().SetDensity(1.5f);
		base.GetComponent<Rigidbody>().AddTorque(Random.insideUnitSphere * 10f, ForceMode.VelocityChange);
	}

	public override void ProjectileUpdate()
	{
	}

	public override void ProjectileDeinit()
	{
		base.Transform.parent = null;
	}

	public override bool IsFinished()
	{
		return !IsInvoking("Explode");
	}

	internal void Explode()
	{
		CancelInvoke("Explode");
		if (m_Explosion != null)
		{
			Explosion explosion = Mission.Instance.ExplosionCache.Get(m_Explosion, base.Transform.position, base.Transform.rotation);
			explosion.causer = base.Agent;
			explosion.damage = base.Damage;
		}
	}

	internal void OnCollisionEnter(Collision collision)
	{
		if (!base.Agent || !collision.transform.IsChildOf(base.Agent.Transform))
		{
			Agent component = collision.gameObject.GetComponent<Agent>();
			if ((bool)component && component.IsPlayer && IsInvoking("Explode"))
			{
				base.Transform.parent = component.transform;
			}
			else if (m_PlayHitSound > 0)
			{
				m_PlayHitSound--;
				PlayHitSound(base.GetComponent<Collider>().gameObject.layer);
			}
		}
	}
}
