using UnityEngine;

public class CameraStateCover : CameraState
{
	private Transform DefaultPos;

	private Transform DefaultLookat;

	private GameObject Offset;

	private Transform OffsetTransform;

	public CameraStateCover(AgentHuman owner)
		: base(owner)
	{
		DefaultPos = Owner.transform.Find("CameraTargetPos");
		DefaultLookat = Owner.transform.Find("CameraTargetDir");
		Offset = new GameObject("CameraOffset");
		OffsetTransform = Offset.transform;
		OffsetTransform.position = DefaultPos.position;
		OffsetTransform.LookAt(DefaultLookat.position);
	}

	public override Transform GetDesiredCameraTransform()
	{
		OffsetTransform.position = DefaultPos.position;
		OffsetTransform.LookAt(DefaultLookat.position);
		OffsetTransform.RotateAround(DefaultLookat.position, DefaultLookat.right, Owner.BlackBoard.Desires.Rotation.eulerAngles.x - Owner.Transform.rotation.eulerAngles.x);
		OffsetTransform.RotateAround(DefaultLookat.position, DefaultLookat.up, Owner.BlackBoard.Desires.Rotation.eulerAngles.y - Owner.Transform.rotation.eulerAngles.y);
		return OffsetTransform;
	}

	public override void Activate(Transform t)
	{
		base.Activate(t);
	}

	public override void Deactivate()
	{
		base.Deactivate();
	}
}
