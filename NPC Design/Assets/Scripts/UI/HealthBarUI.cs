using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private bool disableOnStart = false;
    [SerializeField] private CanvasGroup healthDisplayCanvasGroup;
    [SerializeField] protected Slider healthSliderUI;
    [SerializeField] protected TextMeshProUGUI healthTextUI;
    protected float baseHealth { get; private set; } = 0f;
    protected float currentHealth { get; set; } = 0f;

    public virtual void Awake()
    {
        if(healthDisplayCanvasGroup == null)
        {
            healthDisplayCanvasGroup.GetComponent<CanvasGroup>();
            if(healthDisplayCanvasGroup == null)
            {
                Debug.LogWarning("Health Display Canvas Group component on: " + name + " Can't be found! Disabling UI object!");
                gameObject.SetActive(false);
                return;
            }
        }
        if(healthSliderUI == null || healthTextUI == null)
        {
            Debug.LogWarning("Health Slider or Health Text UI has not been assigned on Health Bar UI: " + name + ". Disabling UI game object!");
            gameObject.SetActive(false);
            return;
        }
    }

    public virtual void Start()
    {
        SetAndDisplayHealthBarUI(baseHealth);

        //disable on start (if enabled) has to be the last statement in Start() method
        if (disableOnStart) EnableHealthBar(false);
    }

    protected void GetBaseHealthValue(float health)
    {
        baseHealth = health;
        currentHealth = health;
    }

    public virtual void SetAndDisplayHealthBarUI(float health)
    {
        currentHealth = health;
        float normalizedHP = currentHealth / baseHealth;
        healthSliderUI.value = normalizedHP;
        healthTextUI.text = currentHealth.ToString() + " / " + baseHealth.ToString();
    }

    public virtual void EnableHealthBar(bool enabled)
    {
        if (enabled)
        {
            healthDisplayCanvasGroup.alpha = 1f;
            return;
        }

        healthDisplayCanvasGroup.alpha = 0f;
    }

}
