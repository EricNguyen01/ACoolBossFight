using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAbilityInventory : MonoBehaviour
{
    public Boss bossHoldingThisAbilityInventory { get; private set; }

    public List<BossAbility> bossAbilities { get; private set; } = new List<BossAbility>();
    public Queue<BossAbility> availableAbilityQueue { get; private set; } = new Queue<BossAbility>();

    private void Start()
    {
        FillBossAbilityList();
    }

    private void FillBossAbilityList()
    {
        foreach(BossAbility bossAbility in GetComponentsInChildren<BossAbility>())
        {
            if (!bossAbility.gameObject.activeInHierarchy || bossAbility.isAbilityDisabled) continue;
            if (!bossAbilities.Contains(bossAbility)) bossAbilities.Add(bossAbility);
        }

        if (bossAbilities.Count == 0) Debug.LogWarning("Boss Ability Inventory has found no ability children object!");
    }

    public void GetBossHoldingThisAbilityInventory(Boss boss)
    {
        bossHoldingThisAbilityInventory = boss;
    }

    public void EnqueueAbility(BossAbility ability)
    {
        if (!availableAbilityQueue.Contains(ability)) availableAbilityQueue.Enqueue(ability);
    }

    public void UseFirstAvailableAbilityInQueue()
    {
        availableAbilityQueue.Dequeue().UseAbility();
    }

    public void SetAllAbilityOnCooldown()
    {
        if (bossAbilities == null || bossAbilities.Count == 0) return;
        availableAbilityQueue.Clear();
        for(int i = 0; i < bossAbilities.Count; i++)
        {
            bossAbilities[i].StartCoroutine(bossAbilities[i].CooldownTimer());
        }
    }
}
