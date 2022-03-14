using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHeal
{
    public bool ShouldReceiveHealing();
    public void OnHeal(float healingAmount);
}
