using UnityEngine;

public class EnemyAirborneState : IEnemyRigidbodyState
{
    public Enemy Enemy { get; }
    public Animator EnemyAnimator { get; }
    private Rigidbody2D _enemyRb;
    private float _animLength;

    public EnemyAirborneState(Enemy enemy)
    {
        Enemy = enemy;
        EnemyAnimator = enemy.EnemyAnimator;
        _enemyRb = enemy.EnemyRb;
    }
    public void EnterState()
    {
        _enemyRb.gravityScale = 3f;
        EnemyAnimator.Play(Enemy.Airborne);
        _animLength = EnemyAnimator.GetCurrentAnimatorStateInfo(0).length;
    }

    public void FixedExecute()
    {
        
    }

    public void Execute()
    {
        if (_animLength > 0f)
        {
            _animLength -= Time.deltaTime;
            return;
        }
        Enemy.StateChangePublic(Enemy.ChaseState);
    }

    public void ExitState()
    {
        _enemyRb.gravityScale = 1f;
        _enemyRb.bodyType = RigidbodyType2D.Kinematic;
    }
}
