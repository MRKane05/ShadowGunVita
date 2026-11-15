using UnityEngine;

public class LogResources : MonoBehaviour
{
	private void Start()
	{
		Invoke("Log", 1f);
	}

	private void Update()
	{
	}

	private void Log()
	{
		Object[] array = Resources.FindObjectsOfTypeAll(typeof(Texture));
		Debug.Log("number of textures " + array.Length);
		for (int i = 0; i < array.Length; i++)
		{
			Debug.Log(array[i].name + " " + (array[i] as Texture).width + "x" + (array[i] as Texture).height);
		}
		Object[] array2 = Resources.FindObjectsOfTypeAll(typeof(Object));
		Debug.Log("number of objects " + array.Length);
		for (int j = 0; j < array.Length; j++)
		{
			Debug.Log(array2[j].name);
		}
	}
}
