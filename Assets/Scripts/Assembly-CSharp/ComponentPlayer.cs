using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AnimSetPlayer))]
[RequireComponent(typeof(AgentHuman))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animation))]
[RequireComponent(typeof(ComponentSensors))]
[RequireComponent(typeof(CameraBehaviour))]
[RequireComponent(typeof(AnimComponent))]
[RequireComponent(typeof(ComponentWeaponsPlayer))]
public class ComponentPlayer : MonoBehaviour
{
	private enum E_TouchCommandType
	{
		E_TC_UNKNOWN = 0,
		E_TC_TAP = 1,
		E_TC_DOUBLE_TAP = 2,
		E_TC_LEFT_TO_RIGHT = 3,
		E_TC_RIGHT_TO_LEFT = 4,
		E_TC_UP = 5,
		E_TC_DOWN = 6,
		E_TC_TOUCH_START = 7,
		E_TC_MOVING = 8
	}

	[Serializable]
	public class SoundInfo
	{
		public AudioClip[] TakeAmmo = new AudioClip[0];

		public AudioClip TakeAmmoBox = null;

		public AudioClip Roll = null;
	}

	public SoundInfo SoundSetup = new SoundInfo();

	public float HeightStand = 1.72f;

	public float HeightCover = 1.3f;

	public float HeightRoll = 1f;

	private int Experience;

	private float LastInjuryTime;

	private float NextUpdateTime;

	private float SendStepEventTime;

	private bool UseMode;

	private AgentActionRoll AgentActionRoll;

	public PlayerControlStates Controls = new PlayerControlStates();

	public AudioClip SoundTakeAmmo
	{
		get
		{
			if (SoundSetup.TakeAmmo.Length == 0)
			{
				return null;
			}
			return SoundSetup.TakeAmmo[Random.Range(0, SoundSetup.TakeAmmo.Length)];
		}
	}

	public AudioClip SoundTakeAmmoFromBox
	{
		get
		{
			return SoundSetup.TakeAmmoBox;
		}
	}

	public AudioClip SoundRoll
	{
		get
		{
			return SoundSetup.Roll;
		}
	}

	public AgentHuman Owner { get; private set; }

	public bool InUseMode
	{
		get
		{
			return UseMode;
		}
	}

	private void Awake()
	{
		Player.Instance = this;
		Owner = GetComponent<AgentHuman>();
		Controls.Start();
		Owner.BlackBoard.AimAnimationsEnabled = true;
	}

