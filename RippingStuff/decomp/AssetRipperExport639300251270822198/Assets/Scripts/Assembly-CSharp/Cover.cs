using System;
using UnityEngine;

[Serializable]
[AddComponentMenu("Game/Cover")]
public class Cover : MonoBehaviour
{
	[Serializable]
	public enum E_CoverFlags
	{
		LeftCrouch = 0,
		RightCrouch = 1,
		LeftStand = 2,
		RightStand = 3,
		UpCrouch = 4,
		Max = 5
	}

	[Serializable]
	public class CoverFlagsBitArray
	{
		[SerializeField]
		private int Flags;

		public void Set(E_CoverFlags index, bool on)
		{
			if (on)
			{
				Flags |= 1 << (int)index;
			}
			else
			{
				Flags &= ~(1 << (int)index);
			}
		}

		public bool Get(E_CoverFlags index)
		{
			return (Flags & (1 << (int)index)) != 0;
		}
	}

	public CoverFlagsBitArray CoverFlags = new CoverFlagsBitArray();

	private Vector3 _LeftEdge;

	private Vector3 _RightEdge;

	public AgentHuman LeftAgent { get; private set; }

	public AgentHuman RightAgent { get; private set; }

	public AgentHuman MiddleAgent { get; private set; }

	public Vector3 Position
	{
		get
		{
			return Transform.position;
		}
	}

	public Transform Transform { get; private set; }

	public Vector3 LeftEdge
	{
		get
		{
			return _LeftEdge;
		}
	}

	public Vector3 RightEdge
	{
		get
		{
			return _RightEdge;
		}
	}

	public Vector3 EyeLeftEdge
	{
		get
		{
			return _LeftEdge - Transform.right * 0.5f + Vector3.up * 1.6f;
		}
	}

	public Vector3 EyeRightEdge
	{
		get
		{
			return _RightEdge + Transform.right * 0.5f + Vector3.up * 1.6f;
		}
	}

	public Vector3 EyeMiddle
	{
		get
		{
			return Position + Vector3.up * 1.6f;
		}
	}

	public Vector3 Forward
	{
		get
		{
			return Transform.forward;
		}
	}

	public Vector3 Right
	{
		get
		{
			return Transform.right;
		}
	}

	public Vector3 Left
	{
		get
		{
			return -Transform.right;
		}
	}

	public bool IsStandAllowed
	{
		get
		{
			return CoverFlags.Get(E_CoverFlags.LeftStand) || CoverFlags.Get(E_CoverFlags.RightStand);
		}
	}

	public bool IsMiddleAllowed
	{
		get
		{
			return CoverFlags.Get(E_CoverFlags.UpCrouch);
		}
	}

	public bool IsLeftAllowed
	{
		get
		{
			return CoverFlags.Get(E_CoverFlags.LeftStand) || CoverFlags.Get(E_CoverFlags.LeftCrouch);
		}
	}

	public bool IsRightAllowed
	{
		get
		{
			return CoverFlags.Get(E_CoverFlags.RightCrouch) || CoverFlags.Get(E_CoverFlags.RightStand);
		}
	}

	public Vector3 MiddleEntryPos
	{
		get
		{
			return Position - Forward * 1.5f;
		}
	}

	public Vector3 GetEyePos(E_CoverDirection pos)
	{
		switch (pos)
		{
		case E_CoverDirection.Middle:
			return EyeMiddle;
		case E_CoverDirection.Left:
			return EyeLeftEdge;
		default:
			return EyeRightEdge;
		}
	}

