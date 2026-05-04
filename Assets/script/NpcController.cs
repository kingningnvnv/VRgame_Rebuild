using UnityEngine;
using UnityEngine.AI;

public class NpcController : MonoBehaviour
{
    private enum NpcStates
    {
        WALK,
        SIT
    }

    private NpcStates npcStates;

    private NavMeshAgent agent;
    private Animator anim;

    [Header("Target Point")]
    public Transform TargetPoint;

    [Header("Sit Settings")]
    public Vector3 sitFaceDirection = Vector3.right;
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

    private bool hasSetDestination = false;
    private bool canRun = false;
    private bool hasWarnedInvalidPath = false;

    private Vector3 finalTargetPosition;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        npcStates = NpcStates.WALK;

        // 防止动画 Root Motion 和 NavMeshAgent 抢移动控制权
        if (anim != null)
        {
            anim.applyRootMotion = false;
        }
    }

    void Start()
    {
        canRun = CheckBasicSettings();

        if (canRun)
        {
            MoveToTarget();
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

    void SwitchStates()
    {
        switch (npcStates)
        {
            case NpcStates.WALK:
                isWalk = true;
                isSit = false;

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