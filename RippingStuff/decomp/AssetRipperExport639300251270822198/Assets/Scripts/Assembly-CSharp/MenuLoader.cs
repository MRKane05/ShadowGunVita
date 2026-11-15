using UnityEngine;

public class MenuLoader : MonoBehaviour
{
	private void Start()
	{
		Application.LoadLevel(Game.MainMenuLevelName);
	}
}
