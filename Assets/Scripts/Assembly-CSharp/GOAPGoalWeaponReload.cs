using UnityEngine;

internal class GOAPGoalWeaponReload : GOAPGoal
{
	public GOAPGoalWeaponReload(AgentHuman owner)
		: base(E_GOAPGoals.WeaponReload, owner)
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
		if (!base.Owner.BlackBoard.ComlinkOn)
		{
			WorldStateProp wSProperty = base.Owner.WorldState.GetWSProperty(E_PropKey.WeaponLoaded);
			if (!(wSProperty == null) && !wSProperty.GetBool() && base.Owner.WeaponComponent.GetCurrentWeapon().WeaponAmmo != 0 && !base.Owner.WeaponComponent.GetCurrentWeapon().IsBusy())
			{
				base.GoalRelevancy = base.Owner.BlackBoard.GoapSetup.ReloadRelevancy;
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
