using System.Collections;
using UnityEngine;

public class FallingState: IRigidbodyState
{
    private IEnumerator resetCoroutine;
    public static bool RollingCommand { get; set; }
    public Animator PlayerAnimator { get; private set; }

    public void EnterState(Player player)
    {
        RollingCommand = false;
        player.CharacterRb.gravityScale = 5f;
        //Debug.Log("falling");
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
        if (/*player.CharacterRb.linearVelocity.magnitude < 0.01f ||*/ !player.IsAirBorne)
        {
            player.StateChangePublic(player.LandingState);
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.StateChangePublic(player.JumpCancelState);
            return;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            RollingCommand = !RollingCommand;
            return;
        }
        if (player.AttachWall && Input.GetKeyDown(KeyCode.C))
        {
            player.StateChangePublic(player.WallClimbIdleState);
            return;
        }
    }

    public void ExitState(Player player)
    {
        PlayerAnimator.ResetTrigger(player.FallingAndCancel);
        player.CharacterRb.gravityScale = 1f;
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