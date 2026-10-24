using System;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Sun Shafts")]
[ExecuteInEditMode]
public class SunShafts : PostEffectsBase
{
	public Shader clearShader;

	private Material _clearMaterial;

	public Shader depthDecodeShader;

	private Material _encodeDepthRGBA8Material;

	public Shader depthBlurShader;

	private Material _radialDepthBlurMaterial;

	public Shader sunShaftsShader;

	private Material _sunShaftsMaterial;

	public Shader simpleClearShader;

	private Material _simpleClearMaterial;

	public Shader compShader;

	private Material _compMaterial;

	public SunShaftsResolution resolution;

	public Transform sunTransform;

	public int radialBlurIterations;

	public Color sunColor;

	public float sunShaftBlurRadius;

	public float sunShaftIntensity;

	public float useSkyBoxAlpha;

	public float maxRadius;

	public bool useDepthTexture;

	public SunShafts()
	{
		radialBlurIterations = 2;
		sunColor = Color.white;
		sunShaftBlurRadius = 0.0164f;
		sunShaftIntensity = 1.25f;
		useSkyBoxAlpha = 0.75f;
		maxRadius = 1.25f;
		useDepthTexture = true;
	}

	public override void Start()
	{
		CreateMaterials();
	}

	public virtual void CreateMaterials()
	{
		if (!_clearMaterial)
		{
			if (!CheckShader(clearShader))
			{
				enabled = false;
				return;
			}
			_clearMaterial = new Material(clearShader);
			_clearMaterial.hideFlags = HideFlags.HideAndDontSave;
		}
		if (!_sunShaftsMaterial)
		{
			if (!CheckShader(sunShaftsShader))
			{
				enabled = false;
				return;
			}
			_sunShaftsMaterial = new Material(sunShaftsShader);
			_sunShaftsMaterial.hideFlags = HideFlags.HideAndDontSave;
		}
		if (!_encodeDepthRGBA8Material)
		{
			if (!CheckShader(depthDecodeShader))
			{
				enabled = false;
				return;
			}
			_encodeDepthRGBA8Material = new Material(depthDecodeShader);
			_encodeDepthRGBA8Material.hideFlags = HideFlags.HideAndDontSave;
		}
		if (!_radialDepthBlurMaterial)
		{
			if (!CheckShader(depthBlurShader))
			{
				enabled = false;
				return;
			}
			_radialDepthBlurMaterial = new Material(depthBlurShader);
			_radialDepthBlurMaterial.hideFlags = HideFlags.HideAndDontSave;
		}
		if (!_simpleClearMaterial)
		{
			if (!CheckShader(simpleClearShader))
			{
				enabled = false;
				return;
			}
			_simpleClearMaterial = new Material(simpleClearShader);
			_simpleClearMaterial.hideFlags = HideFlags.HideAndDontSave;
		}
		if (!_compMaterial)
		{
			if (!CheckShader(compShader))
			{
				enabled = false;
				return;
			}
			_compMaterial = new Material(compShader);
			_compMaterial.hideFlags = HideFlags.HideAndDontSave;
		}
	}

	public virtual void OnEnable()
	{
		if (useDepthTexture && SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
		{
			GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
		}
	}

	public override void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (useDepthTexture && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
		{
			useDepthTexture = false;
		}
		CreateMaterials();
		float num = 4f;
		if (resolution == SunShaftsResolution.Normal)
		{
			num = 2f;
		}
		if (resolution == SunShaftsResolution.High)
		{
			num = 1f;
		}
		RenderTexture temporary = RenderTexture.GetTemporary((int)((float)source.width / num), (int)((float)source.height / num), 0);
		RenderTexture temporary2 = RenderTexture.GetTemporary((int)((float)source.width / num), (int)((float)source.height / num), 0);
		Graphics.Blit(source, destination);
		if (!useDepthTexture)
		{
			RenderTexture renderTexture = (RenderTexture.active = RenderTexture.GetTemporary(source.width, source.height, 0));
			GL.ClearWithSkybox(false, GetComponent<Camera>());
			_compMaterial.SetTexture("_Skybox", renderTexture);
			Graphics.Blit(source, renderTexture, _compMaterial);
			RenderTexture.ReleaseTemporary(renderTexture);
		}
		else
		{
			Graphics.Blit(source, source, _clearMaterial);
		}
		_encodeDepthRGBA8Material.SetFloat("noSkyBoxMask", 1f - useSkyBoxAlpha);
		_encodeDepthRGBA8Material.SetFloat("dontUseSkyboxBrightness", 0f);
		Graphics.Blit(source, temporary2, _encodeDepthRGBA8Material);
		DrawBorder(temporary2, _simpleClearMaterial);
		Vector3 vector = Vector3.one * 0.5f;
		vector = ((!sunTransform) ? new Vector3(0.5f, 0.5f, 0f) : GetComponent<Camera>().WorldToViewportPoint(sunTransform.position));
		_radialDepthBlurMaterial.SetVector("blurRadius4", new Vector4(1f, 1f, 0f, 0f) * sunShaftBlurRadius);
		_radialDepthBlurMaterial.SetVector("sunPosition", new Vector4(vector.x, vector.y, vector.z, maxRadius));
		if (radialBlurIterations < 1)
		{
			radialBlurIterations = 1;
		}
		for (int i = 0; i < radialBlurIterations; i++)
		{
			Graphics.Blit(temporary2, temporary, _radialDepthBlurMaterial);
			Graphics.Blit(temporary, temporary2, _radialDepthBlurMaterial);
		}
		_sunShaftsMaterial.SetFloat("sunShaftIntensity", sunShaftIntensity);
		if (!(vector.z < 0f))
		{
			_sunShaftsMaterial.SetVector("sunColor", new Vector4(sunColor.r, sunColor.g, sunColor.b, sunColor.a));
		}
		else
		{
			_sunShaftsMaterial.SetVector("sunColor", new Vector4(0f, 0f, 0f, 0f));
		}
		_sunShaftsMaterial.SetTexture("_ColorBuffer", source);
		Graphics.Blit(temporary2, destination, _sunShaftsMaterial);
		RenderTexture.ReleaseTemporary(temporary2);
		RenderTexture.ReleaseTemporary(temporary);
	}

	public override void Main()
	{
	}
}
