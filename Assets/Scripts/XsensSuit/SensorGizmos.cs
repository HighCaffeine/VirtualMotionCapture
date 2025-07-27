using UnityEngine;

public class SensorGizmo : MonoBehaviour
{
    public float axisLength = 0.1f;

    [SerializeField] private SensorOffsetData.SensorEntry sensorData;

    //public void Init()

    void OnDrawGizmos()
    {
        Vector3 pos = transform.position;

        // X+ (red)
        Gizmos.color = Color.red;
        Gizmos.DrawLine(pos, pos + transform.right * axisLength);

        // Y+ (green)
        Gizmos.color = Color.green;
        Gizmos.DrawLine(pos, pos + transform.up * axisLength);

        // Z+ (blue)
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(pos, pos + transform.forward * axisLength);
    }
}
