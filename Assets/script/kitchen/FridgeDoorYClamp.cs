using UnityEngine;

public class FridgeDoorYClamp : MonoBehaviour
{
    [SerializeField] private float minY = 0f;
    [SerializeField] private float maxY = 120f;

    private void LateUpdate()
    {
        Vector3 euler = transform.localEulerAngles;

        float y = euler.y;
        if (y > 180f)
        {
            y -= 360f;
        }

        y = Mathf.Clamp(y, minY, maxY);

        transform.localEulerAngles = new Vector3(0f, y, 0f);
    }
}