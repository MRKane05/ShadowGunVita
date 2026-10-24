using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[AddComponentMenu("Interaction/Play Camera Anim on Event")]
public class PlayCameraAnimOnEvent : MonoBehaviour, IGameZoneControledObject
{
	public GameObject CutsceneCamera;

	public AnimationClip CameraAnim;

	public float FadeOutTime = 0.3f;

	public float FadeInTime = 0.3f;

	public Transform LeaveTransform;

	public Transform StartTransform;

	public List<OnGameEvent> OnGameEvents = new List<OnGameEvent>();

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
		if ((bool)StartTransform)
		{
			Player.Instance.Owner.Teleport(StartTransform);
		}
		Camera old = Camera.main;
		GameCamera.Instance.StoreShakingState();
		old.gameObject.SetActiveRecursively(false);
		GuiHUD.Instance.Hide();
		GuiHUD.Instance.PlayCutsceneBegin();
		CutsceneCamera.SetActiveRecursively(true);
		CutsceneCamera.GetComponent<Animation>().Play(CameraAnim.name);
		if (FadeInTime > 0f)
		{
			MFGuiManager.Instance.FadeIn(FadeInTime * 0.5f);
			yield return new WaitForSeconds(FadeInTime * 0.5f);
		}
		yield return new WaitForSeconds(CutsceneCamera.GetComponent<Animation>()[CameraAnim.name].length);
		MFGuiManager.Instance.FadeOut(FadeInTime * 0.5f);
		yield return new WaitForSeconds(FadeOutTime * 0.5f);
		if ((bool)LeaveTransform)
		{
			Player.Instance.Owner.Teleport(LeaveTransform);
		}
		CutsceneCamera.SetActiveRecursively(false);
		old.gameObject.SetActiveRecursively(true);
		GameCamera.Instance.ReStoreShakingState();
		MFGuiManager.Instance.FadeIn(FadeOutTime * 0.5f);
		Player.Instance.Owner.BlackBoard.DontUpdate = false;
		Player.Instance.StopMove(false);
		GuiHUD.Instance.PlayCutsceneEnd();
		GuiHUD.Instance.Show();
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
