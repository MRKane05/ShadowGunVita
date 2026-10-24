using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[AddComponentMenu("Weapons/WeaponBase")]
[RequireComponent(typeof(AudioSource))]
public abstract class WeaponBase : MonoBehaviour
{
	[Serializable]
	public class SoundsInfo
	{
		public AudioClip SoundShow;

		public AudioClip SoundReload;

		public AudioClip[] SoundFire = new AudioClip[0];
	}

	public int MaxAmmoInClip;

	public int MaxAmmoInWeapon;

	public float FireTime;

	public float Damage;

	public float Speed;

	public float CriticalChance;

	public float Impuls;

	public float Dispersion;

	public SoundsInfo SoundsSetupAI = new SoundsInfo();

	public SoundsInfo SoundsSetupPlayer = new SoundsInfo();

	public GameObject Muzzle;

	public ParticleSystem Shells;

	protected AgentHuman Owner;

	protected int _AmmoInClip;

	protected int _AmmoInWeapon;

	protected Transform TransformShot;

	protected GameObject GameObject;

	protected Transform Transform;

	protected AudioSource Audio;

	protected Quaternion Temp = default(Quaternion);

	protected float TimeReload;

	protected float TimeArm;

	protected float TimeDisarm;

	protected Projectile.InitSettings InitProjSettings;

	private float Busy;

	public E_WeaponType WeaponType { get; private set; }

	public bool IsPlayerWeapon { get; protected set; }

	public E_ProjectileType ProjectileType { get; private set; }

	public int ClipAmmo
	{
		get
		{
			return _AmmoInClip;
		}
		protected set
		{
			_AmmoInClip = Mathf.Clamp(value, 0, MaxAmmoInClip);
		}
	}

	public int WeaponAmmo
	{
		get
		{
			return _AmmoInWeapon;
		}
		protected set
		{
			_AmmoInWeapon = Mathf.Clamp(value, 0, MaxAmmoInWeapon);
		}
	}

	public virtual bool IsFull
	{
		get
		{
			return WeaponAmmo == MaxAmmoInWeapon;
		}
	}

	public bool IsFullyLoaded
	{
		get
		{
			return ClipAmmo == MaxAmmoInClip;
		}
	}

	public bool HasAnyAmmo
	{
		get
		{
			return ClipAmmo + WeaponAmmo > 0;
		}
	}

	public Vector3 ShotPos
	{
		get
		{
			return TransformShot.position;
		}
	}

	public bool IsBusy()
	{
		return Busy > Time.timeSinceLevelLoad;
	}

	public void SetBusy(float busyTime)
	{
		Busy = Time.timeSinceLevelLoad + busyTime;
	}

	public void Initialize(E_WeaponType type, E_ProjectileType projType)
	{
		WeaponType = type;
		ProjectileType = projType;
		_AmmoInClip = MaxAmmoInClip;
		_AmmoInWeapon = MaxAmmoInWeapon;
		GameObject = base.gameObject;
		Audio = base.GetComponent<AudioSource>();
		Transform = GameObject.transform;
		TransformShot = base.transform.Find("Shoot");
		InitProjSettings = new Projectile.InitSettings();
		InitProjSettings.Weapon = WeaponType;
		InitProjSettings.Speed = Speed;
		InitProjSettings.Damage = Damage;
		InitProjSettings.Impuls = Impuls;
	}

	private void OnDestroy()
	{
		SoundsSetupAI = null;
		SoundsSetupPlayer = null;
		Muzzle = null;
		Shells = null;
	}

	public virtual void Reset(bool unlink)
	{
		Busy = 0f;
		_AmmoInClip = MaxAmmoInClip;
		_AmmoInWeapon = MaxAmmoInWeapon;
		if (unlink)
		{
			UnlinkFromOwner();
		}
		ShowWeapon(false);
	}

	public virtual void LinkToOwner(AgentHuman owner, Transform linkTo)
	{
		Owner = owner;
		Transform.parent = linkTo;
		Transform.localPosition = Vector3.zero;
		Transform.localRotation = Quaternion.identity;
		TimeReload = Owner.AnimSet.GetWeaponAnimTime(WeaponType, E_WeaponAction.Reload);
		TimeArm = Owner.AnimSet.GetWeaponAnimTime(WeaponType, E_WeaponAction.Arm);
		TimeDisarm = Owner.AnimSet.GetWeaponAnimTime(WeaponType, E_WeaponAction.Disarm);
		ShowWeapon(true);
	}

	public virtual void UnlinkFromOwner()
	{
		Owner = null;
		Transform.parent = null;
		Transform.localPosition = Vector3.zero;
		Transform.localRotation = Quaternion.identity;
		ShowWeapon(false);
	}

