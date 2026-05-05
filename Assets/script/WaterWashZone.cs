using UnityEngine;

public class WaterWashZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PanWashState panWashState = other.GetComponentInParent<PanWashState>();

        if (panWashState != null)
        {
            panWashState.SetInWater(true);
            Debug.Log("WaterWashZone: Pan entered water zone.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PanWashState panWashState = other.GetComponentInParent<PanWashState>();

        if (panWashState != null)
        {
            panWashState.SetInWater(false);
            Debug.Log("WaterWashZone: Pan exited water zone.");
        }
    }
}