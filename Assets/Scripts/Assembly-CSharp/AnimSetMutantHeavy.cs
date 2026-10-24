using System;
using UnityEngine;

[Serializable]
public class AnimSetMutantHeavy : AnimSet
{
	private BlackBoard BlackBoard;

	private Animation Animation;

	private static Randomizer InjuryFront;

	private static Randomizer DeathFront;

	static AnimSetMutantHeavy()
	{
		InjuryFront = new Randomizer();
		DeathFront = new Randomizer();
		InjuryFront.Add("InjuryIdleFront1");
		InjuryFront.Add("InjuryIdleFront2");
		InjuryFront.Add("InjuryIdleFront3");
		InjuryFront.Add("InjuryIdleFront4");
		DeathFront.Add("DeathIdleFront1");
		DeathFront.Add("DeathIdleFront2");
		DeathFront.Add("DeathIdleFront3");
	}

	private void Awake()
	{
		BlackBoard = GetComponent<AgentHuman>().BlackBoard;
		Animation = base.GetComponent<Animation>();
	}

	private void Start()
	{
	}

	public override string GetBlockAnim(E_BlockState state)
	{
		return null;
	}

	public override string GetIdleAnim()
	{
		return "Idle";
	}

	public override string GetIdleActionAnim()
	{
		return "Idle";
	}

	public override string GetMoveAnim()
	{
		if (BlackBoard.MoveType == E_MoveType.Forward)
		{
			return (BlackBoard.MotionType != E_MotionType.Walk) ? "RunF" : "WalkF";
		}
		if (BlackBoard.MoveType == E_MoveType.Backward)
		{
			return (BlackBoard.MotionType != E_MotionType.Walk) ? "RunB" : "WalkB";
		}
		if (BlackBoard.MoveType == E_MoveType.StrafeLeft)
		{
			return (BlackBoard.MotionType != E_MotionType.Walk) ? "RunL" : "WalkL";
		}
		if (BlackBoard.MoveType == E_MoveType.StrafeRight)
		{
			return (BlackBoard.MotionType != E_MotionType.Walk) ? "RunR" : "WalkR";
		}
		return "Idle";
	}

	public override string GetRotateAnim(E_RotationType rotationType)
	{
		return (rotationType != E_RotationType.Left) ? "TurnR" : "TurnL";
	}

	public override string GetDodgeAnim(E_StrafeDirection dir)
	{
		return (dir != E_StrafeDirection.Left) ? "WalkR" : "WalkL";
	}

	public override string GetWeaponAnim(E_WeaponAction action)
	{
		switch (action)
		{
		case E_WeaponAction.Fire:
			return "IdleFireLauncher";
		case E_WeaponAction.Reload:
			return "ReloadIdleLauncher";
		default:
			return "Idle";
		}
	}

	public override float GetWeaponAnimTime(E_WeaponType type, E_WeaponAction action)
	{
		switch (action)
		{
		case E_WeaponAction.Reload:
			return Animation["ReloadIdleLauncher"].length;
		case E_WeaponAction.Fire:
			return Animation["IdleFireLauncher"].length;
		default:
			return 0f;
		}
	}

	public override string GetInjuryAnim(E_WeaponType weapon)
	{
		return InjuryFront.Get();
	}

	public override string GetDeathAnim(E_WeaponType weapon)
	{
		if (BlackBoard.MotionType == E_MotionType.Run)
		{
			return "DeathRunF";
		}
		if (BlackBoard.MotionType == E_MotionType.Walk)
		{
			return "DeathWalkF";
		}
		return DeathFront.Get();
	}

	public override string GetKnockdowAnim(E_KnockdownState knockdownState)
	{
		throw new NotImplementedException();
	}

	public override string GetCoverAnim(E_CoverAnim type, E_CoverPose pose, E_CoverDirection direction)
	{
		throw new NotImplementedException();
	}

	public override string GetTeleportAnim(E_TeleportAnim type)
	{
		return (type != E_TeleportAnim.In) ? "TeleportEnd" : "TeleportStart";
	}

	public override string GetInjuryCritAnim()
	{
		throw new NotImplementedException();
	}

	public override string GetAimAnim(E_AimDirection direction)
	{
		switch (direction)
		{
		case E_AimDirection.Down:
			return "AimD";
		case E_AimDirection.Up:
			return "AimU";
		default:
			throw new NotImplementedException();
		}
	}

	public override string GetRollAnim(E_Direction moveType)
	{
		throw new NotImplementedException();
	}
}
