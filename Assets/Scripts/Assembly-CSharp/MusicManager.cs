using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[AddComponentMenu("Music + Sound/Music Manager")]
public class MusicManager : MonoBehaviour
{
	[Serializable]
	public class MusicEvent
	{
		public string Name;

		public GameEvents.E_State State;

		public AudioClip Clip;

		public float FadeOutTime = 1.5f;

		public float FadeInTime = 1.5f;

		public float Volume = 1f;

		public MusicEvent(string name, GameEvents.E_State state, AudioClip clip, float fadeoutTime, float fadeInTime, float volume)
		{
			Name = name;
			State = state;
			Clip = clip;
			FadeOutTime = fadeoutTime;
			FadeInTime = fadeInTime;
			Volume = volume;
		}
	}

	private const float MaxMusicVolume = 0f;

	public AudioClip DefaultClip;

	public float DefaultFadeInTime = 1.5f;

	public float DefaultVolume = 1f;

	public List<MusicEvent> MusicEvents = new List<MusicEvent>();

	public static MusicManager Instance;

	private AudioSource Audio;

	private float UnmodifiedVolume = 1f;

	private void Awake()
	{
		Instance = this;
		Audio = base.GetComponent<AudioSource>();
	}

	private void Start()
	{
		foreach (MusicEvent musicEvent in MusicEvents)
		{
			GameBlackboard.Instance.GameEvents.AddEventChangeHandler(musicEvent.Name, EventHandler);
		}
		PlayDefaultMusic();
	}

	private void OnDestroy()
	{
		if ((bool)Audio)
		{
			Audio.Stop();
		}
	}

	public void ApplyVolumeChange()
	{
		Audio.volume = UnmodifiedVolume * GuiOptions.musicVolume;
	}

	public void PlayDefaultMusic()
	{
		SetNewMusic(DefaultClip, DefaultVolume, 0f, DefaultFadeInTime);
	}

	public void SetNewMusic(AudioClip clip, float volume, float fadeOutTime, float fadeIntime)
	{
		StopCoroutine("FadeOutInMusic");
		UnmodifiedVolume = volume;
		StartCoroutine(SwitchMusic(clip, UnmodifiedVolume * GuiOptions.musicVolume, fadeOutTime, fadeIntime));
	}

	public void FadeOutMusic(float fadeOutTime)
	{
		StopCoroutine("FadeOutInMusic");
		StartCoroutine(SwitchMusic(null, 0f, fadeOutTime, 0f));
	}

	private IEnumerator SwitchMusic(AudioClip clip, float musicVolume, float fadeOutTime, float fadeIntime)
	{
		if (Audio.clip == clip)
		{
			yield break;
		}
		if (Audio.isPlaying)
		{
			if (fadeOutTime == 0f)
			{
				Audio.volume = 0f;
				Audio.Stop();
			}
			else
			{
				float maxVolume = Audio.volume;
				float volume = Audio.volume;
				while (volume > 0f)
				{
					volume -= 1f / fadeOutTime * Time.deltaTime * maxVolume;
					if (volume < 0f)
					{
						volume = 0f;
					}
					Audio.volume = volume;
					yield return new WaitForEndOfFrame();
				}
				Audio.Stop();
				Audio.clip = null;
			}
		}
		yield return new WaitForEndOfFrame();
		if (!(clip != null))
		{
			yield break;
		}
		Audio.clip = clip;
		Audio.Play();
		if (fadeIntime == 0f)
		{
			Audio.volume = musicVolume;
			yield break;
		}
		float volume2 = 0f;
		while (volume2 < musicVolume)
		{
			volume2 += 1f / fadeIntime * Time.deltaTime * musicVolume;
			if (volume2 > musicVolume)
			{
				volume2 = musicVolume;
			}
			Audio.volume = volume2;
			yield return new WaitForEndOfFrame();
		}
		Audio.volume = musicVolume;
	}

	public void EventHandler(string name, GameEvents.E_State state)
	{
		foreach (MusicEvent musicEvent in MusicEvents)
		{
			if (musicEvent.Name == name && musicEvent.State == state)
			{
				SetNewMusic(musicEvent.Clip, musicEvent.Volume, musicEvent.FadeOutTime, musicEvent.FadeInTime);
				break;
			}
		}
	}
}
