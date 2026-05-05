using UnityEngine;

using UnityEngine;

public class OilCheck : MonoBehaviour
{
    private GameObject oilVisual;

    [Header("Oil State")]
    public bool hasOil = false;

    private void Awake()
    {
        if (transform.childCount > 0)
        {
            oilVisual = transform.GetChild(0).gameObject;
        }

        HideOil();
    }

    public void AddOil()
    {
        hasOil = true;

        if (oilVisual != null)
        {
            oilVisual.SetActive(true);
        }

        Debug.Log("OilCheck: Oil added.");
    }

    public void CleanOil()
    {
        hasOil = false;

        if (oilVisual != null)
        {
            oilVisual.SetActive(false);
        }

        Debug.Log("OilCheck: Oil cleaned.");
    }

    public bool HasOil()
    {
        return hasOil;
    }

    private void HideOil()
    {
        hasOil = false;

        if (oilVisual != null)
        {
            oilVisual.SetActive(false);
        }
    }
}