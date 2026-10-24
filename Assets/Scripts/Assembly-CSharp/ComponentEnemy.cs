using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(ComponentSensors))]
[RequireComponent(typeof(AnimComponent))]
[RequireComponent(typeof(Animation))]
[RequireComponent(typeof(AgentHuman))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(ComponentBody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ComponentWeaponsAI))]
public class ComponentEnemy : MonoBehaviour
{
	[Serializable]
	public class SoundInfo
	{
		public AudioClip[] SeeEnemy = new AudioClip[0];

		public AudioClip[] FriendInjuryDeath = new AudioClip[0];

		public AudioClip[] Fear = new AudioClip[0];

		public AudioClip[] Aggro = new AudioClip[0];
	}

	public SoundInfo SoundSetup = new SoundInfo();

	private static float DontSpeakTimer;

	public AgentHuman Owner { get; private set; }

	public AudioClip SeeEnemy
	{
		get
		{
			if (SoundSetup.SeeEnemy.Length == 0)
			{
				return null;
			}
			return SoundSetup.SeeEnemy[Random.Range(0, SoundSetup.SeeEnemy.Length)];
		}
	}

	public AudioClip FriendInjuryDeath
	{
		get
		{
			if (SoundSetup.FriendInjuryDeath.Length == 0)
			{
				return null;
			}
			return SoundSetup.FriendInjuryDeath[Random.Range(0, SoundSetup.FriendInjuryDeath.Length)];
		}
	}

	public AudioClip Fear
	{
		get
		{
			if (SoundSetup.Fear.Length == 0)
			{
				return null;
			}
			return SoundSetup.Fear[Random.Range(0, SoundSetup.Fear.Length)];
		}
	}

	public AudioClip Aggro
	{
		get
		{
			if (SoundSetup.Aggro.Length == 0)
			{
				return null;
			}
			return SoundSetup.Aggro[Random.Range(0, SoundSetup.Aggro.Length)];
		}
	}

	protected void Awake()
	{
		Owner = GetComponent<AgentHuman>();
	}

	protected void Start()
	{
		Owner.BlackBoard.IsPlayer = false;
		BlackBoard blackBoard = Owner.BlackBoard;
		blackBoard.ActionHandler = (BlackBoard.AgentActionHandler)Delegate.Combine(blackBoard.ActionHandler, new BlackBoard.AgentActionHandler(HandleAction));
	}

	protected void Activate(SpawnPoint spawn)
	{
		DontSpeakTimer = 0f;
		Owner.BlackBoard.SpawnPointEnemy = spawn as SpawnPointEnemy;
		Owner.BlackBoard.SightRange = Owner.BlackBoard.SpawnPointEnemy.InitSightRange;
		Owner.BlackBoard.SightFov = Owner.BlackBoard.SpawnPointEnemy.InitSightFov;
		if (Owner.BlackBoard.SpawnPointEnemy.InitState != SpawnPointEnemy.E_InitState.None)
		{
			Owner.WorldState.SetWSProperty(E_PropKey.DoSpawnAction, true);
			Owner.NavMeshAgent.walkableMask = -1;
		}
		else
		{
			Owner.NavMeshAgent.walkableMask &= ~(1 << LayerMask.NameToLayer("WalkOnlyWhenSpawn"));
		}
		if (Owner.BlackBoard.SpawnPointEnemy.InitState == SpawnPointEnemy.E_InitState.Patrol)
		{
			Owner.BlackBoard.Desires.PatrolPoints.AddRange(Owner.BlackBoard.SpawnPointEnemy.PatrolPoints);
		}
		if (Owner.BlackBoard.SpawnPointEnemy.InitState == SpawnPointEnemy.E_InitState.PlayAnim)
		{
			if (Owner.GetComponent<Animation>().GetClip(Owner.BlackBoard.SpawnPointEnemy.SpawnAnimation.name) == null)
			{
				Owner.GetComponent<Animation>().AddClip(Owner.BlackBoard.SpawnPointEnemy.SpawnAnimation, Owner.BlackBoard.SpawnPointEnemy.SpawnAnimation.name);
			}
			Owner.BlackBoard.SpawnPointEnemy.SpawnAnimation.SampleAnimation(Owner.gameObject, 0f);
		}
	}

	protected void Deactivate()
	{
		Owner.NavMeshAgent.walkableMask = 286331153;
	}

