using UnityEngine;

public class RotateObject : MonoBehaviour
{
	public float RotationSpeedX;

	public float RotationSpeedY;

	public float RotationSpeedZ;

	private Transform MyTransform;

	private Renderer Mesh;

	private void Awake()
	{
		MyTransform = base.transform;
		Mesh = base.GetComponent<Renderer>();
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (!(Mesh != null) || Mesh.isVisible)
		{
			MyTransform.Rotate(RotationSpeedX * Time.deltaTime, RotationSpeedY * Time.deltaTime, RotationSpeedZ * Time.deltaTime);
		}
	}
}
