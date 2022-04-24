using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : MonoBehaviour, IDamageable
{
    public Player playerToTarget { get; set; }

    [field: Header("Boss Stats")]
    [field: SerializeField] public string bossName { get; private set; }
    [field: SerializeField] public float bossHP { get; private set; } = 5000;
    [field: SerializeField] public float bossSpeed { get; private set; } = 5f;
    public float currentMoveSpeed { get; set; }
    [field: SerializeField] public float bossTriggerRange { get; private set; } = 16.45f;

    [field: Header("Boss Melee Stats")]
    [field: SerializeField] public BossWeapon bossMeleeWeapon { get; private set; }
    [field: SerializeField] public float bossMeleeRange { get; private set; } = 5f;
    [field: SerializeField] public float bossMeleeMinDamage { get; private set; } = 14.8f;
    [field: SerializeField] public float bossMeleeMaxDamage { get; private set; } = 23.25f;
    [field: SerializeField] public float bossMeleeSpeed { get; private set; } = 1.125f;
    public float currentMeleeSpeed { get; set; }
    [field: SerializeField] public float bossMeleeComboCooldown { get; private set; } = 1.3f;
    public float currentMeleeComboCooldown { get; set; } = 0f;
    public int bossMeleeNum { get; private set; } = 2;
    public int currentMeleeNum { get; set; } = 0;
    public bool isInMeleeRange { get; set; }

    [field: Header("Boss Ability Use Stats")]
    [field: SerializeField] public float minWaitTimeBetweenAbilities { get; private set; } = 6.48f;
    [field: SerializeField] public float maxWaitTimeBetweenAbilities { get; private set; } = 14.75f;
    public float currentWaitTimeBetweenAbilities { get; set; } = 0f;

    private float minAvailableAbilityCheckInterval = 1f;
    private float maxAvailableAbilityCheckInterval = 3f;
    public BossAbility currentBossAbilityInUse { get; private set; }
    public bool isPerformingAbility { get; private set; } = false;

    public BossIdle bossIdle { get; private set; }
    public BossMove bossMove { get; private set; }
    public BossMelee bossMelee { get; private set; }
    public BossDead bossDead { get; private set; }
    public BossUsingAbility bossUsingAbility { get; private set; }
    public BossState bossCurrentState { get; private set; }

    public NavMeshAgent bossNavMeshAgent { get; private set; }
    public Animator bossAnimator { get; private set; }
    public Collider bossCollider { get; private set; }
    public BossAbilityInventory bossAbilityInventory { get; private set; }
    public BossHealthBarUI bossHealthBarUI { get; set; }

    protected virtual void Awake()
    {
        bossCollider = GetComponent<Collider>();
        bossAbilityInventory = GetComponentInChildren<BossAbilityInventory>();
        if (bossAbilityInventory == null) Debug.LogWarning("Boss Ability Inventory children game object not found!");
        else bossAbilityInventory.GetBossHoldingThisAbilityInventory(this);

        bossNavMeshAgent = GetComponent<NavMeshAgent>();
        if (bossNavMeshAgent == null) bossNavMeshAgent = gameObject.AddComponent<NavMeshAgent>();
        bossNavMeshAgent.speed = bossSpeed;

        bossAnimator = GetComponent<Animator>();
        if (bossAnimator == null) bossAnimator = gameObject.AddComponent<Animator>();

        currentMeleeSpeed = bossMeleeSpeed;

        if (bossMeleeWeapon != null)
        {
            bossMeleeWeapon.GetWeaponDamageValues(bossMeleeMinDamage, bossMeleeMaxDamage);
            bossMeleeWeapon.EnableBossWeapon(false);
        }

        GenerateBossStates();
    }

    private void OnEnable()
    {
        BossAbility.OnAbilityUsed += OnAbilityUsed;
        BossAbility.OnAbilityEnded += OnAbilityEnded;

        BossTriggerDome bossTriggerDome = GetComponentInChildren<BossTriggerDome>();
        if (bossTriggerDome != null)
        {
            if (bossTriggerDome.triggerDomeCollider != null) bossTriggerDome.triggerDomeCollider.radius = bossTriggerRange;
        }
    }

    private void OnDisable()
    {
        BossAbility.OnAbilityUsed -= OnAbilityUsed;
        BossAbility.OnAbilityEnded -= OnAbilityEnded;
    }

    private void OnDestroy()
    {
        if(bossHealthBarUI != null) bossHealthBarUI.gameObject.SetActive(false);
    }

    /*private void Start()
    {
        
    }*/

    protected virtual void Update()
    {
        if(bossHP > 0f)
        {
            IsPlayerInRange();
            BossMeleeComboResetAndCooldown();
            if(bossCurrentState != null) bossCurrentState.OnStateUpdate();
        }
    }

    public void OnAnimationEventEnded()
    {
        bossCurrentState.OnAnimationEventEnded();
    }

    public void OnAnimationEventStarted()
    {
        bossCurrentState.OnAnimationEventStarted();
    }

    public void StartCheckingForAvailableAbilityToUse()
    {
        if (bossAbilityInventory == null) return;
        if (bossAbilityInventory.bossAbilities.Count == 0) return;
        StartCoroutine(CheckForAvailableAbilityToUse());
    }

    private IEnumerator CheckForAvailableAbilityToUse()
    {
        while (bossHP > 0f)
        {
            if(!bossCurrentState.GetType().Equals(bossMelee.GetType()) && !bossCurrentState.GetType().Equals(bossUsingAbility.GetType()))
            {
                if(bossAbilityInventory != null && bossAbilityInventory.availableAbilityQueue.Count > 0)
                {
                    if (currentWaitTimeBetweenAbilities <= 0f)
                    {
                        currentMeleeSpeed = 0f;
                        bossAbilityInventory.UseFirstAvailableAbilityInQueue();
                    }
                }
            }
            yield return new WaitForSeconds(Random.Range(minAvailableAbilityCheckInterval, maxAvailableAbilityCheckInterval));
        }
    }

    public void GetDamaged(float damage, IDamageable.DamageEffect damageEffect)
    {
        if(bossHP > 0f)
        {
            bossHP -= damage;
            if (bossHP <= 0f) bossHP = 0f;
        }

        if (bossHealthBarUI != null) bossHealthBarUI.SetAndDisplayHealthBarUI(bossHP);
    }

    public void GetDamageDirection(Vector3 dir)
    {

    }

    public void OnAbilityUsed(BossAbility ability)
    {
        currentBossAbilityInUse = ability;
        isPerformingAbility = true;
    }

    public void OnAbilityEnded(BossAbility ability)
    {
        if(ability == currentBossAbilityInUse)
        {
            isPerformingAbility = false;
            if(currentWaitTimeBetweenAbilities <= 0f) currentWaitTimeBetweenAbilities = Random.Range(minWaitTimeBetweenAbilities, maxWaitTimeBetweenAbilities);
            StartCoroutine(WaitTimeBetweenAbilitiesTimer(currentWaitTimeBetweenAbilities));
        }
    }

    private IEnumerator WaitTimeBetweenAbilitiesTimer(float time)
    {
        yield return new WaitForSeconds(time);
        currentWaitTimeBetweenAbilities = 0f;
    }

    public void RotateBossToPos(Vector3 pos)
    {
        Quaternion rot = RotationToPos(pos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, 1.4f * Time.fixedDeltaTime);
    }

    public void RotateBossToPos(Vector3 pos, float rotSpeedMultiplier)
    {
        Quaternion rot = RotationToPos(pos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, 1.4f * rotSpeedMultiplier * Time.fixedDeltaTime);
    }

    public Quaternion RotationToPos(Vector3 pos)
    {
        Vector3 dir = pos - transform.position;
        //if (dir.magnitude < 0.15f) return Quaternion.Euler(transform.forward);
        dir.Normalize();
        Vector3 dirWithoutY = new Vector3(dir.x, 0f, dir.z);
        Quaternion rot = Quaternion.LookRotation(dirWithoutY);
        return rot;
    }

    protected virtual void GenerateBossStates()
    {
        bossIdle = new BossIdle(this);
        bossMove = new BossMove(this);
        bossMelee = new BossMelee(this);
        bossDead = new BossDead(this);
        bossUsingAbility = new BossUsingAbility(this);
    }

    public void EnableBossState(BossState state)
    {
        if(bossCurrentState == null)
        {
            bossCurrentState = state;
            bossCurrentState.OnStateEnter();
            return;
        }

        bossCurrentState.OnStateTransition();
        bossCurrentState = state;
        bossCurrentState.OnStateEnter();
    }

    public bool IsPlayerInRange()
    {
        if (playerToTarget == null) return false;
        float dist = Vector3.Distance(playerToTarget.transform.position, transform.position);
        if (dist <= bossMeleeRange) return true;
        return false;
    }

    private void BossMeleeComboResetAndCooldown()
    {
        if (currentMeleeComboCooldown >= bossMeleeComboCooldown)
        {
            currentMeleeComboCooldown -= Time.deltaTime;
            if (currentMeleeComboCooldown <= 0f)
            {
                currentMeleeComboCooldown = 0f;
                currentMeleeNum = 0;
            }
        }

        if (currentMeleeSpeed < bossMeleeSpeed)
        {
            currentMeleeSpeed += Time.deltaTime;
            if (currentMeleeSpeed >= bossMeleeSpeed)
            {
                currentMeleeSpeed = bossMeleeSpeed;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if(playerToTarget == null)Gizmos.DrawWireSphere(transform.position, bossTriggerRange);
    }

    public bool GetColliderEnabledStatus()
    {
        if (bossCollider == null) return false;
        return bossCollider.enabled;
    }

    public void DestroyBossOnDeadAnimationEnded()
    {
        Destroy(gameObject, 1f);
    }
}
