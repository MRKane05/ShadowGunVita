using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[AddComponentMenu("Miscellaneous/Play Sound From Animations")]
[RequireComponent(typeof(Animation))]
public class PlaySoundFromAnimations : MonoBehaviour
{
	public void AnimNotify_PlayOneShot(AudioClip inClip)
	{
		base.GetComponent<AudioSource>().PlayOneShot(inClip);
	}
}
