using UnityEngine;

public class PanBottomDetector : MonoBehaviour
{
    [SerializeField] private PanHeatReceiver panHeatReceiver;

    private void OnTriggerEnter(Collider other)
    {
        TrySetBurner(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TrySetBurner(other);
    }

    private void OnTriggerExit(Collider other)
    {
        StoveBurnerHeatSource burner = other.GetComponent<StoveBurnerHeatSource>();
        if (burner != null && panHeatReceiver != null)
        {
            panHeatReceiver.ClearCurrentBurner(burner);
        }
    }

    private void TrySetBurner(Collider other)
    {
        StoveBurnerHeatSource burner = other.GetComponent<StoveBurnerHeatSource>();
        if (burner != null && panHeatReceiver != null)
        {
            panHeatReceiver.SetCurrentBurner(burner);
        }
    }
}