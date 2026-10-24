using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[AddComponentMenu("Interaction/Event Target")]
public class InteractionTarget : MonoBehaviour
{
	[SerializeField]
	private List<InteractionTargetState> InteractionStates = new List<InteractionTargetState>();

	private bool Enabled;

	public List<InteractionTargetState> States
	{
		get
		{
			return InteractionStates;
		}
	}

	private void Start()
	{
		foreach (InteractionTargetState interactionState in InteractionStates)
		{
			interactionState.Initialize(base.gameObject);
		}
	}

	private void OnDestroy()
	{
		InteractionStates.Clear();
	}

	public void Enable()
	{
		if (Enabled)
		{
			return;
		}
		foreach (InteractionTargetState interactionState in InteractionStates)
		{
			interactionState.Enable();
		}
		Enabled = true;
	}

	public void Disable()
	{
		if (!Enabled)
		{
			return;
		}
		foreach (InteractionTargetState interactionState in InteractionStates)
		{
			interactionState.Disable();
		}
		Enabled = false;
	}

	public void Reset()
	{
		foreach (InteractionTargetState interactionState in InteractionStates)
		{
			interactionState.Reset();
		}
	}
}
