using UnityEngine;
using UnityEngine.AI;

public class AnimState
{
	public enum E_AnimEvent
	{
		Loop = 0
	}

	protected Animation Animation;

	private bool m_Finished = true;

	protected AgentHuman Owner;

	protected Transform Transform;

	protected Transform RootTransform;

	protected Transform Stomach;

	private float PlayInjuryTime;

	public AnimState(Animation anims, AgentHuman owner)
	{
		Animation = anims;
		Owner = owner;
		Transform = Owner.transform;
		RootTransform = Transform.Find("root");
		Stomach = Transform.Find("pelvis/stomach");
	}

	public virtual void OnActivate(AgentAction action)
	{
		SetFinished(false);
		Initialize(action);
	}

	public virtual void OnDeactivate()
	{
	}

	public virtual void Reset()
	{
	}

	public virtual void Release()
	{
		SetFinished(true);
	}

	public virtual bool HandleNewAction(AgentAction action)
	{
		return false;
	}

	public virtual void Update()
	{
	}

	public virtual bool IsFinished()
	{
		return m_Finished;
	}

	public virtual void SetFinished(bool finished)
	{
		m_Finished = finished;
	}

	public virtual void HandleAnimationEvent(E_AnimEvent animEvent)
	{
	}

	protected virtual void Initialize(AgentAction action)
	{
	}

	protected virtual bool SetTargetLocation(Vector3 pos)
	{
		return Owner.NavMeshAgent.SetDestination(pos);
	}

	protected virtual bool Move(Vector3 velocity, bool slide = true)
	{
		if (Owner.CharacterController == null)
		{
			return false;
		}
		if (!Owner.CharacterController.isGrounded)
		{
			velocity.y -= 30f * Time.deltaTime;
		}
		Owner.CharacterController.Move(velocity);
		return true;
	}

	protected virtual bool MoveEx(Vector3 velocity)
	{
		if (Owner.CharacterController == null)
		{
			return false;
		}
		Vector3 position = Transform.position;
		Transform.position += Vector3.up * Time.deltaTime;
		velocity.y -= 9f * Time.deltaTime;
		RaycastHit hitInfo;
		if (Owner.CharacterController.Move(velocity) == CollisionFlags.None && !Physics.Raycast(Transform.position, -Vector3.up, out hitInfo, 3f, 8192))
		{
			Transform.position = position;
			return false;
		}
		return true;
	}

	protected bool IsGroundThere(Vector3 pos)
	{
		return Physics.Raycast(pos + Vector3.up, -Vector3.up, 5f, 16384);
	}

	protected void CrossFade(string anim, float fadeInTime, PlayMode mode)
	{
		if (Animation.IsPlaying(anim))
		{
			Animation.CrossFadeQueued(anim, fadeInTime, QueueMode.PlayNow);
		}
		else
		{
			Animation.CrossFade(anim, fadeInTime, mode);
		}
	}

	protected void Blend(string anim, float fadeInTime)
	{
		Animation.Blend(anim, 1f, fadeInTime);
	}

	protected void Blend(string anim, float weight, float fadeInTime)
	{
		Animation.Blend(anim, weight, fadeInTime);
	}

	protected void PlayInjuryAnimation(AgentActionInjury action)
	{
		if (PlayInjuryTime > Time.timeSinceLevelLoad)
		{
			action.SetSuccess();
			return;
		}
		string injuryAnim = Owner.AnimSet.GetInjuryAnim(action.FromWeapon);
		Animation[injuryAnim].blendMode = AnimationBlendMode.Blend;
		Animation[injuryAnim].layer = 0;
		Animation[injuryAnim].AddMixingTransform(Stomach);
		if (Owner.BlackBoard.MotionType == E_MotionType.None)
		{
			Animation[injuryAnim].RemoveMixingTransform(Stomach);
		}
		Blend(injuryAnim, 1f, 0.1f);
		PlayInjuryTime = Time.timeSinceLevelLoad + Animation[injuryAnim].length * 0.35f;
		action.SetSuccess();
	}
}
