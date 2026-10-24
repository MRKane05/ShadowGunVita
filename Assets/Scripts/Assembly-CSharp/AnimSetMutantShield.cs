using System;
using UnityEngine;

[Serializable]
public class AnimSetMutantShield : AnimSet
{
	private BlackBoard BlackBoard;

	private Animation Animation;

	private static Randomizer InjuryWithoutShield;

	private static Randomizer DeathFront;

	static AnimSetMutantShield()
	{
		InjuryWithoutShield = new Randomizer();
		DeathFront = new Randomizer();
		InjuryWithoutShield.Add("InjuryUniversal1");
		InjuryWithoutShield.Add("InjuryUniversal2");
		InjuryWithoutShield.Add("InjuryUniversal3");
		DeathFront.Add("DeathFront1");
		DeathFront.Add("DeathFront2");
	}

	private void Awake()
	{
		BlackBoard = GetComponent<AgentHuman>().BlackBoard;
		Animation = base.GetComponent<Animation>();
		Animation["WalkLoopF"].speed = 1.5f;
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
		throw new NotImplementedException();
	}

	public override string GetMoveAnim()
	{
		if (BlackBoard.MotionType == E_MotionType.AnimationDrive)
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
		}
		return "WalkLoopF";
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
			return "IdleFireSMG";
		case E_WeaponAction.Reload:
			return "IdleReloadSMG";
		default:
			return "Idle";
		}
	}

	public override float GetWeaponAnimTime(E_WeaponType type, E_WeaponAction action)
	{
		string[,] array = new string[1, 4] { { "IdleFireSMG", "IdleFireSMG", "IdleFireSMG", "IdleReloadSMG" } };
		if (type == E_WeaponType.PistolMutant)
		{
			return (BlackBoard.MotionType != E_MotionType.None) ? Animation[array[1, (int)action]].length : Animation[array[0, (int)action]].length;
		}
		return 0f;
	}

	public override string GetInjuryAnim(E_WeaponType weapon)
	{
		return InjuryWithoutShield.Get();
	}

	public override string GetDeathAnim(E_WeaponType weapon)
	{
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
		return "UncoverWalk";
	}

	public override string GetAimAnim(E_AimDirection direction)
	{
		return null;
	}

	public override string GetRollAnim(E_Direction moveType)
	{
		throw new NotImplementedException();
	}
}