	private void OnDestroy()
	{
		SoundSetup = null;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void Start()
	{
		Owner.BlackBoard.IsPlayer = true;
		Owner.BlackBoard.Rage = 0f;
		Owner.BlackBoard.Dodge = 0f;
		Owner.BlackBoard.Fear = 0f;
		Owner.SensorsComponent.AddSensor(E_SensorType.CoverPlayer, true);
		Owner.AddGOAPAction(E_GOAPAction.Goto);
		Owner.AddGOAPAction(E_GOAPAction.Move);
		Owner.AddGOAPAction(E_GOAPAction.CoverEnter);
		Owner.AddGOAPAction(E_GOAPAction.CoverMove);
		Owner.AddGOAPAction(E_GOAPAction.CoverLeave);
		Owner.AddGOAPAction(E_GOAPAction.CoverFire);
		Owner.AddGOAPAction(E_GOAPAction.CoverJumpOverPlayer);
		Owner.AddGOAPAction(E_GOAPAction.CoverLeaveRightLeft);
		Owner.AddGOAPAction(E_GOAPAction.Use);
		Owner.AddGOAPAction(E_GOAPAction.WeaponReload);
		Owner.AddGOAPAction(E_GOAPAction.WeaponChange);
		Owner.AddGOAPAction(E_GOAPAction.Roll);
		Owner.AddGOAPGoal(E_GOAPGoals.Move);
		Owner.AddGOAPGoal(E_GOAPGoals.CoverEnter);
		Owner.AddGOAPGoal(E_GOAPGoals.CoverLeave);
		Owner.AddGOAPGoal(E_GOAPGoals.CoverFire);
		Owner.AddGOAPGoal(E_GOAPGoals.CoverMove);
		Owner.AddGOAPGoal(E_GOAPGoals.UseWorldObject);
		Owner.AddGOAPGoal(E_GOAPGoals.WeaponReload);
		Owner.AddGOAPGoal(E_GOAPGoals.WeaponChange);
		Owner.AddGOAPGoal(E_GOAPGoals.Roll);
		Owner.InitializeGOAP();
		Controls.FireDownDelegate = ActionBeginFire;
		Controls.FireUpDelegate = ActionEndFire;
		Controls.UseDelegate = ActionUse;
		Controls.UseObjectDelegate = ActionUseObject;
		Controls.ReloadDelegate = ActionReload;
		Controls.RollDelegate = ActionRoll;
		Controls.ChangeWeaponDelegate = ActionChangeWeapon;
		BlackBoard blackBoard = Owner.BlackBoard;
		blackBoard.ActionHandler = (BlackBoard.AgentActionHandler)Delegate.Combine(blackBoard.ActionHandler, new BlackBoard.AgentActionHandler(HandleAction));
	}

	private void Activate(SpawnPoint spawn)
	{
		Owner.CharacterController.center = new Vector3(0f, HeightStand * 0.5f + 0.08f, 0f);
		Owner.CharacterController.height = HeightStand;
		AgentActionRoll = null;
		Controls.SwitchToCombatMode();
		NextUpdateTime = 0f;
	}

	private void Deactivate()
	{
		UseMode = false;
		AgentActionRoll = null;
	}

	private void Update()
	{
		if (NextUpdateTime < Time.timeSinceLevelLoad)
		{
			SlowUpdate();
			NextUpdateTime = Time.timeSinceLevelLoad + 0.1f;
		}
		if (Owner.BlackBoard.Stop)
		{
			Controls.Update();
			return;
		}
		if (AgentActionRoll != null && !AgentActionRoll.IsActive())
		{
			Owner.CharacterController.center = new Vector3(0f, HeightStand * 0.5f + 0.08f, 0f);
			Owner.CharacterController.height = HeightStand;
			AgentActionRoll = null;
		}
		Controls.Update();
		if (!Owner.IsAlive || Camera.main == null)
		{
			return;
		}
		if (Controls.Use)
		{
			CreateOrderUse();
		}
		if (Controls.Move.Enabled && Controls.Move.Direction != Vector3.zero)
		{
			bool flag = true;
			if (Owner.IsEnteringToCover)
			{
				flag = false;
			}
			else if (Owner.IsInCover)
			{
				float num = Vector3.Dot(Owner.BlackBoard.Cover.Right, Controls.Move.Direction);
				float num2 = Vector3.Dot(Owner.BlackBoard.Cover.Forward, Controls.Move.Direction);
				flag = false;
				if (num2 > 0.75f)
				{
					flag = true;
				}
				else if (num2 < -0.75f)
				{
					flag = true;
				}
				else if (num > 0f)
				{
					Vector3 rightEdge = Owner.BlackBoard.Cover.RightEdge;
					rightEdge.y = Owner.Position.y;
					if ((Owner.Position - rightEdge).magnitude > 0.01f)
					{
						flag = true;
					}
				}
				else if (num < 0f)
				{
					Vector3 leftEdge = Owner.BlackBoard.Cover.LeftEdge;
					leftEdge.y = Owner.Position.y;
					if ((Owner.Position - leftEdge).magnitude > 0.01f)
					{
						flag = true;
					}
				}
			}
			else if (Owner.BlackBoard.MotionType == E_MotionType.Roll)
			{
				flag = false;
			}
			if (flag)
			{
				Owner.BlackBoard.Desires.MoveDirection = Controls.Move.Direction;
				Owner.BlackBoard.Desires.MoveSpeedModifier = Controls.Move.Force;
				Owner.WorldState.SetWSProperty(E_PropKey.AtTargetPos, false);
			}
			else
			{
				Owner.BlackBoard.Desires.MoveDirection = Vector3.zero;
				Owner.BlackBoard.Desires.MoveSpeedModifier = 0f;
				Owner.WorldState.SetWSProperty(E_PropKey.AtTargetPos, true);
			}
		}
		else
		{
			Owner.BlackBoard.Desires.MoveDirection = Vector3.zero;
			Owner.BlackBoard.Desires.MoveSpeedModifier = 0f;
			Owner.WorldState.SetWSProperty(E_PropKey.AtTargetPos, true);
		}
		if (Owner.BlackBoard.MotionType != E_MotionType.Roll)
		{
			Vector3 vector = new Vector3(Controls.View.PitchAdd, Controls.View.YawAdd);
			if (Owner.BlackBoard.Desires.WeaponTriggerOn || Owner.IsInCover)
			{
				vector *= 0.7f;
			}
			Owner.BlackBoard.Desires.Rotation.eulerAngles += vector;
			ClipRotation();
		}
		UpdateIdealFireDir();
		AmmoBox nearestDroppedAmmoClip = Mission.Instance.CurrentGameZone.GetNearestDroppedAmmoClip(Owner, 0.1f);
		if (nearestDroppedAmmoClip != null)
		{
			Owner.WeaponComponent.AddAmmoToWeapon(nearestDroppedAmmoClip);
		}
	}

	private void SlowUpdate()
	{
		bool useMode = UseMode;
		if (Owner.BlackBoard.ComlinkOn)
		{
			UseMode = false;
		}
		else
		{
			UseMode = Mission.Instance.CurrentGameZone.IsInteractionObjectInRange(Owner.Position, 2f);
		}
		if (UseMode != useMode)
		{
			if (UseMode)
			{
				Controls.SwitchToUseMode();
			}
			else
			{
				Controls.SwitchToCombatMode();
			}
		}
		if (Owner.IsInCover && Owner.BlackBoard.CoverPosition == E_CoverDirection.Middle && !Owner.BlackBoard.Cover.CoverFlags.Get(Cover.E_CoverFlags.UpCrouch))
		{
			GuiHUD.Instance.EnableCrosshair(false);
		}
		else
		{
			GuiHUD.Instance.EnableCrosshair(true);
		}
		if (SendStepEventTime < Time.timeSinceLevelLoad && (Owner.BlackBoard.MotionType == E_MotionType.Run || Owner.BlackBoard.MotionType == E_MotionType.Walk))
		{
			Mission.Instance.CurrentGameZone.SendFactToEnemies(Owner, Owner, E_EventTypes.EnemyStep, 12f, 1f, false);
			SendStepEventTime = Time.timeSinceLevelLoad + 1f;
		}
	}

	private void LateUpdate()
	{
		if ((bool)BloodFXManager.Instance)
		{
			BloodFXManager.Instance.SetHealthNormalized(Owner.BlackBoard.Health / Owner.BlackBoard.RealMaxHealth);
		}
		if (Owner.IsAlive && !(LastInjuryTime + Game.Instance.DontHealTime > Time.timeSinceLevelLoad) && Owner.BlackBoard.Health != Owner.BlackBoard.RealMaxHealth)
		{
			Owner.BlackBoard.Health = Mathf.Min(Owner.BlackBoard.Health + Game.Instance.HealingModificator * Time.deltaTime, Owner.BlackBoard.RealMaxHealth);
		}
	}

	public void LoadAllWeapon()
	{
		Owner.WeaponComponent.AddAmmoToAllWeapon();
	}

	public void AddWeaponToInventory(E_WeaponType weapon)
	{
		Owner.WeaponComponent.AddWeapon(weapon);
	}

	public void UpdateUseModeHACK()
	{
		if (UseMode)
		{
			Controls.SwitchToUseMode();
		}
		else
		{
			Controls.SwitchToCombatMode();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
	}

	public void HandleAction(AgentAction a)
	{
		if (a is AgentActionInjury)
		{
			LastInjuryTime = Time.timeSinceLevelLoad;
		}
		else if (a is AgentActionDeath)
		{
			Mission.Instance.EndOfMissionFail(GuiHUD.E_HudMessageType.E_DEATH);
		}
		else if (a is AgentActionRoll)
		{
			Owner.CharacterController.height = HeightRoll;
			Owner.CharacterController.center = new Vector3(0f, Owner.CharacterController.height * 0.5f, 0f);
			AgentActionRoll = a as AgentActionRoll;
			if ((bool)SoundRoll)
			{
				Owner.SoundPlay(SoundRoll);
			}
		}
		else if (a is AgentActionCoverEnter)
		{
			if ((a as AgentActionCoverEnter).Cover.IsStandAllowed)
			{
				Owner.CharacterController.height = HeightStand;
			}
			else
			{
				Owner.CharacterController.height = HeightCover;
			}
			Owner.CharacterController.center = new Vector3(0f, Owner.CharacterController.height * 0.5f, 0f);
		}
		else if (a is AgentActionCoverTerminated)
		{
			Owner.CharacterController.center = new Vector3(0f, HeightStand * 0.5f + 0.08f, 0f);
			Owner.CharacterController.height = HeightStand;
		}
		else if (a is AgentActionCoverLeave)
		{
			Owner.CharacterController.center = new Vector3(0f, HeightStand * 0.5f + 0.08f, 0f);
			Owner.CharacterController.height = HeightStand;
		}
		else if (a is AgentActionCoverFire)
		{
			if (Owner.BlackBoard.CoverPosition == E_CoverDirection.Middle)
			{
				Owner.CharacterController.height = HeightStand;
				Owner.CharacterController.center = new Vector3(0f, Owner.CharacterController.height * 0.5f, 0f);
			}
			else if (Owner.BlackBoard.CoverPosition == E_CoverDirection.Left)
			{
				Owner.CharacterController.center = new Vector3(-0.4f, Owner.CharacterController.height * 0.5f, 0f);
			}
			else
			{
				Owner.CharacterController.center = new Vector3(0.4f, Owner.CharacterController.height * 0.5f, 0f);
			}
		}
		else if (a is AgentActionCoverFireCancel)
		{
			if (Owner.BlackBoard.CoverPose == E_CoverPose.Crouch)
			{
				Owner.CharacterController.height = HeightCover;
			}
			else
			{
				Owner.CharacterController.height = HeightStand;
			}
			Owner.CharacterController.center = new Vector3(0f, Owner.CharacterController.height * 0.5f, 0f);
		}
	}

	public void CreateOrderUse()
	{
		Owner.BlackBoard.Desires.InteractionObject = Mission.Instance.CurrentGameZone.GetNearestInteractionObject(Owner.Position, 2f);
		if ((bool)Owner.BlackBoard.Desires.InteractionObject)
		{
			Owner.WorldState.SetWSProperty(E_PropKey.UseWorldObject, true);
		}
	}

	public void HandleBadUse()
	{
	}

	public void StopMove(bool stop)
	{
		Owner.Stop(stop);
		if (stop)
		{
			Controls.DisableInput();
		}
		else
		{
			Controls.EnableInput();
		}
		Owner.WorldState.SetWSProperty(E_PropKey.AtTargetPos, true);
		Owner.WorldState.SetWSProperty(E_PropKey.KillTarget, false);
	}

	public void AddExperience(int exp, float scoreModifier)
	{
	}

	private void ActionBeginFire()
	{
		if (!Owner.BlackBoard.Stop)
		{
			if (Owner.CanFire())
			{
				Owner.BlackBoard.Desires.WeaponTriggerOn = true;
			}
			Controls.Fire = true;
		}
	}

	private void ActionEndFire()
	{
		Controls.Fire = false;
		Owner.BlackBoard.Desires.WeaponTriggerOn = false;
	}

	private void ActionUse()
	{
		if (!Owner.BlackBoard.Stop && !Owner.BlackBoard.ComlinkOn)
		{
			Controls.Use = true;
			CreateOrderUse();
		}
	}

	private void ActionUseObject(InteractionObject obj)
	{
		if (!Owner.BlackBoard.Stop && !Owner.BlackBoard.ComlinkOn)
		{
			Controls.Use = true;
			Owner.BlackBoard.Desires.InteractionObject = obj;
			if ((bool)Owner.BlackBoard.Desires.InteractionObject)
			{
				Owner.WorldState.SetWSProperty(E_PropKey.UseWorldObject, true);
			}
		}
	}

	private void ActionReload()
	{
		if (!Owner.BlackBoard.Stop && !Owner.BlackBoard.ComlinkOn && !Owner.WeaponComponent.GetCurrentWeapon().IsFullyLoaded && Owner.WeaponComponent.GetCurrentWeapon().WeaponAmmo > 0)
		{
			Owner.WorldState.SetWSProperty(E_PropKey.WeaponLoaded, false);
		}
	}

	private void ActionRoll()
	{
		if (Owner.BlackBoard.Stop || Owner.BlackBoard.ComlinkOn)
		{
			return;
		}
		Cover coverForPlayer = Mission.Instance.CurrentGameZone.GetCoverForPlayer(Owner.Position, Owner.Forward, 3f);
		if (coverForPlayer != null)
		{
			Owner.BlackBoard.Desires.CoverNear.Cover = coverForPlayer;
			return;
		}
		Vector3 targetPosition;
		switch (Owner.GetMoveDirectionType(Owner.BlackBoard.Desires.MoveDirection))
		{
		case E_Direction.Forward:
			targetPosition = Owner.Position + Owner.Forward * 2f;
			break;
		case E_Direction.Right:
			targetPosition = Owner.Position + Owner.Right * 2f;
			break;
		case E_Direction.Left:
			targetPosition = Owner.Position - Owner.Right * 2f;
			break;
		default:
			targetPosition = Owner.Position - Owner.Forward * 2f;
			break;
		}
		NavMeshHit hit;
		if (!NavMesh.Raycast(Owner.Position, targetPosition, out hit, int.MaxValue))
		{
			Owner.WorldState.SetWSProperty(E_PropKey.InDodge, true);
		}
	}

	public bool CanChangeWeapon()
	{
		if (Owner.BlackBoard.Stop || Owner.BlackBoard.ComlinkOn || Owner.BlackBoard.BusyAction)
		{
			return false;
		}
		return true;
	}

	private void ActionChangeWeapon(E_WeaponType weaponType)
	{
		if (CanChangeWeapon())
		{
			Owner.BlackBoard.Desires.Weapon = weaponType;
			Owner.WorldState.SetWSProperty(E_PropKey.WeaponChange, true);
		}
	}

	public void OnGuiPrepared()
	{
		Controls.EnableInput();
	}

	private void ClipRotation()
	{
		if ((bool)Owner.BlackBoard.Cover)
		{
			Vector3 eulerAngles = Owner.BlackBoard.Cover.Transform.rotation.eulerAngles;
			Quaternion quaternion = new Quaternion(0f - Owner.BlackBoard.Cover.Transform.rotation.x, 0f - Owner.BlackBoard.Cover.Transform.rotation.y, 0f - Owner.BlackBoard.Cover.Transform.rotation.z, Owner.BlackBoard.Cover.Transform.rotation.w);
			Quaternion quaternion2 = quaternion * Owner.BlackBoard.Cover.Transform.rotation;
			Quaternion quaternion3 = new Quaternion(0f - Owner.Transform.rotation.x, 0f - Owner.Transform.rotation.y, 0f - Owner.Transform.rotation.z, Owner.Transform.rotation.w);
			Quaternion quaternion4 = quaternion3 * Owner.BlackBoard.Desires.Rotation;
			Vector3 eulerAngles2 = Owner.BlackBoard.Desires.Rotation.eulerAngles;
			Vector3 eulerAngles3 = Quaternion.FromToRotation(quaternion4 * Vector3.forward, quaternion2 * Vector3.forward).eulerAngles;
			if (Owner.BlackBoard.CoverPosition == E_CoverDirection.Right)
			{
				if (eulerAngles3.x > 180f && eulerAngles3.x < 360f)
				{
					if (360f - eulerAngles3.x > Owner.BlackBoard.CoverSetup.RightMaxDown)
					{
						eulerAngles2.x = eulerAngles.x + Owner.BlackBoard.CoverSetup.RightMaxDown;
					}
				}
				else if (eulerAngles3.x > Owner.BlackBoard.CoverSetup.RightMaxUp)
				{
					eulerAngles2.x = eulerAngles.x - Owner.BlackBoard.CoverSetup.RightMaxUp;
				}
				if (eulerAngles3.y > 180f && eulerAngles3.y < 360f)
				{
					if (360f - eulerAngles3.y > Owner.BlackBoard.CoverSetup.RightMaxRight)
					{
						eulerAngles2.y = eulerAngles.y + Owner.BlackBoard.CoverSetup.RightMaxRight;
					}
				}
				else if (eulerAngles3.y > Owner.BlackBoard.CoverSetup.RightMaxLeft)
				{
					eulerAngles2.y = eulerAngles.y - Owner.BlackBoard.CoverSetup.RightMaxLeft;
				}
			}
			else if (Owner.BlackBoard.CoverPosition == E_CoverDirection.Left)
			{
				if (eulerAngles3.x > 180f && eulerAngles3.x < 360f)
				{
					if (360f - eulerAngles3.x > Owner.BlackBoard.CoverSetup.LeftMaxDown)
					{
						eulerAngles2.x = eulerAngles.x + Owner.BlackBoard.CoverSetup.LeftMaxDown;
					}
				}
				else if (eulerAngles3.x > Owner.BlackBoard.CoverSetup.LeftMaxUp)
				{
					eulerAngles2.x = eulerAngles.x - Owner.BlackBoard.CoverSetup.LeftMaxUp;
				}
				if (eulerAngles3.y > 180f && eulerAngles3.y < 360f)
				{
					if (360f - eulerAngles3.y > Owner.BlackBoard.CoverSetup.LeftMaxRight)
					{
						eulerAngles2.y = eulerAngles.y + Owner.BlackBoard.CoverSetup.LeftMaxRight;
					}
				}
				else if (eulerAngles3.y > Owner.BlackBoard.CoverSetup.LeftMaxLeft)
				{
					eulerAngles2.y = eulerAngles.y - Owner.BlackBoard.CoverSetup.LeftMaxLeft;
				}
			}
			else
			{
				if (eulerAngles3.x > 180f && eulerAngles3.x < 360f)
				{
					if (360f - eulerAngles3.x > Owner.BlackBoard.CoverSetup.CenterMaxDown)
					{
						eulerAngles2.x = eulerAngles.x + Owner.BlackBoard.CoverSetup.CenterMaxDown;
					}
				}
				else if (eulerAngles3.x > Owner.BlackBoard.CoverSetup.CenterMaxUp)
				{
					eulerAngles2.x = eulerAngles.x - Owner.BlackBoard.CoverSetup.CenterMaxUp;
				}
				if (eulerAngles3.y > 180f && eulerAngles3.y < 360f)
				{
					if (360f - eulerAngles3.y > Owner.BlackBoard.CoverSetup.CenterMaxRight)
					{
						eulerAngles2.y = eulerAngles.y + Owner.BlackBoard.CoverSetup.CenterMaxRight;
					}
				}
				else if (eulerAngles3.y > Owner.BlackBoard.CoverSetup.CenterMaxLeft)
				{
					eulerAngles2.y = eulerAngles.y - Owner.BlackBoard.CoverSetup.CenterMaxLeft;
				}
			}
			Owner.BlackBoard.Desires.Rotation.eulerAngles = eulerAngles2;
		}
		else
		{
			Vector3 eulerAngles4 = Owner.BlackBoard.Desires.Rotation.eulerAngles;
			if (eulerAngles4.x >= 180f)
			{
				eulerAngles4.x -= 360f;
			}
			if (eulerAngles4.x < -60f)
			{
				eulerAngles4.x = -60f;
			}
			else if (eulerAngles4.x > 50f)
			{
				eulerAngles4.x = 50f;
			}
			Owner.BlackBoard.Desires.Rotation.eulerAngles = eulerAngles4;
		}
	}

	private void UpdateIdealFireDir()
	{
		if (Owner.IsInCover)
		{
			Owner.BlackBoard.Desires.FireDirection = GameCamera.Instance.CameraForward;
		}
		else
		{
			Owner.BlackBoard.Desires.FireDirection = Owner.BlackBoard.Desires.Rotation * Vector3.forward;
		}
		if (Owner.WeaponComponent.GetCurrentWeapon() == null)
		{
			return;
		}
		Vector3 shotPos = Owner.WeaponComponent.GetCurrentWeapon().ShotPos;
		RaycastHit[] array = Physics.RaycastAll(Camera.main.transform.position, Owner.BlackBoard.Desires.FireDirection, 60f);
		if (array.Length > 1)
		{
			Array.Sort(array, CollisionUtils.CompareHits);
		}
		Vector3 vector = shotPos + Owner.BlackBoard.Desires.FireDirection * 30f;
		RaycastHit[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			RaycastHit raycastHit = array2[i];
			if (!(raycastHit.transform == Owner.Transform) && !raycastHit.collider.isTrigger)
			{
				vector = raycastHit.point;
				break;
			}
		}
		if (Owner.WeaponComponent.CurrentWeapon == E_WeaponType.GrenadeLauncher)
		{
			Vector3 vector2 = vector - Owner.WeaponComponent.GetCurrentWeapon().ShotPos;
			float num = Mathf.Max(1f, vector2.magnitude - 1f);
			vector2.Normalize();
			float speed = Owner.WeaponComponent.GetCurrentWeapon().Speed;
			float num2 = 0.5f * Mathf.Asin(num * (0f - Physics.gravity.y) / (speed * speed));
			if (float.IsNaN(num2))
			{
				num2 = (float)Math.PI / 6f;
			}
			num2 += Mathf.Atan2(vector2.y, Mathf.Sqrt(vector2.x * vector2.x + vector2.z * vector2.z));
			num2 *= 57.29578f;
			float y = Mathf.Atan2(vector2.x, vector2.z) * 57.29578f;
			Vector3 euler = new Vector3(0f - num2, y, 0f);
			Owner.BlackBoard.Desires.FireDirection = Quaternion.Euler(euler) * Vector3.forward;
			Owner.BlackBoard.Desires.FireDirection.Normalize();
		}
		else
		{
			Owner.BlackBoard.Desires.FireDirection = (vector - shotPos).normalized;
		}
	}
}
