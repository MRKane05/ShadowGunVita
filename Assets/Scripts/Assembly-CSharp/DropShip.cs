using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[AddComponentMenu("Enemies/DropShip/Main")]
public class DropShip : Agent, IBossHealth, IHitZoneOwner
{
	[Serializable]
	public class AnimInfo
	{
		public AnimationClip SpawnAnim;

		public AnimationClip Reload;

		public AnimationClip Idle;

		public AnimationClip MoveLeft;

		public AnimationClip MoveRight;

		public AnimationClip Destroy;
	}

	[Serializable]
	public class SoundInfo
	{
		public AudioClip FlyLoop;

		public AudioClip MoveBurst;

		public AudioClip Reload;

		public AudioClip Destroy;

		public AudioClip RocketLaunch;

		public AudioClip[] MachineGunLaunch;

		public AudioClip machineGunLaunch
		{
			get
			{
				if (MachineGunLaunch.Length == 0)
				{
					return null;
				}
				return MachineGunLaunch[Random.Range(0, MachineGunLaunch.Length)];
			}
		}
	}

	public float m_HitPoints = 1000f;

	private float m_DefaultHitPoints;

	public AnimInfo m_AnimInfo;

	private Animation m_Animations;

	public SoundInfo m_SoundInfo;

	private AudioSource m_Audio;

	private AgentHuman m_Target;

	private DropShipWeapons m_WeaponComp;

	private Transform m_AutoAimTarget;

	public ParticleSystem[] m_DestroyParticles;

