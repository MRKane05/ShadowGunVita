using UnityEngine;
using UnityEngine.AI;

public static class NavMeshUtils
{
	public static bool CheckIfPathExist(Vector3 inFromPos, Vector3 inTargetPos, int inPassableMask, float inMinEndDistance = 0.5f)
	{
		NavMeshPath navMeshPath = new NavMeshPath();
		if (!NavMesh.CalculatePath(inFromPos, inTargetPos, inPassableMask, navMeshPath))
		{
			return false;
		}
		if (!IsPathValid(navMeshPath, inTargetPos, inMinEndDistance))
		{
			return false;
		}
		return true;
	}

	public static bool IsPathValid(NavMeshPath inPath, Vector3 inTargetPos, float inMinEndDistance = 0.5f)
	{
		if (inPath.status == NavMeshPathStatus.PathInvalid)
		{
			return false;
		}
		if (inPath.corners == null || inPath.corners.Length == 0)
		{
			return false;
		}
		if (inPath.status == NavMeshPathStatus.PathPartial)
		{
			Vector3 vector = inPath.corners[inPath.corners.Length - 1];
			if ((vector - inTargetPos).sqrMagnitude > inMinEndDistance * inMinEndDistance)
			{
				return false;
			}
		}
		return true;
	}
}
