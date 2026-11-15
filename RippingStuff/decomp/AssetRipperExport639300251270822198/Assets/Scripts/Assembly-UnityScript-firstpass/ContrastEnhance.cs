using System;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Contrast Enhance (Unsharp Mask)")]
[ExecuteInEditMode]
public class ContrastEnhance : PostEffectsBase
{
	public float intensity;

	public float threshhold;

	private Material _separableBlurMaterial;

	private Material _contrastCompositeMaterial;

	public float blurSpread;

	public Shader separableBlurShader;

	public Shader contrastCompositeShader;

	public ContrastEnhance()
	{
		intensity = 0.5f;
		blurSpread = 1f;
	}

	public virtual void CreateMaterials()
	{
		if (!_contrastCompositeMaterial)
		{
			if (!CheckShader(contrastCompositeShader))
			{
				enabled = false;
				return;
			}
			_contrastCompositeMaterial = new Material(contrastCompositeShader);
			_contrastCompositeMaterial.hideFlags = HideFlags.HideAndDontSave;
		}
		if (!_separableBlurMaterial)
		{
			if (!CheckShader(separableBlurShader))
			{
				enabled = false;
				return;
			}
			_separableBlurMaterial = new Material(separableBlurShader);
			_separableBlurMaterial.hideFlags = HideFlags.HideAndDontSave;
		}
	}

	public override void Start()
	{
		CreateMaterials();
	}

	public override void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		CreateMaterials();
		RenderTexture temporary = RenderTexture.GetTemporary((int)((float)source.width / 2f), (int)((float)source.height / 2f), 0);
		RenderTexture temporary2 = RenderTexture.GetTemporary((int)((float)source.width / 4f), (int)((float)source.height / 4f), 0);
		RenderTexture temporary3 = RenderTexture.GetTemporary((int)((float)source.width / 4f), (int)((float)source.height / 4f), 0);
		Graphics.Blit(source, temporary);
		Graphics.Blit(temporary, temporary2);
		_separableBlurMaterial.SetVector("offsets", new Vector4(0f, blurSpread * 1f / (float)temporary2.height, 0f, 0f));
		Graphics.Blit(temporary2, temporary3, _separableBlurMaterial);
		_separableBlurMaterial.SetVector("offsets", new Vector4(blurSpread * 1f / (float)temporary2.width, 0f, 0f, 0f));
		Graphics.Blit(temporary3, temporary2, _separableBlurMaterial);
		_contrastCompositeMaterial.SetTexture("_MainTexBlurred", temporary2);
		_contrastCompositeMaterial.SetFloat("intensity", intensity);
		_contrastCompositeMaterial.SetFloat("threshhold", threshhold);
		Graphics.Blit(source, destination, _contrastCompositeMaterial);
		RenderTexture.ReleaseTemporary(temporary);
		RenderTexture.ReleaseTemporary(temporary2);
		RenderTexture.ReleaseTemporary(temporary3);
	}

	public override void Main()
	{
	}
}
