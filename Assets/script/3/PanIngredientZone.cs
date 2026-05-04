using System.Collections.Generic;
using UnityEngine;

public class PanIngredientZone : MonoBehaviour
{
    public PanHeatReceiver panHeatReceiver;

    private List<SteakManager> steaksInZone = new List<SteakManager>();

    public bool IsHeated
    {
        get
        {
            return panHeatReceiver != null && panHeatReceiver.IsHeated;
        }
    }

    public List<SteakManager> SteaksInZone
    {
        get { return steaksInZone; }
    }

    private void Reset()
    {
        if (panHeatReceiver == null)
        {
            panHeatReceiver = GetComponentInParent<PanHeatReceiver>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        SteakManager steak = other.GetComponentInParent<SteakManager>();
        if (steak != null && !steaksInZone.Contains(steak))
        {
            steaksInZone.Add(steak);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        SteakManager steak = other.GetComponentInParent<SteakManager>();
        if (steak != null)
        {
            steaksInZone.Remove(steak);
        }
    }

    private void LateUpdate()
    {
        steaksInZone.RemoveAll(item => item == null);
    }
}