using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;

public class SensorDebugger : MonoBehaviour
{
    [SerializeField] private SensorOffsetData sensorData;
    [SerializeField] private float gizmoSize = 0.02f;

    void OnDrawGizmos()
    {
        if (sensorData == null) return;

        int index = 0;

        foreach (var sensor in sensorData.sensors)
        {
            var bone = XsensJointManager.Instance.GetBone(sensor);

            if (bone == null)
            {
                continue;
            }

            Vector3 basePos = bone.position;
            //Vector3 sensorPos = bone.TransformPoint(sensor.localPositionOffset);
            Vector3 sensorPos = XsensJointManager.Instance.ConvertSensorPosToBase(sensor);

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
                Gizmos.DrawLine(basePos, XsensJointManager.Instance.GetBone(parentSensor).position);
                //sensor 
                //Gizmos.DrawLine(bone.TransformPoint(sensorData.sensors[index].localPositionOffset), 
                //                  animator.GetBoneTransform(parentSensor.bone).TransformPoint(parentSensor.localPositionOffset));
            }

            index++;
        }
    }
}
