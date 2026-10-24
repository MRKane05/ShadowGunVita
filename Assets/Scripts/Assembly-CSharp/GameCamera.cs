using System;
using System.Collections;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
	public enum ShakingEffect
	{
		None = -1,
		Small = 0,
		Medium = 1,
		Big = 2
	}

	public enum ShakingLoopEffect
	{
		Stop = -1,
		Play_1 = 0
	}

	public static GameCamera Instance;

	public CameraBehaviour CameraBehaviour;

	public Transform CameraOrigin;

	private Animation Animation;

	private Animation ParentAnimation;

	private Transform CameraTransform;

	private Camera Camera;

	private float DisabledTime;

	private float DefaultFOV;

	private float ChangeFOVSpeed;

	private float DesiredFov;

	private Collider IgnoredCollider;

	private float m_LoopShakingIntensity;

	public Vector3 CameraForward
	{
		get
		{
			return CameraTransform.forward;
		}
	}

	private void Awake()
	{
		Instance = this;
		Camera = Camera.main;
		Animation = Camera.GetComponent<Animation>();
		ParentAnimation = base.GetComponent<Animation>();
		CameraTransform = base.transform;
		if (Screen.width == 1024 && Screen.height == 768)
		{
			Camera.fieldOfView = 70f;
		}
		else
		{
			Camera.fieldOfView = 55f;
		}
		DesiredFov = (DefaultFOV = Camera.fieldOfView);
	}

	private void Start()
	{
		DisabledTime = 0f;
		IgnoredCollider = Player.Instance.Owner.CharacterController;
	}

	private void LateUpdate()
	{
		if (DisabledTime >= Time.timeSinceLevelLoad || Time.deltaTime == 0f)
		{
			return;
		}
		Transform desiredCameraTransform = CameraBehaviour.GetDesiredCameraTransform();
		if (!desiredCameraTransform)
		{
			return;
		}
		Vector3 vector = desiredCameraTransform.position - CameraOrigin.position;
		RaycastHit[] array = Physics.SphereCastAll(CameraOrigin.position, 0.2f, vector.normalized, vector.magnitude);
		if (array.Length > 1)
		{
			Array.Sort(array, CollisionUtils.CompareHits);
		}
		CameraTransform.position = desiredCameraTransform.position;
		Camera.nearClipPlane = 0.3f;
		RaycastHit[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			RaycastHit raycastHit = array2[i];
			if (raycastHit.collider.gameObject.layer != InteractionObject.UseLayer && !raycastHit.collider.isTrigger && !(raycastHit.collider == IgnoredCollider))
			{
				CameraTransform.position = raycastHit.point + raycastHit.normal * 0.3f;
				Camera.nearClipPlane = 0.05f;
			}
		}
		CameraTransform.rotation = desiredCameraTransform.rotation;
	}

	private void Update()
	{
		if (Camera.fieldOfView < DesiredFov)
		{
			Camera.fieldOfView = Mathf.Min(DesiredFov, Camera.fieldOfView + ChangeFOVSpeed * Time.deltaTime);
		}
		else if (Camera.fieldOfView > DesiredFov)
		{
			Camera.fieldOfView = Mathf.Max(DesiredFov, Camera.fieldOfView - ChangeFOVSpeed * Time.deltaTime);
		}
	}

	public void StoreShakingState()
	{
	}

	public void ReStoreShakingState()
	{
		RunBlendBackGroundAnim();
		if (m_LoopShakingIntensity > 0.01f)
		{
			ShakingLoopAction(ShakingLoopEffect.Play_1, m_LoopShakingIntensity);
		}
	}

	public void PlayShakingAnim(ShakingEffect inShakingEffect, float inIntensity)
	{
		if (inShakingEffect >= ShakingEffect.Small && inShakingEffect <= ShakingEffect.Big)
		{
			string[] array = new string[3] { "cameraShakeOnceSmall", "cameraShakeOnce", "cameraShakeOnceBig" };
			string text = array[(int)inShakingEffect];
			float targetWeight = Mathf.Max(0f, inIntensity);
			if (!(Animation[text] == null) && !(inIntensity <= 0f))
			{
				Animation[text].blendMode = AnimationBlendMode.Blend;
				Animation[text].layer = 2;
				Animation.Blend(text, targetWeight);
			}
		}
	}

	public void ShakingLoopAction(ShakingLoopEffect inShakingEffect, float inIntensity)
	{
		switch (inShakingEffect)
		{
		case ShakingLoopEffect.Stop:
			m_LoopShakingIntensity = 0f;
			{
				foreach (AnimationState item in Animation)
				{
					if (item.layer == 1)
					{
						item.enabled = false;
					}
				}
				break;
			}
		case ShakingLoopEffect.Play_1:
		{
			string text = "cameraShakeLoop";
			if (!(Animation[text] == null) && !(inIntensity <= 0f))
			{
				Animation[text].blendMode = AnimationBlendMode.Blend;
				Animation[text].wrapMode = WrapMode.Loop;
				Animation[text].layer = 1;
				float num = Mathf.Max(0f, inIntensity);
				Animation.Blend(text, num);
				m_LoopShakingIntensity = num;
			}
			break;
		}
		default:
			Debug.LogError("Unknown Shaking Loop Effect");
			break;
		}
	}

	public void Reset()
	{
		Camera.fieldOfView = DefaultFOV;
		DesiredFov = DefaultFOV;
		foreach (AnimationState item in Animation)
		{
			item.enabled = false;
		}
		RunBlendBackGroundAnim();
	}

	public void Activate(Vector3 pos, Vector3 lookAt)
	{
		DisabledTime = 0f;
		CameraTransform.position = pos;
		CameraTransform.LookAt(lookAt);
	}

	public void SetFov(float fov, float speed)
	{
		DesiredFov = fov;
		ChangeFOVSpeed = speed;
	}

	public void SetDefaultFov(float speed)
	{
		SetFov(DefaultFOV, speed);
	}

	private IEnumerator FadeInOutAndCameraPlay(string animName)
	{
		MFGuiManager.Instance.FadeOut(0.1f, 1f);
		yield return new WaitForSeconds(0.2f);
		ParentAnimation[animName].blendMode = AnimationBlendMode.Blend;
		ParentAnimation.CrossFade(animName, 0.5f);
		DisabledTime = Time.timeSinceLevelLoad + ParentAnimation[animName].length;
		StartCoroutine(FadeInOutAndCameraPlayEnd(ParentAnimation[animName].length - 0.5f));
		yield return new WaitForSeconds(0.1f);
		MFGuiManager.Instance.FadeIn();
	}

	private IEnumerator FadeInOutAndCameraPlayEnd(float delay)
	{
		yield return new WaitForSeconds(delay);
		MFGuiManager.Instance.FadeOut(0.3f, 1f);
		yield return new WaitForSeconds(0.5f);
		MFGuiManager.Instance.FadeIn();
	}

	private void RunBlendBackGroundAnim()
	{
		string text = "cameraShakeLoopNull";
		Animation[text].blendMode = AnimationBlendMode.Blend;
		Animation[text].layer = 0;
		Animation[text].wrapMode = WrapMode.Loop;
		Animation.Play(text);
	}
}
