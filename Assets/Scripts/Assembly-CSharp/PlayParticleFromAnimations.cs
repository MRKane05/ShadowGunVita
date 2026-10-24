using UnityEngine;

[AddComponentMenu("Miscellaneous/Play Particle From Animation")]
public class PlayParticleFromAnimations : MonoBehaviour
{
	public void AnimNotify_PlayOneShot(string name)
	{
		GameObject gameObject = GameObject.Find(name);
		if ((bool)gameObject)
		{
			ParticleSystem[] componentsInChildren = gameObject.gameObject.GetComponentsInChildren<ParticleSystem>();
			ParticleSystem[] array = componentsInChildren;
			foreach (ParticleSystem particleSystem in array)
			{
				particleSystem.Play();
			}
		}
	}

	public void AnimNotify_Stop(string name)
	{
		GameObject gameObject = GameObject.Find(name);
		if ((bool)gameObject)
		{
			ParticleSystem[] componentsInChildren = gameObject.GetComponentsInChildren<ParticleSystem>();
			ParticleSystem[] array = componentsInChildren;
			foreach (ParticleSystem particleSystem in array)
			{
				particleSystem.Stop();
			}
		}
	}
}
