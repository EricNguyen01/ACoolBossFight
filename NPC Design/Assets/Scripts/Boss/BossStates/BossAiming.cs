using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAiming : BossState
{
    public BossAiming(Boss boss) : base(boss)
    {
    }

    public override void OnStateEnter()
    {
        if (boss.bossMeleeWeapon.GetType().Equals(typeof(BossGun)))
        {
            BossGun bossGun = (BossGun)boss.bossMeleeWeapon;
            bossGun.EnableAimLine(true);
        }

        boss.bossAnimator.SetBool("Aiming", true);
        base.OnStateEnter();
    }

    public override void OnStateUpdate()
    {
        boss.RotateBossToPos(boss.playerToTarget.transform.position + (boss.playerToTarget.transform.forward * 3f), 30f);
        base.OnStateUpdate();
    }

    public override void OnStateTransition()
    {
        boss.bossAnimator.SetBool("Aiming", false);

        if (boss.bossMeleeWeapon.GetType().Equals(typeof(BossGun)))
        {
            BossGun bossGun = (BossGun)boss.bossMeleeWeapon;
            bossGun.EnableAimLine(false);
        }

        base.OnStateTransition();
    }
}
