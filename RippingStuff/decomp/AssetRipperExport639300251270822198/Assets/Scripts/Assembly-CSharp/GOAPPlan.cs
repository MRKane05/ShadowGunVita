using System.Collections.Generic;

public class GOAPPlan
{
	private List<GOAPAction> m_Actions = new List<GOAPAction>();

	private int CurrentStep;

	public int NumberOfSteps
	{
		get
		{
			return m_Actions.Count;
		}
	}

	public int CurrentStepIndex
	{
		get
		{
			return CurrentStep;
		}
	}

	public GOAPAction CurrentAction
	{
		get
		{
			if (IsDone())
			{
				return null;
			}
			return m_Actions[CurrentStep];
		}
	}

	public void PushBack(GOAPAction action)
	{
		m_Actions.Add(action);
	}

	public void Update()
	{
		if (!IsDone())
		{
			m_Actions[CurrentStep].Update();
		}
	}

	public bool IsPlanStepComplete()
	{
		if (IsDone())
		{
			return true;
		}
		return m_Actions[CurrentStep].IsActionComplete();
	}

	public bool IsDone()
	{
		return CurrentStep >= m_Actions.Count;
	}

	public bool IsPlanStepInterruptible()
	{
		if (IsDone())
		{
			return false;
		}
		return m_Actions[CurrentStep].Interruptible;
	}

	public bool IsPlanValid()
	{
		if (IsDone())
		{
			return false;
		}
		return CurrentAction.ValidateAction();
	}

	public bool Activate(AgentHuman ai, GOAPGoal goal)
	{
		if (m_Actions.Count == 0)
		{
			return false;
		}
		CurrentStep = 0;
		GOAPAction currentAction = CurrentAction;
		if (currentAction != null)
		{
			if (!currentAction.ValidateContextPreconditions(ai.WorldState, false))
			{
				return false;
			}
			currentAction.Activate();
		}
		return true;
	}

	public void Deactivate()
	{
		if (CurrentAction != null)
		{
			CurrentAction.Deactivate();
		}
		m_Actions.Clear();
		CurrentStep = 0;
	}

	public bool AdvancePlan()
	{
		if (!IsDone())
		{
			CurrentAction.Deactivate();
			CurrentStep++;
			if (IsDone())
			{
				return true;
			}
			if (!CurrentAction.ValidateContextPreconditions(null, false))
			{
				return false;
			}
			CurrentAction.Activate();
			return true;
		}
		return true;
	}

	public override string ToString()
	{
		string text = "GOAPPlan : ";
		for (int i = 0; i < m_Actions.Count; i++)
		{
			string text2 = text;
			text = text2 + (i + 1) + ". " + m_Actions[i].Type.ToString() + " ";
		}
		return text;
	}
}
