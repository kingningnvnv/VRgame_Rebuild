using UnityEngine;
using System.Collections.Generic;

public class AllSteakCheck : MonoBehaviour
{
    private static AllSteakCheck _instance;

    public static AllSteakCheck Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AllSteakCheck>();

                if (_instance == null)
                {
                    GameObject go = new GameObject("AllSteakCheck");
                    _instance = go.AddComponent<AllSteakCheck>();
                }
            }

            return _instance;
        }
    }

    [Header("All Steaks")]
    public List<SteakManager> allSteaks = new List<SteakManager>();

    private bool hasExecuted = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }

    private void Update()
    {
        if (!hasExecuted)
        {
            FindAllSteaks();
            hasExecuted = true;
        }
    }

    private void FindAllSteaks()
    {
        allSteaks.Clear();

        GameObject[] steakObjects = GameObject.FindGameObjectsWithTag("Steak");

        foreach (GameObject steakObj in steakObjects)
        {
            SteakManager steakManager = steakObj.GetComponent<SteakManager>();

            if (steakManager != null)
            {
                allSteaks.Add(steakManager);
            }
            else
            {
                Debug.LogWarning($"对象 {steakObj.name} 带有 Steak 标签，但没有找到 SteakManager 组件。");
            }
        }

        Debug.Log($"共找到 {allSteaks.Count} 个牛排对象。");
    }

    public void RefreshSteakList()
    {
        FindAllSteaks();
    }
}