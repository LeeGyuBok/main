using UnityEngine;

public class IdleRigidbodyState : IRigidbodyState
{
    public void EnterState(Player player)
    {
        
    }

    public void FixedExecute(Player player)
    {
        player.CharacterRb.linearVelocity = Vector2.zero;
    }

    public void Execute(Player player)
    {
        //Debug.Log("idle");
       

        if (player.IsAirBorne)
        {
            player.StateChangePublic(player.JumpStart);
            return;
        } 
        
        if (player.IsMove)
        {
            player.StateChangePublic(player.MoveRigidbodyState);
        }
    }

    public void ExitState(Player player)
    {
        
    }
}