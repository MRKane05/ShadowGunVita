using System;
using System.Collections.Generic;
using UnityEngine;

public class CombatEffectsManager : MonoBehaviour
{
	public class CacheData
	{
		public GameObject GameObject;

		public ParticleSystem[] Emitters;

		public Transform Transform;

		~CacheData()
		{
			GameObject = null;
			Emitters = null;
			Transform = null;
		}
	}

	[Serializable]
	public class CombatEffect
	{
		private Queue<CacheData> Cache = new Queue<CacheData>();

		private List<CacheData> InUse = new List<CacheData>();

		public GameObject Prefab;

		private Quaternion Temp = default(Quaternion);

		~CombatEffect()
		{
			Cache.Clear();
			InUse.Clear();
			Prefab = null;
		}

		public void Init(int count)
		{
			if (!(Prefab == null))
			{
				for (int i = 0; i < count; i++)
				{
					CacheData cacheData = new CacheData();
					cacheData.GameObject = UnityEngine.Object.Instantiate(Prefab) as GameObject;
					cacheData.Emitters = cacheData.GameObject.GetComponentsInChildren<ParticleSystem>();
					cacheData.Transform = cacheData.GameObject.transform;
					Cache.Enqueue(cacheData);
					cacheData.GameObject.SetActiveRecursively(false);
				}
			}
		}

		public void Update()
		{
			for (int num = InUse.Count - 1; num >= 0; num--)
			{
				CacheData cacheData = InUse[num];
				bool flag = false;
				for (int i = 0; i < cacheData.Emitters.Length; i++)
				{
					if (cacheData.Emitters[i].isPlaying || cacheData.Emitters[i].particleCount != 0)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					cacheData.Transform.parent = null;
					Cache.Enqueue(InUse[num]);
					InUse.RemoveAt(num);
					cacheData.GameObject.SetActiveRecursively(false);
				}
			}
		}

		public CacheData Get()
		{
			if (Cache.Count == 0)
			{
				Init(2);
			}
			return Cache.Dequeue();
		}

		public void Return(CacheData c)
		{
			InUse.Add(c);
		}

		public void Play(Vector3 pos, Vector3 dir)
		{
			if (Cache.Count == 0)
			{
				Init(2);
			}
			CacheData cacheData = Cache.Dequeue();
			InUse.Add(cacheData);
			cacheData.GameObject.SetActiveRecursively(true);
			cacheData.Transform.position = pos;
			Temp.SetLookRotation(dir);
			cacheData.Transform.rotation = Temp;
			for (int i = 0; i < cacheData.Emitters.Length; i++)
			{
				cacheData.Emitters[i].Play();
			}
		}
	}

	[SerializeField]
	private CombatEffect PlayerHit = new CombatEffect();

	[SerializeField]
	private CombatEffect HumanHit = new CombatEffect();

	[SerializeField]
	private CombatEffect DefaultHit = new CombatEffect();

	[SerializeField]
	private CombatEffect MetalHit = new CombatEffect();

	[SerializeField]
	private CombatEffect WaterHit = new CombatEffect();

	[SerializeField]
	private CombatEffect PlasmaGunHit = new CombatEffect();

	[SerializeField]
	private CombatEffect MongoGunHit = new CombatEffect();

	private Dictionary<int, CombatEffect> HitEffects = new Dictionary<int, CombatEffect>();

	public static CombatEffectsManager Instance;

	private void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError(base.name + " is singleton, somebody is creating another instance !!");
		}
		Instance = this;
		PlayerHit.Init(5);
		HumanHit.Init(10);
		DefaultHit.Init(10);
		MetalHit.Init(10);
		WaterHit.Init(10);
		HitEffects.Add(0, DefaultHit);
		HitEffects.Add(27, WaterHit);
		HitEffects.Add(28, DefaultHit);
		HitEffects.Add(29, MetalHit);
		HitEffects.Add(31, HumanHit);
		PlasmaGunHit.Init(10);
		MongoGunHit.Init(5);
	}

	private void OnDestroy()
	{
		PlayerHit = null;
		HumanHit = null;
		DefaultHit = null;
		MetalHit = null;
		WaterHit = null;
		MongoGunHit = null;
		PlasmaGunHit = null;
		HitEffects.Clear();
	}

	private void LateUpdate()
	{
		PlayerHit.Update();
		HumanHit.Update();
		DefaultHit.Update();
		MetalHit.Update();
		WaterHit.Update();
		MongoGunHit.Update();
		PlasmaGunHit.Update();
	}

	public void PlayHitEffect(GameObject parent, Vector3 pos, Vector3 dir)
	{
		PlayHitEffect(parent, parent.layer, pos, dir, E_ProjectileType.None);
	}

	public void PlayHitEffect(GameObject parent, int layer, Vector3 pos, Vector3 dir, E_ProjectileType inProjectileType)
	{
		Renderer renderer = parent.GetComponent<Renderer>();
		if (renderer != null && !renderer.isVisible)
		{
			return;
		}
		switch (inProjectileType)
		{
		case E_ProjectileType.MongoSMG:
			if (MongoGunHit.Prefab != null)
			{
				MongoGunHit.Play(pos, dir);
				return;
			}
			break;
		case E_ProjectileType.Plasma:
			if (PlasmaGunHit.Prefab != null)
			{
				PlasmaGunHit.Play(pos, dir);
				return;
			}
			break;
		}
		if (parent.GetComponent<ComponentPlayer>() != null && PlayerHit.Prefab != null)
		{
			PlayerHit.Play(pos, dir);
		}
		else if (HitEffects.ContainsKey(layer))
		{
			HitEffects[layer].Play(pos, dir);
		}
		else
		{
			HitEffects[0].Play(pos, dir);
		}
	}
}
