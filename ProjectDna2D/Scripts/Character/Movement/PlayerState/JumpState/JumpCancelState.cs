using UnityEngine;
using System.Collections;

public class JumpCancelState : IRigidbodyState
{
    private IEnumerator resetCoroutine;
    public static bool RollingCommand { get; set; }
    public void EnterState(Player player)
    {
        RollingCommand = false;
        player.CharacterRb.gravityScale = 40f;
        resetCoroutine = ResetState(player);
        player.CharacterAnimator.SetTrigger(player.FallingAndCancel);
        player.StartCoroutine(resetCoroutine);
    }

    public void FixedExecute(Player player)
    {
        
    }

    public void Execute(Player player)
    {
        if (!player.IsAirBorne || player.IsGrounded)
        {
            player.StateChangePublic(player.LandingState);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            RollingCommand = !RollingCommand;
        }
    }

    public void ExitState(Player player)
    {
        player.CharacterRb.gravityScale = 1f;
    }
    
    private IEnumerator ResetState(Player player)
    {
        float time = 0;
        while (time < 5f)
        {
            time += Time.deltaTime;
            if (time > 5f)
            {
                if (player.CurrentRigidbodyState.Equals(this))
                {
                    Debug.Log("reset");
                    player.CharacterRb.linearVelocity = Vector2.zero;
                    player.IsAirBorne = false;
                    player.CharacterAnimator.ResetTrigger(player.FallingAndCancel);
                    player.CharacterAnimator.ResetTrigger(player.Idle);
                    player.StateChangePublic(player.IdleRigidbodyState);
                    player.gameObject.transform.position = new Vector3(0f, 0.65f, 0f);
                    yield break;
                }
            }
            yield return null;
        }
    }
}
