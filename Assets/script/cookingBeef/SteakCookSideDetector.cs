using System.Collections.Generic;
using UnityEngine;

public class SteakCookSideDetector : MonoBehaviour
{
    public enum Side
    {
        Front,
        Back
    }

    [SerializeField] private Side side;

    private HashSet<CookHeatZoneBase> overlappingHeatZones = new HashSet<CookHeatZoneBase>();

    public bool IsTouchingHeatedZone()
    {
        overlappingHeatZones.RemoveWhere(zone => zone == null);

        foreach (CookHeatZoneBase zone in overlappingHeatZones)
        {
            if (zone != null && zone.IsActiveHeat)
            {
                return true;
            }
        }

        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        CookHeatZoneBase zone = other.GetComponent<CookHeatZoneBase>();
        if (zone != null)
        {
            overlappingHeatZones.Add(zone);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CookHeatZoneBase zone = other.GetComponent<CookHeatZoneBase>();
        if (zone != null)
        {
            overlappingHeatZones.Remove(zone);
        }
    }
}