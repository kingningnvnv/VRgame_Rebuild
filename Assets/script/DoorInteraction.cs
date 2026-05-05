using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    [Header("Door Pivot")]
    public Transform doorPivot;

    [Header("Door Settings")]
    public float openAngle = 90f;
    public float openSpeed = 2f;

    private bool isOpening = false;
    private bool hasOpened = false;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    private void Start()
    {
        if (doorPivot == null)
        {
            doorPivot = transform;
        }

        closedRotation = doorPivot.rotation;
        openRotation = closedRotation * Quaternion.Euler(0f, openAngle, 0f);
    }

    private void Update()
    {
        if (!isOpening) return;

        doorPivot.rotation = Quaternion.Lerp(
            doorPivot.rotation,
            openRotation,
            Time.deltaTime * openSpeed
        );

        if (Quaternion.Angle(doorPivot.rotation, openRotation) < 1f)
        {
            doorPivot.rotation = openRotation;
            isOpening = false;
            hasOpened = true;
        }
    }

    public void OpenDoor()
    {
        if (hasOpened) return;

        isOpening = true;
    }
}