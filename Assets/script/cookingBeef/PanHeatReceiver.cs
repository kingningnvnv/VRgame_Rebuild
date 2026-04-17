using UnityEngine;

public class PanHeatReceiver : MonoBehaviour
{
    [Header("当前锅底接触到的炉口")]
    [SerializeField] private StoveBurnerHeatSource currentBurner;

    public bool IsHeated
    {
        get
        {
            return currentBurner != null && currentBurner.IsHeating;
        }
    }

    public void SetCurrentBurner(StoveBurnerHeatSource burner)
    {
        currentBurner = burner;
    }

    public void ClearCurrentBurner(StoveBurnerHeatSource burner)
    {
        if (currentBurner == burner)
        {
            currentBurner = null;
        }
    }
}