using UnityEngine;

internal class GOAPGoalWeaponReloadAI : GOAPGoal
{
	public GOAPGoalWeaponReloadAI(AgentHuman owner)
		: base(E_GOAPGoals.WeaponReloadAI, owner)
	{
	}

	public override void InitGoal()
	{
	}

	public override float GetMaxRelevancy()
	{
		return base.Owner.BlackBoard.GoapSetup.ReloadRelevancy;
	}

	public override void CalculateGoalRelevancy()
	{
		base.GoalRelevancy = 0f;
		WeaponBase currentWeapon = base.Owner.WeaponComponent.GetCurrentWeapon();
		if (currentWeapon.WeaponAmmo != 0 && !currentWeapon.IsBusy() && !base.Owner.BlackBoard.BusyAction)
		{
			base.GoalRelevancy = base.Owner.BlackBoard.GoapSetup.ReloadRelevancy * (1f - (float)currentWeapon.ClipAmmo / (float)currentWeapon.MaxAmmoInClip);
			base.GoalRelevancy *= base.GoalRelevancy;
			if (base.GoalRelevancy < 0.4f)
			{
				base.GoalRelevancy = 0f;
			}
		}
	}

	public override void SetDisableTime()
	{
		base.NextEvaluationTime = base.Owner.BlackBoard.GoapSetup.ReloadDelay + Time.timeSinceLevelLoad;
	}

	public override void SetWSSatisfactionForPlanning(WorldState worldState)
	{
		worldState.SetWSProperty(E_PropKey.WeaponLoaded, true);
	}

	public override void ChangeCurrentWSForPlanning(WorldState worldState)
	{
		worldState.SetWSProperty(E_PropKey.WeaponLoaded, false);
	}

	public override bool IsWSSatisfiedForPlanning(WorldState worldState)
	{
		WorldStateProp wSProperty = worldState.GetWSProperty(E_PropKey.WeaponLoaded);
		return wSProperty.GetBool();
	}

	public override bool IsSatisfied()
	{
		return IsPlanFinished();
	}
}
