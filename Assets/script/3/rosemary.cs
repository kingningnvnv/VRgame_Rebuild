using UnityEngine;

public class rosemary : MonoBehaviour
{
    private PanContainedItem panContainedItem;
    private bool hasApplied = false;

    private void Awake()
    {
        panContainedItem = GetComponent<PanContainedItem>();
    }

    private void Update()
    {
        if (hasApplied) return;
        if (panContainedItem == null) return;
        if (panContainedItem.CurrentPanZone == null) return;

        PanIngredientZone zone = panContainedItem.CurrentPanZone;

        if (!zone.IsHeated) return;
        if (zone.SteaksInZone.Count == 0) return;

        for (int i = 0; i < zone.SteaksInZone.Count; i++)
        {
            if (zone.SteaksInZone[i] != null)
            {
                zone.SteaksInZone[i].HasRosemary = true;
            }
        }

        hasApplied = true;
        Debug.Log("Rosemary applied to steak in heated pan.");
    }
}