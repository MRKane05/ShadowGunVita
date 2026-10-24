using System;
using System.Collections;
using UnityEngine;

[AddComponentMenu("Interaction/Break Object With Anim")]
public class BreakableObject : MonoBehaviour, IProjectileHitReceiver
{
	[Serializable]
	public class InteractionParticle
	{
		public ParticleSystem Emitter;

		public float Delay;

		public bool LinkOnRoot;
	}

	[Serializable]
	public class InteractionSound
	{
		public AudioSource Audio;

		public float Delay;

		public float Life;

		public Transform Parent;
	}

	public float Health;

	public AnimationClip AnimBreak;

	public InteractionParticle[] Emitters;

	public InteractionSound[] Sounds;

	protected bool Active = true;

	protected Transform Root;

	private Animation Animation;

	private GameObject GameObject;

	public bool IsActive
	{
		get
		{
			return Active;
		}
	}

	public Vector3 Position
	{
		get
		{
			return Root.position;
		}
	}

	public void Initialize()
	{
		GameObject = base.gameObject;
		Root = base.transform;
		Animation = GameObject.GetComponent<Animation>();
		if ((bool)Animation)
		{
			Animation.wrapMode = WrapMode.Once;
		}
	}

	public void OnProjectileHit(Projectile projectile)
	{
		if (Active)
		{
			Health -= projectile.Damage;
			if (Health < 0f)
			{
				Break();
			}
		}
	}

	public virtual void Break()
	{
		Active = false;
		if ((bool)Animation && (bool)AnimBreak)
		{
			Animation.Play(AnimBreak.name);
		}
		int num = 0;
		while (Emitters != null && num < Emitters.Length)
		{
			Mission.Instance.StartCoroutine(ParticleRun(Emitters[num].Emitter, Emitters[num].Delay));
			if (Emitters[num].LinkOnRoot)
			{
				Emitters[num].Emitter.transform.parent = Root;
			}
			num++;
		}
		int num2 = 0;
		while (Sounds != null && num2 < Sounds.Length)
		{
			Mission.Instance.StartCoroutine(SoundRun(Sounds[num2].Audio, Sounds[num2].Delay));
			Mission.Instance.StartCoroutine(SoundStop(Sounds[num2].Audio, Sounds[num2].Delay + Sounds[num2].Life));
			if ((bool)Sounds[num2].Parent)
			{
				Sounds[num2].Audio.transform.parent = Sounds[num2].Parent;
			}
			num2++;
		}
		OnStart();
	}

	protected virtual void OnStart()
	{
	}

	protected virtual void OnDone()
	{
	}

	public virtual void Reset()
	{
		if ((bool)Animation && (bool)AnimBreak)
		{
			Animation.Stop();
			AnimBreak.SampleAnimation(GameObject, 0f);
		}
		Active = true;
	}

	public void Enable()
	{
		GameObject.SetActiveRecursively(true);
	}

	public void Disable()
	{
		GameObject.SetActiveRecursively(false);
	}

	private IEnumerator ParticleRun(ParticleSystem emitter, float delay)
	{
		yield return new WaitForSeconds(delay);
		emitter.Play();
	}

	private IEnumerator ParticleStop(ParticleSystem emitter, float delay)
	{
		yield return new WaitForSeconds(delay);
		emitter.Stop();
	}

	private IEnumerator SoundRun(AudioSource audio, float delay)
	{
		yield return new WaitForSeconds(delay);
		audio.Play();
	}

	private IEnumerator SoundStop(AudioSource audio, float delay)
	{
		yield return new WaitForSeconds(delay);
		audio.Stop();
	}
}
