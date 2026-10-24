using UnityEngine;

[AddComponentMenu("Interaction/Collectible")]
public class InteractionObjectCollectible : InteractionObject
{
	private const int maxId = 99;

	public int colectibleID;

	public float textTimeout = 5f;

	public GameObject Visual;

	public AnimationClip AnimationClip;

	public AudioSource Audio;

	public GameObject GameObject { get; private set; }

	private static string GetCollectibleName(int id)
	{
		return "Collectible" + id;
	}

	public static bool CollectibleFound(int id)
	{
		return PlayerPrefs.GetInt(GetCollectibleName(id), 0) != 0;
	}

	public static void ResetCollectiblesSave()
	{
		for (int i = 0; i < 99; i++)
		{
			string collectibleName = GetCollectibleName(i);
			if (PlayerPrefs.HasKey(collectibleName))
			{
				PlayerPrefs.DeleteKey(collectibleName);
			}
		}
	}

	private void Awake()
	{
		Transform = base.transform;
		GameObject = base.gameObject;
		if (Visual != null)
		{
			Animation = Visual.GetComponent<Animation>();
			Animation.wrapMode = WrapMode.Once;
		}
	}

	private void Start()
	{
		GameZone firstComponentUpward = GameObject.GetFirstComponentUpward<GameZone>();
		if (firstComponentUpward == null)
		{
			Debug.LogError("This object must be a child of GameZone: '" + base.name + "'");
		}
		else if (colectibleID > 99)
		{
			Debug.LogError("Collectible '" + base.name + "' has id > " + 99 + ". (" + colectibleID + ")");
		}
		else if (AlreadyFound())
		{
			Hide();
		}
		else
		{
			Initialize();
			base.DisableDuringFight = false;
		}
	}

	public override void Enable()
	{
		if (AlreadyFound())
		{
			Hide();
		}
		else
		{
			base.Enable();
		}
	}

	public override void Reset()
	{
		if (AlreadyFound())
		{
			Hide();
		}
		else
		{
			base.Reset();
		}
	}

	public override void DoInteraction()
	{
		float num = 0f;
		base.InteractionObjectUsable = false;
		if ((bool)Audio)
		{
			Audio.Play();
		}
		if ((bool)AnimationClip && (bool)Animation)
		{
			num = AnimationClip.length;
			Animation.Play(AnimationClip.name);
		}
		if (num < UseTime)
		{
			num = UseTime + 0.1f;
		}
		ShowMessage();
		SaveCollectible();
		ReportCollectiblesProgress();
		Invoke("HideAfterInteraction", num);
	}

	private void ShowMessage()
	{
		GuiHUD.Instance.ShowMessageTimed(GuiHUD.E_HudMessageType.E_011_Collectible, textTimeout);
	}

	private void HideAfterInteraction()
	{
		Hide();
	}

	private void Hide()
	{
		Disable();
		GameObject.SetActiveRecursively(false);
	}

	private bool AlreadyFound()
	{
		return CollectibleFound(colectibleID);
	}

	private void SaveCollectible()
	{
		PlayerPrefs.SetInt(GetCollectibleName(colectibleID), 1);
	}

	private void ReportCollectiblesProgress()
	{
		int num = 0;
		for (int i = 1; i <= 16; i++)
		{
			if (CollectibleFound(i))
			{
				num++;
			}
		}
	}
}
