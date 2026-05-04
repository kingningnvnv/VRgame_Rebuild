using UnityEngine;

public class SteakCookController : MonoBehaviour
{
    public enum CookStage
    {
        生,
        三,
        五,
        熟,
        焦
    }

    [Header("Detectors")]
    [SerializeField] private SteakCookSideDetector frontDetector;
    [SerializeField] private SteakCookSideDetector backDetector;

    [Header("Audio")]
    [SerializeField] private AudioSource sizzleAudioSource;

    [Header("Cook Settings")]
    [SerializeField] private float secondsPerStage = 10f;

    [Header("Cook Time Debug")]
    [SerializeField] private float frontCookTime = 0f;
    [SerializeField] private float backCookTime = 0f;

    [Header("Cook State Debug")]
    [SerializeField] private CookStage frontStage = CookStage.生;
    [SerializeField] private CookStage backStage = CookStage.生;

    [Header("Visual States: 正生")]
    [SerializeField] private GameObject 正生反生;
    [SerializeField] private GameObject 正生反三;
    [SerializeField] private GameObject 正生反五;
    [SerializeField] private GameObject 正生反熟;
    [SerializeField] private GameObject 正生反焦;

    [Header("Visual States: 正三")]
    [SerializeField] private GameObject 正三反生;
    [SerializeField] private GameObject 正三反三;
    [SerializeField] private GameObject 正三反五;
    [SerializeField] private GameObject 正三反熟;
    [SerializeField] private GameObject 正三反焦;

    [Header("Visual States: 正五")]
    [SerializeField] private GameObject 正五反生;
    [SerializeField] private GameObject 正五反三;
    [SerializeField] private GameObject 正五反五;
    [SerializeField] private GameObject 正五反熟;
    [SerializeField] private GameObject 正五反焦;

    [Header("Visual States: 正熟")]
    [SerializeField] private GameObject 正熟反生;
    [SerializeField] private GameObject 正熟反三;
    [SerializeField] private GameObject 正熟反五;
    [SerializeField] private GameObject 正熟反熟;
    [SerializeField] private GameObject 正熟反焦;

    [Header("Visual States: 正焦")]
    [SerializeField] private GameObject 正焦反生;
    [SerializeField] private GameObject 正焦反三;
    [SerializeField] private GameObject 正焦反五;
    [SerializeField] private GameObject 正焦反熟;
    [SerializeField] private GameObject 正焦反焦;

    private CookStage lastFrontStage;
    private CookStage lastBackStage;

    private void Start()
    {
        frontStage = CalculateStage(frontCookTime);
        backStage = CalculateStage(backCookTime);

        lastFrontStage = frontStage;
        lastBackStage = backStage;

        UpdateVisualState();
        UpdateSizzleAudio(false);

        SteakManager steakManager = GetComponent<SteakManager>();
        if (steakManager == null)
        {
            gameObject.AddComponent<SteakManager>();
        }
    }

    private void Update()
    {
        bool isFrontTouchingHeatedZone = frontDetector != null && frontDetector.IsTouchingHeatedZone();
        bool isBackTouchingHeatedZone = backDetector != null && backDetector.IsTouchingHeatedZone();

        bool isCooking = false;

        if (isFrontTouchingHeatedZone && !isBackTouchingHeatedZone)
        {
            frontCookTime += Time.deltaTime;
            isCooking = true;
        }
        else if (isBackTouchingHeatedZone && !isFrontTouchingHeatedZone)
        {
            backCookTime += Time.deltaTime;
            isCooking = true;
        }

        UpdateSizzleAudio(isCooking);

        frontStage = CalculateStage(frontCookTime);
        backStage = CalculateStage(backCookTime);

        if (frontStage != lastFrontStage || backStage != lastBackStage)
        {
            UpdateVisualState();
            lastFrontStage = frontStage;
            lastBackStage = backStage;
        }
    }

    private CookStage CalculateStage(float timeValue)
    {
        if (timeValue < secondsPerStage) return CookStage.生;
        if (timeValue < secondsPerStage * 2f) return CookStage.三;
        if (timeValue < secondsPerStage * 3f) return CookStage.五;
        if (timeValue < secondsPerStage * 4f) return CookStage.熟;
        return CookStage.焦;
    }

    private void UpdateSizzleAudio(bool shouldPlay)
    {
        if (sizzleAudioSource == null) return;

        if (shouldPlay)
        {
            if (!sizzleAudioSource.isPlaying)
            {
                sizzleAudioSource.Play();
            }
        }
        else
        {
            if (sizzleAudioSource.isPlaying)
            {
                sizzleAudioSource.Stop();
            }
        }
    }

    private void UpdateVisualState()
    {
        DisableAllVisuals();

        GameObject targetVisual = GetCurrentVisual(frontStage, backStage);
        if (targetVisual != null)
        {
            targetVisual.SetActive(true);
        }
    }

