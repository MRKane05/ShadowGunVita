using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[AddComponentMenu("Interaction/Play Cutscene")]
public class InteractionObjectCutscene : InteractionObject
{
	public AudioSource Audio;

	public List<GameObject> HideGameObjects = new List<GameObject>();

	public List<GameObject> ShowGameObjects = new List<GameObject>();

	public bool DisableAfterUse = true;

	public GameObject CutsceneCamera;

	public AnimationClip CameraAnim;

	public float FadeOutTime = 0.3f;

	public float FadeInTime = 0.3f;

	public Transform StartCutsceneTransform;

	public Transform EndCutsceneTransform;

	public List<GameEvent> GameEvents = new List<GameEvent>();

	public override float UseTime
	{
		get
		{
			return CameraAnim.length + FadeInTime + FadeOutTime;
		}
	}

	public override bool IsInteractionFinished { get; protected set; }

	private void Awake()
	{
		CutsceneCamera.SetActiveRecursively(false);
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
		IsInteractionFinished = false;
		base.Reset();
	}

	public override void DoInteraction()
	{
		if (DisableAfterUse)
		{
			base.InteractionObjectUsable = false;
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
		StartCoroutine(PlayCutscene());
	}

	private IEnumerator PlayCutscene()
	{
		IsInteractionFinished = false;
		Player.Instance.StopMove(true);
		Player.Instance.Owner.BlackBoard.DontUpdate = true;
		if (FadeInTime > 0f)
		{
			MFGuiManager.Instance.FadeOut(FadeInTime * 0.5f);
			yield return new WaitForSeconds(FadeInTime * 0.5f);
		}
		Camera old = Camera.main;
		GameCamera.Instance.StoreShakingState();
		old.gameObject.SetActiveRecursively(false);
		GuiHUD.Instance.Hide();
		CutsceneCamera.SetActiveRecursively(true);
		CutsceneCamera.GetComponent<Animation>().Play(CameraAnim.name);
		if ((bool)StartCutsceneTransform)
		{
			Player.Instance.Owner.Teleport(StartCutsceneTransform);
		}
		AgentActionPlayAnim action = AgentActionFactory.Create(AgentActionFactory.E_Type.PlayAnim) as AgentActionPlayAnim;
		action.AnimName = GetUserAnimation();
		Player.Instance.Owner.BlackBoard.ActionAdd(action);
		if ((bool)Audio)
		{
			Audio.Play();
		}
		if (FadeInTime > 0f)
		{
			MFGuiManager.Instance.FadeIn(FadeInTime * 0.5f);
			yield return new WaitForSeconds(FadeInTime * 0.5f);
		}
		yield return new WaitForSeconds(CutsceneCamera.GetComponent<Animation>()[CameraAnim.name].length);
		if (FadeOutTime > 0f)
		{
			MFGuiManager.Instance.FadeOut(FadeOutTime * 0.5f);
			yield return new WaitForSeconds(FadeOutTime * 0.5f);
		}
		if ((bool)EndCutsceneTransform)
		{
			Player.Instance.Owner.Teleport(EndCutsceneTransform);
		}
		CutsceneCamera.SetActiveRecursively(false);
		old.gameObject.SetActiveRecursively(true);
		GameCamera.Instance.ReStoreShakingState();
		if (FadeOutTime > 0f)
		{
			MFGuiManager.Instance.FadeIn(FadeOutTime * 0.5f);
		}
		Player.Instance.Owner.BlackBoard.DontUpdate = false;
		Player.Instance.StopMove(false);
		GuiHUD.Instance.Show();
		IsInteractionFinished = true;
	}
}
