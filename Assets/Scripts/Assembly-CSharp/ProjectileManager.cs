using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[AddComponentMenu("Weapons/ProjectileManager")]
public class ProjectileManager : MonoBehaviour
{
	[Serializable]
	public class SoundInfo
	{
		public AudioClip[] DefaultHits = new AudioClip[0];

		public AudioClip[] MetalHits = new AudioClip[0];

		public AudioClip[] BloodHits = new AudioClip[0];

		public AudioClip[] WaterHits = new AudioClip[0];

		public AudioClip HitDefault
		{
			get
			{
				if (DefaultHits.Length == 0)
				{
					return null;
				}
				return DefaultHits[Random.Range(0, DefaultHits.Length)];
			}
		}

		public AudioClip HitMetal
		{
			get
			{
				if (MetalHits.Length == 0)
				{
					return null;
				}
				return MetalHits[Random.Range(0, MetalHits.Length)];
			}
		}

		public AudioClip HitBlood
		{
			get
			{
				if (BloodHits.Length == 0)
				{
					return null;
				}
				return BloodHits[Random.Range(0, BloodHits.Length)];
			}
		}

		public AudioClip HitWater
		{
			get
			{
				if (WaterHits.Length == 0)
				{
					return null;
				}
				return WaterHits[Random.Range(0, WaterHits.Length)];
			}
		}
	}

	public static ProjectileManager Instance;

	public SoundInfo ProjectileSounds = new SoundInfo();

	public SoundInfo GrenadeSounds = new SoundInfo();

	public SoundInfo PlasmaSounds = new SoundInfo();

	private GameObject Audio;

	private AudioSource AudioSource;

	private Dictionary<E_ProjectileType, ProjectileCacheEx> CacheOfProjectiles = new Dictionary<E_ProjectileType, ProjectileCacheEx>();

	private List<Projectile> ActiveProjectiles = new List<Projectile>();

	private void Awake()
	{
		Instance = this;
		CacheOfProjectiles[E_ProjectileType.SMG] = new ProjectileCacheEx("Weapons/ProjectileSMGWithTrace", E_ProjectileType.SMG, 10);
		CacheOfProjectiles[E_ProjectileType.Grenade] = new ProjectileCacheEx("Weapons/ProjectileGrenade", E_ProjectileType.Grenade, 10);
		CacheOfProjectiles[E_ProjectileType.Shotgun] = new ProjectileCacheEx("Weapons/ProjectileSMG", E_ProjectileType.Shotgun, 20);
		CacheOfProjectiles[E_ProjectileType.Rocket] = new ProjectileCacheEx("Weapons/ProjectileRocket", E_ProjectileType.Rocket, 10);
		CacheOfProjectiles[E_ProjectileType.Plasma] = new ProjectileCacheEx("Weapons/ProjectilePlasma", E_ProjectileType.Plasma, 10);
		CacheOfProjectiles[E_ProjectileType.Turret] = new ProjectileCacheEx("Weapons/ProjectileSMG", E_ProjectileType.Turret, 10);
		CacheOfProjectiles[E_ProjectileType.Flyer] = new ProjectileCacheEx("Weapons/ProjectileFlayer", E_ProjectileType.Flyer, 10);
		CacheOfProjectiles[E_ProjectileType.MongoSMG] = new ProjectileCacheEx("Weapons/MongoProjectileSMG", E_ProjectileType.MongoSMG, 5);
		CacheOfProjectiles[E_ProjectileType.MongoGrenade] = new ProjectileCacheEx("Weapons/MongoProjectileGrenade", E_ProjectileType.MongoGrenade, 4);
		CacheOfProjectiles[E_ProjectileType.MongoRocket] = new ProjectileCacheEx("Weapons/MongoProjectileRocket", E_ProjectileType.MongoRocket, 5);
		CacheOfProjectiles[E_ProjectileType.BigSpiderRocket] = new ProjectileCacheEx("Weapons/BigSpiderProjectileRocket", E_ProjectileType.BigSpiderRocket, 4);
		Audio = new GameObject("ProjectilesAudio", typeof(AudioSource));
		AudioSource = Audio.GetComponent<AudioSource>();
		AudioSource.playOnAwake = false;
		AudioSource.minDistance = 1f;
		AudioSource.maxDistance = 30f;
		AudioSource.rolloffMode = AudioRolloffMode.Linear;
	}

