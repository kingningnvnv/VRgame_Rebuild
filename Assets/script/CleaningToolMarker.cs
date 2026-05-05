using UnityEngine;
using Oculus.Interaction;

public class CleaningToolMarker : MonoBehaviour
{
    [Header("Cleaning")]
    public float cleanPower = 1f;

    [Header("Grab State")]
    public bool requireToolGrabbed = true;
    public Grabbable toolGrabbable;

    private void Awake()
    {
        if (toolGrabbable == null)
        {
            toolGrabbable = GetComponent<Grabbable>();
        }

        if (toolGrabbable == null)
        {
            toolGrabbable = GetComponentInParent<Grabbable>();
        }
    }

    public bool IsToolGrabbed()
    {
        if (!requireToolGrabbed)
        {
            return true;
        }

        if (toolGrabbable == null)
        {
            return true;
        }

        return toolGrabbable.SelectingPointsCount > 0;
    }
}