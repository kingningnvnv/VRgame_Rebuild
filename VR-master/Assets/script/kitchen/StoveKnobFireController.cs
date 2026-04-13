using UnityEngine;

public class StoveKnobFireController : MonoBehaviour
{
    [Header("Fire")]
    [SerializeField] private GameObject fireOnObject;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip startClip;

    [Header("Angle Settings")]
    [SerializeField] private float onMinAngle = 75f;
    [SerializeField] private float onMaxAngle = 90f;

    // 为了防止在 75 度附近抖动来回开关，建议关火阈值比 75 小一点
    [SerializeField] private float offResetAngle = 60f;

    [Header("Options")]
    [SerializeField] private bool turnOffWhenRotateBack = true;

    private bool isFireOn = false;

    private void Start()
    {
        float currentZ = GetNormalizedZAngle();

        // 初始化状态，避免开场就误触发音效
        bool shouldBeOn = currentZ >= onMinAngle && currentZ <= onMaxAngle;
        isFireOn = shouldBeOn;

        if (fireOnObject != null)
        {
            fireOnObject.SetActive(shouldBeOn);
        }
    }

    private void Update()
    {
        float currentZ = GetNormalizedZAngle();

        // 进入开火区间：只触发一次
        if (!isFireOn && currentZ >= onMinAngle && currentZ <= onMaxAngle)
        {
            TurnOnFire();
        }

        // 转回去则关火，并允许下次再次触发
        if (turnOffWhenRotateBack && isFireOn && currentZ < offResetAngle)
        {
            TurnOffFire();
        }
    }

    private float GetNormalizedZAngle()
    {
        float z = transform.localEulerAngles.z;

        // 把角度统一成 0~360
        if (z < 0f)
            z += 360f;

        return z;
    }

    private void TurnOnFire()
    {
        isFireOn = true;

        if (fireOnObject != null)
        {
            fireOnObject.SetActive(true);
        }

        if (audioSource != null && startClip != null)
        {
            audioSource.PlayOneShot(startClip);
        }
    }

    private void TurnOffFire()
    {
        isFireOn = false;

        if (fireOnObject != null)
        {
            fireOnObject.SetActive(false);
        }
    }
}