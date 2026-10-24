using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[AddComponentMenu("Interaction/Play Animations On Event")]
public class PlayAnimationsOnEvent : MonoBehaviour
{
	[Serializable]
	public class Actor
	{
		public Transform StartTransform;

		public Transform LeaveTransform;

		public AnimationClip Animation;

		public GameObject AnimatedGameObject;

		public Actor(GameObject go, AnimationClip animation, Transform startTransform, Transform leaveTransform)
		{
			AnimatedGameObject = go;
			Animation = animation;
			StartTransform = startTransform;
			LeaveTransform = leaveTransform;
		}
	}

	public List<Actor> Actors = new List<Actor>();

	public List<OnGameEvent> OnGameEvents = new List<OnGameEvent>();

	private void Start()
	{
		foreach (OnGameEvent onGameEvent in OnGameEvents)
		{
			GameBlackboard.Instance.GameEvents.AddEventChangeHandler(onGameEvent.Name, EventHandler);
		}
	}

	public void EventHandler(string name, GameEvents.E_State state)
	{
		foreach (OnGameEvent onGameEvent in OnGameEvents)
		{
			if (GameBlackboard.Instance.GameEvents.GetState(onGameEvent.Name) != onGameEvent.State)
			{
				return;
			}
		}
		foreach (Actor actor in Actors)
		{
			if ((bool)actor.StartTransform && (bool)actor.AnimatedGameObject)
			{
				actor.AnimatedGameObject.transform.position = actor.StartTransform.position;
				actor.AnimatedGameObject.transform.rotation = actor.StartTransform.rotation;
				actor.AnimatedGameObject.GetComponent<Animation>().Stop();
				actor.AnimatedGameObject.GetComponent<Animation>().AddClip(actor.Animation, actor.Animation.name);
				actor.AnimatedGameObject.GetComponent<Animation>().Play(actor.Animation.name);
				if ((bool)actor.LeaveTransform)
				{
					StartCoroutine(SetOnTransform(actor.Animation.length, actor));
				}
			}
		}
	}

	private IEnumerator SetOnTransform(float delay, Actor a)
	{
		yield return new WaitForSeconds(delay);
		a.AnimatedGameObject.transform.position = a.LeaveTransform.position;
		a.AnimatedGameObject.transform.rotation = a.LeaveTransform.rotation;
	}
}
