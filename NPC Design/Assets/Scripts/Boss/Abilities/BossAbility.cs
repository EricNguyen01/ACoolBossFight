using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Werewolf.StatusIndicators.Components;

public class BossAbility : MonoBehaviour
{
    protected BossAbilityInventory bossAbilityInventoryHoldingThisAbility;
    protected Boss bossUsingThisAbility;

    [field: SerializeField] public bool isAbilityDisabled { get; private set; } = false;

    public enum StatusEffect { None, KnockUp }
    public enum DamageType { Instant, Overtime }

    [field: Header("Ability Damage Type and Status Effects")]
    [field: SerializeField] public StatusEffect abilityStatusEffect { get; private set; } = StatusEffect.None;
    [field: SerializeField] public DamageType abilityDamageType { get; private set; } = DamageType.Instant;

    [field: Header("Damage Overtime Config")]
    [field: SerializeField] public float timeBetweenTick { get; private set; } = 1f;
    public float damageOvertimeTotalDuration { get; private set; }

    [field: Header("Ability Basic Stats")]
    [field: SerializeField] public float abilityMinDamage { get; private set; }
    [field: SerializeField] public float abilityMaxDamage { get; private set; }
    [field: SerializeField] public float abilityCastTime { get; private set; }
    public float timeCastingAbility { get; private set; } = 0f;
    [field: SerializeField] public float abilityDuration { get; private set; }
    protected float timePerformingAbility = 0f;
    [field: SerializeField] public float abilityCooldownTime { get; private set; }
    [field: SerializeField] protected AbilityEffect abilityMainEffectPrefab { get; private set; }
    [field: SerializeField] protected Transform abilityMainEffectSpawnTransform { get; private set; }
    [field: SerializeField] protected AbilityEffect rightHandCastingEffectPrefab { get; private set; }
    [field: SerializeField] protected Transform rightHandEffectSpawnTransform { get; private set; }
    [field: SerializeField] protected AbilityEffect leftHandCastingEffectPrefab { get; private set; }
    [field: SerializeField] protected Transform leftHandEffectSpawnTransform { get; private set; }

    [Header("Ability Indicators")]
    [SerializeField] protected Cone abilityConeIndicator;
    [SerializeField] protected LineMissile abilityRangeIndicator;
    [SerializeField] protected float abilityIndicatorRange;
    [SerializeField] protected float abilityIndicatorWidth;

    protected bool hasSpawnedCastingEffects = false;
    protected bool hasSpawnedMainEffects = false;

    public static event System.Action<BossAbility> OnAbilityUsed;
    public static event System.Action<BossAbility> OnAbilityEnded;


    public virtual void Awake()
    {
        damageOvertimeTotalDuration = abilityDuration;

        bossAbilityInventoryHoldingThisAbility = GetComponentInParent<BossAbilityInventory>();

        if(bossAbilityInventoryHoldingThisAbility == null)
        {
            Debug.LogError("Found no boss ability inventory to hold this ability game object: " + name + ". Disabling...");
            gameObject.SetActive(false);
            return;
        }

        if(abilityMainEffectPrefab == null || rightHandCastingEffectPrefab == null || leftHandCastingEffectPrefab == null)
        {
            Debug.LogError("Found no required Ability Effect Prefab components on ability: " + name + " Disabling...");
            gameObject.SetActive(false);
            return;
        }

        if (abilityConeIndicator != null) abilityConeIndicator.gameObject.SetActive(false);
        if (abilityRangeIndicator != null) abilityRangeIndicator.gameObject.SetActive(false);

    }

    public virtual void Start()
    {
        if (bossAbilityInventoryHoldingThisAbility.bossHoldingThisAbilityInventory == null)
        {
            Debug.LogError("Found no boss holding the inventory that is holding this ability: " + name + ". Disabling...");
            gameObject.SetActive(false);
            return;
        }

        bossUsingThisAbility = bossAbilityInventoryHoldingThisAbility.bossHoldingThisAbilityInventory;
    }

    public virtual void UseAbility()
    {
        timePerformingAbility = 0f;
        timeCastingAbility = 0f;
        bossUsingThisAbility.currentWaitTimeBetweenAbilities = Random.Range(bossUsingThisAbility.minWaitTimeBetweenAbilities, bossUsingThisAbility.maxWaitTimeBetweenAbilities);
        OnAbilityUsed?.Invoke(this);
        StartCoroutine(CooldownTimer());
    }