	protected void Update()
	{
		if (Owner.WeaponComponent.CurrentWeapon == E_WeaponType.GrenadeLauncherMutant)
		{
			if ((bool)Owner.BlackBoard.DangerousEnemy)
			{
				Vector3 vector = Owner.BlackBoard.DangerousEnemy.Position - Owner.WeaponComponent.Hand.position;
				float num = Mathf.Max(1f, Owner.BlackBoard.DistanceToTarget);
				vector.Normalize();
				float speed = Owner.WeaponComponent.GetCurrentWeapon().Speed;
				float num2 = 0.5f * Mathf.Asin(num * (0f - Physics.gravity.y) / (speed * speed));
				if (float.IsNaN(num2))
				{
					num2 = (float)Math.PI / 4f;
				}
				num2 += Mathf.Atan2(vector.y, Mathf.Sqrt(vector.x * vector.x + vector.z * vector.z));
				num2 *= 57.29578f;
				float y = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
				Vector3 euler = new Vector3(0f - num2, y, 0f);
				Owner.BlackBoard.Desires.FireDirection = Quaternion.Euler(euler) * Vector3.forward;
				if (Owner.BlackBoard.DistanceToTarget < Owner.BlackBoard.CombatRange)
				{
					Owner.BlackBoard.SetFear(Owner.BlackBoard.Fear + (1f - Owner.BlackBoard.DistanceToTarget / Owner.BlackBoard.CombatRange) * Time.deltaTime * 15f);
				}
			}
			else
			{
				Owner.BlackBoard.Desires.FireDirection = Owner.Forward;
			}
		}
		else if ((bool)Owner.BlackBoard.VisibleTarget)
		{
			Owner.BlackBoard.Desires.FireDirection = Owner.BlackBoard.VisibleTarget.TransformTarget.position - Owner.WeaponComponent.GetCurrentWeapon().ShotPos;
		}
		else if ((bool)Owner.BlackBoard.DangerousEnemy)
		{
			if (Owner.BlackBoard.DangerousEnemy.IsInCover)
			{
				Cover cover = Owner.BlackBoard.DangerousEnemy.BlackBoard.Cover;
				E_CoverDirection coverPosition = Owner.BlackBoard.DangerousEnemy.BlackBoard.CoverPosition;
				Vector3 vector2;
				if (Owner.BlackBoard.DangerousEnemy.WorldState.GetWSProperty(E_PropKey.CoverState).GetCoverState() == E_CoverState.Aim)
				{
					vector2 = Owner.BlackBoard.DangerousEnemy.TransformTarget.position;
				}
				else
				{
					switch (coverPosition)
					{
					case E_CoverDirection.Left:
						vector2 = cover.EyeLeftEdge;
						break;
					case E_CoverDirection.Right:
						vector2 = cover.EyeRightEdge;
						break;
					default:
					{
						Fact fact = Owner.Memory.GetFact(E_EventTypes.EnemyHideInCover);
						vector2 = ((fact == null) ? (cover.Position + Vector3.up * 1.3f) : (fact.Position + Vector3.up * 1.3f));
						break;
					}
					}
				}
				Owner.BlackBoard.Desires.FireDirection = vector2 - Owner.WeaponComponent.GetCurrentWeapon().ShotPos;
			}
			else
			{
				Owner.BlackBoard.Desires.FireDirection = Owner.BlackBoard.DangerousEnemy.TransformTarget.position - Owner.WeaponComponent.GetCurrentWeapon().ShotPos;
			}
			Owner.BlackBoard.Desires.FireDirection.Normalize();
		}
		else
		{
			Owner.BlackBoard.Desires.FireDirection = Owner.Forward;
		}
		if (!Owner.BlackBoard.Stop)
		{
			ClipFireDirection();
		}
	}

	private void LateUpdate()
	{
		UpdateCombatSetting();
		UpdateSpeech();
	}

	public void HandleAction(AgentAction a)
	{
		if (a is AgentActionInjury)
		{
			if (Mission.Instance.CurrentGameZone != null)
			{
				Mission.Instance.CurrentGameZone.SendFactToEnemies(Owner, (a as AgentActionInjury).Attacker, E_EventTypes.FriendInjured, 30f, 3f, true);
			}
		}
		else if (a is AgentActionDeath && Mission.Instance.CurrentGameZone != null)
		{
			Mission.Instance.CurrentGameZone.SendFactToEnemies(Owner, (a as AgentActionDeath).Attacker, E_EventTypes.FriendDeath, 30f, 3f, true);
		}
		Owner.BlackBoard.UpdateCombatSetting(a);
	}

