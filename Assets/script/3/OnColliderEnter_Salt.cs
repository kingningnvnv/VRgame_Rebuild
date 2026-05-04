using UnityEngine;
using System.Collections;

public class OnColliderEnter_Salt : MonoBehaviour
{
    protected Collider myselfCollider;
    public bool NeedDestroy;

    private void Start()
    {
        myselfCollider = this.GetComponentInChildren<Collider>();
        if (NeedDestroy)
        {
            StartCoroutine(DestroyAfterDelay(10f));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnColliderSTEAK(collision);
    }

    private void OnColliderSTEAK(Collision collision)
    {
        if (collision.gameObject.CompareTag("Steak"))
        {

            SteakManager mn = collision.transform.GetComponentInParent<SteakManager>();
            if (mn != null)
            {
                Transform visuals = mn.transform.Find("Visuals");
                if (visuals != null)
                {
                    transform.SetParent(visuals);
                }
            }
            // 先调用虚函数（让子类有机会执行逻辑）
            OnCollideSteak(collision);

            // 等1帧后再执行删除脚本、删除Rigidbody、禁用碰撞器、设置父物体
            StartCoroutine(DestroyComponentsNextFrame(collision));
        }
    }

    private void OnColliderSTEAK(Collider collision)
    {
        if (collision.transform.parent != null && collision.transform.parent.gameObject.CompareTag("Steak"))
        {

            SteakManager mn = collision.transform.GetComponentInParent<SteakManager>();
            if (mn != null)
            {
                Transform visuals = mn.transform.Find("Visuals");
                if (visuals != null)
                {
                    transform.SetParent(visuals);
                }
            }
            // 先调用虚函数
            OnCollideSteak(collision);


            // 等1帧后再执行删除脚本、删除Rigidbody、禁用碰撞器、设置父物体
            StartCoroutine(DestroyComponentsNextFrame(collision));
        }
    }

    // 等1帧后再执行
    private IEnumerator DestroyComponentsNextFrame(Collision collision)
    {
        yield return null; // 等待1帧

        // 删除所有脚本
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script==this)
            {
                continue;
            }
            Destroy(script);
        }

        // 删除 Rigidbody
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Destroy(rb);
        }

        myselfCollider.enabled = false;


    }

    // 为 Trigger 版本重载
    private IEnumerator DestroyComponentsNextFrame(Collider collision)
    {
        yield return null; // 等待1帧

        // 删除所有脚本
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            Destroy(script);
        }

        // 删除 Rigidbody
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Destroy(rb);
        }

        myselfCollider.enabled = false;

        SteakManager mn = collision.transform.GetComponentInParent<SteakManager>();
        if (mn != null)
        {
            Transform visuals = mn.transform.Find("Visuals");
            if (visuals != null)
            {
                transform.SetParent(visuals);
            }
        }
    }

    public virtual void OnCollideSteak(Collision collision)
    {

    }

    public virtual void OnCollideSteak(Collider collision)
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        OnColliderSTEAK(other);
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (NeedDestroy)
        {
            Destroy(gameObject);
        }
    }
}