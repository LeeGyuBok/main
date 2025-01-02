using UnityEngine;

public class EndAttackState : IAttackState, IRigidbodyState
{
    public float Timer { get; private set; } = 0;
    public float TimerLimit { get; private set; }

    private Animator characterAnimator;

    public void EnterState(Player player)
    {
        Timer = 0f;
        if (!characterAnimator)
        {
            characterAnimator = player.CharacterAnimator;    
        }
        characterAnimator.SetTrigger(player.EndAttack);
    }

    public void FixedExecute(Player player)
    {
            
    }

    public void Execute(Player player)
    {
        if (characterAnimator.IsInTransition(0))
        {
            return;
        }
        if (TimerLimit == 0f)
        {
            TimerLimit = characterAnimator.GetCurrentAnimatorStateInfo(0).length;    
        }
        
        Timer += Time.deltaTime;
        if (TimerLimit <= Timer)
        {
            player.StateChangePublic(player.IdleRigidbodyState);    
        }
    }

    public void ExitState(Player player)
    {
        Timer = 0f;
        player.IsAttacking = false;
    }
}

    

