using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[AddComponentMenu("Interaction/Use Object")]
public class InteractionObjectUse : InteractionObject
{
	public AnimationClip AnimationClip;

	public AudioSource Audio;

	public List<GameObject> HideGameObjects = new List<GameObject>();

	public List<GameObject> ShowGameObjects = new List<GameObject>();

	public bool DisableAfterUse = true;

	public GameObject Visual;

	public List<GameEvent> GameEvents = new List<GameEvent>();

	private void Awake()
	{
		if (Visual != null)
		{
			Animation = Visual.GetComponent<Animation>();
			Animation.wrapMode = WrapMode.Once;
		}
	}

	private void OnDestroy()
	{
		AnimationClip = null;
		Audio = null;
		HideGameObjects.Clear();
		ShowGameObjects.Clear();
		Visual = null;
	}

	private void Start()
	{
		Initialize();
	}

	public override void Enable()
	{
		base.Enable();
	}

	public override void Disable()
	{
		base.Disable();
	}

	public override void Reset()
	{
		base.Reset();
		if (Visual != null)
		{
			Animation.Stop();
			if ((bool)AnimationClip)
			{
				AnimationClip.SampleAnimation(Visual, 0f);
			}
		}
	}

	public override void DoInteraction()
	{
		base.DoInteraction();
		if (DisableAfterUse)
		{
			base.InteractionObjectUsable = false;
		}
		if ((bool)Audio)
		{
			Audio.Play();
		}
		if ((bool)AnimationClip && (bool)Animation)
		{
			Animation.Play(AnimationClip.name);
		}
		foreach (GameEvent gameEvent in GameEvents)
		{
			Mission.Instance.SendGameEvent(gameEvent.Name, gameEvent.State, gameEvent.Delay);
		}
		foreach (GameObject showGameObject in ShowGameObjects)
		{
			showGameObject.SetActiveRecursively(true);
		}
		foreach (GameObject hideGameObject in HideGameObjects)
		{
			hideGameObject.SetActiveRecursively(false);
		}
	}
}
