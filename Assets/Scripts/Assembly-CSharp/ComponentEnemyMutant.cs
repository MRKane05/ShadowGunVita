using System;
using UnityEngine;

[RequireComponent(typeof(AnimSetMutant))]
public class ComponentEnemyMutant : ComponentEnemy
{
	public float HeightStand = 2f;

	public float HeightCover = 1.3f;

	private new void Awake()
	{
		base.Awake();
		base.Owner.BlackBoard.AiRecon = new AiRecon(base.Owner, 16, 4f);
	}

	private new void Start()
	{
		base.Start();
		base.Owner.SensorsComponent.AddSensor(E_SensorType.EyesAi, true);
		base.Owner.SensorsComponent.AddSensor(E_SensorType.CoverAI, true);
		base.Owner.SensorsComponent.AddSensor(E_SensorType.Positions, true);
		base.Owner.AddGOAPAction(E_GOAPAction.Goto);
		base.Owner.AddGOAPAction(E_GOAPAction.Fallback);
		base.Owner.AddGOAPAction(E_GOAPAction.WeaponReload);
		base.Owner.AddGOAPAction(E_GOAPAction.FireAI);
		base.Owner.AddGOAPAction(E_GOAPAction.DodgeStrafe);
		base.Owner.AddGOAPAction(E_GOAPAction.LookatTarget);
		base.Owner.AddGOAPAction(E_GOAPAction.CheckEvent);
		base.Owner.AddGOAPAction(E_GOAPAction.CheckLostEnemy);
		base.Owner.AddGOAPAction(E_GOAPAction.FireAISuppressive);
		base.Owner.AddGOAPAction(E_GOAPAction.AdvanceToCoveredTarget);
		base.Owner.AddGOAPAction(E_GOAPAction.CoverEnter);
		base.Owner.AddGOAPAction(E_GOAPAction.CoverLeave);
		base.Owner.AddGOAPAction(E_GOAPAction.CoverJumpOver);
		base.Owner.AddGOAPAction(E_GOAPAction.CoverLookAtAndFire);
		base.Owner.AddGOAPAction(E_GOAPAction.CoverFireAI);
		base.Owner.AddGOAPAction(E_GOAPAction.CoverFireAISuppressive);
		base.Owner.AddGOAPAction(E_GOAPAction.PlayAnim);
		base.Owner.AddGOAPAction(E_GOAPAction.Patrol);
		base.Owner.AddGOAPGoal(E_GOAPGoals.KillTarget);
		base.Owner.AddGOAPGoal(E_GOAPGoals.WeaponReloadAI);
		base.Owner.AddGOAPGoal(E_GOAPGoals.Dodge);
		base.Owner.AddGOAPGoal(E_GOAPGoals.LookAtTarget);
		base.Owner.AddGOAPGoal(E_GOAPGoals.CheckEvent);
		base.Owner.AddGOAPGoal(E_GOAPGoals.SuppressiveFire);
		base.Owner.AddGOAPGoal(E_GOAPGoals.Advance);
		base.Owner.AddGOAPGoal(E_GOAPGoals.AdvanceTargetCovered);
		base.Owner.AddGOAPGoal(E_GOAPGoals.AdvanceToCover);
		base.Owner.AddGOAPGoal(E_GOAPGoals.FallBack);
		base.Owner.AddGOAPGoal(E_GOAPGoals.HideInCover);
		base.Owner.AddGOAPGoal(E_GOAPGoals.GetOutOfCover);
		base.Owner.AddGOAPGoal(E_GOAPGoals.SpawnAction);
		base.Owner.InitializeGOAP();
		BlackBoard blackBoard = base.Owner.BlackBoard;
		blackBoard.ActionHandler = (BlackBoard.AgentActionHandler)Delegate.Combine(blackBoard.ActionHandler, new BlackBoard.AgentActionHandler(HandleAction));
	}

	private new void Activate(SpawnPoint spawn)
	{
		base.Activate(spawn);
		base.Owner.CollisionController.center = new Vector3(0f, HeightStand * 0.5f, 0f);
		base.Owner.CollisionController.height = HeightStand;
	}

	private new void Deactivate()
	{
		base.Deactivate();
	}

	private new void Update()
	{
		base.Update();
	}

	public new void HandleAction(AgentAction a)
	{
		if (a is AgentActionCoverEnter)
		{
			base.Owner.CollisionController.center = new Vector3(0f, HeightCover * 0.5f, 0f);
			base.Owner.CollisionController.height = HeightCover;
			base.Owner.BlackBoard.Desires.Rotation = (a as AgentActionCoverEnter).Cover.Transform.rotation;
		}
		else if (a is AgentActionCoverLeave)
		{
			base.Owner.CollisionController.center = new Vector3(0f, HeightStand * 0.5f, 0f);
			base.Owner.CollisionController.height = HeightStand;
			base.Owner.BlackBoard.Desires.Rotation.SetLookRotation((a as AgentActionCoverLeave).FinalViewDirection);
		}
		else if (a is AgentActionCoverFire)
		{
			if (base.Owner.BlackBoard.CoverPosition == E_CoverDirection.Middle)
			{
				base.Owner.CollisionController.height = HeightStand;
				base.Owner.CollisionController.center = new Vector3(0f, HeightStand * 0.5f, 0f);
			}
			else if (base.Owner.BlackBoard.CoverPosition == E_CoverDirection.Left)
			{
				base.Owner.CollisionController.height = HeightCover;
				base.Owner.CollisionController.center = new Vector3(-0.7f, HeightCover * 0.5f, 0f);
			}
			else
			{
				base.Owner.CollisionController.height = HeightCover;
				base.Owner.CollisionController.center = new Vector3(0.7f, HeightCover * 0.5f, 0f);
			}
		}
		else if (a is AgentActionCoverFireCancel)
		{
			base.Owner.CollisionController.center = new Vector3(0f, HeightCover * 0.5f, 0f);
			base.Owner.CollisionController.height = HeightCover;
		}
		else if (a is AgentActionCoverTerminated)
		{
			base.Owner.CollisionController.center = new Vector3(0f, HeightStand * 0.5f + 0.08f, 0f);
			base.Owner.CollisionController.height = HeightStand;
		}
	}
}
