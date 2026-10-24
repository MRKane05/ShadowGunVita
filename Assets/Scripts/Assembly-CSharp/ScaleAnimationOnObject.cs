using System;
using UnityEngine;

public class ScaleAnimationOnObject : MonoBehaviour
{
	public enum Scaletype
	{
		Coserp = 0,
		Sinerp = 1
	}

	public Vector3 ScalePeek = new Vector3(2f, 2f, 2f);

	public Scaletype Type = Scaletype.Sinerp;

	public float Speed = 1f;

	private Transform MyTransform;

	private Renderer Mesh;

	private Vector3 Center;

	private float ScaleTime;

	private void Awake()
	{
		MyTransform = base.transform;
		Center = MyTransform.localScale;
	}

	private void Start()
	{
		Mesh = base.GetComponent<Renderer>();
		ScaleTime = 0f;
	}

	private void Update()
	{
		if (!(Mesh != null) || Mesh.isVisible)
		{
			ScaleTime += Speed * Time.deltaTime;
			switch (Type)
			{
			case Scaletype.Coserp:
				MyTransform.localScale = Center + ScalePeek * Mathf.Cos(ScaleTime * (float)Math.PI);
				break;
			case Scaletype.Sinerp:
				MyTransform.localScale = Center + ScalePeek * Mathf.Sin(ScaleTime * (float)Math.PI);
				break;
			}
		}
	}
}
