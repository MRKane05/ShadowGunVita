#define DEBUG
using UnityEngine;

[AddComponentMenu("Image Effects/MADFINGER color correction")]
[ExecuteInEditMode]
public class MFColorCorrectionEffect : ImageEffectBase
{
	public float Brightness = 1f;

	public float Contrast = 1f;

	public float Saturation = 1f;

	public float R_offs;

	public float G_offs;

	public float B_offs;

	private Matrix4x4 CalcSaturationMatrix(float sat)
	{
		Matrix4x4 identity = Matrix4x4.identity;
		float num = 0.3086f;
		float num2 = 0.6094f;
		float num3 = 0.082f;
		float value = (1f - sat) * num + sat;
		float value2 = (1f - sat) * num;
		float value3 = (1f - sat) * num;
		float value4 = (1f - sat) * num2;
		float value5 = (1f - sat) * num2 + sat;
		float value6 = (1f - sat) * num2;
		float value7 = (1f - sat) * num3;
		float value8 = (1f - sat) * num3;
		float value9 = (1f - sat) * num3 + sat;
		identity[0, 0] = value;
		identity[1, 0] = value2;
		identity[2, 0] = value3;
		identity[3, 0] = 0f;
		identity[0, 1] = value4;
		identity[1, 1] = value5;
		identity[2, 1] = value6;
		identity[3, 1] = 0f;
		identity[0, 2] = value7;
		identity[1, 2] = value8;
		identity[2, 2] = value9;
		identity[3, 2] = 0f;
		identity[0, 3] = 0f;
		identity[1, 3] = 0f;
		identity[2, 3] = 0f;
		identity[3, 3] = 1f;
		return identity;
	}

	private Matrix4x4 ColorOffsetMatrix(float rOffs, float gOffs, float bOffs)
	{
		Matrix4x4 identity = Matrix4x4.identity;
		identity[0, 3] = rOffs;
		identity[1, 3] = gOffs;
		identity[2, 3] = bOffs;
		return identity;
	}

	private Matrix4x4 ColorScaleMatrix(float r, float g, float b)
	{
		Matrix4x4 identity = Matrix4x4.identity;
		identity[0, 0] = r;
		identity[1, 1] = g;
		identity[2, 2] = b;
		return identity;
	}

	private Matrix4x4 ColorContrastMatrix(float c)
	{
		return ColorOffsetMatrix(0.5f, 0.5f, 0.5f) * ColorScaleMatrix(c, c, c) * ColorOffsetMatrix(-0.5f, -0.5f, -0.5f);
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		DebugUtils.Assert(shader);
		Debug.Log("tt");
		if ((bool)base.material)
		{
			Matrix4x4 matrix4x = CalcSaturationMatrix(Saturation);
			Matrix4x4 matrix4x2 = ColorContrastMatrix(Contrast);
			Matrix4x4 matrix4x3 = ColorOffsetMatrix(R_offs, G_offs, B_offs);
			Matrix4x4 matrix4x4 = ColorOffsetMatrix(Brightness - 1f, Brightness - 1f, Brightness - 1f);
			Matrix4x4 matrix4x5 = matrix4x4 * matrix4x2 * matrix4x * matrix4x3;
			base.material.shader = shader;
			base.material.SetMatrix("_ColorMatrix", matrix4x5.transpose);
		}
		Graphics.Blit(source, destination, base.material);
	}
}
