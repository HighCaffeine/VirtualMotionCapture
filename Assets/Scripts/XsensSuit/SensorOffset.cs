using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SensorOffsetData", menuName = "Xsens/SensorOffsetData")]
public class SensorOffsetData : ScriptableObject
{
    [System.Serializable]
    public class SensorEntry
    {
        public XsensJointManager.eXSensSuitJointPoint jointPoint;   //관절 포인트
        public HumanBodyBones bone;                                 //관절 포인트에 해당하는 휴머노이드 리깅 본
        public Vector3 localPositionOffset;                         //본에서 관절 포인트가 얼마나 떨어져있는지
        public Vector3 forward = Vector3.forward;
        public Vector3 up = Vector3.up;
    }

    public List<SensorEntry> sensors;
}
