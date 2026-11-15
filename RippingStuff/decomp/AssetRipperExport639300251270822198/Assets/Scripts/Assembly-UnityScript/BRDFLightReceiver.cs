using System;
using UnityEngine;
using UnityScript.Lang;

[Serializable]
[ExecuteInEditMode]
public class BRDFLightReceiver : MonoBehaviour
{
	public float intensity;

	private Component[] renderers;

	private Shader shader;

	public float diffuseIntensity;

	public Color keyColor;

	public Color fillColor;

	public Color backColor;

	public float wrapAround;

	public float metalic;

	public float specularIntensity;

	public float specularShininess;

	public float fresnelIntensity;

	public float fresnelSharpness;

	public Color fresnelReflectionColor;

	public float translucency;

	public Color translucentColor;

	public int lookupTextureWidth;

	public int lookupTextureHeight;

	public Texture2D lookupTexture;

	private Texture2D internallyCreatedTexture;

	public int offsetRenderQueue;

	public bool affectChildren;

	public BRDFLightReceiver()
	{
		intensity = 1f;
		diffuseIntensity = 1f;
		keyColor = new Color(0.7372549f, 0.61960787f, 0.4627451f, 0f);
		fillColor = new Color(0.3372549f, 0.35686275f, 36f / 85f, 0f);
		backColor = new Color(0.17254902f, 18f / 85f, 19f / 85f, 0f);
		specularIntensity = 1f;
		specularShininess = 5f / 64f;
		fresnelSharpness = 0.5f;
		fresnelReflectionColor = new Color(0.3372549f, 0.35686275f, 36f / 85f, 0f);
		translucentColor = new Color(1f, 0.32156864f, 0.32156864f, 0f);
		lookupTextureWidth = 128;
		lookupTextureHeight = 128;
		affectChildren = true;
	}

	public virtual void Start()
	{
		if ((bool)lookupTexture)
		{
			lookupTexture.wrapMode = TextureWrapMode.Clamp;
		}
		if (Application.isEditor)
		{
			shader = Shader.Find("MADFINGER/Characters/BRDFLit  (Supports Backlight)");
			UpdateRenderers();
			if (!lookupTexture)
			{
				Preview();
			}
		}
	}

	public virtual void Preview()
	{
		UpdateRenderers();
		UpdateBRDFTexture(32, 64);
	}

	public virtual void Bake()
	{
		UpdateRenderers();
		UpdateBRDFTexture(lookupTextureWidth, lookupTextureHeight);
	}

	public virtual void Update()
	{
		if (Application.isEditor)
		{
			UpdateRenderers();
			SetupShader(shader, lookupTexture);
			if (internallyCreatedTexture != lookupTexture)
			{
				UnityEngine.Object.DestroyImmediate(internallyCreatedTexture);
			}
		}
	}

	private void UpdateRenderers()
	{
		if (affectChildren)
		{
			renderers = gameObject.GetComponentsInChildren(typeof(Renderer), true);
			return;
		}
		renderers = (Component[])new UnityScript.Lang.Array((Renderer)gameObject.GetComponent(typeof(Renderer))).ToBuiltin(typeof(Component));
	}

	private void SetupShader(Shader shader, Texture2D brdfLookupTex)
	{
		brdfLookupTex.wrapMode = TextureWrapMode.Clamp;
		int i = 0;
		Component[] array = renderers;
		for (int length = array.Length; i < length; i++)
		{
			Renderer renderer = array[i] as Renderer;
			int j = 0;
			Material[] sharedMaterials = renderer.sharedMaterials;
			for (int length2 = sharedMaterials.Length; j < length2; j++)
			{
				if ((bool)shader && sharedMaterials[j].shader != shader)
				{
					sharedMaterials[j].shader = shader;
				}
				if ((bool)brdfLookupTex)
				{
					sharedMaterials[j].SetTexture("_BRDFTex", brdfLookupTex);
				}
				sharedMaterials[j].renderQueue = 2000 + offsetRenderQueue;
			}
		}
	}

	private Color PixelFunc(float ndotl, float ndoth)
	{
		ndotl *= Mathf.Pow(ndoth, metalic);
		float num = (1f + metalic * 0.25f) * Mathf.Max(0f, diffuseIntensity - (1f - ndoth) * metalic);
		float t = Mathf.Clamp01(Mathf.InverseLerp(0f - wrapAround, 1f, ndotl * 2f - 1f));
		float t2 = Mathf.Clamp01(Mathf.InverseLerp(-1f, Mathf.Max(-0.99f, 0f - wrapAround), ndotl * 2f - 1f));
		Color color = num * Color.Lerp(backColor, Color.Lerp(fillColor, keyColor, t), t2);
		float num2 = specularShininess * 128f;
		float num3 = (num2 + 2f) * (num2 + 4f) / ((float)Math.PI * 8f * (Mathf.Pow(2f, (0f - num2) / 2f) + num2));
		float a = specularIntensity * num3 * Mathf.Pow(ndoth, num2);
		float num4 = Mathf.Lerp(0.3f, -1f, fresnelSharpness);
		float num5 = fresnelIntensity * Mathf.Max(0f, num4 + (1f - num4) * Mathf.Pow(1f - ndoth, 5f));
		float num6 = 0.5f * translucency * Mathf.Clamp01(1f - ndoth) * Mathf.Clamp01(1f - ndotl);
		Color color2 = color * intensity + fresnelReflectionColor * num5 + translucentColor * num6 + new Color(0f, 0f, 0f, a);
		return color2 * intensity;
	}

	private void FillPseudoBRDF(Texture2D tex)
	{
		for (int i = 0; i < tex.height; i++)
		{
			for (int j = 0; j < tex.width; j++)
			{
				float num = tex.width;
				float num2 = tex.height;
				float num3 = (float)j / num;
				float num4 = (float)i / num2;
				float ndotl = num3;
				float ndoth = num4;
				Color color = PixelFunc(ndotl, ndoth);
				tex.SetPixel(j, i, color);
			}
		}
	}

	private void UpdateBRDFTexture(int width, int height)
	{
		Texture2D texture2D = null;
		if (lookupTexture == internallyCreatedTexture && (bool)lookupTexture && lookupTexture.width == width && lookupTexture.height == height)
		{
			texture2D = lookupTexture;
		}
		else
		{
			if (lookupTexture == internallyCreatedTexture)
			{
				UnityEngine.Object.DestroyImmediate(lookupTexture);
			}
			texture2D = (internallyCreatedTexture = new Texture2D(width, height, TextureFormat.ARGB32, false));
		}
		FillPseudoBRDF(texture2D);
		texture2D.Apply();
		SetupShader(shader, texture2D);
		lookupTexture = texture2D;
	}

	public virtual void Main()
	{
	}
}
