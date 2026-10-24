#define DEBUG
using UnityEngine;

public class LightningFX : MonoBehaviour
{
	private struct S_EmitPlaneInfo
	{
		public Vector3 m_Base;

		public Vector3 m_AxisU;

		public Vector3 m_AxisV;
	}

	private const float m_MinLineLength = 0.5f;

	private const int m_MaxLinePtSearchIters = 32;

	private const int m_MaxLineSegments = 32;

	public Vector3 m_Extents = new Vector3(3f, 3f, 3f);

	public bool m_EmitPlaneX = true;

	public bool m_EmitPlaneY;

	public bool m_EmitPlaneZ;

	public bool m_EmitParallelLines = true;

	public int m_MaxLines = 20;

	public Vector4 m_NoiseAmplitudes = new Vector4(2f, 1f, 0.5f, 0.125f);

	public Vector4 m_NoiseFrequencies = new Vector4(4f, 8f, 16f, 32f);

	public Vector4 m_NoiseSpeeds = new Vector4(3.2f, 2.3f, 0.5f, 1f);

	public Vector2 m_Amplitude = new Vector2(0.2f, 0.01f);

	public Vector2 m_DurationOnOff = new Vector2(0.1f, 2f);

	public float m_LinesWidth = 0.2f;

	public float m_InvWaveSpeed = 0.025f;

	public Color m_Color = new Color(0.4f, 0.49803922f, 0.99215686f, 1f);

	private MeshFilter m_MeshFilter;

	private MeshRenderer m_MeshRenderer;

	private Mesh m_Mesh;

	private Material m_Material;

	private S_EmitPlaneInfo[] m_EmitPlanes = new S_EmitPlaneInfo[6];

	private int m_NumEmitPlanes;

	private int m_NumEmitAxes;

	private void Awake()
	{
		m_Material = Resources.Load("effects/m_lightning_bolt", typeof(Material)) as Material;
		if ((bool)m_Material)
		{
			m_Material = Object.Instantiate(m_Material) as Material;
		}
		if (!m_Material)
		{
			Debug.LogError("Cannot load lighting bolt material");
		}
		DebugUtils.Assert(m_Material);
		InitMeshes();
		Generate();
		if ((bool)m_Material)
		{
			SetMaterialParams();
		}
	}

	private bool InitMeshes()
	{
		base.gameObject.AddComponent<MeshFilter>();
		base.gameObject.AddComponent<MeshRenderer>();
		m_MeshFilter = (MeshFilter)GetComponent(typeof(MeshFilter));
		m_MeshRenderer = (MeshRenderer)GetComponent(typeof(MeshRenderer));
		m_MeshRenderer.GetComponent<Renderer>().material = m_Material;
		m_MeshRenderer.GetComponent<Renderer>().enabled = true;
		m_MeshRenderer.GetComponent<Renderer>().castShadows = false;
		m_MeshRenderer.GetComponent<Renderer>().receiveShadows = false;
		m_Mesh = m_MeshFilter.mesh;
		return true;
	}

	private void InitInternalMeshBuffers(int numLines)
	{
		Vector3[] array = new Vector3[numLines * 32 * 4];
		Vector3[] array2 = new Vector3[numLines * 32 * 4];
		Vector2[] array3 = new Vector2[numLines * 32 * 4];
		Vector2[] array4 = new Vector2[numLines * 32 * 4];
		int[] array5 = new int[numLines * 32 * 2 * 3];
		float num = 1f / 32f;
		for (int i = 0; i < numLines; i++)
		{
			float x = i;
			for (int j = 0; j < 32; j++)
			{
				int num2 = (i * 32 + j) * 4;
				int num3 = (i * 32 + j) * 6;
				array3[num2].x = 0f;
				array3[num2].y = 1f;
				array3[num2 + 1].x = 0f;
				array3[num2 + 1].y = -1f;
				array3[num2 + 2].x = 0f;
				array3[num2 + 2].y = 1f;
				array3[num2 + 3].x = 0f;
				array3[num2 + 3].y = -1f;
				array4[num2].x = x;
				array4[num2].y = (float)j * num;
				array4[num2 + 1].x = x;
				array4[num2 + 1].y = (float)j * num;
				array4[num2 + 2].x = x;
				array4[num2 + 2].y = (float)(j + 1) * num;
				array4[num2 + 3].x = x;
				array4[num2 + 3].y = (float)(j + 1) * num;
				array5[num3] = num2;
				array5[num3 + 1] = num2 + 1;
				array5[num3 + 2] = num2 + 2;
				array5[num3 + 3] = num2 + 2;
				array5[num3 + 4] = num2 + 1;
				array5[num3 + 5] = num2 + 3;
			}
		}
		for (int k = 0; k < array.Length; k++)
		{
			array[k] = Vector3.zero;
			array2[k] = Vector3.zero;
		}
		m_Mesh.Clear();
		m_Mesh.vertices = array;
		m_Mesh.normals = array2;
		m_Mesh.uv = array3;
		m_Mesh.uv2 = array4;
		m_Mesh.triangles = array5;
	}

