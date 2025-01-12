using UnityEngine;
using System.Collections;

public class JumpCancelState : IRigidbodyState
{
    private IEnumerator resetCoroutine;
    public static bool RollingCommand { get; set; }
    public Animator PlayerAnimator { get; private set; }

    public void EnterState(Player player)
    {
        RollingCommand = false;
        player.CharacterRb.gravityScale = 40f;
        resetCoroutine = ResetState(player);
        if (!PlayerAnimator)
        {
            PlayerAnimator = player.CharacterAnimator;
        }
        PlayerAnimator.SetTrigger(player.FallingAndCancel);
        player.StartCoroutine(resetCoroutine);
    }

    public void FixedExecute(Player player)
    {
        
    }

    public void Execute(Player player)
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            RollingCommand = !RollingCommand;
        }
        if (!player.IsAirBorne)
        {
            player.StateChangePublic(player.LandingState);
            return;
        }
   
    }

    public void ExitState(Player player)
    {
        player.CharacterRb.gravityScale = 1f;
        PlayerAnimator.ResetTrigger(player.FallingAndCancel);
        player.StopCoroutine(resetCoroutine);
        resetCoroutine = null;
        player.CharacterRb.linearVelocity = Vector2.zero;
    }
    
    private IEnumerator ResetState(Player player)
    {
        float time = 0;
        while (time < 5f)
        {
            time += Time.deltaTime;
            if (player.OnWall || player.AttachWall)
            {
                yield break;
            }
            if (time > 5f)
            {
                if (player.CurrentRigidbodyState.Equals(this))
                {
                    Debug.Log("reset");
                    player.CharacterRb.linearVelocity = Vector2.zero;
                    player.IsAirBorne = false;
                    PlayerAnimator.ResetTrigger(player.FallingAndCancel);
                    PlayerAnimator.ResetTrigger(player.Idle);
                    player.StateChangePublic(player.IdleRigidbodyState);
                    player.gameObject.transform.position = new Vector3(0f, 0.65f, 0f);
                    yield break;
                }
            }
            yield return null;
        }
    }
}
