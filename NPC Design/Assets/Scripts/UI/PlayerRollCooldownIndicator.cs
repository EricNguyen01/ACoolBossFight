using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRollCooldownIndicator : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private CanvasGroup indicatorCanvasGroup;
    [SerializeField] private Slider indicatorSlider;
    private bool resetSlider = false;

    private void Awake()
    {
        if (player != null) player.rollCooldownIndicator = this;
        if (indicatorCanvasGroup == null) indicatorCanvasGroup.GetComponent<CanvasGroup>();
        if (indicatorSlider == null) indicatorSlider.GetComponent<Slider>();

        if (indicatorCanvasGroup != null) indicatorCanvasGroup.alpha = 0f;
        if (indicatorSlider != null) indicatorSlider.value = 1f;
    }

    public void EnableAndDisplayRollCooldownUI(float start, float end, float current)
    {
        if (!resetSlider)
        {
            resetSlider = true;
            indicatorSlider.value = 1f;
            indicatorCanvasGroup.alpha = 1f;
        }

        if(start < end)
        {
            if (current >= end)
            {
                indicatorCanvasGroup.alpha = 0f;
                indicatorSlider.value = 1f;
                resetSlider = false;
                return;
            }

            float v = 1f - (start / end);
            indicatorSlider.value = v;
        }
        else
        {
            if (current <= end)
            {
                indicatorCanvasGroup.alpha = 0f;
                indicatorSlider.value = 1f;
                resetSlider = false;
                return;
            }

            float v = current / start;
            indicatorSlider.value = v;
        }
    }
}
