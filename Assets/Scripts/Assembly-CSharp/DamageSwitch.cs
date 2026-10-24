using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Interaction/Damage Switch")]
[RequireComponent(typeof(BoxCollider))]
public class DamageSwitch : MonoBehaviour, IGameZoneControledObject
{
	public enum E_State
	{
		On = 0,
		Off = 1
	}

	public List<GameEvent> SetGameEventsOn = new List<GameEvent>();

	public List<GameEvent> SetGameEventsOff = new List<GameEvent>();

	public List<OnGameEvent> OnGameEvents = new List<OnGameEvent>();

	public float MaxHealth;

	public float SwitchDelay;

	public E_State DefaultState;

	public GameObject Icon;

	private GameObject GameObject;

	private Collider Collider;

	public float Health { get; private set; }

	public E_State State { get; private set; }

	private void Awake()
	{
		GameObject = base.gameObject;
		Collider = GameObject.GetComponent<Collider>();
		GameZone firstComponentUpward = GameObject.GetFirstComponentUpward<GameZone>();
		firstComponentUpward.RegisterControllableObject(this);
		Reset();
	}

	public void Enable()
	{
		if ((bool)Icon)
		{
			Icon.SetActiveRecursively(true);
		}
		Collider.enabled = true;
	}

	public void Reset()
	{
		Health = MaxHealth;
		State = DefaultState;
		StopAllCoroutines();
	}

	public void Disable()
	{
		if ((bool)Icon)
		{
			Icon.SetActiveRecursively(false);
		}
		Collider.enabled = false;
	}

	private void OnDrawGizmos()
	{
	}

	public void OnProjectileHit(Projectile p)
	{
		Health -= p.Damage;
		if (Health <= 0f)
		{
			if (State == E_State.Off)
			{
				StartCoroutine(SwitchToOn());
			}
			else
			{
				StartCoroutine(SwitchToOff());
			}
		}
	}

	private IEnumerator SwitchToOff()
	{
		foreach (GameEvent gameEvent in SetGameEventsOff)
		{
			Mission.Instance.SendGameEvent(gameEvent.Name, gameEvent.State, gameEvent.Delay);
		}
		Disable();
		yield return new WaitForSeconds(SwitchDelay);
		State = E_State.Off;
		Health = MaxHealth;
		Enable();
	}

	private IEnumerator SwitchToOn()
	{
		foreach (GameEvent gameEvent in SetGameEventsOn)
		{
			Mission.Instance.SendGameEvent(gameEvent.Name, gameEvent.State, gameEvent.Delay);
		}
		Disable();
		yield return new WaitForSeconds(SwitchDelay);
		State = E_State.On;
		Health = MaxHealth;
		Enable();
	}
}
