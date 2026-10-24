using UnityEngine;

[ExecuteInEditMode]
public class RadialBlur : MonoBehaviour
{
	public Shader rbShader;

	public float blurStrength = 2.2f;

	public float blurWidth = 1f;

	private Material rbMaterial;

	private bool isOpenGL;

	private Material GetMaterial()
	{
		if (rbMaterial == null)
		{
			rbMaterial = new Material(rbShader);
			rbMaterial.hideFlags = HideFlags.HideAndDontSave;
		}
		return rbMaterial;
	}

	private void Start()
	{
		if (rbShader == null)
		{
			Debug.LogError("shader missing!", this);
		}
		isOpenGL = SystemInfo.graphicsDeviceVersion.StartsWith("OpenGL");
	}

	private void OnRenderImage(RenderTexture source, RenderTexture dest)
	{
		float value = 1f;
		float value2 = 1f;
		if (isOpenGL)
		{
			value = source.width;
			value2 = source.height;
		}
		GetMaterial().SetFloat("_BlurStrength", blurStrength);
		GetMaterial().SetFloat("_BlurWidth", blurWidth);
		GetMaterial().SetFloat("_iHeight", value);
		GetMaterial().SetFloat("_iWidth", value2);
		Graphics.Blit(source, dest, GetMaterial());
	}
}
