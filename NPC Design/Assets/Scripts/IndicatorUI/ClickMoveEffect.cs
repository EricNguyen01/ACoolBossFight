using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickMoveEffect : MonoBehaviour
{
    private ParticleSystem particle;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        if(particle != null)
        {
            StartCoroutine(EffectLastDuration(particle.main.duration + 0.15f));
        }
    }

    private IEnumerator EffectLastDuration(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }
}
