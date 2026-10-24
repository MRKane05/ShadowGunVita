using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameZoneBaze : MonoBehaviour
{
	public SpawnPointPlayer PlayerSpawn;

	protected List<Agent> _Enemies = new List<Agent>();

	protected GameObject GameObject;

	private Cover[] Covers;

	private List<AmmoBox> AmmoClips = new List<AmmoBox>();

	private AmmoBox[] AmmoBoxes = new AmmoBox[0];

	public List<Agent> Enemies
	{
		get
		{
			return _Enemies;
		}
	}

	public virtual void Initialize()
	{
		Covers = base.gameObject.GetComponentsInChildren<Cover>();
		AmmoBoxes = GameObject.GetComponentsInChildren<AmmoBox>();
	}

	public void AddEnemy(Agent enemy)
	{
		if (!_Enemies.Contains(enemy))
		{
			_Enemies.Add(enemy);
		}
	}

	public virtual void Enable()
	{
		AmmoBox[] ammoBoxes = AmmoBoxes;
		foreach (AmmoBox ammoBox in ammoBoxes)
		{
			ammoBox.Enable();
		}
	}

	public virtual void Disable()
	{
		AmmoBox[] ammoBoxes = AmmoBoxes;
		foreach (AmmoBox ammoBox in ammoBoxes)
		{
			ammoBox.Disable();
		}
		foreach (AmmoBox ammoClip in AmmoClips)
		{
			ammoClip.Reset();
			WeaponManager.Instance.Return(ammoClip);
		}
		AmmoClips.Clear();
	}

	public virtual void CleanAllEnamies()
	{
		for (int num = _Enemies.Count - 1; num >= 0; num--)
		{
			Mission.Instance.ReturnAgent(_Enemies[num].GameObject);
		}
		_Enemies.Clear();
	}

	public virtual void Reset()
	{
		StopAllCoroutines();
		for (int num = _Enemies.Count - 1; num >= 0; num--)
		{
			Mission.Instance.ReturnAgent(_Enemies[num].GameObject);
		}
		_Enemies.Clear();
		foreach (AmmoBox ammoClip in AmmoClips)
		{
			ammoClip.Reset();
			WeaponManager.Instance.Return(ammoClip);
		}
		AmmoClips.Clear();
		AmmoBox[] ammoBoxes = AmmoBoxes;
		foreach (AmmoBox ammoBox in ammoBoxes)
		{
			ammoBox.Reset();
		}
	}

	public virtual InteractionObject GetNearestInteractionObject(Vector3 center, float maxLen)
	{
		return null;
	}

	public virtual bool IsInteractionObjectInRange(Vector3 center, float maxLen)
	{
		return false;
	}

	public virtual InteractionObject InteractionObjectWithIcon(GameObject Icon)
	{
		return null;
	}

	public bool IsEnemyInRange(Vector3 center, float maxLen)
	{
		float num = maxLen * maxLen;
		for (int num2 = _Enemies.Count - 1; num2 >= 0; num2--)
		{
			Agent agent = _Enemies[num2];
			if (agent.IsAlive && (agent.Position - center).sqrMagnitude < num)
			{
				return true;
			}
		}
		return false;
	}

	public float GetDistanceToNearestEnemy(Vector3 center, Agent ignoreAgent)
	{
		float num = float.MaxValue;
		for (int i = 0; i < _Enemies.Count; i++)
		{
			Agent agent = _Enemies[i];
			if (!(agent == ignoreAgent) && agent.IsAlive)
			{
				float num2 = Vector3.Distance(center, agent.Position);
				if (num2 < num)
				{
					num = num2;
				}
			}
		}
		return num;
	}

	public Agent GetNearestAliveEnemy(Vector3 start, Vector3 end, float radius, AgentHuman ignoreAgent)
	{
		float num = radius;
		Agent result = null;
		for (int i = 0; i < _Enemies.Count; i++)
		{
			Agent agent = _Enemies[i];
			if (!(agent == ignoreAgent) && agent.IsAlive)
			{
				float num2 = Mathfx.DistanceFromPointToVector(start, end, agent.Position);
				if (num2 < num)
				{
					num = num2;
					result = agent;
				}
			}
		}
		return result;
	}

	public Agent GetNearestAliveEnemy(AgentHuman agent, E_Direction direction, float maxRadius)
	{
		Vector3 vector;
		switch (direction)
		{
		case E_Direction.Forward:
			vector = agent.Forward;
			break;
		case E_Direction.Backward:
			vector = -agent.Forward;
			break;
		case E_Direction.Right:
			vector = agent.Right;
			break;
		case E_Direction.Left:
			vector = -agent.Right;
			break;
		default:
			return null;
		}
		return GetNearestAliveEnemy(agent.Position + vector, agent.Position + vector * 3f, maxRadius, agent);
	}

	public Cover GetCoverForPlayer(Vector3 pos, Vector3 coverdir, float distance)
	{
		for (int i = 0; i < Covers.Length; i++)
		{
			Cover cover = Covers[i];
			if (cover.IsLockedForPlayer() || cover.GetCoverDirAngle(coverdir) >= 60f)
			{
				continue;
			}
			Vector3 normalized = (pos - cover.Position).normalized;
			if (!(Vector3.Dot(cover.Forward, normalized) > 0f))
			{
				Vector3 lhs = cover.RightEdge - cover.LeftEdge;
				float magnitude = lhs.magnitude;
				lhs.Normalize();
				float num = Vector3.Dot(lhs, pos - cover.LeftEdge);
				if (!(num < -0.3f) && !(num > magnitude + 0.3f) && !(cover.GetDistanceTo(pos) > distance))
				{
					return cover;
				}
			}
		}
		return null;
	}

	public Cover GetCoverForEnemy(Vector3 pos, Vector3 coverdir, float distance, Cover ignore)
	{
		Cover result = null;
		for (int i = 0; i < Covers.Length; i++)
		{
			Cover cover = Covers[i];
			if (!(cover == ignore) && !cover.IsLockedForEnemy() && !(cover.GetCoverDirAngle(coverdir) >= 75f))
			{
				float distanceTo = cover.GetDistanceTo(pos);
				if (!(distanceTo > distance))
				{
					result = cover;
					distance = distanceTo;
				}
			}
		}
		return result;
	}

	public Cover GetOppositeCover(Cover cover)
	{
		if (!cover.CoverFlags.Get(Cover.E_CoverFlags.UpCrouch))
		{
			return null;
		}
		Transform parent = cover.Transform.parent;
		if (parent == null)
		{
			return null;
		}
		Cover[] componentsInChildren = parent.GetComponentsInChildren<Cover>();
		Cover[] array = componentsInChildren;
		foreach (Cover cover2 in array)
		{
			if (!(cover2 == cover) && !cover2.IsLockedPosition(E_CoverDirection.Middle) && !(Vector3.SqrMagnitude(cover2.Position - cover.Position) > 4.84f) && !(Vector3.Dot(cover2.Forward, cover.Forward) > -0.9f))
			{
				return cover2;
			}
		}
		return null;
	}

	public Cover GetCoverAgainst(AgentHuman agent, AgentHuman enemy, Vector3 coverdir, float distance)
	{
		Vector3 vector = ((!agent.IsInCover) ? agent.Position : agent.BlackBoard.Cover.MiddleEntryPos);
		Cover result = null;
		float num = distance;
		Fact validFact = agent.Memory.GetValidFact(E_EventTypes.CoverCompromised);
		Cover cover = ((validFact == null) ? null : validFact.Cover);
		for (int i = 0; i < Covers.Length; i++)
		{
			Cover cover2 = Covers[i];
			if (cover2 == cover || agent.BlackBoard.Cover == cover2 || !cover2.IsAvailableForEnemy() || cover2.GetCoverDirAngle(coverdir) >= 75f)
			{
				continue;
			}
			float distanceTo = cover2.GetDistanceTo(vector);
			if (distanceTo > num)
			{
				continue;
			}
			Vector3 rhs = enemy.Position - cover2.Position;
			if (!(rhs.magnitude < 5f))
			{
				rhs.Normalize();
				if (!(Vector3.Dot(cover2.Forward, rhs) < 0.2f) && NavMeshUtils.CheckIfPathExist(vector, cover2.MiddleEntryPos, agent.NavMeshAgent.walkableMask))
				{
					result = cover2;
					num = distanceTo;
				}
			}
		}
		return result;
	}

	public Cover GetCoverForAdvance(AgentHuman agent, AgentHuman enemy, float minDistanceToMove, float minDistanceToEnemy)
	{
		Vector3 vector = ((!agent.IsInCover) ? agent.Position : agent.BlackBoard.Cover.MiddleEntryPos);
		Vector3 lhs = enemy.Position - vector;
		lhs.Normalize();
		Fact validFact = agent.Memory.GetValidFact(E_EventTypes.CoverCompromised);
		Cover cover = ((validFact == null) ? null : validFact.Cover);
		Cover result = null;
		for (int i = 0; i < Covers.Length; i++)
		{
			Cover cover2 = Covers[i];
			if (cover2 == cover || cover2 == agent.BlackBoard.Cover || !cover2.IsAvailableForEnemy())
			{
				continue;
			}
			Vector3 rhs = cover2.Position - agent.Position;
			rhs.Normalize();
			if (Vector3.Dot(lhs, rhs) < 0.8f)
			{
				continue;
			}
			rhs = cover2.Position - enemy.Position;
			rhs.Normalize();
			if (!(Vector3.Dot(lhs, rhs) > -0.75f) && !(cover2.GetDistanceTo(agent.Position) < minDistanceToMove) && !(cover2.GetDistanceTo(enemy.Position) < minDistanceToEnemy) && !(cover2.GetCoverDirAngleToPos(enemy.Position) > 60f) && NavMeshUtils.CheckIfPathExist(vector, cover2.MiddleEntryPos, agent.NavMeshAgent.walkableMask))
			{
				if (!cover2.IsLockedForPlayer())
				{
					return cover2;
				}
				result = cover2;
			}
		}
		return result;
	}

	public Cover GetCoverForFallback(AgentHuman agent, AgentHuman enemy, float minDistance, float maxDistance)
	{
		Vector3 vector = ((!agent.IsInCover) ? agent.Position : agent.BlackBoard.Cover.MiddleEntryPos);
		Vector3 lhs = agent.Position - enemy.Position;
		lhs.Normalize();
		for (int i = 0; i < Covers.Length; i++)
		{
			Cover cover = Covers[i];
			if (cover.IsAvailableForEnemy())
			{
				Vector3 rhs = cover.Position - vector;
				rhs.Normalize();
				if (!(Vector3.Dot(lhs, rhs) < 0.5f) && !(cover.GetDistanceTo(agent.Position) < minDistance) && !(cover.GetDistanceTo(enemy.Position) > maxDistance) && !(cover.GetCoverDirAngleToPos(enemy.Position) > 60f) && NavMeshUtils.CheckIfPathExist(vector, cover.MiddleEntryPos, agent.NavMeshAgent.walkableMask))
				{
					return cover;
				}
			}
		}
		return null;
	}

	public Cover GetCoverForFlank(AgentHuman agent, bool left, AgentHuman enemy, float minDistance, float maxDistance)
	{
		Vector3 inFromPos = ((!agent.IsInCover) ? agent.Position : agent.BlackBoard.Cover.MiddleEntryPos);
		Vector3 lhs = enemy.Position - agent.Position;
		Vector3 lhs2 = Vector3.Cross(lhs, Vector3.up) * ((!left) ? 1 : (-1));
		for (int i = 0; i < Covers.Length; i++)
		{
			Cover cover = Covers[i];
			if (!cover.IsAvailableForEnemy())
			{
				continue;
			}
			Vector3 rhs = enemy.Position - cover.Position;
			rhs.Normalize();
			if (!(Vector3.Dot(lhs2, rhs) < 0.3f) && !(cover.GetCoverDirAngleToPos(enemy.Position) >= 30f))
			{
				float distanceTo = cover.GetDistanceTo(enemy.Position);
				if (!(distanceTo < minDistance) && !(distanceTo > maxDistance) && NavMeshUtils.CheckIfPathExist(inFromPos, cover.MiddleEntryPos, agent.NavMeshAgent.walkableMask))
				{
					return cover;
				}
			}
		}
		return null;
	}

	public void SendFactToEnemies(AgentHuman owner, AgentHuman attacker, E_EventTypes eventType, float radius, float lifetime, bool toFriends)
	{
		float num = radius * radius;
		foreach (Agent enemy in Enemies)
		{
			AgentHuman agentHuman = enemy as AgentHuman;
			if ((bool)agentHuman)
			{
				if (!(owner == agentHuman) && (owner.Position - agentHuman.Position).sqrMagnitude < num)
				{
					agentHuman.AddFactToMemory(eventType, (!attacker) ? owner : attacker, lifetime, Random.Range(0.3f, 0.6f));
				}
			}
			else if (eventType == E_EventTypes.EnemyFire && owner.IsPlayer && (owner.Position - enemy.transform.position).sqrMagnitude < num)
			{
				enemy.OnShootSound(owner);
			}
		}
	}

	public void RegisterDroppedAmmoClip(E_WeaponType weapon, Vector3 pos)
	{
		AmmoBox ammo = WeaponManager.Instance.GetAmmo(weapon);
		ammo.Enable(pos);
		AmmoClips.Add(ammo);
	}

	public void UnRegisterDroppedAmmoClip(AmmoBox ammo)
	{
		ammo.Disable();
		AmmoClips.Remove(ammo);
		WeaponManager.Instance.Return(ammo);
	}

	public AmmoBox GetNearestDroppedAmmoClip(AgentHuman agent, float dist)
	{
		float num = dist * dist;
		foreach (AmmoBox ammoClip in AmmoClips)
		{
			Vector3 vector = agent.CharacterController.ClosestPointOnBounds(ammoClip.Transform.position);
			if ((ammoClip.Transform.position - vector).sqrMagnitude < num)
			{
				return ammoClip;
			}
		}
		AmmoBox[] ammoBoxes = AmmoBoxes;
		foreach (AmmoBox ammoBox in ammoBoxes)
		{
			if (ammoBox.enabled)
			{
				Vector3 vector2 = agent.CharacterController.ClosestPointOnBounds(ammoBox.Transform.position);
				if ((ammoBox.Transform.position - vector2).sqrMagnitude < num)
				{
					return ammoBox;
				}
			}
		}
		return null;
	}

	private void Debug_DrawPath(Vector3 inFrom, Vector3 inTo, NavMeshPath inPath, int inDuration)
	{
		Debug.DrawLine(inFrom, inTo, Color.white, inDuration);
		if (inPath.corners.Length > 0)
		{
			Vector3 start = inFrom;
			for (int i = 0; i < inPath.corners.Length; i++)
			{
				Vector3 vector = inPath.corners[i];
				Debug.DrawRay(vector, Vector3.up, Color.yellow, inDuration);
				Debug.DrawLine(start, vector, Color.blue, inDuration);
				start = vector;
			}
		}
	}
}
