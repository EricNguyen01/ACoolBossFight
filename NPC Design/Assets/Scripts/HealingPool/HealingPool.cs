using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealingPool : MonoBehaviour
{
    [SerializeField] private float healAmount;
    [SerializeField] private float poolReEnableTime;
    [SerializeField] private Image healingIconImage;

    private Collider healingPoolCollider;
    private AudioSource healingPoolAudio;
    private ParticleSystem[] healingParticles;

    private void Awake()
    {
        healingParticles = GetComponentsInChildren<ParticleSystem>();
        healingPoolCollider = GetComponent<Collider>();
        healingPoolAudio = GetComponent<AudioSource>();

        if (healingPoolCollider == null)
        {
            enabled = false;
            return;
        }

        healingPoolCollider.isTrigger = true;
    }

    private void OnTriggerStay(Collider other)
    {
        IHeal objToHeal = other.GetComponent<IHeal>();
        if (objToHeal == null) return;

        bool shouldHeal = objToHeal.ShouldReceiveHealing();
        if (shouldHeal)
        {
            objToHeal.OnHeal(healAmount);
            if (healingPoolAudio != null) healingPoolAudio.Play();
            StartCoroutine(PoolTemporaryDisable());
        }
    }

    private IEnumerator PoolTemporaryDisable()
    {
        healingPoolCollider.enabled = false;

        if(healingParticles.Length > 0)
        {
            for(int i = 0; i < healingParticles.Length; i++)
            {
                healingParticles[i].gameObject.SetActive(false);
            }
        }

        if (healingIconImage != null) healingIconImage.enabled = false;

        yield return new WaitForSeconds(poolReEnableTime);

        healingPoolCollider.enabled = true;
        if (healingParticles.Length > 0)
        {
            for (int i = 0; i < healingParticles.Length; i++)
            {
                healingParticles[i].gameObject.SetActive(true);
            }
        }

        if (healingIconImage != null) healingIconImage.enabled = true;
    }
}
