using System.Collections;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
	public enum SPRITE_PLANE
	{
		XY = 0,
		XZ = 1,
		YZ = 2
	}

	public enum WINDING_ORDER
	{
		CCW = 0,
		CW = 1
	}

	public Material material;

	public int allocBlockSize = 16;

	public SPRITE_PLANE plane;

	public WINDING_ORDER winding;

	public bool autoUpdateBounds;

	protected ArrayList availableBlocks = new ArrayList();

	protected bool vertsChanged;

	protected bool uvsChanged;

	protected bool colorsChanged;

	protected bool vertCountChanged;

	protected bool updateBounds;

	protected Sprite[] sprites;

	protected ArrayList activeBlocks = new ArrayList();

	protected ArrayList activeBillboards = new ArrayList();

	protected float boundUpdateInterval;

	protected MeshFilter meshFilter;

	protected MeshRenderer meshRenderer;

	protected Mesh mesh;

	protected Vector3[] vertices;

	protected int[] triIndices;

	protected Vector2[] UVs;

	protected Color[] colors;

	public Vector2 PixelSpaceToUVSpace(Vector2 xy)
	{
		Texture texture = material.GetTexture("_MainTex");
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
		material = mat;
		meshRenderer.GetComponent<Renderer>().material = material;
	}

	protected virtual void Awake()
	{
		base.gameObject.AddComponent<MeshFilter>();
		base.gameObject.AddComponent<MeshRenderer>();
		meshFilter = (MeshFilter)GetComponent(typeof(MeshFilter));
		meshRenderer = (MeshRenderer)GetComponent(typeof(MeshRenderer));
		meshRenderer.GetComponent<Renderer>().material = material;
		mesh = meshFilter.mesh;
		EnlargeArrays(allocBlockSize);
		base.transform.position = Vector3.zero;
		base.transform.rotation = Quaternion.identity;
	}

	protected void InitArrays()
	{
		sprites = new Sprite[1];
		GameObject gameObject = new GameObject("UiTemp");
		sprites[0] = (Sprite)gameObject.AddComponent(typeof(Sprite));
		sprites[0].dummyGO = gameObject;
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
		Sprite[] array = sprites;
		sprites = new Sprite[sprites.Length + count];
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
			GameObject gameObject = new GameObject("UiTemp");
			sprites[j] = (Sprite)gameObject.AddComponent(typeof(Sprite));
			sprites[j].dummyGO = gameObject;
			sprites[j].index = j;
			sprites[j].manager = this;
			sprites[j].SetBuffers(vertices, UVs);
			sprites[j].mv1 = j * 4;
			sprites[j].mv2 = j * 4 + 1;
			sprites[j].mv3 = j * 4 + 2;
			sprites[j].mv4 = j * 4 + 3;
			sprites[j].uv1 = j * 4;
			sprites[j].uv2 = j * 4 + 1;
			sprites[j].uv3 = j * 4 + 2;
			sprites[j].uv4 = j * 4 + 3;
			sprites[j].cv1 = j * 4;
			sprites[j].cv2 = j * 4 + 1;
			sprites[j].cv3 = j * 4 + 2;
			sprites[j].cv4 = j * 4 + 3;
			sprites[j].SetColor(Color.white);
			availableBlocks.Add(sprites[j]);
			if (winding == WINDING_ORDER.CCW)
			{
				triIndices[j * 6] = j * 4;
				triIndices[j * 6 + 1] = j * 4 + 1;
				triIndices[j * 6 + 2] = j * 4 + 3;
				triIndices[j * 6 + 3] = j * 4 + 3;
				triIndices[j * 6 + 4] = j * 4 + 1;
				triIndices[j * 6 + 5] = j * 4 + 2;
			}
			else
			{
				triIndices[j * 6] = j * 4;
				triIndices[j * 6 + 1] = j * 4 + 3;
				triIndices[j * 6 + 2] = j * 4 + 1;
				triIndices[j * 6 + 3] = j * 4 + 3;
				triIndices[j * 6 + 4] = j * 4 + 2;
				triIndices[j * 6 + 5] = j * 4 + 1;
			}
		}
		vertsChanged = true;
		uvsChanged = true;
		colorsChanged = true;
		vertCountChanged = true;
		return num;
	}

	public Sprite AddSprite(GameObject client, float width, float height, int leftPixelX, int bottomPixelY, int pixelWidth, int pixelHeight, bool billboarded)
	{
		return AddSprite(client, width, height, PixelCoordToUVCoord(leftPixelX, bottomPixelY), PixelSpaceToUVSpace(pixelWidth, pixelHeight), billboarded);
	}

	public Sprite AddSprite(GameObject client, float width, float height, Vector2 lowerLeftUV, Vector2 UVDimensions, bool billboarded)
	{
		if (availableBlocks.Count < 1)
		{
			EnlargeArrays(allocBlockSize);
		}
		int index = ((Sprite)availableBlocks[0]).index;
		availableBlocks.RemoveAt(0);
		Sprite sprite = sprites[index];
		sprite.client = client;
		sprite.lowerLeftUV = lowerLeftUV;
		sprite.uvDimensions = UVDimensions;
		switch (plane)
		{
		case SPRITE_PLANE.XY:
			sprite.SetSizeXY(width, height);
			break;
		case SPRITE_PLANE.XZ:
			sprite.SetSizeXZ(width, height);
			break;
		case SPRITE_PLANE.YZ:
			sprite.SetSizeYZ(width, height);
			break;
		default:
			sprite.SetSizeXY(width, height);
			break;
		}
		if (billboarded)
		{
			sprite.billboarded = true;
			activeBillboards.Add(sprite);
		}
		else
		{
			activeBlocks.Add(sprite);
		}
		sprite.Transform();
		UVs[sprite.uv1] = lowerLeftUV + Vector2.up * UVDimensions.y;
		UVs[sprite.uv2] = lowerLeftUV;
		UVs[sprite.uv3] = lowerLeftUV + Vector2.right * UVDimensions.x;
		UVs[sprite.uv4] = lowerLeftUV + UVDimensions;
		vertsChanged = true;
		uvsChanged = true;
		return sprite;
	}

	public void SetBillboarded(Sprite sprite)
	{
		activeBlocks.Remove(sprite);
		activeBillboards.Add(sprite);
	}

	public void RemoveSprite(Sprite sprite)
	{
		sprite.SetSizeXY(0f, 0f);
		sprite.v1 = Vector3.zero;
		sprite.v2 = Vector3.zero;
		sprite.v3 = Vector3.zero;
		sprite.v4 = Vector3.zero;
		vertices[sprite.mv1] = sprite.v1;
		vertices[sprite.mv2] = sprite.v2;
		vertices[sprite.mv3] = sprite.v3;
		vertices[sprite.mv4] = sprite.v4;
		if (sprite.billboarded)
		{
			activeBillboards.Remove(sprite);
		}
		else
		{
			activeBlocks.Remove(sprite);
		}
		sprite.client = null;
		sprite.billboarded = false;
		sprite.hidden = false;
		sprite.SetColor(Color.white);
		sprite.offset = Vector3.zero;
		availableBlocks.Add(sprite);
		vertsChanged = true;
	}

	public void HideSprite(Sprite sprite)
	{
		if (sprite.billboarded)
		{
			activeBillboards.Remove(sprite);
		}
		else
		{
			activeBlocks.Remove(sprite);
		}
		sprite.m_hidden___DoNotAccessExternally = true;
		vertices[sprite.mv1] = Vector3.zero;
		vertices[sprite.mv2] = Vector3.zero;
		vertices[sprite.mv3] = Vector3.zero;
		vertices[sprite.mv4] = Vector3.zero;
		vertsChanged = true;
	}

	public void ShowSprite(Sprite sprite)
	{
		if (!(sprite.client == null) && sprite.m_hidden___DoNotAccessExternally)
		{
			sprite.m_hidden___DoNotAccessExternally = false;
			sprite.Transform();
			if (sprite.billboarded)
			{
				activeBillboards.Add(sprite);
			}
			else
			{
				activeBlocks.Add(sprite);
			}
			vertsChanged = true;
		}
	}

	public Sprite GetSprite(int i)
	{
		if (i < sprites.Length)
		{
			return sprites[i];
		}
		return null;
	}

	public void Transform(Sprite sprite)
	{
		sprite.Transform();
		vertsChanged = true;
	}

	public void TransformToGround(Sprite sprite)
	{
		sprite.TransformToGround();
		vertsChanged = true;
	}

	public void TransformBillboarded(Sprite sprite)
	{
		Vector3 position = sprite.clientTransform.position;
		Transform transform = Camera.main.transform;
		vertices[sprite.mv1] = position + transform.TransformDirection(sprite.v1);
		vertices[sprite.mv2] = position + transform.TransformDirection(sprite.v2);
		vertices[sprite.mv3] = position + transform.TransformDirection(sprite.v3);
		vertices[sprite.mv4] = position + transform.TransformDirection(sprite.v4);
		vertsChanged = true;
	}

	public void UpdatePositions()
	{
		vertsChanged = true;
	}

	public void UpdateUV(Sprite sprite)
	{
		UVs[sprite.uv1] = sprite.lowerLeftUV + Vector2.up * sprite.uvDimensions.y;
		UVs[sprite.uv2] = sprite.lowerLeftUV;
		UVs[sprite.uv3] = sprite.lowerLeftUV + Vector2.right * sprite.uvDimensions.x;
		UVs[sprite.uv4] = sprite.lowerLeftUV + sprite.uvDimensions;
		uvsChanged = true;
	}

	public void UpdateColors(Sprite sprite)
	{
		colors[sprite.cv1] = sprite.color;
		colors[sprite.cv2] = sprite.color;
		colors[sprite.cv3] = sprite.color;
		colors[sprite.cv4] = sprite.color;
		colorsChanged = true;
	}

	public void UpdateBounds()
	{
		updateBounds = true;
	}

	public void ScheduleBoundsUpdate(float seconds)
	{
		boundUpdateInterval = seconds;
		InvokeRepeating("UpdateBounds", seconds, seconds);
	}

	public void CancelBoundsUpdate()
	{
		CancelInvoke("UpdateBounds");
	}

	private void Start()
	{
	}

	protected virtual void Update()
	{
		int count = activeBillboards.Count;
		for (int i = 0; i < count; i++)
		{
			TransformBillboarded(activeBillboards[i] as Sprite);
		}
	}

	protected virtual void LateUpdate()
	{
		if (vertCountChanged)
		{
			vertCountChanged = false;
			colorsChanged = false;
			vertsChanged = false;
			uvsChanged = false;
			updateBounds = false;
			mesh.Clear();
			mesh.vertices = vertices;
			mesh.uv = UVs;
			mesh.colors = colors;
			mesh.triangles = triIndices;
			return;
		}
		if (vertsChanged)
		{
			vertsChanged = false;
			if (autoUpdateBounds)
			{
				updateBounds = true;
			}
			mesh.vertices = vertices;
		}
		if (updateBounds)
		{
			mesh.RecalculateBounds();
			updateBounds = false;
		}
		if (colorsChanged)
		{
			colorsChanged = false;
			mesh.colors = colors;
		}
		if (uvsChanged)
		{
			uvsChanged = false;
			mesh.uv = UVs;
		}
	}
}
