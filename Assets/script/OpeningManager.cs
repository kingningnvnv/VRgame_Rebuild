using UnityEngine;

public class OpeningManager : MonoBehaviour
{
    [Header("Player")]
    public Transform xrOrigin;
    public Transform kitchenSpawnPoint;

    [Header("Opening UI")]
    public GameObject storyCanvas;
    public GameObject doorHintCanvas;

    [Header("Kitchen UI")]
    public GameObject kitchenStartCanvas;

    private bool hasEnteredKitchen = false;

    private void Start()
    {
        if (storyCanvas != null)
        {
            storyCanvas.SetActive(true);
        }

        if (doorHintCanvas != null)
        {
            doorHintCanvas.SetActive(false);
        }

        if (kitchenStartCanvas != null)
        {
            kitchenStartCanvas.SetActive(false);
        }
    }

    public void EnterKitchen()
    {
        if (hasEnteredKitchen) return;

        hasEnteredKitchen = true;

        if (storyCanvas != null)
        {
            storyCanvas.SetActive(false);
        }

        if (doorHintCanvas != null)
        {
            doorHintCanvas.SetActive(false);
        }

        if (xrOrigin != null && kitchenSpawnPoint != null)
        {
            xrOrigin.position = kitchenSpawnPoint.position;
            xrOrigin.rotation = kitchenSpawnPoint.rotation;
        }

        if (kitchenStartCanvas != null)
        {
            kitchenStartCanvas.SetActive(true);
        }

        Invoke(nameof(HideKitchenStartCanvas), 5f);
    }

    private void HideKitchenStartCanvas()
    {
        if (kitchenStartCanvas != null)
        {
            kitchenStartCanvas.SetActive(false);
        }
    }
}