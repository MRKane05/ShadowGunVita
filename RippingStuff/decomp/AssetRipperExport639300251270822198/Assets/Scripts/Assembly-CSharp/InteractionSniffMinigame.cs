using UnityEngine;

[AddComponentMenu("Interaction/Sniff Minigame")]
public class InteractionSniffMinigame : InteractionObject
{
	public AnimationClip AnimationClip;

	public AudioSource Audio;

	public GameObject Visual;

	public bool DisableOnSuccess = true;

	public bool DisableOnFail;

	public GuiSniffMinigame.Config config = new GuiSniffMinigame.Config();

	public EventSender sendOnSuccess = new EventSender();

	public EventSender sendOnFailure = new EventSender();

	public override bool IsInteractionFinished { get; protected set; }

	private void Start()
	{
		Initialize();
	}

	public override void Enable()
	{
		base.Enable();
	}

	public override void Disable()
	{
		base.Disable();
	}

	public override void Reset()
	{
		base.Reset();
	}

	public override void DoInteraction()
	{
		GuiSniffMinigame.Instance.InitGui();
		GuiSniffMinigame.Instance.Show(this);
		IsInteractionFinished = false;
	}

	public void OnSuccess()
	{
		if (DisableOnSuccess)
		{
			base.InteractionObjectUsable = false;
		}
		sendOnSuccess.Send();
		IsInteractionFinished = true;
	}

	public void OnExit()
	{
		IsInteractionFinished = true;
	}

	public void OnFailure()
	{
		if (DisableOnFail)
		{
			base.InteractionObjectUsable = false;
		}
		sendOnFailure.Send();
	}
}
