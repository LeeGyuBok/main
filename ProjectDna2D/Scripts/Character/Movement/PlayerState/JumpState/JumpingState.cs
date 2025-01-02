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
    
    public void EnterState(Player player)
    {
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
        player.CharacterAnimator.SetTrigger(player.Jumping);
        //Debug.Log("JumpingState");
    }

    public void FixedExecute(Player player)
    {

    }

    public void Execute(Player player)
    {
        //Debug.Log(player.CharacterRb.linearVelocity.magnitude);
        if (Mathf.Abs(player.CharacterRb.linearVelocityY) < 0.2f)
        {
            if (!EnterHighest)
            {
                player.StateChangePublic(_highestState);
                EnterHighest = true;
            }
        }
        if (!player.IsAirBorne)
        {
            player.StateChangePublic(player.IdleRigidbodyState);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.StateChangePublic(player.JumpCancelState);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            AirAttackCommand = true;
        }

        if (Input.GetButton("Vertical"))
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                FallingAttackCommand = true;
                AirAttackCommand = false;
            }
        }
    }

    public void ExitState(Player player)
    {
        EnterHighest = false;
    }
}
