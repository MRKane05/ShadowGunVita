using System.Collections.Generic;
using UnityEngine;

// Alternative player skins.
//
// Characters built on the player's 24-bone skeleton (rooted at "pelvis", same bone names: the Mutant,
// Elite, Heavy, Shielder, Scientist and Sara) can be drawn on the player instead. The player keeps its
// own bones, animations, weapons, collision and scripts; only the skinned mesh and its material are
// borrowed. Each mesh lists its bones in its own order, so the bone array is rebuilt by name against
// the player's bones. Bones the player doesn't have (such as Sara's jaw) are added to the player's
// skeleton in their rest pose, worked out from the mesh's bind poses.
//
// The chosen skin is GuiOptions.playerSkin, applied by ComponentPlayer.Start.
public static class PlayerSkins
{
	public class Skin
	{
		public string Name;

		// Either: a prefab (Resources path) whose skinned mesh and material are borrowed...
		public string PrefabResource;

		// ...or a bare mesh and material (Resources paths) with the mesh's bone names in bind order.
		// Meshes don't store bone names, so these come from the renderer that used the mesh in a scene.
		public string MeshResource;

		public string MaterialResource;

		public string[] BoneNames;

		// Bones to add to the player if missing, as pairs: bone name, parent bone name
		public string[] ExtraBones;

		// true = the player's own model
		public bool IsDefault
		{
			get
			{
				return PrefabResource == null && MeshResource == null;
			}
		}

		public static Skin FromPrefab(string name, string prefabResource)
		{
			Skin skin = new Skin();
			skin.Name = name;
			skin.PrefabResource = prefabResource;
			return skin;
		}

		public static Skin FromMesh(string name, string meshResource, string materialResource, string[] boneNames, string[] extraBones)
		{
			Skin skin = new Skin();
			skin.Name = name;
			skin.MeshResource = meshResource;
			skin.MaterialResource = materialResource;
			skin.BoneNames = boneNames;
			skin.ExtraBones = extraBones;
			return skin;
		}
	}

	public static readonly Skin[] Skins = new Skin[]
	{
		Skin.FromPrefab("Default", null),
		Skin.FromPrefab("Mutant", "enemies/EnemyMutant"),
		Skin.FromPrefab("Shielder", "enemies/EnemyShielder"),
		Skin.FromPrefab("Elite", "enemies/EnemyElite"),
		Skin.FromPrefab("Heavy", "enemies/EnemyHeavy"),
		Skin.FromPrefab("Scientist", "enemies/EnemyScientist"),
		// Sara only exists in level_14_dlc's scene, so her mesh and material live in Resources/skins.
		// Bone order taken from her SkinnedMeshRenderer in level_14_dlc; face_jaw is extra.
		Skin.FromMesh("Sara", "skins/sara", "skins/sara_MAT", new string[25]
		{
			"pelvis", "chest", "Larm", "Lshoulder", "neck", "head", "Ribs", "stomach", "Lthigh", "Rarm",
			"Rshoulder", "Rthigh", "Lcalf", "Lfoot", "Lforearm", "Lthumb", "Lwrist", "Lfingers", "Rcalf", "Rfoot",
			"Rforearm", "Rthumb", "Rwrist", "Rfingers", "face_jaw"
		}, new string[2] { "face_jaw", "head" })
	};

	// The player's own model, captured before the first swap so it can be put back
	public class OriginalModel
	{
		public Mesh Mesh;

		public Material[] Materials;

		public Transform[] Bones;

		public Transform RootBone;

		public Bounds LocalBounds;

		public Material DiffuseMaterial;
	}

	public static int ClampIndex(int index)
	{
		if (index < 0 || index >= Skins.Length)
		{
			return 0;
		}
		return index;
	}

