using UnityEngine;

public class IdleRigidbodyState : IRigidbodyState
{
    public Animator PlayerAnimator { get; private set; }

    public void EnterState(Player player)
    {
        player.IsMoving = false;
        player.CanAct();
        if (!PlayerAnimator)
        {
            PlayerAnimator = player.CharacterAnimator;    
        }
        PlayerAnimator.SetTrigger(player.Idle);
    }

    public void FixedExecute(Player player)
    {
        //player.CharacterRb.linearVelocity = Vector2.zero;
    }

    public void Execute(Player player)
    {
        //Debug.Log("idle");
        if (PlayerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle 1"))
        {
            if (player.Horizontal != 0)//ad or <- ->
            {
                player.StateChangePublic(player.MoveRigidbodyState);
                return;
            }
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                player.StateChangePublic(player.JumpStartState);
                return;
            }

            if (player.AttachWall && Input.GetKeyDown(KeyCode.C))
            {
                player.StateChangePublic(player.WallClimbIdleState);
                return;
            }
            if (player.IsAttacking)//X
            {
                player.StateChangePublic(player.StartAttackState);
                return;
            }

            
            /*if (player.IsAirBorne)//space
            {
                player.StateChangePublic(player.JumpStartState);
                return;
            } 
        
            if (player.OnWall)//C
            {
                player.StateChangePublic(player.WallClimbIdleState);
                return;
            }*/
        }
    }

    public void ExitState(Player player)
    {
        player.CharacterAnimator.ResetTrigger(player.Idle);
    }
}