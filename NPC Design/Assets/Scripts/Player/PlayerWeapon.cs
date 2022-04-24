using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField]
    [Tooltip("If a weapon has multiple colliding parts, only use this script once and place all the parts of the weapon with a collider into this list.")]
    private List<Collider> weaponPartsColliders = new List<Collider>();
    private List<PlayerWeapon> playerWeaponParts = new List<PlayerWeapon>();//to use with weapons with multiple colliding parts (e.g: 2 fists or double blade,etc)
    public Collider weaponCollider { get; private set; }
    private float minDamage = 1f;
    private float maxDamage = 2f;
    //private List<IDamageable> objectsHit = new List<IDamageable>();

    private void Awake()
    {
        if (weaponPartsColliders.Count > 0)
        {
            for (int i = 0; i < weaponPartsColliders.Count; i++)
            {
                if (!weaponPartsColliders[i].isTrigger) weaponPartsColliders[i].isTrigger = true;

                PlayerWeapon playerWeaponComponent = weaponPartsColliders[i].GetComponent<PlayerWeapon>();
                if (playerWeaponComponent != null)
                {
                    if (!playerWeaponParts.Contains(playerWeaponComponent)) playerWeaponParts.Add(playerWeaponComponent);
                    continue;
                }

                playerWeaponComponent = weaponPartsColliders[i].gameObject.AddComponent<PlayerWeapon>();
                if (!playerWeaponParts.Contains(playerWeaponComponent)) playerWeaponParts.Add(playerWeaponComponent);

            }
        }

        weaponCollider = GetComponent<Collider>();

        if(weaponCollider != null)
        {
            weaponCollider.isTrigger = true;
        }
        else if(weaponPartsColliders.Count == 0 && weaponCollider == null)
        {
            Debug.LogWarning("Cant find weapon or weapon parts collider on weapon: " + name);
        }
    }

    public void GetWeaponDamageValues(float minDamage, float maxDamage)
    {
        this.minDamage = minDamage;
        this.maxDamage = maxDamage;

        if (playerWeaponParts.Count > 0)
        {
            for (int i = 0; i < playerWeaponParts.Count; i++)
            {
                playerWeaponParts[i].GetWeaponDamageValues(minDamage, maxDamage);
            }
        }
    }

    public void EnablePlayerWeapon(bool enabled)
    {
        if (weaponCollider != null) weaponCollider.enabled = enabled;
        if (playerWeaponParts.Count > 0)
        {
            for (int i = 0; i < playerWeaponParts.Count; i++)
            {
                playerWeaponParts[i].EnablePlayerWeapon(enabled);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) return;
        IDamageable damageable = other.GetComponent<IDamageable>();
        if(damageable != null)
        {
            if (damageable.GetColliderEnabledStatus()) damageable.GetDamaged(Random.Range(minDamage, maxDamage), IDamageable.DamageEffect.None);
            /*if (!objectsHit.Contains(damageable))
            {
                objectsHit.Add(damageable);
                if(damageable.GetColliderEnabledStatus()) damageable.GetDamaged(Random.Range(minDamage, maxDamage));
            }*/
        }
    }

    /*private void OnTriggerExit(Collider other)
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
    }*/
}
