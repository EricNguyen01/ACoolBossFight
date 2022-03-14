using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossUsingAbility : BossState
{
    public BossUsingAbility(Boss boss) : base(boss)
    {
    }

    public override void OnStateEnter()
    {
        boss.currentBossAbilityInUse.AbilityStart();
        base.OnStateEnter();
    }

    public override void OnStateTransition()
    {
        boss.bossNavMeshAgent.isStopped = false;
        base.OnStateTransition();
    }

    public override void OnStateUpdate()
    {
        boss.currentBossAbilityInUse.AbilitySequenceUpdate();
        base.OnStateUpdate();
    }
}
