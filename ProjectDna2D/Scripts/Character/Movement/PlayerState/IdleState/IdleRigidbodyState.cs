using UnityEngine;

public class IdleRigidbodyState : IRigidbodyState
{
    private Animator characterAnimator;
    public void EnterState(Player player)
    {
        player.CanMove = true;
        if (!characterAnimator)
        {
            characterAnimator = player.CharacterAnimator;    
        }
        characterAnimator.SetTrigger(player.Idle);
    }

    public void FixedExecute(Player player)
    {
        player.CharacterRb.linearVelocity = Vector2.zero;
    }

    public void Execute(Player player)
    {
        //Debug.Log("idle");
        if (characterAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle 1"))
        {
            if (player.IsAirBorne)
            {
                player.StateChangePublic(player.JumpStartState);
                return;
            } 
        
            if (player.Horizontal != 0)
            {
                player.StateChangePublic(player.MoveRigidbodyState);
                return;
            }

            if (player.IsAttacking)
            {
                player.StateChangePublic(player.StartAttackState);
                return;
            }
        }
    }

    public void ExitState(Player player)
    {
        player.CharacterAnimator.ResetTrigger(player.Idle);
    }
}