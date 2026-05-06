using UnityEngine;

public class WaterWashZone : MonoBehaviour
{
    [Header("Reference")]
    public FaucetController faucetController;

    private void OnTriggerStay(Collider other)
    {
        PanWashState panWashState = other.GetComponentInParent<PanWashState>();

        if (panWashState != null)
        {
            if (faucetController != null && faucetController.IsWaterOn)
            {
                panWashState.SetInWater(true);
            }
            else
            {
                panWashState.SetInWater(false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PanWashState panWashState = other.GetComponentInParent<PanWashState>();

        if (panWashState != null)
        {
            panWashState.SetInWater(false);
        }
    }
}