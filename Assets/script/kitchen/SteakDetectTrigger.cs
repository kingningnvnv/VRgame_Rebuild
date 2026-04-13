using UnityEngine;

public class SteakDetectTrigger : MonoBehaviour
{
    public TongsSteakController controller;

    private void OnTriggerEnter(Collider other)
    {
        GameObject target = other.attachedRigidbody != null
            ? other.attachedRigidbody.gameObject
            : other.gameObject;

        if (target.CompareTag("Steak"))
        {
            controller.SetCandidate(target);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject target = other.attachedRigidbody != null
            ? other.attachedRigidbody.gameObject
            : other.gameObject;

        if (target.CompareTag("Steak"))
        {
            controller.ClearCandidate(target);
        }
    }
}