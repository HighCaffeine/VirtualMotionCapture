using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;

public class SensorDebugger : MonoBehaviour
{
    [SerializeField] private SensorOffsetData sensorData;
    [SerializeField] private Animator animator;         //VRM Avatar
    [SerializeField] private float gizmoSize = 0.02f;

    [SerializeField] private Animator baseAvatar;       //Xsens Live Animation Model

    private Vector3 ConvertSensorPosToBase(SensorOffsetData.SensorEntry sensorEntry)
    {
        var baseBone = baseAvatar.GetBoneTransform(sensorEntry.bone);
        var targetBone = animator.GetBoneTransform(sensorEntry.bone);
        float avatarScale = GetScaleRatio();

        Vector3 newSensorOffset = sensorEntry.localPositionOffset * avatarScale;

        Vector3 baseSensorWorldPos = baseBone.position + baseBone.rotation * newSensorOffset;
        Vector3 targetLocalOffset = Quaternion.Inverse(targetBone.rotation) * (baseSensorWorldPos - baseBone.position);
        Vector3 targetSensorWorldPos = targetBone.position + targetBone.rotation * targetLocalOffset;

        return targetSensorWorldPos;
    }

    private float GetScaleRatio()
    {
        Transform baseBoneStart = baseAvatar.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        Transform baseBoneEnd = baseAvatar.GetBoneTransform(HumanBodyBones.LeftLowerArm);

        Transform targetBoneStart = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        Transform targetBoneEnd = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);

        float baseLength = Vector3.Distance(baseBoneStart.position, baseBoneEnd.position);
        float targetLength = Vector3.Distance(targetBoneStart.position, targetBoneEnd.position);

        return targetLength / baseLength;
    }

    void OnDrawGizmos()
    {
        if (sensorData == null || animator == null) return;

        int index = 0;

        foreach (var sensor in sensorData.sensors)
        {
            var bone = animator.GetBoneTransform(sensor.bone);
            if (bone == null)
            {
                continue;
            }

            Vector3 basePos = bone.position;
            //Vector3 sensorPos = bone.TransformPoint(sensor.localPositionOffset);
            Vector3 sensorPos = ConvertSensorPosToBase(sensor);

            // Draw line
            //Gizmos.color = Color.yellow;
            //Gizmos.DrawLine(basePos, sensorPos);

            // Draw sensor position
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(sensorPos, gizmoSize * 0.1f);

            // Draw forward/up

            Gizmos.color = Color.red;
            //Gizmos.DrawLine(sensorPos, sensorPos + forward * gizmoSize);
            Gizmos.DrawLine(basePos, basePos + sensor.forward * gizmoSize);

            Gizmos.color = Color.green;
            //Gizmos.DrawLine(sensorPos, sensorPos + up * gizmoSize);
            Gizmos.DrawLine(basePos, basePos + sensor.up * gizmoSize);


            //모든 센서를 휴머노이드 계층 구조에 따라 연결시켜줌
            int parentIndex = JointSpawner.Instance.GetSensorParentIndex(index);

            if (parentIndex != -1)
            {
                var parentSensor = sensorData.sensors[parentIndex];
                // Draw line
                Gizmos.color = Color.yellow;
                //bone base
                Gizmos.DrawLine(basePos, animator.GetBoneTransform(parentSensor.bone).position);
                //sensor 
                //Gizmos.DrawLine(bone.TransformPoint(sensorData.sensors[index].localPositionOffset), 
                //                  animator.GetBoneTransform(parentSensor.bone).TransformPoint(parentSensor.localPositionOffset));
            }

            index++;
        }
    }
}
