using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public Rigidbody2D CharacterRb { get; private set; }
    [SerializeField] private GameObject character;
    [SerializeField] private float movementSpeed;
    
    public Animator CharacterAnimator { get; private set; }
    public float MovementSpeed => movementSpeed;
    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }
    public Vector2 Direction { get; private set; }
    
    [SerializeField] private float jumpForce;
    public float JumpForce => jumpForce;

    public bool IsGrounded { get; set; }

    public IRigidbodyState CurrentRigidbodyState { get; private set; }
    
    public IdleRigidbodyState IdleRigidbodyState { get; private set; }
    public MoveRigidbodyState MoveRigidbodyState { get; private set; }
    public bool CanMove { get; set; }
    public bool IsMoving { get; set; }
    public bool CanAttack { get; set; }
    public bool IsAttacking { get; set; }
    public JumpStartState JumpStartState { get; private set; }
    public FallingState FallingState { get; private set; }
    public LandingState LandingState { get; private set; }
    public JumpCancelState JumpCancelState { get; private set; }
    public bool IsAirBorne { get; set; }
    public RollingState RollingState { get; private set; }
    public StartAttackState StartAttackState {get; private set;}
    public StartMiddleAttackState StartMiddleAttackState { get; private set; }
    public MiddleEndAttackState MiddleEndAttackState { get; private set; } 
    public EndAttackState EndAttackState { get; private set; } 
    public InHighestAttackState InHighestAttackState { get; private set; }
    public FallingAttack FallingAttack { get; private set; }
    private int _groundLayerMask;
    
    public readonly int Idle = Animator.StringToHash("Idle");
    public readonly int Move = Animator.StringToHash("Move");
    public readonly int Jumping = Animator.StringToHash("Jumping");
    public readonly int Highest = Animator.StringToHash("Highest");
    public readonly int FallingAndCancel = Animator.StringToHash("FallingAndCancel");
    public readonly int Landing = Animator.StringToHash("Landing");
    public readonly int Rolling = Animator.StringToHash("Rolling");
    public readonly int StartAttack = Animator.StringToHash("StartAttack");
    public readonly int StartMiddleAttack = Animator.StringToHash("StartMiddleAttack");
    public readonly int MiddleEndAttack = Animator.StringToHash("MiddleEndAttack");
    public readonly int EndAttack = Animator.StringToHash("EndAttack");
    

    private void Awake()
    {
        IdleRigidbodyState = new IdleRigidbodyState();
        MoveRigidbodyState = new MoveRigidbodyState();
        JumpStartState = new JumpStartState();
        FallingState = new FallingState();
        JumpCancelState = new JumpCancelState();
        RollingState = new RollingState();
        LandingState = new LandingState();
        StartAttackState = new StartAttackState();
        StartMiddleAttackState = new StartMiddleAttackState();
        MiddleEndAttackState = new MiddleEndAttackState();
        EndAttackState = new EndAttackState();
        InHighestAttackState = new InHighestAttackState();
        FallingAttack = new FallingAttack();
        CharacterAnimator = character.GetComponent<Animator>();
        _groundLayerMask = LayerMask.GetMask("Ground");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CharacterRb = GetComponent<Rigidbody2D>();
        StateChange(IdleRigidbodyState);
        CanMove = true;
        CanAttack = true;
        IsGrounded = true;

    }

    public void StateChangePublic(IRigidbodyState state)
    {
        StateChange(state);
    }
    
    private void StateChange(IRigidbodyState state)
    {
        if (CurrentRigidbodyState == null)
        {
            CurrentRigidbodyState = state;
            CurrentRigidbodyState.EnterState(this);
            return;
        }
        CurrentRigidbodyState?.ExitState(this);
        CurrentRigidbodyState = state;
        Debug.Log(CurrentRigidbodyState.ToString());
        CurrentRigidbodyState.EnterState(this);
    }

    // Update is called once per frame
    void Update()
    {
        CurrentRigidbodyState?.Execute(this);
        //Debug.Log(CurrentRigidbodyState.ToString());
        
        if (Input.GetButton("Horizontal"))
        {
            //Debug.Log("getbuttondown");
            Horizontal = Input.GetAxis("Horizontal");
            if (Horizontal < 0)
            {
                character.transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                character.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            Direction = new Vector2(Horizontal, 0f).normalized;
            if (CanMove)
            {
                IsMoving = true;
            }
        }
        if (IsGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            IsAirBorne = true;
        }

        if (IsGrounded && Input.GetKeyDown(KeyCode.X))
        {
            if (CanAttack)
            {
                IsAttacking = true;
            }
        }

        if (Input.GetButtonUp("Horizontal"))
        {
            Horizontal = 0;
            Direction = Vector2.zero;
        }

        
    }
    void FixedUpdate()
    {
        CurrentRigidbodyState?.FixedExecute(this);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((1 << collision.gameObject.layer & _groundLayerMask) != 0)
        {
            IsAirBorne = false;
            IsGrounded = true;
        }
    }
}
