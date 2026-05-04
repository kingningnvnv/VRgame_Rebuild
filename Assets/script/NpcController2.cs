using UnityEngine;
using UnityEngine.AI;

public class NpcController2 : MonoBehaviour
{
    private enum NpcStates
    {
        WAIT,
        WALK,
        SIT
    }

    private NpcStates npcStates;

    private NavMeshAgent agent;
    private Animator anim;

    [Header("Target Point")]
    public Transform TargetPoint;

    [Header("Delay Settings")]
    public float delayTime = 15f;

    [Header("Sit Settings")]
    public Vector3 sitFaceDirection = Vector3.left;
    // Vector3.right   = 面向 X 正方向
    // Vector3.left    = 面向 X 反方向
    // Vector3.forward = 面向 Z 正方向
    // Vector3.back    = 面向 Z 反方向

    [Header("Arrive Settings")]
    public float arriveTolerance = 0.2f;

    [Header("NavMesh Settings")]
    public float targetSampleMaxDistance = 2f;
    // 如果 TargetPoint 不完全在 NavMesh 上，会在附近 2 米内寻找最近的 NavMesh 点

    private bool isWalk;
    private bool isSit;

    private float timer = 0f;

    private bool hasStarted = false;
    private bool hasSetDestination = false;
    private bool canRun = false;
    private bool hasWarnedInvalidPath = false;

    private Vector3 finalTargetPosition;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        npcStates = NpcStates.WAIT;

        // 防止动画 Root Motion 和 NavMeshAgent 抢移动控制权
        if (anim != null)
        {
            anim.applyRootMotion = false;
        }

        // 延迟期间隐藏模型，但不禁用整个 GameObject
        // 这样 Update 还能继续计时
        SetVisible(false);
    }

    void Start()
    {
        canRun = CheckBasicSettings();

        if (canRun)
        {
            agent.isStopped = true;
        }
    }

    void Update()
    {
        if (!canRun) return;

        SwitchStates();
        SwitchAnimation();
    }

    bool CheckBasicSettings()
    {
        if (TargetPoint == null)
        {
            Debug.LogError(name + " 没有设置 TargetPoint，NPC 不会移动！");
            return false;
        }

        if (agent == null)
        {
            Debug.LogError(name + " 没有 NavMeshAgent 组件！");
            return false;
        }

        if (!agent.enabled)
        {
            Debug.LogError(name + " 的 NavMeshAgent 被禁用了！");
            return false;
        }

        if (!agent.isOnNavMesh)
        {
            Debug.LogError(name + " 的 NavMeshAgent 不在 NavMesh 上，请检查 NPC 是否放在蓝色 NavMesh 区域内！");
            return false;
        }

        return true;
    }

    void SwitchStates()
    {
        switch (npcStates)
        {
            case NpcStates.WAIT:
                isWalk = false;
                isSit = false;

                agent.isStopped = true;

                timer += Time.deltaTime;

                if (timer >= delayTime)
                {
                    hasStarted = true;

                    // 延迟结束后显示 NPC
                    SetVisible(true);

                    npcStates = NpcStates.WALK;
                    MoveToTarget();
                }
                break;

            case NpcStates.WALK:
                isWalk = true;
                isSit = false;

                if (!hasStarted)
                {
                    return;
                }

                if (!hasSetDestination)
                {
                    MoveToTarget();
                    return;
                }

                if (!agent.pathPending && agent.pathStatus == NavMeshPathStatus.PathInvalid)
                {
                    if (!hasWarnedInvalidPath)
                    {
                        Debug.LogWarning(name + " 找不到有效路径，请检查 NPC 和 TargetPoint 是否在同一块可行走 NavMesh 上！");
                        hasWarnedInvalidPath = true;
                    }

                    return;
                }

                // 用 remainingDistance 判断是否到达，比 Vector3.Distance 更适合 NavMeshAgent
                if (!agent.pathPending &&
                    agent.remainingDistance <= agent.stoppingDistance + arriveTolerance)
                {
                    SitDown();
                }
                break;

            case NpcStates.SIT:
                isWalk = false;
                isSit = true;

                if (agent.isOnNavMesh)
                {
                    agent.isStopped = true;
                }
                break;
        }
    }

    void MoveToTarget()
    {
        if (!TryGetNavMeshTarget(TargetPoint.position, out finalTargetPosition))
        {
            Debug.LogError(name + " 的 TargetPoint 附近没有找到 NavMesh，请把目标点放到蓝色 NavMesh 区域上！");
            return;
        }

        agent.isStopped = false;
        agent.ResetPath();

        bool success = agent.SetDestination(finalTargetPosition);

        if (!success)
        {
            Debug.LogError(name + " 设置目标点失败，请检查 NavMesh 是否烘焙正确！");
            hasSetDestination = false;
            return;
        }

        hasSetDestination = true;
    }

    void SitDown()
    {
        npcStates = NpcStates.SIT;

        agent.isStopped = true;
        agent.ResetPath();

        isWalk = false;
        isSit = true;

        FaceSitDirection();
    }

    void SwitchAnimation()
    {
        if (anim == null) return;

        anim.SetBool("Walk", isWalk);
        anim.SetBool("Sit", isSit);
    }

    void FaceSitDirection()
    {
        if (sitFaceDirection == Vector3.zero) return;

        transform.rotation = Quaternion.LookRotation(sitFaceDirection.normalized, Vector3.up);
    }

    void SetVisible(bool visible)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);

        foreach (Renderer r in renderers)
        {
            r.enabled = visible;
        }

        // 如果 NPC 身上有子 Canvas，比如头顶 UI，也一起隐藏/显示
        Canvas[] canvases = GetComponentsInChildren<Canvas>(true);

        foreach (Canvas c in canvases)
        {
            c.enabled = visible;
        }
    }

    bool TryGetNavMeshTarget(Vector3 targetPosition, out Vector3 result)
    {
        NavMeshHit hit;

        if (NavMesh.SamplePosition(targetPosition, out hit, targetSampleMaxDistance, agent.areaMask))
        {
            result = hit.position;
            return true;
        }

        result = targetPosition;
        return false;
    }
}