using System.Collections.Generic;
using UnityEngine;

public class MissionBlackBoard : MonoBehaviour
{
	public List<Transform> EnemiesTransform;

	public float LastAttackTime;

	public static MissionBlackBoard Instance;

	private void Awake()
	{
		Instance = this;
	}

	private void Reset()
	{
		LastAttackTime = 0f;
	}
}
