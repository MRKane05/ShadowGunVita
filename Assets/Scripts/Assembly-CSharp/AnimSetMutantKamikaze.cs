using System;
using UnityEngine;

[Serializable]
public class AnimSetMutantKamikaze : AnimSet
{
	private BlackBoard BlackBoard;

	private Animation Animation;

	private static Randomizer Injury;

	private static Randomizer DeathRun;

	static AnimSetMutantKamikaze()
	{
		Injury = new Randomizer();
		DeathRun = new Randomizer();
		Injury.Add("InjuryUniversal1");
		Injury.Add("InjuryUniversal2");
		Injury.Add("InjuryUniversal3");
		DeathRun.Add("DeathRun1");
		DeathRun.Add("DeathRun2");
	}

	private void Awake()
	{
		BlackBoard = GetComponent<AgentHuman>().BlackBoard;
		Animation = base.GetComponent<Animation>();
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
			Animation["RunF"].speed = 0.8f;
			return "RunF";
		}
		if (BlackBoard.MoveType == E_MoveType.Backward)
		{
			throw new NotImplementedException();
		}
		if (BlackBoard.MoveType == E_MoveType.StrafeLeft)
		{
			return "RunL";
		}
		if (BlackBoard.MoveType == E_MoveType.StrafeRight)
		{
			return "RunR";
		}
		throw new NotImplementedException();
	}

	public override string GetRotateAnim(E_RotationType rotationType)
	{
		return (rotationType != E_RotationType.Left) ? "TurnR" : "TurnL";
	}

	public override string GetDodgeAnim(E_StrafeDirection dir)
	{
		throw new NotImplementedException();
	}

	public override string GetWeaponAnim(E_WeaponAction action)
	{
		return "ThrowAndTurn";
	}

	public override float GetWeaponAnimTime(E_WeaponType type, E_WeaponAction action)
	{
		return 0f;
	}

	public override string GetInjuryAnim(E_WeaponType weapon)
	{
		return Injury.Get();
	}

	public override string GetDeathAnim(E_WeaponType weapon)
	{
		if (BlackBoard.MotionType == E_MotionType.Run)
		{
			return DeathRun.Get();
		}
		return "DeathIdle";
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
