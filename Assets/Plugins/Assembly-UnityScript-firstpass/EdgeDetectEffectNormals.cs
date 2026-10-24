using System;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Edge Detection (Depth-Normals)")]
[ExecuteInEditMode]
public class EdgeDetectEffectNormals : PostEffectsBase
{
	public Shader edgeDetectHqShader;

	private Material _edgeDetectHqMaterial;

	public Shader edgeDetectShader;

	private Material _edgeDetectMaterial;

	public Shader sepBlurShader;

	private Material _sepBlurMaterial;

	public Shader edgeApplyShader;

	private Material _edgeApplyMaterial;

	public bool highQuality;

	public float sensitivityDepth;

	public float sensitivityNormals;

	public float spread;

	public float edgesIntensity;

	public float edgesOnly;

	public Color edgesOnlyBgColor;

	public bool edgeBlur;

	public float blurSpread;

	public int blurIterations;

	public EdgeDetectEffectNormals()
	{
		sensitivityDepth = 1f;
		sensitivityNormals = 1f;
		spread = 1f;
		edgesIntensity = 1f;
		edgesOnlyBgColor = Color.white;
		blurSpread = 0.75f;
		blurIterations = 1;
	}

	public virtual void CreateMaterials()
	{
		if (!_edgeDetectHqMaterial)
		{
			if (!CheckShader(edgeDetectHqShader))
			{
				enabled = false;
				return;
			}
			_edgeDetectHqMaterial = new Material(edgeDetectHqShader);
			_edgeDetectHqMaterial.hideFlags = HideFlags.HideAndDontSave;
		}
		if (!_edgeDetectMaterial)
		{
			if (!CheckShader(edgeDetectShader))
			{
				enabled = false;
				return;
			}
			_edgeDetectMaterial = new Material(edgeDetectShader);
			_edgeDetectMaterial.hideFlags = HideFlags.HideAndDontSave;
		}
		if (!_sepBlurMaterial)
		{
			if (!CheckShader(sepBlurShader))
			{
				enabled = false;
				return;
			}
			_sepBlurMaterial = new Material(sepBlurShader);
			_sepBlurMaterial.hideFlags = HideFlags.HideAndDontSave;
		}
		if (!_edgeApplyMaterial)
		{
			if (!CheckShader(edgeApplyShader))
			{
				enabled = false;
				return;
			}
			_edgeApplyMaterial = new Material(edgeApplyShader);
			_edgeApplyMaterial.hideFlags = HideFlags.HideAndDontSave;
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
		GetComponent<Camera>().depthTextureMode |= DepthTextureMode.DepthNormals;
	}

	public override void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		CreateMaterials();
		Vector2 vector = new Vector2
		{
			x = sensitivityDepth,
			y = sensitivityNormals
		};
		if (highQuality)
		{
			RenderTexture temporary = RenderTexture.GetTemporary(source.width / 2, source.height / 2, 0);
			RenderTexture temporary2 = RenderTexture.GetTemporary(source.width / 2, source.height / 2, 0);
			_edgeDetectHqMaterial.SetVector("sensitivity", new Vector4(vector.x, vector.y, Mathf.Max(0.1f, spread), vector.y));
			_edgeDetectHqMaterial.SetFloat("edgesOnly", edgesOnly);
			Vector4 vector2 = edgesOnlyBgColor;
			_edgeDetectHqMaterial.SetVector("edgesOnlyBgColor", vector2);
			Graphics.Blit(source, source, _edgeDetectHqMaterial);
			if (edgeBlur)
			{
				Graphics.Blit(source, temporary);
				for (int i = 0; i < blurIterations; i++)
				{
					_sepBlurMaterial.SetVector("offsets", new Vector4(0f, blurSpread / (float)temporary.height, 0f, 0f));
					Graphics.Blit(temporary, temporary2, _sepBlurMaterial);
					_sepBlurMaterial.SetVector("offsets", new Vector4(blurSpread / (float)temporary.width, 0f, 0f, 0f));
					Graphics.Blit(temporary2, temporary, _sepBlurMaterial);
				}
				_edgeApplyMaterial.SetTexture("_EdgeTex", temporary);
				_edgeApplyMaterial.SetFloat("edgesIntensity", edgesIntensity);
				Graphics.Blit(source, destination, _edgeApplyMaterial);
			}
			else
			{
				_edgeApplyMaterial.SetTexture("_EdgeTex", source);
				_edgeApplyMaterial.SetFloat("edgesIntensity", edgesIntensity);
				Graphics.Blit(source, destination, _edgeApplyMaterial);
			}
			RenderTexture.ReleaseTemporary(temporary);
			RenderTexture.ReleaseTemporary(temporary2);
		}
		else
		{
			Graphics.Blit(source, destination, _edgeDetectMaterial);
		}
	}

	public override void Main()
	{
	}
}
