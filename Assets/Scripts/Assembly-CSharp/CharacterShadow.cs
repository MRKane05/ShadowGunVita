using System.Collections.Generic;
using UnityEngine;

public class CharacterShadow : MonoBehaviour
{
	private struct S_CurrAnimInfo
	{
		public string m_AnimId;

		public float m_NormTime;
	}

	private struct S_AnimIndexRec
	{
		public int m_StartFrameInAtlas;

		public int m_NumFrames;
	}

	public Material m_Material;

	public Mesh m_ShadowPlaneMesh;

	public float m_GroundOffset = 0.05f;

	public int m_ShadowTexFPS = 7;

	public int m_ShadowTexNumTiles = 32;

	public float m_PelvisAOSphereRadius = 0.4f;

	public float m_FootAOSphereRadius = 0.4f;

	private float m_OrthoHalfExt = 1f;

	private float m_AOPlaneScale = 1f;

	private MeshFilter m_MeshFilter;

	private MeshRenderer m_MeshRenderer;

	private GameObject m_GameObject;

	private bool m_InitOK;

	private Transform m_PelvisTransform;

	private Transform m_LFootTransform;

	private Transform m_RFootTransform;

	private Dictionary<string, S_AnimIndexRec> m_AnimInfo = new Dictionary<string, S_AnimIndexRec>();

	private void Awake()
	{
		m_InitOK = DoInit();
	}

	private bool DoInit()
	{
		m_GameObject = new GameObject("ShadowPlaneGameObject");
		m_GameObject.AddComponent<MeshFilter>();
		m_GameObject.AddComponent<MeshRenderer>();
		m_GameObject.transform.parent = base.transform;
		m_GameObject.transform.localEulerAngles = new Vector3(-90f, 0f, 180f);
		m_GameObject.transform.localPosition = new Vector3(0f, m_GroundOffset, 0f);
		m_MeshFilter = (MeshFilter)m_GameObject.GetComponent(typeof(MeshFilter));
		m_MeshRenderer = (MeshRenderer)m_GameObject.GetComponent(typeof(MeshRenderer));
		m_MeshFilter.name = "characterShadowPlane";
		m_MeshFilter.mesh = m_ShadowPlaneMesh;
		m_MeshRenderer.material = m_Material;
		m_PelvisTransform = base.transform.Find("pelvis");
		m_LFootTransform = base.transform.Find("pelvis/Lthigh/Lcalf/Lfoot");
		m_RFootTransform = base.transform.Find("pelvis/Rthigh/Rcalf/Rfoot");
		if (!m_PelvisTransform || !m_RFootTransform || !m_LFootTransform)
		{
			return false;
		}
		return true;
	}

	private void LateUpdate()
	{
		if (m_InitOK)
		{
			UpdateAOBasedShadow();
		}
	}

	private void UpdateTextureBasedShadow()
	{
		S_CurrAnimInfo currAnimInfo = GetCurrAnimInfo();
		Material material = m_MeshRenderer.material;
		Vector3 position = m_PelvisTransform.position;
		position.y = m_GameObject.transform.position.y;
		Vector3 localScale = default(Vector3);
		localScale.x = m_OrthoHalfExt / m_MeshFilter.mesh.bounds.extents[0];
		localScale.y = 1f;
		localScale.z = m_OrthoHalfExt / m_MeshFilter.mesh.bounds.extents[1];
		m_GameObject.transform.position = position;
		m_GameObject.transform.localScale = localScale;
		if ((bool)material)
		{
			Vector4 vector = default(Vector4);
			vector.x = m_ShadowTexNumTiles - 1;
			vector.y = m_ShadowTexNumTiles - 1;
			vector.z = 0f;
			vector.w = 0f;
			Vector4 vector2 = default(Vector4);
			vector2.x = m_ShadowTexNumTiles;
			vector2.y = m_ShadowTexNumTiles;
			vector2.z = 0f;
			vector2.w = 0f;
			S_AnimIndexRec value;
			if (currAnimInfo.m_AnimId.Length > 0 && m_AnimInfo.TryGetValue(currAnimInfo.m_AnimId, out value))
			{
				int numFrames = value.m_NumFrames;
				float num = currAnimInfo.m_NormTime * (float)numFrames;
				int num2 = Mathf.FloorToInt(num);
				float z = num - (float)num2;
				int num3 = value.m_StartFrameInAtlas + num2;
				int num4 = value.m_StartFrameInAtlas + (num2 + 1) % numFrames;
				vector.x = num3 % m_ShadowTexNumTiles;
				vector.y = num3 / m_ShadowTexNumTiles;
				vector.z = num4 % m_ShadowTexNumTiles;
				vector.w = num4 / m_ShadowTexNumTiles;
				vector2.z = z;
			}
			material.SetVector("_NumTexTilesAndLerpInfo", vector2);
			material.SetVector("_SrcTileDstTile", vector);
		}
	}

	private void UpdateAOBasedShadow()
	{
		Material material = m_MeshRenderer.material;
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		Vector3 zero3 = Vector3.zero;
		zero = m_PelvisTransform.position;
		zero2 = m_LFootTransform.position;
		zero3 = m_RFootTransform.position;
		Vector3 one = Vector3.one;
		one.x = (one.y = m_AOPlaneScale);
		m_GameObject.transform.localScale = one;
		if ((bool)material)
		{
			Vector4 vector = default(Vector4);
			vector.x = zero.x;
			vector.y = zero.y + m_GroundOffset;
			vector.z = zero.z;
			vector.w = m_PelvisAOSphereRadius;
			Vector4 vector2 = default(Vector4);
			vector2.x = zero2.x;
			vector2.y = zero2.y + m_FootAOSphereRadius * 0.8f;
			vector2.z = zero2.z;
			vector2.w = m_FootAOSphereRadius;
			Vector4 vector3 = default(Vector4);
			vector3.x = zero3.x;
			vector3.y = zero3.y + m_FootAOSphereRadius * 0.8f;
			vector3.z = zero3.z;
			vector3.w = m_FootAOSphereRadius;
			material.SetVector("_Sphere0", vector);
			material.SetVector("_Sphere1", vector2);
			material.SetVector("_Sphere2", vector3);
		}
	}

	private S_CurrAnimInfo GetCurrAnimInfo()
	{
		Animation animation = base.gameObject.GetComponent("Animation") as Animation;
		S_CurrAnimInfo result = default(S_CurrAnimInfo);
		result.m_AnimId = string.Empty;
		result.m_NormTime = 0f;
		if ((bool)animation)
		{
			float num = 0f;
			foreach (AnimationState item in animation)
			{
				if (item.enabled && item.weight > 0f && item.weight > num)
				{
					result.m_AnimId = item.name;
					result.m_NormTime = item.normalizedTime - Mathf.Floor(item.normalizedTime);
					num = item.weight;
				}
			}
		}
		else
		{
			Debug.LogError(base.name + " No anim");
		}
		return result;
	}

	private bool InitAnimIndex()
	{
		Animation animation = base.gameObject.GetComponent("Animation") as Animation;
		if ((bool)animation)
		{
			int num = 0;
			S_AnimIndexRec value = default(S_AnimIndexRec);
			foreach (AnimationState item in animation)
			{
				int num2 = Mathf.CeilToInt(item.clip.length * (float)m_ShadowTexFPS);
				value.m_StartFrameInAtlas = num;
				value.m_NumFrames = num2;
				num += num2;
				m_AnimInfo[item.name] = value;
			}
			return true;
		}
		return true;
	}
}
