using UnityEngine;

public class SeasoningBurnAway : MonoBehaviour
{
    [Header("受热后多久消失（秒）")]
    public float burnDelay = 3f;

    private SteakManager steakManager;
    private float heatTimer = 0f;
    private bool isBurning = false;

    void Start()
    {
        // 自动往父级找牛排
        steakManager = GetComponentInParent<SteakManager>();
    }

    void Update()
    {
        if (steakManager == null) return;

        bool isHeated =
            steakManager.currentPanHeatReceiver != null &&
            steakManager.currentPanHeatReceiver.IsHeated;

        if (isHeated)
        {
            heatTimer += Time.deltaTime;

            if (!isBurning)
            {
                isBurning = true;
            }

            if (heatTimer >= burnDelay)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            // 一旦没加热，就重置计时
            heatTimer = 0f;
            isBurning = false;
        }
    }
}