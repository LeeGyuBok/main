using UnityEngine;

public class LandingState : IRigidbodyState
{
    private float timer;
    private float TimerLimit;
    private Vector2 direction;
    private float horizontal;
    

    public Animator PlayerAnimator { get; private set; }

    public void EnterState(Player player)
    {
        timer = 0f;
        if (!PlayerAnimator)
        {
            PlayerAnimator = player.CharacterAnimator;    
        }
        PlayerAnimator.SetTrigger(player.Landing);
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
        //리미트타임도 애니메이션의 지속시간으로 설정가능
        timer += Time.deltaTime;
        if (timer <= TimerLimit)
        {
            if (Input.GetKeyDown(KeyCode.C) || FallingState.RollingCommand)
            {
                //롤링!
                direction = player.HorizontalDirection;
                if (direction == Vector2.zero)
                {
                    return;
                }
                player.CharacterRb.AddForce(15f * direction, ForceMode2D.Impulse);    
                player.StateChangePublic(player.RollingState);
                return;
            }
        }

        if (timer > TimerLimit)
        {
            PlayerAnimator.ResetTrigger(player.Landing);
            player.CanMoveHorizontal = true;
            player.StateChangePublic(player.IdleRigidbodyState);
        }
    }

    public void ExitState(Player player)
    {

    }
}
