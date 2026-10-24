using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[AddComponentMenu("")]
public class MFGuiRenderer : MonoBehaviour
{
	public enum ZeroLocationEnum
	{
		LowerLeft = -1,
		UpperLeft = 1
	}

	public enum SPRITE_PLANE
	{
		XY = 0,
		XZ = 1,
		YZ = 2
	}

	public LayerMask UILayer = 0;

	public ZeroLocationEnum ZeroLocation = ZeroLocationEnum.LowerLeft;

	private float _xOffset;

	private float _yOffset;

	public Material m_Material;

	public int allocBlockSize = 10;

	public SPRITE_PLANE plane;

	protected List<MFGuiSprite> availableBlocks = new List<MFGuiSprite>();

	protected bool vertsChanged;

	protected bool uvsChanged;

	protected bool colorsChanged;

	protected MFGuiSprite[] sprites;

	protected List<MFGuiSprite> activeBlocks = new List<MFGuiSprite>();

	protected float boundUpdateInterval;

	protected MeshFilter meshFilter;

	protected MeshRenderer meshRenderer;

	protected Mesh mesh;

	protected Vector3[] vertices;

	protected int[] triIndices;

	protected Vector2[] UVs;

	protected Color[] colors;

	private static int s_RenderersCount;

	public int m_RegisterdWidgetsCount;

	public Vector2 PixelSpaceToUVSpace(Vector2 xy)
	{
		Texture texture = m_Material.GetTexture("_MainTex");
		return new Vector2(xy.x / (float)texture.width, xy.y / (float)texture.height);
	}

	public Vector2 PixelSpaceToUVSpace(int x, int y)
	{
		return PixelSpaceToUVSpace(new Vector2(x, y));
	}

	public Vector2 PixelCoordToUVCoord(Vector2 xy)
	{
		Vector2 result = PixelSpaceToUVSpace(xy);
		result.y = 1f - result.y;
		return result;
	}

	public Vector2 PixelCoordToUVCoord(int x, int y)
	{
		return PixelCoordToUVCoord(new Vector2(x, y));
	}

	public void SetMaterial(Material mat)
	{
		m_Material = mat;
		meshRenderer.GetComponent<Renderer>().material = m_Material;
	}

	public Material GetMaterial()
	{
		return m_Material;
	}

	public bool IsAnySpriteActive()
	{
		return activeBlocks.Count > 0;
	}

	protected virtual void Awake()
	{
		s_RenderersCount++;
		base.gameObject.AddComponent<MeshFilter>();
		base.gameObject.AddComponent<MeshRenderer>();
		meshFilter = (MeshFilter)GetComponent(typeof(MeshFilter));
		meshRenderer = (MeshRenderer)GetComponent(typeof(MeshRenderer));
		meshRenderer.castShadows = false;
		meshRenderer.receiveShadows = false;
		meshRenderer.GetComponent<Renderer>().material = m_Material;
		mesh = meshFilter.mesh;
		EnlargeArrays(allocBlockSize);
		base.transform.position = Vector3.zero;
		base.transform.rotation = Quaternion.identity;
		UpdateUISize();
	}

	private void OnDestroy()
	{
		mesh = null;
		if (m_Material != null)
		{
			m_Material = null;
		}
		availableBlocks.Clear();
		activeBlocks.Clear();
		sprites = null;
		meshFilter = null;
		meshRenderer = null;
	}

	public void UpdateUISize()
	{
		_xOffset = (float)(-Screen.width) / 2f;
		_yOffset = (float)Screen.height / 2f;
	}

	protected void InitArrays()
	{
		sprites = new MFGuiSprite[1];
		sprites[0] = new MFGuiSprite();
		vertices = new Vector3[4];
		UVs = new Vector2[4];
		colors = new Color[4];
		triIndices = new int[6];
	}

