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
    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.CompareTag("Steak"))
        {
            SteakManager manager = collision.gameObject.GetComponent<SteakManager>();
            manager.HasButter = HasButter;
            manager.HasOil = HasOil;
        }
    }
    public bool HasOil 
    {
        get => hasOil;
        set 
        {

            hasOil=value;
            if (hasOil)
            {
                Oil.gameObject.SetActive(true);
            }
            else { Oil.gameObject.SetActive(false); }
        }
    }
    public bool hasOil;
    public bool HasButter;
    public GameObject Oil;

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