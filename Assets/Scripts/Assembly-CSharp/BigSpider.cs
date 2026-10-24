using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[AddComponentMenu("Enemies/BigSpider/Main")]
public class BigSpider : Agent, IBossHealth, IHitZoneOwner
{
	[Serializable]
	public class AnimInfo
	{
		[Serializable]
		public class MoveAnimInfo
		{
			public AnimationClip Left;

			public AnimationClip Right;

			public AnimationClip Idle;
		}

		public AnimationClip SpawnAnim;

		public AnimationClip Reload;

		public AnimationClip Destroy;

		public MoveAnimInfo MovePhase1;

		public MoveAnimInfo MovePhase2;
	}

	[Serializable]
	public class SoundInfo
	{
		public AudioClip JumpDown;

		public AudioClip Reload;

		public AudioClip Step;

		public AudioClip Shot;

		public AudioClip Death;
	}

	public float m_HitPoints = 1000f;

	private float m_DefaultHitPoints;

	public float m_2ndPhaseHitPoints = 500f;

	private Transform m_AutoAimTarget;

	public AnimInfo m_AnimInfo = new AnimInfo();

	private Animation m_Animations;

	public SoundInfo m_SoundInfo = new SoundInfo();

	private AudioSource m_Audio;

	public ParticleSystem[] m_DestroyParticles;

	private AgentHuman m_Target;

	private BigSpiderWeapon m_WeaponComp;

	public Material m_FadeoutMaterial;

	public float m_FadeoutDelay = 2f;

	private float m_FadeoutFXDuration = 3f;

	private Dictionary<MeshRenderer, Material> m_BackupDefaultMaterials;

	public override bool IsPlayer
	{
		get
		{
			return false;
		}
	}

	public override bool IsAlive
	{
		get
		{
			return m_HitPoints > 0f;
		}
	}

	public override bool IsVisible
	{
		get
		{
			return true;
		}
	}

	public override bool IsInvulnerable
	{
		get
		{
			return false;
		}
	}

	public override bool IsInCover
	{
		get
		{
			return false;
		}
	}

	public override Vector3 ChestPosition
	{
		get
		{
			return GetAutoAimTarget();
		}
	}

	public override bool IsEnteringToCover
	{
		get
		{
			return false;
		}
	}

	public override float HealthPercent
	{
		get
		{
			return m_HitPoints / m_DefaultHitPoints;
		}
	}

