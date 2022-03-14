using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIdle : BossState
{
    private float currentDecelSpd;

    public BossIdle(Boss boss) : base(boss)
    {

    }

    public override void OnStateEnter()
    {
        boss.bossNavMeshAgent.isStopped = true;
        boss.bossNavMeshAgent.ResetPath();
        currentDecelSpd = boss.currentMoveSpeed;

        base.OnStateEnter();
    }

    public override void OnStateTransition()
    {
        boss.bossNavMeshAgent.isStopped = false;
        base.OnStateTransition();
    }

    public override void OnStateUpdate()
    {
        if(boss.currentMoveSpeed > 0f)
        {
            boss.currentMoveSpeed = Mathf.Lerp(currentDecelSpd, 0f, 4f * Time.deltaTime);
            if (boss.currentMoveSpeed <= 0f) boss.currentMoveSpeed = 0f;

            boss.bossAnimator.SetFloat("Speed", boss.currentMoveSpeed / boss.bossSpeed);
        }

        base.OnStateUpdate();
    }
}
