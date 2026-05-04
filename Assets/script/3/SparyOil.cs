using UnityEngine;

public class SparyOil : MonoBehaviour
{
    public AudioClip sparyClip;
    public ParticleSystem par;

    [Header("射线检测设置")]
    public Transform rayOriginPoint; // 射线发射点
    public float rayDistance = 5f;   // 射线距离
    public LayerMask targetLayers = -1; // 目标图层（-1 表示所有图层）

    private void Start()
    {
        // 如果没有指定发射点，默认使用当前物体位置
        if (rayOriginPoint == null)
        {
            rayOriginPoint = transform;
        }
    }

    public void Spary()
    {
        // 播放音效
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null && sparyClip != null)
        {
            audioSource.PlayOneShot(sparyClip);
        }

        // 播放粒子效果
        if (par != null)
        {
            par.Play();
        }

        // 发射射线
        ShootRay();
    }

    void ShootRay()
    {
        if (rayOriginPoint == null) return;

        Vector3 origin = rayOriginPoint.position;
        Vector3 direction = rayOriginPoint.forward; // 使用发射点的 Forward 方向

        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, rayDistance, targetLayers))
        {
            Debug.Log($"射线命中: {hit.collider.gameObject.name}");
            Debug.Log(hit.collider.gameObject.name + hit.transform.parent);

            // 判断是否命中 Tag 为 Pan 的物体
            if (hit.collider.transform.CompareTag("Pan"))
            {
                Debug.Log("Spary " + hit.collider.transform.parent);

                // 获取 PanHeatReceiver 组件
                PanHeatReceiver panHeatReceiver = hit.collider.GetComponentInParent<PanHeatReceiver>();

                if (panHeatReceiver != null)
                {
                    Debug.Log("成功获取到 PanHeatReceiver 组件");
                    OnPanHit(panHeatReceiver, hit);
                }
                else
                {
                    Debug.LogWarning("命中了 Pan 物体，但没有找到 PanHeatReceiver 组件");
                }
            }
        }
        else
        {
            Debug.Log($"射线未命中任何物体，距离: {rayDistance}");
        }
    }

    // 命中 Pan 时的回调函数，可在这里扩展逻辑
    protected virtual void OnPanHit(PanHeatReceiver panHeatReceiver, RaycastHit hit)
    {
        Debug.Log($"喷油命中的物体: {hit.collider.gameObject.name}");
        panHeatReceiver.HasOil = true;

        // 例如：在这里播放喷油成功的效果
    }

    float sprayTime;

    private void Update()
    {
        // if (Time.time - sprayTime >= 1)
        // {
        //     sprayTime = Time.time;
        //     Spary();
        // }
    }

    // 在 Scene 视图中绘制射线，方便调试
    private void OnDrawGizmos()
    {
        if (rayOriginPoint != null)
        {
            Vector3 origin = rayOriginPoint.position;
            Vector3 direction = rayOriginPoint.forward;
            Vector3 endPoint = origin + direction * rayDistance;

            // 绘制主射线
            Gizmos.color = Color.red;
            Gizmos.DrawLine(origin, endPoint);

            // 绘制起点球
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(origin, 0.05f);

            // 绘制终点球
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(endPoint, 0.05f);

            // 绘制箭头
            Vector3 arrowStart = endPoint - direction * 0.2f;
            Vector3 right = Vector3.Cross(direction, Vector3.up).normalized;
            Vector3 up = Vector3.Cross(right, direction).normalized;
            Gizmos.DrawLine(endPoint, arrowStart + right * 0.1f);
            Gizmos.DrawLine(endPoint, arrowStart - right * 0.1f);
            Gizmos.DrawLine(endPoint, arrowStart + up * 0.1f);

            // 绘制蓝色 forward 指示线
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(origin, direction * 0.5f);
        }
    }

    // 选中物体时也显示 Gizmos
    private void OnDrawGizmosSelected()
    {
        OnDrawGizmos();
    }
}