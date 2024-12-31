using UnityEngine;

public class IJumpCancel : IRigidbodyState
{
    public void EnterState(Player player)
    {
        player.CharacterRb.gravityScale = 20f;
    }

    public void FixedExecute(Player player)
    {
        
    }

    public void Execute(Player player)
    {
        if (player.CharacterRb.linearVelocity.magnitude < 0.05f || !player.IsAirBorne)
        {
            player.StateChangePublic(player.IdleRigidbodyState);
        }
    }

    public void ExitState(Player player)
    {
        player.CharacterRb.gravityScale = 1f;
    }
}
