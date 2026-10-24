using UnityEngine;

// Optional scene component for anything that needs TextDatabase present in the scene.
// The database itself remains a ScriptableObject singleton.
public class TextDatabaseHolder : MonoBehaviour
{
	public TextDatabase Database
	{
		get
		{
			return TextDatabase.instance;
		}
	}
}
