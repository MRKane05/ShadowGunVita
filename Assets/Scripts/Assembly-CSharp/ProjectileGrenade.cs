using UnityEngine;

[AddComponentMenu("Weapons/ProjectileGrenade")]
public class ProjectileGrenade : Projectile
{
	public Explosion m_Explosion;

	public Vector3 m_ExplosionOffset = Vector3.zero;

	public float m_ExplodeAfter = 5f;

	public int m_PlayHitSoundCount = 3;

	private int m_PlayHitSound;

	private Collider m_WaterVolume;

	private void OnDestroy()
	{
		m_Explosion = null;
	}

	public override void ProjectileInit(Vector3 pos, Vector3 dir, InitSettings inSettings)
	{
		base.Transform.position = pos;
		base.Transform.forward = dir;
		Settings = inSettings;
		m_PlayHitSound = m_PlayHitSoundCount;
		Invoke("Explode", m_ExplodeAfter);
		base.GetComponent<Rigidbody>().velocity = base.transform.forward * base.Speed;
		base.GetComponent<Rigidbody>().SetDensity(1.5f);
		base.GetComponent<Rigidbody>().WakeUp();
	}

	public override void ProjectileUpdate()
	{
	}

	public override void ProjectileDeinit()
	{
		m_WaterVolume = null;
		CancelInvoke();
		base.GetComponent<Rigidbody>().Sleep();
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
			Explosion explosion = Mission.Instance.ExplosionCache.Get(m_Explosion, base.Transform.position + m_ExplosionOffset, base.Transform.rotation, new Transform[2] { base.transform, Settings.IgnoreTransform });
			explosion.causer = base.Agent;
			explosion.damage = base.Damage;
		}
	}

	internal void OnCollisionEnter(Collision collision)
	{
		if (!IsInvoking("Explode") || ((bool)base.Agent && collision.transform.IsChildOf(base.Agent.Transform)))
		{
			return;
		}
		Agent component = collision.gameObject.GetComponent<Agent>();
		HitZone component2 = collision.gameObject.GetComponent<HitZone>();
		AgentHuman agentHuman = component as AgentHuman;
		if ((bool)agentHuman && !agentHuman.BlackBoard.GrenadesExplodeOnHit)
		{
			if (m_PlayHitSound > 0)
			{
				m_PlayHitSound--;
				PlayHitSound(base.GetComponent<Collider>().gameObject.layer);
			}
		}
		else if ((bool)component)
		{
			Explode();
		}
		else if ((bool)component2)
		{
			Explode();
		}
		else if (m_WaterVolume == null && m_PlayHitSound > 0)
		{
			m_PlayHitSound--;
			PlayHitSound(base.GetComponent<Collider>().gameObject.layer);
		}
	}
}
