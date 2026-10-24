using System;
using UnityEngine;

[Serializable]
[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Color Correction (Curves)")]
public class ColorCorrectionCurves : PostEffectsBase
{
	public AnimationCurve redChannel;

	public AnimationCurve greenChannel;

	public AnimationCurve blueChannel;

	public bool useDepthCorrection;

	public AnimationCurve zCurve;

	public AnimationCurve depthRedChannel;

	public AnimationCurve depthGreenChannel;

	public AnimationCurve depthBlueChannel;

	private Material _ccMaterial;

	private Material _ccDepthMaterial;

	private Material _selectiveCcMaterial;

	private Texture2D _rgbChannelTex;

	private Texture2D _rgbDepthChannelTex;

	private Texture2D _zCurve;

	public bool selectiveCc;

	public Color selectiveFromColor;

	public Color selectiveToColor;

	public bool updateTextures;

	public ColorCorrectionMode mode;

	public Shader colorCorrectionCurvesShader;

	public Shader simpleColorCorrectionCurvesShader;

	public Shader colorCorrectionSelectiveShader;

	public ColorCorrectionCurves()
	{
		selectiveFromColor = Color.white;
		selectiveToColor = Color.white;
		updateTextures = true;
	}

	public override void Start()
	{
		updateTextures = true;
		CreateMaterials();
	}

	public virtual void CreateMaterials()
	{
		if (!_ccMaterial)
		{
			if (!CheckShader(simpleColorCorrectionCurvesShader))
			{
				enabled = false;
				return;
			}
			_ccMaterial = new Material(simpleColorCorrectionCurvesShader);
			_ccMaterial.hideFlags = HideFlags.HideAndDontSave;
		}
		if (!_ccDepthMaterial)
		{
			if (!CheckShader(colorCorrectionCurvesShader))
			{
				enabled = false;
				return;
			}
			_ccDepthMaterial = new Material(colorCorrectionCurvesShader);
			_ccDepthMaterial.hideFlags = HideFlags.HideAndDontSave;
		}
		if (!_selectiveCcMaterial)
		{
			if (!CheckShader(colorCorrectionSelectiveShader))
			{
				enabled = false;
				return;
			}
			_selectiveCcMaterial = new Material(colorCorrectionSelectiveShader);
			_selectiveCcMaterial.hideFlags = HideFlags.HideAndDontSave;
		}
		if (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
		{
			enabled = false;
			return;
		}
		if (!_rgbChannelTex)
		{
			_rgbChannelTex = new Texture2D(256, 4, TextureFormat.ARGB32, false);
			_rgbChannelTex.hideFlags = HideFlags.HideAndDontSave;
		}
		if (!_rgbDepthChannelTex)
		{
			_rgbDepthChannelTex = new Texture2D(256, 4, TextureFormat.ARGB32, false);
			_rgbDepthChannelTex.hideFlags = HideFlags.HideAndDontSave;
		}
		if (!_zCurve)
		{
			_zCurve = new Texture2D(256, 1, TextureFormat.ARGB32, false);
			_zCurve.hideFlags = HideFlags.HideAndDontSave;
		}
		_rgbChannelTex.wrapMode = TextureWrapMode.Clamp;
		_rgbDepthChannelTex.wrapMode = TextureWrapMode.Clamp;
		_zCurve.wrapMode = TextureWrapMode.Clamp;
	}

	public virtual void OnEnable()
	{
		if (useDepthCorrection)
		{
			GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
		}
	}

	public virtual void OnDisable()
	{
	}

	public virtual void UpdateParameters()
	{
		if (updateTextures && redChannel != null && greenChannel != null && blueChannel != null)
		{
			for (float num = 0f; num <= 1f; num += 0.003921569f)
			{
				float num2 = Mathf.Clamp(redChannel.Evaluate(num), 0f, 1f);
				float num3 = Mathf.Clamp(greenChannel.Evaluate(num), 0f, 1f);
				float num4 = Mathf.Clamp(blueChannel.Evaluate(num), 0f, 1f);
				_rgbChannelTex.SetPixel((int)Mathf.Floor(num * 255f), 0, new Color(num2, num2, num2));
				_rgbChannelTex.SetPixel((int)Mathf.Floor(num * 255f), 1, new Color(num3, num3, num3));
				_rgbChannelTex.SetPixel((int)Mathf.Floor(num * 255f), 2, new Color(num4, num4, num4));
				float num5 = Mathf.Clamp(zCurve.Evaluate(num), 0f, 1f);
				_zCurve.SetPixel((int)Mathf.Floor(num * 255f), 0, new Color(num5, num5, num5));
				num2 = Mathf.Clamp(depthRedChannel.Evaluate(num), 0f, 1f);
				num3 = Mathf.Clamp(depthGreenChannel.Evaluate(num), 0f, 1f);
				num4 = Mathf.Clamp(depthBlueChannel.Evaluate(num), 0f, 1f);
				_rgbDepthChannelTex.SetPixel((int)Mathf.Floor(num * 255f), 0, new Color(num2, num2, num2));
				_rgbDepthChannelTex.SetPixel((int)Mathf.Floor(num * 255f), 1, new Color(num3, num3, num3));
				_rgbDepthChannelTex.SetPixel((int)Mathf.Floor(num * 255f), 2, new Color(num4, num4, num4));
			}
			_rgbChannelTex.Apply();
			_rgbDepthChannelTex.Apply();
			_zCurve.Apply();
			updateTextures = false;
		}
	}

	public override void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		CreateMaterials();
		UpdateParameters();
		if (useDepthCorrection)
		{
			GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
		}
		RenderTexture renderTexture = destination;
		if (selectiveCc)
		{
			renderTexture = RenderTexture.GetTemporary(source.width, source.height);
		}
		if (useDepthCorrection)
		{
			_ccDepthMaterial.SetTexture("_RgbTex", _rgbChannelTex);
			_ccDepthMaterial.SetTexture("_ZCurve", _zCurve);
			_ccDepthMaterial.SetTexture("_RgbDepthTex", _rgbDepthChannelTex);
			Graphics.Blit(source, renderTexture, _ccDepthMaterial);
		}
		else
		{
			_ccMaterial.SetTexture("_RgbTex", _rgbChannelTex);
			Graphics.Blit(source, renderTexture, _ccMaterial);
		}
		if (selectiveCc)
		{
			_selectiveCcMaterial.SetVector("selColor", new Vector4(selectiveFromColor.r, selectiveFromColor.g, selectiveFromColor.b, selectiveFromColor.a));
			_selectiveCcMaterial.SetVector("targetColor", new Vector4(selectiveToColor.r, selectiveToColor.g, selectiveToColor.b, selectiveToColor.a));
			Graphics.Blit(renderTexture, destination, _selectiveCcMaterial);
			RenderTexture.ReleaseTemporary(renderTexture);
		}
	}

	public override void Main()
	{
	}
}
