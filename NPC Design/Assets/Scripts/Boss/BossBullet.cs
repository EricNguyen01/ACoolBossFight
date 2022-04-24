using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBullet : MonoBehaviour
{
    private float minDamage;
    private float maxDamage;
    private float maxRange;
    private float bulletSpeed;
    private Rigidbody bulletRb;
    private Vector3 startPos;

    private void Awake()
    {
        bulletRb = GetComponent<Rigidbody>();
        if (bulletRb == null) bulletRb = gameObject.AddComponent<Rigidbody>();
        Collider collider = GetComponent<Collider>();
        if (collider == null) Debug.LogWarning("Bullet Collider is missing on bullet: " + name);
        startPos = transform.position;
        Destroy(gameObject, 10f);
    }

    private void FixedUpdate()
    {
        bulletRb.AddForce(transform.forward * bulletSpeed * Time.fixedDeltaTime, ForceMode.Impulse);
    }

    public void InitializeBullet(float min, float max, float range, float speed)
    {
        minDamage = min;
        maxDamage = max;
        maxRange = range;
        bulletSpeed = speed;
        StartCoroutine(DestroyOnMaxRangeReached());
    }

    private IEnumerator DestroyOnMaxRangeReached()
    {
        yield return new WaitUntil(() => Vector3.Distance(transform.position, startPos) > maxRange);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Boss")) return;
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            if (damageable.GetColliderEnabledStatus())
            {
                damageable.GetDamaged(Random.Range(minDamage, maxDamage), IDamageable.DamageEffect.Stagger);
                damageable.GetDamageDirection(transform.forward);
            }
        }

        Destroy(gameObject);
    }
}
