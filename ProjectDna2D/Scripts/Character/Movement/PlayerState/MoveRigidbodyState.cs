using UnityEngine;

public class MoveRigidbodyState : IRigidbodyState
{
    public void EnterState(Player player)
    {
        
    }

    public void FixedExecute(Player player)
    {
        player.CharacterRb.linearVelocity = player.Direction * (Time.deltaTime * player.MovementSpeed);
    }

    public void Execute(Player player)
    {
        if (!player.IsMove)
        {
            player.CharacterRb.linearVelocity = Vector3.zero;
            player.StateChangePublic(player.IdleRigidbodyState);
            return;
        }

        if (player.IsAirBorne)
        {
            player.StateChangePublic(player.JumpStart);
        }
    }

    public void ExitState(Player player)
    {

    }
}
