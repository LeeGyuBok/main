using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PatrolState : IEnemyRigidbodyState
{
    public Enemy Enemy { get; private set; }
    public Animator EnemyAnimator { get; private set; }
    private float _patrolSpeed;
    private float _patrolSight;
    private List<Transform> _patrolPoints;
    private Rigidbody2D _enemyRigidbody;
    private int _currentPointIndex;
    public static (int, Vector3) ContinuePoint { get; private set; }
    private Vector3 _currentPoint;

    private Vector3 _movement;
    
    private int _playerLayerMask;
    
    private Vector2 _sightDirection;
    public Vector2 SightDirection => _sightDirection;

    public PatrolState(Enemy enemy)
    {
        Enemy = enemy;
        EnemyAnimator = enemy.EnemyAnimator;
        _patrolSpeed = enemy.MoveSpeed;
        _patrolSight = enemy.SightRange;
        _patrolPoints = new List<Transform>(enemy.PatrolPoints);
        _currentPointIndex = 0;
        _currentPoint = _patrolPoints[_currentPointIndex].position;
        _enemyRigidbody = enemy.EnemyRb;
        _playerLayerMask = enemy.PlayerLayerMask;
    }
    public void EnterState()
    {
        //리스트 내용물 뒤집기
        //_patrolPoints.Reverse(); 
        EnemyAnimator.SetTrigger(Enemy.Patrol);
        SetNextPatrolPoint();
    }

    public void FixedExecute()
    {
        _movement = new Vector2(Vector2.MoveTowards(_enemyRigidbody.position, _currentPoint, _patrolSpeed * Time.fixedDeltaTime).x, 0f);
        _enemyRigidbody.MovePosition(_movement);    
    }

    public void Execute()
    {
        Debug.DrawRay(_enemyRigidbody.position, _sightDirection * _patrolSight, Color.red);
        //Debug.Log(_sightDirection);//방향 잘 가다가
        RaycastHit2D hit = Physics2D.Raycast(_enemyRigidbody.position, _sightDirection, _patrolSight, _playerLayerMask);
        if (hit)
        {
            //Debug.Log(hit.collider.gameObject.name);
            Enemy.SetPlayer(hit.collider.gameObject);
            Enemy.StateChangePublic(Enemy.ChaseState);
            return;
        }
        
        SetNextPatrolPoint();

        
        //이 부분은 특수한 조건 ex) 죽은 아군의 시체를 봤을 때
        /*if (_currentPointIndex == 0)
        {
            Enemy.StateChangePublic(Enemy.TensePatrolState);    
        }*/
        
 

        
        //1. 현재위치(rb)와 목표위치?를 비교한다. 현재위치를 비교해서 다음 위치를 설정해야함.
        //도달하지 않았으면 목표위치로 이동한다.
        //도달했으면 다음 위치가 있는지 확인한다.
        //다음 위치가 있으면 다음 위치를 목표위치로하고 이동한다.
        //다음위치가 없으면 패트롤 포인트를 뒤집어서 반복한다.
        
        
    
    }

    public void ExitState()
    {
        EnemyAnimator.ResetTrigger(Enemy.Patrol);
    }

    private void SetNextPatrolPoint()
    {
        float distance = Vector3.Distance(_enemyRigidbody.position, _currentPoint);
        if (distance <= 0.2f)
        {
            _currentPointIndex = (_currentPointIndex + 1) % _patrolPoints.Count; 
            _currentPoint = _patrolPoints[_currentPointIndex].position;
            ContinuePoint = (_currentPointIndex, _currentPoint);
            _sightDirection = ((Vector2)_patrolPoints[_currentPointIndex].position - _enemyRigidbody.position);
            _sightDirection.y = 0f;
            _sightDirection.Normalize();
            Enemy.gameObject.transform.rotation = Quaternion.Euler(0, _sightDirection.x < 0 ? 180 : 0, 0);
            Enemy.StateChangePublic(Enemy.HoldingState);    
        }
        /*if (_currentPointIndex == 0)
        {
            Enemy.StateChangePublic(Enemy.TensePatrolState);
            return;
        }*/
        

    }
}