	private void OnDestroy()
	{
		ProjectileSounds = null;
		GrenadeSounds = null;
		PlasmaSounds = null;
		Audio = null;
		AudioSource = null;
		CacheOfProjectiles.Clear();
		ActiveProjectiles.Clear();
	}

	private void Update()
	{
		if (Game.Instance.GameState != E_GameState.Game || Time.deltaTime <= 0f)
		{
			return;
		}
		foreach (Projectile activeProjectile in ActiveProjectiles)
		{
			if (!activeProjectile.IsFinished())
			{
				activeProjectile.ProjectileUpdate();
			}
		}
	}

	private void FixedUpdate()
	{
		for (int i = 0; i < ActiveProjectiles.Count; i++)
		{
			if (ActiveProjectiles[i].IsFinished())
			{
				ReturnProjectile(ActiveProjectiles[i]);
				ActiveProjectiles.RemoveAt(i);
			}
		}
	}

	public void SpawnProjectile(E_ProjectileType inProjeType, Vector3 inPos, Vector3 inDir, Projectile.InitSettings inSettings)
	{
		if (!CacheOfProjectiles.ContainsKey(inProjeType))
		{
			Debug.LogError("ProjectileFactory: unknown type " + inProjeType);
			return;
		}
		if (CacheOfProjectiles[inProjeType] == null)
		{
			Debug.LogError(string.Concat("ProjectileFactory: For this type ", inProjeType, " we don't have resource"));
			return;
		}
		Projectile projectile = CacheOfProjectiles[inProjeType].Get();
		if (projectile == null)
		{
			Debug.LogError("ProjectileFactory: Can't create projectile for type " + inProjeType);
			return;
		}
		projectile.ProjectileInit(inPos, inDir.normalized, inSettings);
		ActiveProjectiles.Add(projectile);
	}

	public void ReturnProjectile(Projectile inProjectile)
	{
		if (inProjectile == null)
		{
			Debug.LogError("ProjectileFactory: sombody is trying return null object to cache");
			return;
		}
		if (!CacheOfProjectiles.ContainsKey(inProjectile.ProjectileType))
		{
			Debug.LogError("ProjectileFactory: unknown type " + inProjectile.ProjectileType);
			return;
		}
		if (CacheOfProjectiles[inProjectile.ProjectileType] == null)
		{
			Debug.LogError("ProjectileFactory: We don't have cache for this projectile type. This object was not created by this manager");
			return;
		}
		inProjectile.ProjectileDeinit();
		CacheOfProjectiles[inProjectile.ProjectileType].Return(inProjectile);
	}

	public void Reset()
	{
		for (int i = 0; i < ActiveProjectiles.Count; i++)
		{
			ActiveProjectiles[i].ProjectileDeinit();
			ReturnProjectile(ActiveProjectiles[i]);
		}
		ActiveProjectiles.Clear();
	}

	public void PlaySound(AudioClip clip, Vector3 pos)
	{
		AudioSource.transform.position = pos;
		AudioSource.PlayOneShot(clip);
	}

	public void PlayHitSound(int layer, Vector3 pos, E_ProjectileType inProjType)
	{
		AudioClip audioClip = null;
		switch (inProjType)
		{
		case E_ProjectileType.Plasma:
			audioClip = PlasmaSounds.HitDefault;
			break;
		case E_ProjectileType.Grenade:
		case E_ProjectileType.MongoGrenade:
			switch (layer)
			{
			case 27:
				audioClip = GrenadeSounds.HitWater;
				break;
			case 28:
				audioClip = GrenadeSounds.HitDefault;
				break;
			case 29:
				audioClip = GrenadeSounds.HitMetal;
				break;
			case 31:
				audioClip = GrenadeSounds.HitBlood;
				break;
			default:
				audioClip = GrenadeSounds.HitDefault;
				break;
			}
			break;
		default:
			switch (layer)
			{
			case 27:
				audioClip = ProjectileSounds.HitWater;
				break;
			case 28:
				audioClip = ProjectileSounds.HitDefault;
				break;
			case 29:
				audioClip = ProjectileSounds.HitMetal;
				break;
			case 31:
				audioClip = ProjectileSounds.HitBlood;
				break;
			default:
				audioClip = ProjectileSounds.HitDefault;
				break;
			}
			break;
		}
		if (audioClip != null)
		{
			PlaySound(audioClip, pos);
		}
	}
}