	private bool m_Alive;

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
			return m_Alive;
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
		m_WeaponComp = GetComponent<DropShipWeapons>();
		if (m_WeaponComp == null)
		{
			Debug.LogWarning("Neccessary script missing - DropShipWeapons");
		}
		m_Audio = GetComponentInChildren<AudioSource>();
		m_Animations = base.GetComponent<Animation>();
		m_Animations.playAutomatically = false;
		CheckObjectValidity();
		SetupAnimations();
		m_AutoAimTarget = base.transform.Find("mover/dropship_mesh");
	}

	private void Start()
	{
		m_Target = Player.Instance.Owner;
	}

	private void Activate(SpawnPoint spawn)
	{
		m_Alive = true;
		base.transform.position = spawn.Transform.position;
		base.transform.rotation = spawn.Transform.rotation;
		m_HitPoints = m_DefaultHitPoints;
		base.GetComponent<Animation>().playAutomatically = false;
		m_WeaponComp.enabled = false;
		Audio_PlayDefaultFlySound();
		StartCoroutine(UpdateState());
	}

	private void Deactivate()
	{
		m_Alive = false;
		m_Audio.Stop();
		base.GetComponent<Animation>().Stop();
		m_Animations.Stop();
		m_Animations[m_AnimInfo.SpawnAnim.name].time = 0f;
		m_Animations[m_AnimInfo.SpawnAnim.name].enabled = true;
		m_Animations.Sample();
		m_Animations[m_AnimInfo.SpawnAnim.name].enabled = false;
		m_Animations[m_AnimInfo.Destroy.name].time = 0f;
		m_Animations[m_AnimInfo.Destroy.name].enabled = true;
		m_Animations.Sample();
		m_Animations[m_AnimInfo.Destroy.name].enabled = false;
		m_Animations.Stop();
		m_WeaponComp.enabled = false;
		ParticleSystem[] destroyParticles = m_DestroyParticles;
		foreach (ParticleSystem particleSystem in destroyParticles)
		{
			particleSystem.Stop();
			particleSystem.Clear();
			ParticleSystem[] componentsInChildren = particleSystem.GetComponentsInChildren<ParticleSystem>();
			ParticleSystem[] array = componentsInChildren;
			foreach (ParticleSystem particleSystem2 in array)
			{
				particleSystem2.Stop();
				particleSystem2.Clear();
			}
		}
		m_HitPoints = 0f;
		StopAllCoroutines();
		CancelInvoke();
	}

	private void Update()
	{
	}

	private Vector3 GetAutoAimTarget()
	{
		return m_AutoAimTarget.position;
	}

	public void OnProjectileHit(Projectile inProjectile, HitZone inHitZone)
	{
		if (IsAlive && base.enabled && base.gameObject.active)
		{
			float num = inProjectile.Damage * inHitZone.DamageModifier;
			m_HitPoints -= num;
			if (m_HitPoints <= 0f)
			{
				StopAllCoroutines();
				CancelInvoke();
				StartCoroutine(Destroy());
			}
		}
	}

	public void OnReceiveRangeDamage(float inDamage, HitZone inHitZone)
	{
		if (IsAlive && base.enabled && base.gameObject.active)
		{
			m_HitPoints -= inDamage * inHitZone.DamageModifier;
			if (m_HitPoints <= 0f)
			{
				StopAllCoroutines();
				CancelInvoke();
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

	private IEnumerator UpdateState()
	{
		yield return StartCoroutine(Spawn());
		yield return StartCoroutine(FightPhase());
		yield return StartCoroutine(Destroy());
	}

	private IEnumerator Spawn()
	{
		yield return StartCoroutine(PlaySpawnAnim());
		Audio_PlayReloadSound();
		StartCoroutine(PlayReloadAnim());
		yield return StartCoroutine(PlayIdleAnim());
	}

	private IEnumerator FightPhase()
	{
		m_WeaponComp.enabled = true;
		while (m_HitPoints > 0f)
		{
			if (!m_Target.IsAlive)
			{
				yield return new WaitForSeconds(0.5f);
				continue;
			}
			StartCoroutine(LaunchRockets_Corutine());
			int idx = Random.Range(0, 2);
			Audio_PlayMoveBurstSound();
			Invoke("Audio_PlayMoveBurstSound", 8f);
			DropShip dropShip = this;
			IEnumerator routine;
			if (idx == 0)
			{
				IEnumerator enumerator = PlayMoveLeftAnim();
				routine = enumerator;
			}
			else
			{
				routine = PlayMoveRightAnim();
			}
			yield return dropShip.StartCoroutine(routine);
			yield return StartCoroutine(ReloadPhase());
		}
	}

	private IEnumerator ReloadPhase()
	{
		Audio_PlayReloadSound();
		StartCoroutine(PlayReloadAnim());
		m_WeaponComp.ReloadRockets2();
		yield return StartCoroutine(PlayIdleAnim());
		m_WeaponComp.FireGun();
		yield return StartCoroutine(PlayIdleAnim());
		yield return StartCoroutine(PlayIdleAnim());
		m_WeaponComp.FireGun();
		yield return StartCoroutine(PlayIdleAnim());
		yield return StartCoroutine(PlayIdleAnim());
	}

	private IEnumerator Destroy()
	{
		m_WeaponComp.enabled = false;
		Audio_PlayDestroySound();
		m_Animations.CrossFade(m_AnimInfo.Destroy.name, 2f, PlayMode.StopAll);
		yield return new WaitForSeconds(m_AnimInfo.Destroy.length - 0.3f);
		m_Alive = false;
		Mission.Instance.ReturnAgent(base.gameObject);
	}

	private IEnumerator LaunchRockets_Corutine()
	{
		yield return new WaitForSeconds(2f);
		m_WeaponComp.FireRockets2();
	}

	private bool CheckObjectValidity()
	{
		if (m_Animations == null)
		{
			Debug.LogError("DropShip : Missing animation component !!!");
			return false;
		}
		if (m_AnimInfo.SpawnAnim == null || m_Animations[m_AnimInfo.SpawnAnim.name] == null)
		{
			Debug.LogError("DropShip : Missing AnimationClip 'SpawnAnim' !!!");
			return false;
		}
		if (m_AnimInfo.Reload == null || m_Animations[m_AnimInfo.Reload.name] == null)
		{
			Debug.LogError("DropShip : Missing AnimationClip 'Reload' !!!");
			return false;
		}
		if (m_AnimInfo.Destroy == null || m_Animations[m_AnimInfo.Destroy.name] == null)
		{
			Debug.LogError("DropShip : Missing AnimationClip 'Destroy' !!!");
			return false;
		}
		if (m_AnimInfo.Idle == null || m_Animations[m_AnimInfo.Idle.name] == null)
		{
			Debug.LogError("DropShip : Missing AnimationClip 'Idle' !!!");
			return false;
		}
		if (m_AnimInfo.MoveLeft == null || m_Animations[m_AnimInfo.MoveLeft.name] == null)
		{
			Debug.LogError("DropShip : Missing AnimationClip 'MoveLeft' !!!");
			return false;
		}
		if (m_AnimInfo.MoveRight == null || m_Animations[m_AnimInfo.MoveRight.name] == null)
		{
			Debug.LogError("DropShip : Missing AnimationClip 'MoveRight' !!!");
			return false;
		}
		return true;
	}

	private void SetupAnimations()
	{
		string text = m_AnimInfo.Idle.name;
		m_Animations[text].layer = 0;
		m_Animations[text].wrapMode = WrapMode.Loop;
		text = m_AnimInfo.SpawnAnim.name;
		m_Animations[text].layer = 1;
		text = m_AnimInfo.MoveLeft.name;
		m_Animations[text].layer = 1;
		text = m_AnimInfo.MoveRight.name;
		m_Animations[text].layer = 1;
		text = m_AnimInfo.Destroy.name;
		m_Animations[text].layer = 1;
		text = m_AnimInfo.Reload.name;
		m_Animations[text].layer = 2;
		m_Animations[text].wrapMode = WrapMode.ClampForever;
		m_Animations[text].weight = 0.2f;
	}

	private IEnumerator PlaySpawnAnim()
	{
		string animName = m_AnimInfo.SpawnAnim.name;
		m_Animations.CrossFadeQueued(animName, 0.3f, QueueMode.PlayNow);
		yield return new WaitForSeconds(m_Animations[animName].length - 0.3f);
	}

	private IEnumerator PlayIdleAnim()
	{
		string animName = m_AnimInfo.Idle.name;
		m_Animations.Play(m_AnimInfo.Idle.name);
		yield return new WaitForSeconds(m_Animations[animName].length - 0.3f);
	}

	private IEnumerator PlayReloadAnim()
	{
		string animName = m_AnimInfo.Reload.name;
		m_Animations.Rewind(animName);
		m_Animations.Play(animName);
		yield return new WaitForSeconds(m_Animations[animName].length - 0.3f);
	}

	private IEnumerator PlayMoveLeftAnim()
	{
		string animName = m_AnimInfo.MoveLeft.name;
		m_Animations.CrossFadeQueued(animName, 0.3f, QueueMode.PlayNow);
		yield return new WaitForSeconds(m_Animations[animName].length - 0.3f);
	}

	private IEnumerator PlayMoveRightAnim()
	{
		string animName = m_AnimInfo.MoveRight.name;
		m_Animations.CrossFadeQueued(animName, 0.3f, QueueMode.PlayNow);
		yield return new WaitForSeconds(m_Animations[animName].length - 0.3f);
	}

	private IEnumerator PlayDestroyAnim()
	{
		string animName = m_AnimInfo.Destroy.name;
		m_Animations.CrossFadeQueued(animName, 0.3f, QueueMode.PlayNow);
		yield return new WaitForSeconds(m_Animations[animName].length - 0.3f);
	}

	public void Audio_PlayGunShotSound()
	{
		m_Audio.PlayOneShot(m_SoundInfo.machineGunLaunch);
	}

	public void Audio_PlayRocketLaunchSound()
	{
		m_Audio.PlayOneShot(m_SoundInfo.RocketLaunch);
	}

	private void Audio_PlayDestroySound()
	{
		if ((bool)m_Audio && (bool)m_SoundInfo.Destroy)
		{
			m_Audio.Stop();
			m_Audio.clip = m_SoundInfo.Destroy;
			m_Audio.loop = false;
			m_Audio.Play();
		}
	}

	private void Audio_PlayDefaultFlySound()
	{
		if ((bool)m_Audio && (bool)m_SoundInfo.FlyLoop)
		{
			m_Audio.Stop();
			m_Audio.clip = m_SoundInfo.FlyLoop;
			m_Audio.loop = true;
			m_Audio.Play();
		}
	}

	private void Audio_PlayMoveBurstSound()
	{
		m_Audio.PlayOneShot(m_SoundInfo.MoveBurst);
	}

	private void Audio_PlayReloadSound()
	{
		if ((bool)m_Audio && (bool)m_SoundInfo.Reload)
		{
			m_Audio.PlayOneShot(m_SoundInfo.Reload);
		}
	}

	private IEnumerator TestAnims()
	{
		yield return new WaitForSeconds(5f);
		yield return StartCoroutine(PlaySpawnAnim());
		yield return StartCoroutine(PlayIdleAnim());
		yield return StartCoroutine(PlayMoveLeftAnim());
		yield return StartCoroutine(PlayIdleAnim());
		yield return StartCoroutine(PlayMoveRightAnim());
		yield return StartCoroutine(PlayIdleAnim());
		yield return StartCoroutine(PlayIdleAnim());
		yield return StartCoroutine(PlayMoveLeftAnim());
		yield return StartCoroutine(PlayIdleAnim());
		yield return StartCoroutine(PlayMoveRightAnim());
		yield return StartCoroutine(PlayIdleAnim());
		yield return StartCoroutine(PlayIdleAnim());
		yield return StartCoroutine(PlayDestroyAnim());
	}

	private IEnumerator TestAnims2()
	{
		yield return new WaitForSeconds(2f);
		yield return StartCoroutine(PlaySpawnAnim());
		yield return StartCoroutine(PlayIdleAnim());
		StartCoroutine(LaunchRockets_Corutine());
		yield return StartCoroutine(PlayMoveLeftAnim());
		m_WeaponComp.ReloadRockets2();
		yield return StartCoroutine(PlayIdleAnim());
		StartCoroutine(LaunchRockets_Corutine());
		yield return StartCoroutine(PlayMoveRightAnim());
		m_WeaponComp.ReloadRockets2();
		yield return StartCoroutine(PlayIdleAnim());
		yield return StartCoroutine(PlayIdleAnim());
		StartCoroutine(LaunchRockets_Corutine());
		yield return StartCoroutine(PlayMoveLeftAnim());
		m_WeaponComp.ReloadRockets2();
		yield return StartCoroutine(PlayIdleAnim());
		StartCoroutine(LaunchRockets_Corutine());
		yield return StartCoroutine(PlayMoveRightAnim());
		m_WeaponComp.ReloadRockets2();
		yield return StartCoroutine(PlayIdleAnim());
		yield return StartCoroutine(PlayIdleAnim());
		yield return StartCoroutine(PlayDestroyAnim());
	}

	private IEnumerator TestRockets()
	{
		yield return new WaitForSeconds(5f);
		yield return StartCoroutine(PlaySpawnAnim());
		yield return StartCoroutine(PlayIdleAnim());
		while (true)
		{
			m_WeaponComp.FireRockets2();
			for (int i = 0; i < 5; i++)
			{
				yield return StartCoroutine(PlayIdleAnim());
			}
			m_WeaponComp.ReloadRockets2();
			for (int j = 0; j < 2; j++)
			{
				yield return StartCoroutine(PlayIdleAnim());
			}
		}
	}
}