	protected int EnlargeArrays(int count)
	{
		int num;
		if (sprites == null)
		{
			InitArrays();
			num = 0;
			count--;
		}
		else
		{
			num = sprites.Length;
		}
		MFGuiSprite[] array = sprites;
		sprites = new MFGuiSprite[sprites.Length + count];
		array.CopyTo(sprites, 0);
		Vector3[] array2 = vertices;
		vertices = new Vector3[vertices.Length + count * 4];
		array2.CopyTo(vertices, 0);
		Vector2[] uVs = UVs;
		UVs = new Vector2[UVs.Length + count * 4];
		uVs.CopyTo(UVs, 0);
		Color[] array3 = colors;
		colors = new Color[colors.Length + count * 4];
		array3.CopyTo(colors, 0);
		int[] array4 = triIndices;
		triIndices = new int[triIndices.Length + count * 6];
		array4.CopyTo(triIndices, 0);
		for (int i = 0; i < num; i++)
		{
			sprites[i].SetBuffers(vertices, UVs);
		}
		for (int j = num; j < sprites.Length; j++)
		{
			sprites[j] = new MFGuiSprite();
			sprites[j].index = j;
			sprites[j].guiRenderer = this;
			sprites[j].SetBuffers(vertices, UVs);
			sprites[j].SetColor(Color.white);
			availableBlocks.Add(sprites[j]);
			triIndices[j * 6] = j * 4;
			triIndices[j * 6 + 1] = j * 4 + 3;
			triIndices[j * 6 + 2] = j * 4 + 1;
			triIndices[j * 6 + 3] = j * 4 + 3;
			triIndices[j * 6 + 4] = j * 4 + 2;
			triIndices[j * 6 + 5] = j * 4 + 1;
		}
		vertsChanged = true;
		uvsChanged = true;
		colorsChanged = true;
		return num;
	}

	public int RegisterWidget(GUIBase_Widget inWidget)
	{
		m_RegisterdWidgetsCount++;
		return m_RegisterdWidgetsCount;
	}

	public int UnRegisterWidget(GUIBase_Widget inWidget)
	{
		m_RegisterdWidgetsCount--;
		return m_RegisterdWidgetsCount;
	}

	public MFGuiSprite AddSprite(GameObject client, Matrix4x4 matrix, float width, float height, float rotAngle, float depth, Vector2 lowerLeftUV, Vector2 UVDimensions)
	{
		if (availableBlocks.Count < 1)
		{
			EnlargeArrays(allocBlockSize);
		}
		int index = availableBlocks[0].index;
		availableBlocks.RemoveAt(0);
		MFGuiSprite mFGuiSprite = sprites[index];
		mFGuiSprite.client = true;
		mFGuiSprite.matrix = matrix;
		mFGuiSprite.lowerLeftUV = lowerLeftUV;
		mFGuiSprite.uvDimensions = UVDimensions;
		Vector3 vector = default(Vector3);
		vector = Mathfx.Matrix_GetEulerAngles(mFGuiSprite.matrix);
		vector.z = rotAngle * ((float)Math.PI / 180f);
		Mathfx.Matrix_SetEulerAngles(ref mFGuiSprite.matrix, vector);
		switch (plane)
		{
		case SPRITE_PLANE.XY:
			mFGuiSprite.SetSizeXY(width, height, depth);
			break;
		case SPRITE_PLANE.XZ:
			mFGuiSprite.SetSizeXZ(width, height, depth);
			break;
		case SPRITE_PLANE.YZ:
			mFGuiSprite.SetSizeYZ(width, height, depth);
			break;
		default:
			mFGuiSprite.SetSizeXY(width, height, depth);
			break;
		}
		activeBlocks.Add(mFGuiSprite);
		mFGuiSprite.Transform();
		int num = 4 * mFGuiSprite.index;
		UVs[num] = lowerLeftUV + Vector2.up * UVDimensions.y;
		UVs[num + 1] = lowerLeftUV;
		UVs[num + 2] = lowerLeftUV + Vector2.right * UVDimensions.x;
		UVs[num + 3] = lowerLeftUV + UVDimensions;
		vertsChanged = true;
		uvsChanged = true;
		return mFGuiSprite;
	}

	public void RemoveSprite(MFGuiSprite sprite)
	{
		sprite.SetSizeXY(0f, 0f, 0f);
		sprite.v1 = Vector3.zero;
		sprite.v2 = Vector3.zero;
		sprite.v3 = Vector3.zero;
		sprite.v4 = Vector3.zero;
		int num = 4 * sprite.index;
		vertices[num] = sprite.v1;
		vertices[num + 1] = sprite.v2;
		vertices[num + 2] = sprite.v3;
		vertices[num + 3] = sprite.v4;
		activeBlocks.Remove(sprite);
		sprite.client = false;
		sprite.matrix = Matrix4x4.identity;
		sprite.hidden = false;
		sprite.SetColor(Color.white);
		sprite.offset = Vector3.zero;
		availableBlocks.Add(sprite);
		vertsChanged = true;
	}

