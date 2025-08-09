using UnityEngine;

public class SensorDataSender : MonoBehaviour, ISensorData
{
    private XsensJointManager.eModelName modelName;
    private XsensJointManager.eXSensSuitJointPoint jointPoint;
    private XsensJointManager.OnSetSensorData OnSetSensorData;

    private Transform TEST_Bone;

    private void Update()
    {

        Debug.Log($"Sensor : {transform.name} {transform.position} / {transform.rotation}");
        Debug.Log($"Bone : {transform.name} {TEST_Bone.position} / {TEST_Bone.rotation}");

        return;

        if (OnSetSensorData(modelName, jointPoint, transform.position, transform.rotation))
        {
            Debug.Log($"[Success] Sensor Data Send To {modelName} ({jointPoint})");
        }

        Debug.Log($"[Fail] Sensor Data Send Fail {transform.name}");
    }

    public Vector3 GetPos()
    {
        return transform.position;
    }

    public Quaternion GetRotation()
    {
        return transform.rotation;
    }

    public void Init(XsensJointManager.OnSetSensorData OnSetSensorData)
    {
        //Pooling으로 나중에 변경
        this.OnSetSensorData = XsensJointManager.Instance.SetJointPoint;
    }

    public void SetJointPoint(XsensJointManager.eXSensSuitJointPoint jointPoint)
    {
        this.jointPoint = jointPoint;
    }

    public void TEST_SetBone(Transform bone)
    {
        TEST_Bone = bone;
    }

    public void SetModelName(XsensJointManager.eModelName modelName)
    {
        this.modelName = modelName;
    }
}