	// Draws the player with skin 'index'. 'original' should start as null and be kept by the caller;
	// it records the player's own model the first time so the default skin can be restored.
	// Returns false (and leaves the current model) if the skin can't be applied.
	public static bool Apply(AgentHuman player, ref OriginalModel original, int index)
	{
		if (player == null)
		{
			return false;
		}
		SkinnedMeshRenderer renderer = player.GetComponentInChildren(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
		if (renderer == null)
		{
			Debug.LogWarning("[PlayerSkin] Player has no SkinnedMeshRenderer");
			return false;
		}
		if (original == null)
		{
			original = new OriginalModel();
			original.Mesh = renderer.sharedMesh;
			original.Materials = renderer.sharedMaterials;
			original.Bones = renderer.bones;
			original.RootBone = renderer.rootBone;
			original.LocalBounds = renderer.localBounds;
			original.DiffuseMaterial = player.DiffuseMaterial;
		}

		Skin skin = Skins[ClampIndex(index)];
		if (skin.IsDefault)
		{
			renderer.sharedMesh = original.Mesh;
			renderer.bones = original.Bones;
			renderer.rootBone = original.RootBone;
			renderer.localBounds = original.LocalBounds;
			renderer.sharedMaterials = original.Materials;
			player.DiffuseMaterial = original.DiffuseMaterial;
			return true;
		}

		Mesh mesh;
		Material material;
		string[] boneNames;
		string rootBoneName;
		Bounds localBounds;
		if (!LoadSkin(skin, out mesh, out material, out boneNames, out rootBoneName, out localBounds))
		{
			return false;
		}
		Matrix4x4[] bindPoses = mesh.bindposes;
		if (bindPoses.Length != boneNames.Length)
		{
			Debug.LogWarning("[PlayerSkin] " + skin.Name + ": mesh has " + bindPoses.Length + " bind poses but " + boneNames.Length + " bone names");
			return false;
		}

		// The player's bones by name (first match wins; bones come before weapon parts in the hierarchy)
		Dictionary<string, Transform> playerBones = new Dictionary<string, Transform>();
		Component[] transforms = player.GetComponentsInChildren(typeof(Transform), true);
		for (int i = 0; i < transforms.Length; i++)
		{
			if (!playerBones.ContainsKey(transforms[i].name))
			{
				playerBones.Add(transforms[i].name, transforms[i] as Transform);
			}
		}

		// Add any extra bones the skin needs (left in place afterwards; unused bones cost nothing)
		if (skin.ExtraBones != null)
		{
			for (int i = 0; i + 1 < skin.ExtraBones.Length; i += 2)
			{
				if (!AddExtraBone(skin, skin.ExtraBones[i], skin.ExtraBones[i + 1], boneNames, bindPoses, playerBones))
				{
					return false;
				}
			}
		}

		// The mesh's bones, in its own order, mapped onto the player's skeleton
		Transform[] bones = new Transform[boneNames.Length];
		for (int i = 0; i < boneNames.Length; i++)
		{
			if (!playerBones.TryGetValue(boneNames[i], out bones[i]))
			{
				Debug.LogWarning("[PlayerSkin] " + skin.Name + ": player has no bone named " + boneNames[i]);
				return false;
			}
		}
		Transform rootBone;
		if (rootBoneName == null || !playerBones.TryGetValue(rootBoneName, out rootBone))
		{
			rootBone = original.RootBone;
		}

		renderer.sharedMesh = mesh;
		renderer.bones = bones;
		renderer.rootBone = rootBone;
		renderer.localBounds = localBounds;
		renderer.sharedMaterials = new Material[1] { material };
		// The player's spawn fade-in and ResetMaterial switch back to DiffuseMaterial
		player.DiffuseMaterial = material;
		return true;
	}

	private static bool LoadSkin(Skin skin, out Mesh mesh, out Material material, out string[] boneNames, out string rootBoneName, out Bounds localBounds)
	{
		mesh = null;
		material = null;
		boneNames = null;
		rootBoneName = null;
		localBounds = new Bounds(Vector3.zero, Vector3.one);

		if (skin.PrefabResource != null)
		{
			GameObject prefab = Resources.Load(skin.PrefabResource, typeof(GameObject)) as GameObject;
			if (prefab == null)
			{
				Debug.LogWarning("[PlayerSkin] Can't load " + skin.PrefabResource);
				return false;
			}
			SkinnedMeshRenderer source = prefab.GetComponentInChildren(typeof(SkinnedMeshRenderer), true) as SkinnedMeshRenderer;
			if (source == null || source.sharedMesh == null)
			{
				Debug.LogWarning("[PlayerSkin] " + skin.PrefabResource + " has no skinned mesh");
				return false;
			}
			Transform[] sourceBones = source.bones;
			boneNames = new string[sourceBones.Length];
			for (int i = 0; i < sourceBones.Length; i++)
			{
				if (sourceBones[i] == null)
				{
					Debug.LogWarning("[PlayerSkin] " + skin.PrefabResource + " has a missing bone");
					return false;
				}
				boneNames[i] = sourceBones[i].name;
			}
			mesh = source.sharedMesh;
			rootBoneName = (source.rootBone != null) ? source.rootBone.name : null;
			localBounds = source.localBounds;

			// Enemies spawn with a different renderer material (the Mutant's is a glowing-eyes spawn
			// variant) and switch to their DiffuseMaterial afterwards, so use that as the normal look.
			AgentHuman sourceAgent = prefab.GetComponent<AgentHuman>();
			material = (sourceAgent != null && sourceAgent.DiffuseMaterial != null) ? sourceAgent.DiffuseMaterial : source.sharedMaterial;
			return true;
		}

		mesh = Resources.Load(skin.MeshResource, typeof(Mesh)) as Mesh;
		material = Resources.Load(skin.MaterialResource, typeof(Material)) as Material;
		if (mesh == null || material == null)
		{
			Debug.LogWarning("[PlayerSkin] Can't load " + skin.MeshResource + " / " + skin.MaterialResource);
			return false;
		}
		boneNames = skin.BoneNames;
		rootBoneName = "pelvis";
		// Generous box around the pelvis so the character is never culled while on screen
		localBounds = new Bounds(Vector3.zero, new Vector3(2.5f, 2.5f, 2.5f));
		return true;
	}

	// Adds 'boneName' under the player's 'parentName' bone, in the rest pose the mesh was bound with
	private static bool AddExtraBone(Skin skin, string boneName, string parentName, string[] boneNames, Matrix4x4[] bindPoses, Dictionary<string, Transform> playerBones)
	{
		if (playerBones.ContainsKey(boneName))
		{
			return true;
		}
		Transform parent;
		int boneIndex = System.Array.IndexOf(boneNames, boneName);
		int parentIndex = System.Array.IndexOf(boneNames, parentName);
		if (boneIndex < 0 || parentIndex < 0 || !playerBones.TryGetValue(parentName, out parent))
		{
			Debug.LogWarning("[PlayerSkin] " + skin.Name + ": can't add bone " + boneName + " under " + parentName);
			return false;
		}
		// Bind poses are each bone's world-to-local matrix at bind time, so the bone's local
		// transform relative to its parent is parentBindPose * inverse(boneBindPose)
		Matrix4x4 local = bindPoses[parentIndex] * bindPoses[boneIndex].inverse;
		GameObject go = new GameObject(boneName);
		Transform bone = go.transform;
		bone.parent = parent;
		bone.localPosition = local.GetColumn(3);
		bone.localRotation = Quaternion.LookRotation(local.GetColumn(2), local.GetColumn(1));
		bone.localScale = Vector3.one;
		playerBones.Add(boneName, bone);
		return true;
	}
}
