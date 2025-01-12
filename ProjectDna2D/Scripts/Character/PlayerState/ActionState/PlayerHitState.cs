using UnityEngine;

public class PlayerHitState : IRigidbodyState
{
    public Animator PlayerAnimator { get; private set; }
    private Rigidbody2D _playerRigidbody;
    private HitBoxType _hitBoxType;
    
    private float _animLength;
    
    public void EnterState(Player player)
    {
        if (!PlayerAnimator)
        {
            PlayerAnimator = player.CharacterAnimator;    
        }

        if (!_playerRigidbody)
        {
            _playerRigidbody = player.CharacterRb;
        }
        

        player.CanAct(false);
        _hitBoxType = player.HitBoxType;
        switch (_hitBoxType)
        {
            case HitBoxType.Stiff:
                PlayerAnimator.Play(player.Stiff);
                break;
            case HitBoxType.Stun:
                PlayerAnimator.Play(player.Stun);
                break;
            case HitBoxType.Airborne:
                player.StateChangePublic(player.PlayerAirborneState);
                break;
        }

        _animLength = PlayerAnimator.GetCurrentAnimatorStateInfo(0).length;
    }

    public void FixedExecute(Player player)
    {
        
    }

    public void Execute(Player player)
    {
        /*if (PlayerAnimator.IsInTransition(0))
        {
            return;
        }*/

        if (_animLength > 0f)
        {
            _animLength -= Time.deltaTime;
            return;
        }
        player.StateChangePublic(player.IdleRigidbodyState);
    }

    public void ExitState(Player player)
    {
        if (player.HitBoxType != HitBoxType.Airborne)
        {
            player.HitBoxType = HitBoxType.None;
            player.CanAct();
        }
    }
}
