using UnityEngine;
using Oculus.Interaction;

public class PanWashState : MonoBehaviour
{
    [Header("References")]
    public OilCheck oilCheck;
    public Grabbable panGrabbable;

    [Header("Water State")]
    public bool isInWater = false;

    [Header("Wash Progress")]
    public float washProgress = 0f;
    public float requiredWashProgress = 5f;

    [Header("Grab Requirement")]
    public bool requirePanGrabbed = true;

    [Header("Debug")]
    public bool showDebugLog = true;

    public bool IsWashed
    {
        get
        {
            if (oilCheck == null) return false;
            return !oilCheck.HasOil();
        }
    }

    private void Awake()
    {
        if (oilCheck == null)
        {
            oilCheck = GetComponentInChildren<OilCheck>();
        }

        if (panGrabbable == null)
        {
            panGrabbable = GetComponent<Grabbable>();
        }

        if (oilCheck == null)
        {
            Debug.LogWarning("PanWashState: No OilCheck found.");
        }

        if (panGrabbable == null)
        {
            Debug.LogWarning("PanWashState: No Grabbable found on pan.");
        }
    }

    public void SetInWater(bool state)
    {
        isInWater = state;

        if (showDebugLog)
        {
            Debug.Log("PanWashState: isInWater = " + isInWater);
        }
    }

    public bool IsPanGrabbed()
    {
        if (!requirePanGrabbed) return true;
        if (panGrabbable == null) return true;

        return panGrabbable.SelectingPointsCount > 0;
    }

    public void AddWashProgress(float amount)
    {
        if (!isInWater)
        {
            if (showDebugLog)
            {
                Debug.Log("PanWashState: Cannot wash. Pan is not in water.");
            }
            return;
        }

        if (!IsPanGrabbed())
        {
            if (showDebugLog)
            {
                Debug.Log("PanWashState: Cannot wash. Pan is not being held.");
            }
            return;
        }

        if (oilCheck == null)
        {
            if (showDebugLog)
            {
                Debug.LogWarning("PanWashState: Cannot wash. OilCheck missing.");
            }
            return;
        }

        if (!oilCheck.HasOil())
        {
            if (showDebugLog)
            {
                Debug.Log("PanWashState: Cannot wash. No oil in pan.");
            }
            return;
        }

        washProgress += amount;
        washProgress = Mathf.Clamp(washProgress, 0f, requiredWashProgress);

        if (showDebugLog)
        {
            Debug.Log("PanWashState: Wash progress = " + washProgress + " / " + requiredWashProgress);
        }

        if (washProgress >= requiredWashProgress)
        {
            CompleteWash();
        }
    }

    private void CompleteWash()
    {
        if (oilCheck != null)
        {
            oilCheck.CleanOil();
        }

        washProgress = 0f;

        if (showDebugLog)
        {
            Debug.Log("PanWashState: Pan washing completed.");
        }
    }

    public void ResetWashProgress()
    {
        washProgress = 0f;
    }
}