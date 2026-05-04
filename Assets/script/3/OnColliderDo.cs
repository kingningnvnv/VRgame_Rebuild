using UnityEngine;

public class OnColliderDo : MonoBehaviour
{
    public GameObject Show;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            Show.gameObject.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Show.gameObject.SetActive(false);
        }
    }
}
