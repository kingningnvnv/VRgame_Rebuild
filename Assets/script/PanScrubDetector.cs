using UnityEngine;

public class PanScrubDetector : MonoBehaviour
{
    public PanWashState panWashState;

    [Header("Scrub Settings")]
    public float movementThreshold = 0.03f;
    public float progressPerScrub = 0.6f;
    public float scrubCooldown = 0.15f;

    [Header("Debug")]
    public bool showDebugLog = true;

    private Vector3 lastToolPosition;
    private bool hasLastPosition = false;
    private float lastScrubTime = 0f;

    private void Awake()
    {
        if (panWashState == null)
        {
            panWashState = GetComponentInParent<PanWashState>();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (panWashState == null) return;
        if (!panWashState.isInWater) return;

        CleaningToolMarker tool = other.GetComponentInParent<CleaningToolMarker>();
        if (tool == null) return;

        if (!tool.IsToolGrabbed())
        {
            if (showDebugLog)
            {
                Debug.Log("PanScrubDetector: Cleaning tool is not being held.");
            }
            return;
        }

        Vector3 currentPosition = other.transform.position;

        if (!hasLastPosition)
        {
            lastToolPosition = currentPosition;
            hasLastPosition = true;
            return;
        }

        float movement = Vector3.Distance(currentPosition, lastToolPosition);

        if (movement >= movementThreshold && Time.time - lastScrubTime >= scrubCooldown)
        {
            float amount = progressPerScrub * tool.cleanPower;
            panWashState.AddWashProgress(amount);

            lastToolPosition = currentPosition;
            lastScrubTime = Time.time;

            if (showDebugLog)
            {
                Debug.Log("PanScrubDetector: Scrubbing detected.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CleaningToolMarker tool = other.GetComponentInParent<CleaningToolMarker>();

        if (tool != null)
        {
            hasLastPosition = false;
        }
    }
}