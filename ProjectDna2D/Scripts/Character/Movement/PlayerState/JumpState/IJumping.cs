using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class IJumping : IRigidbodyState
{
    private IRigidbodyState _highestState;
    private WaitForSeconds _jumpWait = new WaitForSeconds(0.2f);
    private bool EnterHighest;
    
    public void EnterState(Player player)
    {
        if (_highestState == null)
        {
            _highestState = new IJumpInHighest();
        }
        else
        {
            if (EnterHighest)
            {
                EnterHighest = false;
            }
        }
        
        //Debug.Log("IJumping");
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
            player.StateChangePublic(player.JumpCancel);
        }
    }

    public void ExitState(Player player)
    {
        EnterHighest = false;
    }
}
