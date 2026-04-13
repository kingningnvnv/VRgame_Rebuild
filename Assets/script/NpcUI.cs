using UnityEngine;
using UnityEngine.UI;

public class SimpleUIAttach : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject UIPrefab;          // 要挂载的 UI 预制件
    public Transform attachPoint;        // UI 挂载位置（空对象）

    private Transform UIInstance;        // 实例化后的 UI
    private Transform cam;

    void OnEnable()
    {
        cam = Camera.main.transform;

        if (UIPrefab == null)
        {
            Debug.LogError("UIPrefab 未挂载！");
            return;
        }

        if (attachPoint == null)
        {
            Debug.LogError("attachPoint 未设置！");
            return;
        }

        // 找到场景中所有 Canvas，挂在 World Space Canvas 下
        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                UIInstance = Instantiate(UIPrefab, canvas.transform).transform;
                UIInstance.gameObject.SetActive(true);
                break;  // 找到第一个 World Space Canvas 就行
            }
        }
    }

    void LateUpdate()
    {
        if (UIInstance != null && attachPoint != null)
        {
            // 跟随挂载点，并始终朝向摄像机
            UIInstance.position = attachPoint.position;
            UIInstance.forward = -cam.forward;
        }
    }
}