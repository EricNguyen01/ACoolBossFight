using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BossMelee : BossState
{
    public BossMelee(Boss boss) : base(boss)
    {

    }

    public override void OnStateEnter()
    {
        boss.bossNavMeshAgent.isStopped = true;
        boss.bossNavMeshAgent.ResetPath();

        boss.bossAnimator.SetFloat("MeleeNum", boss.currentMeleeNum);
        boss.bossAnimator.SetTrigger("Melee");

        if (boss.bossMeleeWeapon == null) return;
        if (boss.bossMeleeWeapon.weaponCollider == null) return;
        boss.bossMeleeWeapon.weaponCollider.enabled = true;

        base.OnStateEnter();
    }

    public override void OnStateUpdate()
    {
        if(timeInState <= 0.25f) boss.RotateBossToPos(boss.playerToTarget.transform.position);
        base.OnStateUpdate();
    }

    public override void OnStateTransition()
    {
        if (boss.currentMeleeNum == boss.bossMeleeNum) boss.currentMeleeNum = 0;
        else boss.currentMeleeNum += 1;

        boss.bossNavMeshAgent.isStopped = false;

        if (boss.bossMeleeWeapon == null) return;
        if (boss.bossMeleeWeapon.weaponCollider == null) return;
        boss.bossMeleeWeapon.weaponCollider.enabled = false;

        base.OnStateTransition();
    }

    public override void OnAnimationEventEnded()
    {
        boss.currentMeleeSpeed = 0f;
        base.OnAnimationEventEnded();
    }
}
