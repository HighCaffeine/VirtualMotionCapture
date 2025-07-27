using UnityEngine;
using System.Collections.Generic;

public class SensorDebugger : MonoBehaviour
{
    public SensorOffsetData sensorData;
    public Animator animator;
    public float gizmoSize = 0.02f;

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
            Vector3 sensorPos = bone.TransformPoint(sensor.localPositionOffset);

            // Draw line
            //Gizmos.color = Color.yellow;
            //Gizmos.DrawLine(basePos, sensorPos);

            // Draw sensor position
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(sensorPos, gizmoSize * 0.1f);

            // Draw forward/up
            Vector3 forward = bone.TransformDirection(sensor.forward.normalized);
            Vector3 up = bone.TransformDirection(sensor.up.normalized);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(sensorPos, sensorPos + forward * gizmoSize);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(sensorPos, sensorPos + up * gizmoSize);

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
