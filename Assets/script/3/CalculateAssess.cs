using UnityEngine;

public class CalculateAssess : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;                 // 目标物体（例如玩家/相机）
    public bool useMainCamera = true;        // 若未指定 target，是否自动使用主相机
    public bool invertDirection = false;     // 是否反转朝向（背对目标）
    public float rotationSpeed = 5f;         // 旋转速度，<= 0 表示瞬间旋转

    private void Start()
    {
        // 如果没有手动指定目标，并且允许使用主相机，则自动获取主相机
        if (target == null && useMainCamera)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                target = mainCamera.transform;
            }
            else
            {
                Debug.LogWarning("CalculateAssess: 未找到 Main Camera，请手动指定 target。");
            }
        }
    }

    private void Update()
    {
        if (target == null) return;

        // 计算朝向目标的方向
        Vector3 direction = target.position - transform.position;

        // 如果需要反向朝向，则取反
        if (invertDirection)
        {
            direction = -direction;
        }

        // 只保留水平朝向，不上下倾斜
        direction.y = 0f;

        // 防止零向量报错
        if (direction.sqrMagnitude < 0.0001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        if (rotationSpeed <= 0f)
        {
            // 瞬间旋转
            transform.rotation = targetRotation;
        }
        else
        {
            // 平滑旋转
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}