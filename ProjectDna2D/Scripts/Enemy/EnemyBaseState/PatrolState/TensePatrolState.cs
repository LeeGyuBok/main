using System.Collections.Generic;
using UnityEngine;

public class TensePatrolState : IEnemyRigidbodyState
{
    public Enemy Enemy { get; private set; }
    public Animator EnemyAnimator { get; private set; }
    private float _patrolSpeed;
    private float _patrolSight;
    private List<Transform> _patrolPoints;
    private Rigidbody2D _enemyRigidbody;
    private int _currentPointIndex;
    private Vector3 _currentPoint;

    private (int, Vector3) _continuePoint;
    
    private Vector3 _movement;
    
    private float _holdingTimer;
    private bool _isHolding;
    private int _playerLayerMask;

    private Vector2 _sightDirection;

    public TensePatrolState(Enemy enemy)
    {
        Enemy = enemy;
        EnemyAnimator = enemy.EnemyAnimator;
        _patrolSpeed = enemy.MoveSpeed * 0.7f;
        _patrolSight = enemy.SightRange*2f;
        _patrolPoints = new List<Transform>(enemy.PatrolPoints);
        
        _enemyRigidbody = enemy.EnemyRb;
        _continuePoint = PatrolState.ContinuePoint;
        _currentPointIndex = _continuePoint.Item1;
        _currentPoint = _continuePoint.Item2;
        _playerLayerMask = enemy.PlayerLayerMask;
    }
    public void EnterState()
    {
        EnemyAnimator.SetTrigger(Enemy.TensePatrol);
        _isHolding = false;
        //SetNextPatrolPoint();
        
        _sightDirection = ((Vector2)_currentPoint - _enemyRigidbody.position);
        _sightDirection.y = 0f;
        _sightDirection.Normalize();
    }

    public void FixedExecute()
    {
        if (!_isHolding)
        {
            _movement = new Vector2(Vector2.MoveTowards(_enemyRigidbody.position, _currentPoint, _patrolSpeed * Time.fixedDeltaTime).x, 0f);
            _enemyRigidbody.MovePosition(_movement);    
            //Debug.Log(_enemyRigidbody.position - (Vector2)_patrolPoints[_currentPointIndex].position);
            
        }
    }

    public void Execute()
    {
        Debug.DrawRay(_enemyRigidbody.position, _sightDirection * _patrolSight, Color.red);
        //Debug.Log(_sightDirection);
        RaycastHit2D hit = Physics2D.Raycast(_enemyRigidbody.position, _sightDirection, _patrolSight, _playerLayerMask);
        if (hit)
        {
            //Debug.Log(hit.collider.gameObject.name);
            Enemy.SetPlayer(hit.collider.gameObject);
            Enemy.StateChangePublic(Enemy.ChaseState);
            return;
        }
        
        SetNextPatrolPoint();
        
        if (_isHolding)
        {
            if (_holdingTimer > 0.0f)
            {
                _holdingTimer -= Time.deltaTime;
                return;
            }
            _isHolding = false;
            return;
        }
     
 
        
        
        /*if (_currentPointIndex == 0)
        {
            Enemy.StateChangePublic(Enemy.PatrolState);
            return;
        }*/
        
    
    }

    public void ExitState()
    {
        EnemyAnimator.ResetTrigger(Enemy.TensePatrol);
    }
    
    private void SetNextPatrolPoint()
    {
        float distance = Vector3.Distance(_enemyRigidbody.position, _currentPoint);
        if (distance <= 0.2f)
        {
            _currentPointIndex = (_currentPointIndex + 1) % _patrolPoints.Count; 
            _currentPoint = _patrolPoints[_currentPointIndex].position;
            _holdingTimer = Random.Range(2, 5);
            _isHolding = true;
            _sightDirection = ((Vector2)_patrolPoints[_currentPointIndex].position - _enemyRigidbody.position);
            _sightDirection.y = 0f;
            _sightDirection.Normalize();
            Enemy.gameObject.transform.rotation = Quaternion.Euler(0, _sightDirection.x < 0 ? 180 : 0, 0);
        }
    }
}
