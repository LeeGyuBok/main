using UnityEngine;

public class HoldingState : IEnemyRigidbodyState
{
    public Enemy Enemy { get; }
    public Animator EnemyAnimator { get; }
    private Rigidbody2D _enemyRigidbody;

    private Vector2 _sightDirection;
    private float _patrolSight;
    private int _playerLayerMask;
    private float _holdingTimer;

    public HoldingState(Enemy enemy)
    {
        Enemy = enemy;
        EnemyAnimator = enemy.EnemyAnimator;
        _enemyRigidbody = enemy.EnemyRb;
        _playerLayerMask = enemy.PlayerLayerMask;
        _patrolSight = enemy.SightRange;
    }
    public void EnterState()
    {
        _enemyRigidbody.linearVelocity = Vector2.zero;
        _sightDirection = Enemy.PatrolState.SightDirection;
        int randomRepeat = Random.Range(1, 4);
        _holdingTimer = EnemyAnimator.GetCurrentAnimatorStateInfo(0).length * randomRepeat;
        EnemyAnimator.SetTrigger(Enemy.Holding);
    }

    public void FixedExecute()
    {
        _enemyRigidbody.linearVelocity = Vector2.zero;
    }

    public void Execute()
    {
        Debug.DrawRay(_enemyRigidbody.position, _sightDirection * _patrolSight, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(_enemyRigidbody.position, _sightDirection, _patrolSight, _playerLayerMask);
        if (hit)
        {
            //Debug.Log(hit.collider.gameObject.name);
            Enemy.SetPlayer(hit.collider.gameObject);
            Enemy.StateChangePublic(Enemy.ChaseState);
            return;
        }
        if (_holdingTimer > 0.0f)
        {
            _holdingTimer -= Time.deltaTime;
            return;
        }
        Enemy.StateChangePublic(Enemy.PatrolState);
    }

    public void ExitState()
    {
        
    }
}