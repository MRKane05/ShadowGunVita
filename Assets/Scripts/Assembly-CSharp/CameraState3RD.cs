using UnityEngine;

public class CameraState3RD : CameraState
{
	private Transform DefaultPos;

	private Transform DefaultLookat;

	private GameObject Offset;

	private Transform OffsetTransform;

	public CameraState3RD(AgentHuman owner)
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
		OffsetTransform.RotateAround(DefaultLookat.position, DefaultLookat.right, Owner.BlackBoard.Desires.Rotation.eulerAngles.x);
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
