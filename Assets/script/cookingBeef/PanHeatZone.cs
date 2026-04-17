using UnityEngine;

public class PanHeatZone : CookHeatZoneBase
{
    [SerializeField] private PanHeatReceiver panHeatReceiver;

    public override bool IsActiveHeat
    {
        get
        {
            return panHeatReceiver != null && panHeatReceiver.IsHeated;
        }
    }
}