using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossMove : BossState
{
    public BossMove(Boss boss) : base(boss)
    {

    }

    public override void OnStateEnter()
    {
        boss.bossNavMeshAgent.isStopped = false;
        base.OnStateEnter();
    }

    public override void OnStateTransition()
    {
        base.OnStateTransition();
    }

    public override void OnStateUpdate()
    {
        if (boss.playerToTarget != null)
        {
            boss.bossNavMeshAgent.SetDestination(boss.playerToTarget.transform.position);


            float dist = boss.bossNavMeshAgent.remainingDistance;

            if (dist > 3f)
            {
                if (boss.currentMoveSpeed < boss.bossSpeed)
                {
                    boss.currentMoveSpeed += 3.7f * Time.deltaTime;
                    if (boss.currentMoveSpeed >= boss.bossSpeed) boss.currentMoveSpeed = boss.bossSpeed;
                }
            }
            else
            {
                if (boss.currentMoveSpeed > 0f)
                {
                    boss.currentMoveSpeed -= 4f * Time.deltaTime;
                    if (boss.currentMoveSpeed <= 0f) boss.currentMoveSpeed = 0f;
                }
            }

            if (dist <= boss.bossNavMeshAgent.stoppingDistance) boss.RotateBossToPos(boss.playerToTarget.transform.position);
        }

        boss.bossAnimator.SetFloat("Speed", boss.currentMoveSpeed / boss.bossSpeed);

        base.OnStateUpdate();
    }
}