	public void HideSprite(MFGuiSprite sprite)
	{
		activeBlocks.Remove(sprite);
		sprite.m_hidden___DoNotAccessExternally = true;
		int num = 4 * sprite.index;
		vertices[num] = Vector3.zero;
		vertices[num + 1] = Vector3.zero;
		vertices[num + 2] = Vector3.zero;
		vertices[num + 3] = Vector3.zero;
		vertsChanged = true;
	}

	public void ShowSprite(MFGuiSprite sprite)
	{
		if (sprite.client && sprite.m_hidden___DoNotAccessExternally)
		{
			sprite.m_hidden___DoNotAccessExternally = false;
			sprite.Transform();
			activeBlocks.Add(sprite);
			vertsChanged = true;
		}
	}

	public MFGuiSprite GetSprite(int i)
	{
		if (i < sprites.Length)
		{
			return sprites[i];
		}
		return null;
	}

	public void Transform(MFGuiSprite sprite)
	{
		sprite.Transform();
		vertsChanged = true;
	}

	public void UpdatePositions()
	{
		vertsChanged = true;
	}

	public void UpdateUV(MFGuiSprite sprite)
	{
		int num = 4 * sprite.index;
		UVs[num] = sprite.lowerLeftUV + Vector2.up * sprite.uvDimensions.y;
		UVs[num + 1] = sprite.lowerLeftUV;
		UVs[num + 2] = sprite.lowerLeftUV + Vector2.right * sprite.uvDimensions.x;
		UVs[num + 3] = sprite.lowerLeftUV + sprite.uvDimensions;
		uvsChanged = true;
	}

	public void UpdateColors(MFGuiSprite sprite)
	{
		int num = 4 * sprite.index;
		colors[num] = sprite.m_Color;
		colors[num + 1] = sprite.m_Color;
		colors[num + 2] = sprite.m_Color;
		colors[num + 3] = sprite.m_Color;
		colorsChanged = true;
	}

	protected virtual void LateUpdate()
	{
		if (!vertsChanged && !colorsChanged && !uvsChanged)
		{
			return;
		}
		if (!vertsChanged)
		{
			if (uvsChanged)
			{
				mesh.uv = UVs;
			}
			if (colorsChanged)
			{
				mesh.colors = colors;
			}
		}
		else
		{
			mesh.Clear();
			mesh.vertices = vertices;
			mesh.uv = UVs;
			mesh.colors = colors;
			mesh.triangles = triIndices;
		}
		colorsChanged = false;
		vertsChanged = false;
		uvsChanged = false;
	}

	public MFGuiSprite AddElement(Vector2 leftDown, float width, float height, float rotAngle, float depth, int leftPixelX, int bottomPixelY, int pixelWidth, int pixelHeight)
	{
		return AddElement(leftDown, width, height, rotAngle, depth, PixelCoordToUVCoord(leftPixelX, bottomPixelY), PixelSpaceToUVSpace(pixelWidth, pixelHeight));
	}

	public MFGuiSprite AddElement(Vector2 leftDown, float width, float height, float rotAngle, float depth, Vector2 lowerLeftUV, Vector2 uvSize)
	{
		UpdateUISize();
		float x = leftDown.x + _xOffset + width / 2f;
		float y = (float)ZeroLocation * (0f - leftDown.y + _yOffset - height / 2f);
		Matrix4x4 inoutMatrix = Matrix4x4.identity;
		Mathfx.Matrix_SetPos(ref inoutMatrix, new Vector3(x, y, depth));
		return AddSprite(null, inoutMatrix, width, height, rotAngle, depth, lowerLeftUV, uvSize);
	}

	public void UpdateSpritePosSize(MFGuiSprite sprite, float rx, float ry, float width, float height, float rotAngle, float depth)
	{
		Vector3 vector = default(Vector3);
		vector = Mathfx.Matrix_GetEulerAngles(sprite.matrix);
		vector.z = rotAngle * ((float)Math.PI / 180f);
		Mathfx.Matrix_SetEulerAngles(ref sprite.matrix, vector);
		sprite.SetSizeXY(width, height, depth);
		Mathfx.Matrix_SetPos(inPos: new Vector3(z: Mathfx.Matrix_GetPos(sprite.matrix).z, x: rx - width / 2f + _xOffset + sprite.width / 2f, y: (float)ZeroLocation * (0f - (ry - height / 2f) + _yOffset - sprite.height / 2f)), inoutMatrix: ref sprite.matrix);
		Transform(sprite);
	}

	[Conditional("DEBUG_GUI_RENDERER")]
	public static void LogFuncCall(string funcName, string rendererId)
	{
	}
}
