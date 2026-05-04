using UnityEngine;

public class ButterController : MonoBehaviour
{
    [Header("黄油融化设置")]
    public float meltDuration = 10f;
    public bool isMelting = false;

    private Vector3 originalScale;
    private float meltTimer = 0f;

    private PanContainedItem panContainedItem;
    private PanHeatReceiver currentPanHeatReceiver;
    private bool hasAppliedButter = false;

    private void Awake()
    {
        panContainedItem = GetComponent<PanContainedItem>();
    }

    private void Start()
    {
        originalScale = transform.localScale;
    }

    private void Update()
    {
        if (panContainedItem == null) return;
        if (panContainedItem.CurrentPanZone == null) return;

        PanIngredientZone zone = panContainedItem.CurrentPanZone;
        currentPanHeatReceiver = zone.panHeatReceiver;

        if (!hasAppliedButter && zone.IsHeated && zone.SteaksInZone.Count > 0)
        {
            for (int i = 0; i < zone.SteaksInZone.Count; i++)
            {
                if (zone.SteaksInZone[i] != null)
                {
                    zone.SteaksInZone[i].HasButter = true;
                }
            }

            if (currentPanHeatReceiver != null)
            {
                currentPanHeatReceiver.HasButter = true;
            }

            hasAppliedButter = true;
            StartMelting();
        }

        if (isMelting)
        {
            UpdateMelting();
        }
    }

    void StartMelting()
    {
        if (isMelting) return;

        isMelting = true;
        meltTimer = 0f;
        Debug.Log("Butter started melting...");
    }

    void UpdateMelting()
    {
        meltTimer += Time.deltaTime;

        float progress = meltTimer / meltDuration;
        progress = Mathf.Clamp01(progress);

        float currentScaleFactor = Mathf.Lerp(1f, 0f, progress);
        transform.localScale = originalScale * currentScaleFactor;

        if (progress >= 1f)
        {
            CompleteMelting();
        }
    }

    void CompleteMelting()
    {
        isMelting = false;
        Debug.Log("Butter fully melted.");

        Destroy(gameObject, 0.1f);
    }

    public void ResetMelting()
    {
        isMelting = false;
        meltTimer = 0f;
        transform.localScale = originalScale;
    }
}