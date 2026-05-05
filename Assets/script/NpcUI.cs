using UnityEngine;
using UnityEngine.UI;

public class SimpleUIAttach : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject UIPrefab;      // 要挂载的 UI 预制件
    public Transform attachPoint;    // UI 挂载位置，比如 NPC 头顶空物体

    private Transform UIInstance;    // 实例化后的 UI
    private Transform cam;

    private bool shouldBeVisible = true;

    void Awake()
    {
        RefreshCamera();
    }

    void OnEnable()
    {
        RefreshCamera();

        if (UIInstance == null)
        {
            CreateUI();
        }
        else
        {
            UIInstance.gameObject.SetActive(shouldBeVisible);
        }
    }

    void OnDisable()
    {
        if (UIInstance != null)
        {
            UIInstance.gameObject.SetActive(false);
        }
    }

    void OnDestroy()
    {
        if (UIInstance != null)
        {
            Destroy(UIInstance.gameObject);
            UIInstance = null;
        }
    }

    void LateUpdate()
    {
        if (UIInstance == null || attachPoint == null)
        {
            return;
        }

        if (!UIInstance.gameObject.activeSelf)
        {
            return;
        }

        if (cam == null)
        {
            RefreshCamera();
        }

        if (cam == null)
        {
            return;
        }

        UIInstance.position = attachPoint.position;
        UIInstance.forward = -cam.forward;
    }

    void CreateUI()
    {
        if (UIPrefab == null)
        {
            Debug.LogError(name + " 的 UIPrefab 没有挂载！");
            return;
        }

        if (attachPoint == null)
        {
            Debug.LogError(name + " 的 attachPoint 没有设置！");
            return;
        }

        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                UIInstance = Instantiate(UIPrefab, canvas.transform).transform;
                UIInstance.gameObject.SetActive(shouldBeVisible);
                return;
            }
        }

        Debug.LogError(name + " 没有找到 World Space Canvas，NPC 头顶 UI 无法生成！");
    }

    public void SetUIVisible(bool visible)
    {
        shouldBeVisible = visible;

        if (UIInstance != null)
        {
            UIInstance.gameObject.SetActive(visible);
        }
    }

    public void DestroyUI()
    {
        if (UIInstance != null)
        {
            Destroy(UIInstance.gameObject);
            UIInstance = null;
        }
    }

    void RefreshCamera()
    {
        if (Camera.main != null)
        {
            cam = Camera.main.transform;
        }
    }
}