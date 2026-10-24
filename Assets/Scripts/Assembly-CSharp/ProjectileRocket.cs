using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Weapons/ProjectileRocket")]
public class ProjectileRocket : Projectile
{
	public Explosion m_Explosion;

	public Vector3 m_ExplosionOffset = Vector3.zero;

	private Agent m_Target;

	public float m_AngularSpeed = 90f;

	public ParticleSystem m_Trail;

	public override void ProjectileInit(Vector3 pos, Vector3 dir, InitSettings inSettings)
	{
		base.Transform.position = pos;
		base.Transform.forward = dir;
		Settings = inSettings;
		Timer = 0f;
		Hit = false;
		base.GetComponent<Rigidbody>().detectCollisions = true;
		if (!(inSettings.Agent is AgentHuman))
		{
			m_Target = Player.Instance.Owner;
		}
	}

	public override void ProjectileUpdate()
	{
		if (Hit)
		{
			return;
		}
		if (m_Target != null)
		{
			NavigateToTarget(m_Target.ChestPosition);
		}
		Vector3 position = base.Transform.position + base.Transform.forward * Settings.Speed * Time.deltaTime;
		RaycastHit[] array = base.GetComponent<Rigidbody>().SweepTestAll(base.Transform.forward, Settings.Speed * Time.deltaTime);
		if (array.Length > 1)
		{
			Array.Sort(array, CollisionUtils.CompareHits);
		}
		RaycastHit[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			RaycastHit raycastHit = array2[i];
			if (!(raycastHit.transform == Settings.IgnoreTransform))
			{
				base.Transform.position = raycastHit.point;
				if (!raycastHit.collider.isTrigger)
				{
					position = raycastHit.point;
					HitReaction();
					break;
				}
				raycastHit.transform.SendMessage("OnProjectileHit", this, SendMessageOptions.DontRequireReceiver);
			}
		}
		base.Transform.position = position;
	}

	public override void ProjectileDeinit()
	{
		base.GetComponent<Rigidbody>().detectCollisions = true;
	}

	public override bool IsFinished()
	{
		return Hit && !IsTrailVisible();
	}

	public void OnCollisionEnter(Collision collision)
	{
		Hit = true;
		ContactPoint contactPoint = collision.contacts[0];
		base.Transform.position = contactPoint.point;
		HitReaction();
	}

	internal bool IsTrailVisible()
	{
		return (bool)m_Trail && (m_Trail.isPlaying || m_Trail.particleCount > 0);
	}

	internal void HitReaction()
	{
		Hit = true;
		if (IsTrailVisible())
		{
			m_Trail.Stop();
			DeactivateAllWithoutTrail();
		}
		SpawnExplosion();
	}

	internal void DeactivateAllWithoutTrail()
	{
		DeactivateGameObjects(base.gameObject, m_Trail.gameObject);
	}

	internal void DeactivateGameObjects(GameObject inGameObject, GameObject inIgnore)
	{
		if (inGameObject == inIgnore)
		{
			return;
		}
		inGameObject.active = false;
		foreach (Transform item in inGameObject.transform)
		{
			DeactivateGameObjects(item.gameObject, inIgnore);
		}
	}

	internal void NavigateToTarget(Vector3 inTargetPosition)
	{
		Vector3 lookRotation = inTargetPosition - base.Transform.position;
		Quaternion to = default(Quaternion);
		to.SetLookRotation(lookRotation);
		base.Transform.rotation = Quaternion.RotateTowards(base.Transform.rotation, to, m_AngularSpeed * Time.deltaTime);
	}

	internal Quaternion RotateToward(Quaternion inFrom, Quaternion inTo, float inRotSpeed, float inTime)
	{
		float num = Quaternion.Angle(inFrom, inTo);
		float num2 = num / inRotSpeed;
		float t = ((num2 != 0f) ? Mathf.Clamp(inTime / num2, 0f, 1f) : 0f);
		return Quaternion.Slerp(inFrom, inTo, t);
	}

	internal void SpawnExplosion()
	{
		if (m_Explosion != null)
		{
			Explosion explosion = Mission.Instance.ExplosionCache.Get(m_Explosion, base.transform.position + m_ExplosionOffset, base.transform.rotation);
			explosion.causer = base.Agent;
			explosion.damage = base.Damage;
		}
	}

	internal Agent FindBestTarget(float inMaxAngle)
	{
		if (Mission.Instance.CurrentGameZone == null)
		{
			return null;
		}
		List<Agent> enemies = Mission.Instance.CurrentGameZone.Enemies;
		if (enemies == null || enemies.Count == 0)
		{
			return null;
		}
		Agent result = null;
		float num = float.MaxValue;
		Vector3 position = base.Transform.position;
		Vector3 normalized = base.Transform.forward.normalized;
		foreach (Agent item in enemies)
		{
			if (!item.IsAlive)
			{
				continue;
			}
			Vector3 to = item.ChestPosition - position;
			if (!(Vector3.Angle(normalized, to) > inMaxAngle))
			{
				float num2 = Vector3.Distance(position, item.Position);
				if (num2 < num)
				{
					num = num2;
					result = item;
				}
			}
		}
		return result;
	}
}
