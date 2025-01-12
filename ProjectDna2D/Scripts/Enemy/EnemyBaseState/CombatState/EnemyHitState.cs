using UnityEngine;

public class EnemyHitState : IEnemyRigidbodyState
{
    public Enemy Enemy { get; }
    public Animator EnemyAnimator { get; }
    private Rigidbody2D _enemyRb;
    private HitBoxType _hitBoxType;
    private float _animLength;

    public EnemyHitState(Enemy enemy)
    {
        Enemy = enemy;
        EnemyAnimator = enemy.EnemyAnimator;
        _enemyRb = enemy.EnemyRb;
    }
    public void EnterState()
    {
        _enemyRb.bodyType = RigidbodyType2D.Dynamic;
        _hitBoxType = Enemy.HitBoxType;
        switch (_hitBoxType)
        {
            case HitBoxType.Stiff:
                EnemyAnimator.Play(Enemy.Stiff);
                break;
            case HitBoxType.Stun:
                EnemyAnimator.Play(Enemy.Stun);
                break;
            case HitBoxType.Airborne:
                Enemy.StateChangePublic(Enemy.EnemyAirborneState);
                break;
        }

        _animLength = EnemyAnimator.GetCurrentAnimatorStateInfo(0).length;
    }

    public void FixedExecute()
    {
        
    }

    public void Execute()
    {
        if (EnemyAnimator.IsInTransition(0))
        {
            return;
        }

        if (_animLength > 0f)
        {
            _animLength -= Time.deltaTime;
            return;
        }
        Enemy.StateChangePublic(Enemy.ChaseState);
    }

    public void ExitState()
    {
        _enemyRb.bodyType = RigidbodyType2D.Kinematic;
        //Enemy.HitBoxType = HitBoxType.None;
    }
}
