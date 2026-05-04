using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PanContainedItem : MonoBehaviour
{
    [Header("锅内稳定参数")]
    public float maxSpeedInPan = 0.35f;
    public float maxAngularSpeedInPan = 4f;
    public float holdDownForce = 2.5f;
    public float inPanDrag = 4f;
    public float inPanAngularDrag = 6f;

    private Rigidbody rb;
    private float originalDrag;
    private float originalAngularDrag;

    public PanIngredientZone CurrentPanZone { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        originalDrag = rb.linearDamping;
        originalAngularDrag = rb.angularDamping;
    }

    private void OnTriggerEnter(Collider other)
    {
        PanIngredientZone zone = other.GetComponent<PanIngredientZone>();
        if (zone == null)
        {
            zone = other.GetComponentInParent<PanIngredientZone>();
        }

        if (zone != null)
        {
            CurrentPanZone = zone;
            rb.linearDamping = inPanDrag;
            rb.angularDamping = inPanAngularDrag;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PanIngredientZone zone = other.GetComponent<PanIngredientZone>();
        if (zone == null)
        {
            zone = other.GetComponentInParent<PanIngredientZone>();
        }

        if (zone != null && zone == CurrentPanZone)
        {
            CurrentPanZone = null;
            rb.linearDamping = originalDrag;
            rb.angularDamping = originalAngularDrag;
        }
    }

    private void FixedUpdate()
    {
        if (CurrentPanZone == null) return;

        // 限制线速度
        if (rb.linearVelocity.magnitude > maxSpeedInPan)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeedInPan;
        }

        // 限制角速度
        if (rb.angularVelocity.magnitude > maxAngularSpeedInPan)
        {
            rb.angularVelocity = rb.angularVelocity.normalized * maxAngularSpeedInPan;
        }

        // 轻微往锅底方向压，减少被撞飞
        Vector3 pushDownDir = -CurrentPanZone.transform.up;
        rb.AddForce(pushDownDir * holdDownForce, ForceMode.Acceleration);
    }
}