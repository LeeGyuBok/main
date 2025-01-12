using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    public Animator EnemyAnimator { get; private set; }
    public Rigidbody2D EnemyRb { get; private set; }

    [SerializeField] private List<Transform> patrolPoints;
    public List<Transform> PatrolPoints => patrolPoints;
    
    [SerializeField] private float moveSpeed;
    public float MoveSpeed => moveSpeed;

    [SerializeField] private float sightRange;
    public float SightRange => sightRange;
    [SerializeField] private float chaseSpeed;
    public float ChaseSpeed => chaseSpeed;
    
    [SerializeField] private float attackRange;
    public float AttackRange => attackRange;
    
    public GameObject PlayerObject { get; private set; }

    public void SetPlayer(GameObject playerObject)
    {
        PlayerObject = playerObject;
    }
    public int PlayerLayerMask { get; private set; }
    
    private IEnemyRigidbodyState _currentState;
    public PatrolState PatrolState { get; private set; }
    public TensePatrolState TensePatrolState { get; private set; }
    public ChaseState ChaseState { get; private set; }
    public AttackState AttackState { get; private set; }
    public HoldingState HoldingState { get; private set; }
    public EnemyHitState EnemyHitState { get; private set; }
    
    public EnemyAirborneState EnemyAirborneState { get; private set; }
    
    public HitBoxType HitBoxType { get; set; } = HitBoxType.None;
    
    public readonly int Patrol = Animator.StringToHash("Patrol");
    public readonly int TensePatrol = Animator.StringToHash("TensePatrol");
    public readonly int Holding = Animator.StringToHash("Holding");
    public readonly int Chase = Animator.StringToHash("Chase");
   
    public readonly int Stiff = Animator.StringToHash("Stiff");
    public readonly int Stun = Animator.StringToHash("Stun");
    public readonly int Airborne = Animator.StringToHash("Airborne");
    
    /*public readonly int High = Animator.StringToHash("High");
    public readonly int Middle = Animator.StringToHash("Middle");
    public readonly int Spin = Animator.StringToHash("Spin");
    public readonly int Straight = Animator.StringToHash("Straight");
    public readonly int Jab = Animator.StringToHash("Jab");
    public readonly int Upper = Animator.StringToHash("Upper");*/
    
    public readonly List<int> AttackMotions = new List<int>()
    {
        Animator.StringToHash("High"),
        Animator.StringToHash("Middle"),
        Animator.StringToHash("Spin"),
        Animator.StringToHash("Straight"),
        Animator.StringToHash("Jab"),
        Animator.StringToHash("Upper")
    };

    private void Awake()
    {
        EnemyRb = GetComponent<Rigidbody2D>();
        EnemyAnimator = enemyPrefab.GetComponent<Animator>();
        PlayerLayerMask = LayerMask.GetMask("Player");
        
        PatrolState = new PatrolState(this);
        TensePatrolState = new TensePatrolState(this);
        ChaseState = new ChaseState(this);
        AttackState = new AttackState(this);
        HoldingState = new HoldingState(this);
        EnemyHitState = new EnemyHitState(this);
        EnemyAirborneState = new EnemyAirborneState(this);
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.transform.position = new Vector2(patrolPoints[0].position.x, patrolPoints[0].position.y);
        
        StateChange(PatrolState);
    }
    
    public void StateChangePublic(IEnemyRigidbodyState state)
    {
        StateChange(state);
    }
    private void StateChange(IEnemyRigidbodyState state)
    {
        if (_currentState == null)
        {
            _currentState = state;
            _currentState.EnterState();
            return;
        }
        _currentState?.ExitState();
        _currentState = state;
        _currentState.EnterState();
        //Debug.Log(_currentState);
    }

    // Update is called once per frame
    void Update()
    {
        _currentState.Execute();
    }

    private void FixedUpdate()
    {
        _currentState?.FixedExecute();
    }
}
