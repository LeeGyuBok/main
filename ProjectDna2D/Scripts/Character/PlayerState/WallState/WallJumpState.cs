using UnityEngine;

public class WallJumpState : IRigidbodyState
{
    public Animator PlayerAnimator { get; private set; }
    private float timer;
    private float TimerLimit;
    private Vector2 direction;
    private bool inJump = false;
    public void EnterState(Player player)
    {
        player.OnTheWall(false);
        player.OnTheGround(false);
        player.CharacterRb.gravityScale = 2f;
        inJump = false;
        timer = 0f;
        if (!PlayerAnimator)
        {
            PlayerAnimator = player.CharacterAnimator;    
        }
        Vector2 xDirection = new Vector2(player.WallOverCheckDirection.x, 0f).normalized;
        direction = new Vector2(-xDirection.x, 2f).normalized;
        if (direction.x < 0)
        {
            player.gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            player.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        PlayerAnimator.SetTrigger(player.WallJump);
    }

    public void FixedExecute(Player player)
    {
        if (!inJump)
        {
            player.CharacterRb.AddForce(direction * player.JumpForce, ForceMode2D.Impulse);
            inJump = true;
        }
        
    }

    public void Execute(Player player)
    {
        Debug.DrawRay(player.CharacterRb.transform.position, direction * 2f, Color.red);
        if (PlayerAnimator.IsInTransition(0))
        {
            return;
        }
        if (TimerLimit == 0f)
        {
            TimerLimit = PlayerAnimator.GetCurrentAnimatorStateInfo(0).length;    
        }
        timer += Time.deltaTime;
        if (timer >= TimerLimit)
        {
            player.StateChangePublic(player.FallingState);   
            return;
        }
        
        
    }

    public void ExitState(Player player)
    {
        
    }
}
