using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class PlayerRoll : PlayerState
{
    private bool enableRolling = false;
    private Vector3 rollLookAtPos;
    private Vector3 preRollPos;
    private float baseStoppingDist;
    private float baseSpd;
    private float baseAccel;
    private NavMeshPath path;

    public PlayerRoll(Player player) : base(player)
    {
        path = new NavMeshPath();
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();

        //If roll number = 0 which means that the player can't roll (roll is completely disabled)
        if (player.playerRollNumber <= 0) 
        { 
            enableRolling = false;
            player.isRolling = false;
            return;
        }

        //if roll number > 0
        if (player.playerRollNumber > 0)
        {
            //if the player has rolled an amount equal to the allowed roll number -> can't roll anymore until roll wait time is over and roll num reset
            //OR if the wait time between each roll (if more than 1 roll allowed) is not over -> cant roll either
            if (player.currentRollNumber == player.playerRollNumber || player.currentPlayerRollWaitTime > 0f) 
            { 
                enableRolling = false;
                player.isRolling = false;
                return; 
            }

            //else -> can roll
            enableRolling = true;
            //stop moving to mouse click pos before rolling
            player.currentRightMouseClickPos = Vector3.zero;
            //reset move speed
            player.currentMoveSpeed = 0f;

            player.playerNavMesh.isStopped = false;
            player.playerNavMesh.ResetPath();
            player.playerNavMesh.velocity = Vector3.zero;

            baseAccel = player.playerNavMesh.acceleration;
            player.playerNavMesh.acceleration = 14f;
            baseSpd = player.playerNavMesh.speed;
            player.playerNavMesh.speed = 10f;
            baseStoppingDist = player.playerNavMesh.stoppingDistance;
            player.playerNavMesh.stoppingDistance = 0f;
            player.playerNavMesh.autoBraking = false;
            player.playerNavMesh.autoRepath = false;
            
            CalculateRollLookAtPos();
            preRollPos = player.transform.position;
        }
    }

    public override void OnStateTransition()
    {
        player.playerNavMesh.ResetPath();
        player.playerNavMesh.acceleration = baseAccel;
        player.playerNavMesh.speed = baseSpd;
        player.playerNavMesh.stoppingDistance = baseStoppingDist;
        player.playerNavMesh.autoBraking = true;
        player.playerNavMesh.autoRepath = true;
        if(!charCollider.enabled) charCollider.enabled = true;

        base.OnStateTransition();
    }

    public override void OnUnityAnimationEventTriggered()
    {
        if (charCollider != null) charCollider.enabled = false;
        base.OnUnityAnimationEventTriggered();
    }

    public override void OnUnityAnimationEventEnded()
    {
        if (charCollider != null) charCollider.enabled = true;
        base.OnUnityAnimationEventEnded();
    }

    public override void OnStateUpdate()
    {
        base.OnStateUpdate();

        if (!enableRolling) return;

        //if(timeInState <= 0.3f) player.RotatePlayerToPos(rollLookAtPos, 4f);

        float rollDist = player.playerRollDistance * (timeInState / player.playerRollTime);

        //if is using transform rotation and movement (rotate using Quaternion and move using charController.move()):
        //roll to pos is calculated by: start roll pos + the end point on the forward dir (end of the current magnitude of forward) based on time
        //player.transform.position = preRollPos + player.transform.forward * rollDist;

        //else if is using nav mesh agent with rotation handled by the nav mesh agent itself:
        //the positions on the roll direction will slowly be incremented until the end pointof the roll direction is reached
        //the current position on the roll direction if on nav mesh will be set as new nav mesh agent destination
        //else not set as destination and the nav mesh agent will stop at the last moveable position during roll.
        Vector3 rollToPos = preRollPos + (rollLookAtPos - preRollPos).normalized * rollDist;
        player.playerNavMesh.CalculatePath(rollToPos, path);
        if (path.status == NavMeshPathStatus.PathComplete) player.playerNavMesh.SetDestination(rollToPos);


        //if roll anim is done and roll time is reached -> process finished rolling 
        if (timeInState >= player.playerRollTime)
        {
            enableRolling = false;
            player.isRolling = false;
            player.currentRollNumber += 1;
            /*if (player.currentRollNumber == player.playerRollNumber)*/ player.currentRollCooldown = player.playerRollCooldown;
            player.currentPlayerRollWaitTime = player.playerRollWaitTime;
        }
    }

    private void CalculateRollLookAtPos()
    {
        Ray ray = player.mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        //if a valid roll pos was clicked -> executes roll
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Walkable")))
        {
            player.playerAnimator.SetTrigger("Roll");
            rollLookAtPos = new Vector3(hit.point.x, player.transform.position.y, hit.point.z);
        }
        //else -> cancel roll
        else
        {
            enableRolling = false;
            player.isRolling = false;
        }
    }
}
