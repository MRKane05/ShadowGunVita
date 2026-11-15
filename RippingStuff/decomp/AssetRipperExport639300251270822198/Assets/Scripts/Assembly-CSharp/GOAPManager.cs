using System.Collections.Generic;
using UnityEngine;

internal class GOAPManager
{
	private Dictionary<E_GOAPGoals, GOAPGoal> Goals = new Dictionary<E_GOAPGoals, GOAPGoal>();

	public GOAPGoal CurrentGoal;

	private AgentHuman Owner;

	private AStarEngine AStar;

	private AStarStorage Storage;

	private AStarGOAPMap Map;

	private AStarGOAPGoal Goal;

	public GOAPManager(AgentHuman ai)
	{
		Owner = ai;
		Map = new AStarGOAPMap();
		Storage = new AStarStorage();
		Goal = new AStarGOAPGoal();
		AStar = new AStarEngine();
	}

	public void Initialize()
	{
		Map.Initialise(Owner);
		Map.BuildActionsEffectsTable();
		AStar.Setup(Goal, Storage, Map);
	}

	public void Reset()
	{
		if (CurrentGoal != null)
		{
			CurrentGoal.Deactivate();
			CurrentGoal = null;
		}
		foreach (KeyValuePair<E_GOAPGoals, GOAPGoal> goal in Goals)
		{
			goal.Value.Reset();
		}
	}

	public void Clean()
	{
		AStar.Cleanup();
		AStar = null;
		Map = null;
		Storage = null;
		Goal = null;
	}

	public GOAPGoal AddGoal(E_GOAPGoals type)
	{
		if (!Goals.ContainsKey(type))
		{
			GOAPGoal gOAPGoal = GOAPGoalFactory.Create(type, Owner);
			Goals.Add(type, gOAPGoal);
			return gOAPGoal;
		}
		return null;
	}

	public GOAPGoal GetGoal(E_GOAPGoals type)
	{
		if (Goals.ContainsKey(type))
		{
			return Goals[type];
		}
		return null;
	}

	public void UpdateCurrentGoal()
	{
		if (CurrentGoal == null)
		{
			return;
		}
		if (CurrentGoal.UpdateGoal())
		{
			if (CurrentGoal.ReplanRequired())
			{
				ReplanCurrentGoal();
			}
			if (CurrentGoal.IsPlanFinished())
			{
				CurrentGoal.Deactivate();
				CurrentGoal = null;
			}
		}
		else
		{
			CurrentGoal.Deactivate();
			CurrentGoal = null;
		}
	}

	public void ManageGoals()
	{
		if (CurrentGoal != null)
		{
			if (CurrentGoal.ReplanRequired())
			{
				if (!ReplanCurrentGoal())
				{
					FindNewGoal();
				}
			}
			else if (!CurrentGoal.IsPlanValid())
			{
				FindNewGoal();
			}
			else if (CurrentGoal.IsSatisfied())
			{
				FindNewGoal();
			}
			else if (CurrentGoal.IsPlanInterruptible())
			{
				FindMostImportantGoal();
			}
		}
		else
		{
			FindNewGoal();
		}
	}

	private bool ReplanCurrentGoal()
	{
		if (CurrentGoal == null)
		{
			return false;
		}
		CurrentGoal.ReplanReset();
		GOAPPlan gOAPPlan = BuildPlan(CurrentGoal);
		if (gOAPPlan == null)
		{
			return false;
		}
		CurrentGoal.Activate(gOAPPlan);
		return true;
	}

	private void FindNewGoal()
	{
		if (CurrentGoal != null)
		{
			CurrentGoal.Deactivate();
			CurrentGoal = null;
		}
		while (CurrentGoal == null)
		{
			GOAPGoal mostImportantGoal = GetMostImportantGoal(0f);
			if (mostImportantGoal == null)
			{
				break;
			}
			if (mostImportantGoal.FailChance > Random.Range(0, 100))
			{
				mostImportantGoal.SetDisableTime();
				continue;
			}
			CreatePlan(mostImportantGoal);
			if (CurrentGoal == null)
			{
				mostImportantGoal.SetDisableTime();
			}
		}
	}

	private void FindMostImportantGoal()
	{
		GOAPGoal mostImportantGoal = GetMostImportantGoal(CurrentGoal.GoalRelevancy);
		if (mostImportantGoal != null)
		{
			if (mostImportantGoal.FailChance > Random.Range(0, 100))
			{
				mostImportantGoal.SetDisableTime();
			}
			else if (mostImportantGoal != CurrentGoal)
			{
				CreatePlan(mostImportantGoal);
			}
		}
	}

	private void CreatePlan(GOAPGoal goal)
	{
		GOAPPlan gOAPPlan = BuildPlan(goal);
		if (gOAPPlan == null)
		{
			goal.SetDisableTime();
			return;
		}
		if (CurrentGoal != null)
		{
			CurrentGoal.Deactivate();
			CurrentGoal = null;
		}
		CurrentGoal = goal;
		CurrentGoal.Activate(gOAPPlan);
	}

	private GOAPGoal GetMostImportantGoal(float minRelevancy)
	{
		GOAPGoal result = null;
		float num = minRelevancy;
		foreach (KeyValuePair<E_GOAPGoals, GOAPGoal> goal in Goals)
		{
			GOAPGoal value = goal.Value;
			if (!value.IsDisabled() && !value.IsDisabledForEveryone() && !(num >= value.GetMaxRelevancy()))
			{
				if (!value.Active)
				{
					value.CalculateGoalRelevancy();
				}
				if (value.GoalRelevancy > num)
				{
					num = value.GoalRelevancy;
					result = value;
				}
			}
		}
		return result;
	}

	public GOAPPlan BuildPlan(GOAPGoal goal)
	{
		if (goal == null)
		{
			return null;
		}
		Map.Initialise(Owner);
		Goal.Initialise(Owner, Map, goal);
		Storage.ResetStorage(Map);
		AStar.End = -1;
		AStar.RunAStar(Owner);
		AStarNode aStarNode = AStar.CurrentNode;
		if (aStarNode == null || aStarNode.NodeID == -1)
		{
			return null;
		}
		GOAPPlan gOAPPlan = new GOAPPlan();
		while (aStarNode.NodeID != -1)
		{
			GOAPAction action = Map.GetAction(aStarNode.NodeID);
			if (action == null)
			{
				Debug.LogError(Time.timeSinceLevelLoad + " " + goal.ToString() + ": canot find action (" + aStarNode.NodeID + ")");
				return null;
			}
			gOAPPlan.PushBack(action);
			aStarNode = aStarNode.Parent;
		}
		if (gOAPPlan.IsDone())
		{
			Debug.LogError(Time.timeSinceLevelLoad + " " + goal.ToString() + ": plan is already  done !!! (" + gOAPPlan.CurrentStepIndex + "," + gOAPPlan.NumberOfSteps + ")");
			return null;
		}
		return gOAPPlan;
	}
}
