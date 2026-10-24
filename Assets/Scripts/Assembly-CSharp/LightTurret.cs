using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[AddComponentMenu("Enemies/LightTurret/Main")]
public class LightTurret : Agent, IHitZoneOwner
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

	public AnimInfo m_AnimInfo = new AnimInfo();

	private Animation m_Animations;

	public SoundInfo m_SoundInfo = new SoundInfo();

	private AudioSource m_Audio;

	private AgentHuman m_Target;

	private LightTurretWeapon m_WeaponComp;

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
			return base.Position;
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
		m_WeaponComp = GetComponent<LightTurretWeapon>();
		if (m_WeaponComp == null)
		{
			Debug.LogWarning("Neccessary script missing - LightTurretWeapon");
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
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void Activate(SpawnPoint spawn)
	{
		if (spawn != null)
		{
			base.transform.position = spawn.Transform.position;
			base.transform.rotation = spawn.Transform.rotation;
		}
		m_HitPoints = m_DefaultHitPoints;
		base.GetComponent<Animation>().playAutomatically = false;
		m_WeaponComp.enabled = false;
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
		yield return 0;
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
}
