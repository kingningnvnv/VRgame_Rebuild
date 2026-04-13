using UnityEngine;

public class KnobZClamp : MonoBehaviour
{
    public float minZ = -90f;
    public float maxZ = 0f;

    void LateUpdate()
    {
        Vector3 euler = transform.localEulerAngles;

        float z = euler.z;
        if (z > 180f) z -= 360f;   // 转成 -180 ~ 180

        z = Mathf.Clamp(z, minZ, maxZ);

        transform.localEulerAngles = new Vector3(0f, 0f, z);
    }
}