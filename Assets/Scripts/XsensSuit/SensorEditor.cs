#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class SensorEditorWindow : EditorWindow
{
    SensorOffsetData data;
    Vector2 scroll;

    [MenuItem("Xsens/Sensor Editor")]
    static void Init()
    {
        GetWindow<SensorEditorWindow>("Sensor Editor");
    }

    void OnGUI()
    {
        EditorGUILayout.Space();
        if (GUILayout.Button("Export to JSON"))
        {
            string path = EditorUtility.SaveFilePanel("Export Sensor Data", "Assets", "XsensSensorData", "json");
            if (!string.IsNullOrEmpty(path))
            {
                SensorJsonUtility.ExportToJson(data, path);
            }
        }

        if (GUILayout.Button("Import from JSON"))
        {
            string path = EditorUtility.OpenFilePanel("Import Sensor Data", "Assets", "json");
            if (!string.IsNullOrEmpty(path))
            {
                SensorJsonUtility.ImportFromJson(data, path);
            }
        }

        data = (SensorOffsetData)EditorGUILayout.ObjectField("Sensor Data", data, typeof(SensorOffsetData), false);
        if (data == null) return;

        scroll = EditorGUILayout.BeginScrollView(scroll);

        foreach (var sensor in data.sensors)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(string.Format($"Joint Sensor Name : {sensor.jointPoint}"), EditorStyles.boldLabel);
            sensor.jointPoint = (XsensJointManager.eXSensSuitJointPoint)EditorGUILayout.EnumPopup("Joint List", sensor.jointPoint);
            sensor.bone = (HumanBodyBones)EditorGUILayout.EnumPopup("Bone", sensor.bone);
            sensor.localPositionOffset = EditorGUILayout.Vector3Field("Position Offset", sensor.localPositionOffset);
            sensor.forward = EditorGUILayout.Vector3Field("Forward", sensor.forward);
            sensor.up = EditorGUILayout.Vector3Field("Up", sensor.up);
        }

        EditorGUILayout.EndScrollView();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(data);
        }
    }
}
public static class SensorJsonUtility
{
    [System.Serializable]
    private class SensorEntryJson
    {
        public string name;
        public string bone;
        public Vector3 localPositionOffset;
        public Vector3 forward;
        public Vector3 up;
    }

    [System.Serializable]
    private class SensorEntryListJson
    {
        public List<SensorEntryJson> sensors;
    }

    public static void ExportToJson(SensorOffsetData data, string path)
    {
        var jsonData = new SensorEntryListJson { sensors = new List<SensorEntryJson>() };

        foreach (var sensor in data.sensors)
        {
            jsonData.sensors.Add(new SensorEntryJson
            {
                name = sensor.jointPoint.ToString(),
                bone = sensor.bone.ToString(),
                localPositionOffset = sensor.localPositionOffset,
                forward = sensor.forward,
                up = sensor.up
            });
        }

        string json = JsonUtility.ToJson(jsonData, true);
        File.WriteAllText(path, json);
        AssetDatabase.Refresh();
        Debug.Log("Sensor data exported to: " + path);
    }

    public static void ImportFromJson(SensorOffsetData data, string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("File not found: " + path);
            return;
        }

        string json = File.ReadAllText(path);
        var jsonData = JsonUtility.FromJson<SensorEntryListJson>(json);

        data.sensors.Clear();

        foreach (var entry in jsonData.sensors)
        {
            if (!System.Enum.TryParse(entry.bone, out HumanBodyBones bone))
            {
                Debug.LogWarning($"Unknown bone: {entry.bone}");
                continue;
            }

            XsensJointManager.eXSensSuitJointPoint joint;

            if (!System.Enum.TryParse(entry.name, out joint))
            {
                Debug.LogWarning($"Unknown Joint : {entry.name}");
                continue;
            }

            data.sensors.Add(new SensorOffsetData.SensorEntry
            {
                jointPoint = joint,
                bone = bone,
                localPositionOffset = entry.localPositionOffset,
                forward = entry.forward,
                up = entry.up
            });
        }

        EditorUtility.SetDirty(data);
        Debug.Log("Sensor data imported from: " + path);
    }
}

#endif
