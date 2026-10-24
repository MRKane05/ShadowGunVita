using System.Collections;
using UnityEngine;

[AddComponentMenu("Interaction/Weapon Box")]
public class InteractionWeaponBox : InteractionObject
{
	public E_WeaponType Weapon;

	public GameObject WeaponVisual;

	public AudioClip SoundTake;

	public GameObject CutsceneCamera;

	public AnimationClip CameraAnim;

	public float FadeOutTime = 0.3f;

	public float FadeInTime = 0.3f;

	public AnimationClip AnimOpen;

	public AnimationClip AnimClose;

	public AudioClip SoundOpen;

	public AudioClip SoundClose;

	public ParticleSystem ParticleEffect;

	public float ParticleDelay;

	private AudioSource AudioSource;

	public override bool IsInteractionFinished { get; protected set; }

	private void Start()
	{
		Initialize();
		CutsceneCamera.SetActiveRecursively(false);
		Animation = base.GetComponent<Animation>();
		AudioSource = base.GetComponent<AudioSource>();
	}

	private void OnDestroy()
	{
		WeaponVisual = null;
		SoundTake = null;
		CutsceneCamera = null;
		CameraAnim = null;
		AnimOpen = null;
		AnimClose = null;
		SoundOpen = null;
		SoundClose = null;
		ParticleEffect = null;
		AudioSource = null;
	}

	public override void DoInteraction()
	{
		base.InteractionObjectUsable = false;
		Disable();
		if ((bool)SoundTake)
		{
			Player.Instance.Owner.SoundPlay(SoundTake);
		}
		StartCoroutine(PlayCutscene());
	}

	private IEnumerator PlayCutscene()
	{
		IsInteractionFinished = false;
		Invoke("PlayParticle", ParticleDelay);
		if ((bool)AnimOpen)
		{
			Animation.Play(AnimOpen.name);
		}
		if ((bool)SoundOpen)
		{
			AudioSource.PlayOneShot(SoundOpen);
		}
		Player.Instance.StopMove(true);
		if (FadeInTime > 0f)
		{
			MFGuiManager.Instance.FadeOut(FadeInTime * 0.5f);
			yield return new WaitForSeconds(FadeInTime * 0.5f);
		}
		Camera old = Camera.main;
		GameCamera.Instance.StoreShakingState();
		old.gameObject.SetActiveRecursively(false);
		GuiHUD.Instance.Hide();
		CutsceneCamera.SetActiveRecursively(true);
		CutsceneCamera.GetComponent<Animation>().Play(CameraAnim.name);
		if (FadeInTime > 0f)
		{
			MFGuiManager.Instance.FadeIn(FadeInTime * 0.5f);
			yield return new WaitForSeconds(FadeInTime * 0.5f);
		}
		yield return new WaitForSeconds(CutsceneCamera.GetComponent<Animation>()[CameraAnim.name].length - FadeInTime * 0.5f);
		if (FadeOutTime > 0f)
		{
			MFGuiManager.Instance.FadeOut(FadeOutTime * 0.5f);
			yield return new WaitForSeconds(FadeOutTime * 0.5f);
		}
		Player.Instance.AddWeaponToInventory(Weapon);
		CutsceneCamera.SetActiveRecursively(false);
		old.gameObject.SetActiveRecursively(true);
		GameCamera.Instance.ReStoreShakingState();
		if ((bool)WeaponVisual)
		{
			WeaponVisual.SetActiveRecursively(false);
		}
		if (FadeOutTime > 0f)
		{
			MFGuiManager.Instance.FadeIn(FadeOutTime * 0.5f);
		}
		Player.Instance.StopMove(false);
		GuiHUD.Instance.Show();
		GuiHUD.E_HudMessageType weaponMsg = GuiHUD.E_HudMessageType.E_NONE;
		switch (Weapon)
		{
		case E_WeaponType.Shotgun:
			weaponMsg = GuiHUD.E_HudMessageType.E_007_NewWeaponShotg;
			break;
		case E_WeaponType.GrenadeLauncher:
			weaponMsg = GuiHUD.E_HudMessageType.E_008_NewWeaponGrnd;
			break;
		case E_WeaponType.RocketLauncher:
			weaponMsg = GuiHUD.E_HudMessageType.E_009_NewWeaponRPG;
			break;
		}
		GuiHUD.Instance.ShowMessageTimed(weaponMsg, GuiHUD.InfoMessageTime);
		MFGuiManager.Instance.FadeIn(1f);
		if ((bool)AnimClose)
		{
			Animation.Play(AnimClose.name);
		}
		if ((bool)SoundClose)
		{
			AudioSource.PlayOneShot(SoundClose);
		}
		if (Player.Instance.Owner.WeaponComponent.CurrentWeapon != Weapon)
		{
			Player.Instance.Owner.BlackBoard.Desires.Weapon = Weapon;
			Player.Instance.Owner.WorldState.SetWSProperty(E_PropKey.WeaponChange, true);
			GuiHUD.Instance.OnCurrentWeaponChanged(Weapon);
		}
		IsInteractionFinished = true;
	}

	public override void Reset()
	{
		if ((bool)WeaponVisual)
		{
			WeaponVisual.SetActiveRecursively(true);
		}
		CancelInvoke();
		StopAllCoroutines();
		base.Reset();
	}

	private void PlayParticle()
	{
		if ((bool)ParticleEffect)
		{
			ParticleEffect.Play();
		}
	}
}
