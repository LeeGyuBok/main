using UnityEngine;

public class PlayerAirborneState : IRigidbodyState
{
    public Animator PlayerAnimator { get; private set; }
    private Rigidbody2D _playerRigidbody;
    private Vector2 _direction;

    private float _timer = 0f;
    private float _possibleRolling;
    
    public void EnterState(Player player)
    {
        
        player.CanAct(false);
        if (!PlayerAnimator)
        {
            PlayerAnimator = player.CharacterAnimator;
        }
        if (!_playerRigidbody)
        {
            _playerRigidbody = player.CharacterRb;
        }
        _playerRigidbody.gravityScale = 3f;
        
        PlayerAnimator.Play(player.Airborne, 0, 0f);
        PlayerAnimator.Update(0f);//애니메이션을 0초 지난 시점에서부터 재생한다.
        _timer = PlayerAnimator.GetCurrentAnimatorStateInfo(0).length;
        Debug.Log(PlayerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);//현재 애니메이션이 0%만큼 지났다.
        _possibleRolling = 0.3f * _timer;
    }

    public void FixedExecute(Player player)
    {
        
    }

    public void Execute(Player player)
    {
        if (_timer >= 0f)
        {
            _timer -= Time.deltaTime;    
        }

        if (_timer <= _possibleRolling)
        {
            //Debug.Log(_timer);
            if (Input.GetKeyDown(KeyCode.C))
            {
                //롤링!
                _direction = player.HorizontalDirection;
                if (_direction == Vector2.zero)
                {
                    return;
                }
                player.CharacterRb.AddForce(15f * _direction, ForceMode2D.Impulse);
                player.StateChangePublic(player.RollingState);
                return;    
            }
        }

        if (PlayerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            player.StateChangePublic(player.IdleRigidbodyState);
            return;
        }
        
        
        /*if (PlayerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                //롤링!
                _direction = player.HorizontalDirection;
                if (_direction == Vector2.zero)
                {
                    return;
                }
                player.CharacterRb.AddForce(15f * _direction, ForceMode2D.Impulse);
                player.StateChangePublic(player.RollingState);
                return;    
            }
            
        }
        if (PlayerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f)
        {
            //Debug.Log(PlayerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            return;
        }
           
        player.StateChangePublic(player.IdleRigidbodyState);*/
    }

    public void ExitState(Player player)
    {
        _timer = 0f;
        _playerRigidbody.gravityScale = 1f;
        player.HitBoxType = HitBoxType.None;
    }
}
