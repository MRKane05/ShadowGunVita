using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[AddComponentMenu("Interaction/Play Cutscene On Event")]
public class PlayCutsceneOnEvent : MonoBehaviour, IGameZoneControledObject
{
	[Serializable]
	public class Actor
	{
		public Transform LeaveTransform;

		public Transform StartTransform;

		public AnimationClip Animation;

		public AgentHuman Agent;

		public GameObject AnimatedGameObject;

		public Actor(AgentHuman agent, GameObject go, AnimationClip animation, Transform startTransform, Transform leaveTransform)
		{
			Agent = agent;
			AnimatedGameObject = go;
			Animation = animation;
			StartTransform = startTransform;
			LeaveTransform = leaveTransform;
		}
	}

	public GameObject CutsceneCamera;

	public AnimationClip CameraAnim;

	public float FadeOutTime = 0.3f;

	public float FadeInTime = 0.3f;

	public List<Actor> Actors = new List<Actor>();

	public List<OnGameEvent> OnGameEvents = new List<OnGameEvent>();

	public EventSender sendEvents = new EventSender();

	public bool PlayOnce;

	private bool Played;

	private void Start()
	{
		foreach (OnGameEvent onGameEvent in OnGameEvents)
		{
			GameBlackboard.Instance.GameEvents.AddEventChangeHandler(onGameEvent.Name, EventHandler);
		}
		CutsceneCamera.SetActiveRecursively(false);
	}

	public void EventHandler(string name, GameEvents.E_State state)
	{
		if (PlayOnce && Played)
		{
			return;
		}
		Played = true;
		foreach (OnGameEvent onGameEvent in OnGameEvents)
		{
			if (GameBlackboard.Instance.GameEvents.GetState(onGameEvent.Name) != onGameEvent.State)
			{
				return;
			}
		}
		StartCoroutine(PlayCutscene());
	}

	private IEnumerator PlayCutscene()
	{
		Player.Instance.Owner.BlackBoard.DontUpdate = true;
		Player.Instance.StopMove(true);
		if (FadeInTime > 0f)
		{
			MFGuiManager.Instance.FadeOut(FadeInTime * 0.5f);
			yield return new WaitForSeconds(FadeInTime * 0.5f);
		}
		foreach (Actor a in Actors)
		{
			if ((bool)a.StartTransform)
			{
				if ((bool)a.AnimatedGameObject)
				{
					a.AnimatedGameObject.transform.position = a.StartTransform.position;
					a.AnimatedGameObject.transform.rotation = a.StartTransform.rotation;
				}
				if ((bool)a.Agent)
				{
					a.Agent.Teleport(a.StartTransform);
				}
			}
		}
		Camera old = Camera.main;
		GameCamera.Instance.StoreShakingState();
		old.gameObject.SetActiveRecursively(false);
		GuiHUD.Instance.Hide();
		GuiHUD.Instance.PlayCutsceneBegin();
		CutsceneCamera.SetActiveRecursively(true);
		CutsceneCamera.GetComponent<Animation>().Play(CameraAnim.name);
		foreach (Actor a2 in Actors)
		{
			if ((bool)a2.Animation)
			{
				if ((bool)a2.Agent)
				{
					a2.Agent.Animation.AddClip(a2.Animation, a2.Animation.name);
					AgentActionPlayAnim action = AgentActionFactory.Create(AgentActionFactory.E_Type.PlayAnim) as AgentActionPlayAnim;
					action.AnimName = a2.Animation.name;
					a2.Agent.BlackBoard.ActionAdd(action);
				}
				if ((bool)a2.AnimatedGameObject)
				{
					a2.AnimatedGameObject.GetComponent<Animation>().AddClip(a2.Animation, a2.Animation.name);
					a2.AnimatedGameObject.GetComponent<Animation>().Play(a2.Animation.name);
				}
			}
		}
		if (FadeInTime > 0f)
		{
			MFGuiManager.Instance.FadeIn(FadeInTime * 0.5f);
			yield return new WaitForSeconds(FadeInTime * 0.5f);
		}
		yield return new WaitForSeconds(CutsceneCamera.GetComponent<Animation>()[CameraAnim.name].length);
		MFGuiManager.Instance.FadeOut(FadeInTime * 0.5f);
		yield return new WaitForSeconds(FadeOutTime * 0.5f);
		foreach (Actor a3 in Actors)
		{
			if ((bool)a3.LeaveTransform)
			{
				if ((bool)a3.AnimatedGameObject)
				{
					a3.AnimatedGameObject.transform.position = a3.LeaveTransform.position;
					a3.AnimatedGameObject.transform.rotation = a3.LeaveTransform.rotation;
				}
				if ((bool)a3.Agent)
				{
					a3.Agent.Teleport(a3.LeaveTransform);
				}
			}
		}
		CutsceneCamera.SetActiveRecursively(false);
		old.gameObject.SetActiveRecursively(true);
		GameCamera.Instance.ReStoreShakingState();
		MFGuiManager.Instance.FadeIn(FadeOutTime * 0.5f);
		Player.Instance.Owner.BlackBoard.DontUpdate = false;
		Player.Instance.StopMove(false);
		GuiHUD.Instance.PlayCutsceneEnd();
		GuiHUD.Instance.Show();
		sendEvents.Send();
	}

	public void Enable()
	{
	}

	public void Disable()
	{
	}

	public void Reset()
	{
	}
}
