using System.Collections;
using UnityEngine;

public class MoveRigidbodyState : IRigidbodyState
{
    WaitForSeconds _delay = new WaitForSeconds(0.2f);
    public Animator PlayerAnimator { get; private set; }

    public void EnterState(Player player)
    {
        if (!PlayerAnimator)
        {
            PlayerAnimator = player.CharacterAnimator;
        }
        PlayerAnimator.SetTrigger(player.Move);
    }

    public void FixedExecute(Player player)
    {
        player.CharacterRb.linearVelocity = player.HorizontalDirection * (Time.deltaTime * player.MovementSpeed);
    }

    public void Execute(Player player)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.StateChangePublic(player.JumpStartState);
            return;
        }
        if (Input.GetButtonUp("Horizontal"))
        {
            player.StateChangePublic(player.IdleRigidbodyState);    
            return;
        }
        if (player.IsAttacking)
        {
            player.CharacterRb.linearVelocity = Vector3.zero;
            player.IsMoving = false;
            player.CanMoveHorizontal = false;
            player.StateChangePublic(player.StartAttackState);
            //player.StateChangePublic(player.IdleRigidbodyState);
            return;
        }
        if (player.OnWall)
        {
            player.StateChangePublic(player.WallClimbIdleState);
            return;
        }

        if (player.IsAirBorne)
        {
            player.StateChangePublic(player.FallingState);
        }
    }

    public void ExitState(Player player)
    {
        player.CharacterRb.linearVelocity = Vector2.zero;
        PlayerAnimator.ResetTrigger(player.Move);
    }
}
