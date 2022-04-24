using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondaryBoss : Boss
{
    [field: Header("Secondary Boss Config")]
    //[field: SerializeField] public float fleeRange { get; private set; } = 20f;
    [field: SerializeField] public float aimDuration { get; private set; } = 0.67f;
    public bool hasFinishedAiming { get; set; } = false;
    [field: SerializeField] public float attackDuration { get; private set; } = 1.05f;
    [field: SerializeField] public float minTimeBeforeChangingPos { get; private set; } = 1f;
    [field: SerializeField] public float maxTimeBeforeChangingPos { get; private set; } = 3f;
    public float timeBeforePosChange { get; set; }
    public float currentTimeBeforePosChange { get; set; }

    public BossMoveToWaypoint bossMoveToWaypoint { get; private set; }
    public BossAiming bossAiming { get; private set; }
    public BossShoot bossShoot { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        currentMeleeSpeed = 0f;

        timeBeforePosChange = Random.Range(minTimeBeforeChangingPos, maxTimeBeforeChangingPos);
        currentTimeBeforePosChange = timeBeforePosChange;

        if (bossMeleeWeapon.GetType().Equals(typeof(BossGun)))
        {
            BossGun bossGun = (BossGun)bossMeleeWeapon;
            bossGun.EnableAimLine(false);
        }
    }

    protected override void Update()
    {
        if(bossHP > 0f)
        {
            ProcessPosChangeTime();
            ProcessAimAndShootTime();
        }

        base.Update();
    }
    protected override void GenerateBossStates()
    {
        bossMoveToWaypoint = new BossMoveToWaypoint(this);
        bossAiming = new BossAiming(this);
        bossShoot = new BossShoot(this);
        base.GenerateBossStates();
    }

    private void ProcessPosChangeTime()
    {
        if(currentTimeBeforePosChange < timeBeforePosChange)
        {
            currentTimeBeforePosChange += Time.deltaTime;
            if(currentTimeBeforePosChange >= timeBeforePosChange)
            {
                timeBeforePosChange = Random.Range(minTimeBeforeChangingPos, maxTimeBeforeChangingPos);
                currentTimeBeforePosChange = timeBeforePosChange;
            }
        }
    }

    public void ResetPosChangeTime()
    {
        currentTimeBeforePosChange = 0f;
    }

    private void ProcessAimAndShootTime()
    {
        if (bossCurrentState == null) return;

        if(currentMeleeSpeed >= bossMeleeSpeed)
        {
            if (bossCurrentState.GetType().Equals(bossAiming.GetType()))
            {
                if (bossCurrentState.GetTimeInState() >= aimDuration)
                {
                    hasFinishedAiming = true;
                }
            }
            else if (bossCurrentState.GetType().Equals(bossShoot.GetType()))
            {
                if (bossCurrentState.GetTimeInState() >= attackDuration)
                {
                    currentMeleeSpeed = 0f;
                    hasFinishedAiming = false;
                }
            }
        }
    }
}
