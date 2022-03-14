using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBefall : BossAbility
{
    public override void AbilityStart()
    {
        bossUsingThisAbility.bossAnimator.SetTrigger("AbilityUseB");

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
                AbilityEffect abilityEffect = GenerateEffect(abilityMainEffectPrefab, abilityMainEffectSpawnTransform, Quaternion.identity);
                abilityEffect.transform.SetParent(null);
                if (abilityEffect != null) abilityEffect.StartEffectLastDuration();
            }
        }

        base.PerformingAbility();
    }
}
