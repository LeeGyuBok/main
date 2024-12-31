using UnityEngine;

public class IJumpInHighest: IRigidbodyState
{
    private IRigidbodyState _fallingState;
    public void EnterState(Player player)
    {
        if (_fallingState == null)
        {
            _fallingState = new IFalling();
        }
        //Debug.Log("IJumpInHighest EnterState");
    }

    public void FixedExecute(Player player)
    {
        
    }

    public void Execute(Player player)
    {
        if (player.CharacterRb.linearVelocity.magnitude >= 0.3f)
        {
            player.StateChangePublic(_fallingState);
            return;
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
        
    }
}
