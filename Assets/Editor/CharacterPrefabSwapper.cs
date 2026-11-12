// Editor tool: replace selected scene objects with a prefab.
//  - Copies the Animation component settings (default clip and clip list) from the old object.
//  - Updates references elsewhere in the open scene that pointed at the old object, its children
//    or its components, so cutscene controllers and other scripts point at the new character.
//
// Place this file in an Editor folder (e.g. Assets/Editor/CharacterPrefabSwapper.cs).
// Open it from: Tools > Character Swap > Replace Selection With Prefab...
//
// Unity 2018 / C# 6.

using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterPrefabSwapper : EditorWindow
{
    private GameObject m_Prefab;
    private bool m_KeepName = true;
    private bool m_MoveChildren = false;
    private bool m_UpdateReferences = true;

    // Serialized Animation component properties copied from the old object.
    // Missing names are logged and skipped, so this list is safe if a version differs.
    private static readonly string[] s_AnimationProperties =
    {
        "m_Animation",          // default clip
        "m_Animations",         // the clip list shown in the Inspector
        "m_WrapMode",
        "m_PlayAutomatically",
        "m_AnimatePhysics",
        "m_CullingType"
    };

    [MenuItem("Tools/Character Swap/Replace Selection With Prefab...")]
    public static void ShowWindow()
    {
        GetWindow<CharacterPrefabSwapper>("Prefab Swap");
    }

    private void OnGUI()
    {
        m_Prefab = (GameObject)EditorGUILayout.ObjectField("Replacement prefab", m_Prefab, typeof(GameObject), false);
        m_KeepName = EditorGUILayout.Toggle("Keep object name", m_KeepName);
        m_MoveChildren = EditorGUILayout.Toggle("Move children across", m_MoveChildren);
        m_UpdateReferences = EditorGUILayout.Toggle("Update scene references", m_UpdateReferences);

        List<GameObject> targets = GetSceneTargets();

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "Select one or more scene objects (not prefab assets). Each is replaced with an instance of the prefab. " +
            "The Animation clip list is copied from the old object, and scene references to it are redirected " +
            "to the new object where a matching object exists. Only the open scene is searched.",
            MessageType.Info);

        using (new EditorGUI.DisabledScope(m_Prefab == null || targets.Count == 0))
        {
            if (GUILayout.Button("Replace " + targets.Count + " selected object(s)"))
            {
                ReplaceAll(targets, m_Prefab);
            }
        }
    }

    // Only objects that live in an open scene can be replaced.
    private static List<GameObject> GetSceneTargets()
    {
        var result = new List<GameObject>();
        foreach (GameObject go in Selection.gameObjects)
        {
            if (go != null && go.scene.IsValid() && !EditorUtility.IsPersistent(go))
            {
                result.Add(go);
            }
        }
        return result;
    }

    private void ReplaceAll(List<GameObject> targets, GameObject prefab)
    {
        var replacements = new List<GameObject>();
        foreach (GameObject old in targets)
        {
            GameObject replacement = ReplaceOne(old, prefab);
            if (replacement != null)
            {
                replacements.Add(replacement);
            }
        }

        // Select the new objects so the user can check them straight away
        Selection.objects = replacements.ToArray();
    }

    private GameObject ReplaceOne(GameObject old, GameObject prefab)
    {
        Scene scene = old.scene;
        Transform parent = old.transform.parent;
        int siblingIndex = old.transform.GetSiblingIndex();
        string oldName = old.name; // read now: the old object is destroyed below

        // Instantiate as a prefab instance so the link to the prefab (or variant) is kept
        GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
        if (newObj == null)
        {
            Debug.LogError("Could not instantiate " + prefab.name + " for " + old.name, old);
            return null;
        }
        Undo.RegisterCreatedObjectUndo(newObj, "Replace with prefab");

        // Same place in the hierarchy and the same local transform
        Undo.SetTransformParent(newObj.transform, parent, "Replace with prefab");
        newObj.transform.localPosition = old.transform.localPosition;
        newObj.transform.localRotation = old.transform.localRotation;
        newObj.transform.localScale = old.transform.localScale;
        newObj.transform.SetSiblingIndex(siblingIndex);

        if (m_KeepName)
        {
            newObj.name = old.name;
        }

        // Work out which old object corresponds to which new object, before anything is moved or destroyed
        var map = new Dictionary<Object, Object>();
        BuildMap(old, newObj, map);

        // Animation: copy the clip list and settings from the old object
        Animation oldAnim = old.GetComponent<Animation>();
        if (oldAnim != null)
        {
            Animation newAnim = newObj.GetComponent<Animation>();
            if (newAnim == null)
            {
                newAnim = Undo.AddComponent<Animation>(newObj);
            }
            CopyAnimation(oldAnim, newAnim);
        }
        else
        {
            Debug.LogWarning(old.name + " has no Animation component, so its animation list was not copied.", old);
        }

        // Children: move them across if asked, otherwise they are destroyed with the old object
        HandleChildren(old, newObj);

        // Redirect references elsewhere in the scene before the old object is destroyed
        int updated = 0;
        if (m_UpdateReferences)
        {
            updated = RemapSceneReferences(scene, old, map);
        }

        Undo.DestroyObjectImmediate(old);
        EditorSceneManager.MarkSceneDirty(scene);

        Debug.Log("Replaced " + oldName + " with " + prefab.name + ". Updated " + updated + " scene reference(s).", newObj);
        return newObj;
    }

    // ---- Mapping -------------------------------------------------------------------

    // Maps each object on the old hierarchy to its counterpart on the replacement.
    // GameObjects and Transforms match by relative path; components match by type and order on that object.
    private static void BuildMap(GameObject old, GameObject replacement, Dictionary<Object, Object> map)
    {
        map[old] = replacement;

        foreach (Transform oldT in old.GetComponentsInChildren<Transform>(true))
        {
            string path = RelativePath(old.transform, oldT);
            Transform newT = path.Length == 0 ? replacement.transform : replacement.transform.Find(path);
            if (newT == null)
            {
                continue;
            }

            map[oldT.gameObject] = newT.gameObject;

            foreach (Component oldComp in oldT.GetComponents<Component>())
            {
                int index = IndexOfComponent(oldT, oldComp);
                Component[] candidates = newT.GetComponents(oldComp.GetType());
                if (index >= 0 && index < candidates.Length)
                {
                    map[oldComp] = candidates[index];
                }
            }
        }
    }

    private static string RelativePath(Transform root, Transform t)
    {
        if (t == root)
        {
            return "";
        }

        string path = t.name;
        Transform p = t.parent;
        while (p != null && p != root)
        {
            path = p.name + "/" + path;
            p = p.parent;
        }
        return path;
    }

    private static int IndexOfComponent(Transform owner, Component comp)
    {
        int index = 0;
        foreach (Component c in owner.GetComponents(comp.GetType()))
        {
            if (c == comp)
            {
                return index;
            }
            index++;
        }
        return -1;
    }

    // ---- Reference updating -----------------------------------------------------------

    // Walks every component in the scene and points references to the old object (or its
    // children and components) at the matching object on the replacement. Inspector fields,
    // arrays, UnityEvents and Timeline bindings are all serialized, so they are covered.
    private static int RemapSceneReferences(Scene scene, GameObject old, Dictionary<Object, Object> map)
    {
        int updated = 0;
        var unmapped = new HashSet<string>();

        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Component comp in root.GetComponentsInChildren<Component>(true))
            {
                // Skip components on the old object: they are about to be destroyed
                if (comp == null || comp.transform.IsChildOf(old.transform))
                {
                    continue;
                }

                SerializedObject so = new SerializedObject(comp);
                SerializedProperty prop = so.GetIterator();
                bool recorded = false;
                bool changed = false;

                while (prop.NextVisible(true))
                {
                    if (prop.propertyType != SerializedPropertyType.ObjectReference)
                    {
                        continue;
                    }

                    Object value = prop.objectReferenceValue;
                    if (value == null || !IsUnder(value, old))
                    {
                        continue;
                    }

                    Object target;
                    if (map.TryGetValue(value, out target))
                    {
                        if (!recorded)
                        {
                            Undo.RecordObject(comp, "Update scene references");
                            recorded = true;
                        }
                        prop.objectReferenceValue = target;
                        changed = true;
                        updated++;
                    }
                    else
                    {
                        unmapped.Add(comp.gameObject.name + " (" + comp.GetType().Name + ")." + prop.propertyPath);
                    }
                }

                if (changed)
                {
                    so.ApplyModifiedProperties();
                }
            }
        }

        if (unmapped.Count > 0)
        {
            var sb = new StringBuilder();
            foreach (string entry in unmapped)
            {
                sb.AppendLine(entry);
            }
            Debug.LogWarning("These references still point at " + old.name +
                " and have no matching object on the replacement:\n" + sb, old);
        }

        return updated;
    }

    // True if the object is the old GameObject, one of its children, or a component on either.
    private static bool IsUnder(Object value, GameObject old)
    {
        Transform t = null;
        GameObject go = value as GameObject;
        Component comp = value as Component;

        if (go != null)
        {
            t = go.transform;
        }
        else if (comp != null)
        {
            t = comp.transform;
        }

        return t != null && t.IsChildOf(old.transform);
    }

    // ---- Animation ----------------------------------------------------------------------

    private static void CopyAnimation(Animation source, Animation target)
    {
        SerializedObject src = new SerializedObject(source);
        SerializedObject dst = new SerializedObject(target);
        Undo.RecordObject(target, "Copy animation list");

        foreach (string propName in s_AnimationProperties)
        {
            SerializedProperty s = src.FindProperty(propName);
            SerializedProperty d = dst.FindProperty(propName);
            if (s == null || d == null)
            {
                Debug.LogWarning("Animation property '" + propName + "' was not found, so it was not copied.", target);
                continue;
            }
            CopyProperty(s, d);
        }

        dst.ApplyModifiedProperties();
    }

    private static void CopyProperty(SerializedProperty s, SerializedProperty d)
    {
        if (s.isArray)
        {
            d.arraySize = s.arraySize;
            for (int i = 0; i < s.arraySize; i++)
            {
                CopyValue(s.GetArrayElementAtIndex(i), d.GetArrayElementAtIndex(i));
            }
        }
        else
        {
            CopyValue(s, d);
        }
    }

    private static void CopyValue(SerializedProperty s, SerializedProperty d)
    {
        switch (s.propertyType)
        {
            case SerializedPropertyType.ObjectReference:
                d.objectReferenceValue = s.objectReferenceValue;
                break;
            case SerializedPropertyType.Integer:
                d.intValue = s.intValue;
                break;
            case SerializedPropertyType.Boolean:
                d.boolValue = s.boolValue;
                break;
            case SerializedPropertyType.Float:
                d.floatValue = s.floatValue;
                break;
            case SerializedPropertyType.Enum:
                d.enumValueIndex = s.enumValueIndex;
                break;
            default:
                Debug.LogWarning("Unsupported property type " + s.propertyType + " for '" + s.name + "'.");
                break;
        }
    }

    // ---- Children -------------------------------------------------------------------------

    private void HandleChildren(GameObject old, GameObject newObj)
    {
        // Copy the list first, because moving children changes the hierarchy
        var children = new List<Transform>();
        foreach (Transform child in old.transform)
        {
            children.Add(child);
        }

        var leftBehind = new StringBuilder();
        foreach (Transform child in children)
        {
            bool nameClash = newObj.transform.Find(child.name) != null;

            if (m_MoveChildren && !nameClash)
            {
                Undo.SetTransformParent(child, newObj.transform, "Move child");
            }
            else
            {
                if (leftBehind.Length > 0) leftBehind.Append(", ");
                leftBehind.Append(child.name);
            }
        }

        if (leftBehind.Length > 0)
        {
            Debug.LogWarning("Children of " + old.name + " were not moved and will be destroyed with it: " +
                leftBehind, newObj);
        }
    }
}