using System;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Crease")]
[ExecuteInEditMode]
public class Crease : PostEffectsBase
{
	public Shader blurShader;

	private Material _blurMaterial;

	public Shader depthFetchShader;

	private Material _depthFetchMaterial;

	public Shader creaseApplyShader;

	private Material _creaseApplyMaterial;

	public float intensity;

	public int softness;

	public float spread;

	public Crease()
	{
		intensity = 0.5f;
		softness = 1;
		spread = 1f;
	}

	public virtual void CreateMaterials()
	{
		if (!_blurMaterial)
		{
			if (!CheckShader(blurShader))
			{
				enabled = false;
				return;
			}
			_blurMaterial = new Material(blurShader);
			_blurMaterial.hideFlags = HideFlags.HideAndDontSave;
		}
		if (!_depthFetchMaterial)
		{
			if (!CheckShader(depthFetchShader))
			{
				enabled = false;
				return;
			}
			_depthFetchMaterial = new Material(depthFetchShader);
			_depthFetchMaterial.hideFlags = HideFlags.HideAndDontSave;
		}
		if (!_creaseApplyMaterial)
		{
			if (!CheckShader(creaseApplyShader))
			{
				enabled = false;
				return;
			}
			_creaseApplyMaterial = new Material(creaseApplyShader);
			_creaseApplyMaterial.hideFlags = HideFlags.HideAndDontSave;
		}
		if (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
		{
			enabled = false;
		}
	}

	public override void Start()
	{
		CreateMaterials();
	}

	public virtual void OnEnable()
	{
		GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
	}

	public override void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		CreateMaterials();
		RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height, 0);
		RenderTexture temporary2 = RenderTexture.GetTemporary(source.width / 2, source.height / 2, 0);
		RenderTexture temporary3 = RenderTexture.GetTemporary(source.width / 2, source.height / 2, 0);
		Graphics.Blit(source, temporary, _depthFetchMaterial);
		Graphics.Blit(temporary, temporary2);
		for (int i = 0; i < softness; i++)
		{
			_blurMaterial.SetVector("offsets", new Vector4(0f, spread / (float)temporary2.height, 0f, 0f));
			Graphics.Blit(temporary2, temporary3, _blurMaterial);
			_blurMaterial.SetVector("offsets", new Vector4(spread / (float)temporary2.width, 0f, 0f, 0f));
			Graphics.Blit(temporary3, temporary2, _blurMaterial);
		}
		_creaseApplyMaterial.SetTexture("_HrDepthTex", temporary);
		_creaseApplyMaterial.SetTexture("_LrDepthTex", temporary2);
		_creaseApplyMaterial.SetFloat("intensity", intensity);
		Graphics.Blit(source, destination, _creaseApplyMaterial);
		RenderTexture.ReleaseTemporary(temporary);
		RenderTexture.ReleaseTemporary(temporary2);
		RenderTexture.ReleaseTemporary(temporary3);
	}

	public override void Main()
	{
	}
}
