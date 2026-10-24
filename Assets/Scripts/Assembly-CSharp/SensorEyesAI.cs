using UnityEngine;

public class SensorEyesAI : SensorBase
{
	private AgentHuman MyEnemy;

	public float FieldOfView = 120f;

	public SensorEyesAI(AgentHuman owner)
		: base(owner)
	{
		base.Owner.BlackBoard.VisibleTarget = null;
		MyEnemy = Player.Instance.Owner;
	}

	public override void Update()
	{
		if (base.Owner.BlackBoard.Stop)
		{
			return;
		}
		base.Owner.WorldState.SetWSProperty(E_PropKey.SeeEnemy, false);
		base.Owner.WorldState.SetWSProperty(E_PropKey.LookingAtTarget, false);
		base.Owner.WorldState.SetWSProperty(E_PropKey.AheadOfEnemy, false);
		base.Owner.WorldState.SetWSProperty(E_PropKey.EnemyLookingAtMe, false);
		base.Owner.BlackBoard.VisibleTarget = null;
		if (!MyEnemy.IsAlive)
		{
			return;
		}
		Vector3 vector = ((!base.Owner.IsInCover) ? base.Owner.TransformEye.position : base.Owner.BlackBoard.Cover.GetEyePos(base.Owner.BlackBoard.CoverPosition));
		Vector3 vector2 = MyEnemy.TransformTarget.position - vector;
		float magnitude = vector2.magnitude;
		base.Owner.BlackBoard.DistanceToTarget = magnitude;
		base.Owner.BlackBoard.DirToTarget = vector2;
		Vector3 vector3 = ((!base.Owner.BlackBoard.Cover) ? base.Owner.Forward : base.Owner.BlackBoard.Cover.Forward);
		if (magnitude > base.Owner.BlackBoard.SightRange || (base.Owner.BlackBoard.Cover == null && Vector3.Angle(vector3, vector2) > base.Owner.BlackBoard.SightFov))
		{
			SendLostEvent(MyEnemy);
			return;
		}
		vector2.Normalize();
		RaycastHit[] array = Physics.RaycastAll(vector, vector2, magnitude);
		RaycastHit[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			RaycastHit raycastHit = array2[i];
			if (!(raycastHit.transform == base.Owner.Transform) && !raycastHit.collider.isTrigger)
			{
				if (raycastHit.transform == MyEnemy.Transform)
				{
					break;
				}
				SendLostEvent(MyEnemy);
				return;
			}
		}
		SendSeeEvent(MyEnemy);
		if (base.Owner.WorldState.GetWSProperty(E_PropKey.SeeEnemy).GetBool())
		{
			base.Owner.BlackBoard.VisibleTarget = MyEnemy;
			if (Vector3.Angle(vector3, vector2) < 10f)
			{
				base.Owner.WorldState.SetWSProperty(E_PropKey.LookingAtTarget, true);
			}
			if (Vector3.Angle(MyEnemy.Forward, -vector2) < 10f)
			{
				base.Owner.WorldState.SetWSProperty(E_PropKey.EnemyLookingAtMe, true);
			}
			float num = Vector3.Angle(vector2, MyEnemy.Forward);
			if (num > 135f && num < 225f)
			{
				base.Owner.WorldState.SetWSProperty(E_PropKey.AheadOfEnemy, true);
			}
		}
	}

	public override void Reset()
	{
		base.Owner.WorldState.SetWSProperty(E_PropKey.SeeEnemy, false);
		base.Owner.WorldState.SetWSProperty(E_PropKey.LookingAtTarget, false);
		base.Owner.WorldState.SetWSProperty(E_PropKey.AheadOfEnemy, false);
		base.Owner.WorldState.SetWSProperty(E_PropKey.EnemyLookingAtMe, false);
		base.Owner.BlackBoard.VisibleTarget = null;
	}

	private void SendLostEvent(AgentHuman target)
	{
		Fact fact = base.Owner.Memory.GetFact(E_EventTypes.EnemySee);
		if (fact == null)
		{
			return;
		}
		base.Owner.Memory.RemoveFact(E_EventTypes.EnemySee);
		if (target.IsInCover || target.IsEnteringToCover)
		{
			base.Owner.Memory.RemoveFact(E_EventTypes.EnemyLost);
			fact = FactsFactory.Create(E_EventTypes.EnemyHideInCover);
			fact.CoverDirection = target.BlackBoard.CoverPosition;
			fact.Cover = target.BlackBoard.Cover;
			if (fact.Cover == null)
			{
				fact.Cover = target.BlackBoard.Desires.CoverSelected;
			}
		}
		else
		{
			base.Owner.Memory.RemoveFact(E_EventTypes.EnemyHideInCover);
			fact = FactsFactory.Create(E_EventTypes.EnemyLost);
		}
		fact.Agent = target;
		fact.Position = target.Position;
		fact.LiveTime = 180f;
		fact.Delay = Random.Range(0.2f, 0.6f);
		base.Owner.AddFactToMemory(fact);
	}

	private void SendSeeEvent(AgentHuman target)
	{
		base.Owner.Memory.RemoveFact(E_EventTypes.EnemyLost);
		base.Owner.Memory.RemoveFact(E_EventTypes.EnemyHideInCover);
		Fact fact = FactsFactory.Create(E_EventTypes.EnemySee);
		fact.Type = E_EventTypes.EnemySee;
		fact.Position = target.Position;
		fact.Delay = Random.Range(0.3f, 0.7f);
		fact.LiveTime = 100f;
		fact.Agent = target;
		base.Owner.AddFactToMemory(fact);
		if (base.Owner.Memory.GetValidFact(E_EventTypes.EnemySee) != null)
		{
			base.Owner.WorldState.SetWSProperty(E_PropKey.SeeEnemy, true);
		}
	}
}
