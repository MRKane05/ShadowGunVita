using UnityEngine;

public abstract class GOAPGoal
{
	private GOAPPlan Plan;

	private static int id;

	public int UID;

	public AgentHuman Owner { get; private set; }

	public float GoalRelevancy { get; protected set; }

	public int FailChance { get; protected set; }

	public E_GOAPGoals GoalType { get; private set; }

	public bool Active { get; private set; }

	protected float NextEvaluationTime { get; set; }

	protected virtual float DisabledForEverybodyTimer { get; set; }

	protected virtual float DisabledForEverybodyDelay { get; set; }

	protected GOAPGoal(E_GOAPGoals type, AgentHuman ai)
	{
		DisabledForEverybodyDelay = 0f;
		GoalType = type;
		Owner = ai;
		FailChance = 0;
	}

	public abstract void SetWSSatisfactionForPlanning(WorldState worldState);

	public abstract bool IsWSSatisfiedForPlanning(WorldState worldState);

	public virtual void ChangeCurrentWSForPlanning(WorldState worldState)
	{
	}

	public abstract float GetMaxRelevancy();

	public abstract void CalculateGoalRelevancy();

	public void ClearGoalRelevancy()
	{
		GoalRelevancy = 0f;
	}

	public virtual void SetDisableTime()
	{
		NextEvaluationTime = Time.timeSinceLevelLoad + Random.Range(0.1f, 0.2f);
	}

	public virtual bool ReplanRequired()
	{
		return false;
	}

	public abstract bool IsSatisfied();

	public bool IsDisabled()
	{
		return NextEvaluationTime >= Time.timeSinceLevelLoad;
	}

	private void DisableGoalForEveryone()
	{
		DisabledForEverybodyTimer = Time.timeSinceLevelLoad + Random.Range(DisabledForEverybodyDelay * 0.5f, DisabledForEverybodyDelay * 1.5f);
	}

	public bool IsDisabledForEveryone()
	{
		return DisabledForEverybodyTimer > Time.timeSinceLevelLoad;
	}

	public abstract void InitGoal();

	public bool UpdateGoal()
	{
		if (Plan == null)
		{
			return false;
		}
		Plan.Update();
		if (Plan.IsPlanStepComplete())
		{
			return Plan.AdvancePlan();
		}
		return true;
	}

	public virtual bool Activate(GOAPPlan plan)
	{
		UID = ++id;
		Active = true;
		Plan = plan;
		DisableGoalForEveryone();
		return Plan.Activate(Owner, this);
	}

	public virtual void ReplanReset()
	{
		Active = false;
		if (Plan != null)
		{
			Plan.Deactivate();
		}
		Plan = null;
	}

	public virtual void Reset()
	{
		Active = false;
		if (Plan != null)
		{
			Plan.Deactivate();
		}
		Plan = null;
		ClearGoalRelevancy();
		NextEvaluationTime = 0f;
		DisabledForEverybodyTimer = 0f;
	}

	public virtual void Deactivate()
	{
		Active = false;
		if (Plan != null)
		{
			Plan.Deactivate();
		}
		Plan = null;
		ClearGoalRelevancy();
		SetDisableTime();
	}

	public virtual bool IsPlanInterruptible()
	{
		return Plan == null || Plan.IsPlanStepInterruptible();
	}

	public virtual bool IsPlanValid()
	{
		return Plan != null && Plan.IsPlanValid();
	}

	public bool IsPlanFinished()
	{
		return Plan == null || Plan.IsDone();
	}

	public virtual void HandlePlanBuildFailure()
	{
		ClearGoalRelevancy();
	}

	public override string ToString()
	{
		return base.ToString() + "(Releavancy: " + GoalRelevancy + ((!Active) ? " Deactive " : "Active ") + ((!IsDisabled()) ? " Enabled " : " Disabled ") + ")";
	}
}
