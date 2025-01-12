using UnityEngine;

public class WallClimbOverState : IRigidbodyState
{
    public Animator PlayerAnimator { get; private set; }
    private float timerLimit;
    private float timer;

    public void EnterState(Player player)
    {
        timer = 0f;
        if (!PlayerAnimator)
        {
            PlayerAnimator = player.CharacterAnimator;
        }
        PlayerAnimator.SetTrigger(player.WallClimbOver);
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
        if (timer >= timerLimit)
        {
            player.CharacterRb.transform.position = player.OverPosition + Vector2.up;
            player.StateChangePublic(player.IdleRigidbodyState);
            return;
        }
    }

    public void ExitState(Player player)
    {
        player.OnWall = false;
        player.AttachWall = false;
        player.CanMoveVertical = false;
    }
}
