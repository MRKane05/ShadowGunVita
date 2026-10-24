using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public abstract class AnimSet : MonoBehaviour
{
	protected class Randomizer
	{
		private List<string> List1 = new List<string>();

		private List<string> List2 = new List<string>();

		private List<string> Current;

		public Randomizer()
		{
			Current = List1;
		}

		public void Add(string s)
		{
			List1.Add(s);
		}

		public string Get()
		{
			int index = Random.Range(0, Current.Count);
			string text = Current[index];
			if (Current.Count == 1)
			{
				if (Current == List1)
				{
					Current = List2;
				}
				else
				{
					Current = List1;
				}
			}
			else
			{
				Current.RemoveAt(index);
				if (Current == List1)
				{
					List2.Add(text);
				}
				else
				{
					List1.Add(text);
				}
			}
			return text;
		}
	}

	public enum E_CoverAnim
	{
		Enter = 0,
		Leave = 1,
		AimStart = 2,
		AimLoop = 3,
		AimEnd = 4,
		Fire = 5,
		JumpOver = 6,
		LeaveLeft = 7,
		LeaveRight = 8
	}

	public enum E_TeleportAnim
	{
		In = 0,
		Out = 1
	}

	public abstract string GetIdleAnim();

	public abstract string GetIdleActionAnim();

	public abstract string GetMoveAnim();

	public abstract string GetRotateAnim(E_RotationType rotationType);

	public abstract string GetDodgeAnim(E_StrafeDirection dir);

	public abstract string GetRollAnim(E_Direction moveType);

	public abstract string GetBlockAnim(E_BlockState block);

	public abstract string GetKnockdowAnim(E_KnockdownState knockdownState);

	public abstract string GetWeaponAnim(E_WeaponAction action);

	public abstract float GetWeaponAnimTime(E_WeaponType type, E_WeaponAction action);

	public abstract string GetInjuryAnim(E_WeaponType weapon);

	public abstract string GetInjuryCritAnim();

	public abstract string GetDeathAnim(E_WeaponType weapon);

	public abstract string GetTeleportAnim(E_TeleportAnim type);

	public abstract string GetCoverAnim(E_CoverAnim type, E_CoverPose pos, E_CoverDirection direction);

	public abstract string GetAimAnim(E_AimDirection direction);
}
