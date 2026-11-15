#define DEBUG
using System;
using UnityEngine;

[AddComponentMenu("Weapons/Projectile")]
public class Projectile : MonoBehaviour
{
	[Serializable]
	public class InitSettingsInspector
	{
		public float Damage;

		public float Impuls;

		public float Speed = 10f;
	}

	[Serializable]
	public class InitSettings
	{
		public float Damage;

		public float Impuls;

		public float Speed = 10f;

		public AgentHuman Agent;

		public E_WeaponType Weapon = E_WeaponType.None;

		public Transform IgnoreTransform;

		public InitSettings()
		{
		}

		public InitSettings(InitSettings inSettings)
		{
			Damage = inSettings.Damage;
			Impuls = inSettings.Impuls;
			Speed = inSettings.Speed;
			Agent = inSettings.Agent;
			Weapon = inSettings.Weapon;
			IgnoreTransform = inSettings.IgnoreTransform;
		}

		public InitSettings(InitSettingsInspector inSettings)
		{
			Damage = inSettings.Damage;
			Impuls = inSettings.Impuls;
			Speed = inSettings.Speed;
		}
	}

	protected InitSettings Settings;

	private bool m_ScaleProjectile = true;

	protected float Timer;

	protected bool Hit;

	public ProjectileTrail m_ProjectileTrail;

	public E_ProjectileType ProjectileType { get; set; }

	public GameObject GameObject { get; private set; }

	public Transform Transform { get; private set; }

	public float Damage
	{
		get
		{
			return Settings.Damage;
		}
	}

	public float Speed
	{
		get
		{
			return Settings.Speed;
		}
	}

	public float Impuls
	{
		get
		{
			return Settings.Impuls;
		}
	}

	public AgentHuman Agent
	{
		get
		{
			return Settings.Agent;
		}
	}

	public E_WeaponType Weapon
	{
		get
		{
			return Settings.Weapon;
		}
	}

	public bool ignoreThisHit { get; set; }

	public bool ricochetThisHit { get; set; }

	public void Awake()
	{
		GameObject = base.gameObject;
		Transform = GameObject.transform;
	}

	public virtual void ProjectileInit(Vector3 pos, Vector3 dir, InitSettings inSettings)
	{
		Transform.position = pos;
		Transform.forward = dir;
		Settings = inSettings;
		Timer = 0f;
		Hit = false;
		ignoreThisHit = false;
		ricochetThisHit = false;
		if (m_ScaleProjectile)
		{
			Transform.localScale = new Vector3(1f, 1f, 0f);
		}
		if (m_ProjectileTrail != null)
		{
			m_ProjectileTrail.InitTrail(pos);
			if (Game.Instance.IsXperiaPlay || Game.Instance.IsAdreno205_Graphics)
			{
				m_ProjectileTrail.gameObject.active = !Agent.IsPlayer && Random.Range(0f, 1f) < 0.1f;
			}
			else
			{
				m_ProjectileTrail.gameObject.active = true;
			}
		}
		if (base.renderer != null)
		{
			base.renderer.enabled = true;
		}
	}

	public virtual void ProjectileUpdate()
	{
		DebugUtils.Assert(!IsFinished());
		Timer += Time.deltaTime;
		if (Hit)
		{
			return;
		}
		if (Transform.localScale.z != 1f)
		{
			Transform.localScale = new Vector3(1f, 1f, Mathf.Min(1f, Transform.localScale.z + Time.deltaTime * 8f));
		}
		Vector3 vector = Transform.position + Transform.forward * Settings.Speed * Time.deltaTime;
		RaycastHit[] array = Physics.RaycastAll(Transform.position, Transform.forward, Settings.Speed * Time.deltaTime);
		if (array.Length > 1)
		{
			Array.Sort(array, CollisionUtils.CompareHits);
		}
		RaycastHit[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			RaycastHit raycastHit = array2[i];
			if (raycastHit.transform == Settings.IgnoreTransform)
			{
				continue;
			}
			Transform.position = raycastHit.point;
			raycastHit.transform.SendMessage("OnProjectileHit", this, SendMessageOptions.DontRequireReceiver);
			if (ignoreThisHit)
			{
				ignoreThisHit = false;
			}
			else
			{
				if (raycastHit.collider.isTrigger)
				{
					continue;
				}
				vector = raycastHit.point;
				if (ricochetThisHit)
				{
					ricochetThisHit = false;
					Transform.forward = raycastHit.normal;
					PlayHitSound(29);
					CombatEffectsManager.Instance.PlayHitEffect(raycastHit.transform.gameObject, 29, raycastHit.point, raycastHit.normal, ProjectileType);
					if (m_ProjectileTrail != null)
					{
						m_ProjectileTrail.AddTrailPos(vector);
					}
				}
				else
				{
					Hit = true;
					PlayHitSound(raycastHit.transform.gameObject.layer);
					CombatEffectsManager.Instance.PlayHitEffect(raycastHit.transform.gameObject, raycastHit.transform.gameObject.layer, raycastHit.point, raycastHit.normal, ProjectileType);
				}
				if (base.renderer != null)
				{
					base.renderer.enabled = false;
				}
				break;
			}
		}
		Transform.position = vector;
		if (m_ProjectileTrail != null)
		{
			m_ProjectileTrail.UpdateTrailPos(vector);
		}
	}

	public virtual void ProjectileDeinit()
	{
	}

	public virtual bool IsFinished()
	{
		if (m_ProjectileTrail == null)
		{
			return Timer > 1f || Hit;
		}
		return Timer > 5f || !m_ProjectileTrail.IsVisible();
	}

	protected void PlayHitSound(int layer)
	{
		ProjectileManager.Instance.PlayHitSound(layer, Transform.position, ProjectileType);
	}
}
