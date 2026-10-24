using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animation))]
public class Turret : Agent, IHitZoneOwner
{
	[Serializable]
	public class TurretAnims
	{
		public AnimationClip Hidden;

		public AnimationClip Spawn;

		public AnimationClip Activate;

		public AnimationClip Destroy;

		public AnimationClip Observing;

		public AnimationClip Arming;

		public AnimationClip Waiting;

		public AnimationClip Dearming;

		public AnimationClip Shooting;
	}

	[Serializable]
	public class SoundsSettings
	{
		public AudioClip Spawn;

		public AudioClip Activated;

		public AudioClip TargetDetected;

		public AudioClip TargetLost;

		public AudioClip Observing;

		public AudioClip ShieldLost;

		public AudioClip Destroyed;

		public AudioClip ShootingBegin;

		public AudioClip[] Fire = new AudioClip[0];

		public AudioClip[] Taunt = new AudioClip[0];
	}

	[Serializable]
	public class BodyParts
	{
		public Transform Neck;

		public Transform Head;

		public Transform Eye;

		public Armor Armor;

		public GameObject ActiveLight;

		public GameObject AlarmedLight;
	}

	[Serializable]
	public class Activation
	{
		public Transform AlarmSensor;

		public float detectRange = 80f;
	}

	[Serializable]
	public class Gun
	{
		public Transform shoot;

		public GameObject muzzle;
	}

	[Serializable]
	public class ShootSettings
	{
		public int bulletsInBurst = 40;

		public float fireRate = 0.1f;

		public float afterBurstPauseMin = 1.5f;

		public float afterBurstPauseMax = 3.5f;

		public float projectileSpeed = 100f;

		public float rndSpeedVariation = 0.1f;

		public float projectileDamage = 5f;

		public float dispersion = 0.05f;

		public float rndMuzzleScale = 0.1f;
	}

	public enum E_State
	{
		Hidden = 0,
		Activating = 1,
		Observing = 2,
		Targeting = 3,
		Arming = 4,
		Waiting = 5,
		Shooting = 6,
		Dearming = 7,
		Destroyed = 8
	}

	private enum E_AlarmLight
	{
		Off = 0,
		Active = 1,
		Alarmed = 2
	}

	private class Runtime
	{
		public int bulletCounter;

		public float nextTauntTime;

		public float shootWarmupTime;

		public bool shootEffectsOn;

		public float shootPauseTime;

		public float reloadingTime;

		public float damageShockTime;

		public bool shootingInProgress;

		public void Reset()
		{
			bulletCounter = 0;
			nextTauntTime = 0f;
			shootWarmupTime = 0f;
			shootEffectsOn = false;
			shootPauseTime = 0f;
			reloadingTime = 0f;
			damageShockTime = 0f;
			shootingInProgress = false;
		}
	}

	private class Target
	{
		public AgentHuman agent;

		public bool inRange;

		public bool isVisible;

		public bool inFOV;

		public bool isAquired;

		public float angleToTarget;

		public Vector3 lastVisiblePos;

		public float lastVisibleTime;

		public Vector3 AimPos
		{
			get
			{
				return agent.TransformTarget.position;
			}
		}

		public bool WithinSensor
		{
			get
			{
				return inRange && isVisible;
			}
		}

		public bool WithinEye
		{
			get
			{
				return inRange && isVisible && inFOV;
			}
		}

		public void Reset()
		{
			inRange = false;
			isVisible = false;
			inFOV = false;
			isAquired = false;
			lastVisibleTime = 0f;
		}
	}

	public TurretAnims anims;

	public SoundsSettings sounds;

	public BodyParts bodyParts;

	public Activation activation;

	public Gun leftGun;

	public Gun rightGun;

	public float shootingRange = 80f;

	public float rotationSpeed = 180f;

	public float FieldOfView = 60f;

	public float targetLostTimeout = 10f;

	public float minTauntDelay = 10f;

	public ShootSettings shootSetup;

	public float hitPoints = 200f;

	private float origHitPoints;

