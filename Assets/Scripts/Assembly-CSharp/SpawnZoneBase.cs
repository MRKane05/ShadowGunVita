using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class SpawnZoneBase : MonoBehaviour
{
	public enum E_ShowFirstTimeEnemy
	{
		No = 0,
		Yes = 1
	}

	[Serializable]
	public class ShowFirstEnemy
	{
		public float Delay;

		public GameObject Camera;

		public AnimationClip CameraAnim;

		public float FadeOutTime = 0.3f;

		public float FadeInTime = 0.3f;

		public Transform PlayerFinalPos;

		public GameObject Enemy;
	}

	public enum E_State
	{
		E_WAITING_FOR_START = 0,
		E_SPAWNING_ENEMIES = 1,
		E_IN_PROGRESS = 2,
		E_FINISHED = 3
	}

	public E_ShowFirstTimeEnemy ShowFirstTimeEnemy;

	public ShowFirstEnemy FirstEnemyShow;

	public List<SpawnPointEnemy> SpawnPoints;

	public int SendEventWhenNumberOfEnemiesLeft;

	public List<GameEvent> GameEventsToSend = new List<GameEvent>();

	public List<GameEvent> GameEventsToSendWhenDone = new List<GameEvent>();

	private GameObject GameObject;

	private List<Agent> EnemiesAlive = new List<Agent>();

	private bool SendEvents;

	private bool SendEventsWhenDone;

	private GameZone MyGameZone;

	public E_State State { get; private set; }

	public bool IsActive()
	{
		return EnemiesAlive.Count > 0;
	}

	public Agent GetEnemy(int index)
	{
		return EnemiesAlive[index];
	}

	public int GetEnemyCount()
	{
		return EnemiesAlive.Count;
	}

	private void Awake()
	{
		State = E_State.E_WAITING_FOR_START;
		SendEvents = true;
		SendEventsWhenDone = true;
		GameObject = base.gameObject;
		MyGameZone = GameObject.transform.parent.GetComponent<GameZone>();
	}

	public void Enable()
	{
		if (ShowFirstTimeEnemy == E_ShowFirstTimeEnemy.Yes)
		{
			if ((bool)FirstEnemyShow.Camera)
			{
				FirstEnemyShow.Camera.SetActiveRecursively(false);
			}
			if ((bool)FirstEnemyShow.Enemy)
			{
				FirstEnemyShow.Enemy.SetActiveRecursively(false);
			}
		}
		GameObject.SetActiveRecursively(true);
	}

	public void Disable()
	{
		StopAllCoroutines();
		GameObject.SetActiveRecursively(false);
	}

	private void OnDrawGizmos()
	{
		BoxCollider boxCollider = GetComponent("BoxCollider") as BoxCollider;
		if (boxCollider != null)
		{
			Gizmos.color = new Color(0f, 1f, 1f, 1f);
			Gizmos.matrix = boxCollider.transform.localToWorldMatrix;
			Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
			Gizmos.matrix = Matrix4x4.identity;
		}
		else
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawCube(base.transform.position, Vector3.one);
		}
		if (SpawnPoints == null)
		{
			return;
		}
		for (int i = 0; i < SpawnPoints.Count; i++)
		{
			if (boxCollider != null)
			{
				Gizmos.DrawLine(boxCollider.transform.position + boxCollider.center, SpawnPoints[i].transform.position);
			}
			else
			{
				Gizmos.DrawLine(base.gameObject.transform.position, SpawnPoints[i].transform.position);
			}
		}
	}

	private void FixedUpdate()
	{
		if (State != E_State.E_IN_PROGRESS)
		{
			return;
		}
		for (int num = EnemiesAlive.Count - 1; num >= 0; num--)
		{
			if (!EnemiesAlive[num].IsAlive)
			{
				EnemiesAlive.RemoveAt(num);
				if (SendEvents && SendEventWhenNumberOfEnemiesLeft == EnemiesAlive.Count)
				{
					foreach (GameEvent item in GameEventsToSend)
					{
						Mission.Instance.SendGameEvent(item.Name, item.State, item.Delay);
					}
					SendEvents = false;
				}
				if (SendEventsWhenDone && EnemiesAlive.Count == 0)
				{
					foreach (GameEvent item2 in GameEventsToSendWhenDone)
					{
						Mission.Instance.SendGameEvent(item2.Name, item2.State, item2.Delay);
					}
					SendEventsWhenDone = false;
				}
			}
		}
	}

	public void Reset()
	{
		StopAllCoroutines();
		State = E_State.E_WAITING_FOR_START;
		SendEvents = true;
		SendEventsWhenDone = true;
		EnemiesAlive.Clear();
	}

	protected void StartSpawn()
	{
		MyGameZone.EnableAllActiveInteractionInFight(false);
		if (ShowFirstTimeEnemy == E_ShowFirstTimeEnemy.Yes)
		{
			StartCoroutine(SpawnEnemiesEx());
		}
		else
		{
			StartCoroutine(SpawnEnemies());
		}
	}

	private IEnumerator SpawnEnemiesEx()
	{
		State = E_State.E_SPAWNING_ENEMIES;
		yield return new WaitForSeconds(FirstEnemyShow.Delay);
		Player.Instance.StopMove(true);
		MFGuiManager.Instance.FadeOut(FirstEnemyShow.FadeInTime * 0.5f);
		yield return new WaitForSeconds(FirstEnemyShow.FadeInTime * 0.5f);
		Camera old = Camera.main;
		GameCamera.Instance.StoreShakingState();
		old.gameObject.SetActiveRecursively(false);
		GuiHUD.Instance.Hide();
		FirstEnemyShow.Camera.SetActiveRecursively(true);
		FirstEnemyShow.Camera.GetComponent<Animation>().Play(FirstEnemyShow.CameraAnim.name);
		FirstEnemyShow.Enemy.SetActiveRecursively(true);
		MFGuiManager.Instance.FadeIn(FirstEnemyShow.FadeInTime * 0.5f);
		yield return new WaitForSeconds(FirstEnemyShow.FadeInTime * 0.5f);
		yield return new WaitForSeconds(FirstEnemyShow.Camera.GetComponent<Animation>()[FirstEnemyShow.CameraAnim.name].length);
		MFGuiManager.Instance.FadeOut(FirstEnemyShow.FadeInTime * 0.5f);
		yield return new WaitForSeconds(FirstEnemyShow.FadeOutTime * 0.5f);
		if ((bool)FirstEnemyShow.PlayerFinalPos)
		{
			Player.Instance.Owner.Teleport(FirstEnemyShow.PlayerFinalPos);
		}
		old.gameObject.SetActiveRecursively(true);
		GameCamera.Instance.ReStoreShakingState();
		FirstEnemyShow.Camera.SetActiveRecursively(false);
		FirstEnemyShow.Enemy.SetActiveRecursively(false);
		GuiHUD.Instance.Show();
		for (int i = 0; i < SpawnPoints.Count; i++)
		{
			if (SpawnPoints[i].Difficulty <= Game.Instance.GameDifficulty)
			{
				SpawnEnemy(SpawnPoints[i]);
			}
		}
		MFGuiManager.Instance.FadeIn(FirstEnemyShow.FadeOutTime * 0.5f);
		Player.Instance.StopMove(false);
		yield return new WaitForSeconds(4f);
		State = E_State.E_IN_PROGRESS;
	}

	private IEnumerator SpawnEnemies()
	{
		State = E_State.E_SPAWNING_ENEMIES;
		yield return new WaitForEndOfFrame();
		for (int i = 0; i < SpawnPoints.Count; i++)
		{
			if (SpawnPoints[i].Difficulty <= Game.Instance.GameDifficulty)
			{
				SpawnEnemy(SpawnPoints[i]);
				yield return new WaitForSeconds(0.01f);
			}
		}
		yield return new WaitForSeconds(4f);
		State = E_State.E_IN_PROGRESS;
	}

	private void SpawnEnemy(SpawnPointEnemy spawnpoint)
	{
		if (spawnpoint.EnemyType != E_EnemyType.None)
		{
			GameObject agendFromCache = Mission.Instance.GetAgendFromCache(spawnpoint.EnemyType, spawnpoint.Transform);
			if (agendFromCache == null)
			{
				Debug.LogError(string.Concat(base.name, " No enemy in cache ", spawnpoint.EnemyType, " on ", spawnpoint.name));
			}
			else
			{
				agendFromCache.SendMessage("Activate", spawnpoint);
				Agent component = agendFromCache.GetComponent<Agent>();
				MyGameZone.AddEnemy(component);
				EnemiesAlive.Add(component);
			}
		}
	}

	private SpawnPointEnemy GetAvailableSpawnPoint(SpawnPointEnemy[] spawnPoints)
	{
		Vector3 position = Player.Instance.Owner.Position;
		float num = 0f;
		int num2 = -1;
		for (int i = 0; i < spawnPoints.Length; i++)
		{
			if (!MyGameZone.IsEnemyInRange(spawnPoints[i].transform.position, 2f))
			{
				float num3 = 0f;
				float num4 = Mathf.Min(14f, (spawnPoints[i].Transform.position - position).magnitude);
				num3 = Mathfx.Hermite(0f, 7f, num4 / 7f);
				if (!(num3 <= num))
				{
					num = num3;
					num2 = i;
				}
			}
		}
		if (num2 == -1)
		{
			return spawnPoints[Random.Range(0, spawnPoints.Length)];
		}
		return spawnPoints[num2];
	}
}
