using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTriggerDome : MonoBehaviour
{
    [SerializeField] private GameObject triggerDomeEffect;
    private Boss boss;
    private BossThemePlayer bossThemePlayer;
    public CapsuleCollider triggerDomeCollider { get; private set; }

    private void Awake()
    {
        boss = GetComponentInParent<Boss>();
        if(boss == null)
        {
            Debug.LogWarning("Boss component cannot be found in parent object of: " + name + ". Disabling dome!");
            gameObject.SetActive(false);
            return;
        }

        triggerDomeCollider = GetComponent<CapsuleCollider>();
        if(triggerDomeCollider == null)
        {
            Debug.LogWarning("Trigger Dome Capsule Collider component cannot be found on : " + name + ". Disabling dome!");
            gameObject.SetActive(false);
            return;
        }

        bossThemePlayer = boss.GetComponentInChildren<BossThemePlayer>();
        if(bossThemePlayer == null)
        {
            Debug.LogWarning("Boss Theme Player component as this boss child not found!");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            boss.bossHealthBarUI.EnableHealthBar(true);
            Player player = other.GetComponent<Player>();
            boss.playerToTarget = player;
            if (boss.bossAbilityInventory != null) boss.bossAbilityInventory.SetAllAbilityOnCooldown();
            boss.StartCheckingForAvailableAbilityToUse();
            TriggerDomeEffectConfigOnTriggered();
            if (bossThemePlayer != null) bossThemePlayer.PlayBossThemeInstant(true);
            //add any additional pre-boss fight stuff before disabling trigger dome
            gameObject.SetActive(false);
        }
    }

    private void TriggerDomeEffectConfigOnTriggered()
    {
        if(triggerDomeEffect != null)
        {
            triggerDomeEffect.transform.SetParent(null);
            ParticleSystem[] particles = triggerDomeEffect.GetComponentsInChildren<ParticleSystem>();
            if(particles.Length > 0)
            {
                for(int i = 0; i < particles.Length; i++)
                {
                    var main = particles[i].main;
                    main.loop = false;
                }
            }
        }
    }
}
