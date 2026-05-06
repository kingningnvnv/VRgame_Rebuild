using UnityEngine;

public enum SeasoningType
{
    Salt,
    Pepper
}

public class SeasoningHit : MonoBehaviour
{
    [Header("Seasoning Type")]
    public SeasoningType seasoningType = SeasoningType.Salt;

    [Header("Stuck Visual")]
    public GameObject stuckVisualPrefab;
    public float stickOffset = 0.001f;
    public Vector3 stuckVisualScale = new Vector3(0.008f, 0.008f, 0.008f);

    [Header("Life")]
    public float selfDestroyAfter = 3f;

    private Vector3 previousPosition;
    private bool hasProcessedHit = false;

    private void Start()
    {
        previousPosition = transform.position;

        if (selfDestroyAfter > 0f)
        {
            Destroy(gameObject, selfDestroyAfter);
        }
    }

    private void FixedUpdate()
    {
        previousPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasProcessedHit) return;

        SteakManager steak = other.GetComponentInParent<SteakManager>();
        if (steak == null) return;

        hasProcessedHit = true;

        Vector3 hitPoint;
        Vector3 hitNormal;

        bool gotFrontFace = TryGetFrontFaceHit(other, out hitPoint, out hitNormal);

        if (!gotFrontFace)
        {
            Destroy(gameObject);
            return;
        }

        ApplySeasoningState(steak);
        SpawnStuckVisual(steak.transform, hitPoint, hitNormal);

        Destroy(gameObject);
    }

    private void ApplySeasoningState(SteakManager steak)
    {
        switch (seasoningType)
        {
            case SeasoningType.Salt:
                steak.RegisterCookingStep(SteakCookingStepType.Salt);
                break;

            case SeasoningType.Pepper:
                steak.RegisterCookingStep(SteakCookingStepType.Pepper);
                break;
        }
    }

    private bool TryGetFrontFaceHit(Collider targetCollider, out Vector3 hitPoint, out Vector3 hitNormal)
    {
        hitPoint = Vector3.zero;
        hitNormal = Vector3.up;

        Vector3 currentPosition = transform.position;
        Vector3 move = currentPosition - previousPosition;

        if (move.sqrMagnitude < 0.000001f)
        {
            return false;
        }

        Vector3 dir = move.normalized;
        float distance = move.magnitude + 0.03f;

        Vector3 rayStart = previousPosition - dir * 0.005f;
        Ray ray = new Ray(rayStart, dir);

        RaycastHit[] hits = Physics.RaycastAll(ray, distance);

        float bestDistance = float.MaxValue;
        bool found = false;

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];

            if (hit.collider != targetCollider) continue;

            float dot = Vector3.Dot(hit.normal, dir);
            if (dot >= 0f) continue;

            if (hit.distance < bestDistance)
            {
                bestDistance = hit.distance;
                hitPoint = hit.point;
                hitNormal = hit.normal;
                found = true;
            }
        }

        return found;
    }

    private void SpawnStuckVisual(Transform steakRoot, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (stuckVisualPrefab == null)
        {
            Debug.LogWarning("SeasoningHit: stuckVisualPrefab is not assigned.");
            return;
        }

        Transform marksRoot = GetOrCreateMarksRoot(steakRoot);

        Quaternion alignRotation = Quaternion.FromToRotation(Vector3.up, hitNormal);
        Quaternion randomAroundNormal = Quaternion.AngleAxis(Random.Range(0f, 360f), hitNormal);
        Quaternion finalRotation = randomAroundNormal * alignRotation;

        Vector3 spawnPos = hitPoint + hitNormal * stickOffset;

        GameObject mark = Instantiate(stuckVisualPrefab, spawnPos, finalRotation, marksRoot);
        mark.SetActive(true);
        mark.transform.localScale = stuckVisualScale;
    }

    private Transform GetOrCreateMarksRoot(Transform steakRoot)
    {
        Transform marksRoot = steakRoot.Find("SeasoningMarks");
        if (marksRoot == null)
        {
            GameObject root = new GameObject("SeasoningMarks");
            root.transform.SetParent(steakRoot);
            root.transform.localPosition = Vector3.zero;
            root.transform.localRotation = Quaternion.identity;
            root.transform.localScale = Vector3.one;
            marksRoot = root.transform;
        }

        return marksRoot;
    }
}