	private void SetupLineSegmentLinear(int idx, int startSeg, int numSegments, Vector3 startPt, Vector3 endPt)
	{
		Vector3[] vertices = m_Mesh.vertices;
		Vector3[] normals = m_Mesh.normals;
		Vector2[] uv = m_Mesh.uv;
		int num = idx * 32 * 4;
		int num2 = startSeg + numSegments;
		DebugUtils.Assert(idx < m_MaxLines);
		DebugUtils.Assert(numSegments > 0);
		for (int i = 0; i < startSeg; i++)
		{
			int num3 = num + i * 4;
			vertices[num3++] = startPt;
			vertices[num3++] = startPt;
			vertices[num3++] = startPt;
			vertices[num3++] = startPt;
		}
		Vector3 vector = (endPt - startPt) / numSegments;
		Vector3 vector2 = vector;
		Vector3 vector3 = startPt;
		Vector3 vector4 = startPt + vector;
		float num4 = 0f;
		for (int j = startSeg; j < num2; j++)
		{
			int num5 = num + j * 4;
			vertices[num5] = vector3;
			uv[num5].x = num4;
			normals[num5++] = vector2;
			vertices[num5] = vector3;
			uv[num5].x = num4;
			normals[num5++] = vector2;
			num4 += (vector4 - vector3).magnitude;
			vertices[num5] = vector4;
			uv[num5].x = num4;
			normals[num5++] = vector2;
			vertices[num5] = vector4;
			uv[num5].x = num4;
			normals[num5++] = vector2;
			vector3 = vector4;
			vector4 += vector;
		}
		for (int k = num2; k < 32; k++)
		{
			int num6 = num + k * 4;
			vertices[num6] = endPt;
		}
		m_Mesh.vertices = vertices;
		m_Mesh.normals = normals;
		m_Mesh.uv = uv;
	}

	private void SetupLineSegmentRadial(int idx, Vector3 center, float radius, float angle, Matrix4x4 m)
	{
		int num = 32;
		Vector3[] array = new Vector3[num + 1];
		float num2 = angle / (float)num;
		DebugUtils.Assert(idx < m_MaxLines);
		Vector4 vector = default(Vector4);
		for (int i = 0; i < array.Length; i++)
		{
			vector.x = radius * Mathf.Sin((float)i * num2);
			vector.y = 0f;
			vector.z = radius * Mathf.Cos((float)i * num2);
			vector.w = 1f;
			array[i] = m * vector;
		}
		Vector3[] vertices = m_Mesh.vertices;
		Vector3[] normals = m_Mesh.normals;
		Vector2[] uv = m_Mesh.uv;
		int num3 = idx * 32 * 4;
		float num4 = 0f;
		float num5 = 1f / (float)num;
		for (int j = 0; j < num; j++)
		{
			int num6 = num3 + j * 4;
			Vector3 vector2 = array[j + 1] - array[j];
			vertices[num6] = array[j];
			uv[num6].x = num4;
			normals[num6++] = vector2;
			vertices[num6] = array[j];
			uv[num6].x = num4;
			normals[num6++] = vector2;
			num4 += num5;
			vector2 = array[(j + 2) % array.Length] - array[j + 1];
			vertices[num6] = array[j + 1];
			uv[num6].x = num4;
			normals[num6++] = vector2;
			vertices[num6] = array[j + 1];
			uv[num6].x = num4;
			normals[num6++] = vector2;
		}
		m_Mesh.vertices = vertices;
		m_Mesh.normals = normals;
		m_Mesh.uv = uv;
	}

