using UnityEngine;
using UnityEngine.InputSystem;

public class SprayOilInputController : MonoBehaviour
{
    [Header("Reference")]
    public SparyOil sparyOil;
    public InputActionReference sprayAction;

    [Header("Settings")]
    public float sprayCooldown = 0.15f;

    private bool isGrabbed = false;
    private float lastSprayTime = -999f;

    private void Awake()
    {
        if (sparyOil == null)
        {
            sparyOil = GetComponent<SparyOil>();
        }
    }

    private void OnEnable()
    {
        if (sprayAction != null && sprayAction.action != null)
        {
            sprayAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (sprayAction != null && sprayAction.action != null)
        {
            sprayAction.action.Disable();
        }
    }

    public void SetGrabbed()
    {
        isGrabbed = true;
    }

    public void SetReleased()
    {
        isGrabbed = false;
    }

    private void Update()
    {
        if (!isGrabbed) return;
        if (sparyOil == null) return;
        if (sprayAction == null || sprayAction.action == null) return;

        if (Time.time - lastSprayTime < sprayCooldown) return;

        if (sprayAction.action.WasPressedThisFrame())
        {
            sparyOil.Spary();
            lastSprayTime = Time.time;
        }
    }
}