	private void Awake()
	{
		Initialize();
		m_DefaultHitPoints = m_HitPoints;
		m_WeaponComp = GetComponent<BigSpiderWeapon>();
		if (m_WeaponComp == null)
		{
			Debug.LogWarning("Neccessary script missing - BigSpiderWeapon");
		}
		m_Audio = base.GetComponent<AudioSource>();
		m_Animations = base.GetComponent<Animation>();
		m_Animations.playAutomatically = false;
		if (m_Animations[m_AnimInfo.Reload.name] != null)
		{
			m_Animations[m_AnimInfo.Reload.name].layer = 1;
			Transform mix = base.transform.Find("mover/body/head");
			m_Animations[m_AnimInfo.Reload.name].AddMixingTransform(mix);
		}
		if (m_Animations[m_AnimInfo.Destroy.name] != null)
		{
			m_Animations[m_AnimInfo.Destroy.name].layer = 2;
			Transform mix2 = base.transform.Find("mover/body");
			m_Animations[m_AnimInfo.Destroy.name].AddMixingTransform(mix2);
		}
		m_AutoAimTarget = base.transform.Find("mover/body/head/rockets");
		MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
		if (componentsInChildren.Length > 0)
		{
			m_BackupDefaultMaterials = new Dictionary<MeshRenderer, Material>();
			MeshRenderer[] array = componentsInChildren;
			foreach (MeshRenderer meshRenderer in array)
			{
				m_BackupDefaultMaterials[meshRenderer] = meshRenderer.material;
			}
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnDestroy()
	{
		m_BackupDefaultMaterials.Clear();
	}

	private void Activate(SpawnPoint spawn)
	{
		base.transform.position = spawn.Transform.position;
		base.transform.rotation = spawn.Transform.rotation;
		m_Animations.Stop();
		m_Animations[m_AnimInfo.Reload.name].time = 0f;
		m_Animations[m_AnimInfo.Reload.name].enabled = true;
		m_Animations.Sample();
		m_Animations[m_AnimInfo.Reload.name].enabled = false;
		m_HitPoints = m_DefaultHitPoints;
		base.GetComponent<Animation>().playAutomatically = false;
		m_WeaponComp.enabled = false;
		Transform transform = base.transform.Find("mover/body/head/rockets");
		transform.localScale = Vector3.one;
		StartCoroutine(UpdateState());
	}

	private void Deactivate()
	{
		base.GetComponent<AudioSource>().Stop();
		base.GetComponent<Animation>().Stop();
		base.GetComponent<Animation>().playAutomatically = false;
		m_WeaponComp.enabled = false;
		m_HitPoints = 0f;
		StopAllCoroutines();
		CancelInvoke();
		RestoreMaterials();
	}

	public void Reload()
	{
		if (m_AnimInfo.Reload != null && m_Animations[m_AnimInfo.Reload.name] != null && !IsReloading())
		{
			m_Animations.Play(m_AnimInfo.Reload.name);
		}
		if ((bool)m_Audio && (bool)m_SoundInfo.Reload)
		{
			m_Audio.PlayOneShot(m_SoundInfo.Reload);
		}
	}

	public bool IsReloading()
	{
		return m_AnimInfo.Reload != null && m_Animations.IsPlaying(m_AnimInfo.Reload.name);
	}

	private Vector3 GetAutoAimTarget()
	{
		if (IsReloading() && m_AutoAimTarget != null)
		{
			return m_AutoAimTarget.position;
		}
		return base.Transform.position + Vector3.up * 3f;
	}

	public void PlayShotSound()
	{
		if ((bool)m_Audio && (bool)m_SoundInfo.Shot)
		{
			m_Audio.PlayOneShot(m_SoundInfo.Shot);
		}
	}

	public void PlayStepSound()
	{
		if ((bool)m_Audio && (bool)m_SoundInfo.Step)
		{
			m_Audio.PlayOneShot(m_SoundInfo.Step);
		}
	}

	private IEnumerator UpdateState()
	{
		yield return StartCoroutine(Spawn());
		yield return StartCoroutine(MovePhase1());
		yield return StartCoroutine(MovePhase2());
		yield return StartCoroutine(Destroy());
	}

	private IEnumerator Spawn()
	{
		if (m_AnimInfo.SpawnAnim != null && m_Animations[m_AnimInfo.SpawnAnim.name] != null)
		{
			m_Animations.Play(m_AnimInfo.SpawnAnim.name);
			if ((bool)m_Audio && (bool)m_SoundInfo.JumpDown)
			{
				m_Audio.Stop();
				m_Audio.clip = m_SoundInfo.JumpDown;
				m_Audio.Play();
			}
			yield return new WaitForSeconds(m_AnimInfo.SpawnAnim.length - 0.3f);
		}
	}

	private IEnumerator MovePhase1()
	{
		m_WeaponComp.enabled = true;
		AnimationClip[] anims = new AnimationClip[3]
		{
			m_AnimInfo.MovePhase1.Left,
			m_AnimInfo.MovePhase1.Right,
			m_AnimInfo.MovePhase1.Idle
		};
		while (m_HitPoints > m_2ndPhaseHitPoints)
		{
			int idx = Random.Range(0, 3);
			if (anims[idx] != null && m_Animations[anims[idx].name] != null)
			{
				m_Animations.CrossFadeQueued(anims[idx].name, 0.3f, QueueMode.PlayNow);
				yield return new WaitForSeconds(anims[idx].length - 0.3f);
			}
			else
			{
				yield return 0;
			}
		}
	}

	private IEnumerator MovePhase2()
	{
		m_WeaponComp.enabled = true;
		AnimationClip[] anims = new AnimationClip[3]
		{
			m_AnimInfo.MovePhase2.Left,
			m_AnimInfo.MovePhase2.Right,
			m_AnimInfo.MovePhase2.Idle
		};
		while (m_HitPoints > 0f)
		{
			int idx = Random.Range(0, 3);
			if (anims[idx] != null && m_Animations[anims[idx].name] != null)
			{
				m_Animations.CrossFadeQueued(anims[idx].name, 0.3f, QueueMode.PlayNow);
				yield return new WaitForSeconds(anims[idx].length - 0.3f);
			}
			else
			{
				yield return 0;
			}
		}
	}

	private IEnumerator Destroy()
	{
		m_WeaponComp.enabled = false;
		foreach (AnimationState state in m_Animations)
		{
			state.enabled = false;
		}
		if (m_AnimInfo.Destroy != null && m_Animations[m_AnimInfo.Destroy.name] != null)
		{
			m_Animations.Blend(m_AnimInfo.Destroy.name);
			if ((bool)m_Audio && (bool)m_SoundInfo.Death)
			{
				m_Audio.Stop();
				m_Audio.clip = m_SoundInfo.Death;
				m_Audio.Play();
			}
			yield return new WaitForSeconds(m_AnimInfo.Destroy.length - 0.3f);
		}
		yield return StartCoroutine(Fadeout(m_FadeoutDelay));
	}

	protected IEnumerator Fadeout(float delay)
	{
		yield return new WaitForSeconds(delay);
		if (m_FadeoutMaterial == null)
		{
			Mission.Instance.ReturnAgent(base.GameObject);
			yield break;
		}
		foreach (MeshRenderer renderer in m_BackupDefaultMaterials.Keys)
		{
			renderer.material = m_FadeoutMaterial;
			renderer.material.SetFloat("_TimeOffs", 0f - Time.time);
			renderer.material.SetFloat("_Invert", 0f);
			renderer.material.SetFloat("_Duration", m_FadeoutFXDuration);
		}
		yield return new WaitForSeconds(m_FadeoutFXDuration);
		Mission.Instance.ReturnAgent(base.GameObject);
	}

	private void RestoreMaterials()
	{
		foreach (MeshRenderer key in m_BackupDefaultMaterials.Keys)
		{
			key.material = m_BackupDefaultMaterials[key];
		}
	}

	public void OnProjectileHit(Projectile inProjectile, HitZone inHitZone)
	{
		if (IsAlive && base.enabled && base.gameObject.active)
		{
			float num = inProjectile.Damage * inHitZone.DamageModifier;
			m_HitPoints -= num;
			if (m_HitPoints < 0f)
			{
				StopAllCoroutines();
				StartCoroutine(Destroy());
			}
		}
	}

	public void OnReceiveRangeDamage(float inDamage, HitZone inHitZone)
	{
		if (IsAlive && base.enabled && base.gameObject.active)
		{
			float num = inDamage * inHitZone.DamageModifier;
			m_HitPoints -= num;
			if (m_HitPoints < 0f)
			{
				StopAllCoroutines();
				StartCoroutine(Destroy());
			}
		}
	}

	public void AnimNotify_PlayDestroyParticle(int inParticleIndex)
	{
		if (inParticleIndex < m_DestroyParticles.Length && m_DestroyParticles[inParticleIndex] != null)
		{
			ParticleSystem[] componentsInChildren = m_DestroyParticles[inParticleIndex].GetComponentsInChildren<ParticleSystem>();
			ParticleSystem[] array = componentsInChildren;
			foreach (ParticleSystem particleSystem in array)
			{
				particleSystem.Play();
			}
		}
		else
		{
			Debug.LogError("AnimNotify_PlayDestroyParticle : Destroy particle for index " + inParticleIndex + " is not defined !!");
		}
	}
}
