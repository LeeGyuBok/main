using System;
using UnityEngine;

public class WallHangState : IRigidbodyState
{
    private float timerLimit;
    private float timer;
    public Animator PlayerAnimator { get; private set; }
    public void EnterState(Player player)
    {
        timer = 0f;
        if (!PlayerAnimator)
        {
            PlayerAnimator = player.CharacterAnimator;
        }
        PlayerAnimator.SetTrigger(player.WallHang);
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
        if (timerLimit == 0f)
        {
            timerLimit = PlayerAnimator.GetCurrentAnimatorStateInfo(0).length;    
        } 
        timer += Time.deltaTime;
        if (timerLimit >= timer)
        {
            player.StateChangePublic(player.WallClimbOverState);
            return;
        }
    }

    public void ExitState(Player player)
    {
        PlayerAnimator.ResetTrigger(player.WallHang);
        timer = 0f;
    }
}
