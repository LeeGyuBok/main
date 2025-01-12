using UnityEngine;

public class GroundSlamSlammingState : IAttackState, IRigidbodyState
{
    public float Timer { get; private set; }
    public float TimerLimit { get; private set; }
    public Animator PlayerAnimator { get; private set; }

    public void EnterState(Player player)
    {
        Timer = 0f;
        if (!PlayerAnimator)
        {
            PlayerAnimator = player.CharacterAnimator;    
        }
        PlayerAnimator.SetTrigger(player.GroundSlamSlamming);
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
        
    }
}
