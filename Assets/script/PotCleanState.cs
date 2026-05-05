using UnityEngine;

public class PotCleanState : MonoBehaviour
{
    [Header("Clean State")]
    public bool isDirty = true;
    public bool isInWater = false;
    public float cleanProgress = 0f;
    public float requiredCleanProgress = 5f;

    [Header("Visual")]
    public GameObject dirtyVisual;
    public GameObject cleanVisual;

    public bool IsClean => cleanProgress >= requiredCleanProgress;

    private void Start()
    {
        UpdateVisual();
    }

    public void SetInWater(bool state)
    {
        isInWater = state;
    }

    public void AddCleanProgress(float amount)
    {
        if (!isDirty) return;
        if (!isInWater) return;

        cleanProgress += amount;
        cleanProgress = Mathf.Clamp(cleanProgress, 0f, requiredCleanProgress);

        if (cleanProgress >= requiredCleanProgress)
        {
            isDirty = false;
        }

        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (dirtyVisual != null)
        {
            dirtyVisual.SetActive(isDirty);
        }

        if (cleanVisual != null)
        {
            cleanVisual.SetActive(!isDirty);
        }
    }
}