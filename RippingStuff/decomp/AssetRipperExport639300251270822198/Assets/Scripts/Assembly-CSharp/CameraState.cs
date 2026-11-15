using UnityEngine;

public abstract class CameraState
{
	protected AgentHuman Owner;

	public CameraState(AgentHuman owner)
	{
		Owner = owner;
	}

	public abstract Transform GetDesiredCameraTransform();

	public virtual void Activate(Transform t)
	{
		GameCamera.Instance.Reset();
	}

	public virtual void Deactivate()
	{
	}
}
