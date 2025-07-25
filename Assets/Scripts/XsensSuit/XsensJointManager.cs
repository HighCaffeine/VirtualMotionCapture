using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;

public class XsensJointManager : MonoBehaviour
{
    public enum eXSensSuitJointPoint
    {
        Head, Sternum, Pelvis,

        L_Shoulder, L_UpperArm, L_ForeArm, L_Hand,
        L_UpperLeg, L_LowerLeg, L_Foot,

        R_Shoulder, R_UpperArm, R_Forearm, R_Hand,
        R_UpperLeg, R_LowerLeg, R_Foot,
        Count
    }

    private List<GameObject> jointPoints;

    private Quaternion[] jointCalibrations = new Quaternion[Devcat.ValueCastTo<int>.From(eXSensSuitJointPoint.Count)];

    [Header("Target Model Animator")][SerializeField] private Animator animator;

    void Start()
    {
        jointPoints = JointSpawner.Instance.CreateJoint();
    }


    private void Calibration()
    {
        //T포즈 캘리브레이션

        SetCalibrationData();
    }

    //캘리브레이션 데이터 저장
    private void SetCalibrationData()
    {
        int index = 0;

        foreach (var joint in jointPoints)
        {
            jointCalibrations[index++] = joint.transform.rotation;
        }
    }

    //매개변수로 넘어온 관절 포인트의 회전값 반환
    private Quaternion GetSensorRotation(eXSensSuitJointPoint eXSensSuitJointPoint)
    {
        return jointCalibrations[Devcat.ValueCastTo<int>.From(eXSensSuitJointPoint)];
    }

    //매개변수로 넘어온 관절 포인트의 센서 회전값을 통해 본의 회전값을 역산하여 추정 (offset vector 회전수치 사용)
    private Quaternion GetBoneRotation(eXSensSuitJointPoint eXSensSuitJointPoint)
    {
        Transform boneTransform = animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
        Quaternion initialBoneRotation = boneTransform.rotation; // Unity 기준 회전

        Quaternion initialSensorRotation = GetSensorRotation(eXSensSuitJointPoint); // 센서가 처음 측정한 회전
        Quaternion R_offset = initialSensorRotation * Quaternion.Inverse(initialBoneRotation);
        Quaternion calibratedBoneRotation = jointPoints[Devcat.ValueCastTo<int>.From(eXSensSuitJointPoint)].transform.rotation * Quaternion.Inverse(R_offset);

        return calibratedBoneRotation;
    }

    // List<JointInfo> sensorJointInfos = new List<JointInfo>
    // {
    //     new JointInfo { name = GetJointName(eXSensSuitJointPoint.Pelvis),       bone = HumanBodyBones.Hips,             forward = Vector3.forward,  up = Vector3.up },
    //     new JointInfo { name = GetJointName(eXSensSuitJointPoint.Sternum),      bone = HumanBodyBones.Chest,            forward = Vector3.forward,  up = Vector3.up },
    //     new JointInfo { name = GetJointName(eXSensSuitJointPoint.Head),         bone = HumanBodyBones.Head,             forward = Vector3.forward,  up = Vector3.up },

    //     new JointInfo { name = GetJointName(eXSensSuitJointPoint.L_Shoulder),   bone = HumanBodyBones.LeftShoulder,     forward = Vector3.right,    up = Vector3.down },
    //     new JointInfo { name = GetJointName(eXSensSuitJointPoint.L_UpperArm),   bone = HumanBodyBones.LeftUpperArm,     forward = Vector3.down,     up = Vector3.back },
    //     new JointInfo { name = GetJointName(eXSensSuitJointPoint.L_ForeArm),    bone = HumanBodyBones.LeftLowerArm,     forward = Vector3.down,     up = Vector3.back },
    //     new JointInfo { name = GetJointName(eXSensSuitJointPoint.L_Hand),       bone = HumanBodyBones.LeftHand,         forward = Vector3.forward,  up = Vector3.up },
    //     new JointInfo { name = GetJointName(eXSensSuitJointPoint.L_UpperLeg),   bone = HumanBodyBones.LeftUpperLeg,     forward = Vector3.down,     up = Vector3.forward },
    //     new JointInfo { name = GetJointName(eXSensSuitJointPoint.L_LowerLeg),   bone = HumanBodyBones.LeftLowerLeg,     forward = Vector3.down,     up = Vector3.forward },
    //     new JointInfo { name = GetJointName(eXSensSuitJointPoint.L_Foot),       bone = HumanBodyBones.LeftFoot,         forward = Vector3.forward,  up = Vector3.up },