    public virtual void AbilityStart()
    {
        hasSpawnedCastingEffects = false;
        hasSpawnedMainEffects = false;

        bossUsingThisAbility.bossNavMeshAgent.isStopped = true;
        bossUsingThisAbility.bossNavMeshAgent.ResetPath();

        if (abilityConeIndicator != null)
        {
            abilityConeIndicator.Scale = abilityIndicatorRange;
            abilityConeIndicator.gameObject.SetActive(true);
        }

        if(abilityRangeIndicator != null)
        {
            abilityRangeIndicator.MinimumRange = abilityIndicatorRange;
            abilityRangeIndicator.Range = abilityIndicatorRange;
            abilityRangeIndicator.Width = abilityIndicatorWidth;
            abilityRangeIndicator.gameObject.SetActive(true);
        }
    }

    public void AbilitySequenceUpdate()
    {
        if (abilityCastTime <= 0f)
        {
            PerformingAbility();
            return;
        }

        if (timeCastingAbility < abilityCastTime)
        {
            CastingAbility();
        }
        else
        {
            PerformingAbility();
        }
    }

    protected virtual void CastingAbility()
    {
        //spawn casting effects
        if (!hasSpawnedCastingEffects)
        {
            hasSpawnedCastingEffects = true;

            Quaternion rot = Quaternion.LookRotation(bossUsingThisAbility.transform.forward);

            if (rightHandEffectSpawnTransform != null)
            {
                AbilityEffect abilityEffect = GenerateEffect(rightHandCastingEffectPrefab, rightHandEffectSpawnTransform, rot);
                if (abilityEffect != null)
                {
                    abilityEffect.ModifyEffectLastDuration(abilityCastTime);
                    abilityEffect.StartEffectLastDuration();
                }
            }
            if (leftHandEffectSpawnTransform != null)
            {
                AbilityEffect abilityEffect = GenerateEffect(leftHandCastingEffectPrefab, leftHandEffectSpawnTransform, rot);
                if (abilityEffect != null)
                {
                    abilityEffect.ModifyEffectLastDuration(abilityCastTime);
                    abilityEffect.StartEffectLastDuration();
                }
            }
        }

        if (timeCastingAbility < abilityCastTime)
        {
            timeCastingAbility += Time.deltaTime;

            if (abilityConeIndicator != null) abilityConeIndicator.Progress = timeCastingAbility / abilityCastTime;
            if (abilityRangeIndicator != null)
            {
                abilityRangeIndicator.Progress = timeCastingAbility / abilityCastTime;
                abilityRangeIndicator.transform.rotation = Quaternion.LookRotation(bossUsingThisAbility.transform.forward);
            }
            
            if(timeCastingAbility >= abilityCastTime)
            {
                timeCastingAbility = abilityCastTime;
                if (abilityConeIndicator != null) abilityConeIndicator.gameObject.SetActive(false);
                if (abilityRangeIndicator != null) abilityRangeIndicator.gameObject.SetActive(false);
            }
        }
    }

    protected virtual void PerformingAbility()
    {
        if(abilityDuration <= 0f)
        {
            OnAbilityFinished();
            return;
        }

        if (timePerformingAbility < abilityDuration)
        {
            timePerformingAbility += Time.deltaTime;
            if (timePerformingAbility >= abilityDuration)
            {
                timePerformingAbility = abilityDuration;
                OnAbilityFinished();
            }
        }
    }

    protected virtual void OnAbilityFinished()
    {
        OnAbilityEnded?.Invoke(this);
    }

    public IEnumerator CooldownTimer()
    {
        yield return new WaitForSeconds(abilityCooldownTime);
        bossAbilityInventoryHoldingThisAbility.EnqueueAbility(this);
    }

    protected AbilityEffect GenerateEffect(AbilityEffect effect, Transform spawn, Quaternion rot)
    {
        GameObject effectObj = Instantiate(effect.gameObject, spawn.position, rot, spawn);
       
        AbilityEffect abilityEffect = effectObj.GetComponent<AbilityEffect>();

        if(abilityEffect != null)
        {
            abilityEffect.SetBossAbilityThatGeneratedThisAbilityEffect(this);
            return abilityEffect;
            /*if (abilityDamageType == DamageType.Instant)
            {
                abilityEffect.AbilityEffectInstantDamageConfig(abilityMinDamage, abilityMaxDamage, abilityStatusEffect);
                return abilityEffect;
            }

            if(abilityDamageType == DamageType.Overtime)
            {
                abilityEffect.AbilityEffectOvertimeDamageConfig(abilityMinDamage, abilityMaxDamage, timeBetweenTick, abilityStatusEffect);
                return abilityEffect;
            }*/
        }
        return null;
    }
}
