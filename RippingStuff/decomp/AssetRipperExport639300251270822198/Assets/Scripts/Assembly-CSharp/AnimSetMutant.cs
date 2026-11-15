using System;
using UnityEngine;

[Serializable]
public class AnimSetMutant : AnimSet
{
	private BlackBoard BlackBoard;

	private Animation Animation;

	private WorldState WorldState;

	private static Randomizer InjuryFront;

	private static Randomizer Death;

	private static Randomizer DeathSMG;

	private static Randomizer DeathShotgun;

	private static Randomizer DeathRange;

	private static Randomizer DeathInMove;

	static AnimSetMutant()
	{
		InjuryFront = new Randomizer();
		Death = new Randomizer();
		DeathSMG = new Randomizer();
		DeathShotgun = new Randomizer();
		DeathRange = new Randomizer();
		DeathInMove = new Randomizer();
		InjuryFront.Add("InjuryIdleFront1");
		InjuryFront.Add("InjuryIdleFront2");
		InjuryFront.Add("InjuryIdleFront3");
		InjuryFront.Add("InjuryIdleFront4");
		Death.Add("DeathIdleFront1");
		Death.Add("DeathIdleFront2");
		DeathSMG.Add("DeathIdleFront1");
		DeathSMG.Add("DeathIdleFront2");
		DeathSMG.Add("DeathRunF");
		DeathShotgun.Add("DeathIdleFront3");
		DeathShotgun.Add("DeathIdleHeadShot");
		DeathRange.Add("DeathIdleFront3");
		DeathRange.Add("DeathIdleRight");
		DeathRange.Add("DeathIdleLeft");
		DeathInMove.Add("DeathWalkF");
		DeathInMove.Add("DeathRunF");
	}

	private void Awake()
	{
		BlackBoard = GetComponent<AgentHuman>().BlackBoard;
		Animation = base.animation;
	}

	private void Start()
	{
		WorldState = GetComponent<AgentHuman>().WorldState;
	}

	public override string GetBlockAnim(E_BlockState state)
	{
		return null;
	}

	public override string GetIdleAnim()
	{
		if (BlackBoard.Cover != null)
		{
			if (BlackBoard.CoverPosition == E_CoverDirection.Middle)
			{
				return (BlackBoard.CoverPose != 0) ? "CoverCrouchIdleCenter" : "CoverStandIdleCenter";
			}
			if (BlackBoard.CoverPosition == E_CoverDirection.Left)
			{
				return (BlackBoard.CoverPose != 0) ? "CoverCrouchIdleLeft" : "CoverStandIdleLeft";
			}
			if (BlackBoard.CoverPosition == E_CoverDirection.Right)
			{
				return (BlackBoard.CoverPose != 0) ? "CoverCrouchIdleRight" : "CoverStandIdleRight";
			}
		}
		return "Idle";
	}

	public override string GetIdleActionAnim()
	{
		return "Idle";
	}

	public override string GetMoveAnim()
	{
		if ((bool)BlackBoard.Cover)
		{
			if (BlackBoard.MoveType == E_MoveType.StrafeLeft)
			{
				return (BlackBoard.CoverPose != 0) ? "CoverCrouchStrafeLeft" : "CoverStandStrafeLeft";
			}
			if (BlackBoard.MoveType == E_MoveType.StrafeRight)
			{
				return (BlackBoard.CoverPose != 0) ? "CoverCrouchStrafeRight" : "CoverStandStrafeRight";
			}
		}
		if (BlackBoard.MoveType == E_MoveType.Forward)
		{
			return (BlackBoard.MotionType != E_MotionType.Walk) ? "RunF" : "WalkSlowF";
		}
		if (BlackBoard.MoveType == E_MoveType.Backward)
		{
			return (BlackBoard.MotionType != E_MotionType.Walk) ? "RunB" : "WalkSlowB";
		}
		if (BlackBoard.MoveType == E_MoveType.StrafeLeft)
		{
			return (BlackBoard.MotionType != E_MotionType.Walk) ? "RunL" : "WalkSlowL";
		}
		if (BlackBoard.MoveType == E_MoveType.StrafeRight)
		{
			return (BlackBoard.MotionType != E_MotionType.Walk) ? "RunR" : "WalkSlowR";
		}
		return "Idle";
	}

	public override string GetRotateAnim(E_RotationType rotationType)
	{
		return (rotationType != E_RotationType.Left) ? "TurnR" : "TurnL";
	}

	public override string GetDodgeAnim(E_StrafeDirection dir)
	{
		return (dir != 0) ? "DodgeStrafeR" : "DodgeStrafeL";
	}