	public virtual void Fire(Vector3 direction)
	{
		if (ClipAmmo != 0 && !IsBusy())
		{
			PlaySoundFire();
			SpawnProjectile(direction);
			if ((bool)Muzzle)
			{
				float num = Random.Range(0.95f, 1.1f);
				Muzzle.transform.localScale = new Vector3(num, num, num);
				Muzzle.transform.localEulerAngles = new Vector3(Muzzle.transform.localEulerAngles.x, Muzzle.transform.localEulerAngles.y, Random.Range(0, 50));
				Muzzle.SetActiveRecursively(true);
			}
			if ((bool)Shells)
			{
				Shells.Emit(1);
			}
			DecreaseAmmo();
			SetBusy(FireTime);
			if (Mission.Instance.CurrentGameZone != null && Owner.IsPlayer)
			{
				Mission.Instance.CurrentGameZone.SendFactToEnemies(Owner, Owner, E_EventTypes.EnemyFire, 30f, 10f, false);
			}
		}
	}

	protected virtual void SpawnProjectile(Vector3 direction)
	{
		float num = (Owner.IsPlayer ? ((!Owner.IsInCover) ? Dispersion : (Dispersion * 0.5f)) : ((Game.Instance.GameDifficulty != E_GameDifficulty.Hard) ? Dispersion : (Dispersion * 0.5f)));
		Temp.SetLookRotation(direction);
		Temp.eulerAngles = new Vector3(Temp.eulerAngles.x + Random.Range(0f - num, num), Temp.eulerAngles.y + Random.Range(0f - num, num), 0f);
		InitProjSettings.Agent = Owner;
		ProjectileManager.Instance.SpawnProjectile(ProjectileType, TransformShot.position, Temp * Vector3.forward, InitProjSettings);
	}

	protected virtual void PlaySoundFire()
	{
		AudioClip[] array = ((!Owner.IsPlayer) ? SoundsSetupAI.SoundFire : SoundsSetupPlayer.SoundFire);
		if (array.Length <= 0)
		{
			return;
		}
		AudioClip audioClip = array[Random.Range(0, array.Length)];
		if (Audio.isPlaying)
		{
			if (Audio.clip == audioClip)
			{
				Audio.time = 0f;
				return;
			}
			Audio.Stop();
			Audio.clip = audioClip;
			Audio.Play();
		}
		else
		{
			Audio.clip = audioClip;
			Audio.Play();
		}
	}

	public virtual void Reload()
	{
		Owner.SoundPlay((!Owner.IsPlayer) ? SoundsSetupAI.SoundReload : SoundsSetupPlayer.SoundReload);
		if (MaxAmmoInWeapon == -1)
		{
			_AmmoInClip = MaxAmmoInClip;
		}
		else if (_AmmoInWeapon > 0)
		{
			int a = MaxAmmoInClip - _AmmoInClip;
			int num = Mathf.Min(a, _AmmoInWeapon);
			_AmmoInClip += num;
			_AmmoInWeapon -= num;
		}
		SetBusy(TimeReload);
	}

	public virtual void WeaponArm()
	{
		Owner.SoundPlay((!Owner.IsPlayer) ? SoundsSetupAI.SoundShow : SoundsSetupPlayer.SoundShow);
		SetBusy(TimeArm);
	}

	public virtual void DecreaseAmmo()
	{
		_AmmoInClip--;
	}

	public virtual void AddAmmo(int ammo)
	{
		if (ammo == -1)
		{
			_AmmoInClip = MaxAmmoInClip;
			_AmmoInWeapon = MaxAmmoInWeapon;
		}
		else if (_AmmoInWeapon + ammo > MaxAmmoInWeapon)
		{
			_AmmoInWeapon = MaxAmmoInWeapon;
		}
		else
		{
			_AmmoInWeapon += ammo;
		}
	}

	public void SetAmmo(int ammoInClip, int ammoInWeapon)
	{
		_AmmoInClip = ammoInClip;
		_AmmoInWeapon = ammoInWeapon;
	}

	public void ShowWeapon(bool show)
	{
		GameObject.SetActiveRecursively(show);
		if ((bool)Muzzle)
		{
			Muzzle.SetActiveRecursively(false);
		}
		if (show)
		{
			StartCoroutine(UpdateFireEffect());
		}
		else
		{
			StopCoroutine("UpdateFireEffect");
		}
		if ((bool)Shells)
		{
			if (show)
			{
				Shells.Play();
			}
			else
			{
				Shells.Stop();
			}
		}
	}

	protected virtual IEnumerator UpdateFireEffect()
	{
		while (true)
		{
			if ((bool)Muzzle && Muzzle.active)
			{
				yield return new WaitForSeconds(Random.Range(0f, 0.05f));
				Muzzle.SetActiveRecursively(false);
			}
			yield return new WaitForEndOfFrame();
		}
	}
}
