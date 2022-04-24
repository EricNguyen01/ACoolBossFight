using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGun : BossWeapon
{
    [SerializeField] private BossBullet bulletPrefab;
    [SerializeField] private Transform gunNozzleTransform;
    [SerializeField] private float shootRange;
    [SerializeField] private float fireRate = 0.08f;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private GameObject aimLineObj;
    private float currentFireRate;

    private bool startFiring = false;

    private void Update()
    {
        if (startFiring)
        {
            if(currentFireRate >= fireRate)
            {
                SpawnBullet();
                currentFireRate = 0f;
            }

            currentFireRate += Time.deltaTime;
        }
    }

    public override void EnableBossWeapon(bool enabled)
    {
        startFiring = enabled;
        currentFireRate = fireRate;
    }

    public void EnableAimLine(bool enabled)
    {
        if (enabled) aimLineObj.SetActive(true);
        else aimLineObj.SetActive(false);
    }

    private void SpawnBullet()
    {
        //rotate the bullet to the player pos ON THE Y-AXIS only (if player not null)......................
        //if player null -> rot = some random points close to nozzle forward dir.
        Quaternion rot;
        Vector3 targetPos = new Vector3(bossHoldingThisWeapon.transform.forward.x + Random.Range(-1.5f, 1.5f), bossHoldingThisWeapon.transform.forward.y, bossHoldingThisWeapon.transform.forward.z);
        Vector3 dir;

        if (bossHoldingThisWeapon != null)
        {
            if (bossHoldingThisWeapon.playerToTarget != null)
            {
                targetPos = new Vector3(bossHoldingThisWeapon.playerToTarget.transform.position.x + Random.Range(-1.5f, 1.5f), bossHoldingThisWeapon.playerToTarget.transform.position.y + Random.Range(0.5f, 1f), bossHoldingThisWeapon.playerToTarget.transform.position.z);
            }
        }
        
        dir = targetPos - bossHoldingThisWeapon.transform.position;
        //dir.Normalize();
        rot = Quaternion.LookRotation(dir);
        //............................................................................

        //spawn and initialize bullet
        GameObject bulletObj = Instantiate(bulletPrefab.gameObject, gunNozzleTransform.position, rot);
        BossBullet bossBulletComponent = bulletObj.GetComponent<BossBullet>();
        if (bossBulletComponent == null) Debug.LogWarning("Bullet prefab: " + name + " is spawned with no boss bullet script component attached to it!");
        if(bossBulletComponent != null)
        {
            bossBulletComponent.InitializeBullet(minDamage, maxDamage, shootRange, bulletSpeed);
        }

        //bullet launch/fly using rigidbody is handled by BossBullet script component attached to bullet object spawned above
    }
}
