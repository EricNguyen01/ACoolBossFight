using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShoot : BossState
{
    public BossShoot(Boss boss) : base(boss)
    {
    }

    public override void OnStateEnter()
    {
        boss.bossNavMeshAgent.isStopped = true;
        boss.bossNavMeshAgent.ResetPath();

        boss.bossAnimator.SetBool("Shoot", true);

        if (boss.bossMeleeWeapon == null) return;
        boss.bossMeleeWeapon.GetBossHoldingThisWeapon(boss);
        boss.bossMeleeWeapon.EnableBossWeapon(true);

        base.OnStateEnter();
    }

    public override void OnStateUpdate()
    {
        if (timeInState <= 0.2f) boss.RotateBossToPos(boss.playerToTarget.transform.position);
        if(timeInState >= 1f)
        {
            if (boss.bossMeleeWeapon != null)
            {
                boss.bossMeleeWeapon.GetBossHoldingThisWeapon(boss);
                boss.bossMeleeWeapon.EnableBossWeapon(false);

                if (boss.bossMeleeWeapon.GetType().Equals(typeof(BossGun)))
                {
                    BossGun bossGun = (BossGun)boss.bossMeleeWeapon;
                    bossGun.EnableAimLine(false);
                }
            }
        }
        base.OnStateUpdate();
    }

    public override void OnStateTransition()
    {
        boss.bossAnimator.SetBool("Shoot", false);

        boss.bossNavMeshAgent.isStopped = false;

        if (boss.bossMeleeWeapon == null) return;
        boss.bossMeleeWeapon.EnableBossWeapon(false);
        base.OnStateTransition();
    }
}
