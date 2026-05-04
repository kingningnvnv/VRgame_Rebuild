using UnityEngine;
using UnityEngine.Events;
using System.Text;
using UnityEngine.UI;

public class EndShow : MonoBehaviour
{
    [Header("显示设置")]
    public GameObject targetObject; // 要显示的指定物体

    [Header("评分设置")]
    public Text resultText; // 显示评分结果（分数+原因）的UI文本

    public UnityEvent onShowTrigger;

    private void OnTriggerEnter(Collider other)
    {
        Trigger(other);
    }

    private void Trigger(Collider other)
    {
        SteakManager steakManager = other.GetComponentInParent<SteakManager>();
        if (steakManager == null) return;

        if (targetObject != null)
        {
            targetObject.SetActive(true);
        }

        if (resultText != null)
        {
            CalculateAndShowScore(steakManager);
        }

        if (onShowTrigger != null)
        {
            onShowTrigger.Invoke();
        }

        Debug.Log($"牛排进入触发器，{targetObject.name} 已显示");
    }

    private void CalculateAndShowScore(SteakManager steakManager)
    {
        float baseScore = 0f;
        StringBuilder result = new StringBuilder();

        result.AppendLine("━━━━ Score Details ━━━━");

        // 盐
        if (steakManager.HasSalt)
        {
            baseScore += 100f / 6f;
            result.AppendLine("✓ Salt Added");
        }
        else
        {
            result.AppendLine("✗ No Salt");
        }

        // 大蒜
        if (steakManager.HasGarlic)
        {
            baseScore += 100f / 6f;
            result.AppendLine("✓ Garlic Added");
        }
        else
        {
            result.AppendLine("✗ No Garlic");
        }

        // 迷迭香
        if (steakManager.HasRosemary)
        {
            baseScore += 100f / 6f;
            result.AppendLine("✓ Rosemary Added");
        }
        else
        {
            result.AppendLine("✗ No Rosemary");
        }

        // 黑胡椒
        if (steakManager.HasPepper)
        {
            baseScore += 100f / 6f;
            result.AppendLine("✓Pepper Added");
        }
        else
        {
            result.AppendLine("✗ No Pepper");
        }

        // 黄油
        if (steakManager.HasButter)
        {
            baseScore += 100f / 6f;
            result.AppendLine("✓ Butter Added");
        }
        else
        {
            result.AppendLine("✗ No Butter");
        }

        // 油
        if (steakManager.HasOil)
        {
            baseScore += 100f / 6f;
            result.AppendLine("✓ Oil Added");
        }
        else
        {
            result.AppendLine("✗ No Oil");
        }

        // 获取熟度控制器
        SteakCookController cookController = steakManager.GetComponent<SteakCookController>();
        if (cookController == null)
        {
            cookController = steakManager.GetComponentInChildren<SteakCookController>(true);
        }

        float donenessCoefficient = 1f;
        string donenessText = "Not Read";

        if (cookController != null)
        {
            donenessCoefficient = cookController.GetDonenessCoefficient();
            donenessText = cookController.GetOverallCookStageText();
        }
        else
        {
            Debug.LogWarning("EndShow：没有找到 SteakCookController，默认熟度系数按 1 处理。");
        }

        float finalScore = baseScore * donenessCoefficient;

        result.AppendLine("━━━━━━━━━━━━━━");
        result.AppendLine($"Steak Doneness: {donenessText}");
        result.AppendLine($"Final Score: {finalScore:F1} / 100");

        resultText.text = result.ToString();

        Debug.Log($"评分完成：步骤分={baseScore:F1}，熟度={donenessText}，最终分={finalScore:F1}");
    }
}