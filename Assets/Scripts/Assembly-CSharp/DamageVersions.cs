using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[AddComponentMenu("Interaction/Damage Versions")]
[RequireComponent(typeof(AudioSource))]
public class DamageVersions : MonoBehaviour, IGameZoneControledObject, IHitZoneOwner
{
	[Serializable]
	public class Version
	{
		public List<GameObject> GameObjects = new List<GameObject>();

		public AudioClip Audio;

		public ParticleSystem Effect;

		public float MaxHealth;

		public float Health { get; set; }

		public GameObject GameObject { get; set; }

		public void Activate()
		{
			Health = MaxHealth;
			GameObject = GameObjects[Random.Range(0, GameObjects.Count)];
			GameObject.SetActiveRecursively(true);
		}

		public void DeActivate()
		{
			if (GameObject != null)
			{
				GameObject.SetActiveRecursively(false);
			}
			GameObject = null;
		}

		public void Initialize()
		{
			foreach (GameObject gameObject in GameObjects)
			{
				gameObject.SetActiveRecursively(false);
			}
		}
	}

	public List<Version> Versions = new List<Version>();

	private int CurrentVersion;

	private AudioSource Audio;

	private float Health;

	private void Awake()
	{
		Audio = base.gameObject.GetComponent<AudioSource>();
		GameZone firstComponentUpward = base.gameObject.GetFirstComponentUpward<GameZone>();
		firstComponentUpward.RegisterControllableObject(this);
	}

	private void Start()
	{
		foreach (Version version in Versions)
		{
			version.Initialize();
		}
	}

	public void Enable()
	{
		Versions[CurrentVersion].Activate();
	}

	public void Reset()
	{
		foreach (Version version in Versions)
		{
			version.DeActivate();
		}
		CurrentVersion = 0;
	}

	public void Disable()
	{
		Versions[CurrentVersion].Activate();
	}

	public void OnProjectileHit(Projectile p)
	{
	}

	public void OnProjectileHit(Projectile p, HitZone inHitZone)
	{
		if (CurrentVersion != Versions.Count - 1)
		{
			if (p.Agent != null && !p.Agent.IsPlayer)
			{
				DoDamage(p.Damage * 2f);
			}
			else
			{
				DoDamage(p.Damage);
			}
		}
	}

	public void OnReceiveRangeDamage(float inDamage, HitZone inHitZone)
	{
		if (CurrentVersion != Versions.Count - 1)
		{
			DoDamage(inDamage);
		}
	}

	private void DoDamage(float damage)
	{
		Version version = Versions[CurrentVersion];
		version.Health -= damage;
		if (version.Health <= 0f)
		{
			if ((bool)version.Effect)
			{
				version.Effect.Play();
			}
			if ((bool)version.Audio)
			{
				Audio.PlayOneShot(version.Audio);
			}
			version.DeActivate();
			CurrentVersion++;
			Versions[CurrentVersion].Activate();
		}
	}
}
