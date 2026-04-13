using UnityEngine;
using UnityEngine.AI;

public class NpcSitControllerDelayed : MonoBehaviour
{
    private enum NpcStates { WALK, SIT }
    private NpcStates npcStates;

    private NavMeshAgent agent;
    private Animator anim;

    [Header("Target Point")]
    public Transform TargetPoint; // NPC 要移动到的目标点

    [Header("Delay Settings")]
    public float delayTime = 15f; // 延迟秒数，可在 Inspector 调整

    private bool isWalk;
    private bool isSit;
    private float speed;

    private float timer = 0f;
    private bool hasStarted = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        speed = agent.speed;

        npcStates = NpcStates.WALK;

        // 延迟期间隐藏模型，但 GameObject 保持启用
        SetVisible(false);

        // NavMeshAgent 保持启用，计时器可以继续运行
        agent.enabled = true;
        agent.isStopped = true; // 先不移动
    }

    void Update()
    {
        // 延迟逻辑
        if (!hasStarted)
        {
            timer += Time.deltaTime;
            if (timer >= delayTime)
            {
                hasStarted = true;

                // 延迟结束，显示模型并允许移动
                SetVisible(true);
                agent.isStopped = false;
            }
            else return; // 延迟期间不执行移动逻辑
        }

        // 状态机控制移动和动画
        SwitchStates();
        SwitchAnimation();
    }

    void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Sit", isSit);
    }

    void SwitchStates()
    {
        if (TargetPoint == null) return;

        switch (npcStates)
        {
            case NpcStates.WALK:
                isWalk = true;
                isSit = false;

                agent.isStopped = false;
                agent.destination = TargetPoint.position;

                // 到达目标点判断
                float distance = Vector3.Distance(transform.position, TargetPoint.position);
                if (distance <= agent.stoppingDistance + 0.2f) // 容差 0.2
                {
                    npcStates = NpcStates.SIT;
                    agent.isStopped = true;

                    // 坐下时朝向正 X 轴
                    transform.rotation = Quaternion.LookRotation(Vector3.left, Vector3.up);

                    // 确保 Animator 立即切换动画
                    isWalk = false;
                    isSit = true;
                }
                break;

            case NpcStates.SIT:
                isWalk = false;
                isSit = true;
                agent.isStopped = true;
                break;
        }
    }

    // 控制模型可见性（不禁用 GameObject）
    void SetVisible(bool visible)
    {
        var renderers = GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            r.enabled = visible;
        }
    }
}