using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDead : BossState
{
    public BossDead(Boss boss) : base(boss)
    {

    }

    public override void OnStateEnter()
    {
        boss.bossNavMeshAgent.isStopped = true;
        boss.bossNavMeshAgent.ResetPath();
        boss.bossNavMeshAgent.velocity = Vector3.zero;
        boss.currentMeleeSpeed = 0f;
        boss.bossAnimator.SetTrigger("Dead");

        BossThemePlayer bossThemePlayer = boss.GetComponentInChildren<BossThemePlayer>();
        if (bossThemePlayer != null)
        {
            bossThemePlayer.transform.SetParent(null);
            bossThemePlayer.StopThemeOnBossDead();
        }

        boss.StopAllCoroutines();

        base.OnStateEnter();
    }
}
