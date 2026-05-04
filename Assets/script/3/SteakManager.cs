using UnityEngine;

public class SteakManager : MonoBehaviour
{
    [Header("Seasoning State")]
    public bool HasSalt = false;
    public bool HasGarlic = false;
    public bool HasRosemary = false;
    public bool HasPepper = false;
    public bool HasButter = false;
    public bool HasOil = false;

    [Header("Pan Reference")]
    public PanHeatReceiver currentPanHeatReceiver;

    private void OnCollisionEnter(Collision collision)
    {
        // 当碰到 Tag 为 Pan 的物体时，记录该锅对应的 PanHeatReceiver
        if (collision.gameObject.CompareTag("Pan"))
        {
            currentPanHeatReceiver = collision.gameObject.GetComponentInParent<PanHeatReceiver>();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // 当离开 Tag 为 Pan 的物体时，清空锅引用
        if (collision.gameObject.CompareTag("Pan"))
        {
            currentPanHeatReceiver = null;
        }
    }
}