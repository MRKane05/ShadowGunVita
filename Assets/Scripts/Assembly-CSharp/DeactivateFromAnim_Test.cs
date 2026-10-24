using UnityEngine;

public class DeactivateFromAnim_Test : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnBecameInvisible()
	{
		Debug.Log("OnBecameInvisible");
	}

	private void OnBecameVisible()
	{
		Debug.Log("OnBecameVisible");
	}

	public void DeactivateFromAnim()
	{
		Debug.Log("-- DeactivateFromAnim");
		base.gameObject.SetActiveRecursively(false);
	}
}
