using UnityEngine;

public class AgentActionCoverLeave : AgentAction
{
	public enum E_Type
	{
		Back = 0,
		Jump = 1,
		Left = 2,
		Right = 3
	}

	public E_Type TypeOfLeave;

	public Vector3 FinalViewDirection;

	public Cover Cover;

	public AgentActionCoverLeave()
		: base(AgentActionFactory.E_Type.CoverLeave)
	{
	}

	public override void Reset()
	{
		TypeOfLeave = E_Type.Back;
		Cover = null;
	}
}