    //     new JointInfo { name = GetJointName(eXSensSuitJointPoint.R_Shoulder),   bone = HumanBodyBones.RightShoulder,    forward = Vector3.left,     up = Vector3.down },
    //     new JointInfo { name = GetJointName(eXSensSuitJointPoint.R_UpperArm),   bone = HumanBodyBones.RightUpperArm,    forward = Vector3.down,     up = Vector3.back },
    //     new JointInfo { name = GetJointName(eXSensSuitJointPoint.R_Forearm),    bone = HumanBodyBones.RightLowerArm,    forward = Vector3.down,     up = Vector3.back },
    //     new JointInfo { name = GetJointName(eXSensSuitJointPoint.R_Hand),       bone = HumanBodyBones.RightHand,        forward = Vector3.forward,  up = Vector3.up },
    //     new JointInfo { name = GetJointName(eXSensSuitJointPoint.R_UpperLeg),   bone = HumanBodyBones.RightUpperLeg,    forward = Vector3.down,     up = Vector3.forward },
    //     new JointInfo { name = GetJointName(eXSensSuitJointPoint.R_LowerLeg),   bone = HumanBodyBones.RightLowerLeg,    forward = Vector3.down,     up = Vector3.forward },
    //     new JointInfo { name = GetJointName(eXSensSuitJointPoint.R_Foot),       bone = HumanBodyBones.RightFoot,        forward = Vector3.forward,  up = Vector3.up }
    // };

    // private static string GetJointName(eXSensSuitJointPoint eXSensSuitJointPoint)
    // {
    //     string str = string.Empty;

    //     switch (eXSensSuitJointPoint)
    //     {
    //         case eXSensSuitJointPoint.Head:
    //             str = eXSensSuitJointPoint.Head.ToString();
    //             break;
    //         case eXSensSuitJointPoint.Sternum:
    //             str = eXSensSuitJointPoint.Sternum.ToString();
    //             break;
    //         case eXSensSuitJointPoint.Pelvis:
    //             str = eXSensSuitJointPoint.Pelvis.ToString();
    //             break;
    //         case eXSensSuitJointPoint.L_Shoulder:
    //             str = eXSensSuitJointPoint.L_Shoulder.ToString();
    //             break;
    //         case eXSensSuitJointPoint.L_UpperArm:
    //             str = eXSensSuitJointPoint.L_UpperArm.ToString();
    //             break;
    //         case eXSensSuitJointPoint.L_ForeArm:
    //             str = eXSensSuitJointPoint.L_ForeArm.ToString();
    //             break;
    //         case eXSensSuitJointPoint.L_Hand:
    //             str = eXSensSuitJointPoint.L_Hand.ToString();
    //             break;
    //         case eXSensSuitJointPoint.L_UpperLeg:
    //             str = eXSensSuitJointPoint.L_UpperLeg.ToString();
    //             break;
    //         case eXSensSuitJointPoint.L_LowerLeg:
    //             str = eXSensSuitJointPoint.L_LowerLeg.ToString();
    //             break;
    //         case eXSensSuitJointPoint.L_Foot:
    //             str = eXSensSuitJointPoint.L_Foot.ToString();
    //             break;
    //         case eXSensSuitJointPoint.R_Shoulder:
    //             str = eXSensSuitJointPoint.R_Shoulder.ToString();
    //             break;
    //         case eXSensSuitJointPoint.R_UpperArm:
    //             str = eXSensSuitJointPoint.R_UpperArm.ToString();
    //             break;
    //         case eXSensSuitJointPoint.R_Forearm:
    //             str = eXSensSuitJointPoint.R_Forearm.ToString();
    //             break;
    //         case eXSensSuitJointPoint.R_Hand:
    //             str = eXSensSuitJointPoint.R_Hand.ToString();
    //             break;
    //         case eXSensSuitJointPoint.R_UpperLeg:
    //             str = eXSensSuitJointPoint.R_UpperLeg.ToString();
    //             break;
    //         case eXSensSuitJointPoint.R_LowerLeg:
    //             str = eXSensSuitJointPoint.R_LowerLeg.ToString();
    //             break;
    //         case eXSensSuitJointPoint.R_Foot:
    //             str = eXSensSuitJointPoint.R_Foot.ToString();
    //             break;
    //     }

    //     return str;
    // }

}
