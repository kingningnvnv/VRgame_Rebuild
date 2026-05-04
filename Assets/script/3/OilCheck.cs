using UnityEngine;

public class OilCheck : MonoBehaviour
{
    PanHeatReceiver pan;
    GameObject par;
    private void Awake()
    {
        pan = this.GetComponentInParent<PanHeatReceiver>();
        par = this.transform.GetChild(0).gameObject;
    }
    // Update is called once per frame
    void Update()
    {
        if (pan != null)
        {

            if (pan.IsHeated) 
            {
                par.SetActive(true);
            }
            else
            {
                par.SetActive(false);
            }
        }
    }
}
