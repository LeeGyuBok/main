using UnityEngine;

public class WallClimbIdleState : IRigidbodyState
{
    public Animator PlayerAnimator { get; private set; }

    public void EnterState(Player player)
    {
        player.OnTheWall(true);
        player.CharacterRb.linearVelocity = Vector2.zero;
        player.CharacterRb.gravityScale = 0;
        //player.IsGrounded = false;
        if (!PlayerAnimator)
        {
            PlayerAnimator = player.CharacterAnimator;
        }
        PlayerAnimator.SetTrigger(player.WallClimbIdle);
    }

    public void FixedExecute(Player player)
    {
        
    }

    public void Execute(Player player)
    {
        if (player.Vertical != 0)
        {
            player.IsMoving = true;
            player.StateChangePublic(player.WallClimbingState);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.StateChangePublic(player.WallJumpState);
            return;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (player.IsGrounded())
            {
                player.OnTheWall(false);
                player.OnTheGround(true);
                player.StateChangePublic(player.IdleRigidbodyState);
                return;
            }
        }

    }

    public void ExitState(Player player)
    {
        PlayerAnimator.ResetTrigger(player.WallClimbIdle);
    }
}
