using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
	private enum E_State
	{
		ThirdPerson = 0,
		Cover = 1
	}

	private AgentHuman Owner;

	private AgentAction WaitForEndOfAction;

	private CameraState State;

	private Dictionary<E_State, CameraState> States = new Dictionary<E_State, CameraState>();

	private void Awake()
	{
		Owner = GetComponent<AgentHuman>();
		States.Add(E_State.ThirdPerson, new CameraState3RD(Owner));
		States.Add(E_State.Cover, new CameraStateCover(Owner));
		State = States[E_State.ThirdPerson];
		BlackBoard blackBoard = Owner.BlackBoard;
		blackBoard.ActionHandler = (BlackBoard.AgentActionHandler)Delegate.Combine(blackBoard.ActionHandler, new BlackBoard.AgentActionHandler(HandleAction));
	}

	public Transform GetDesiredCameraTransform()
	{
		if (State != null)
		{
			return State.GetDesiredCameraTransform();
		}
		return null;
	}

	private void OnTriggerEnter(Collider other)
	{
	}

	private void OnTriggerExit(Collider other)
	{
	}

	public void Activate(SpawnPoint spawn)
	{
		State = States[E_State.ThirdPerson];
		State.Activate(spawn.Transform);
		WaitForEndOfAction = null;
	}

	public void HandleAction(AgentAction a)
	{
		if (a is AgentActionCoverEnter)
		{
			State.Deactivate();
			State = States[E_State.Cover];
			State.Activate(Owner.Transform);
		}
		else if (a is AgentActionCoverLeave)
		{
			WaitForEndOfAction = a;
		}
	}

	private void Update()
	{
		if (WaitForEndOfAction != null && !WaitForEndOfAction.IsActive())
		{
			State.Deactivate();
			State = States[E_State.ThirdPerson];
			State.Activate(Owner.Transform);
			WaitForEndOfAction = null;
		}
	}
}
