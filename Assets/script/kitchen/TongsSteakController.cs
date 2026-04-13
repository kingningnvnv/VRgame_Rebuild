using UnityEngine;
using UnityEngine.InputSystem;
using Oculus.Interaction;

public class TongsSteakController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform steakHoldPoint;
    [SerializeField] private GameObject hintUI;

    [Header("Input")]
    [SerializeField] private InputActionReference tongsToggleAction; // 绑定 B / Y

    private Rigidbody tongRb;
    private Collider[] tongColliders;

    private bool isGrabbed = false;

    private GameObject candidateSteak;
    private GameObject heldSteak;

    private Rigidbody heldSteakRb;
    private Grabbable heldSteakGrabbable;
    private PointableUnityEventWrapper heldSteakPointable;
    private Collider[] heldSteakColliders;

    private void Awake()
    {
        tongRb = GetComponent<Rigidbody>();
        tongColliders = GetComponentsInChildren<Collider>(true);
    }

    private void OnEnable()
    {
        if (tongsToggleAction != null && tongsToggleAction.action != null)
        {
            tongsToggleAction.action.performed += OnTongsTogglePerformed;
            tongsToggleAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (tongsToggleAction != null && tongsToggleAction.action != null)
        {
            tongsToggleAction.action.performed -= OnTongsTogglePerformed;
        }
    }

    private void OnTongsTogglePerformed(InputAction.CallbackContext context)
    {
        if (!isGrabbed)
            return;

        if (heldSteak != null)
        {
            ReleaseSteak();
        }
        else if (candidateSteak != null)
        {
            AttachSteak(candidateSteak);
        }

        UpdateHint();
    }

    // 绑定到 Tong_grab 的 Pointable Unity Event Wrapper -> When Select
    public void OnTongSelected()
    {
        isGrabbed = true;
        UpdateHint();
    }

    // 绑定到 Tong_grab 的 Pointable Unity Event Wrapper -> When Unselect
    public void OnTongUnselected()
    {
        isGrabbed = false;

        if (heldSteak != null)
        {
            ReleaseSteak();
        }

        UpdateHint();
    }

    public void SetCandidate(GameObject steak)
    {
        if (!isGrabbed) return;
        if (heldSteak != null) return;

        candidateSteak = steak;
        UpdateHint();
    }

    public void ClearCandidate(GameObject steak)
    {
        if (candidateSteak == steak)
        {
            candidateSteak = null;
        }

        UpdateHint();
    }

    private void AttachSteak(GameObject steak)
    {
        heldSteak = steak;
        heldSteakRb = steak.GetComponent<Rigidbody>();
        heldSteakGrabbable = steak.GetComponent<Grabbable>();
        heldSteakPointable = steak.GetComponent<PointableUnityEventWrapper>();
        heldSteakColliders = steak.GetComponentsInChildren<Collider>(true);

        if (heldSteakRb != null)
        {
            heldSteakRb.linearVelocity = Vector3.zero;
            heldSteakRb.angularVelocity = Vector3.zero;
            heldSteakRb.useGravity = false;
            heldSteakRb.isKinematic = true;
        }

        // 被夹住时，暂时不允许玩家再次直接抓牛排
        if (heldSteakGrabbable != null)
            heldSteakGrabbable.enabled = false;

        if (heldSteakPointable != null)
            heldSteakPointable.enabled = false;

        // 忽略夹子与牛排之间的碰撞，避免抖动
        if (heldSteakColliders != null && tongColliders != null)
        {
            foreach (var tongCol in tongColliders)
            {
                foreach (var steakCol in heldSteakColliders)
                {
                    if (tongCol != null && steakCol != null)
                    {
                        Physics.IgnoreCollision(tongCol, steakCol, true);
                    }
                }
            }
        }

        heldSteak.transform.SetParent(steakHoldPoint);
        heldSteak.transform.localPosition = Vector3.zero;
        heldSteak.transform.localRotation = Quaternion.identity;

        candidateSteak = null;
        UpdateHint();
    }

    private void ReleaseSteak()
    {
        if (heldSteak == null) return;

        heldSteak.transform.SetParent(null, true);

        // 恢复夹子与牛排碰撞
        if (heldSteakColliders != null && tongColliders != null)
        {
            foreach (var tongCol in tongColliders)
            {
                foreach (var steakCol in heldSteakColliders)
                {
                    if (tongCol != null && steakCol != null)
                    {
                        Physics.IgnoreCollision(tongCol, steakCol, false);
                    }
                }
            }
        }

        if (heldSteakRb != null)
        {
            heldSteakRb.isKinematic = false;
            heldSteakRb.useGravity = true;

            if (tongRb != null)
            {
                heldSteakRb.linearVelocity = tongRb.linearVelocity;
                heldSteakRb.angularVelocity = tongRb.angularVelocity;
            }
        }

        if (heldSteakGrabbable != null)
            heldSteakGrabbable.enabled = true;

        if (heldSteakPointable != null)
            heldSteakPointable.enabled = true;

        heldSteak = null;
        heldSteakRb = null;
        heldSteakGrabbable = null;
        heldSteakPointable = null;
        heldSteakColliders = null;

        UpdateHint();
    }

    private void UpdateHint()
    {
        if (hintUI == null) return;

        bool show = isGrabbed && (candidateSteak != null || heldSteak != null);
        hintUI.SetActive(show);
    }
}