    private void DisableAllVisuals()
    {
        SetVisualActive(正生反生, false);
        SetVisualActive(正生反三, false);
        SetVisualActive(正生反五, false);
        SetVisualActive(正生反熟, false);
        SetVisualActive(正生反焦, false);

        SetVisualActive(正三反生, false);
        SetVisualActive(正三反三, false);
        SetVisualActive(正三反五, false);
        SetVisualActive(正三反熟, false);
        SetVisualActive(正三反焦, false);

        SetVisualActive(正五反生, false);
        SetVisualActive(正五反三, false);
        SetVisualActive(正五反五, false);
        SetVisualActive(正五反熟, false);
        SetVisualActive(正五反焦, false);

        SetVisualActive(正熟反生, false);
        SetVisualActive(正熟反三, false);
        SetVisualActive(正熟反五, false);
        SetVisualActive(正熟反熟, false);
        SetVisualActive(正熟反焦, false);

        SetVisualActive(正焦反生, false);
        SetVisualActive(正焦反三, false);
        SetVisualActive(正焦反五, false);
        SetVisualActive(正焦反熟, false);
        SetVisualActive(正焦反焦, false);
    }

    private void SetVisualActive(GameObject visualObject, bool isActive)
    {
        if (visualObject != null)
        {
            visualObject.SetActive(isActive);
        }
    }

    private GameObject GetCurrentVisual(CookStage front, CookStage back)
    {
        if (front == CookStage.生 && back == CookStage.生) return 正生反生;
        if (front == CookStage.生 && back == CookStage.三) return 正生反三;
        if (front == CookStage.生 && back == CookStage.五) return 正生反五;
        if (front == CookStage.生 && back == CookStage.熟) return 正生反熟;
        if (front == CookStage.生 && back == CookStage.焦) return 正生反焦;

        if (front == CookStage.三 && back == CookStage.生) return 正三反生;
        if (front == CookStage.三 && back == CookStage.三) return 正三反三;
        if (front == CookStage.三 && back == CookStage.五) return 正三反五;
        if (front == CookStage.三 && back == CookStage.熟) return 正三反熟;
        if (front == CookStage.三 && back == CookStage.焦) return 正三反焦;

        if (front == CookStage.五 && back == CookStage.生) return 正五反生;
        if (front == CookStage.五 && back == CookStage.三) return 正五反三;
        if (front == CookStage.五 && back == CookStage.五) return 正五反五;
        if (front == CookStage.五 && back == CookStage.熟) return 正五反熟;
        if (front == CookStage.五 && back == CookStage.焦) return 正五反焦;

        if (front == CookStage.熟 && back == CookStage.生) return 正熟反生;
        if (front == CookStage.熟 && back == CookStage.三) return 正熟反三;
        if (front == CookStage.熟 && back == CookStage.五) return 正熟反五;
        if (front == CookStage.熟 && back == CookStage.熟) return 正熟反熟;
        if (front == CookStage.熟 && back == CookStage.焦) return 正熟反焦;

        if (front == CookStage.焦 && back == CookStage.生) return 正焦反生;
        if (front == CookStage.焦 && back == CookStage.三) return 正焦反三;
        if (front == CookStage.焦 && back == CookStage.五) return 正焦反五;
        if (front == CookStage.焦 && back == CookStage.熟) return 正焦反熟;
        if (front == CookStage.焦 && back == CookStage.焦) return 正焦反焦;

        return null;
    }

    public int GetCookStageValue(CookStage stage)
    {
        switch (stage)
        {
            case CookStage.生:
                return 0;

            case CookStage.三:
                return 1;

            case CookStage.五:
                return 2;

            case CookStage.熟:
                return 3;

            case CookStage.焦:
                return 4;

            default:
                return 0;
        }
    }

    public CookStage GetOverallCookStage()
    {
        float average = (GetCookStageValue(frontStage) + GetCookStageValue(backStage)) / 2f;
        int rounded = Mathf.RoundToInt(average);

        switch (rounded)
        {
            case 0:
                return CookStage.生;

            case 1:
                return CookStage.三;

            case 2:
                return CookStage.五;

            case 3:
                return CookStage.熟;

            case 4:
                return CookStage.焦;

            default:
                return CookStage.生;
        }
    }

    public float GetDonenessCoefficient()
    {
        CookStage overallStage = GetOverallCookStage();

        switch (overallStage)
        {
            case CookStage.生:
            case CookStage.焦:
                return 0.2f;

            case CookStage.三:
                return 0.6f;

            case CookStage.五:
                return 0.8f;

            case CookStage.熟:
                return 1f;

            default:
                return 0.2f;
        }
    }

    public string GetOverallCookStageText()
    {
        CookStage overallStage = GetOverallCookStage();

        switch (overallStage)
        {
            case CookStage.生:
                return "生";

            case CookStage.三:
                return "三分熟";

            case CookStage.五:
                return "五分熟";

            case CookStage.熟:
                return "全熟";

            case CookStage.焦:
                return "焦";

            default:
                return "未知";
        }
    }
}