	public override string GetWeaponAnim(E_WeaponAction action)
	{
		switch (action)
		{
		case E_WeaponAction.Fire:
			if (BlackBoard.Cover != null)
			{
				string[] array = new string[4]
				{
					string.Empty,
					"CoverCrouchFireCenterSMG",
					"CoverCrouchFireLeftSMG",
					"CoverCrouchFireRightSMG"
				};
				string[] array2 = new string[4]
				{
					string.Empty,
					string.Empty,
					"CoverStandFireLeftSMG",
					"CoverStandFireRightSMG"
				};
				if (BlackBoard.CoverPose == E_CoverPose.Crouch)
				{
					return array[(int)BlackBoard.CoverPosition];
				}
				return array2[(int)BlackBoard.CoverPosition];
			}
			if (BlackBoard.MotionType == E_MotionType.None)
			{
				return "IdleFireSMG";
			}
			return "RunFireSMG";
		case E_WeaponAction.Reload:
			if (BlackBoard.Cover != null)
			{
				if (BlackBoard.CoverPose == E_CoverPose.Crouch)
				{
					return "CoverCrouchReloadSMG";
				}
				return "CoverStandReloadSMG";
			}
			if (BlackBoard.MotionType == E_MotionType.None)
			{
				return "IdleReloadSMG";
			}
			return "RunReloadSMG";
		default:
			return "Idle";
		}
	}

	public override float GetWeaponAnimTime(E_WeaponType type, E_WeaponAction action)
	{
		string[,] array = new string[2, 4]
		{
			{ "IdleFireSMG", "IdleFireSMG", "IdleFireSMG", "IdleReloadSMG" },
			{ "IdleFireSMG", "IdleFireSMG", "RunFireSMG", "IdleReloadSMG" }
		};
		if (type == E_WeaponType.SMG)
		{
			return (BlackBoard.MotionType != 0) ? Animation[array[1, (int)action]].length : Animation[array[0, (int)action]].length;
		}
		return 0f;
	}

	public override string GetInjuryAnim(E_WeaponType weapon)
	{
		if (BlackBoard.Cover != null)
		{
			if (WorldState.GetWSProperty(E_PropKey.CoverState).GetCoverState() == E_CoverState.Middle)
			{
				return (BlackBoard.CoverPose != 0) ? "InjuryCoverCrouchIdle" : "InjuryCoverStandIdle";
			}
			if (BlackBoard.CoverPosition == E_CoverDirection.Middle)
			{
				return (BlackBoard.CoverPose != 0) ? "InjuryCoverCrouchAimCenter" : string.Empty;
			}
			if (BlackBoard.CoverPosition == E_CoverDirection.Left)
			{
				return (BlackBoard.CoverPose != 0) ? "InjuryCoverCrouchAimLeft" : "InjuryCoverStandAimLeft";
			}
			if (BlackBoard.CoverPosition == E_CoverDirection.Right)
			{
				return (BlackBoard.CoverPose != 0) ? "InjuryCoverCrouchAimRight" : "InjuryCoverStandAimRight";
			}
		}
		return InjuryFront.Get();
	}

	public override string GetDeathAnim(E_WeaponType weapon)
	{
		if (BlackBoard.Cover != null)
		{
			if (WorldState.GetWSProperty(E_PropKey.CoverState).GetCoverState() != E_CoverState.Aim)
			{
				return (BlackBoard.CoverPose != 0) ? "DeathCoverCrouchIdle" : "DeathCoverStandIdle";
			}
			if (BlackBoard.CoverPosition == E_CoverDirection.Middle)
			{
				return (BlackBoard.CoverPose != 0) ? "DeathCoverCrouchAimCenter" : string.Empty;
			}
			if (BlackBoard.CoverPosition == E_CoverDirection.Left)
			{
				return (BlackBoard.CoverPose != 0) ? "DeathCoverCrouchAimLeft" : "DeathCoverStandAimLeft";
			}
			if (BlackBoard.CoverPosition == E_CoverDirection.Right)
			{
				return (BlackBoard.CoverPose != 0) ? "DeathCoverCrouchAimRight" : "DeathCoverStandAimRight";
			}
		}
		switch (weapon)
		{
		case E_WeaponType.SMG:
			if (BlackBoard.MotionType != 0)
			{
				return DeathInMove.Get();
			}
			return DeathSMG.Get();
		case E_WeaponType.Shotgun:
			if (BlackBoard.MotionType != 0)
			{
				return DeathInMove.Get();
			}
			return DeathShotgun.Get();
		case E_WeaponType.GrenadeLauncher:
		case E_WeaponType.RocketLauncher:
			return DeathRange.Get();
		default:
			return Death.Get();
		}
	}

