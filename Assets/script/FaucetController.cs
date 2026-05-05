using UnityEngine;

public class FaucetController : MonoBehaviour
{
    [Header("Water")]
    public GameObject waterEffect;
    public Collider waterWashZone;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip waterOnClip;
    public AudioClip waterOffClip;
    public AudioClip waterLoopClip;

    public bool IsWaterOn { get; private set; } = false;

    private void Start()
    {
        SetWater(false);
    }

    public void ToggleWater()
    {
        SetWater(!IsWaterOn);
    }

    public void SetWater(bool state)
    {
        IsWaterOn = state;

        if (waterEffect != null)
        {
            waterEffect.SetActive(IsWaterOn);
        }

        if (waterWashZone != null)
        {
            waterWashZone.enabled = IsWaterOn;
        }

        if (audioSource != null)
        {
            if (IsWaterOn)
            {
                if (waterOnClip != null)
                {
                    audioSource.PlayOneShot(waterOnClip);
                }

                if (waterLoopClip != null)
                {
                    audioSource.clip = waterLoopClip;
                    audioSource.loop = true;
                    audioSource.Play();
                }
            }
            else
            {
                if (audioSource.loop)
                {
                    audioSource.Stop();
                    audioSource.loop = false;
                    audioSource.clip = null;
                }

                if (waterOffClip != null)
                {
                    audioSource.PlayOneShot(waterOffClip);
                }
            }
        }

        Debug.Log("FaucetController: Water state = " + IsWaterOn);
    }
}