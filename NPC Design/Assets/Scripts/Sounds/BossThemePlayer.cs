using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossThemePlayer : MonoBehaviour
{
    private AudioSource audioSource;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if(audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlayBossThemeInstant(bool play)
    {
        if (play)
        {
            audioSource.Play();
            return;
        }

        audioSource.Stop();
    }

    public void StopThemeOnBossDead()
    {
        audioSource.loop = false;

        if(audioSource.clip != null)
        {
            audioSource.time = audioSource.clip.length * 0.95f;
        }
    }
}