	private void Awake()
	{
		Transform = base.transform;
		RaycastHit hitInfo;
		if (Physics.Raycast(Transform.position + Vector3.up * 0.2f, Vector3.down, out hitInfo))
		{
			Transform.position = hitInfo.point;
		}
		if (Physics.Raycast(Transform.position + Vector3.up * 0.45f, Forward, out hitInfo))
		{
			Transform.position += Forward * (hitInfo.distance - 0.4f);
		}
		_LeftEdge = Transform.position - Transform.right * Transform.lossyScale.x * 0.5f;
		_RightEdge = Transform.position + Transform.right * Transform.lossyScale.x * 0.5f;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public float GetCoverDirAngle(Vector3 dir)
	{
		return Vector3.Angle(Forward, dir);
	}

	public float GetCoverDirAngleToPos(Vector3 pos)
	{
		Vector3 to = pos - Transform.position;
		to.Normalize();
		return Vector3.Angle(Forward, to);
	}

	public float GetDistanceTo(Vector3 pos)
	{
		Vector3 vector = Mathfx.NearestPointStrict(_LeftEdge, _RightEdge, pos);
		return (pos - vector).magnitude;
	}

	public Vector3 GetNearestPointOnCover(Vector3 pos)
	{
		return Mathfx.NearestPointStrict(_LeftEdge, _RightEdge, pos);
	}

	public bool CanFire(AgentHuman agent)
	{
		if (agent.BlackBoard.CoverPose == E_CoverPose.Stand)
		{
			if (agent.BlackBoard.CoverPosition == E_CoverDirection.Middle)
			{
				return false;
			}
			if (agent.BlackBoard.CoverPosition == E_CoverDirection.Left && !CoverFlags.Get(E_CoverFlags.LeftStand))
			{
				return false;
			}
			if (agent.BlackBoard.CoverPosition == E_CoverDirection.Right && !CoverFlags.Get(E_CoverFlags.RightStand))
			{
				return false;
			}
		}
		else
		{
			if (agent.BlackBoard.CoverPosition == E_CoverDirection.Middle && !CoverFlags.Get(E_CoverFlags.UpCrouch))
			{
				return false;
			}
			if (agent.BlackBoard.CoverPosition == E_CoverDirection.Left && !CoverFlags.Get(E_CoverFlags.LeftCrouch))
			{
				return false;
			}
			if (agent.BlackBoard.CoverPosition == E_CoverDirection.Right && !CoverFlags.Get(E_CoverFlags.RightCrouch))
			{
				return false;
			}
		}
		return true;
	}

	public void LockPosition(E_CoverDirection position, AgentHuman agent)
	{
		switch (position)
		{
		case E_CoverDirection.Left:
			LeftAgent = agent;
			break;
		case E_CoverDirection.Right:
			RightAgent = agent;
			break;
		case E_CoverDirection.Middle:
			MiddleAgent = agent;
			break;
		case E_CoverDirection.Unknown:
			if (IsLeftAllowed)
			{
				LeftAgent = agent;
			}
			if (IsRightAllowed)
			{
				RightAgent = agent;
			}
			if (IsMiddleAllowed)
			{
				MiddleAgent = agent;
			}
			break;
		}
	}

	public bool IsLockedForEnemy(AgentHuman enemy, E_CoverDirection position)
	{
		if (IsLockedForEnemy())
		{
			return true;
		}
		switch (position)
		{
		case E_CoverDirection.Left:
			return !IsLeftAllowed || (LeftAgent != null && LeftAgent != enemy);
		case E_CoverDirection.Right:
			return !IsRightAllowed || (RightAgent != null && RightAgent != enemy);
		case E_CoverDirection.Middle:
			return !IsMiddleAllowed || (MiddleAgent != null && MiddleAgent != enemy);
		default:
			return false;
		}
	}

	public bool IsLockedPosition(E_CoverDirection position)
	{
		switch (position)
		{
		case E_CoverDirection.Left:
			return !IsLeftAllowed || LeftAgent != null;
		case E_CoverDirection.Right:
			return !IsRightAllowed || RightAgent != null;
		case E_CoverDirection.Middle:
			return !IsMiddleAllowed || MiddleAgent != null;
		default:
			return true;
		}
	}

	public bool IsAvailableForEnemy()
	{
		if (IsLockedForEnemy())
		{
			return false;
		}
		if (IsLeftAllowed && LeftAgent == null)
		{
			return true;
		}
		if (IsRightAllowed && RightAgent == null)
		{
			return true;
		}
		if (IsMiddleAllowed && MiddleAgent == null)
		{
			return true;
		}
		return false;
	}

	public bool IsLockedForEnemy()
	{
		return LeftAgent == Player.Instance.Owner || RightAgent == Player.Instance.Owner || MiddleAgent == Player.Instance.Owner;
	}

	public bool IsLockedForPlayer()
	{
		return (LeftAgent != null && !LeftAgent.IsPlayer) || (RightAgent != null && !RightAgent.IsPlayer) || (MiddleAgent != null && !MiddleAgent.IsPlayer);
	}

	public bool CanUseItAgainst(AgentHuman owner, AgentHuman attacker)
	{
		if (IsLockedForEnemy())
		{
			return false;
		}
		if (attacker == null)
		{
			return true;
		}
		Vector3 normalized = (attacker.Position - Position).normalized;
		if (Vector3.Dot(Forward, normalized) < 0.3f)
		{
			return false;
		}
		return true;
	}
}