	public Explosion m_Explosion;

	public BoxCollider boxCollider;

	private E_State State;

	private bool friendly;

	private Quaternion origNeckLocalRotation;

	private Animation anim;

	protected MeshRenderer m_ShieldFXMeshRenderer;

	protected float m_ShieldFXLastPlayTime = -1f;

	protected float m_ShieldFXDuration = 0.5f;

	public bool m_EnableShieldFX;

	private float targetToleranceAngle = 5f;

	private float eyeFrequency = 0.5f;

	private Runtime runtime = new Runtime();

	private int numGuns;

	private Gun[] guns = new Gun[2];

	private Target target = new Target();

	private Quaternion NeckRotation
	{
		get
		{
			return bodyParts.Neck.rotation;
		}
		set
		{
			bodyParts.Neck.rotation = value;
		}
	}

	private Quaternion EyeRotation
	{
		get
		{
			return bodyParts.Eye.rotation;
		}
		set
		{
			bodyParts.Eye.rotation = value;
		}
	}

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
			return hitPoints > 0f;
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

	public override bool IsEnteringToCover
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
			return boxCollider.center;
		}
	}

	private void Awake()
	{
		Init();
	}

	private void OnDestroy()
	{
		m_Explosion = null;
	}

	private void Init()
	{
		Initialize();
		anim = GetComponent<Animation>();
		anim.Play(anims.Hidden.name);
		AudioSource component = GetComponent<AudioSource>();
		component.playOnAwake = false;
		if (leftGun != null)
		{
			guns[numGuns++] = leftGun;
		}
		if (rightGun != null)
		{
			guns[numGuns++] = rightGun;
		}
		bodyParts.Armor.OnHit = OnArmorHit;
		bodyParts.Armor.OnDestroy = OnArmorDestroy;
		origHitPoints = hitPoints;
		origNeckLocalRotation = bodyParts.Neck.localRotation;
		Transform transform = base.transform.Find("/turret/tur_legs/tur_neck/tur_head/hitbox");
		if ((bool)transform)
		{
			m_ShieldFXMeshRenderer = (MeshRenderer)transform.GetComponent(typeof(MeshRenderer));
		}
		if (m_EnableShieldFX && !m_ShieldFXMeshRenderer)
		{
			Debug.LogWarning("Turrent has no 'hitbox' child");
		}
		if (!boxCollider)
		{
			Debug.LogError("boxCollider is not set on Turret '" + base.name + "'");
		}
		m_ShieldFXLastPlayTime = -1f;
	}

	public void Activate(SpawnPoint spawn)
	{
		base.Transform.position = spawn.Transform.position;
		base.Transform.rotation = spawn.Transform.rotation;
		State = E_State.Hidden;
		hitPoints = origHitPoints;
		m_ShieldFXLastPlayTime = -1f;
		runtime.Reset();
		target.Reset();
		bodyParts.Armor.Reset();
		HideMuzzles();
		SetAlarmLight(E_AlarmLight.Off);
		SelectTarget();
		bodyParts.Armor.Enable();
		StartCoroutine("Spawn_Coroutine");
	}

	private IEnumerator Spawn_Coroutine()
	{
		if ((bool)anims.Spawn)
		{
			anim.Play(anims.Spawn.name);
			base.GetComponent<AudioSource>().PlayOneShot(sounds.Spawn);
			yield return new WaitForSeconds(anims.Spawn.length);
		}
		StartCoroutine("DetectTarget");
	}

	public void Deactivate()
	{
		StopAllCoroutines();
		CancelInvoke();
		anim.Stop();
		bodyParts.Neck.localRotation = origNeckLocalRotation;
		anim.Play(anims.Hidden.name);
		bodyParts.Armor.Disable();
	}

	private void SetAlarmLight(E_AlarmLight light)
	{
		switch (light)
		{
		case E_AlarmLight.Off:
			bodyParts.ActiveLight.active = false;
			bodyParts.AlarmedLight.active = false;
			break;
		case E_AlarmLight.Active:
			bodyParts.ActiveLight.active = true;
			bodyParts.AlarmedLight.active = false;
			break;
		case E_AlarmLight.Alarmed:
			bodyParts.ActiveLight.active = false;
			bodyParts.AlarmedLight.active = true;
			break;
		}
	}

	private void SelectTarget()
	{
		if (!friendly)
		{
			target.agent = Player.Instance.Owner;
		}
		if (target.agent == null)
		{
			Debug.LogError("Turret '" + base.name + "' has not found target!");
		}
	}

	private void Update()
	{
		if (Game.Instance.GameState == E_GameState.Game)
		{
			switch (State)
			{
			case E_State.Destroyed:
				break;
			case E_State.Hidden:
				UpadateHidden();
				break;
			case E_State.Activating:
				break;
			case E_State.Observing:
				UpdateObserving();
				break;
			case E_State.Targeting:
				UpdateTargeting();
				break;
			case E_State.Arming:
				UpdateAiming();
				break;
			case E_State.Dearming:
				break;
			case E_State.Shooting:
				UpdateShooting();
				break;
			case E_State.Waiting:
				UpdateWaiting();
				break;
			}
		}
	}

	private void UpadateHidden()
	{
		if (target.WithinSensor)
		{
			StartCoroutine("Activate_Coroutine");
		}
	}

	private void UpdateTaunt()
	{
		if (Time.time > target.lastVisibleTime + targetLostTimeout * 0.5f && Time.time > runtime.nextTauntTime)
		{
			if (sounds.Taunt.Length > 0)
			{
				AudioClip clip = sounds.Taunt[Random.Range(0, sounds.Taunt.Length)];
				base.GetComponent<AudioSource>().PlayOneShot(clip);
			}
			runtime.nextTauntTime = Time.time + minTauntDelay;
		}
	}

	private bool TargetLost()
	{
		return !target.isVisible && Time.time > target.lastVisibleTime + targetLostTimeout;
	}

	private void UpdateAiming()
	{
		if (!(Time.deltaTime <= float.Epsilon))
		{
			Vector3 vector = ((!target.isVisible) ? target.lastVisiblePos : target.AimPos);
			Vector3 view = vector - bodyParts.Eye.position;
			view.Normalize();
			Quaternion quaternion = default(Quaternion);
			quaternion.SetLookRotation(view, bodyParts.Neck.up);
			NeckRotation = Quaternion.RotateTowards(NeckRotation, quaternion, rotationSpeed * Time.deltaTime);
			target.angleToTarget = Quaternion.Angle(EyeRotation, quaternion);
			target.isAquired = target.angleToTarget < targetToleranceAngle;
			target.lastVisiblePos = vector;
		}
	}

	private void StartTargeting()
	{
		anim.Stop();
		NeckRotation = bodyParts.Head.rotation;
		bodyParts.Head.localRotation = Quaternion.identity;
		State = E_State.Targeting;
		SetAlarmLight(E_AlarmLight.Alarmed);
		StopObserveSound();
		base.GetComponent<AudioSource>().PlayOneShot(sounds.TargetDetected);
	}

	private void UpdateTargeting()
	{
		UpdateAiming();
		if (target.isAquired)
		{
			StartCoroutine("Arming");
		}
	}

	private void UpdateShooting()
	{
		UpdateAiming();
		if (!target.isAquired && !runtime.shootingInProgress)
		{
			return;
		}
		if (!target.isVisible && !runtime.shootingInProgress)
		{
			StartWaiting();
		}
		else
		{
			if (runtime.reloadingTime > Time.time || runtime.shootPauseTime > Time.time || runtime.damageShockTime > Time.time)
			{
				return;
			}
			if (!runtime.shootingInProgress)
			{
				StartShooting();
			}
			if (Time.time > runtime.shootWarmupTime)
			{
				if (!runtime.shootEffectsOn)
				{
					StartShootingEffects();
				}
				Fire(target.lastVisiblePos);
				ShootPause();
				if (runtime.bulletCounter >= shootSetup.bulletsInBurst)
				{
					EndShooting();
					Reload();
				}
			}
		}
	}

	private void StartObserve()
	{
		if (State != E_State.Destroyed)
		{
			anim.Stop();
			anim.Play(anims.Observing.name);
			StartObserveSound();
			State = E_State.Observing;
		}
	}

	private void StartObserveSound()
	{
		base.GetComponent<AudioSource>().Stop();
		base.GetComponent<AudioSource>().clip = sounds.Observing;
		base.GetComponent<AudioSource>().loop = true;
		base.GetComponent<AudioSource>().Play();
	}

	private void StartWaiting()
	{
		if (Time.time > runtime.nextTauntTime)
		{
			runtime.nextTauntTime = Time.time + 1f;
		}
		anim.Stop();
		anim.Play(anims.Waiting.name);
		State = E_State.Waiting;
	}

	private void StopWaiting()
	{
		anim.Stop();
		NeckRotation = bodyParts.Head.rotation;
		bodyParts.Head.localRotation = Quaternion.identity;
	}

	private void UpdateWaiting()
	{
		UpdateTaunt();
		if (target.isVisible)
		{
			StopWaiting();
			State = E_State.Shooting;
		}
		else if (TargetLost())
		{
			StopWaiting();
			StartCoroutine("Dearming");
		}
	}

	private void StopObserveSound()
	{
		base.GetComponent<AudioSource>().Stop();
		base.GetComponent<AudioSource>().loop = false;
	}

	private void UpdateObserving()
	{
		target.inFOV = IsTargetInFOV();
		if (target.WithinEye)
		{
			StartTargeting();
		}
	}

	private void StartShooting()
	{
		if (State != E_State.Destroyed)
		{
			State = E_State.Shooting;
			base.GetComponent<AudioSource>().PlayOneShot(sounds.ShootingBegin);
			runtime.shootingInProgress = true;
			runtime.shootWarmupTime = Time.time + sounds.ShootingBegin.length;
		}
	}

	private void StartShootingEffects()
	{
		runtime.shootEffectsOn = true;
		anim.Play(anims.Shooting.name);
		ShowMuzzles();
	}

	private void EndShooting()
	{
		runtime.shootingInProgress = false;
		runtime.shootEffectsOn = false;
		anim.Stop();
		HideMuzzles();
	}

	private IEnumerator Arming()
	{
		State = E_State.Arming;
		anim.Play(anims.Arming.name);
		yield return new WaitForSeconds(anims.Arming.length);
		anim.Stop();
		StartShooting();
	}

	private IEnumerator Dearming()
	{
		State = E_State.Dearming;
		SetAlarmLight(E_AlarmLight.Active);
		base.GetComponent<AudioSource>().PlayOneShot(sounds.TargetLost);
		anim.Play(anims.Dearming.name);
		yield return new WaitForSeconds(anims.Dearming.length);
		anim.Stop();
		StartObserve();
	}

	private void ShowMuzzles()
	{
		for (int i = 0; i < numGuns; i++)
		{
			if (guns[i] != null && guns[i].muzzle != null)
			{
				guns[i].muzzle.active = true;
			}
		}
	}

	private void HideMuzzles()
	{
		for (int i = 0; i < numGuns; i++)
		{
			if (guns[i] != null && guns[i].muzzle != null)
			{
				guns[i].muzzle.active = false;
			}
		}
	}

	private void Reload()
	{
		runtime.reloadingTime = Time.time + Random.Range(shootSetup.afterBurstPauseMin, shootSetup.afterBurstPauseMax);
		runtime.bulletCounter = 0;
	}

	private void ShootPause()
	{
		runtime.shootPauseTime = Time.time + shootSetup.fireRate;
	}

	private void DamegeShock()
	{
		runtime.damageShockTime = Time.time + 1.8f;
		runtime.bulletCounter = 0;
	}

	private bool IsTargetVisible(Vector3 fromPos, Vector3 dirToTarget)
	{
		if (target.agent == null)
		{
			return false;
		}
		float magnitude = dirToTarget.magnitude;
		dirToTarget.Normalize();
		RaycastHit[] array = Physics.RaycastAll(fromPos, dirToTarget, magnitude);
		RaycastHit[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			RaycastHit raycastHit = array2[i];
			if (!(raycastHit.transform == base.transform) && !(raycastHit.transform == bodyParts.Armor.transform) && !raycastHit.collider.isTrigger && !(raycastHit.transform == target.agent.transform))
			{
				return false;
			}
		}
		return true;
	}

	private bool IsTargetInFOV()
	{
		if (State != E_State.Observing)
		{
			return false;
		}
		Vector3 view = target.AimPos - bodyParts.Eye.position;
		view.Normalize();
		Quaternion b = default(Quaternion);
		b.SetLookRotation(view, bodyParts.Neck.up);
		target.angleToTarget = Quaternion.Angle(EyeRotation, b);
		return target.angleToTarget < FieldOfView;
	}

	private IEnumerator DetectTarget()
	{
		while (true)
		{
			Vector3 detectorPos;
			float sqrRange;
			if (State == E_State.Hidden)
			{
				detectorPos = activation.AlarmSensor.position;
				sqrRange = activation.detectRange * activation.detectRange;
			}
			else
			{
				detectorPos = bodyParts.Eye.position;
				sqrRange = shootingRange * shootingRange;
			}
			Vector3 dirToTarget = target.AimPos - detectorPos;
			target.inRange = dirToTarget.sqrMagnitude < sqrRange;
			if (target.inRange && IsTargetVisible(detectorPos, dirToTarget))
			{
				target.isVisible = true;
				target.lastVisibleTime = Time.time;
			}
			else
			{
				target.isVisible = false;
			}
			yield return new WaitForSeconds(eyeFrequency);
		}
	}

	public void Fire(Vector3 targetPos)
	{
		runtime.bulletCounter++;
		for (int i = 0; i < numGuns; i++)
		{
			Gun gun = guns[i];
			Vector3 vector = (targetPos - bodyParts.Eye.position) * 1.2f;
			Vector3 vector2 = bodyParts.Eye.position + vector;
			Vector3 position = gun.shoot.position;
			Vector3 lookRotation = vector2 - position;
			Quaternion quaternion = default(Quaternion);
			quaternion.SetLookRotation(lookRotation);
			Vector2 vector3 = new Vector2(Random.Range(0f - shootSetup.dispersion, shootSetup.dispersion), Random.Range(0f - shootSetup.dispersion, shootSetup.dispersion));
			quaternion.eulerAngles = new Vector3(quaternion.eulerAngles.x + vector3.x, quaternion.eulerAngles.y + vector3.y, 0f);
			float speed = shootSetup.projectileSpeed * (1f + Random.Range(0f - shootSetup.rndSpeedVariation, shootSetup.rndSpeedVariation));
			Projectile.InitSettings initSettings = new Projectile.InitSettings();
			initSettings.Speed = speed;
			initSettings.Damage = shootSetup.projectileDamage;
			initSettings.Impuls = 0f;
			initSettings.IgnoreTransform = base.transform;
			ProjectileManager.Instance.SpawnProjectile(E_ProjectileType.Turret, position, quaternion * Vector3.forward, initSettings);
			if ((bool)gun.muzzle)
			{
				float num = 1f + Random.Range(0f - shootSetup.rndMuzzleScale, shootSetup.rndMuzzleScale);
				gun.muzzle.transform.localScale = new Vector3(num, num, num);
				gun.muzzle.transform.localEulerAngles = new Vector3(gun.muzzle.transform.localEulerAngles.x, gun.muzzle.transform.localEulerAngles.y, Random.Range(0, 50));
			}
		}
		PlayFireSound();
	}

	private void PlayFireSound()
	{
		if (sounds.Fire.Length <= 0)
		{
			return;
		}
		AudioClip audioClip = sounds.Fire[Random.Range(0, sounds.Fire.Length)];
		if (base.GetComponent<AudioSource>().isPlaying)
		{
			if (base.GetComponent<AudioSource>().clip == audioClip)
			{
				base.GetComponent<AudioSource>().time = 0f;
				return;
			}
			base.GetComponent<AudioSource>().Stop();
			base.GetComponent<AudioSource>().clip = audioClip;
			base.GetComponent<AudioSource>().Play();
		}
		else
		{
			base.GetComponent<AudioSource>().clip = audioClip;
			base.GetComponent<AudioSource>().Play();
		}
	}

	private void PlayOnHitFX()
	{
		if (!(m_ShieldFXLastPlayTime > 0f) || !(Time.time - m_ShieldFXLastPlayTime < m_ShieldFXDuration))
		{
			if ((bool)m_ShieldFXMeshRenderer)
			{
				m_ShieldFXMeshRenderer.material.SetFloat("_TimeOffs", 0f - Time.time);
			}
			m_ShieldFXLastPlayTime = Time.time;
		}
	}

	public void OnProjectileHit(Projectile inProjectile, HitZone inHitZone)
	{
		DoHitDamage(inProjectile.Damage * inHitZone.DamageModifier);
	}

	public void OnReceiveRangeDamage(float inDamage, HitZone inHitZone)
	{
		DoHitDamage(inDamage * inHitZone.DamageModifier);
	}

	private void DoHitDamage(float damage)
	{
		ReactOnHit();
		if (State != E_State.Destroyed)
		{
			hitPoints -= damage;
			if (hitPoints <= 0f)
			{
				Destroy();
			}
		}
	}

	private void OnArmorHit(float damage)
	{
		ReactOnHit();
	}

	private void OnArmorDestroy()
	{
		EndShooting();
		DamegeShock();
		base.GetComponent<AudioSource>().PlayOneShot(sounds.ShieldLost);
	}

	private void ReactOnHit()
	{
		if (State != E_State.Destroyed)
		{
			if (m_EnableShieldFX)
			{
				PlayOnHitFX();
			}
			if (State == E_State.Hidden)
			{
				StartCoroutine("Activate_Coroutine");
			}
			else if (State == E_State.Observing)
			{
				target.lastVisiblePos = target.AimPos;
				StartTargeting();
			}
			else if (State == E_State.Waiting)
			{
				StopWaiting();
				target.lastVisiblePos = target.AimPos;
				target.lastVisibleTime = Time.time;
				StartShooting();
			}
			else
			{
				target.lastVisiblePos = target.AimPos;
				target.lastVisibleTime = Time.time;
			}
		}
	}

	private IEnumerator Activate_Coroutine()
	{
		if (State == E_State.Hidden)
		{
			State = E_State.Activating;
			SetAlarmLight(E_AlarmLight.Active);
			anim.Play(anims.Activate.name);
			base.GetComponent<AudioSource>().PlayOneShot(sounds.Activated);
			yield return new WaitForSeconds(anims.Activate.length);
			target.isVisible = false;
			StartObserve();
		}
	}

	private void Destroy()
	{
		StopObserveSound();
		State = E_State.Destroyed;
		if (m_Explosion != null)
		{
			Explosion explosion = Mission.Instance.ExplosionCache.Get(m_Explosion, base.transform.position + base.transform.up, base.transform.rotation);
			if ((bool)explosion)
			{
				explosion.damage = 0f;
			}
		}
		base.GetComponent<AudioSource>().PlayOneShot(sounds.Destroyed);
		SetAlarmLight(E_AlarmLight.Off);
		StopAllCoroutines();
		HideMuzzles();
		anim.Play(anims.Destroy.name);
		Invoke("ReturnToCache", anims.Destroy.length);
	}

	private void ReturnToCache()
	{
		Mission.Instance.ReturnAgent(base.gameObject);
	}

	public override void OnShootSound(AgentHuman inAgent)
	{
		if (State == E_State.Hidden && !anim.IsPlaying(anims.Spawn.name))
		{
			StartCoroutine("Activate_Coroutine");
		}
	}
}
