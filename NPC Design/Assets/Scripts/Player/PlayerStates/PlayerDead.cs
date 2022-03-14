using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDead : PlayerState
{
    public static event System.Action OnPlayerDead;

    public PlayerDead(Player player) : base(player)
    {
    }

    public override void OnStateEnter()
    {
        OnPlayerDead?.Invoke();

        player.currentMoveSpeed = 0f;

        player.playerNavMesh.isStopped = true;
        player.playerNavMesh.velocity = Vector3.zero;
        player.playerNavMesh.ResetPath();

        player.playerAnimator.SetTrigger("Dead");
        base.OnStateEnter();
    }
}
