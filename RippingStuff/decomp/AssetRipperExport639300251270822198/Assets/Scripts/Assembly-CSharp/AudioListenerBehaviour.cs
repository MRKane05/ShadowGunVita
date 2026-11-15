using UnityEngine;

public class AudioListenerBehaviour : MonoBehaviour
{
	private Transform CameraTransform;

	private Transform Transform;

	private void Start()
	{
		CameraTransform = Camera.main.transform;
		Transform = base.transform;
	}

	private void LateUpdate()
	{
		Transform.rotation = CameraTransform.rotation;
		Transform.rotation.SetLookRotation(CameraTransform.forward);
	}
}
