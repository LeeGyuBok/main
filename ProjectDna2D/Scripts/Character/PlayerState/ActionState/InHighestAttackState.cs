using UnityEngine;

public class InHighestAttackState : IAttackState, IRigidbodyState
{
    public float Timer { get; private set; }
    public float TimerLimit { get; } = 0.3f;
    public Animator PlayerAnimator { get; private set; }

    public void EnterState(Player player)
    {
        if (!PlayerAnimator)
        {
            PlayerAnimator = player.CharacterAnimator;
        }
        Debug.Log("enter highestAttack");
        player.CharacterRb.gravityScale = 0;
        Timer = 0f;
    }

    public void FixedExecute(Player player)
    {
        
    }

    public void Execute(Player player)
    {
        Timer += Time.deltaTime;
        if (TimerLimit <= Timer)
        {
            player.CharacterRb.gravityScale = 1;
            player.StateChangePublic(player.FallingState);
        }
    }

    public void ExitState(Player player)
    {
        Timer = 0f;
        JumpingState.AirAttackCommand = false;
        player.CharacterRb.gravityScale = 1;
    }
}
