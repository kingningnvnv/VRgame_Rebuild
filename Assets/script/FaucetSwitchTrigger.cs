using UnityEngine;
using Oculus.Interaction;

public class FaucetGrabSwitch : MonoBehaviour
{
    [Header("References")]
    public FaucetController faucetController;
    public Grabbable switchGrabbable;

    [Header("Settings")]
    public float toggleCooldown = 1.0f;

    [Header("Debug")]
    public bool showDebugLog = true;

    private bool wasGrabbed = false;
    private float lastToggleTime = -999f;

    private void Awake()
    {
        if (switchGrabbable == null)
        {
            switchGrabbable = GetComponent<Grabbable>();
        }

        if (switchGrabbable == null)
        {
            Debug.LogWarning("FaucetGrabSwitch: No Grabbable found.");
        }
    }

    private void Update()
    {
        if (switchGrabbable == null || faucetController == null) return;

        bool isGrabbed = switchGrabbable.SelectingPointsCount > 0;

        if (isGrabbed && !wasGrabbed)
        {
            TryToggleWater();
        }

        wasGrabbed = isGrabbed;
    }

    private void TryToggleWater()
    {
        if (Time.time - lastToggleTime < toggleCooldown) return;

        lastToggleTime = Time.time;

        faucetController.ToggleWater();

        if (showDebugLog)
        {
            Debug.Log("FaucetGrabSwitch: Faucet toggled by grab.");
        }
    }
}