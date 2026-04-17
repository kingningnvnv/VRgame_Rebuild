using UnityEngine;

public class StoveBurnerHeatSource : MonoBehaviour
{
    [Header("对应这个炉口的火焰对象")]
    [SerializeField] private GameObject fireOnObject;

    public bool IsHeating
    {
        get
        {
            return fireOnObject != null && fireOnObject.activeInHierarchy;
        }
    }
}