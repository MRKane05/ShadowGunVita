using System.Collections.Generic;
using UnityEngine;

public abstract class InteractionObject : MonoBehaviour
{
	public GameObject Icon;

	public Transform EntryTransform;

	public AnimationClip UserAnimationClip;

	public static int UseLayer = 16;

	protected Animation Animation;

	protected Transform Transform;

	private bool _InteractionObjectUsable;

	protected bool AllEventsAreOn;

	public List<OnGameEvent> OnGameEvents = new List<OnGameEvent>();

	private float EndOfInteraction;

	public bool InteractionObjectUsable
	{
		get
		{
			return _InteractionObjectUsable;
		}
		protected set
		{
			_InteractionObjectUsable = value;
			if (!_InteractionObjectUsable)
			{
				Icon.SetActiveRecursively(false);
			}
		}
	}

	public bool DisableDuringFight { get; protected set; }

	public Vector3 Position
	{
		get
		{
			return (!EntryTransform) ? Transform.position : EntryTransform.position;
		}
	}

	public bool IsActive
	{
		get
		{
			return InteractionObjectUsable && AllEventsAreOn && IsEnabled;
		}
	}

	public bool IsEnabled { get; protected set; }

	public virtual bool IsInteractionFinished
	{
		get
		{
			return EndOfInteraction < Time.timeSinceLevelLoad;
		}
		protected set
		{
		}
	}

	public virtual float UseTime
	{
		get
		{
			return UserAnimationClip.length;
		}
	}

	public virtual Transform GetEntryTransform()
	{
		return EntryTransform;
	}

	public string GetUserAnimation()
	{
		return UserAnimationClip.name;
	}

	public void Initialize()
	{
		InteractionObjectUsable = true;
		DisableDuringFight = true;
		Transform = base.transform;
		foreach (OnGameEvent onGameEvent in OnGameEvents)
		{
			GameBlackboard.Instance.GameEvents.AddEventChangeHandler(onGameEvent.Name, EventHandler);
		}
		if (Icon.GetComponent<Collider>() == null)
		{
			SphereCollider sphereCollider = Icon.AddComponent<SphereCollider>();
			sphereCollider.gameObject.layer = UseLayer;
			sphereCollider.radius *= 1.5f;
		}
	}

	private void OnDestroy()
	{
		UserAnimationClip = null;
	}

	public virtual void Enable()
	{
		IsEnabled = true;
		AllEventsAreOn = true;
		foreach (OnGameEvent onGameEvent in OnGameEvents)
		{
			if (GameBlackboard.Instance.GameEvents.GetState(onGameEvent.Name) != onGameEvent.State)
			{
				AllEventsAreOn = false;
				break;
			}
		}
		Icon.SetActiveRecursively(IsActive);
	}

	public virtual void Disable()
	{
		Icon.SetActiveRecursively(false);
		IsEnabled = false;
	}

	public virtual void DoInteraction()
	{
		EndOfInteraction = UserAnimationClip.length + Time.timeSinceLevelLoad - 0.3f;
	}

	public virtual void Reset()
	{
		InteractionObjectUsable = true;
	}

	public void EventHandler(string name, GameEvents.E_State state)
	{
		if (!InteractionObjectUsable || !base.enabled)
		{
			return;
		}
		AllEventsAreOn = true;
		foreach (OnGameEvent onGameEvent in OnGameEvents)
		{
			if (GameBlackboard.Instance.GameEvents.GetState(onGameEvent.Name) != onGameEvent.State)
			{
				AllEventsAreOn = false;
				return;
			}
		}
		if (IsActive)
		{
			Icon.SetActiveRecursively(true);
		}
		else
		{
			Icon.SetActiveRecursively(false);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawIcon(base.transform.position, "InteractionUse.tif");
	}
}
