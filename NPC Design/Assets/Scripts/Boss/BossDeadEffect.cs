using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDeadEffect : MonoBehaviour
{
    [SerializeField] private GameObject deadEffectPrefab;
    private GameObject spawnedEffect;

    public void OnBossDeadAnimationStarted()
    {
        if (deadEffectPrefab == null) return;

        Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        GameObject obj = Instantiate(deadEffectPrefab, spawnPos, Quaternion.LookRotation(transform.up));
        spawnedEffect = obj;
    }

    public void OnBossDeadAnimationEnded()
    {
        if(spawnedEffect != null)
        {
            ParticleSystem[] particles = spawnedEffect.GetComponentsInChildren<ParticleSystem>();
            if (particles.Length > 0)
            {
                for (int i = 0; i < particles.Length; i++)
                {
                    var main = particles[i].main;
                    main.loop = false;
                }
            }
        }

        Boss boss = GetComponent<Boss>();
        if (boss != null) boss.DestroyBossOnDeadAnimationEnded();
    }
}
