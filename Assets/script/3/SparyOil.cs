using UnityEngine;

public class SparyOil : MonoBehaviour
{
    public AudioClip sparyClip;
    public ParticleSystem par;

    [Header("Raycast Settings")]
    public Transform rayOriginPoint;
    public float rayDistance = 5f;
    public LayerMask targetLayers = -1;

    private void Start()
    {
        if (rayOriginPoint == null)
        {
            rayOriginPoint = transform;
        }
    }

    public void Spary()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null && sparyClip != null)
        {
            audioSource.PlayOneShot(sparyClip);
        }

        if (par != null)
        {
            par.Play();
        }

        ShootRay();
    }

    void ShootRay()
    {
        if (rayOriginPoint == null) return;

        Vector3 origin = rayOriginPoint.position;
        Vector3 direction = rayOriginPoint.forward;

        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, rayDistance, targetLayers))
        {
            if (hit.collider.transform.CompareTag("Pan"))
            {
                PanHeatReceiver panHeatReceiver = hit.collider.GetComponentInParent<PanHeatReceiver>();

                if (panHeatReceiver != null)
                {
                    OnPanHit(panHeatReceiver, hit);
                }
                else
                {
                    Debug.LogWarning("Hit Pan, but PanHeatReceiver was not found.");
                }
            }
        }
    }

    protected virtual void OnPanHit(PanHeatReceiver panHeatReceiver, RaycastHit hit)
    {
        panHeatReceiver.HasOil = true;
        Debug.Log($"Oil successfully applied to: {hit.collider.gameObject.name}");
    }

    private void OnDrawGizmos()
    {
        if (rayOriginPoint != null)
        {
            Vector3 origin = rayOriginPoint.position;
            Vector3 direction = rayOriginPoint.forward;
            Vector3 endPoint = origin + direction * rayDistance;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(origin, endPoint);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(origin, 0.05f);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(endPoint, 0.05f);

            Vector3 arrowStart = endPoint - direction * 0.2f;
            Vector3 right = Vector3.Cross(direction, Vector3.up).normalized;
            Vector3 up = Vector3.Cross(right, direction).normalized;
            Gizmos.DrawLine(endPoint, arrowStart + right * 0.1f);
            Gizmos.DrawLine(endPoint, arrowStart - right * 0.1f);
            Gizmos.DrawLine(endPoint, arrowStart + up * 0.1f);

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(origin, direction * 0.5f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        OnDrawGizmos();
    }
}