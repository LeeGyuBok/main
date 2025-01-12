using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class JumpingState : IRigidbodyState
{
    private IRigidbodyState _highestState;
    private WaitForSeconds _jumpWait = new WaitForSeconds(0.2f);
    private bool EnterHighest;
    public static bool AirAttackCommand { get; set; }
    public static bool FallingAttackCommand { get; set; }

    public Animator PlayerAnimator { get; private set; }

    public void EnterState(Player player)
    {
        if (!PlayerAnimator)
        {
            PlayerAnimator = player.CharacterAnimator;
        }
        if (_highestState == null)
        {
            _highestState = new JumpInHighestState();
        }
        if (EnterHighest)
        {
            EnterHighest = false;
        }

        if (AirAttackCommand)
        {
            AirAttackCommand = false;
        }

        if (FallingAttackCommand)
        {
            FallingAttackCommand = false;
        }
        PlayerAnimator.SetTrigger(player.Jumping);
        //Debug.Log("JumpingState");
    }

    public void FixedExecute(Player player)
    {

    }

    public void Execute(Player player)
    {
        //Debug.Log(player.CharacterRb.linearVelocity.magnitude);
        if (player.HitBoxType != HitBoxType.Airborne && Mathf.Abs(player.CharacterRb.linearVelocityY) < 0.2f)
        {
            if (!EnterHighest)
            {
                player.StateChangePublic(_highestState);
                EnterHighest = true;
                return;
            }
        }
        /*if (!player.IsAirBorne)//여기서 걸리네 왜지?
        {
            player.StateChangePublic(player.IdleRigidbodyState);
            return;
        }*/

        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.StateChangePublic(player.JumpCancelState);
            return;
        }

        /*if (Input.GetKeyDown(KeyCode.X))
        {
            AirAttackCommand = true;
            return;
        }*/

        if (player.Vertical <= 0f)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                FallingAttackCommand = true;
                //AirAttackCommand = false;
                return;
            }
        }
        
        if (player.AttachWall && Input.GetKeyDown(KeyCode.C))
        {
            player.StateChangePublic(player.WallClimbIdleState);
            return;
        }
    }

    public void ExitState(Player player)
    {
        EnterHighest = false;
    }
}
