#if UNITY_EDITOR_64
using UnityEngine;
using UnityEditor;

public class RebindPoseFixer : EditorWindow
{
    SkinnedMeshRenderer targetMesh;
    Transform targetRoot;

    [MenuItem("Tools/Fix Skinned Mesh BindPose")]
    static void Init()
    {
        GetWindow<RebindPoseFixer>("Fix BindPose");
    }

    void OnGUI()
    {
        //targetMesh
        targetMesh = (SkinnedMeshRenderer)EditorGUILayout.ObjectField("Target Mesh", targetMesh, typeof(SkinnedMeshRenderer), true);
        targetRoot = (Transform)EditorGUILayout.ObjectField("Target Root Bone", targetRoot, typeof(Transform), true);

        if (GUILayout.Button("Fix Bind Pose") && targetMesh != null && targetRoot != null)
        {
            Transform[] newBones = new Transform[targetMesh.bones.Length];
            Matrix4x4[] bindPoses = new Matrix4x4[targetMesh.bones.Length];

            for (int i = 0; i < targetMesh.bones.Length; i++)
            {
                Debug.Log(targetMesh.bones[i].name);
                var boneName = targetMesh.bones[i].name;
                var newBone = FindChildByName(targetRoot, boneName);
                if (newBone != null)
                {
                    newBones[i] = newBone;
                    bindPoses[i] = newBone.worldToLocalMatrix * targetMesh.transform.localToWorldMatrix;
                }
                else
                {
                    Debug.LogWarning($"Bone not found: {boneName}");
                }
            }

            targetMesh.bones = newBones;
            targetMesh.sharedMesh.bindposes = bindPoses;
            Debug.Log("Bind pose updated!");
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