using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BossHealthBarUI : HealthBarUI
{
    [SerializeField] private Boss boss;
    [SerializeField] private TextMeshProUGUI healthBarBossNameText;

    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        if (boss == null)
        { 
            Debug.LogWarning("Boss Reference in Boss Health Bar UI: " + name + " Is not assigned! Disabling UI game object!");
            gameObject.SetActive(false);
            return;
        }

        if (healthBarBossNameText == null)
        {
            Debug.LogWarning("Health Bar Name Text in Boss Health Bar UI: " + name + " Is Not Assigned! Disabling UI game object!");
            gameObject.SetActive(false);
            return;
        }

        boss.bossHealthBarUI = this;
        GetBaseHealthValue(boss.bossHP);
        healthBarBossNameText.text = boss.bossName;

        base.Start();
    }
}
