using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public Collider weaponCollider { get; private set; }
    private float minDamage = 1f;
    private float maxDamage = 2f;
    private List<IDamageable> objectsHit = new List<IDamageable>();

    private void Awake()
    {
        weaponCollider = GetComponent<Collider>();
        if(weaponCollider == null)
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
        if (other.CompareTag("Player")) return;
        IDamageable damageable = other.GetComponent<IDamageable>();
        if(damageable != null)
        {
            if (!objectsHit.Contains(damageable))
            {
                objectsHit.Add(damageable);
                if(damageable.GetColliderEnabledStatus()) damageable.GetDamaged(Random.Range(minDamage, maxDamage));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) return;
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            if (objectsHit.Contains(damageable))
            {
                objectsHit.Remove(damageable);
            }
        }
    }
}