	public override string GetKnockdowAnim(E_KnockdownState knockdownState)
	{
		throw new NotImplementedException();
	}

	public override string GetCoverAnim(E_CoverAnim type, E_CoverPose pose, E_CoverDirection direction)
	{
		switch (type)
		{
		case E_CoverAnim.Enter:
			return (pose != 0) ? "CoverCrouchEnter" : "CoverStandEnter";
		case E_CoverAnim.Leave:
			return (pose != 0) ? "CoverCrouchLeave" : "CoverStandLeave";
		case E_CoverAnim.AimStart:
			switch (direction)
			{
			case E_CoverDirection.Left:
				return (pose != 0) ? "CoverCrouchAimLeftStart" : "CoverStandAimLeftStart";
			case E_CoverDirection.Right:
				return (pose != 0) ? "CoverCrouchAimRightStart" : "CoverStandAimRightStart";
			default:
				return (pose != 0) ? "CoverCrouchAimCenterStart" : string.Empty;
			}
		case E_CoverAnim.AimEnd:
			switch (direction)
			{
			case E_CoverDirection.Left:
				return (pose != 0) ? "CoverCrouchAimLeftBack" : "CoverStandAimLeftBack";
			case E_CoverDirection.Right:
				return (pose != 0) ? "CoverCrouchAimRightBack" : "CoverStandAimRightBack";
			default:
				return (pose != 0) ? "CoverCrouchAimCenterBack" : string.Empty;
			}
		case E_CoverAnim.JumpOver:
			return "JumpOverToIdle";
		default:
			return null;
		}
	}

	public override string GetTeleportAnim(E_TeleportAnim type)
	{
		throw new NotImplementedException();
	}

	public override string GetInjuryCritAnim()
	{
		throw new NotImplementedException();
	}

	public override string GetAimAnim(E_AimDirection direction)
	{
		if (BlackBoard.Cover != null)
		{
			E_CoverPose coverPose = BlackBoard.CoverPose;
			switch (direction)
			{
			case E_AimDirection.Left:
				switch (BlackBoard.CoverPosition)
				{
				case E_CoverDirection.Left:
					return (coverPose != 0) ? "CoverCrouchAimLeftMaxL" : "CoverStandAimLeftMaxL";
				case E_CoverDirection.Right:
					return (coverPose != 0) ? "CoverCrouchAimRightMaxL" : "CoverStandAimRightMaxL";
				default:
					return (coverPose != 0) ? "CoverCrouchAimCenterMaxL" : string.Empty;
				}
			case E_AimDirection.Right:
				switch (BlackBoard.CoverPosition)
				{
				case E_CoverDirection.Left:
					return (coverPose != 0) ? "CoverCrouchAimLeftMaxR" : "CoverStandAimLeftMaxR";
				case E_CoverDirection.Right:
					return (coverPose != 0) ? "CoverCrouchAimRightMaxR" : "CoverStandAimRightMaxR";
				default:
					return (coverPose != 0) ? "CoverCrouchAimCenterMaxR" : string.Empty;
				}
			case E_AimDirection.Up:
				switch (BlackBoard.CoverPosition)
				{
				case E_CoverDirection.Left:
					return (coverPose != 0) ? "CoverCrouchAimLeftMaxU" : "CoverStandAimLeftMaxU";
				case E_CoverDirection.Right:
					return (coverPose != 0) ? "CoverCrouchAimRightMaxU" : "CoverStandAimRightMaxU";
				default:
					return (coverPose != 0) ? "CoverCrouchAimCenterMaxU" : string.Empty;
				}
			case E_AimDirection.Down:
				switch (BlackBoard.CoverPosition)
				{
				case E_CoverDirection.Left:
					return (coverPose != 0) ? "CoverCrouchAimLeftMaxD" : "CoverStandAimLeftMaxD";
				case E_CoverDirection.Right:
					return (coverPose != 0) ? "CoverCrouchAimRightMaxD" : "CoverStandAimRightMaxD";
				default:
					return (coverPose != 0) ? "CoverCrouchAimCenterMaxD" : string.Empty;
				}
			}
		}
		else
		{
			switch (direction)
			{
			case E_AimDirection.Down:
				return "AimD";
			case E_AimDirection.Up:
				return "AimU";
			}
		}
		throw new NotImplementedException();
	}

	public override string GetRollAnim(E_Direction moveType)
	{
		throw new NotImplementedException();
	}
}
