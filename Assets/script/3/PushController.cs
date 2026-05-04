using System.Collections;
using UnityEngine;

public class PushController : MonoBehaviour
{
    [Header("抓取限制")]
    public bool onlySprayWhenGrabbed = true; // 只有被抓住时才允许喷射

    [Header("摇晃检测")]
    public float speedThreshold = 140f; // 角速度阈值（度/秒），超过才算一次有效摇晃
    public float resetThreshold = 50f;  // 角速度回落阈值（度/秒），低于此值才允许下一次喷射
    public float checkInterval = 0.03f; // 检测间隔

    [Header("朝向限制")]
    [Range(-1f, 1f)]
    public float downwardDotThreshold = 0.45f; // 喷口朝下判定，越大越严格

    [Header("撒料喷射设置")]
    public GameObject pepperPrefab; // 颗粒预制体（你现在实际用的是 salt）
    public Transform nozzlePoint;   // 喷口位置
    public int sprayCount = 30;     // 每次喷出的颗粒数量
    public float sprayRadius = 0.02f; // 喷口附近随机散布半径

    [Header("力施加")]
    public float forceMin = 5f;
    public float forceMax = 15f;
    public float horizontalSpread = 30f;
    public float verticalSpread = 20f;
    public ForceMode forceMode = ForceMode.Impulse;

    [Header("喷射节奏")]
    public float sprayCooldown = 1f; // 每次喷射最少间隔 1 秒

    [Header("音效")]
    public AudioClip ShakeAudio;
    public float audioDuration = 1f; // 每次音效最多播放 1 秒

    [Header("调试")]
    public bool showDebugLog = false;

    private bool isGrabbed = false;
    private bool readyForNextShake = true;
    private float lastSprayTime = -999f;

    private Quaternion lastRotation;
    private float lastCheckTime;

    private AudioSource source;
    private Coroutine audioCoroutine;

    void Start()
    {
        lastRotation = transform.rotation;
        lastCheckTime = Time.time;

        source = GetComponent<AudioSource>();
        if (source == null)
        {
            source = gameObject.AddComponent<AudioSource>();
        }

        // 如果没有指定喷口位置，则自动创建一个默认喷口点
        if (nozzlePoint == null)
        {
            GameObject nozzle = new GameObject("NozzlePoint");
            nozzle.transform.SetParent(transform);
            nozzle.transform.localPosition = Vector3.up * 0.08f;
            nozzle.transform.localRotation = Quaternion.identity;
            nozzlePoint = nozzle.transform;

            Debug.LogWarning("未指定喷口位置，已自动创建默认喷口点。");
        }
    }

    void FixedUpdate()
    {
        // 必须先被抓住
        if (onlySprayWhenGrabbed && !isGrabbed)
        {
            ResetMotionTracking();
            return;
        }

        if (nozzlePoint == null)
        {
            ResetMotionTracking();
            return;
        }

        if (Time.time - lastCheckTime < checkInterval)
        {
            return;
        }

        float dt = Time.time - lastCheckTime;
        if (dt <= 0f)
        {
            ResetMotionTracking();
            return;
        }

        // 用旋转变化来判断“摇晃”
        float angleDelta = Quaternion.Angle(lastRotation, transform.rotation);
        float angularSpeed = angleDelta / dt; // 度 / 秒

        lastRotation = transform.rotation;
        lastCheckTime = Time.time;

        // 必须喷口朝下
        if (!IsNozzleFacingDown())
        {
            // 朝向不对时，只在动作缓下来后重新上膛
            if (angularSpeed < resetThreshold)
            {
                readyForNextShake = true;
            }
            return;
        }

        // 晃动已经回落，允许记录下一次摇晃
        if (angularSpeed < resetThreshold)
        {
            readyForNextShake = true;
            return;
        }

        // 还没回落，不允许连着疯狂触发
        if (!readyForNextShake)
        {
            return;
        }

        // 角速度没达到阈值，不算一次有效摇晃
        if (angularSpeed < speedThreshold)
        {
            return;
        }

        // 冷却还没到
        if (Time.time - lastSprayTime < sprayCooldown)
        {
            return;
        }

        // 满足条件：喷一次
        SprayPepper();
        lastSprayTime = Time.time;
        readyForNextShake = false;

        if (showDebugLog)
        {
            Debug.Log($"Shake detected. Angular Speed = {angularSpeed:F2}");
        }
    }

