using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PadLock : MonoBehaviour
{
	public enum E_State
	{
		E_OFF = 0,
		E_ON = 1
	}

	public GameObject Collision;

	private Animation Animation;

	private GameObject GameObject;

	private AudioSource AudioSource;

	public AnimationClip AnimON;

	public AnimationClip AnimOFF;

	public AnimationClip AnimLoop;

	public AudioClip SoundOn;

	public AudioClip SoundOff;

	public E_State State;

	public bool HideWhenUnlock = true;

	private void Awake()
	{
		Animation = base.GetComponent<Animation>();
		GameObject = base.gameObject;
		AudioSource = base.GetComponent<AudioSource>();
		Reset();
	}

	private void OnEnable()
	{
		if (State == E_State.E_OFF)
		{
			if (HideWhenUnlock)
			{
				GameObject.SetActiveRecursively(false);
			}
			if (Collision != null)
			{
				Collision.SetActiveRecursively(false);
			}
		}
	}

	public void Lock()
	{
		if (State != E_State.E_ON)
		{
			State = E_State.E_ON;
			GameObject.SetActiveRecursively(true);
			if (Collision != null)
			{
				Collision.SetActiveRecursively(true);
			}
			if (AnimON != null)
			{
				Animation.Play(AnimON.name);
			}
			if (AnimLoop != null)
			{
				Animation.PlayQueued(AnimLoop.name);
			}
			if (SoundOn != null)
			{
				AudioSource.PlayOneShot(SoundOn);
			}
		}
	}

	public void Unlock()
	{
		if (State != E_State.E_OFF)
		{
			State = E_State.E_OFF;
			StartCoroutine(Hide());
		}
	}

	public void Reset()
	{
		State = E_State.E_OFF;
		Animation.Stop();
		if (HideWhenUnlock)
		{
			GameObject.SetActiveRecursively(false);
		}
		if (Collision != null)
		{
			Collision.SetActiveRecursively(false);
		}
	}

	private IEnumerator Hide()
	{
		yield return new WaitForSeconds(0.3f);
		Animation.Stop();
		if (AnimOFF != null)
		{
			Animation.Play(AnimOFF.name);
		}
		float wait = 0f;
		if (SoundOff != null)
		{
			AudioSource.PlayOneShot(SoundOff);
			wait = SoundOff.length;
		}
		if (AnimOFF != null)
		{
			if (wait < AnimOFF.length)
			{
				wait = AnimOFF.length;
			}
			else
			{
				Invoke("CollisionOff", AnimOFF.length);
			}
		}
		yield return new WaitForSeconds(wait);
		if (HideWhenUnlock)
		{
			GameObject.SetActiveRecursively(false);
		}
		CollisionOff();
	}

	private void CollisionOff()
	{
		if (Collision != null)
		{
			Collision.SetActiveRecursively(false);
		}
	}
}
