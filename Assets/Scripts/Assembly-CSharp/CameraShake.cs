using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Miscellaneous/Camera Shake")]
public class CameraShake : MonoBehaviour
{
	public enum Events
	{
		OnEnter = 0,
		OnExit = 1
	}

	[Serializable]
	public class ShakingCameraAction
	{
		public Events m_OnEvent;

		public GameCamera.ShakingLoopEffect m_ShakingEffect = GameCamera.ShakingLoopEffect.Stop;

		public float m_ShakingEffectScale = 1f;
	}

	public List<ShakingCameraAction> m_Actions;

	private GameCamera m_GameCamera;

	private void Start()
	{
		m_GameCamera = GameCamera.Instance;
	}

	private void Update()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		if (IsPlayer(other) && m_GameCamera != null)
		{
			ReactoOnEvent(Events.OnEnter);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (IsPlayer(other) && m_GameCamera != null)
		{
			ReactoOnEvent(Events.OnExit);
		}
	}

	private void ReactoOnEvent(Events inEvent)
	{
		foreach (ShakingCameraAction action in m_Actions)
		{
			if (action.m_OnEvent == inEvent)
			{
				m_GameCamera.ShakingLoopAction(action.m_ShakingEffect, action.m_ShakingEffectScale);
			}
		}
	}

	private bool IsPlayer(Collider other)
	{
		return other == Player.Instance.Owner.CharacterController;
	}
}
