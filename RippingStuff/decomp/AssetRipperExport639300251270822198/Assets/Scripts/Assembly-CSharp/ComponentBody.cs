using UnityEngine;

public class ComponentBody : MonoBehaviour
{
	private AgentHuman Owner;

	private Quaternion FinalRotation = default(Quaternion);

	private float RotSpeed = 15f;

	private AgentActionRotate ActionRotate;

	private float RotattionDiff;

	private float RotationSpeed
	{
		get
		{
			return RotSpeed;
		}
		set
		{
			RotSpeed = value;
		}
	}

	private void Start()
	{
		Owner = GetComponent<AgentHuman>();
		RotattionDiff = 0f;
	}

	private void LateUpdate()
	{
		if (!Owner.IsAlive)
		{
			RotattionDiff = 0f;
			return;
		}
		if (Owner.IsInCover && !Owner.IsLeavingToCover)
		{
			FinalRotation.eulerAngles = new Vector3(0f, Owner.BlackBoard.Cover.Transform.rotation.eulerAngles.y, 0f);
			if (Time.deltaTime > float.Epsilon)
			{
				Owner.Transform.rotation = Quaternion.Lerp(Owner.Transform.rotation, FinalRotation, RotationSpeed * Time.deltaTime * 5f);
			}
		}
		else
		{
			if (ActionRotate == null)
			{
				float num = Owner.Transform.rotation.eulerAngles.y - Owner.BlackBoard.Desires.Rotation.eulerAngles.y;
				if (num > 180f)
				{
					num -= 360f;
				}
				else if (num < -180f)
				{
					num += 360f;
				}
				if (num == 0f)
				{
					RotattionDiff = 0f;
				}
				else
				{
					RotattionDiff += Mathf.Abs(num);
				}
				if (RotattionDiff > 10f)
				{
					ActionRotate = AgentActionFactory.Create(AgentActionFactory.E_Type.Rotate) as AgentActionRotate;
					ActionRotate.Rotation = ((num > 0f) ? E_RotationType.Left : E_RotationType.Right);
					Owner.BlackBoard.ActionAdd(ActionRotate);
					RotattionDiff = 0f;
				}
			}
			FinalRotation.eulerAngles = new Vector3(0f, Owner.BlackBoard.Desires.Rotation.eulerAngles.y, 0f);
			if (Time.deltaTime > float.Epsilon)
			{
				Owner.Transform.rotation = Quaternion.Lerp(Owner.Transform.rotation, FinalRotation, RotationSpeed * Time.deltaTime);
			}
		}
		UpdateAiming();
		if (ActionRotate != null && !ActionRotate.IsActive())
		{
			ActionRotate = null;
		}
	}

	private void UpdateAiming()
	{
		Owner.BlackBoard.FireDir = Owner.BlackBoard.Desires.FireDirection;
	}
}
