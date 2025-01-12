using UnityEngine;

public class GroundSlamFlyingState : IAttackState, IRigidbodyState
{
    public float Timer { get; private set; }
    public float TimerLimit { get; }
    public Animator PlayerAnimator { get; private set; }

    public void EnterState(Player player)
    {
        if (!PlayerAnimator)
        {
            PlayerAnimator = player.CharacterAnimator;
        }
        PlayerAnimator.SetTrigger(player.GroundSlamFlying);
        //Debug.Log("GroundSlamFlyingState Enter");
        player.CharacterRb.gravityScale = 60f;
    }

    public void FixedExecute(Player player)
    {
        
    }

    public void Execute(Player player)
    {
        if (!player.IsAirBorne)
        {
            player.StateChangePublic(player.GroundSlamSlammingState);
        }
    }

    public void ExitState(Player player)
    {
        Timer = 0f;
        JumpingState.AirAttackCommand = false;
        player.CharacterRb.gravityScale = 1;
    }
}
