using UnityEngine;

public class Salt : MonoBehaviour
{
    [Header("Stuck Visual")]
    public GameObject stuckSaltPrefab;   // 附着在牛排表面的盐粒视觉 prefab
    public float stickOffset = 0.0015f;  // 贴表面时往外抬一点，避免Z-fighting
    public float selfDestroyAfter = 3f;  // 飞行盐粒最长存活时间
    public float randomScaleMin = 0.85f;
    public float randomScaleMax = 1.15f;

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

        SteakManager steakManager = other.GetComponentInParent<SteakManager>();
        if (steakManager == null) return;

        hasProcessedHit = true;

        Vector3 hitPoint;
        Vector3 hitNormal;
        GetSurfacePointAndNormal(other, out hitPoint, out hitNormal);

        // 标记牛排已被撒盐
        steakManager.HasSalt = true;

        // 在命中的那一面生成“附着盐粒视觉”
        SpawnStuckSalt(steakManager.transform, hitPoint, hitNormal);

        // 销毁飞行中的盐粒
        Destroy(gameObject);
    }

    private void GetSurfacePointAndNormal(Collider targetCollider, out Vector3 hitPoint, out Vector3 hitNormal)
    {
        Vector3 currentPosition = transform.position;
        Vector3 moveDir = currentPosition - previousPosition;
        float moveDistance = moveDir.magnitude;

        // 优先用“上一帧位置 -> 当前帧位置”的线段去射这个 collider，
        // 得到真正进入的表面点，避免穿过去后出现在背面
        if (moveDistance > 0.0001f)
        {
            Ray ray = new Ray(previousPosition, moveDir.normalized);
            RaycastHit hit;

            if (targetCollider.Raycast(ray, out hit, moveDistance + 0.05f))
            {
                hitPoint = hit.point;
                hitNormal = hit.normal;
                return;
            }
        }

        // 射不到时，用最近点兜底
        hitPoint = targetCollider.ClosestPoint(transform.position);

        Vector3 approxNormal = transform.position - hitPoint;
        if (approxNormal.sqrMagnitude < 0.0001f)
        {
            approxNormal = -transform.forward;
        }

        hitNormal = approxNormal.normalized;
    }

    private void SpawnStuckSalt(Transform steakRoot, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (stuckSaltPrefab == null)
        {
            Debug.LogWarning("Salt: stuckSaltPrefab 未赋值，无法生成附着盐粒视觉。");
            return;
        }

        Transform marksRoot = GetOrCreateMarksRoot(steakRoot);

        // 让附着物的 local up 对齐表面法线
        Quaternion alignRotation = Quaternion.FromToRotation(Vector3.up, hitNormal);
        Quaternion randomAroundNormal = Quaternion.AngleAxis(Random.Range(0f, 360f), hitNormal);
        Quaternion finalRotation = randomAroundNormal * alignRotation;

        Vector3 spawnPos = hitPoint + hitNormal * stickOffset;

        GameObject mark = Instantiate(stuckSaltPrefab, spawnPos, finalRotation, marksRoot);

        float randomScale = Random.Range(randomScaleMin, randomScaleMax);
        mark.transform.localScale *= randomScale;
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