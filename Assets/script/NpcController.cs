using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NpcController : MonoBehaviour
{
    private enum NpcStates
    {
        WALK,
        SIT,
        STAND_UP,
        PREPARE_LEAVE,
        LEAVE
    }

    private NpcStates npcStates;

    private NavMeshAgent agent;
    private Animator anim;

    [Header("Target Point")]
    public Transform TargetPoint;

    [Header("Sit Settings")]
    public Vector3 sitFaceDirection = Vector3.right;

    [Header("Arrive Settings")]
    public float arriveTolerance = 0.2f;

    [Header("NavMesh Settings")]
    public float targetSampleMaxDistance = 2f;

    [Header("Leave Settings")]
    public Transform LeaveTargetPoint;

    [Tooltip("起身动画时间，也就是 Stand 状态保持时间")]
    public float standUpAnimationTime = 3.5f;

    [Tooltip("从 Stand 切到 Walk 后，原地等多久再真正开始移动")]
    public float walkStartDelay = 0.3f;

    [Tooltip("离开时移动速度")]
    public float leaveSpeed = 0.8f;

    [Tooltip("到达离开点的判定距离")]
    public float leaveArriveTolerance = 0.2f;

    public bool hideWhenArriveLeavePoint = true;

    private bool isWalk;
    private bool isSit;
    private bool isStand;

    private bool hasSetDestination = false;
    private bool canRun = false;
    private bool hasWarnedInvalidPath = false;

    private bool leaveStarted = false;
    private bool hasSetLeaveDestination = false;
    private bool hasWarnedInvalidLeavePath = false;
    private bool hasFinishedLeave = false;

    private float standTimer = 0f;
    private float walkStartTimer = 0f;
    private float originalSpeed;

    private bool originalUpdatePosition = true;
    private bool originalUpdateRotation = true;

    private Vector3 finalTargetPosition;
    private Vector3 finalLeavePosition;
    private Vector3 standLockPosition;

    private Coroutine leaveCoroutine;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        npcStates = NpcStates.WALK;

        if (anim != null)
        {
            anim.applyRootMotion = false;
        }

        if (agent != null)
        {
            originalSpeed = agent.speed;
            originalUpdatePosition = agent.updatePosition;
            originalUpdateRotation = agent.updateRotation;
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

        agent.updatePosition = originalUpdatePosition;
        agent.updateRotation = originalUpdateRotation;
        agent.speed = originalSpeed;
        agent.isStopped = false;
        agent.ResetPath();

        bool success = agent.SetDestination(finalTargetPosition);

        if (!success)
        {
            Debug.LogError(name + " 设置目标点失败，请检查 NavMesh 是否正确！");
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
                isStand = false;

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

                if (!agent.pathPending &&
                    agent.remainingDistance <= agent.stoppingDistance + arriveTolerance)
                {
                    SitDown();
                }
                break;

            case NpcStates.SIT:
                isWalk = false;
                isSit = true;
                isStand = false;

                if (agent != null && agent.enabled && agent.isOnNavMesh)
                {
                    agent.isStopped = true;
                    agent.velocity = Vector3.zero;
                }
                break;

            case NpcStates.STAND_UP:
                isWalk = false;
                isSit = false;
                isStand = true;

                LockStandPosition();

                standTimer += Time.deltaTime;

                if (standTimer >= standUpAnimationTime)
                {
                    PrepareLeave();
                }
                break;

            case NpcStates.PREPARE_LEAVE:
                isWalk = true;
                isSit = false;
                isStand = false;

                LockStandPosition();

                walkStartTimer += Time.deltaTime;

                if (walkStartTimer >= walkStartDelay)
                {
                    StartLeaveMove();
                }
                break;

            case NpcStates.LEAVE:
                isWalk = true;
                isSit = false;
                isStand = false;

                if (!hasSetLeaveDestination)
                {
                    StartLeaveMove();
                    return;
                }

                if (!agent.pathPending && agent.pathStatus == NavMeshPathStatus.PathInvalid)
                {
                    if (!hasWarnedInvalidLeavePath)
                    {
                        Debug.LogWarning(name + " 找不到离开路径，请检查 NPC 和 LeaveTargetPoint 是否在同一块 NavMesh 上！");
                        hasWarnedInvalidLeavePath = true;
                    }

                    return;
                }

                if (!agent.pathPending &&
                    agent.remainingDistance <= agent.stoppingDistance + leaveArriveTolerance)
                {
                    FinishLeave();
                }
                break;
        }
    }

    void SitDown()
    {
        npcStates = NpcStates.SIT;

        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.ResetPath();

        isWalk = false;
        isSit = true;
        isStand = false;

        FaceSitDirection();

        Debug.Log(name + " 已到达座位，开始坐下。");
    }

    public void StartLeaveAfterDelay(Transform leaveTarget, float delayTime)
    {
        if (leaveStarted || hasFinishedLeave)
        {
            return;
        }

        if (leaveTarget != null)
        {
            LeaveTargetPoint = leaveTarget;
        }

        if (LeaveTargetPoint == null)
        {
            Debug.LogError(name + " 没有设置 LeaveTargetPoint，无法离开！");
            return;
        }

        if (!canRun)
        {
            canRun = CheckBasicSettings();
        }

        if (!canRun)
        {
            return;
        }

        leaveStarted = true;

        if (leaveCoroutine != null)
        {
            StopCoroutine(leaveCoroutine);
        }

        leaveCoroutine = StartCoroutine(LeaveAfterDelay(delayTime));

        Debug.Log(name + " 收到离开命令，将在 " + delayTime.ToString("F1") + " 秒后起身。");
    }

    IEnumerator LeaveAfterDelay(float delayTime)
    {
        yield return new WaitForSeconds(Mathf.Max(0f, delayTime));

        BeginStandUp();
    }

    void BeginStandUp()
    {
        if (LeaveTargetPoint == null)
        {
            Debug.LogError(name + " 没有设置 LeaveTargetPoint，无法起身离开！");
            return;
        }

        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
        {
            Debug.LogError(name + " 的 NavMeshAgent 状态不正确，无法起身离开！");
            return;
        }

        standLockPosition = transform.position;

        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.ResetPath();

        // 关键：Stand 阶段关闭 NavMeshAgent 对 transform 的位置更新
        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.nextPosition = standLockPosition;

        standTimer = 0f;
        walkStartTimer = 0f;
        hasSetLeaveDestination = false;
        hasWarnedInvalidLeavePath = false;

        npcStates = NpcStates.STAND_UP;

        isWalk = false;
        isSit = false;
        isStand = true;

        Debug.Log(name + " 开始起身：Stand 动作期间锁住位置，不移动。");
    }

    void PrepareLeave()
    {
        if (LeaveTargetPoint == null)
        {
            Debug.LogError(name + " 没有设置 LeaveTargetPoint，无法离开！");
            return;
        }

        if (!TryGetNavMeshTarget(LeaveTargetPoint.position, out finalLeavePosition))
        {
            Debug.LogError(name + " 的 LeaveTargetPoint 附近没有找到 NavMesh，请把离开点放到蓝色 NavMesh 区域上！");
            return;
        }

        FaceLeaveTarget(finalLeavePosition);

        standLockPosition = transform.position;

        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            agent.ResetPath();
            agent.updatePosition = false;
            agent.updateRotation = false;
            agent.nextPosition = standLockPosition;
        }

        walkStartTimer = 0f;
        npcStates = NpcStates.PREPARE_LEAVE;

        isWalk = true;
        isSit = false;
        isStand = false;

        Debug.Log(name + " Stand 结束，先切 Walk 动画，但暂时不移动。");
    }

    void StartLeaveMove()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
        {
            Debug.LogError(name + " 的 NavMeshAgent 状态不正确，无法开始离开移动！");
            return;
        }

        // 关键：真正开始 Walk 移动前，同步 NavMeshAgent 和当前 transform 位置
        agent.Warp(transform.position);
        agent.updatePosition = originalUpdatePosition;
        agent.updateRotation = originalUpdateRotation;

        agent.speed = leaveSpeed;
        agent.isStopped = false;
        agent.ResetPath();

        bool success = agent.SetDestination(finalLeavePosition);

        if (!success)
        {
            Debug.LogError(name + " 设置离开目标点失败，请检查 NavMesh！");
            hasSetLeaveDestination = false;
            return;
        }

        hasSetLeaveDestination = true;
        npcStates = NpcStates.LEAVE;

        isWalk = true;
        isSit = false;
        isStand = false;

        Debug.Log(name + " Walk 动画已切出，开始真正向离开点移动。");
    }

    void LockStandPosition()
    {
        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            agent.nextPosition = standLockPosition;
        }

        transform.position = standLockPosition;
    }

    void FinishLeave()
    {
        hasFinishedLeave = true;

        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            agent.ResetPath();
            agent.speed = originalSpeed;
            agent.updatePosition = originalUpdatePosition;
            agent.updateRotation = originalUpdateRotation;
        }

        isWalk = false;
        isSit = false;
        isStand = false;

        SwitchAnimation();

        if (hideWhenArriveLeavePoint)
        {
            SetVisible(false);
        }

        Debug.Log(name + " 已到达离开点并隐藏。");
    }

    void SwitchAnimation()
    {
        if (anim == null) return;

        anim.SetBool("Walk", isWalk);
        anim.SetBool("Sit", isSit);
        anim.SetBool("Stand", isStand);
    }

    void FaceSitDirection()
    {
        if (sitFaceDirection == Vector3.zero) return;

        transform.rotation = Quaternion.LookRotation(sitFaceDirection.normalized, Vector3.up);
    }

    void FaceLeaveTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f) return;

        transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
    }

    void SetVisible(bool visible)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);

        foreach (Renderer r in renderers)
        {
            r.enabled = visible;
        }

        Canvas[] canvases = GetComponentsInChildren<Canvas>(true);

        foreach (Canvas c in canvases)
        {
            c.enabled = visible;
        }

        BroadcastMessage("SetUIVisible", visible, SendMessageOptions.DontRequireReceiver);
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