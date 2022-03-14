using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour, IDamageable, IHeal
{
    [field: Header("Player Stats")]
    [field: SerializeField] public float playerHP { get; private set; }
    private float baseHP;
    [field: SerializeField] public float moveSpeed { get; private set; }
    public float currentMoveSpeed { get; set; } = 0f;
    [field: SerializeField] public float turnSpeed { get; private set; } = 0.8f;

    [field: Header("Player Attack Stats")]
    [field: SerializeField] public PlayerWeapon playerMeleeWeapon { get; private set; }
    [field: SerializeField] public float playerDamageMin { get; private set; } = 8f;
    [field: SerializeField] public float playerDamageMax { get; private set; } = 30f;
    public int playerAttackComboNum { get; private set; } = 4;
    public int currentAttackNum { get; set; } = 0;
    [field: SerializeField] public float playerAttackSpeed { get; private set; } = 0.4f;
    public float currentAttackSpeed { get; set; } = 0f;
    public float playerAttackComboResetTime { get; private set; } = 0.45f;
    public float currentComboResetTime { get; set; } = 0f;
    public bool isMeleeing { get; set; } = false;

    [field: Header("Player Roll")]
    [field: SerializeField] public int playerRollNumber { get; private set; } = 2;
    public int currentRollNumber { get; set; } = 0;
    [field: SerializeField] public float playerRollDistance { get; private set; } = 1.3f;
    public float playerRollTime { get; private set; } = 0.85f;//roll time = roll animation time
    [field: SerializeField] public float playerRollCooldown { get; private set; } = 1.37f;
    public float currentRollCooldown { get; set; } = 0f;
    public float playerRollWaitTime { get; private set; } = 0.1f;
    public float currentPlayerRollWaitTime { get; set; } = 0f;
    public bool isRolling { get; set; }

    public Vector3 currentRightMouseClickPos { get; set; }
    public Vector3 currentLeftMouseClickPos { get; private set; }

    public PlayerIdle playerIdle { get; private set; }
    public PlayerMove playerMove { get; private set; }
    public PlayerRoll playerRoll { get; private set; }
    public PlayerMelee playerMelee { get; private set; }
    public PlayerDead playerDead { get; private set; }
    public PlayerState playerCurrentState { get; private set; }

    public Camera mainCam { get; private set; }
    public Animator playerAnimator { get; private set; }
    public Collider playerCollider { get; private set; }
    public NavMeshAgent playerNavMesh { get; private set; }
    private NavMeshPath path;
    public PlayerHealthBarUI playerHeathBarUI { get; set; }
    public PlayerRollCooldownIndicator rollCooldownIndicator { get; set; }

    private PlayerClickMoveEffect playerClickMoveEffect;

    private void Awake()
    {
        baseHP = playerHP;
        mainCam = Camera.main;
        playerCollider = GetComponent<Collider>();
        playerAnimator = GetComponent<Animator>();
        playerNavMesh = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();

        playerClickMoveEffect = GetComponent<PlayerClickMoveEffect>();

        if (playerNavMesh == null) playerNavMesh = gameObject.AddComponent<NavMeshAgent>();
        playerNavMesh.speed = moveSpeed;

        if(playerMeleeWeapon != null)
        {
            playerMeleeWeapon.GetWeaponDamageValues(playerDamageMin, playerDamageMax);
            if (playerMeleeWeapon.weaponCollider != null) playerMeleeWeapon.weaponCollider.enabled = false;
        }

        GeneratePlayerStates();
    }

    private void Update()
    {
        if(playerHP > 0f)
        {
            GetValidMoveClickPos();
            OtherPlayerInputs();
            ProcessPlayerRollNumberAndWaitTime();
            ProcessMeleeComboResetAndCooldown();
            playerCurrentState.OnStateUpdate();
        }
    }

    private void FixedUpdate()
    {
        if(playerHP > 0f)
        {
            playerCurrentState.OnStateFixedUpdate();
        }
    }

    public void OnUnityAnimationEventTriggered()
    {
        playerCurrentState.OnUnityAnimationEventTriggered();
    }

    public void OnUnityAnimationEventEnded()
    {
        playerCurrentState.OnUnityAnimationEventEnded();
    }

    private void GetValidMoveClickPos()
    {
        bool leftClick = Input.GetMouseButtonDown(0);
        bool rightClick = Input.GetMouseButtonDown(1);

        if (leftClick || rightClick)
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Walkable")))
            {
                if (leftClick)
                {
                    currentLeftMouseClickPos = hit.point;
                    if(currentAttackSpeed >= playerAttackSpeed) isMeleeing = true;
                }
                if (rightClick)
                {
                    isMeleeing = false;
                    CalculateValidMoveToRightClickPos(hit);
                    if (playerClickMoveEffect != null) playerClickMoveEffect.SpawnClickMoveEffectAtPos(hit.point);
                }
            }
        }
    }

    private void CalculateValidMoveToRightClickPos(RaycastHit hit)
    {
        if (Vector3.Distance(transform.position, hit.point) >= 0.45f)
        {
            playerNavMesh.CalculatePath(hit.point, path);

            if (path.status == NavMeshPathStatus.PathComplete) currentRightMouseClickPos = hit.point;

            else
            {
                Vector3 moveDir = hit.point - transform.position;
                Vector3 posOnMoveDir;
                float normalizedMagnitude = 0f;
                Vector3 lastValidPos = Vector3.zero;

                while (normalizedMagnitude < 1)
                {
                    normalizedMagnitude += Time.deltaTime;

                    if (normalizedMagnitude >= 1f) normalizedMagnitude = 1f;

                    posOnMoveDir = transform.position + moveDir * (moveDir.magnitude * normalizedMagnitude);

                    NavMeshPath p = new NavMeshPath();

                    playerNavMesh.CalculatePath(posOnMoveDir, p);

                    if (p.status == NavMeshPathStatus.PathComplete)
                    {
                        lastValidPos = posOnMoveDir;
                        continue;
                    }

                    break;
                }

                currentRightMouseClickPos = lastValidPos;
            }
        }
    }

    public void RotatePlayerToPos(Vector3 pos)
    {
        Quaternion rot = PlayerRotationToPos(pos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, turnSpeed * Time.fixedDeltaTime);
    }

    public void RotatePlayerToPos(Vector3 pos, float rotSpeedMultiplier)
    {
        Quaternion rot = PlayerRotationToPos(pos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, turnSpeed * rotSpeedMultiplier * Time.fixedDeltaTime);
    }

    private Quaternion PlayerRotationToPos(Vector3 pos)
    {
        Vector3 dir = pos - transform.position;
        if (dir.magnitude < 0.3f) return Quaternion.Euler(transform.forward);
        dir.Normalize();
        Vector3 dirWithoutY = new Vector3(dir.x, 0f, dir.z);
        Quaternion rot = Quaternion.LookRotation(dirWithoutY);
        return rot;
    }

    private void GeneratePlayerStates()
    {
        playerIdle = new PlayerIdle(this);
        playerMove = new PlayerMove(this);
        playerRoll = new PlayerRoll(this);
        playerMelee = new PlayerMelee(this);
        playerDead = new PlayerDead(this);
    }

    public void EnablePlayerState(PlayerState state)
    {
        if (playerCurrentState == null)
        {
            playerCurrentState = state;
            playerCurrentState.OnStateEnter();
            return;
        }

        playerCurrentState.OnStateTransition();
        playerCurrentState = state;
        playerCurrentState.OnStateEnter();

    }

    public bool HasArrived()
    {
        if (currentRightMouseClickPos == Vector3.zero) return true;

        //float dist = Vector3.Distance(transform.position, currentRightMouseClickPos);

        if (playerNavMesh.hasPath && playerNavMesh.remainingDistance <= playerNavMesh.stoppingDistance)
        {
            //on arriving, reset right mouse click pos until the player clicks on a new position
            //this ensures that on arriving, the Idle state is forced to be active and the player character is coming to a full stop
            //even if target destination has been passed. This avoids the character running in circle because its stuck in "moving to" and "arriving"
            currentRightMouseClickPos = Vector3.zero; 
            return true;
        }

        return false;
    }

    private void OtherPlayerInputs()
    {
        if(!isRolling && Input.GetKeyDown(KeyCode.Space))
        {
            isRolling = true;//NodeCanvas FSM transitions to RollState
        }
    }

    private void ProcessPlayerRollNumberAndWaitTime()
    {
        if(currentPlayerRollWaitTime > 0f)
        {
            currentPlayerRollWaitTime -= Time.deltaTime;
            if(currentPlayerRollWaitTime <= 0f)
            {
                currentPlayerRollWaitTime = 0f;
            }
        }

        if (currentRollCooldown > 0f)
        {
            currentRollCooldown -= Time.deltaTime;

            if (currentRollNumber >= playerRollNumber) rollCooldownIndicator.EnableAndDisplayRollCooldownUI(playerRollCooldown, 0f, currentRollCooldown);

            if (currentRollCooldown <= 0f)
            {
                currentRollNumber = 0;
            }
        }
    }

    private void ProcessMeleeComboResetAndCooldown()
    {
        if(currentComboResetTime >= playerAttackComboResetTime)
        {
            currentComboResetTime -= Time.deltaTime;
            if(currentComboResetTime <= 0f)
            {
                currentComboResetTime = 0f;
                currentAttackNum = 0;
            }
        }

        if(currentAttackSpeed < playerAttackSpeed)
        {
            currentAttackSpeed += Time.deltaTime;
            if(currentAttackSpeed >= playerAttackSpeed)
            {
                currentAttackSpeed = playerAttackSpeed;
            }
        }
    }

    public void GetDamaged(float damage)
    {
        if (playerHP > 0f)
        {
            playerHP -= damage;
            if (playerHP <= 0f) playerHP = 0f;
        }

        if (playerHeathBarUI != null) playerHeathBarUI.SetAndDisplayHealthBarUI(playerHP);
    }
    public bool GetColliderEnabledStatus()
    {
        if (playerCollider == null) return false;
        return playerCollider.enabled;
    }

    public bool ShouldReceiveHealing()
    {
        if (playerHP >= baseHP) return false;
        return true;
    }

    public void OnHeal(float healingAmount)
    {
        if (playerHP > 0f)
        {
            playerHP += healingAmount;
            if (playerHP >= baseHP) playerHP = baseHP;
        }
        if (playerHeathBarUI != null) playerHeathBarUI.SetAndDisplayHealthBarUI(playerHP);
    }
}
