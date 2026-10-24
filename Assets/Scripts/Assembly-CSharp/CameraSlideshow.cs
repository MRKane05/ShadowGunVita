using System.Collections;
using UnityEngine;

[AddComponentMenu("GUI/Game/CameraSlideshow")]
public class CameraSlideshow : MonoBehaviour
{
	public GUIBase_Widget fadeWIdget;

	public float fadeTime = 1f;

	private string[] animName;

	private int currentAnim;

	private float targetFadeTime;

	private int fade;

	private void Awake()
	{
		animName = new string[base.GetComponent<Animation>().GetClipCount()];
		int num = 0;
		foreach (AnimationState item in base.GetComponent<Animation>())
		{
			animName[num++] = item.clip.name;
		}
	}

	private void Start()
	{
		if (animName.Length > 0)
		{
			StartCoroutine("Slideshow");
		}
	}

	private void OnDisable()
	{
		StopCoroutine("Slideshow");
	}

	private void OnDestroy()
	{
		StopCoroutine("Slideshow");
	}

	private void LateUpdate()
	{
		if (fade == 0)
		{
			return;
		}
		if (Time.time < targetFadeTime)
		{
			float num = (targetFadeTime - Time.time) / fadeTime;
			float fadeAlpha = ((fade <= 0) ? num : (1f - num));
			fadeWIdget.m_FadeAlpha = fadeAlpha;
			return;
		}
		if (fade > 0)
		{
			fadeWIdget.m_FadeAlpha = 1f;
		}
		else
		{
			fadeWIdget.m_FadeAlpha = 0f;
			fadeWIdget.Show(false, true);
		}
		fade = 0;
	}

	private void FadeIn()
	{
		fadeWIdget.m_FadeAlpha = 1f;
		fadeWIdget.Show(true, true);
		targetFadeTime = Time.time + fadeTime;
		fade = -1;
	}

	private void FadeOut()
	{
		fadeWIdget.m_FadeAlpha = 0f;
		fadeWIdget.Show(true, true);
		targetFadeTime = Time.time + fadeTime;
		fade = 1;
	}

	private IEnumerator Slideshow()
	{
		yield return new WaitForSeconds(0.1f);
		fadeWIdget.m_FadeAlpha = 1f;
		fadeWIdget.Show(true, true);
		while (true)
		{
			FadeIn();
			base.GetComponent<Animation>().Play(animName[currentAnim]);
			yield return new WaitForSeconds(base.GetComponent<Animation>()[animName[currentAnim]].length - fadeTime);
			FadeOut();
			yield return new WaitForSeconds(fadeTime);
			base.GetComponent<Animation>().Stop();
			currentAnim++;
			if (currentAnim >= animName.Length)
			{
				currentAnim = 0;
			}
		}
	}
}