    public void SprayPepper()
    {
        if (pepperPrefab == null)
        {
            Debug.LogError("颗粒预制体未赋值，请在 Inspector 中设置 pepperPrefab。");
            return;
        }

        if (nozzlePoint == null) return;

        PlayShakeAudioForOneSecond();

        for (int i = 0; i < sprayCount; i++)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-sprayRadius, sprayRadius),
                Random.Range(-sprayRadius * 0.5f, sprayRadius * 0.5f),
                Random.Range(-sprayRadius, sprayRadius)
            );

            Vector3 spawnPosition = nozzlePoint.position + randomOffset;

            GameObject pepper = Instantiate(pepperPrefab, spawnPosition, Quaternion.identity);
            pepper.SetActive(true);
            pepper.transform.rotation = Random.rotation;

            Rigidbody rb = pepper.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = pepper.AddComponent<Rigidbody>();
            }

            Vector3 sprayDirection = GetSprayDirection();
            float randomForce = Random.Range(forceMin, forceMax);

            rb.AddForce(sprayDirection * randomForce, forceMode);

            Vector3 randomTorque = new Vector3(
                Random.Range(-8f, 8f),
                Random.Range(-8f, 8f),
                Random.Range(-8f, 8f)
            );
            rb.AddTorque(randomTorque, ForceMode.Impulse);
        }

        if (showDebugLog)
        {
            Debug.Log($"Spray success. Spawn Count = {sprayCount}");
        }
    }

    Vector3 GetSprayDirection()
    {
        if (nozzlePoint == null) return Vector3.down;

        // 以 nozzlePoint.up 为中心方向，做一个向外散开的随机喷射
        Quaternion spreadRotation =
            Quaternion.AngleAxis(Random.Range(-horizontalSpread, horizontalSpread), nozzlePoint.forward) *
            Quaternion.AngleAxis(Random.Range(-verticalSpread, verticalSpread), nozzlePoint.right);

        Vector3 direction = spreadRotation * nozzlePoint.up;
        return direction.normalized;
    }

    bool IsNozzleFacingDown()
    {
        if (nozzlePoint == null) return false;

        // nozzlePoint.up 越接近世界向下，值越接近 1
        float dot = Vector3.Dot(nozzlePoint.up.normalized, Vector3.down);
        return dot >= downwardDotThreshold;
    }

    void PlayShakeAudioForOneSecond()
    {
        if (source == null || ShakeAudio == null) return;

        source.clip = ShakeAudio;
        source.time = 0f;
        source.Play();

        if (audioCoroutine != null)
        {
            StopCoroutine(audioCoroutine);
        }

        audioCoroutine = StartCoroutine(StopAudioAfterDelay(audioDuration));
    }

    IEnumerator StopAudioAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (source != null && source.isPlaying)
        {
            source.Stop();
        }

        audioCoroutine = null;
    }

    void ResetMotionTracking()
    {
        lastRotation = transform.rotation;
        lastCheckTime = Time.time;
        readyForNextShake = true;
    }

    public void SetGrabbed()
    {
        isGrabbed = true;
        ResetMotionTracking();

        if (showDebugLog)
        {
            Debug.Log("Salt grabbed");
        }
    }

    public void SetReleased()
    {
        isGrabbed = false;
        ResetMotionTracking();

        if (showDebugLog)
        {
            Debug.Log("Salt released");
        }
    }

    public void ManualSpray()
    {
        SprayPepper();
    }

    public void ResetSpraying()
    {
        readyForNextShake = true;
        lastSprayTime = -999f;
    }

    void OnDrawGizmosSelected()
    {
        if (nozzlePoint != null)
        {
            // 喷口位置
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(nozzlePoint.position, 0.02f);

            // 当前喷射方向（nozzlePoint.up）
            Gizmos.color = Color.red;
            Gizmos.DrawRay(nozzlePoint.position, nozzlePoint.up * 0.2f);

            // 世界向下辅助线
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(nozzlePoint.position, Vector3.down * 0.2f);

            // 随机生成范围
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(nozzlePoint.position, new Vector3(sprayRadius * 2, sprayRadius, sprayRadius * 2));
        }
    }
}