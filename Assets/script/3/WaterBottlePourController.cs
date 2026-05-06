using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class WaterBottlePourController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionAsset inputActionsAsset;
    [SerializeField] private string actionMapName = "Kitchen";
    [SerializeField] private string actionName = "PourWater";

    [Header("Pour Detection")]
    [SerializeField] private Transform pourPoint;
    [SerializeField] private float detectRadius = 0.2f;
    [SerializeField] private LayerMask cupLayers = ~0;
    [SerializeField] private float pourCooldown = 0.2f;

    [Header("Pour Timing")]
    [SerializeField] private float pourDuration = 1.5f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pourWaterClip;

    private InputAction pourAction;
    private bool isGrabbed = false;
    private bool isPouring = false;
    private float nextPourTime = 0f;

    private void Awake()
    {
        if (pourPoint == null)
        {
            pourPoint = transform;
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (inputActionsAsset != null)
        {
            InputActionMap actionMap = inputActionsAsset.FindActionMap(actionMapName, true);
            if (actionMap != null)
            {
                pourAction = actionMap.FindAction(actionName, true);
            }
        }
    }

    private void OnEnable()
    {
        if (pourAction != null)
        {
            pourAction.Enable();
        }
    }

    private void OnDisable()
    {
        if (pourAction != null)
        {
            pourAction.Disable();
        }
    }

    private void Update()
    {
        if (!isGrabbed) return;
        if (isPouring) return;
        if (pourAction == null) return;

        if (pourAction.WasPressedThisFrame())
        {
            TryPourWater();
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

    private void TryPourWater()
    {
        if (Time.time < nextPourTime) return;

        CupFillTarget targetCup = FindNearestEmptyCup();
        if (targetCup == null) return;

        StartCoroutine(PourRoutine(targetCup));
    }

    private IEnumerator PourRoutine(CupFillTarget targetCup)
    {
        isPouring = true;
        nextPourTime = Time.time + pourCooldown + pourDuration;

        if (audioSource != null)
        {
            audioSource.Stop();

            if (pourWaterClip != null)
            {
                audioSource.clip = pourWaterClip;
                audioSource.Play();
            }
        }

        yield return new WaitForSeconds(pourDuration);

        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        if (targetCup != null && !targetCup.IsFilled)
        {
            targetCup.FillCup();
            Debug.Log($"Water poured into {targetCup.gameObject.name}");
        }

        isPouring = false;
    }

    private CupFillTarget FindNearestEmptyCup()
    {
        Collider[] hits = Physics.OverlapSphere(
            pourPoint.position,
            detectRadius,
            cupLayers,
            QueryTriggerInteraction.Collide
        );

        CupFillTarget nearestCup = null;
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < hits.Length; i++)
        {
            CupFillTarget cup = hits[i].GetComponentInParent<CupFillTarget>();
            if (cup == null) continue;
            if (cup.IsFilled) continue;

            float distance = Vector3.Distance(pourPoint.position, cup.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestCup = cup;
            }
        }

        return nearestCup;
    }

    private void OnDrawGizmosSelected()
    {
        Transform point = pourPoint != null ? pourPoint : transform;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(point.position, detectRadius);
    }
}