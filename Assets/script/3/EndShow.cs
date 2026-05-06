using UnityEngine;
using UnityEngine.Events;
using System.Text;
using UnityEngine.UI;

public class EndShow : MonoBehaviour
{
    [Header("Display Settings")]
    public GameObject targetObject;

    [Header("Scoring Settings")]
    public Text resultText;

    public UnityEvent onShowTrigger;

    private SteakManager currentSteak;
    private CupFillTarget currentCup;

    // 这个评分区自己的本轮记录
    private int localOrderCounter = 0;
    private bool localWaterCompleted = false;
    private int localWaterOrder = -1;
    private int localSteakPresentedOrder = -1;

    private bool hasShownWindowThisRound = false;
    private bool shouldResetWhenAreaClears = false;

    private void Start()
    {
        HideWindow();
    }

    private void Update()
    {
        // 如果杯子已经在这个评分区里，并且后来才变成满水
        // 那么也记录这个评分区自己的倒水步骤
        if (currentCup != null && currentCup.IsFilled && !localWaterCompleted)
        {
            RegisterLocalWaterStep();
        }

        // 只有这个评分区里有牛排，才刷新评分窗口
        if (currentSteak != null && targetObject != null && targetObject.activeSelf)
        {
            RefreshScoreUI();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        bool steakChanged = false;

        SteakManager steak = other.GetComponentInParent<SteakManager>();
        if (steak != null)
        {
            currentSteak = steak;
            steakChanged = true;

            if (localSteakPresentedOrder < 0)
            {
                localOrderCounter++;
                localSteakPresentedOrder = localOrderCounter;
            }
        }

        CupFillTarget cup = other.GetComponentInParent<CupFillTarget>();
        if (cup != null)
        {
            currentCup = cup;

            // 水杯进入当前评分区，只记录当前评分区自己的倒水状态
            if (cup.IsFilled && !localWaterCompleted)
            {
                RegisterLocalWaterStep();
            }
        }

        // 只有牛排进入当前评分区，才显示这个区域自己的评分窗口
        if (steakChanged)
        {
            ShowWindow();
            RefreshScoreUI();

            if (onShowTrigger != null)
            {
                onShowTrigger.Invoke();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        SteakManager steak = other.GetComponentInParent<SteakManager>();
        if (steak != null && currentSteak == steak)
        {
            currentSteak = null;
        }

        CupFillTarget cup = other.GetComponentInParent<CupFillTarget>();
        if (cup != null && currentCup == cup)
        {
            currentCup = null;
        }

        if (currentSteak == null)
        {
            HideWindow();
        }

        TryResetForNextOrder();
    }

    private void RegisterLocalWaterStep()
    {
        if (localWaterCompleted) return;

        localOrderCounter++;
        localWaterCompleted = true;
        localWaterOrder = localOrderCounter;

        Debug.Log($"{gameObject.name} recorded local Water Served step. order = {localWaterOrder}");
    }

    private void ShowWindow()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(true);
        }

        hasShownWindowThisRound = true;
        shouldResetWhenAreaClears = true;
    }

    private void HideWindow()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(false);
        }

