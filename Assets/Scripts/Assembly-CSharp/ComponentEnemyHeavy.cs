using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AnimSetMutantHeavy))]
public class ComponentEnemyHeavy : ComponentEnemy
{
	public float HeightStand = 2f;

	public float HeightCover = 1.3f;

	private new void Awake()
	{
		base.Awake();
		base.Owner.BlackBoard.AiRecon = new AiRecon(base.Owner, 16, 8f);
		base.Owner.BlackBoard.DontDeathAnimMove = true;
	}

	private new void Start()
	{
		base.Start();
		base.Owner.SensorsComponent.AddSensor(E_SensorType.EyesAi, true);
		base.Owner.SensorsComponent.AddSensor(E_SensorType.Positions, true);
		base.Owner.AddGOAPAction(E_GOAPAction.Goto);
		base.Owner.AddGOAPAction(E_GOAPAction.Fallback);
		base.Owner.AddGOAPAction(E_GOAPAction.WeaponReload);
		base.Owner.AddGOAPAction(E_GOAPAction.FireAI);
		base.Owner.AddGOAPAction(E_GOAPAction.DodgeStrafeWalk);
		base.Owner.AddGOAPAction(E_GOAPAction.LookatTarget);
		base.Owner.AddGOAPAction(E_GOAPAction.CheckEvent);
		base.Owner.AddGOAPAction(E_GOAPAction.CheckLostEnemy);
		base.Owner.AddGOAPAction(E_GOAPAction.FireAISuppressive);
		base.Owner.AddGOAPAction(E_GOAPAction.PlayAnim);
		base.Owner.AddGOAPAction(E_GOAPAction.Patrol);
		base.Owner.AddGOAPAction(E_GOAPAction.Teleport);
		base.Owner.AddGOAPGoal(E_GOAPGoals.KillTarget);
		base.Owner.AddGOAPGoal(E_GOAPGoals.WeaponReloadAI);
		base.Owner.AddGOAPGoal(E_GOAPGoals.Dodge);
		base.Owner.AddGOAPGoal(E_GOAPGoals.LookAtTarget);
		base.Owner.AddGOAPGoal(E_GOAPGoals.CheckEvent);
		base.Owner.AddGOAPGoal(E_GOAPGoals.SuppressiveFire);
		base.Owner.AddGOAPGoal(E_GOAPGoals.KeepCombatRange);
		base.Owner.AddGOAPGoal(E_GOAPGoals.KeepCombatRangeTargetCovered);
		base.Owner.AddGOAPGoal(E_GOAPGoals.SpawnAction);
		base.Owner.AddGOAPGoal(E_GOAPGoals.Teleport);
		base.Owner.InitializeGOAP();
	}

	private new void Activate(SpawnPoint spawn)
	{
		base.Activate(spawn);
		base.Owner.CollisionController.center = new Vector3(0f, HeightStand * 0.5f, 0f);
		base.Owner.CollisionController.height = HeightStand;
		StartCoroutine(SpawnTeleport());
	}

	private IEnumerator SpawnTeleport()
	{
		base.Owner.Stop(true);
		base.Owner.SensorsComponent.DeactivateAllSensors();
		base.Owner.BlackBoard.Invulnerable = true;
		base.Owner.BlackBoard.ReactOnHits = false;
		base.Owner.TeleportFadeIn();
		base.Owner.SoundPlay(base.Owner.TeleportSound);
		string s = base.Owner.AnimSet.GetTeleportAnim(AnimSet.E_TeleportAnim.Out);
		base.Owner.GetComponent<Animation>().CrossFade(s, 0f, PlayMode.StopAll);
		yield return new WaitForSeconds(1f);
		base.Owner.SensorsComponent.ActivateAllSensors();
		base.Owner.BlackBoard.Invulnerable = false;
		base.Owner.BlackBoard.ReactOnHits = true;
		base.Owner.Stop(false);
	}
}
