using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimSetPlayer : AnimSet
{
	public class WeaponAnims
	{
		public string FireIdle;

		public string FireRun;

		public string FireCoverCrouchCenter;

		public string FireCoverCrouchLeft;

		public string FireCoverCrouchRight;

		public string FireCoverStandLeft;

		public string FireCoverStandRight;

		public string ReloadIdle;

		public string ReloadRun;

		public string ReloadCoverStand;

		public string ReloadCoverStandLeft;

		public string ReloadCoverStandRight;

		public string ReloadCoverCrouch;

		public string ArmIdle;

		public string DisarmIdle;

		public string ArmCoverCrouch;

		public string DisarmCoverCrouch;

		public string ArmCoverStand;

		public string DisarmCoverStand;
	}

	private BlackBoard BlackBoard;

	private Animation Animation;

	private WorldState WorldState;

	private ComponentWeapons Weapons;

	private static Randomizer InjuryFront = new Randomizer();

	private static Randomizer DeathFront = new Randomizer();

	private static string[] RunAnims = new string[4] { "RunF", "RunB", "RunL", "RunR" };

	private static string[] WalkComlinkAnims = new string[4] { "WalkDialogF", "WalkDialogB", "WalkDialogL", "WalkDialogR" };

	private Dictionary<E_WeaponType, WeaponAnims> WeaponAnimations = new Dictionary<E_WeaponType, WeaponAnims>();

	private void Awake()
	{
		WeaponAnimations.Add(E_WeaponType.SMG, new WeaponAnims
		{
			FireIdle = "IdleFireSMG",
			FireRun = "RunFireSMG",
			FireCoverCrouchCenter = "CoverCrouchFireCenterSMG",
			FireCoverCrouchLeft = "CoverCrouchFireLeftSMG",
			FireCoverCrouchRight = "CoverCrouchFireRightSMG",
			FireCoverStandLeft = "CoverStandFireLeftSMG",
			FireCoverStandRight = "CoverStandFireRightSMG",
			ReloadIdle = "IdleReloadSMG",
			ReloadRun = "RunReloadSMG",
			ReloadCoverStand = "CoverStandReloadSMG",
			ReloadCoverStandLeft = "CoverStandLeftReloadSMG",
			ReloadCoverStandRight = "CoverStandRightReloadSMG",
			ReloadCoverCrouch = "CoverCrouchReloadSMG",
			ArmIdle = "IdleShowWeapon",
			DisarmIdle = "IdleHideWeapon",
			ArmCoverCrouch = "CoverCrouchShowWeapon",
			DisarmCoverCrouch = "CoverCrouchHideWeapon",
			ArmCoverStand = "CoverStandShowWeapon",
			DisarmCoverStand = "CoverStandHideWeapon"
		});
		WeaponAnimations.Add(E_WeaponType.RocketLauncher, new WeaponAnims
		{
			FireIdle = "IdleFireRocket",
			FireRun = "RunFireRocket",
			FireCoverCrouchCenter = "CoverCrouchFireCenterRocket",
			FireCoverCrouchLeft = "CoverCrouchFireLeftRocket",
			FireCoverCrouchRight = "CoverCrouchFireRightRocket",
			FireCoverStandLeft = "CoverStandFireLeftRocket",
			FireCoverStandRight = "CoverStandFireRightRocket",
			ReloadIdle = "IdleReloadRocket",
			ReloadRun = "RunReloadRocket",
			ReloadCoverStand = "CoverStandCenterReloadRocket",
			ReloadCoverStandLeft = "CoverStandLeftReloadRocket",
			ReloadCoverStandRight = "CoverStandRightReloadRocket",
			ReloadCoverCrouch = "CoverCrouchCenterReloadRocket",
			ArmIdle = "IdleShowWeapon",
			DisarmIdle = "IdleHideWeapon",
			ArmCoverCrouch = "CoverCrouchShowWeapon",
			DisarmCoverCrouch = "CoverCrouchHideWeapon",
			ArmCoverStand = "CoverStandShowWeapon",
			DisarmCoverStand = "CoverStandHideWeapon"
		});
		WeaponAnimations.Add(E_WeaponType.GrenadeLauncher, new WeaponAnims
		{
			FireIdle = "IdleFireGranate",
			FireRun = "RunFireGranate",
			FireCoverCrouchCenter = "CoverCrouchFireCenterGranade",
			FireCoverCrouchLeft = "CoverCrouchFireLeftGranade",
			FireCoverCrouchRight = "CoverCrouchFireRightGranade",
			FireCoverStandLeft = "CoverStandFireLeftGranade",
			FireCoverStandRight = "CoverStandFireRightGranade",
			ReloadIdle = "IdleReloadGranade",
			ReloadRun = "RunReloadGranade",
			ReloadCoverStand = "CoverStandCenterReloadGranade",
			ReloadCoverStandLeft = "CoverStandLeftReloadGranade",
			ReloadCoverStandRight = "CoverStandRightReloadGranade",
			ReloadCoverCrouch = "CoverCrouchCenterReloadGranade",
			ArmIdle = "IdleShowWeapon",
			DisarmIdle = "IdleHideWeapon",
			ArmCoverCrouch = "CoverCrouchShowWeapon",
			DisarmCoverCrouch = "CoverCrouchHideWeapon",
			ArmCoverStand = "CoverStandShowWeapon",
			DisarmCoverStand = "CoverStandHideWeapon"
		});
		WeaponAnimations.Add(E_WeaponType.Shotgun, new WeaponAnims
		{
			FireIdle = "IdleFireShotgun",
			FireRun = "RunFireShotgun",
			FireCoverCrouchCenter = "CoverCrouchFireCenterShotgun",
			FireCoverCrouchLeft = "CoverCrouchFireLeftShotgun",
			FireCoverCrouchRight = "CoverCrouchFireRightShotgun",
			FireCoverStandLeft = "CoverStandFireLeftShotgun",
			FireCoverStandRight = "CoverStandFireRightShotgun",
			ReloadIdle = "IdleReloadShotgun",
			ReloadRun = "RunReloadShotgun",
			ReloadCoverStand = "CoverStandCenterReloadShotgun",
			ReloadCoverStandLeft = "CoverStandLeftReloadShotgun",
			ReloadCoverStandRight = "CoverStandRightReloadShotgun",
			ReloadCoverCrouch = "CoverCrouchCenterReloadShotgun",
			ArmIdle = "IdleShowWeapon",
			DisarmIdle = "IdleHideWeapon",
			ArmCoverCrouch = "CoverCrouchShowWeapon",
			DisarmCoverCrouch = "CoverCrouchHideWeapon",
			ArmCoverStand = "CoverStandShowWeapon",
			DisarmCoverStand = "CoverStandHideWeapon"
		});
		WeaponAnimations.Add(E_WeaponType.PlasmaRifle, new WeaponAnims
		{
			FireIdle = "IdleFireShotgun",
			FireRun = "RunFireShotgun",
			FireCoverCrouchCenter = "CoverCrouchFireCenterShotgun",
			FireCoverCrouchLeft = "CoverCrouchFireLeftShotgun",
			FireCoverCrouchRight = "CoverCrouchFireRightShotgun",
			FireCoverStandLeft = "CoverStandFireLeftShotgun",
			FireCoverStandRight = "CoverStandFireRightShotgun",
			ReloadIdle = "IdleReloadShotgun",
			ReloadRun = "RunReloadShotgun",
			ReloadCoverStand = "CoverStandCenterReloadShotgun",
			ReloadCoverStandLeft = "CoverStandLeftReloadShotgun",
			ReloadCoverStandRight = "CoverStandRightReloadShotgun",
			ReloadCoverCrouch = "CoverCrouchCenterReloadShotgun",
			ArmIdle = "IdleShowWeapon",
			DisarmIdle = "IdleHideWeapon",
			ArmCoverCrouch = "CoverCrouchShowWeapon",
			DisarmCoverCrouch = "CoverCrouchHideWeapon",
			ArmCoverStand = "CoverStandShowWeapon",
			DisarmCoverStand = "CoverStandHideWeapon"
		});
		InjuryFront.Add("InjuryIdleFront1");
		InjuryFront.Add("InjuryIdleFront2");
		InjuryFront.Add("InjuryIdleFront3");
		DeathFront.Add("DeathIdleFront1");
		DeathFront.Add("DeathIdleFront2");
		DeathFront.Add("DeathIdleFront3");
		BlackBoard = GetComponent<AgentHuman>().BlackBoard;
		Weapons = GetComponent<ComponentWeapons>();
		Animation = base.GetComponent<Animation>();
		Animation["AimU"].AddMixingTransform(base.transform.Find("pelvis/stomach"));
		Animation["AimD"].AddMixingTransform(base.transform.Find("pelvis/stomach"));
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
				return (BlackBoard.CoverPose != E_CoverPose.Stand) ? "CoverCrouchIdleCenter" : "CoverStandIdleCenter";
			}
			if (BlackBoard.CoverPosition == E_CoverDirection.Left)
			{
				return (BlackBoard.CoverPose != E_CoverPose.Stand) ? "CoverCrouchIdleLeft" : "CoverStandIdleLeft";
			}
			if (BlackBoard.CoverPosition == E_CoverDirection.Right)
			{
				return (BlackBoard.CoverPose != E_CoverPose.Stand) ? "CoverCrouchIdleRight" : "CoverStandIdleRight";
			}
		}
		if (BlackBoard.ComlinkOn)
		{
			return "IdleDialog";
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
				return (BlackBoard.CoverPose != E_CoverPose.Stand) ? "CoverCrouchStrafeLeft" : "CoverStandStrafeLeft";
			}
			if (BlackBoard.MoveType == E_MoveType.StrafeRight)
			{
				return (BlackBoard.CoverPose != E_CoverPose.Stand) ? "CoverCrouchStrafeRight" : "CoverStandStrafeRight";
			}
		}
		switch (BlackBoard.MotionType)
		{
		case E_MotionType.Run:
			return RunAnims[(int)BlackBoard.MoveType];
		case E_MotionType.Walk:
			return RunAnims[(int)BlackBoard.MoveType];
		case E_MotionType.Comlink:
			return WalkComlinkAnims[(int)BlackBoard.MoveType];
		default:
			Debug.LogError(string.Concat(" No move animation for player ", BlackBoard.MotionType, " ", BlackBoard.MoveType));
			return "Idle";
		}
	}

	public override string GetRotateAnim(E_RotationType rotationType)
	{
		return (rotationType != E_RotationType.Left) ? "TurnR" : "TurnL";
	}

	public override string GetDodgeAnim(E_StrafeDirection dir)
	{
		return "Idle";
	}

	public override string GetRollAnim(E_Direction moveType)
	{
		switch (moveType)
		{
		case E_Direction.Forward:
			return "RollF";
		case E_Direction.Backward:
			return "RollB";
		case E_Direction.Left:
			return "RollL";
		case E_Direction.Right:
			return "RollR";
		default:
			return "Idle";
		}
	}

	public override string GetWeaponAnim(E_WeaponAction action)
	{
		switch (action)
		{
		case E_WeaponAction.Fire:
			if (BlackBoard.Cover != null)
			{
				if (BlackBoard.CoverPose == E_CoverPose.Crouch)
				{
					switch (BlackBoard.CoverPosition)
					{
					case E_CoverDirection.Left:
						return WeaponAnimations[Weapons.CurrentWeapon].FireCoverCrouchLeft;
					case E_CoverDirection.Right:
						return WeaponAnimations[Weapons.CurrentWeapon].FireCoverCrouchRight;
					default:
						return WeaponAnimations[Weapons.CurrentWeapon].FireCoverCrouchCenter;
					}
				}
				switch (BlackBoard.CoverPosition)
				{
				case E_CoverDirection.Left:
					return WeaponAnimations[Weapons.CurrentWeapon].FireCoverStandLeft;
				case E_CoverDirection.Right:
					return WeaponAnimations[Weapons.CurrentWeapon].FireCoverStandRight;
				}
			}
			if (BlackBoard.MotionType == E_MotionType.None)
			{
				return WeaponAnimations[Weapons.CurrentWeapon].FireIdle;
			}
			return WeaponAnimations[Weapons.CurrentWeapon].FireRun;
		case E_WeaponAction.Reload:
			if (BlackBoard.Cover != null)
			{
				if (BlackBoard.CoverPose == E_CoverPose.Crouch)
				{
					return WeaponAnimations[Weapons.CurrentWeapon].ReloadCoverCrouch;
				}
				switch (BlackBoard.CoverPosition)
				{
				case E_CoverDirection.Left:
					return WeaponAnimations[Weapons.CurrentWeapon].ReloadCoverStandLeft;
				case E_CoverDirection.Right:
					return WeaponAnimations[Weapons.CurrentWeapon].ReloadCoverStandRight;
				default:
					return WeaponAnimations[Weapons.CurrentWeapon].ReloadCoverStand;
				}
			}
			if (BlackBoard.MotionType == E_MotionType.None)
			{
				return WeaponAnimations[Weapons.CurrentWeapon].ReloadIdle;
			}
			return WeaponAnimations[Weapons.CurrentWeapon].ReloadRun;
		case E_WeaponAction.Arm:
			if (BlackBoard.Cover != null)
			{
				if (BlackBoard.CoverPose == E_CoverPose.Crouch)
				{
					return WeaponAnimations[Weapons.CurrentWeapon].ArmCoverCrouch;
				}
				return WeaponAnimations[Weapons.CurrentWeapon].ArmCoverStand;
			}
			return WeaponAnimations[Weapons.CurrentWeapon].ArmIdle;
		case E_WeaponAction.Disarm:
			if (BlackBoard.Cover != null)
			{
				if (BlackBoard.CoverPose == E_CoverPose.Crouch)
				{
					return WeaponAnimations[Weapons.CurrentWeapon].DisarmCoverCrouch;
				}
				return WeaponAnimations[Weapons.CurrentWeapon].DisarmCoverStand;
			}
			return WeaponAnimations[Weapons.CurrentWeapon].DisarmIdle;
		default:
			return "Idle";
		}
	}

	public override float GetWeaponAnimTime(E_WeaponType type, E_WeaponAction action)
	{
		string text;
		switch (action)
		{
		case E_WeaponAction.Arm:
			text = WeaponAnimations[type].ArmIdle;
			break;
		case E_WeaponAction.Disarm:
			text = WeaponAnimations[type].DisarmIdle;
			break;
		case E_WeaponAction.Fire:
			text = ((BlackBoard.MotionType != E_MotionType.None) ? WeaponAnimations[type].FireRun : WeaponAnimations[type].FireIdle);
			break;
		case E_WeaponAction.Reload:
			text = ((BlackBoard.MotionType != E_MotionType.None) ? WeaponAnimations[type].ReloadRun : WeaponAnimations[type].ReloadIdle);
			break;
		default:
			throw new NotImplementedException();
		}
		if (text != null && Animation[text] != null)
		{
			return Animation[text].length;
		}
		return 0f;
	}

	public override string GetInjuryAnim(E_WeaponType weapon)
	{
		if (BlackBoard.Cover != null)
		{
			if (WorldState.GetWSProperty(E_PropKey.CoverState).GetCoverState() == E_CoverState.Middle)
			{
				return (BlackBoard.CoverPose != E_CoverPose.Stand) ? "InjuryCoverCrouchIdle" : "InjuryCoverStandIdle";
			}
			if (BlackBoard.CoverPosition == E_CoverDirection.Middle)
			{
				return (BlackBoard.CoverPose != E_CoverPose.Stand) ? "InjuryCoverCrouchAimCenter" : string.Empty;
			}
			if (BlackBoard.CoverPosition == E_CoverDirection.Left)
			{
				return (BlackBoard.CoverPose != E_CoverPose.Stand) ? "InjuryCoverCrouchAimLeft" : "InjuryCoverStandAimLeft";
			}
			if (BlackBoard.CoverPosition == E_CoverDirection.Right)
			{
				return (BlackBoard.CoverPose != E_CoverPose.Stand) ? "InjuryCoverCrouchAimRight" : "InjuryCoverStandAimRight";
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
				return (BlackBoard.CoverPose != E_CoverPose.Stand) ? "DeathCoverCrouchIdle" : "DeathCoverStandIdle";
			}
			if (BlackBoard.CoverPosition == E_CoverDirection.Middle)
			{
				return (BlackBoard.CoverPose != E_CoverPose.Stand) ? "DeathCoverCrouchAimCenter" : string.Empty;
			}
			if (BlackBoard.CoverPosition == E_CoverDirection.Left)
			{
				return (BlackBoard.CoverPose != E_CoverPose.Stand) ? "DeathCoverCrouchAimLeft" : "DeathCoverStandAimLeft";
			}
			if (BlackBoard.CoverPosition == E_CoverDirection.Right)
			{
				return (BlackBoard.CoverPose != E_CoverPose.Stand) ? "DeathCoverCrouchAimRight" : "DeathCoverStandAimRight";
			}
		}
		if (BlackBoard.MotionType == E_MotionType.Run)
		{
			if (BlackBoard.MoveType == E_MoveType.StrafeLeft)
			{
				return "DeathRunL";
			}
			if (BlackBoard.MoveType == E_MoveType.StrafeRight)
			{
				return "DeathRunR";
			}
			if (BlackBoard.MoveType == E_MoveType.Forward)
			{
				return "DeathRunF";
			}
		}
		return DeathFront.Get();
	}

	public override string GetKnockdowAnim(E_KnockdownState knockdownState)
	{
		return null;
	}

	public override string GetCoverAnim(E_CoverAnim type, E_CoverPose pose, E_CoverDirection direction)
	{
		switch (type)
		{
		case E_CoverAnim.Enter:
			return (pose != E_CoverPose.Stand) ? "CoverCrouchEnter" : "CoverStandEnter";
		case E_CoverAnim.Leave:
			return (pose != E_CoverPose.Stand) ? "CoverCrouchLeave" : "CoverStandLeave";
		case E_CoverAnim.AimStart:
			switch (direction)
			{
			case E_CoverDirection.Left:
				return (pose != E_CoverPose.Stand) ? "CoverCrouchAimLeftStart" : "CoverStandAimLeftStart";
			case E_CoverDirection.Right:
				return (pose != E_CoverPose.Stand) ? "CoverCrouchAimRightStart" : "CoverStandAimRightStart";
			default:
				return (pose != E_CoverPose.Stand) ? "CoverCrouchAimCenterStart" : string.Empty;
			}
		case E_CoverAnim.AimEnd:
			switch (direction)
			{
			case E_CoverDirection.Left:
				return (pose != E_CoverPose.Stand) ? "CoverCrouchAimLeftBack" : "CoverStandAimLeftBack";
			case E_CoverDirection.Right:
				return (pose != E_CoverPose.Stand) ? "CoverCrouchAimRightBack" : "CoverStandAimRightBack";
			default:
				return (pose != E_CoverPose.Stand) ? "CoverCrouchAimCenterBack" : string.Empty;
			}
		case E_CoverAnim.JumpOver:
			return "JumpOverToIdle";
		case E_CoverAnim.LeaveLeft:
			return (pose != E_CoverPose.Stand) ? "CoverCrouchLeftLeave" : "CoverStandLeftLeave";
		case E_CoverAnim.LeaveRight:
			return (pose != E_CoverPose.Stand) ? "CoverCrouchRightLeave" : "CoverStandRightLeave";
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
					return (coverPose != E_CoverPose.Stand) ? "CoverCrouchAimLeftMaxL" : "CoverStandAimLeftMaxL";
				case E_CoverDirection.Right:
					return (coverPose != E_CoverPose.Stand) ? "CoverCrouchAimRightMaxL" : "CoverStandAimRightMaxL";
				default:
					return (coverPose != E_CoverPose.Stand) ? "CoverCrouchAimCenterMaxL" : string.Empty;
				}
			case E_AimDirection.Right:
				switch (BlackBoard.CoverPosition)
				{
				case E_CoverDirection.Left:
					return (coverPose != E_CoverPose.Stand) ? "CoverCrouchAimLeftMaxR" : "CoverStandAimLeftMaxR";
				case E_CoverDirection.Right:
					return (coverPose != E_CoverPose.Stand) ? "CoverCrouchAimRightMaxR" : "CoverStandAimRightMaxR";
				default:
					return (coverPose != E_CoverPose.Stand) ? "CoverCrouchAimCenterMaxR" : string.Empty;
				}
			case E_AimDirection.Up:
				switch (BlackBoard.CoverPosition)
				{
				case E_CoverDirection.Left:
					return (coverPose != E_CoverPose.Stand) ? "CoverCrouchAimLeftMaxU" : "CoverStandAimLeftMaxU";
				case E_CoverDirection.Right:
					return (coverPose != E_CoverPose.Stand) ? "CoverCrouchAimRightMaxU" : "CoverStandAimRightMaxU";
				default:
					return (coverPose != E_CoverPose.Stand) ? "CoverCrouchAimCenterMaxU" : string.Empty;
				}
			case E_AimDirection.Down:
				switch (BlackBoard.CoverPosition)
				{
				case E_CoverDirection.Left:
					return (coverPose != E_CoverPose.Stand) ? "CoverCrouchAimLeftMaxD" : "CoverStandAimLeftMaxD";
				case E_CoverDirection.Right:
					return (coverPose != E_CoverPose.Stand) ? "CoverCrouchAimRightMaxD" : "CoverStandAimRightMaxD";
				default:
					return (coverPose != E_CoverPose.Stand) ? "CoverCrouchAimCenterMaxD" : string.Empty;
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
}
