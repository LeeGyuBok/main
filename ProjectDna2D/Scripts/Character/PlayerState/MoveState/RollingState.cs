using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RollingState : IRigidbodyState
{
    private float Timer;
    private float TimerLimit;
    public Animator PlayerAnimator { get; private set; }

    public void EnterState(Player player)
    {
        player.CanMoveHorizontal = false;
        Timer = 0f;
        if (!PlayerAnimator)
        {
            PlayerAnimator = player.CharacterAnimator;    
        }
        PlayerAnimator.SetTrigger(player.Rolling);
        //Debug.Log(TimerLimit);
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
        //Debug.Log(timer);
        //애니메이션 스테이트인포를 조건으로 가능.
        if (Timer >= TimerLimit)
        {
            
            player.CanMoveHorizontal = true;
            player.StateChangePublic(player.IdleRigidbodyState);
        }
    }

    public void ExitState(Player player)
    {
        Timer = 0f;
        PlayerAnimator.ResetTrigger(player.Rolling);
        //Debug.Log("need animationStateInfo to control rolling");
    }
}
