#if UNITY_EDITOR_64
using UnityEngine;
using UnityEditor;

public class BoneRebinder : EditorWindow
{
    SkinnedMeshRenderer targetMesh;
    Transform targetRoot;

    [MenuItem("Tools/Rebind Skinned Mesh Bones")]
    static void Init()
    {
        GetWindow<BoneRebinder>("Rebind Bones");
    }

    void OnGUI()
    {
        targetMesh = (SkinnedMeshRenderer)EditorGUILayout.ObjectField("Target Mesh", targetMesh, typeof(SkinnedMeshRenderer), true);
        targetRoot = (Transform)EditorGUILayout.ObjectField("Target Root Bone", targetRoot, typeof(Transform), true);

        if (GUILayout.Button("Rebind") && targetMesh != null && targetRoot != null)
        {
            Transform[] newBones = new Transform[targetMesh.bones.Length];
            for (int i = 0; i < targetMesh.bones.Length; i++)
            {
                Debug.Log(targetMesh.bones[i].name);
                var boneName = targetMesh.bones[i].name;
                var newBone = FindChildByName(targetRoot, boneName);
                if (newBone != null)
                {
                    newBones[i] = newBone;
                }
                else
                {
                    Debug.LogWarning($"Bone not found: {boneName}");
                }
            }

            targetMesh.bones = newBones;
            Debug.Log("Bones Rebound");
        }
    }

    Transform FindChildByName(Transform parent, string name)
    {
        foreach (Transform t in parent.GetComponentsInChildren<Transform>(true))
        {
            if (t.name == name) return t;
        }

        return null;
    }
}
#endif