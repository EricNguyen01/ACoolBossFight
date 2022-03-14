using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthBarUI : HealthBarUI
{
    [SerializeField] private Player player;

    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        if (player == null)
        {
            Debug.LogWarning("Player Reference in Player Health Bar UI: " + name + " Is not assigned! Disabling UI game object!");
            gameObject.SetActive(false);
            return;
        }

        player.playerHeathBarUI = this;
        GetBaseHealthValue(player.playerHP);

        base.Start();
    }
}
