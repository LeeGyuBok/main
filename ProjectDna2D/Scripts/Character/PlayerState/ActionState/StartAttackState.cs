using UnityEngine;

public class StartAttackState : IAttackState, IRigidbodyState
{
    public float Timer { get; private set; } = 0;
    public float TimerLimit { get; private set; }

    public Animator PlayerAnimator { get; private set; }

    public void EnterState(Player player)
    {
        Timer = 0f;
        if (!PlayerAnimator)
        {
            PlayerAnimator = player.CharacterAnimator;    
        }
        PlayerAnimator.SetTrigger(player.StartAttack);
        //Debug.Log(characterAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle 1")); this is true
        //Debug.Log("ComboStartAttack start");
    }

    public void FixedExecute(Player player)
    {
        
    }

    public void Execute(Player player)
    {
        if (PlayerAnimator.IsInTransition(0))
        {
            return;
        }
        if (TimerLimit == 0f)
        {
            TimerLimit = PlayerAnimator.GetCurrentAnimatorStateInfo(0).length;    
        }
        Timer += Time.deltaTime;
        if (TimerLimit >= Timer)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                player.StateChangePublic(player.StartMiddleAttackState);    
            }
        }
        else
        {
            player.IsAttacking = false;
            player.StateChangePublic(player.IdleRigidbodyState);
        }

        
    }

    public void ExitState(Player player)
    {
        Timer = 0f;
    }
}
