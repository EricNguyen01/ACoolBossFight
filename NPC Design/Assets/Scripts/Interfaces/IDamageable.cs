using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public enum DamageEffect { None, Stagger, KnockUp, KnockBack }

    public void GetDamaged(float damage, IDamageable.DamageEffect damageEffect);
    public void GetDamageDirection(Vector3 dir);
    public bool GetColliderEnabledStatus();
}
