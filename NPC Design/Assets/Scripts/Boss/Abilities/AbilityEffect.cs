using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityEffect : MonoBehaviour
{
    [SerializeField] private float effectLastDuration;
    [SerializeField] List<AbilityDamageTriggerCollider> abilityDamageTriggerColliders = new List<AbilityDamageTriggerCollider>();
    private BossAbility bossAbility;

    private void OnEnable()
    {
        /*foreach(AbilityDamageTriggerCollider damageCollider in GetComponentsInChildren<AbilityDamageTriggerCollider>())
        {
            if (!abilityDamageTriggerColliders.Contains(damageCollider)) abilityDamageTriggerColliders.Add(damageCollider);
        }*/
        AbilityDamageTriggerCollider.OnAbilityTriggerColliderSpawned += RegisterAbilityCollider;
    }

    private void OnDisable()
    {
        AbilityDamageTriggerCollider.OnAbilityTriggerColliderSpawned -= RegisterAbilityCollider;
    }

    public void StartEffectLastDuration()
    {
        Destroy(gameObject, effectLastDuration);
    }

    public void ModifyEffectLastDuration(float time)
    {
        effectLastDuration = time;
    }

    public void SetBossAbilityThatGeneratedThisAbilityEffect(BossAbility bossAbility)
    {
        this.bossAbility = bossAbility;
    }

    public void RegisterAbilityCollider(AbilityDamageTriggerCollider collider)
    {
        if(collider.transform.parent == transform)
        {
            if (!abilityDamageTriggerColliders.Contains(collider))
            {
                abilityDamageTriggerColliders.Add(collider);

                if(bossAbility != null)
                {
                    collider.SetAbilityDamageValues(bossAbility.abilityMinDamage, bossAbility.abilityMaxDamage);
                    collider.SetAbilityStatusEffect(bossAbility.abilityStatusEffect);

                    if (bossAbility.abilityDamageType == BossAbility.DamageType.Overtime)
                    {
                        collider.SetDamageOvertimeValues(bossAbility.timeBetweenTick);
                        collider.SetDamageType(BossAbility.DamageType.Overtime);
                    }
                }
            }
        }
    }

    /*public void AbilityEffectInstantDamageConfig(float minDam, float maxDam, BossAbility.StatusEffect statusEffect)
    {
        if (abilityDamageTriggerColliders.Count == 0) return;
        for(int i = 0; i < abilityDamageTriggerColliders.Count; i++)
        {
            abilityDamageTriggerColliders[i].SetAbilityDamageValues(minDam, maxDam);
            abilityDamageTriggerColliders[i].SetAbilityStatusEffect(statusEffect);
        }
    }

    public void AbilityEffectOvertimeDamageConfig(float minDam, float maxDam, float timeBetweenTick, BossAbility.StatusEffect statusEffect)
    {
        if (abilityDamageTriggerColliders.Count == 0) return;
        for (int i = 0; i < abilityDamageTriggerColliders.Count; i++)
        {
            abilityDamageTriggerColliders[i].SetAbilityDamageValues(minDam, maxDam);
            abilityDamageTriggerColliders[i].SetAbilityStatusEffect(statusEffect);
            abilityDamageTriggerColliders[i].SetDamageOvertimeValues(timeBetweenTick);
            abilityDamageTriggerColliders[i].SetDamageType(BossAbility.DamageType.Overtime);
        }
    }*/
}
