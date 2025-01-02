using UnityEngine;

public class FallingAttack : IAttackState, IRigidbodyState
{
    public float Timer { get; private set; }
    public float TimerLimit { get; }
    public void EnterState(Player player)
    {
        Debug.Log("FallingAttack Enter");
        player.CharacterRb.gravityScale = 60f;
    }

    public void FixedExecute(Player player)
    {
        
    }

    public void Execute(Player player)
    {
        if (player.CharacterRb.linearVelocity.magnitude < 0.05f || !player.IsAirBorne)
        {
            player.StateChangePublic(player.LandingState);
        }
    }

    public void ExitState(Player player)
    {
        Timer = 0f;
        JumpingState.AirAttackCommand = false;
        player.CharacterRb.gravityScale = 1;
    }
}
