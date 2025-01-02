using System.Collections;
using UnityEngine;

public class MoveRigidbodyState : IRigidbodyState
{
    WaitForSeconds _delay = new WaitForSeconds(0.2f);
    public void EnterState(Player player)
    {
        player.CharacterAnimator.SetTrigger(player.Move);
    }

    public void FixedExecute(Player player)
    {
        player.CharacterRb.linearVelocity = player.Direction * (Time.deltaTime * player.MovementSpeed);
    }

    public void Execute(Player player)
    {
        if (player.IsAirBorne)
        {
            player.IsMoving = false;
            player.CanMove = false;
            player.StateChangePublic(player.JumpStartState);
            return;
        }
        if (Input.GetButtonUp("Horizontal"))
        {
            player.IsMoving = false;
            player.CanMove = true;
            player.StateChangePublic(player.IdleRigidbodyState);    
            return;
        }
        if (player.IsAttacking)
        {
            player.CharacterRb.linearVelocity = Vector3.zero;
            player.IsMoving = false;
            player.CanMove = false;
            player.StateChangePublic(player.StartAttackState);
            //player.StateChangePublic(player.IdleRigidbodyState);
            return;
        }
    }

    public void ExitState(Player player)
    {
        player.CharacterAnimator.ResetTrigger(player.Move);
    }
}
