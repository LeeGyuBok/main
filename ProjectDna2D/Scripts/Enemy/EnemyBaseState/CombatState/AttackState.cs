using UnityEngine;

public class AttackState : IEnemyRigidbodyState
{
    public Enemy Enemy { get; }
    public Animator EnemyAnimator { get; }
    private Rigidbody2D _enemyRigidbody;
    private int _selectedAttackStringHash;

    private float _motionTimer;

    public AttackState(Enemy enemy)
    {
        Enemy = enemy;
        EnemyAnimator = enemy.EnemyAnimator;
        _enemyRigidbody = enemy.EnemyRb;

    }
    public void EnterState()
    {
        //이동멈춤
        _enemyRigidbody.linearVelocity = Vector2.zero;
        _enemyRigidbody.MovePosition(_enemyRigidbody.position);

        _selectedAttackStringHash = Enemy.AttackMotions[Random.Range(0, Enemy.AttackMotions.Count)];
        EnemyAnimator.SetTrigger(_selectedAttackStringHash);
        _motionTimer = EnemyAnimator.GetCurrentAnimatorStateInfo(0).length * 1.6f;
    }

    public void FixedExecute()
    {
        _enemyRigidbody.linearVelocity = Vector2.zero;
        _enemyRigidbody.MovePosition(_enemyRigidbody.position);
    }

    public void Execute()
    {
        //공격하기. 공격이 끝나면 다시 ChaseState. 테스트타이머는 공격애니메이션의 길이
        _motionTimer -= Time.deltaTime;
        if (_motionTimer < 0f)
        {
            Enemy.StateChangePublic(Enemy.ChaseState);
        }
        
    }

    public void ExitState()
    {
        EnemyAnimator.ResetTrigger(_selectedAttackStringHash);
        _selectedAttackStringHash = 0;
        _motionTimer = 0f;
    }
}
