using UnityEngine;
using System.Collections.Generic;

public enum CookingStepType
{
    WaterServed,
    Salt,
    Pepper,
    Oil,
    Garlic,
    Rosemary,
    Butter
}

[System.Serializable]
public class CookingStepRecord
{
    public CookingStepType stepType;
    public bool isCompleted;
    public int completeOrder;
}

public class CookingStepTracker : MonoBehaviour
{
    private static CookingStepTracker _instance;

    public static CookingStepTracker Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CookingStepTracker>();

                if (_instance == null)
                {
                    GameObject go = new GameObject("CookingStepTracker");
                    _instance = go.AddComponent<CookingStepTracker>();
                }
            }

            return _instance;
        }
    }

    [Header("Debug Records")]
    [SerializeField] private List<CookingStepRecord> records = new List<CookingStepRecord>();

    [SerializeField] private int currentOrder = 0;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        InitializeRecords();
        ResetTracker();
    }

    private void InitializeRecords()
    {
        records.Clear();

        CookingStepType[] allSteps = (CookingStepType[])System.Enum.GetValues(typeof(CookingStepType));
        for (int i = 0; i < allSteps.Length; i++)
        {
            CookingStepRecord record = new CookingStepRecord();
            record.stepType = allSteps[i];
            record.isCompleted = false;
            record.completeOrder = -1;
            records.Add(record);
        }
    }

    [ContextMenu("Reset Tracker")]
    public void ResetTracker()
    {
        currentOrder = 0;

        for (int i = 0; i < records.Count; i++)
        {
            records[i].isCompleted = false;
            records[i].completeOrder = -1;
        }

        Debug.Log("CookingStepTracker has been reset.");
    }

    public void RegisterStep(CookingStepType stepType)
    {
        CookingStepRecord record = GetRecord(stepType);
        if (record == null) return;

        if (record.isCompleted) return;

        currentOrder++;
        record.isCompleted = true;
        record.completeOrder = currentOrder;

        Debug.Log($"Step registered: {stepType}, order = {record.completeOrder}");
    }

    public bool HasStep(CookingStepType stepType)
    {
        CookingStepRecord record = GetRecord(stepType);
        if (record == null) return false;

        return record.isCompleted;
    }

    public int GetStepOrder(CookingStepType stepType)
    {
        CookingStepRecord record = GetRecord(stepType);
        if (record == null) return -1;

        return record.completeOrder;
    }

    private CookingStepRecord GetRecord(CookingStepType stepType)
    {
        for (int i = 0; i < records.Count; i++)
        {
            if (records[i].stepType == stepType)
            {
                return records[i];
            }
        }

        return null;
    }
}