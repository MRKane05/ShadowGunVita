using UnityEngine;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Vignette")]
[ExecuteInEditMode]
public class Vignetting : MonoBehaviour
{
	public Shader vignetteShader;

	private Material _vignetteMaterial;

	public Shader separableBlurShader;

	private Material _separableBlurMaterial;

	public Shader chromAberrationShader;

	private Material _chromAberrationMaterial;

	public float vignetteIntensity = 0.375f;

	public float chromaticAberrationIntensity;

	public float blurVignette;

	private void Start()
	{
		CreateMaterials();
	}

	private bool CheckShader(Shader s)
	{
		if (!s.isSupported)
		{
			ReportNotSupported();
			return false;
		}
		return true;
	}

	private void ReportNotSupported()
	{
		Debug.LogError("The image effect is not supported on this platform!");
		base.enabled = false;
	}

	private void CreateMaterials()
	{
		if (!_vignetteMaterial)
		{
			if (!CheckShader(vignetteShader))
			{
				base.enabled = false;
				return;
			}
			_vignetteMaterial = new Material(vignetteShader);
			_vignetteMaterial.hideFlags = HideFlags.HideAndDontSave;
		}
		if (!_separableBlurMaterial)
		{
			if (!CheckShader(separableBlurShader))
			{
				base.enabled = false;
				return;
			}
			_separableBlurMaterial = new Material(separableBlurShader);
			_separableBlurMaterial.hideFlags = HideFlags.HideAndDontSave;
		}
		if (!_chromAberrationMaterial)
		{
			if (!CheckShader(chromAberrationShader))
			{
				base.enabled = false;
				return;
			}
			_chromAberrationMaterial = new Material(chromAberrationShader);
			_chromAberrationMaterial.hideFlags = HideFlags.HideAndDontSave;
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		CreateMaterials();
		RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height, 0);
		RenderTexture temporary2 = RenderTexture.GetTemporary(source.width / 2, source.height / 2, 0);
		RenderTexture temporary3 = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0);
		RenderTexture temporary4 = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0);
		Graphics.Blit(source, temporary2);
		Graphics.Blit(temporary2, temporary3);
		for (int i = 0; i < 2; i++)
		{
			_separableBlurMaterial.SetVector("offsets", new Vector4(0f, 1.5f / (float)temporary3.height, 0f, 0f));
			Graphics.Blit(temporary3, temporary4, _separableBlurMaterial);
			_separableBlurMaterial.SetVector("offsets", new Vector4(1.5f / (float)temporary3.width, 0f, 0f, 0f));
			Graphics.Blit(temporary4, temporary3, _separableBlurMaterial);
		}
		_vignetteMaterial.SetFloat("vignetteIntensity", vignetteIntensity);
		_vignetteMaterial.SetFloat("blurVignette", blurVignette);
		_vignetteMaterial.SetTexture("_VignetteTex", temporary3);
		Graphics.Blit(source, temporary, _vignetteMaterial);
		_chromAberrationMaterial.SetFloat("chromaticAberrationIntensity", chromaticAberrationIntensity);
		Graphics.Blit(temporary, destination, _chromAberrationMaterial);
		RenderTexture.ReleaseTemporary(temporary);
		RenderTexture.ReleaseTemporary(temporary2);
		RenderTexture.ReleaseTemporary(temporary3);
		RenderTexture.ReleaseTemporary(temporary4);
	}
}
