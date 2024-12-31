using UnityEngine;

public class IJumpStart : IRigidbodyState
{
    private IRigidbodyState _jumping;
    public void EnterState(Player player)
    {
        if (_jumping == null)
        {
            _jumping = new IJumping();
        }
        //Debug.Log("start jump");
    }

    public void FixedExecute(Player player)
    {
        player.CharacterRb.AddForce(player.CharacterRb.transform.up * (player.JumpForce), ForceMode2D.Impulse);
        player.StateChangePublic(_jumping);
    }

    public void Execute(Player player)
    {
        
    }

    public void ExitState(Player player)
    {
        
    }
}
