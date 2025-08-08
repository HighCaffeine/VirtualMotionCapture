using UnityEngine;
using System.Collections.Generic;

public class JointSpawner : GenericSingleton<JointSpawner>
{
    [SerializeField] private SensorDataSender jointPrefab; // Empty + Gizmo용 프리팹
    [SerializeField] private SensorOffsetData sensorOffsetData;

    //휴머노이드 리깅 계층구조
    Dictionary<int, int> sensorParentMap = new Dictionary<int, int>
    {
        { 1, 0 },   // Sternum      -> Pelvis
        { 2, 1 },   // Head         -> Sternum
    
        { 3, 1 },   // L Shoulder   -> Sternum
        { 4, 3 },   // L Upper Arm  -> L Shoulder
        { 5, 4 },   // L Lower Arm  -> L Upper Arm
        { 6, 5 },   // L Hand       -> L Lower Arm
    
        { 7, 0 },   // L Upper Leg  -> Pelvis
        { 8, 7 },   // L Lower Leg  -> L Upper Leg
        { 9, 8 },   // L Foot       -> L Lower Leg
    
        { 10, 1 },  // R Shoulder   -> Sternum
        { 11, 10 },
        { 12, 11 },
        { 13, 12 },

        { 14, 0 },
        { 15, 14 },
        { 16, 15 }
    };

    //계층 연결된 parent의 index를 반환
    public int GetSensorParentIndex(int currentSensorIndex)
    {
        if (sensorParentMap.TryGetValue(currentSensorIndex, out int parentIndex))
        {
            return parentIndex;
        }
        else
        {
            return -1;
        }
    }

    public List<SensorDataSender> CreateJoint()
    {
        List<SensorDataSender> jointObjects = new List<SensorDataSender>();
        int count = Devcat.ValueCastTo<int>.From(XsensJointManager.eXSensSuitJointPoint.Count);

        for (int i = 0; i < count; i++)
        {
            var sensor = sensorOffsetData.sensors[i];

            //센서에 해당하는 본의 Transform을 받음
            Transform bone = XsensJointManager.Instance.GetBaseBone(sensor);

            if (bone == null)
            {
                Debug.LogWarning($"Bone not found: {sensor.bone}");

                continue;
            }

            Vector3 newSensorPos = XsensJointManager.Instance.ConvertBaseSensor(sensor);
            Quaternion rot = Quaternion.LookRotation(sensor.forward, sensor.up);

            //센서의 로컬 오프셋 값을 기준 본에 더한 위치에 생성
            SensorDataSender joint = Instantiate(jointPrefab, newSensorPos, rot, bone);

            joint.name = $"{i:D2}_{sensor.jointPoint}";
            joint.transform.localScale = Vector3.one * 0.02f;
            jointObjects.Add(joint);

            // 회전 적용 (Z+: forward, Y+: up)
            joint.transform.rotation = Quaternion.LookRotation(
                bone.TransformDirection(sensor.forward.normalized),
                bone.TransformDirection(sensor.up.normalized)
            );
        }

        //실제 생성된 관절 포인트들을 하이라키 계층구로로 바꿔줌
        // for (int i = 0; i < count; i++)
        // {
        //     int parentIndex = GetSensorParentIndex(i);

        //     if (parentIndex != -1)
        //     {
        //         jointObjects[i].transform.SetParent(jointObjects[parentIndex].transform, true);
        //     }
        //     else
        //     {
        //         jointObjects[i].transform.SetParent(jointObjects[Devcat.ValueCastTo<int>.From(XsensJointManager.eXSensSuitJointPoint.Pelvis)].transform);
        //     }
        // }

        return jointObjects;
    }
}