	private void InitEmitPlanes()
	{
		Vector3 vector = m_Extents * 0.5f;
		m_NumEmitPlanes = 0;
		m_NumEmitAxes = 0;
		if (m_EmitPlaneX)
		{
			m_EmitPlanes[m_NumEmitPlanes].m_Base = new Vector3(vector.x, 0f, 0f);
			m_EmitPlanes[m_NumEmitPlanes].m_AxisU = new Vector3(0f, 0f, 1f) * vector.z;
			m_EmitPlanes[m_NumEmitPlanes].m_AxisV = new Vector3(0f, 1f, 0f) * vector.y;
			m_NumEmitPlanes++;
			m_EmitPlanes[m_NumEmitPlanes].m_Base = new Vector3(0f - vector.x, 0f, 0f);
			m_EmitPlanes[m_NumEmitPlanes].m_AxisU = new Vector3(0f, 0f, 1f) * vector.z;
			m_EmitPlanes[m_NumEmitPlanes].m_AxisV = new Vector3(0f, 1f, 0f) * vector.y;
			m_NumEmitPlanes++;
			m_NumEmitAxes++;
		}
		if (m_EmitPlaneY)
		{
			m_EmitPlanes[m_NumEmitPlanes].m_Base = new Vector3(0f, vector.y, 0f);
			m_EmitPlanes[m_NumEmitPlanes].m_AxisU = new Vector3(1f, 0f, 0f) * vector.x;
			m_EmitPlanes[m_NumEmitPlanes].m_AxisV = new Vector3(0f, 0f, 1f) * vector.z;
			m_NumEmitPlanes++;
			m_EmitPlanes[m_NumEmitPlanes].m_Base = new Vector3(0f, 0f - vector.y, 0f);
			m_EmitPlanes[m_NumEmitPlanes].m_AxisU = new Vector3(1f, 0f, 0f) * vector.x;
			m_EmitPlanes[m_NumEmitPlanes].m_AxisV = new Vector3(0f, 0f, 1f) * vector.z;
			m_NumEmitPlanes++;
			m_NumEmitAxes++;
		}
		if (m_EmitPlaneZ)
		{
			m_EmitPlanes[m_NumEmitPlanes].m_Base = new Vector3(0f, 0f, vector.z);
			m_EmitPlanes[m_NumEmitPlanes].m_AxisU = new Vector3(1f, 0f, 0f) * vector.x;
			m_EmitPlanes[m_NumEmitPlanes].m_AxisV = new Vector3(0f, 1f, 0f) * vector.y;
			m_NumEmitPlanes++;
			m_EmitPlanes[m_NumEmitPlanes].m_Base = new Vector3(0f, 0f, 0f - vector.z);
			m_EmitPlanes[m_NumEmitPlanes].m_AxisU = new Vector3(1f, 0f, 0f) * vector.x;
			m_EmitPlanes[m_NumEmitPlanes].m_AxisV = new Vector3(0f, 1f, 0f) * vector.y;
			m_NumEmitPlanes++;
			m_NumEmitAxes++;
		}
	}

	private void GeneratePts(out Vector3 pt0, out Vector3 pt1)
	{
		if (m_NumEmitPlanes > 0)
		{
			int num = Random.Range(0, 9999) % m_NumEmitAxes * 2;
			float num2 = Random.Range(-1f, 1f);
			float num3 = Random.Range(-1f, 1f);
			pt0 = m_EmitPlanes[num].m_Base + m_EmitPlanes[num].m_AxisU * num2 + m_EmitPlanes[num].m_AxisV * num3;
			if (!m_EmitParallelLines)
			{
				num2 = Random.Range(-1f, 1f);
				num3 = Random.Range(-1f, 1f);
			}
			num++;
			pt1 = m_EmitPlanes[num].m_Base + m_EmitPlanes[num].m_AxisU * num2 + m_EmitPlanes[num].m_AxisV * num3;
		}
		else
		{
			pt0 = Vector3.zero;
			pt1 = Vector3.zero;
		}
	}

	private void Generate()
	{
		int maxLines = m_MaxLines;
		InitEmitPlanes();
		InitInternalMeshBuffers(maxLines);
		for (int i = 0; i < maxLines; i++)
		{
			Vector3 pt;
			Vector3 pt2;
			GeneratePts(out pt, out pt2);
			SetupLineSegmentLinear(i, 0, 32, pt, pt2);
		}
		m_Mesh.RecalculateBounds();
	}

	private void SetMaterialParams()
	{
		Vector4 zero = Vector4.zero;
		Vector4 zero2 = Vector4.zero;
		Vector4 zero3 = Vector4.zero;
		zero.x = m_DurationOnOff.x;
		zero.y = m_DurationOnOff.y;
		zero2.x = m_Amplitude.x;
		zero2.y = m_Amplitude.y;
		zero3.x = m_LinesWidth;
		zero3.y = m_InvWaveSpeed;
		zero3.z = (float)Screen.width / (float)Screen.height;
		m_Material.SetVector("_Duration", zero);
		m_Material.SetVector("_Amplitude", zero2);
		m_Material.SetVector("_NoiseFreqs", m_NoiseFrequencies);
		m_Material.SetVector("_NoiseSpeeds", m_NoiseSpeeds);
		m_Material.SetVector("_NoiseAmps", m_NoiseAmplitudes);
		m_Material.SetVector("_OtherParams", zero3);
		m_Material.SetColor("_Color", m_Color);
	}

	private void LateUpdate()
	{
		if ((bool)m_Material)
		{
			SetMaterialParams();
		}
	}

	private void OnDrawGizmos()
	{
		Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.green;
		Gizmos.matrix = localToWorldMatrix;
		Gizmos.DrawWireCube(Vector3.zero, m_Extents);
	}

	private void OnEnable()
	{
		if ((bool)m_MeshRenderer)
		{
			m_MeshRenderer.enabled = true;
		}
	}

	private void OnDisable()
	{
		if ((bool)m_MeshRenderer)
		{
			m_MeshRenderer.enabled = false;
		}
	}
}