        if (resultText != null)
        {
            resultText.text = "";
        }
    }

    private void TryResetForNextOrder()
    {
        if (!shouldResetWhenAreaClears) return;
        if (currentSteak != null) return;
        if (currentCup != null) return;

        ResetLocalAreaRound();

        Debug.Log($"{gameObject.name} area cleared. Local round reset for next order.");
    }

    private void ResetLocalAreaRound()
    {
        localOrderCounter = 0;
        localWaterCompleted = false;
        localWaterOrder = -1;
        localSteakPresentedOrder = -1;

        hasShownWindowThisRound = false;
        shouldResetWhenAreaClears = false;

        HideWindow();
    }

    private void RefreshScoreUI()
    {
        // 只在当前评分区里有牛排时显示
        if (currentSteak == null)
        {
            HideWindow();
            return;
        }

        if (targetObject != null)
        {
            targetObject.SetActive(true);
        }

        if (resultText == null) return;

        StringBuilder result = new StringBuilder();
        float baseScore = 0f;

        result.AppendLine("━━━━ Score Details ━━━━");

        // 当前评分区自己的 Water Served
        float waterScore = GetLocalWaterScore();
        baseScore += waterScore;
        result.AppendLine(GetLocalWaterLine(waterScore));

        // 牛排自己记录的制作步骤
        float saltScore = GetSteakStepScore(currentSteak, SteakCookingStepType.Salt);
        baseScore += saltScore;
        result.AppendLine(GetSteakStepLine("Salt Added", currentSteak, SteakCookingStepType.Salt, saltScore));

        float pepperScore = GetSteakStepScore(currentSteak, SteakCookingStepType.Pepper);
        baseScore += pepperScore;
        result.AppendLine(GetSteakStepLine("Black Pepper Added", currentSteak, SteakCookingStepType.Pepper, pepperScore));

        float oilScore = GetSteakStepScore(currentSteak, SteakCookingStepType.Oil);
        baseScore += oilScore;
        result.AppendLine(GetSteakStepLine("Oil Added", currentSteak, SteakCookingStepType.Oil, oilScore));

        float garlicScore = GetSteakStepScore(currentSteak, SteakCookingStepType.Garlic);
        baseScore += garlicScore;
        result.AppendLine(GetSteakStepLine("Garlic Added", currentSteak, SteakCookingStepType.Garlic, garlicScore));

        float rosemaryScore = GetSteakStepScore(currentSteak, SteakCookingStepType.Rosemary);
        baseScore += rosemaryScore;
        result.AppendLine(GetSteakStepLine("Rosemary Added", currentSteak, SteakCookingStepType.Rosemary, rosemaryScore));

        float butterScore = GetSteakStepScore(currentSteak, SteakCookingStepType.Butter);
        baseScore += butterScore;
        result.AppendLine(GetSteakStepLine("Butter Added", currentSteak, SteakCookingStepType.Butter, butterScore));

        result.AppendLine("━━━━━━━━━━━━━━━━");

        SteakCookController cookController = currentSteak.GetComponent<SteakCookController>();
        if (cookController == null)
        {
            cookController = currentSteak.GetComponentInChildren<SteakCookController>(true);
        }

        float donenessCoefficient = 1f;
        string frontDonenessText = "Not Read";
        string backDonenessText = "Not Read";

        if (cookController != null)
        {
            donenessCoefficient = cookController.GetDonenessCoefficient();
            frontDonenessText = ConvertSingleDonenessToEnglish(cookController.GetFrontCookStageText());
            backDonenessText = ConvertSingleDonenessToEnglish(cookController.GetBackCookStageText());
        }

        float finalScore = baseScore * donenessCoefficient;

        result.AppendLine($"Base Step Score: {baseScore:F1} / 70");
        result.AppendLine($"Doneness: {frontDonenessText} / {backDonenessText}");
        result.AppendLine($"Doneness Multiplier: x {donenessCoefficient:F2}");
        result.AppendLine($"Final Score: {finalScore:F1} / 70");

        resultText.text = result.ToString();
    }

    private float GetLocalWaterScore()
    {
        if (!localWaterCompleted) return 0f;

        return IsLocalWaterOrderCorrect() ? 10f : 5f;
    }

    private string GetLocalWaterLine(float score)
    {
        string mark = localWaterCompleted ? "✓" : "✗";
        return $"{mark} Water Served  |  Step Score: {score:F0}";
    }

    private bool IsLocalWaterOrderCorrect()
    {
        if (!localWaterCompleted) return false;
        if (localSteakPresentedOrder < 0) return false;

        // 当前评分区自己的规则：
        // 水要先于“该区上菜”完成，才算顺序正确
        return localWaterOrder < localSteakPresentedOrder;
    }

    private float GetSteakStepScore(SteakManager steak, SteakCookingStepType stepType)
    {
        if (steak == null) return 0f;
        if (!steak.HasRecordedStep(stepType)) return 0f;

        return steak.IsCookingStepOrderCorrect(stepType) ? 10f : 5f;
    }

    private string GetSteakStepLine(string label, SteakManager steak, SteakCookingStepType stepType, float stepScore)
    {
        bool completed = steak != null && steak.HasRecordedStep(stepType);
        string mark = completed ? "✓" : "✗";
        return $"{mark} {label}  |  Step Score: {stepScore:F0}";
    }

    private string ConvertSingleDonenessToEnglish(string chineseText)
    {
        switch (chineseText)
        {
            case "生":
                return "Raw";
            case "三分熟":
                return "Rare";
            case "五分熟":
                return "Medium";
            case "全熟":
                return "Well Done";
            case "焦":
                return "Burnt";
            case "未读取":
                return "Not Read";
            default:
                return chineseText;
        }
    }
}