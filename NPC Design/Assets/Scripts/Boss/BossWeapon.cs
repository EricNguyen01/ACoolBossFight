using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWeapon : MonoBehaviour
{
    [SerializeField] [Tooltip("If a weapon has multiple colliding parts, only use this script once and place all the parts of the weapon with a collider into this list.")]
    private List<Collider> weaponPartsColliders = new List<Collider>();
    private List<BossWeapon> bossWeaponParts = new List<BossWeapon>();//to use with weapons with multiple colliding parts (e.g: 2 fists or double blade,etc)
    public Collider weaponCollider { get; private set; }
    protected float minDamage = 1f;
    protected float maxDamage = 2f;
    protected Boss bossHoldingThisWeapon;

    protected virtual void Awake()
    {
        if (weaponPartsColliders.Count > 0)
        {
            for (int i = 0; i < weaponPartsColliders.Count; i++)
            {
                if (!weaponPartsColliders[i].isTrigger) weaponPartsColliders[i].isTrigger = true;

                BossWeapon bossWeaponComponent = weaponPartsColliders[i].GetComponent<BossWeapon>();
                if (bossWeaponComponent != null)
                {
                    if (!bossWeaponParts.Contains(bossWeaponComponent)) bossWeaponParts.Add(bossWeaponComponent);
                    continue;
                }

                bossWeaponComponent = weaponPartsColliders[i].gameObject.AddComponent<BossWeapon>();
                if (!bossWeaponParts.Contains(bossWeaponComponent)) bossWeaponParts.Add(bossWeaponComponent);

            }
        }

        weaponCollider = GetComponent<Collider>();

        if(weaponCollider != null)
        {
            weaponCollider.isTrigger = true;
        }
        else if (weaponPartsColliders.Count == 0 && weaponCollider == null)
        {
            Debug.LogWarning("Cant find weapon or weapon parts collider on weapon: " + name);
        }
    }

    public void GetBossHoldingThisWeapon(Boss boss)
    {
        bossHoldingThisWeapon = boss;
    }

    public void GetWeaponDamageValues(float minDamage, float maxDamage)
    {
        this.minDamage = minDamage;
        this.maxDamage = maxDamage;

        if(bossWeaponParts.Count > 0)
        {
            for(int i = 0; i < bossWeaponParts.Count; i++)
            {
                bossWeaponParts[i].GetWeaponDamageValues(minDamage, maxDamage);
            }
        }
    }

    public virtual void EnableBossWeapon(bool enabled)
    {
        if(weaponCollider != null) weaponCollider.enabled = enabled;
        if(bossWeaponParts.Count > 0)
        {
            for(int i = 0; i < bossWeaponParts.Count; i++)
            {
                bossWeaponParts[i].EnableBossWeapon(enabled);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boss")) return;
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            if (damageable.GetColliderEnabledStatus()) damageable.GetDamaged(Random.Range(minDamage, maxDamage), IDamageable.DamageEffect.Stagger);
        }
    }
}