	private void ClipFireDirection()
	{
		Quaternion quaternion = default(Quaternion);
		quaternion.SetLookRotation(Owner.BlackBoard.Desires.FireDirection);
		if (!Owner.BlackBoard.Cover)
		{
			return;
		}
		Vector3 eulerAngles = Owner.BlackBoard.Cover.Transform.rotation.eulerAngles;
		Quaternion quaternion2 = new Quaternion(0f - Owner.BlackBoard.Cover.Transform.rotation.x, 0f - Owner.BlackBoard.Cover.Transform.rotation.y, 0f - Owner.BlackBoard.Cover.Transform.rotation.z, Owner.BlackBoard.Cover.Transform.rotation.w);
		Quaternion quaternion3 = quaternion2 * Owner.BlackBoard.Cover.Transform.rotation;
		Quaternion quaternion4 = new Quaternion(0f - Owner.Transform.rotation.x, 0f - Owner.Transform.rotation.y, 0f - Owner.Transform.rotation.z, Owner.Transform.rotation.w);
		Quaternion quaternion5 = quaternion4 * quaternion;
		Vector3 eulerAngles2 = quaternion.eulerAngles;
		Vector3 eulerAngles3 = Quaternion.FromToRotation(quaternion5 * Vector3.forward, quaternion3 * Vector3.forward).eulerAngles;
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
		quaternion.eulerAngles = eulerAngles2;
		Owner.BlackBoard.Desires.FireDirection = quaternion * Vector3.forward;
	}

	private void UpdateSpeech()
	{
		if (!Owner.IsVisible || Owner.BlackBoard.Stop || !Owner.IsAlive || Owner.BlackBoard.BusyAction || DontSpeakTimer > Time.timeSinceLevelLoad)
		{
			return;
		}
		if (Owner.Memory.GetValidFact(E_EventTypes.EnemySee) != null && Random.Range(0, 100) < 5)
		{
			Owner.SoundPlay(SeeEnemy);
			DontSpeakTimer = Time.timeSinceLevelLoad + Random.Range(10f, 15f);
		}
		else if (Owner.Memory.GetValidFact(E_EventTypes.FriendInjured) != null && Random.Range(0, 100) < 25)
		{
			Owner.SoundPlay(SeeEnemy);
			DontSpeakTimer = Time.timeSinceLevelLoad + Random.Range(10f, 15f);
		}
		else if (Owner.Memory.GetValidFact(E_EventTypes.FriendDeath) != null && Random.Range(0, 100) < 25)
		{
			Owner.SoundPlay(SeeEnemy);
			DontSpeakTimer = Time.timeSinceLevelLoad + Random.Range(10f, 15f);
		}
		else if (Owner.Memory.GetValidFact(E_EventTypes.EnemyInjuredMe) != null && Random.Range(0, 100) < 25)
		{
			if (Owner.BlackBoard.Fear > Owner.BlackBoard.Rage)
			{
				Owner.SoundPlay(Fear);
			}
			else
			{
				Owner.SoundPlay(Aggro);
			}
			DontSpeakTimer = Time.timeSinceLevelLoad + Random.Range(10f, 15f);
		}
	}

	protected virtual void UpdateCombatSetting()
	{
		bool flag = Owner.WorldState.GetWSProperty(E_PropKey.SeeEnemy).GetBool();
		BlackBoard blackBoard = Owner.BlackBoard;
		if (!flag && (Owner.Memory.GetValidFact(E_EventTypes.EnemyHideInCover) != null || Owner.Memory.GetValidFact(E_EventTypes.EnemyLost) != null))
		{
			flag = true;
		}
		if (flag || Owner.IsInCover)
		{
			blackBoard.SetRage(blackBoard.Rage + blackBoard.CombatSetup.RageModificator * Time.deltaTime);
		}
		else
		{
			blackBoard.SetRage(blackBoard.Rage - blackBoard.CombatSetup.RageModificator * Time.deltaTime * 0.5f);
		}
		if (!flag)
		{
			blackBoard.SetFear(blackBoard.Fear - blackBoard.CombatSetup.FearModificator * Time.deltaTime);
		}
		else if (Owner.WorldState.GetWSProperty(E_PropKey.CoverState).GetCoverState() != E_CoverState.None)
		{
			blackBoard.SetFear(blackBoard.Fear - blackBoard.CombatSetup.FearModificator * Time.deltaTime * 0.2f);
		}
		else if (Owner.WorldState.GetWSProperty(E_PropKey.AheadOfEnemy).GetBool())
		{
			blackBoard.SetFear(blackBoard.Fear + blackBoard.CombatSetup.FearModificator * Time.deltaTime);
		}
		else
		{
			blackBoard.SetFear(blackBoard.Fear + blackBoard.CombatSetup.FearModificator * Time.deltaTime * 0.5f);
		}
		if (Owner.WorldState.GetWSProperty(E_PropKey.EnemyLookingAtMe).GetBool())
		{
			blackBoard.SetDodge(blackBoard.Dodge + blackBoard.CombatSetup.DodgeModificator * Time.deltaTime);
		}
		else
		{
			blackBoard.SetDodge(blackBoard.Dodge - blackBoard.CombatSetup.DodgeModificator * Time.deltaTime);
		}
	}
}
