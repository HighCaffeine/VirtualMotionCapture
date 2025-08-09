using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using System;

public interface ISensorData
{
    public void Init(XsensJointManager.OnSetSensorData OnSetSensorData);
}

public class XsensJointManager : GenericSingleton<XsensJointManager>
{
    //TEST
    public enum eModelName
    {
        Xsens_Suit,
        Xsens_Client,

        VRMTest,

        IkomaMiru_MellowHeart,
        IkomaMiru_Original,

        Count,
    }
    //TEST

    public enum eXSensSuitJointPoint
    {
        Head, Sternum, Pelvis,

        L_Shoulder, L_UpperArm, L_ForeArm, L_Hand,
        L_UpperLeg, L_LowerLeg, L_Foot,

        R_Shoulder, R_UpperArm, R_Forearm, R_Hand,
        R_UpperLeg, R_LowerLeg, R_Foot,
        Count
    }

    public class SensorSegmentData
    {
        public float xPos, yPos, zPos;
        public float xRot, yRot, zRot, w;
        public SensorSegmentData()
        {
            xPos = 0.0f; yPos = 0.0f; zPos = 0.0f;
            xRot = 0.0f; yRot = 0.0f; zRot = 0.0f; w = 0.0f;
        }

        public void SetValue(Vector3 pos, Quaternion rot)
        {
            xPos = pos.x; yPos = pos.y; zPos = pos.z;
            xRot = rot.x; yRot = rot.y; zRot = rot.z; w = rot.w;
        }
    }

    public class ModelSensorPoint
    {
        public SensorSegmentData[] sensorSegmentDatas;

        public ModelSensorPoint()
        {
            sensorSegmentDatas = new SensorSegmentData[Devcat.ValueCastTo<int>.From(eXSensSuitJointPoint.Count)];
        }

        public SensorSegmentData GetSensorSegmentData(eXSensSuitJointPoint sensorPoint)
        {
            int index = Devcat.ValueCastTo<int>.From(sensorPoint);

            if (index < 0 || Devcat.ValueCastTo<int>.From(eXSensSuitJointPoint.Count) <= index) return null;

            return sensorSegmentDatas[index];
        }
    }

    private List<SensorDataSender> jointPoints;
    private Dictionary<int, ModelSensorPoint> modelSensorDatas;

    private Quaternion[] jointCalibrations = new Quaternion[Devcat.ValueCastTo<int>.From(eXSensSuitJointPoint.Count)];

    [Header("Target Model Animator")][SerializeField] private Animator targetAvatar;
    [Header("Base Model Animator")][SerializeField] private Animator baseAvatar;

    private new void Awake()
    {
        modelSensorDatas = new Dictionary<int, ModelSensorPoint>();
    }

    private void Start()
    {
        jointPoints = JointSpawner.Instance.CreateJoint(eModelName.IkomaMiru_MellowHeart);
    }

    public Transform GetBone(SensorOffsetData.SensorEntry sensorEntry)
    {
        return targetAvatar.GetBoneTransform(sensorEntry.bone);
    }

    public Transform GetBaseBone(SensorOffsetData.SensorEntry sensorEntry)
    {
        return baseAvatar.GetBoneTransform(sensorEntry.bone);
    }

    //base -> base 변환으로 
    //현재 base 자체 적용도 좌표 변환때문에 똑같이 처리해줘야함
    public Vector3 ConvertBaseSensor(SensorOffsetData.SensorEntry sensorEntry)
    {
        return ConvertSensorPos(baseAvatar, baseAvatar, sensorEntry);
    }

    public Vector3 ConvertSensorPosToBase(SensorOffsetData.SensorEntry sensorEntry)
    {
        return ConvertSensorPos(baseAvatar, targetAvatar, sensorEntry);
    }

    //base -> target으로 좌표 변환 
    //baseAvatar 기준으로 만들어진 offset을 inverse 적용하여 
    //targetAvatar에 다시 회전 적용하여 worldPos를 얻음
    private Vector3 ConvertSensorPos(Animator b, Animator t, SensorOffsetData.SensorEntry sensorEntry)
    {
        var baseBone = b.GetBoneTransform(sensorEntry.bone);
        var targetBone = t.GetBoneTransform(sensorEntry.bone);
        float avatarScale = GetScaleRatio();

        Vector3 newSensorOffset = sensorEntry.localPositionOffset * avatarScale;

        Vector3 baseSensorWorldPos = baseBone.position + baseBone.rotation * newSensorOffset;
        Vector3 targetLocalOffset = Quaternion.Inverse(targetBone.rotation) * (baseSensorWorldPos - baseBone.position);
        Vector3 targetSensorWorldPos = targetBone.position + targetBone.rotation * targetLocalOffset;

        return targetSensorWorldPos;
    }

    public delegate bool OnSetSensorData(eModelName modelName, eXSensSuitJointPoint sensorPoint, Vector3 pos, Quaternion rotate);
    public OnSetSensorData onSetSensorData;

    public bool AddCharacter(eModelName modelName)
    {
        int key = Devcat.ValueCastTo<int>.From(modelName);

        if (modelSensorDatas.ContainsKey(key))
        {
            return false;
        }

        Debug.Log($"Model Create Apply {modelName}({key})");
        modelSensorDatas.Add(key, new ModelSensorPoint());

        return true;
    }


