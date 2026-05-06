using UnityEngine;

public class OilCheck : MonoBehaviour
{
    [Header("Oil Visual")]
    public GameObject oilVisual;

    [Header("Oil State")]
    public bool hasOil = false;

    private void Awake()
    {
        if (oilVisual == null)
        {
            oilVisual = gameObject;
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
    }

    public void CleanOil()
    {
        hasOil = false;

        if (oilVisual != null)
        {
            oilVisual.SetActive(false);
        }
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