using UnityEngine;

public class DoorTriggerZone : MonoBehaviour
{
    public GameObject doorHintCanvas;

    private void Start()
    {
        if (doorHintCanvas != null)
        {
            doorHintCanvas.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (doorHintCanvas != null)
            {
                doorHintCanvas.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (doorHintCanvas != null)
            {
                doorHintCanvas.SetActive(false);
            }
        }
    }
}