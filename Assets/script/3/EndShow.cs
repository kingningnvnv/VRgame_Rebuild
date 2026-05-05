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

    [Header("NPC离开设置")]
    [Tooltip("可以不拖。如果这里不拖，就使用 NPC 自己脚本里的 LeaveTargetPoint")]
    public Transform leaveTargetPoint; // NPC 起身后要走向的位置

    [Tooltip("牛排放入评分区域后，NPC 延迟几秒开始起身")]
    public float minLeaveDelay = 3f;

    [Tooltip("牛排放入评分区域后，NPC 延迟几秒开始起身")]
    public float maxLeaveDelay = 5f;

    [Tooltip("第一个 NPC，拖挂了 NpcController 的 NPC 本体")]
    public NpcController npcController;

    [Tooltip("第二个 NPC，拖挂了 NpcController2 的 NPC 本体")]
    public NpcController2 npcController2;

    [Tooltip("是否只触发一次 NPC 离开")]
    public bool leaveOnlyOnce = true;

    public UnityEvent onShowTrigger;

    private bool hasTriggeredLeave = false;

    private void OnTriggerEnter(Collider other)
    {
        Trigger(other);
    }

    private void Trigger(Collider other)
    {
        if (other == null) return;

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

        TriggerNpcLeave();

        if (targetObject != null)
        {
            Debug.Log($"牛排进入触发器，{targetObject.name} 已显示，并触发 NPC 离开流程");
        }
        else
        {
            Debug.Log("牛排进入触发器，已触发评分和 NPC 离开流程");
        }
    }

    private void TriggerNpcLeave()
    {
        if (leaveOnlyOnce && hasTriggeredLeave)
        {
            return;
        }

        hasTriggeredLeave = true;

        float minDelay = Mathf.Min(minLeaveDelay, maxLeaveDelay);
        float maxDelay = Mathf.Max(minLeaveDelay, maxLeaveDelay);
        float leaveDelay = Random.Range(minDelay, maxDelay);

        int callCount = 0;

        if (npcController != null)
        {
            npcController.StartLeaveAfterDelay(leaveTargetPoint, leaveDelay);
            callCount++;
        }

        if (npcController2 != null)
        {
            npcController2.StartLeaveAfterDelay(leaveTargetPoint, leaveDelay);
            callCount++;
        }

        if (callCount == 0)
        {
            Debug.LogWarning("EndShow 没有拖 NPC，所以评分出现了，但没有 NPC 会起身离开。");
        }
        else
        {
            Debug.Log($"EndShow 已向 {callCount} 个 NPC 发送起身离开命令，延迟时间：{leaveDelay:F1} 秒。");
        }
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
            result.AppendLine("✓ Pepper Added");
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