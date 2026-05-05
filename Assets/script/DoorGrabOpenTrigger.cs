using UnityEngine;
using DoorScript;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;

public class DoorGrabOpenTrigger : MonoBehaviour
{
    [Header("Door")]
    public Door door;

    [Header("UI")]
    public GameObject doorHintCanvas;

    private Grabbable grabbable;
    private GrabInteractable grabInteractable;
    private HandGrabInteractable handGrabInteractable;
    private Rigidbody rb;

    private bool hasOpened = false;

    private void Awake()
    {
        grabbable = GetComponent<Grabbable>();
        grabInteractable = GetComponent<GrabInteractable>();
        handGrabInteractable = GetComponent<HandGrabInteractable>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (hasOpened) return;
        if (grabbable == null) return;

        if (grabbable.SelectingPointsCount > 0)
        {
            OpenDoorFromGrab();
        }
    }

    private void OpenDoorFromGrab()
    {
        if (hasOpened) return;

        hasOpened = true;

        if (doorHintCanvas != null)
        {
            doorHintCanvas.SetActive(false);
        }

        LockHandleImmediately();

        if (door != null)
        {
            door.OpenDoor();
        }
        else
        {
            Debug.LogError("DoorGrabOpenTrigger: Door reference is missing.");
        }
    }

    private void LockHandleImmediately()
    {
        if (grabInteractable != null)
        {
            grabInteractable.enabled = false;
        }

        if (handGrabInteractable != null)
        {
            handGrabInteractable.enabled = false;
        }

        if (grabbable != null)
        {
            grabbable.enabled = false;
        }

        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }
}