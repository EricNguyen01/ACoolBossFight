using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BossState
{
    protected Boss boss;
    protected float timeInState;

    public BossState(Boss boss)
    {
        this.boss = boss;
    }

    public virtual void OnStateEnter()
    {
        timeInState = 0f;
    }

    public virtual void OnStateUpdate()
    {
        timeInState += Time.deltaTime;
    }

    public virtual void OnStateTransition()
    {
        timeInState = 0f;
    }

    public virtual void OnAnimationEventEnded()
    {

    }
}
