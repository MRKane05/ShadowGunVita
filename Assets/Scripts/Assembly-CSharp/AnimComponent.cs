using System;
using UnityEngine;

public class AnimComponent : MonoBehaviour
{
	private AnimFSM FSM;

	private Animation Animation;

	private AgentHuman Owner;

	private Vector3 RootPosition;

	private Transform ContactPlatfrom;

	private Vector3 ContactPoint;

	public E_AnimFSMTypes TypeOfFSM;

	public AnimState CurrentAnimState
	{
		get
		{
			return (FSM == null) ? null : FSM.CurrentAnimState;
		}
	}

	public void Awake()
	{
		Owner = GetComponent<AgentHuman>();
		Animation = base.GetComponent<Animation>();
		switch (TypeOfFSM)
		{
		case E_AnimFSMTypes.Player:
			FSM = new AnimFSMPlayer(Animation, Owner);
			break;
		case E_AnimFSMTypes.Mutant:
			FSM = new AnimFSMMutant(Animation, Owner);
			break;
		case E_AnimFSMTypes.Heavy:
			FSM = new AnimFSMMutantHeavy(Animation, Owner);
			break;
		case E_AnimFSMTypes.Shield:
			FSM = new AnimFSMMutantShield(Animation, Owner);
			break;
		case E_AnimFSMTypes.Elite:
			FSM = new AnimFSMMutantElite(Animation, Owner);
			break;
		case E_AnimFSMTypes.Kamikaze:
			FSM = new AnimFSMMutantKamikaze(Animation, Owner);
			break;
		default:
			Debug.LogError(base.name + " unkown type of FSM");
			break;
		}
		FSM.Initialize();
		base.enabled = false;
	}

	private void Start()
	{
		BlackBoard blackBoard = Owner.BlackBoard;
		blackBoard.ActionHandler = (BlackBoard.AgentActionHandler)Delegate.Combine(blackBoard.ActionHandler, new BlackBoard.AgentActionHandler(HandleAction));
	}

	private void Update()
	{
		if ((bool)ContactPlatfrom)
		{
			Vector3 vector = ContactPlatfrom.position - ContactPoint;
			ContactPlatfrom = null;
			if ((bool)Owner.CharacterController)
			{
				Owner.CharacterController.Move(vector);
			}
			else
			{
				Owner.Transform.position += vector;
			}
		}
		else if ((bool)Owner.CharacterController && Owner.CharacterController.enabled)
		{
			Owner.CharacterController.Move(Vector3.up * 0.1f);
			Owner.CharacterController.Move(Vector3.down * 0.1f);
		}
		if ((bool)Owner.CharacterController)
		{
		}
		FSM.UpdateAnimStates();
	}

	private void LateUpdate()
	{
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (Owner.IsInCover || Owner.IsEnteringToCover)
		{
			ContactPlatfrom = null;
		}
		else if ((bool)hit.rigidbody && hit.rigidbody.isKinematic && hit.gameObject.GetComponent<Projectile>() == null && hit.gameObject.name != "laser")
		{
			ContactPlatfrom = hit.transform;
			ContactPoint = ContactPlatfrom.position;
		}
	}

	public void HandleAction(AgentAction action)
	{
		if (base.enabled && !action.IsFailed() && !FSM.DoAction(action))
		{
			action.SetFailed();
		}
	}

	public void Activate(SpawnPoint spawn)
	{
		base.enabled = true;
		FSM.Activate();
	}

	public void Deactivate()
	{
		Animation.Stop();
		Animation.Rewind();
		FSM.Reset();
		base.enabled = false;
		ContactPlatfrom = null;
	}

	public void OnTeleport()
	{
		Animation.Stop();
		FSM.Reset();
		FSM.Activate();
	}

	private void HandleAnimationEvent(AnimationEvent animEvent)
	{
		FSM.CurrentAnimState.HandleAnimationEvent((AnimState.E_AnimEvent)animEvent.intParameter);
	}
}
