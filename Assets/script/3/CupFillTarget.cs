using UnityEngine;

public class CupFillTarget : MonoBehaviour
{
    [Header("Cup Visual References")]
    [SerializeField] private GameObject emptyCupVisual;
    [SerializeField] private GameObject fullCupVisual;

    [Header("State")]
    [SerializeField] private bool isFilled = false;

    public bool IsFilled
    {
        get { return isFilled; }
    }

    private void Start()
    {
        ApplyVisualState();
    }

    public void FillCup()
    {
        if (isFilled) return;

        isFilled = true;
        ApplyVisualState();

        Debug.Log($"{gameObject.name} has been filled with water.");
    }

    public void EmptyCup()
    {
        isFilled = false;
        ApplyVisualState();

        Debug.Log($"{gameObject.name} has been emptied.");
    }

    private void ApplyVisualState()
    {
        if (emptyCupVisual != null)
        {
            emptyCupVisual.SetActive(!isFilled);
        }

        if (fullCupVisual != null)
        {
            fullCupVisual.SetActive(isFilled);
        }
    }
}