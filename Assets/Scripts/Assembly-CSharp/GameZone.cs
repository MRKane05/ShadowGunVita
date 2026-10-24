using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class GameZone : GameZoneBaze
{
	public enum E_State
	{
		E_WAITING_FOR_START = 0,
		E_IN_PROGRESS = 1
	}

	private E_State State;

	private SpawnZoneBase[] SpawnZones = new SpawnZoneBase[0];

	private InteractionObject[] InteractionObjects = new InteractionObject[0];

	private BreakableObject[] BreakableObjects = new BreakableObject[0];

	private DestroyObject[] DestroyObjects = new DestroyObject[0];

	private TriggerGameEvent[] GameEventTriggers = new TriggerGameEvent[0];

	private InteractionTarget[] InteractionTargets = new InteractionTarget[0];

	private List<IGameZoneControledObject> SpecialObjects = new List<IGameZoneControledObject>();

	private float TimeToUpdate;

	private void Awake()
	{
		GameObject = base.gameObject;
		InteractionObjects = GameObject.GetComponentsInChildren<InteractionObject>();
		BreakableObjects = GameObject.GetComponentsInChildren<BreakableObject>();
		SpawnZones = GameObject.GetComponentsInChildren<SpawnZoneBase>();
		GameEventTriggers = GameObject.GetComponentsInChildren<TriggerGameEvent>();
		InteractionTargets = GameObject.GetComponentsInChildren<InteractionTarget>();
		DestroyObjects = GameObject.GetComponentsInChildren<DestroyObject>();
		Initialize();
	}

	private void Start()
	{
		TimeToUpdate = Time.timeSinceLevelLoad + 0.2f;
	}

	public override void Enable()
	{
		Mission.Instance.CurrentGameZone = this;
		DictionaryFile df = new DictionaryFile();
		Game.Instance.Save_Save(ref df);
		Mission.Instance.Save_Save(ref df);
		df.Save(Game.Instance.GetSaveFileName());
		Mission.Instance.LockPrevGameZone();
		if ((bool)GuiHUD.Instance && GuiHUD.Instance.m_ShowingMessage != GuiHUD.E_HudMessageType.E_001_GameLoaded)
		{
			GuiHUD.Instance.ShowMessageTimed(GuiHUD.E_HudMessageType.E_002_Checkpoint, GuiHUD.InfoMessageTime);
		}
		base.Enable();
		State = E_State.E_IN_PROGRESS;
		SpawnZoneBase[] spawnZones = SpawnZones;
		foreach (SpawnZoneBase spawnZoneBase in spawnZones)
		{
			spawnZoneBase.Enable();
		}
		InteractionObject[] interactionObjects = InteractionObjects;
		foreach (InteractionObject interactionObject in interactionObjects)
		{
			interactionObject.Enable();
		}
		BreakableObject[] breakableObjects = BreakableObjects;
		foreach (BreakableObject breakableObject in breakableObjects)
		{
			breakableObject.Enable();
		}
		DestroyObject[] destroyObjects = DestroyObjects;
		foreach (DestroyObject destroyObject in destroyObjects)
		{
			destroyObject.Enable();
		}
		TriggerGameEvent[] gameEventTriggers = GameEventTriggers;
		foreach (TriggerGameEvent triggerGameEvent in gameEventTriggers)
		{
			triggerGameEvent.Enable();
		}
		foreach (IGameZoneControledObject specialObject in SpecialObjects)
		{
			specialObject.Enable();
		}
		InteractionTarget[] interactionTargets = InteractionTargets;
		foreach (InteractionTarget interactionTarget in interactionTargets)
		{
			interactionTarget.Enable();
		}
	}

	public override void Disable()
	{
		SpawnZoneBase[] spawnZones = SpawnZones;
		foreach (SpawnZoneBase spawnZoneBase in spawnZones)
		{
			spawnZoneBase.Disable();
		}
		InteractionObject[] interactionObjects = InteractionObjects;
		foreach (InteractionObject interactionObject in interactionObjects)
		{
			interactionObject.Disable();
		}
		BreakableObject[] breakableObjects = BreakableObjects;
		foreach (BreakableObject breakableObject in breakableObjects)
		{
			breakableObject.Disable();
		}
		DestroyObject[] destroyObjects = DestroyObjects;
		foreach (DestroyObject destroyObject in destroyObjects)
		{
			destroyObject.Disable();
		}
		TriggerGameEvent[] gameEventTriggers = GameEventTriggers;
		foreach (TriggerGameEvent triggerGameEvent in gameEventTriggers)
		{
			triggerGameEvent.Disable();
		}
		foreach (IGameZoneControledObject specialObject in SpecialObjects)
		{
			specialObject.Disable();
		}
		InteractionTarget[] interactionTargets = InteractionTargets;
		foreach (InteractionTarget interactionTarget in interactionTargets)
		{
			interactionTarget.Disable();
		}
		base.Disable();
	}

	public override void Reset()
	{
		State = E_State.E_WAITING_FOR_START;
		base.Reset();
		StopAllCoroutines();
		SpawnZoneBase[] spawnZones = SpawnZones;
		foreach (SpawnZoneBase spawnZoneBase in spawnZones)
		{
			spawnZoneBase.Reset();
		}
		InteractionObject[] interactionObjects = InteractionObjects;
		foreach (InteractionObject interactionObject in interactionObjects)
		{
			interactionObject.Reset();
		}
		BreakableObject[] breakableObjects = BreakableObjects;
		foreach (BreakableObject breakableObject in breakableObjects)
		{
			breakableObject.Reset();
		}
		DestroyObject[] destroyObjects = DestroyObjects;
		foreach (DestroyObject destroyObject in destroyObjects)
		{
			destroyObject.Reset();
		}
		TriggerGameEvent[] gameEventTriggers = GameEventTriggers;
		foreach (TriggerGameEvent triggerGameEvent in gameEventTriggers)
		{
			triggerGameEvent.Reset();
		}
		foreach (IGameZoneControledObject specialObject in SpecialObjects)
		{
			specialObject.Reset();
		}
		InteractionTarget[] interactionTargets = InteractionTargets;
		foreach (InteractionTarget interactionTarget in interactionTargets)
		{
			interactionTarget.Reset();
		}
	}

	public void RegisterControllableObject(IGameZoneControledObject inObject)
	{
		if (!SpecialObjects.Contains(inObject))
		{
			SpecialObjects.Add(inObject);
		}
	}

	public void UnRegisterControllableObject(IGameZoneControledObject inObject)
	{
		SpecialObjects.Remove(inObject);
	}

	private void OnDrawGizmos()
	{
		BoxCollider boxCollider = GetComponent("BoxCollider") as BoxCollider;
		if (boxCollider != null)
		{
			Gizmos.color = Color.blue;
			Gizmos.matrix = boxCollider.transform.localToWorldMatrix;
			Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
			Gizmos.matrix = Matrix4x4.identity;
		}
	}

	private void OnDrawGizmosSelected()
	{
		BoxCollider boxCollider = GetComponent("BoxCollider") as BoxCollider;
		if (boxCollider != null)
		{
			Gizmos.color = Color.white;
			Gizmos.matrix = boxCollider.transform.localToWorldMatrix;
			Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
			Gizmos.matrix = Matrix4x4.identity;
		}
	}

	private void Update()
	{
		if (base.Enemies.Count == 0 || TimeToUpdate > Time.timeSinceLevelLoad)
		{
			return;
		}
		for (int i = 0; i < base.Enemies.Count; i++)
		{
			if (!base.Enemies[i].IsAlive)
			{
				base.Enemies.RemoveAt(i);
				break;
			}
		}
		if (base.Enemies.Count == 0)
		{
			EnableAllActiveInteractionInFight(true);
		}
		TimeToUpdate = Time.timeSinceLevelLoad + 0.2f;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (State == E_State.E_WAITING_FOR_START && !(other != Player.Instance.Owner.CharacterController))
		{
			Enable();
		}
	}

	private void OnTriggerExit(Collider other)
	{
	}

	public void EnableAllActiveInteractionInFight(bool enable)
	{
		for (int num = InteractionObjects.Length - 1; num >= 0; num--)
		{
			InteractionObject interactionObject = InteractionObjects[num];
			if (interactionObject.InteractionObjectUsable && interactionObject.DisableDuringFight)
			{
				if (enable)
				{
					interactionObject.Enable();
				}
				else
				{
					interactionObject.Disable();
				}
			}
		}
	}

	public override InteractionObject GetNearestInteractionObject(Vector3 center, float maxLen)
	{
		float num = maxLen * maxLen;
		InteractionObject result = null;
		for (int num2 = InteractionObjects.Length - 1; num2 >= 0; num2--)
		{
			InteractionObject interactionObject = InteractionObjects[num2];
			if (interactionObject.IsActive)
			{
				float sqrMagnitude = (interactionObject.Position - center).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					result = interactionObject;
				}
			}
		}
		return result;
	}

	public override bool IsInteractionObjectInRange(Vector3 center, float maxLen)
	{
		float num = maxLen * maxLen;
		for (int num2 = InteractionObjects.Length - 1; num2 >= 0; num2--)
		{
			InteractionObject interactionObject = InteractionObjects[num2];
			if (interactionObject.IsActive && (interactionObject.Position - center).sqrMagnitude < num)
			{
				return true;
			}
		}
		return false;
	}

	public override InteractionObject InteractionObjectWithIcon(GameObject Icon)
	{
		InteractionObject[] interactionObjects = InteractionObjects;
		foreach (InteractionObject interactionObject in interactionObjects)
		{
			if (interactionObject.IsActive && interactionObject.Icon == Icon)
			{
				return interactionObject;
			}
		}
		return null;
	}
}
