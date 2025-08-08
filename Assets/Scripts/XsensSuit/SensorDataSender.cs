using UnityEngine;

public class SensorDataSender : MonoBehaviour
{
    private void Update()
    {
        Debug.Log($"{transform.name} : {GetPos()} / {GetRotation()}");
    }

    public Vector3 GetPos()
    {
        return transform.position;
    }

    public Quaternion GetRotation()
    {
        return transform.rotation;
    }
}
