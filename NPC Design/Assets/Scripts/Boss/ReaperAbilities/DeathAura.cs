using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathAura : BossAbility
{
    public override void AbilityStart()
    {
        bossUsingThisAbility.bossAnimator.SetTrigger("AbilityUseA");

        base.AbilityStart();
    }

    protected override void PerformingAbility()
    {
        //spawn ability main effects
        if (!hasSpawnedMainEffects)
        {
            hasSpawnedMainEffects = true;
            if (abilityMainEffectSpawnTransform != null)
            {
                Quaternion rot = Quaternion.LookRotation(bossUsingThisAbility.transform.forward);
                AbilityEffect abilityEffect = GenerateEffect(abilityMainEffectPrefab, abilityMainEffectSpawnTransform, rot);
                if (abilityEffect != null) abilityEffect.StartEffectLastDuration();
            }
        }
        base.PerformingAbility();
    }
}
