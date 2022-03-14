using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HellfireTrail : BossAbility
{
    private Vector3 aimLookAheadPos;

    public override void AbilityStart()
    {
        bossUsingThisAbility.bossAnimator.SetTrigger("AbilityUseB");
       
        base.AbilityStart();
    }

    protected override void CastingAbility()
    {
        aimLookAheadPos = bossUsingThisAbility.playerToTarget.transform.position + bossUsingThisAbility.transform.forward * bossUsingThisAbility.playerToTarget.currentMoveSpeed * 2f;
        bossUsingThisAbility.RotateBossToPos(aimLookAheadPos, 20f);

        base.CastingAbility();
    }

    /*protected override void OnAbilityFinished()
    {
        base.OnAbilityFinished();
    }*/

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
