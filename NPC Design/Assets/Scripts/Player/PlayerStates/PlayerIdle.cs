using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerIdle : PlayerState
{
    public PlayerIdle(Player player) : base(player)
    {

    }

    public override void OnStateEnter()
    {
        player.playerNavMesh.isStopped = true;
        player.playerNavMesh.ResetPath();
        base.OnStateEnter();
    }

    public override void OnStateUpdate()
    {
        base.OnStateUpdate();

        //if is using transform movement (charController) and not nav mesh movement:
        //if not fully idle (transitioned from move state) -> continue decel until fully idle
        //if during decelerating to idle or fully idle and the player sets a new destination (right click) -> HasArrived() check in Player.cs
        //will set to false which NodeCanvas bound FSM will then trigger PlayerMove state where "move to" and acceleration will be handled.

        if (player.currentMoveSpeed <= 0f)
        {
            player.currentMoveSpeed = 0f;
        }

        float moveAnimBlendSpd = 0f;

        player.currentMoveSpeed -= (2.5f + player.moveSpeed) * Time.deltaTime;
        if (player.currentMoveSpeed <= 0f) player.currentMoveSpeed = 0f;
        moveAnimBlendSpd = player.currentMoveSpeed / player.moveSpeed;


        //charController.Move(player.transform.forward * player.currentMoveSpeed * Time.deltaTime);
        player.playerAnimator.SetFloat("Speed", moveAnimBlendSpd);
    }

    public override void OnStateTransition()
    {
        player.playerNavMesh.isStopped = false;
        base.OnStateTransition();
    }

    public override void OnUnityAnimationEventTriggered()
    {
        player.isMeleeing = false;
        base.OnUnityAnimationEventTriggered();
    }
}
