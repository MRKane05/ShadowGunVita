using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Canvas_GameExtras : MonoBehaviour {
	public static Canvas_GameExtras Instance;

	public CanvasGroup Group_LoadingScreen;
	public Image LoadingScreenBar;

	// Use this for initialization
	void Awake () {
		DontDestroyOnLoad(gameObject);
		if (Instance != null)
        {
			Debug.Log("Instance of canvas class already exists");
			DestroyImmediate(gameObject);
        } else
        {
			Instance = this;
        }

	}
	
	public void UpdateLoadingBar(float amount, bool bState)
    {
		//Debug.Log(amount);
		//PROBLEM: This loading bar might need tidied - Kano!
		Group_LoadingScreen.alpha = bState? 1.0f : 0.0f;
		Group_LoadingScreen.gameObject.SetActive(bState);
		LoadingScreenBar.fillAmount = amount;
    }
}
