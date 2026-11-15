using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Game/Spawn Enemy")]
public class SpawnPointEnemy : SpawnPoint
{
	public enum E_InitState
	{
		None = 0,
		Patrol = 1,
		Goto = 2,
		Cover = 3,
		PlayAnim = 4,
		Ambush = 5
	}

	public E_EnemyType EnemyType = E_EnemyType.None;

	public E_GameDifficulty Difficulty;

	public E_InitState InitState;

	public GameObject InitPosition;

	public Cover InitCover;

	public AnimationClip SpawnAnimation;

	public Vector3 GotoPosition;

	public GameObject[] PatrolPoints;

	public List<WayPoint> Waypoints;

	public float InitSightRange = 12f;

	public float InitSightFov = 45f;

	private void Start()
	{
		base.enabled = false;
		if (InitPosition != null)
		{
			GotoPosition = InitPosition.transform.position;
		}
	}

	private void OnDestroy()
	{
		InitPosition = null;
		InitCover = null;
		SpawnAnimation = null;
		PatrolPoints = null;
		Waypoints.Clear();
	}

	private void OnDrawGizmos()
	{
		switch (EnemyType)
		{
		case E_EnemyType.Mutant:
			Gizmos.DrawIcon(base.transform.position, "SpawnPointMutant.tif");
			break;
		case E_EnemyType.Flyer:
			Gizmos.DrawIcon(base.transform.position, "SpawnPointFlyer.tif");
			break;
		case E_EnemyType.Spider:
			Gizmos.DrawIcon(base.transform.position, "SpawnPointSpider.tif");
			break;
		case E_EnemyType.Heavy:
			Gizmos.DrawIcon(base.transform.position, "SpawnPointHeavy.tif");
			break;
		case E_EnemyType.MutantShield:
			Gizmos.DrawIcon(base.transform.position, "SpawnPointShielder.tif");
			break;
		case E_EnemyType.Elite:
			Gizmos.DrawIcon(base.transform.position, "SpawnPointElite.tif");
			break;
		case E_EnemyType.BigSpider:
			Gizmos.DrawIcon(base.transform.position, "SpawnPointBigSpider.tif");
			break;
		case E_EnemyType.FinalBoss:
			Gizmos.DrawIcon(base.transform.position, "SpawnPointFinalBoss.tif");
			break;
		case E_EnemyType.DropShip:
			Gizmos.DrawIcon(base.transform.position, "SpawnPointDropship.tif");
			break;
		case E_EnemyType.Turret:
			Gizmos.DrawIcon(base.transform.position, "SpawnPointTurret.tif");
			break;
		default:
			Gizmos.DrawIcon(base.transform.position, "SpawnPoint.tif");
			break;
		}
	}
}
