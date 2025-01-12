using UnityEngine;

public class ChaseState : IEnemyRigidbodyState
{
    public Enemy Enemy { get; private set; }
    public Animator EnemyAnimator { get; private set; }
    private Rigidbody2D _enemyRigidbody;
    private float _chaseSpeed;
    private float _attackRange;
    private int _playerLayerMask;
    private Vector2 _movement;
    
    private GameObject _targetedPlayer;
    private Vector2 _targetPlayerPosition;
    
    private Vector2 _sightDirection;
    public Vector2 SightDirection => _sightDirection;
    

    public ChaseState(Enemy enemy)
    {
        Enemy = enemy;
        EnemyAnimator = enemy.EnemyAnimator;
        _enemyRigidbody = enemy.EnemyRb;
        _chaseSpeed = enemy.ChaseSpeed;
        _attackRange = enemy.AttackRange;
        _playerLayerMask = enemy.PlayerLayerMask;
    }
    public void EnterState()
    {
        _targetedPlayer = Enemy.PlayerObject;
        if (!_targetedPlayer)
        {
            _targetedPlayer = GameObject.FindGameObjectWithTag("Player");
        }
        //벡터투는 자동변환을 믿지말자..
        _targetPlayerPosition = new Vector2(_targetedPlayer.transform.position.x, _targetedPlayer.transform.position.y);
        EnemyAnimator.SetTrigger(Enemy.Chase);
    }

    public void FixedExecute()
    {
        if (!(_targetPlayerPosition.y <= _enemyRigidbody.position.y + 2f)) return;
        RaycastHit2D hit = Physics2D.Raycast(_enemyRigidbody.position, _sightDirection, _attackRange, _playerLayerMask);
        if (hit) return;
        _movement = new Vector2(Vector2.MoveTowards(_enemyRigidbody.position, _targetPlayerPosition, _chaseSpeed * Time.fixedDeltaTime).x, 0f);
        _enemyRigidbody.MovePosition(_movement);


    }

    public void Execute()
    {
        //지속적인 적 위치 업데이트. 이때 타겟플레이아포지션이 0, -2~~ 이네? <- 벡터투는 자동변환을 믿지말자..
        _targetPlayerPosition = new Vector2(_targetedPlayer.transform.position.x, _targetedPlayer.transform.position.y);
        _sightDirection = _targetPlayerPosition - Enemy.EnemyRb.position;
        _sightDirection.y = 0f;
        Enemy.gameObject.transform.rotation = Quaternion.Euler(0, _sightDirection.x < 0 ? 180 : 0, 0);
        _sightDirection.Normalize();
        Debug.DrawRay(_enemyRigidbody.position, _sightDirection * _attackRange, Color.black);
        RaycastHit2D hit = Physics2D.Raycast(_enemyRigidbody.position, _sightDirection, _attackRange, _playerLayerMask);
        if (hit)
        {
            //Debug.Log(hit.collider.gameObject.name);
            Enemy.StateChangePublic(Enemy.AttackState);
            return;
        }
    }

    public void ExitState()
    {
        EnemyAnimator.ResetTrigger(Enemy.Chase);
    }
}
