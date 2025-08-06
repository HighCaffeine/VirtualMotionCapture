using UnityEngine;
using UnityEditor;

public class RemoveMissingScripts
{
    [MenuItem("Tools/Cleanup/Remove Missing Scripts in Scene")]
    static void RemoveMissingScriptsInScene()
    {
        int objCount = 0, componentCount = 0, missingCount = 0;

        GameObject[] allGameObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allGameObjects)
        {
            objCount++;
            int countBefore = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(obj);
            if (countBefore > 0)
            {
                Undo.RegisterCompleteObjectUndo(obj, "Remove missing scripts");
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(obj);
                int countAfter = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(obj);
                int removed = countBefore - countAfter;

                if (removed > 0)
                {
                    missingCount += removed;
                    Debug.LogWarning($"Removed {removed} missing scripts from: {GetFullPath(obj)}", obj);
                }
            }
        }

        Debug.Log($"[완료] 총 {objCount}개 오브젝트 중 {missingCount}개 Missing Script 제거됨.");
    }

    static string GetFullPath(GameObject obj)
    {
        return obj.transform.parent == null ? obj.name : GetFullPath(obj.transform.parent.gameObject) + "/" + obj.name;
    }
}
