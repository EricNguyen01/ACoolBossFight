using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWeapon : MonoBehaviour
{
    public Collider weaponCollider { get; private set; }
    private float minDamage = 1f;
    private float maxDamage = 2f;

    private void Awake()
    {
        weaponCollider = GetComponent<Collider>();
        if (weaponCollider == null)
        {
            Debug.LogWarning("Cant find weapon collider on weapon: " + name);
        }
    }

    public void GetWeaponDamageValues(float minDamage, float maxDamage)
    {
        this.minDamage = minDamage;
        this.maxDamage = maxDamage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boss")) return;
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            if (damageable.GetColliderEnabledStatus()) damageable.GetDamaged(Random.Range(minDamage, maxDamage));
        }
    }
}
