using UnityEngine;

public class JumpInHighestState: IRigidbodyState
{
    public Animator PlayerAnimator { get; private set; }

    public void EnterState(Player player)
    {
        //Debug.Log("JumpInHighestState EnterState");
        if (!PlayerAnimator)
        {
            PlayerAnimator = player.CharacterAnimator;
        }
        PlayerAnimator.SetTrigger(player.Highest);
    }

    public void FixedExecute(Player player)
    {
        
    }

    public void Execute(Player player)
    {
        if (JumpingState.FallingAttackCommand)
        {
            player.StateChangePublic(player.GroundSlamFlyingState);
            return;
        }

        if (player.Vertical <= 0f)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                player.StateChangePublic(player.GroundSlamFlyingState);
                return;
            }
        }
        
        if (player.AttachWall && Input.GetKeyDown(KeyCode.C))
        {
            player.StateChangePublic(player.WallClimbIdleState);
            return;
        }
        
        /*if (Input.GetKeyDown(KeyCode.X) || JumpingState.AirAttackCommand)
        {
            player.StateChangePublic(player.InHighestAttackState);
            return;
        }
        */
        
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.StateChangePublic(player.JumpCancelState);
            return;
        }
        
        if (player.CharacterRb.linearVelocity.magnitude >= 0.2f)
        {
            player.StateChangePublic(player.FallingState);
            return;
        }
        
        if (!player.IsAirBorne)
        {
            player.StateChangePublic(player.IdleRigidbodyState);
            return;
        }
    }

    public void ExitState(Player player)
    {
        
    }
}
