using System.Collections;
using UnityEngine;

public class IFalling: IRigidbodyState
{
    private IEnumerator resetCoroutine;
    public void EnterState(Player player)
    {
        player.CharacterRb.gravityScale = 3f;
        //Debug.Log("falling");
        resetCoroutine = ResetState(player);
        player.StartCoroutine(resetCoroutine);
    }

    public void FixedExecute(Player player)
    {
        
    }

    public void Execute(Player player)
    {
        if (player.CharacterRb.linearVelocity.magnitude < 0.05f || !player.IsAirBorne)
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
        player.CharacterRb.gravityScale = 1f;
        player.StopCoroutine(resetCoroutine);
        resetCoroutine = null;
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
                    player.StateChangePublic(player.IdleRigidbodyState);
                    player.gameObject.transform.position = new Vector3(0f, 0.65f, 0f);
                    yield break;
                }
            }
            yield return null;
        }
    }
}