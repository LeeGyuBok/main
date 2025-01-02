using UnityEngine;

public class JumpInHighestState: IRigidbodyState
{
    public void EnterState(Player player)
    {
        //Debug.Log("JumpInHighestState EnterState");
        player.CharacterAnimator.SetTrigger(player.Highest);
    }

    public void FixedExecute(Player player)
    {
        
    }

    public void Execute(Player player)
    {
        if (JumpingState.FallingAttackCommand)
        {
            player.StateChangePublic(player.FallingAttack);
            return;
        }

        if (Input.GetButton("Vertical"))
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                player.StateChangePublic(player.FallingAttack);
                return;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.X) || JumpingState.AirAttackCommand)
        {
            player.StateChangePublic(player.InHighestAttackState);
            return;
        }
        
        
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
