using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerMove : PlayerState
{
    public PlayerMove(Player player) : base(player)
    {
        
    }

    public override void OnStateEnter()
    {
        player.playerNavMesh.isStopped = false;
        base.OnStateEnter();
    }

    public override void OnStateUpdate()
    {
        base.OnStateUpdate();

        //only process the "move to" aspect of player's movements in PlayerMove state
        //upon close to target destination (as checked by HasArrived() method in Player.cs), NodeCanvas bound FSM will trigger a transition
        //to Idle State which only handles the "arriving" and deceleration.

        if (player.currentRightMouseClickPos == Vector3.zero) return;//in case mouse click movement is alr stopped before (e.g transition from roll state)

        //player.RotatePlayerToPos(player.currentRightMouseClickPos);
        player.playerNavMesh.SetDestination(player.currentRightMouseClickPos);

        float moveAnimBlendSpd = 0f;

        player.currentMoveSpeed += (player.moveSpeed + 2f) * Time.deltaTime;
        if (player.currentMoveSpeed >= player.moveSpeed) player.currentMoveSpeed = player.moveSpeed;
        moveAnimBlendSpd = player.currentMoveSpeed / player.moveSpeed;

        //charController.Move(player.transform.forward * player.currentMoveSpeed * Time.deltaTime);
        player.playerAnimator.SetFloat("Speed", moveAnimBlendSpd);
    }

    public override void OnStateTransition()
    {
        base.OnStateTransition();
    }
}
