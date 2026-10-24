using System;
using UnityEngine;

[Serializable]
[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Fisheye")]
public class Fisheye : PostEffectsBase
{
	public Shader fishEyeShader;

	private Material _fisheyeMaterial;

	public float strengthX;

	public float strengthY;

	public Fisheye()
	{
		strengthX = 0.05f;
		strengthY = 0.05f;
	}

	public virtual void CreateMaterials()
	{
		if (!_fisheyeMaterial)
		{
			if (!CheckShader(fishEyeShader))
			{
				enabled = false;
				return;
			}
			_fisheyeMaterial = new Material(fishEyeShader);
			_fisheyeMaterial.hideFlags = HideFlags.HideAndDontSave;
		}
	}

	public override void Start()
	{
		CreateMaterials();
	}

	public override void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		CreateMaterials();
		float num = (float)source.width * 1f / ((float)source.height * 1f);
		_fisheyeMaterial.SetVector("intensity", new Vector4(strengthX * num, strengthY * num, strengthX * num, strengthY * num));
		Graphics.Blit(source, destination, _fisheyeMaterial);
	}

	public override void Main()
	{
	}
}
