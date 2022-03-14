using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerState
{
    protected Player player;
    protected CharacterController charController;
    protected Collider charCollider;
    protected Rigidbody charRigidbody;
    protected float timeInState;

    public PlayerState(Player player)
    {
        this.player = player;

        charController = player.GetComponent<CharacterController>();
        if (charController == null)
        {
            charController = player.gameObject.AddComponent<CharacterController>();
            if (charController != null) charController.detectCollisions = false;
        }

        charRigidbody = player.GetComponent<Rigidbody>();
        if(charRigidbody == null)
        {
            charRigidbody = player.gameObject.AddComponent<Rigidbody>();
        }

        charCollider = player.GetComponent<Collider>();
        if (charCollider == null) Debug.LogWarning("Character doesnt have a capsule collider!");
    }

    public virtual void OnStateEnter()
    {
        timeInState = 0f;
    }

    public virtual void OnStateUpdate()
    {
        timeInState += Time.deltaTime;
    }

    public virtual void OnStateFixedUpdate()
    {

    }

    public virtual void OnStateTransition()
    {
        
    }

    public virtual void OnUnityAnimationEventTriggered()
    {

    }

    public virtual void OnUnityAnimationEventEnded()
    {

    }
}
