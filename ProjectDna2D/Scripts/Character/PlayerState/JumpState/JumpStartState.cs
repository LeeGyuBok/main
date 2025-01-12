using UnityEngine;

public class JumpStartState : IRigidbodyState
{
    private IRigidbodyState _jumping;
    public Animator PlayerAnimator { get; private set; }

    public void EnterState(Player player)
    {
        player.OnTheGround(false);
        if (!PlayerAnimator)
        {
            PlayerAnimator = player.CharacterAnimator;
        }
        if (_jumping == null)
        {
            _jumping = new JumpingState();
        }
    }

    public void FixedExecute(Player player)
    {
        player.CharacterRb.AddForce(player.CharacterRb.transform.up * (player.JumpForce), ForceMode2D.Impulse);
        player.StateChangePublic(_jumping);
    }

    public void Execute(Player player)
    {
        if (player.AttachWall && Input.GetKeyDown(KeyCode.C))
        {
            player.StateChangePublic(player.WallClimbIdleState);
            return;
        }
    }

    public void ExitState(Player player)
    {

    }
}
