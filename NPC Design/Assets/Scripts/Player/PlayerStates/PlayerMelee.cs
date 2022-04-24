using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerMelee : PlayerState
{
    public PlayerMelee(Player player) : base(player)
    {

    }

    public override void OnStateEnter()
    {
        player.playerNavMesh.isStopped = true;
        player.playerNavMesh.ResetPath();
        player.currentRightMouseClickPos = Vector3.zero;
        player.currentMoveSpeed = 0f;
        player.playerAnimator.SetFloat("MeleeNum", player.currentAttackNum);
        player.playerAnimator.SetBool("Melee", true);
        if (player.playerMeleeWeapon != null)
        {
            player.playerMeleeWeapon.EnablePlayerWeapon(true);
        }

        base.OnStateEnter();
    }

    public override void OnStateTransition()
    {
        player.isMeleeing = false;
        player.playerAnimator.SetBool("Melee", false);
        player.playerNavMesh.isStopped = false;
        player.currentAttackSpeed = 0f;
        player.currentComboResetTime = player.playerAttackComboResetTime;
        if (player.currentAttackNum == player.playerAttackComboNum) player.currentAttackNum = 0;
        else player.currentAttackNum += 1;
        if (player.playerMeleeWeapon != null)
        {
            player.playerMeleeWeapon.EnablePlayerWeapon(false);
        }

        base.OnStateTransition();
    }

    public override void OnStateUpdate()
    {
        if(timeInState <= 0.175f) player.RotatePlayerToPos(player.currentLeftMouseClickPos);
        base.OnStateUpdate();
    }

    public override void OnUnityAnimationEventEnded()
    {
        player.isMeleeing = false;
        base.OnUnityAnimationEventEnded();
    }
}
