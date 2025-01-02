using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RollingState : IRigidbodyState
{
    private Animator characterAnimator;
    private float Timer;
    private float TimerLimit;
    
    
    public void EnterState(Player player)
    {
        player.CanMove = false;
        Timer = 0f;
        if (!characterAnimator)
        {
            characterAnimator = player.CharacterAnimator;    
        }
        characterAnimator.SetTrigger(player.Rolling);
        Debug.Log(TimerLimit);
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
        //Debug.Log(timer);
        //애니메이션 스테이트인포를 조건으로 가능.
        if (Timer >= TimerLimit)
        {
            
            player.CanMove = true;
            player.StateChangePublic(player.IdleRigidbodyState);
        }
    }

    public void ExitState(Player player)
    {
        Timer = 0f;
        characterAnimator.ResetTrigger(player.Rolling);
        //Debug.Log("need animationStateInfo to control rolling");
    }
}