    // Xsens Suit -> Xsens Client Model -> Xsens Unity Live Animation Model
    // 3가지 변환을 거치게 됨.
    // 1 -> 2의 경우    1번이 움직일 때 센서의 데이터를 전달, 
    //                 2번이 받은 센서 데이터를 베이스로 JointNumber에 해당된 관절을 움직여줌

    // 2 -> 3의 경우    2번이 관절을 움직인 데이터를 전달(휴머노이드로 변환하여 전송)
    //                 3번이 받은 휴머노이드 데이터를 모델 Avatar에 적용하여 움직임
    //
    // 1 -> 2로 가는 센서 데이터 전송
    // 2에서 센서 데이터를 관절 데이터로 변환하는 함수
    //      관절 데이터를 Unity 휴머노이드로 변환하는 함수
    // 3에서 받은 휴머노이드를 targetAvatar에 적용하는 함수
    public bool SetJointPoint(eModelName modelName, eXSensSuitJointPoint jointPoint, Vector3 pos, Quaternion rotate)
    {
        int key = Devcat.ValueCastTo<int>.From(modelName);
        if (key < 0 || Devcat.ValueCastTo<int>.From(eXSensSuitJointPoint.Count) <= key) return false;

        int jointKey = Devcat.ValueCastTo<int>.From(jointPoint);
        modelSensorDatas[key].sensorSegmentDatas[jointKey].SetValue(pos, rotate);

        return true;
    }

    // // 센서 위치를 기준으로 담당 관절 포인트에 각속도를 넣어줌
    // public Vector3 ConvertSensorToJoint(eModelName modelName, SensorOffsetData.SensorEntry sensorEntry)
    // {
    //     var sensor =;
    //     var bone = ;
    //     //var baseBone = b.GetBoneTransform(sensorEntry.bone);
    //     var targetBone = t.GetBoneTransform(sensorEntry.bone);
    //     float avatarScale = GetScaleRatio();

    //     Vector3 newSensorOffset = sensorEntry.localPositionOffset * avatarScale;

    //     Vector3 baseSensorWorldPos = baseBone.position + baseBone.rotation * newSensorOffset;
    //     Vector3 targetLocalOffset = Quaternion.Inverse(targetBone.rotation) * (baseSensorWorldPos - baseBone.position);
    //     Vector3 targetSensorWorldPos = targetBone.position + targetBone.rotation * targetLocalOffset;

    // }

    // public Vector3 ConvertJointToHumanoid(eModelName modelName, SensorOffsetData.SensorEntry sensorEntry)
    // {

    // }


    public float GetScaleRatio()
    {
        Transform baseBoneStart = baseAvatar.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        Transform baseBoneEnd = baseAvatar.GetBoneTransform(HumanBodyBones.LeftLowerArm);

        Transform targetBoneStart = targetAvatar.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        Transform targetBoneEnd = targetAvatar.GetBoneTransform(HumanBodyBones.LeftLowerArm);

        float baseLength = Vector3.Distance(baseBoneStart.position, baseBoneEnd.position);
        float targetLength = Vector3.Distance(targetBoneStart.position, targetBoneEnd.position);

        return targetLength / baseLength;
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
        Transform boneTransform = targetAvatar.GetBoneTransform(HumanBodyBones.RightLowerLeg);
        Quaternion initialBoneRotation = boneTransform.rotation; // Unity 기준 회전

        Quaternion initialSensorRotation = GetSensorRotation(eXSensSuitJointPoint); // 센서가 처음 측정한 회전
        Quaternion R_offset = initialSensorRotation * Quaternion.Inverse(initialBoneRotation);
        Quaternion calibratedBoneRotation = jointPoints[Devcat.ValueCastTo<int>.From(eXSensSuitJointPoint)].transform.rotation * Quaternion.Inverse(R_offset);

        return calibratedBoneRotation;
    }

    Vector3 ConvertXsensToUnity(Vector3 xsensVector)
    {
        return new Vector3(
            xsensVector.x,    // X는 동일
            xsensVector.z,    // Z (Xsens up) → Unity Y
            xsensVector.y     // Y (Xsens forward) → Unity Z
        );
    }

    Quaternion ConvertXsensToUnity(Quaternion xsensRotation)
    {
        // Xsens은 오른손 좌표계이므로, Z축 반전을 통해 Unity 좌표계에 맞춤
        return new Quaternion(xsensRotation.x, xsensRotation.y, -xsensRotation.z, -xsensRotation.w);
    }

    private void ApplyBoneRotate(Quaternion xsensSensorRotation, SensorOffsetData.SensorEntry sensor)
    {
        var bone = targetAvatar.GetBoneTransform(sensor.bone);

        bone.rotation = ConvertXsensToUnity(xsensSensorRotation) * Quaternion.LookRotation(sensor.forward, sensor.up);
        var sensorRotation = Quaternion.LookRotation(sensor.forward, sensor.up);
        bone.rotation = ConvertXsensToUnity(xsensSensorRotation) * sensorRotation;
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
