using UnityEngine;

public class Subtitles : MonoBehaviour
{
	public Color m_TextColor;

	public Font myFont;

	public GUIStyle cStyle;

	private GUIStyle myStyle;

	private GUIContent myContent;

	private float lastChangeTime;

	private Vector2 scale = new Vector2(2f, 2f);

	private Vector2 pivotPoint;

	private void Awake()
	{
	}

	private void Start()
	{
		InvokeRepeating("RegenerateText", 2f, 2f);
		RegenerateText();
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
		if (myStyle == null)
		{
			myStyle = new GUIStyle(GUI.skin.box);
			myStyle.font = myFont;
			myStyle.normal.textColor = m_TextColor;
			myStyle.wordWrap = true;
			myStyle.alignment = TextAnchor.UpperCenter;
			RegenerateText();
		}
		Vector2 vector = new Vector2(40f, 10f) / 100f;
		Vector2 vector2 = new Vector2(50f, 80f) / 100f;
		Vector2 vector3 = new Vector2(50f, 50f) / 100f;
		float left = (float)Screen.width * (vector2.x - vector.x * vector3.x);
		float top = (float)Screen.height * (vector2.y - vector.y * vector3.y);
		float width = (float)Screen.width * vector.x;
		float height = (float)Screen.height * vector.y;
		if (Time.realtimeSinceStartup > lastChangeTime + 2f)
		{
			lastChangeTime = Time.realtimeSinceStartup;
		}
		if (GUI.Button(new Rect(Screen.width / 2 - 25, Screen.height / 2 - 25, 50f, 50f), "Big!"))
		{
			scale += new Vector2(0.5f, 0.5f);
		}
		GUIUtility.ScaleAroundPivot(pivotPoint: new Vector2((float)Screen.width * vector2.x, (float)Screen.height * vector2.y), scale: scale);
		GUI.Box(new Rect(left, top, width, height), myContent, myStyle);
	}

	private void RegenerateText()
	{
		int num = Random.Range(25, 40);
		char[] array = new char[num + 1];
		for (int i = 0; i < num; i++)
		{
			array[i] = (char)Random.Range(12353, 12446);
		}
		array[num] = '\0';
		myContent = new GUIContent(new string(array));
	}
}
