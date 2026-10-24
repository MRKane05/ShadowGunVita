using System;
using UnityEngine;

[Serializable]
public class AnimSetMutantElite : AnimSet
{
	private BlackBoard BlackBoard;

	private Animation Animation;

	private static Randomizer InjuryFront;

	private static Randomizer DeathFront;

	private static Randomizer DeathInMove;

	private static Randomizer DeathShotgun;

	static AnimSetMutantElite()
	{
		InjuryFront = new Randomizer();
		DeathFront = new Randomizer();
		DeathInMove = new Randomizer();
		DeathShotgun = new Randomizer();
		InjuryFront.Add("InjuryIdleFront1");
		InjuryFront.Add("InjuryIdleFront2");
		InjuryFront.Add("InjuryIdleFront3");
		InjuryFront.Add("InjuryIdleFront4");
		DeathFront.Add("DeathIdleFront1");
		DeathFront.Add("DeathIdleFront2");
		DeathFront.Add("DeathIdleHeadShot");
		DeathShotgun.Add("DeathIdleFront3");
		DeathShotgun.Add("DeathIdleHeadShot");
		DeathInMove.Add("DeathWalkF");
		DeathInMove.Add("DeathRunF");
	}

	private void Awake()
	{
		BlackBoard = GetComponent<AgentHuman>().BlackBoard;
		Animation = base.GetComponent<Animation>();
		Animation["InjuryIdleBig"].speed = 0.7f;
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
			return "WalkF";
		}
		if (BlackBoard.MoveType == E_MoveType.Backward)
		{
			return "WalkB";
		}
		if (BlackBoard.MoveType == E_MoveType.StrafeLeft)
		{
			return "WalkL";
		}
		if (BlackBoard.MoveType == E_MoveType.StrafeRight)
		{
			return "WalkR";
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
			if (BlackBoard.MotionType == E_MotionType.None)
			{
				return "IdleFireSMG";
			}
			return "RunFireSMG";
		case E_WeaponAction.Reload:
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
		if (type == E_WeaponType.GrenadeLauncher)
		{
			return (BlackBoard.MotionType != E_MotionType.None) ? Animation[array[1, (int)action]].length : Animation[array[0, (int)action]].length;
		}
		return 0f;
	}

	public override string GetInjuryAnim(E_WeaponType weapon)
	{
		return InjuryFront.Get();
	}

	public override string GetDeathAnim(E_WeaponType weapon)
	{
		if (BlackBoard.MotionType != E_MotionType.None)
		{
			return DeathInMove.Get();
		}
		if (weapon == E_WeaponType.Shotgun)
		{
			return DeathShotgun.Get();
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
		throw new NotImplementedException();
	}

	public override string GetInjuryCritAnim()
	{
		return "InjuryIdleBig";
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
