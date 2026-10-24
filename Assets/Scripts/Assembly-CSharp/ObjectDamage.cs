using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ObjectDamage : MonoBehaviour, IGameZoneControledObject
{
	public float Damage = 10f;

	public float NextDamageDelay = 1f;

	public List<OnGameEvent> OnGameEvents = new List<OnGameEvent>();

	public Vector3 Size;

	private Bounds Bounds = default(Bounds);

	public List<AudioClip> SoundHit = new List<AudioClip>();

	public float SoundRange = 2f;

	private Transform Transform;

	private AudioSource Audio;

	private bool AllEventsAreOn;

	private float TestTime;

	private void Awake()
	{
		Transform = base.transform;
		Audio = base.GetComponent<AudioSource>();
		Bounds.size = Size;
		GameZone firstComponentUpward = base.gameObject.GetFirstComponentUpward<GameZone>();
		firstComponentUpward.RegisterControllableObject(this);
	}

	private void Start()
	{
		Bounds.size = Size;
		foreach (OnGameEvent onGameEvent in OnGameEvents)
		{
			GameBlackboard.Instance.GameEvents.AddEventChangeHandler(onGameEvent.Name, EventHandler);
		}
	}

	private void Update()
	{
		if (!Player.Instance.Owner.IsAlive)
		{
			return;
		}
		Vector3 position = Player.Instance.Owner.Position;
		if (TestTime < Time.timeSinceLevelLoad && AllEventsAreOn && PointInsideObject(position))
		{
			TestTime = Time.timeSinceLevelLoad + NextDamageDelay;
			if (SoundHit.Count > 0)
			{
				Audio.PlayOneShot(SoundHit[Random.Range(0, SoundHit.Count)]);
			}
			Player.Instance.Owner.OnReceiveEnviromentDamage(Damage, Player.Instance.Owner.Forward * -2f);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
		Bounds.size = Size;
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawCube(Vector3.zero, Bounds.size);
	}

	public bool PointInsideObject(Vector3 point)
	{
		Vector3 point2 = Transform.InverseTransformPoint(point);
		return Bounds.Contains(point2);
	}

	public void EventHandler(string name, GameEvents.E_State state)
	{
		AllEventsAreOn = true;
		foreach (OnGameEvent onGameEvent in OnGameEvents)
		{
			if (GameBlackboard.Instance.GameEvents.GetState(onGameEvent.Name) != onGameEvent.State)
			{
				AllEventsAreOn = false;
				break;
			}
		}
	}

	public bool IsEnabled()
	{
		return AllEventsAreOn;
	}

	public void Enable()
	{
		AllEventsAreOn = true;
		foreach (OnGameEvent onGameEvent in OnGameEvents)
		{
			if (GameBlackboard.Instance.GameEvents.GetState(onGameEvent.Name) != onGameEvent.State)
			{
				AllEventsAreOn = false;
				break;
			}
		}
	}

	public void Reset()
	{
		TestTime = 0f;
		AllEventsAreOn = false;
	}

	public void Disable()
	{
		TestTime = 0f;
		AllEventsAreOn = false;
	}
}
