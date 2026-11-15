using UnityEngine;

internal class GOAPActionAdvanceToCoveredTarget : GOAPAction
{
	private AgentActionGoTo Action;

	private Vector3 FinalPos;

	public GOAPActionAdvanceToCoveredTarget(AgentHuman owner)
		: base(E_GOAPAction.AdvanceToCoveredTarget, owner)
	{
	}

	public override void InitAction()
	{
		WorldEffects.SetWSProperty(E_PropKey.AtTargetPos, true);
		WorldPreconditions.SetWSProperty(E_PropKey.CoverState, E_CoverState.None);
		Cost = 5f;
		Precedence = 70;
	}

	public override bool ValidateContextPreconditions(WorldState current, bool planning)
	{
		Fact validFact = Owner.Memory.GetValidFact(E_EventTypes.EnemyHideInCover);
		if (validFact == null)
		{
			return false;
		}
		return true;
	}

	public override void Update()
	{
	}

	public override void Activate()
	{
		base.Activate();
		Fact validFact = Owner.Memory.GetValidFact(E_EventTypes.EnemyHideInCover);
		Action = AgentActionFactory.Create(AgentActionFactory.E_Type.Goto) as AgentActionGoTo;
		Action.Motion = E_MotionType.Walk;
		Cover cover = validFact.Cover;
		bool flag = cover.CoverFlags.Get(Cover.E_CoverFlags.LeftCrouch) || cover.CoverFlags.Get(Cover.E_CoverFlags.LeftStand);
		bool flag2 = cover.CoverFlags.Get(Cover.E_CoverFlags.RightCrouch) || cover.CoverFlags.Get(Cover.E_CoverFlags.RightStand);
		bool flag3 = cover.CoverFlags.Get(Cover.E_CoverFlags.UpCrouch);
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		if (!flag3)
		{
			if (flag2)
			{
				num3 = (flag ? (num2 = 50) : 100);
			}
			else
			{
				num2 = 100;
			}
		}
		else if (!flag)
		{
			if (!flag2)
			{
				num = 100;
			}
			else
			{
				num3 = (num = 50);
			}
		}
		else if (!flag2)
		{
			if (!flag)
			{
				num = 100;
			}
			else
			{
				num2 = (num = 50);
			}
		}
		else
		{
			num3 = (num2 = (num = 33));
		}
		int num4 = Random.Range(0, 100);
		if (validFact.CoverDirection == E_CoverDirection.Middle)
		{
			if (num > 0 && num2 > 0)
			{
				num2 += num / 3;
			}
			if (num > 0 && num3 > 0)
			{
				num3 += num / 3;
			}
		}
		else if (validFact.CoverDirection == E_CoverDirection.Left)
		{
			if (num2 > 0 && num > 0)
			{
				num += num2 / 3;
			}
			if (num2 > 0 && num3 > 0)
			{
				num3 += num2 / 3;
			}
		}
		else
		{
			if (num3 > 0 && num > 0)
			{
				num += num3 / 3;
			}
			if (num3 > 0 && num2 > 0)
			{
				num2 += num3 / 3;
			}
		}
		if (num4 < num2)
		{
			Action.FinalPosition = cover.LeftEdge + cover.Left * 2f;
		}
		else if (num4 < num3)
		{
			Action.FinalPosition = cover.RightEdge + cover.Right * 2f;
		}
		else
		{
			Action.FinalPosition = cover.Transform.position - cover.Forward * 2f;
		}
		Action.LookTarget = cover.transform;
		Owner.BlackBoard.ActionAdd(Action);
		Owner.WorldState.SetWSProperty(E_PropKey.AtTargetPos, false);
	}

	public override void Deactivate()
	{
		Owner.WorldState.SetWSProperty(E_PropKey.AtTargetPos, true);
		if (Action != null && Action.IsActive())
		{
			AgentActionIdle action = AgentActionFactory.Create(AgentActionFactory.E_Type.Idle) as AgentActionIdle;
			Owner.BlackBoard.ActionAdd(action);
		}
		base.Deactivate();
	}

	public override bool IsActionComplete()
	{
		if (Action != null && Action.IsSuccess())
		{
			WorldEffects.SetWSProperty(E_PropKey.AtTargetPos, true);
			return true;
		}
		return false;
	}

	public override bool ValidateAction()
	{
		if (Action != null && Action.IsFailed())
		{
			return false;
		}
		Fact validFact = Owner.Memory.GetValidFact(E_EventTypes.EnemyInjuredMe);
		if (validFact != null && validFact.Belief > 0.9f)
		{
			return false;
		}
		validFact = Owner.Memory.GetValidFact(E_EventTypes.EnemySee);
		if (validFact != null && validFact.Belief < 0.5f)
		{
			return false;
		}
		return true;
	}
}
