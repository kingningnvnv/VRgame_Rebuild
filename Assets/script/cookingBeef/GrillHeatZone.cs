using UnityEngine;

public class GrillHeatZone : CookHeatZoneBase
{
    [Header("Knob References")]
    [SerializeField] private Transform leftKnob;
    [SerializeField] private Transform rightKnob;

    [Header("Active Angle Range (Z Axis)")]
    [SerializeField] private float activeMinAngle = 85f;
    [SerializeField] private float activeMaxAngle = 95f;

    [Header("Debug")]
    [SerializeField] private float leftKnobAngle;
    [SerializeField] private float rightKnobAngle;
    [SerializeField] private bool isHeatingDebug;

    public override bool IsActiveHeat
    {
        get
        {
            return IsKnobOn(leftKnob) || IsKnobOn(rightKnob);
        }
    }

    private void Update()
    {
        leftKnobAngle = leftKnob != null ? NormalizeAngle(leftKnob.localEulerAngles.z) : -1f;
        rightKnobAngle = rightKnob != null ? NormalizeAngle(rightKnob.localEulerAngles.z) : -1f;
        isHeatingDebug = IsActiveHeat;
    }

    private bool IsKnobOn(Transform knob)
    {
        if (knob == null) return false;

        float angle = NormalizeAngle(knob.localEulerAngles.z);
        return angle >= activeMinAngle && angle <= activeMaxAngle;
    }

    private float NormalizeAngle(float angle)
    {
        while (angle < 0f) angle += 360f;
        while (angle >= 360f) angle -= 360f;
        return angle;
    }
}