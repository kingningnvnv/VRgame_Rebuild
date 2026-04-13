using UnityEngine;
using UnityEngine.AI;

public class NpcSitController : MonoBehaviour
{
    private enum NpcStates { WALK, SIT }
    private NpcStates npcStates;

    private NavMeshAgent agent;
    private Animator anim;

    [Header("Target Point")]
    public Transform TargetPoint; // 拖入场景中的目标点

    private bool isWalk;
    private bool isSit;
    private float speed;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        speed = agent.speed;

        npcStates = NpcStates.WALK; // 初始状态为 WALK
    }

    void Update()
    {
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

                // 判断是否到达目标点
                if (Vector3.Distance(transform.position, TargetPoint.position) <= agent.stoppingDistance + 0.1f)
                {
                    npcStates = NpcStates.SIT;
                    agent.isStopped = true;

                    // 固定 NPC 朝向正 X 轴
                    transform.rotation = Quaternion.LookRotation(Vector3.right, Vector3.up);
                }
                break;

            case NpcStates.SIT:
                isWalk = false;
                isSit = true;
                agent.isStopped = true;
                break;
        }
    }
}