using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteManager))]
public class SpriteEffectsManager : MonoBehaviour
{
	public int MaxSprites = 30;

	private ArrayList Sprites = new ArrayList();

	private SpriteManager SpriteManager;

	public static SpriteEffectsManager Instance;

	private void Awake()
	{
		Instance = this;
		SpriteManager = GetComponent<SpriteManager>();
	}

	private void Start()
	{
	}

	private void FixedUpdate()
	{
		if (Sprites.Count > MaxSprites)
		{
			SpriteManager.RemoveSprite(Sprites[0] as Sprite);
			Sprites.RemoveAt(0);
		}
	}

	public void CreateBloodSlatter(Transform t, float minLen, float maxLen)
	{
		float maxSize = Random.Range(minLen, maxLen);
		GameObject gameObject = new GameObject("splatter");
		gameObject.transform.position = new Vector3(t.position.x, t.position.y, t.position.z);
		gameObject.transform.eulerAngles = new Vector3(0f, Random.Range(0, 360), 0f);
		Sprite s = SpriteManager.AddSprite(gameObject, 0.2f, 0.2f, 0, 128, 128, 128, false);
		StartCoroutine(UpdateSplatter(s, 0.5f, maxSize, 0.8f));
	}

	public void CreateBlood(Vector3 pos)
	{
		float num = Random.Range(0.5f, 0.8f);
		GameObject gameObject = new GameObject("splash");
		pos.y += 0.01f;
		gameObject.transform.position = pos;
		gameObject.transform.eulerAngles = new Vector3(0f, Random.Range(0, 360), 0f);
		if (Random.Range(0, 10) % 2 == 0)
		{
			Sprites.Add(SpriteManager.AddSprite(gameObject, num, num, 128, 128, 128, 128, false));
		}
		else
		{
			Sprites.Add(SpriteManager.AddSprite(gameObject, num, num, 0, 256, 128, 128, false));
		}
	}

	public void ReleaseBloodSprites()
	{
		while (Sprites.Count > 0)
		{
			SpriteManager.RemoveSprite(Sprites[0] as Sprite);
			Sprites.RemoveAt(0);
		}
	}

	private IEnumerator UpdateSplatter(Sprite s, float size, float maxSize, float speed)
	{
		float f = size;
		while (f < maxSize)
		{
			f += speed * Time.deltaTime;
			if (f > maxSize)
			{
				f = maxSize;
			}
			s.SetSizeXZ(f, f);
			yield return new WaitForEndOfFrame();
		}
		Sprites.Add(s);
	}
}
