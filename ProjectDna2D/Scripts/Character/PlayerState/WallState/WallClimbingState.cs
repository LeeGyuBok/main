using UnityEngine;

public class WallClimbingState : IRigidbodyState
{
    public Animator PlayerAnimator { get; private set; }
    
    public void EnterState(Player player)
    {
        if (!PlayerAnimator)
        {
            PlayerAnimator = player.CharacterAnimator;
        }
        PlayerAnimator.SetTrigger(player.WallClimbing);
    }

    public void FixedExecute(Player player)
    {
        //player.CharacterRb.MovePosition(player.VerticalDirection * (Time.deltaTime + player.ClimbSpeed));
        player.CharacterRb.linearVelocity = player.VerticalDirection * (Time.deltaTime + player.ClimbSpeed);
    }

    public void Execute(Player player)
    {
        if (Input.GetButtonUp("Vertical"))
        {
            player.IsMoving = false;
            player.CanMoveVertical = true;
            player.StateChangePublic(player.WallClimbIdleState);    
            return;
        }

        RaycastHit2D hit2D = Physics2D.Raycast(player.CharacterRb.transform.position,
            player.WallOverCheckDirection, 1f,LayerMask.GetMask("Wall"));
        if (!hit2D)
        {
            player.StateChangePublic(player.WallHangState);
            Debug.Log("transit to climbover");
            return;
        }
        else
        {
            player.OverPosition = hit2D.point;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.StateChangePublic(player.WallJumpState);
            return;
        }
        
        /*Debug.DrawRay(player.CharacterRb.transform.position,
            wallDirection.normalized, Color.red);*/
     
    }

    public void ExitState(Player player)
    {
        PlayerAnimator.ResetTrigger(player.WallClimbing);
    }
}
