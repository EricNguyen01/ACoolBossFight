using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDamageTriggerCollider : MonoBehaviour
{
    public Collider abilityCollider { get; private set; }
    private float minDamage = 1f;
    private float maxDamage = 2f;
    private BossAbility.StatusEffect statusEffect = BossAbility.StatusEffect.None;
    private BossAbility.DamageType damageType = BossAbility.DamageType.Instant;
    private float timeBetweenTick;
    private float currentTimeBetweenTick = 0f;
    List<ParticleSystem> particleSystems = new List<ParticleSystem>();

    public static event System.Action<AbilityDamageTriggerCollider> OnAbilityTriggerColliderSpawned;

    private void Start()
    {
        abilityCollider = GetComponent<Collider>();
        if (abilityCollider == null)
        {
            Debug.LogWarning("Cant find ability collider on ability effect: " + name);
        }
        else
        {
            OnAbilityTriggerColliderSpawned?.Invoke(this);
        }

        foreach (ParticleSystem particle in GetComponentsInChildren<ParticleSystem>())
        {
            if (!particleSystems.Contains(particle)) particleSystems.Add(particle);
        }
    }

    public void SetAbilityDamageValues(float minDamage, float maxDamage)
    {
        this.minDamage = minDamage;
        this.maxDamage = maxDamage;
    }

    public void SetAbilityStatusEffect(BossAbility.StatusEffect status)
    {
        statusEffect = status;
    }

    public void SetDamageType(BossAbility.DamageType damType)
    {
        damageType = damType;
    }

    public void SetDamageOvertimeValues(float timeBetweenTick)
    {
        this.timeBetweenTick = timeBetweenTick;
    }

    private void DisableTriggerColliderOnParticleStopEmitting()
    {
        if (particleSystems.Count == 0) return;

        for(int i = 0; i < particleSystems.Count; i++)
        {
            if (particleSystems[i].isEmitting) return;
        }

        if(abilityCollider != null) abilityCollider.enabled = false;
    }

    private void Update()
    {
        DisableTriggerColliderOnParticleStopEmitting();
    }

    //instant damage on enter trigger collider
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boss")) return;

        currentTimeBetweenTick = 0f;

        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            if (damageable.GetColliderEnabledStatus()) damageable.GetDamaged(Random.Range(minDamage, maxDamage));
        }
    }

    //Damage overtime on staying in trigger collider (for damage overtime abilities only - set by boss ability)
    private void OnTriggerStay(Collider other)
    {
        if (damageType != BossAbility.DamageType.Overtime) return;
        if (other.CompareTag("Boss")) return;

        if (currentTimeBetweenTick < timeBetweenTick)
        {
            currentTimeBetweenTick += Time.fixedDeltaTime;
            if(currentTimeBetweenTick >= timeBetweenTick)
            {
                IDamageable damageable = other.GetComponent<IDamageable>();

                if (damageable != null)
                {
                    if (damageable.GetColliderEnabledStatus()) damageable.GetDamaged(Random.Range(minDamage, maxDamage));
                }

                currentTimeBetweenTick = 0f;
            }
        }
    }
}
