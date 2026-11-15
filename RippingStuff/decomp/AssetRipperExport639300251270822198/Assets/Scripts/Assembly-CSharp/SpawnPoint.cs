using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
	public Transform Transform { get; private set; }

	private void Awake()
	{
		Transform = base.transform;
	}

	private void Start()
	{
		base.enabled = false;
	}
}
