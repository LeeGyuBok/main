using UnityEngine;

public class LandingState : IRigidbodyState
{
    private float timer;
    private float TimerLimit;
    private float horizontal;
    private Vector2 direction;
    private Animator characterAnimator;
    
    
    public void EnterState(Player player)
    {
        timer = 0f;
        if (!characterAnimator)
        {
            characterAnimator = player.CharacterAnimator;    
        }
        characterAnimator.SetTrigger(player.Landing);
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
        //리미트타임도 애니메이션의 지속시간으로 설정가능
        timer += Time.deltaTime;
        if (timer <= TimerLimit)
        {
            if (Input.GetKeyDown(KeyCode.C) || FallingState.RollingCommand)
            {
                //롤링!
                player.CanMove = false;
                direction = player.Direction;
                player.CharacterRb.AddForce(20f * direction, ForceMode2D.Impulse);    
                player.StateChangePublic(player.RollingState);
                return;
            }
        }

        if (timer > TimerLimit)
        {
            characterAnimator.ResetTrigger(player.Landing);
            player.CanMove = true;
            player.StateChangePublic(player.IdleRigidbodyState);
        }
    }

    public void ExitState(Player player)
    {

    }
}
